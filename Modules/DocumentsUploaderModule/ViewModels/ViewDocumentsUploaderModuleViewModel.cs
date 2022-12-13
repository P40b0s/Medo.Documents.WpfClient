using Medo.Core.Collections;
using Medo.Core.EventsAggregator;
using Medo.Core.Models;
using NLog;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using Prism.Commands;
using System.ServiceModel;
using System.Threading;
using Medo.Modules.DocumentsUploaderModule.IzdanieIntegrationService;
using Prism.Interactivity.InteractionRequest;
using System.Windows.Threading;
using Medo.Core.Interfaces;
using XmlCardCreator;

namespace Medo.Modules.DocumentsUploaderModule.ViewModels
{
    /// Не реализован функционал FlashSelector выбор флешки, на которую будут копироваться заменненые файлы
    class ViewDocumentsUploaderModuleViewModel : IzdanieUploader
    {
        IEventAggregator _eventAggregator { get; set; }
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// При установке флага true файлы минюста будут копироваться в директорию НТЦ Система
        /// </summary>
        private const bool copyToSystemaFlag = true;
        /// <summary>
        /// При установке флага true файлы после выгрузки в директорию MEDO системы издание начнут обрабатываться сервисом Издания
        /// </summary>
        private const bool uploadToIzdanieFlag = true;
        private const string DirectoryForChangedDocuments = "!!!Документы на замену";
        private delegate void refDocumentsCopy(ref DocumentsCopy data, string message);
        private bool DocumentIsChangeMode { get; set; }
        private Dispatcher UIDispatcher = Dispatcher.CurrentDispatcher;
        public ViewDocumentsUploaderModuleViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SubscribeEvents();
            CopyngList = new AsyncObservableCollection<DocumentsCopy>();
        }

        private void SubscribeEvents()
        {
            _eventAggregator.GetEvent<UploadSelectedDocumentsEvent>().Subscribe(UploadSelectedDocuments);
            _eventAggregator.GetEvent<ChangeDocumentEvent>().Subscribe(ChangeDocument);
        }
      
        /// <summary>
        /// Снимаем выделение с выбранных документов, (если они успешно скопировались)
        /// </summary>
        /// <param name="header"></param>
        private void DeselectDoneDocuments(Guid header)
        {
            //var item = SourceList.Where(h => h.HeaderGuid == header).FirstOrDefault();
            //item.IsSelected = false;
            Document doc = Client.Collections.StaticCollections.MainCollection.AddOrUpdate(header);
            _eventAggregator.GetEvent<UpdateNonBaseStateEvent>().Publish(doc);
        }
        #region Notify members

        private ObservableCollection<DocumentsCopy> _CopyngList { get; set; }
        public ObservableCollection<DocumentsCopy> CopyngList
        {
            get
            {
                return this._CopyngList;
            }
            set
            {
                if (this.CopyngList != value)
                {
                    this._CopyngList = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private IEnumerable<Document> _SourceList { get; set; }
        public IEnumerable<Document> SourceList
        {
            get
            {
                return this._SourceList;
            }
            set
            {
                if (this.SourceList != value)
                {
                    this._SourceList = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private double _ProgressMaximum { get; set; }
        public double ProgressMaximum
        {
            get
            {
                return this._ProgressMaximum;
            }
            set
            {
                if (this.ProgressMaximum != value)
                {
                    Client.Collections.StaticCollections.DocumentsUploadProcessMaximum = value;
                    this._ProgressMaximum = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private double _CopyProgress { get; set; }
        public double CopyProgress
        {
            get
            {
                return this._CopyProgress;
            }
            set
            {
                if (this.CopyProgress != value)
                {
                    Client.Collections.StaticCollections.DocumentsUploadProcessValue = value;
                    this._CopyProgress = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private double _CopyProgressMaximumByte { get; set; }
        public double CopyProgressMaximumByte
        {
            get
            {
                return this._CopyProgressMaximumByte;
            }
            set
            {
                if (this.CopyProgressMaximumByte != value)
                {
                    this._CopyProgressMaximumByte = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private double _CopyProgressByte { get; set; }
        public double CopyProgressByte
        {
            get
            {
                return this._CopyProgressByte;
            }
            set
            {
                if (this.CopyProgressByte != value)
                {
                    this._CopyProgressByte = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private string _CopyngFileInfo { get; set; }
        public string CopyngFileInfo
        {
            get
            {
                return this._CopyngFileInfo;
            }
            set
            {
                if (this.CopyngFileInfo != value)
                {
                    this._CopyngFileInfo = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private bool _BinaryCopyIsActive { get; set; }
        public bool BinaryCopyIsActive
        {
            get
            {
                return this._BinaryCopyIsActive;
            }
            set
            {
                if (this.BinaryCopyIsActive != value)
                {
                    this._BinaryCopyIsActive = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private string modifedNumber { get; set; }
        private string modifedText { get; set; }
        #endregion
        /// <summary>
        /// Выгрузка отмеченых документов на сервер Издания в директорию МЭДО
        /// </summary>
        /// <param name="doc"></param>
        public async void UploadSelectedDocuments(List<Document> doc)
        {
            try
            {
                CopyngList.Clear();
                CopyProgress = 0;
                SourceList = doc;
                BinaryCopyIsActive = true;
                ProgressMaximum = doc.Count();
                TotalPages = doc.Sum(s => s.PagesCount);
                foreach (Document d in doc)
                {
                    if (d != null)
                    {
                        DocumentsCopy copy = getDocumentsCopy(d);
                        bool ok = await UploadDocuments(copy);
                        copy.CanAnimate = false;
                        CopyProgress = CopyngList.Where(c => c.IsCopyComplete).Count();
                    }
                }
                BinaryCopyIsActive = false;
                _eventAggregator.GetEvent<UploadOrChangeDoneEvent>().Publish();
             
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }

        }
        /// <summary>
        /// Создание карточки документа по предоставленному адресу
        /// </summary>
        /// <param name="documentDirectory">Путь к директории документа</param>
        /// <param name="destinationDirectory">Путь к директории места назначения создания карточки XML (используется только для системы)</param>
        /// <param name="data">Модель копирования</param>
        private void XmlCopy(DirectoryInfo documentDirectory, DirectoryInfo destinationDirectory, ref DocumentsCopy data)
        {
            try
            {
                CopyngFileInfo = "Создание XML карточки...";
                CreateDocument xml = new CreateDocument(
                                   Path.Combine(destinationDirectory.FullName, "document.xml"),
                                   data.HeaderGuid,
                                   data.SourceGuid,
                                   data.DocGuid,
                                   data.OrganName,
                                   data.SignPerson,
                                   data.ActType,
                                   data.SignDate,
                                   data.modifedNumber,
                                   data.PagesCount,
                                   data.modifedText,
                                   data.DefaultPdf,
                                   data.MJNumber,
                                   data.MJDate);
                CopyngFileInfo = xml.fileInfo.Name;
                //Копирование XML в НТЦ систему.
                if (copyToSystemaFlag)
                    XmlCopyToSystemDirectory(ref data, xml.fileInfo, documentDirectory);
                data.XmlIsCopy = true;
                UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, "Xml карточка создана по пути " + xml.fileInfo.FullName);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                data.XmlIsCopy = false;
                UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, ex.Message);
            }

        }

        private void insertcopymessage(ref DocumentsCopy data, string message)
        {
            string mes = string.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss"), message);
            data.CopyMessage.Insert(0, mes);
        }

        private void PdfCopy(DirectoryInfo documentDirectory, DirectoryInfo destinationDirectory, ref DocumentsCopy data)
        {
            try
            {
                data.PdfIsCopy = false;
                if (!string.IsNullOrEmpty(data.DefaultPdf))
                {
                    FileInfo pdf = new FileInfo(Path.Combine(documentDirectory.FullName, data.DefaultPdf));
                    if (pdf != null)
                    {
                        CopyngFileInfo = string.Format("Файл: {0}, Размер: {1} Мб", pdf.Name, (pdf.Length / 1024) / 1024);
                        Copy(pdf.FullName, Path.Combine(destinationDirectory.FullName, pdf.Name));
                        if (DocumentIsChangeMode)
                        {
                            Copy(pdf.FullName, Path.Combine(destinationDirectory.FullName, pdf.Name + ".sig"));
                            UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, "Файл .sig успешно создан");
                        }
                        //выгрузка PDF в директорию НТЦ Система
                        if (copyToSystemaFlag)
                            PdfCopyToSystemDirectory(ref data, pdf);
                        data.PdfIsCopy = true;
                        UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, "Pdf файл успешно скопирован в директорию " + Path.Combine(destinationDirectory.FullName, pdf.Name));
                    }
                }
                else
                {
                    var pdf = documentDirectory.GetFiles("*.pdf", SearchOption.AllDirectories).FirstOrDefault();
                    if (pdf != null)
                    {
                        CopyngFileInfo = string.Format("Файл: {0}, Размер: {1} Мб", pdf.Name, (pdf.Length / 1024) / 1024);
                        Copy(pdf.FullName, Path.Combine(destinationDirectory.FullName, pdf.Name));
                        if (DocumentIsChangeMode)
                        {
                            Copy(pdf.FullName, Path.Combine(destinationDirectory.FullName, pdf.Name + ".sig"));
                            UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, "Файл .sig успешно создан");
                        }

                        //выгрузка PDF в директорию НТЦ Система
                        if (copyToSystemaFlag)
                            PdfCopyToSystemDirectory(ref data, pdf);
                        data.PdfIsCopy = true;
                        UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, "Pdf файл успешно скопирован в директорию " + Path.Combine(destinationDirectory.FullName, pdf.Name));
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                data.PdfIsCopy = false;
                UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, ex.Message);
            }
        }

        #region НТЦ СИСТЕМА
        /// <summary>
        /// Создаем XML в директории НТЦ Система
        /// </summary>
        /// <param name="data">Модель документа для выгрузки</param>
        /// <param name="xmlFile">XML файл сформированный для Издания</param>
        /// <param name="documentDirectory">Полный путь к директории документа</param>
        /// <returns></returns>
        private void XmlCopyToSystemDirectory(ref DocumentsCopy data, FileInfo xmlFile, DirectoryInfo documentDirectory)
        {
            try
            {
                if ((data.MJDate != null && data.MJDate != DateTime.MinValue))
                {
                    DirectoryInfo dir = null;
                    if (DocumentIsChangeMode)
                    {
                        dir = Directory.CreateDirectory(Path.Combine(Helpers.Paths.Systema, DirectoryForChangedDocuments, DateTime.Now.ToShortDateString(), data.DirectoryName));
                        //}
                        //else
                        //{
                        //    dir = Directory.CreateDirectory(Path.Combine(Helpers.Paths.Systema, DateTime.Now.ToShortDateString(), data.DirectoryName));
                        //}
                        if (xmlFile != null)
                        {
                            var xml = documentDirectory.GetFiles("*.xml", SearchOption.TopDirectoryOnly).FirstOrDefault();
                            //Новый файл в системе не поддерживается, пришлось оставить пока старый.
                            //File.Copy(doc.fileInfo.FullName, Path.Combine(dir.FullName, doc.fileInfo.Name));
                            //Старая функция, поддерживается только из-за выгрузки документов в систему.
                            WriteDataToXml(data.SignDate, data.ActType, xml, data.modifedNumber, data.DocumentText, data.OrganName, data.MJDate, data.MJNumber).Save(Path.Combine(dir.FullName, xmlFile.Name));
                            UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, "Xml карточка создана в директории " + Path.Combine(dir.FullName, xmlFile.Name));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, "Ошибка выгрузки XML карточки в директорию НТЦ Система: " + ex.Message);
            }
        }
        /// <summary>
        /// Выгрузка PDF в директорию НТЦ Система
        /// </summary>
        /// <param name="data">Модель документа для выгрузки</param>
        /// <param name="pdf">Файл PDF для выгрузки</param>
        /// <returns></returns>
        private void PdfCopyToSystemDirectory(ref DocumentsCopy data, FileInfo pdf)
        {
            try
            {
                if ((data.MJDate != null && data.MJDate != DateTime.MinValue))
                {
                    DirectoryInfo dir = null;
                    if (DocumentIsChangeMode)
                    {
                        dir = Directory.CreateDirectory(Path.Combine(Helpers.Paths.Systema, DirectoryForChangedDocuments, DateTime.Now.ToShortDateString(), data.DirectoryName));
                        //}
                        //else
                        //{
                        //    dir = Directory.CreateDirectory(Path.Combine(Helpers.Paths.Systema, DateTime.Now.ToShortDateString(), data.DirectoryName));
                        //}
                        Copy(pdf.FullName, Path.Combine(dir.FullName, pdf.Name));
                        UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, "Pdf файл успешно выгружен в директорию " + dir.FullName);
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, " Ошибка выгрузки PDF документа в директорию НТЦ Система: " + ex.Message);
            }
        }

        /// <summary>
        /// Создание XML файла для загрузки в Издание (Старый метод, используется для выгрузки документа в Систему)
        /// </summary>
        /// <obsolette>Устаревший</obsolette>
        /// <param name="SignDate">Дата подписания документа</param>
        /// <param name="acttype">Вид документа</param>
        /// <param name="XmlFile">Исходный XML файл который приходит с документом по системе МЭДО</param>
        /// <param name="num">Номер документа</param>
        /// <param name="text">Наименование документа</param>
        /// <param name="organ">Принявший орган</param>
        /// <param name="MJDate">(Опционально) Дата регистрации документа в Министерстве Юстиции</param>
        /// <param name="MJNumber">(Опционально) Номер регистрации документа в Министерстве Юстиции</param>
        /// <returns></returns>
        private XElement WriteDataToXml(DateTime? SignDate, string acttype, FileInfo XmlFile, string num, string text, string organ, DateTime? MJDate = null, string MJNumber = null)
        {
            try
            {
                XElement elem = XElement.Load(XmlFile.FullName);
                //Подключаем namespace xdms
                XNamespace xdms = "http://www.infpres.com/IEDMS";
                elem.Element(xdms + "header").Element(xdms + "source").Element(xdms + "organization").Value = organ;
                elem.Element(xdms + "document").Element(xdms + "kind").Value = acttype;
                elem.Element(xdms + "document").Element(xdms + "annotation").Value = string.IsNullOrEmpty(text) ? "" : text;
                elem.Element(xdms + "document").Element(xdms + "num").Element(xdms + "number").Value = string.IsNullOrEmpty(num) ? "" : num; ;
                elem.Element(xdms + "document").Element(xdms + "num").Element(xdms + "date").Value = SignDate.Value.ToString("yyyy-MM-dd");
                elem.Element(xdms + "document").Element(xdms + "signatories").Element(xdms + "signatory").Element(xdms + "signed").Value = SignDate.Value.ToString("yyyy-MM-dd");
                if (elem.Element(xdms + "document").Element(xdms + "mjregistration") == null && MJDate.HasValue && MJDate != DateTime.MinValue)
                {
                    elem.Element(xdms + "document").AddFirst(new XElement(xdms + "mjregistration",
                                                          new XElement(xdms + "number", MJNumber.Trim()),
                                                          new XElement(xdms + "date", MJDate.Value.ToString("dd.MM.yyyy"))));
                }

                return elem;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return new XElement("");
            }
        }
        #endregion

        /// <summary>
        /// Выгрузка выделенных документов в директорию МЭДО
        /// </summary>
        private async Task<bool> UploadDocuments(DocumentsCopy data)
        {
            CopyngList.Insert(0, data);
            try
            {
                return await Task<bool>.Factory.StartNew(() =>
                {
                    DirectoryInfo DocumentDirectory = new DirectoryInfo(System.IO.Path.Combine(Helpers.Paths.PakMedoFolder, data.DirectoryName));
                    DirectoryInfo DestDir = Directory.CreateDirectory(Path.Combine(Helpers.Paths.IzdanieMedoFolder, DocumentDirectory.Name));

                    XmlCopy(DocumentDirectory, DestDir, ref data);
                    PdfCopy(DocumentDirectory, DestDir, ref data);
                    if (uploadToIzdanieFlag)
                        UploadToIzdanie(ref data);
                    CopyngFileInfo = string.Empty;
                    if (!data.PdfIsCopy)
                        throw new Medo.Core.Exceptions.DocumentCopyErrorException(data, "Не найден PDF файл!");
                    if (!data.XmlIsCopy)
                        throw new Medo.Core.Exceptions.DocumentCopyErrorException(data, "Ошибка создания XML карточки!");
                    if (uploadToIzdanieFlag)
                        if (!data.UploadToIzdanieIsCompliete)
                            throw new Medo.Core.Exceptions.DocumentCopyErrorException(data, "Ошибка добавления файла в систему издание!");
                    IIzdanieInterface b = new IzdanieModel();
                    b.DocGuid = data.DocGuid;
                    b.HeaderGuid = data.HeaderGuid;
                    b.SourceGuid = data.SourceGuid;
                    b.Status = Core.Enums.DocumentStatus.INSERT;
                    _eventAggregator.GetEvent<GetIzdanieStatusUpdateEvent>().Publish(b);
                    data.IsCopyComplete = true;
                    DeselectDoneDocuments(data.HeaderGuid);
                    return true;
                });
            }
            catch (Core.Exceptions.DocumentCopyErrorException c)
            {
                await UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, "Ошибка копирования: " + c.errorMessage);
                data.IsCopyComplete = false;
                logger.Fatal(c);
                return false;
            }
            catch (System.Exception ex)
            {
                await UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), data, "Ошибка копирования: " + ex.Message);
                data.IsCopyComplete = false;
                logger.Fatal(ex);
                return false;
            }
        }

        private DocumentsCopy getDocumentsCopy(Document data)
        {
            DocumentsCopy copy = new DocumentsCopy();
            try
            {
                string number = data.ChangedNumber == null ? data.DocumentNumber : data.ChangedNumber;
                //Если принявший орган МИД, то номер документа нам ни к чему
                if (data.SourceGuid == new Guid(Helpers.SourceGuidOrgansNames.МИД))
                    number = null;
                // если data.DocumentText == null  то поле Annotation в XML файле не появляется вообще,
                // поэтому скрипт Издания не обрабатывает составное название
                string text = data.DocumentText == null ? "" : data.DocumentText;
                copy.Update(data);
                copy.modifedNumber = number;
                copy.modifedText = text;
                copy.CanAnimate = true;
                return copy;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return copy;
            }
        }

        /// <summary>
        /// Побитовое копирование файлов
        /// </summary>
        /// <param name="sourceFilePath">Откуда копируем</param>
        /// <param name="destFilePath">Куда копируем</param>
        private void Copy(string sourceFilePath, string destFilePath)
        {
            try
            {
                byte[] buffer = new byte[1024 * 1024];
                using (FileStream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
                {
                    long fileLenght = source.Length;
                    using (FileStream dest = new FileStream(destFilePath, FileMode.Create, FileAccess.Write))
                    {
                        long totalBytes = 0;
                        int currentBlockSize = 0;
                        while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytes += currentBlockSize;
                            CopyProgressByte = Math.Round(((double)totalBytes * 100 / fileLenght), 0);
                            dest.Write(buffer, 0, currentBlockSize);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }

        }


        #region Копирование Замененных документов на флеш госитель и в директорию НТЦ Системы 
        private async void ChangeDocument(ChangeDocumentModel doc)
        {
            DocumentsCopy copy = getDocumentsCopy(doc.document);
            try
            {
                DocumentIsChangeMode = true;
                CopyProgress = 0;
                CopyngList.Clear();
                BinaryCopyIsActive = true;
                ProgressMaximum = 1;
                CopyngList.Insert(0, copy);

                await Task.Factory.StartNew(() =>
                    {
                        DirectoryInfo DocumentDirectory = new DirectoryInfo(System.IO.Path.Combine(Helpers.Paths.PakMedoFolder, copy.DirectoryName));
                        DirectoryInfo FlashDirectory = Directory.CreateDirectory(Path.Combine(doc.flashDisk.Name, DirectoryForChangedDocuments, copy.DirectoryName));

                        XmlCopy(DocumentDirectory, FlashDirectory, ref copy);
                        PdfCopy(DocumentDirectory, FlashDirectory, ref copy);

                        if (!copy.PdfIsCopy)
                            throw new Medo.Core.Exceptions.DocumentCopyErrorException(copy, "Не найден PDF файл!");
                        if (!copy.XmlIsCopy)
                            throw new Medo.Core.Exceptions.DocumentCopyErrorException(copy, "Ошибка создания XML карточки!");

                        CopyngFileInfo = string.Empty;
                        UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), copy, "Документ на замену скопирован успешно");
                        copy.IsCopyComplete = true;
                        CopyProgress = CopyngList.Where(c => c.IsCopyComplete).Count();
                        BinaryCopyIsActive = false;
                        doc.document.IsChanged = true;
                        
                       
                        //Client.Collections.StaticCollections.MainCollection.AddOrUpdate((IBaseStatesInterface)doc.document);
                        DocumentIsChangeMode = false;
                        copy.CanAnimate = false;
                        _eventAggregator.GetEvent<UpdateBaseStateEvent>().Publish(doc.document);
                        _eventAggregator.GetEvent<UploadOrChangeDoneEvent>().Publish();
                        //_eventAggregator.GetEvent<FilterDocumentEvent>().Publish(doc.document);
                    });

            }        
            catch (System.Exception ex)
            {
                await UIDispatcher.BeginInvoke(DispatcherPriority.Render, new refDocumentsCopy(insertcopymessage), copy, "Ошибка копирования: " + ex.Message);
                copy.IsCopyComplete = false;
                BinaryCopyIsActive = false;
                logger.Fatal(ex);
                DocumentIsChangeMode = false;
                copy.CanAnimate = false;
                _eventAggregator.GetEvent<UploadOrChangeDoneEvent>().Publish();
            }
        }
        #endregion


    }
}
