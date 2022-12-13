using Medo.Core.EventsAggregator;
using Medo.Core.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Medo.Core.Collections;
using Medo.Core.Interfaces;
using System.Windows.Shapes;
using NLog;
using Medo.Controls.PdfRecognitionViewer;
using System.Collections.ObjectModel;
using Prism.Regions;

namespace Medo.Modules.PdfViewerModule.ViewModels
{
    class ViewPdfViewerModuleViewModel : TextBlocksSearch
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        IEventAggregator _aggregator;


        public ViewPdfViewerModuleViewModel(IEventAggregator eventAggregator) : base (eventAggregator)
        {
            _aggregator = eventAggregator;
            Files = new AsyncObservableCollection<ExpanderFileSelectorModel>();
            OpenSelectedFileCommand = new DelegateCommand<object>(OpenSelectedFile);
            SubscribeEvents();
        }
        public DelegateCommand<object> OpenSelectedFileCommand { get; set; }
        #region Подписка на события
        private void SubscribeEvents()
        {
            _aggregator.GetEvent<DocumentSelectionChangedEvent>().Subscribe(DocumentSelectionChanged);
            _aggregator.GetEvent<UpdateNonBaseStateEvent>().Subscribe(StartEditDocumentCard);
        }
        #endregion

        private void OpenSelectedFile(object obj)
        {
            var file = (ExpanderFileSelectorModel)obj;
            System.Diagnostics.Process.Start(file.File.FullName);
        }
        /// <summary>
        /// Выполняем при начале или окончании редактирования карточки документа
        /// </summary>
        /// <param name="edit"></param>
        private async void StartEditDocumentCard(INonBaseStatesInterface edit)
        {
            IsEdit = edit.IsEdit;
            if (edit.IsEdit)
            {
                if(edit.SourceGuid == new Guid(Medo.Helpers.SourceGuidOrgansNames.Минюст)
               && (SelectedDocument.DocumentText == null || SelectedDocument.DocumentText.Length < 20))
                {
                    AutoRecognitionMode = true;
                }
               
                await GetPageForRecognition();
            }
            else
            {
                ClearRectangles();
            }
        }

        #region Выбор нового документа
       
        /// <summary>
        /// сдклать выбор только для списка
        /// </summary>
        /// <param name="doc"></param>
        private void DocumentSelectionChanged(Document doc)
        {
            try
            {
                SelectedDocument = doc;
                Files.Clear();
                if (SelectedDocument != null)
                {
                    DirectoryInfo dirPath = new DirectoryInfo(System.IO.Path.Combine(Helpers.Paths.PakMedoFolder, doc.DirectoryName));
                    IEnumerable<FileInfo> files = dirPath.GetFiles("*.*", SearchOption.AllDirectories);
                    foreach (var f in files)
                        Files.Add(new ExpanderFileSelectorModel() { File = f });

                   

                    if (string.IsNullOrEmpty(doc.DefaultPdf))
                    {
                        var pdf = Files.Where(f => !f.File.Name.ToLower().Contains("preview") && f.File.Extension.ToLower() == ".pdf").ElementAtOrDefault(0);
                        SelectedFile = pdf;
                    }
                    else
                    {
                        ExpanderFileSelectorModel p = Files.Where(f => doc.DefaultPdf.ToLower().Contains(f.File.Name.ToLower())).FirstOrDefault();
                        SelectedFile = p;
                    }
                    if (doc.ConvertPdf)
                    {
                        var pdf = Files.Where(f => f.File.Name.ToLower().Contains("converted") && f.File.Extension.ToLower() == ".pdf").ElementAtOrDefault(0);
                        SelectedFile = pdf;
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

       
        #endregion
    }
}
