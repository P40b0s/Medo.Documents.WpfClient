using Medo.Core.Collections;
using Medo.Core.Enums;
using Medo.Core.EventsAggregator;
using Medo.Core.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.PdfViewerModule
{
    public abstract class Base : INotifyPropertyChanged
    {
        IEventAggregator eventAggregator;
        public Base(IEventAggregator _eventAggregator) { eventAggregator = _eventAggregator; }
        #region PropertyChanged realization
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        public Document SelectedDocument { get; set; }

        private bool _SearchInProgress { get; set; }
        /// <summary>
        /// Активность процесса поиска текстовых блоков
        /// </summary>
        public bool SearchInProgress
        {
            get
            {
                return this._SearchInProgress;
            }
            set
            {
                if (this.SearchInProgress != value)
                {
                    this._SearchInProgress = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private ExpanderFileSelectorModel _SelectedFile { get; set; }
        /// <summary>
        /// Выбранный в данный момент файл
        /// </summary>
        public ExpanderFileSelectorModel SelectedFile
        {
            get
            {
                return this._SelectedFile;
            }
            set
            {
                if (this.SelectedFile != value)
                {
                    if (value != null && value.IsCanSelect)
                    {
                        this._SelectedFile = value;
                        this.OnPropertyChanged();
                        FilesExpanderIsOpen = false;
                        StaticProperty.RecognitionMode = false;
                        eventAggregator.GetEvent<SelectFileEvent>().Publish(value);
                        SelectedFileChanged();
                    }

                }
            }
        }

        private double _SearchProgressValue { get; set; }
        public double SearchProgressValue
        {
            get
            {
                return this._SearchProgressValue;
            }
            set
            {
                if (this.SearchProgressValue != value)
                {
                    this._SearchProgressValue = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private double _SearchProgressMaximum { get; set; }
        public double SearchProgressMaximum
        {
            get
            {
                return this._SearchProgressMaximum;
            }
            set
            {
                if (this.SearchProgressMaximum != value)
                {
                    this._SearchProgressMaximum = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private string _SearchProgressText { get; set; }
        /// <summary>
        /// Отображение прогресса обработки в виде строки
        /// </summary>
        public string SearchProgressText
        {
            get
            {
                return this._SearchProgressText;
            }
            set
            {
                if (this.SearchProgressText != value)
                {
                    this._SearchProgressText = value;
                    this.OnPropertyChanged();

                }
            }
        }



        #region Список фалов в директории, просмотр и выбор файлов
        private AsyncObservableCollection<ExpanderFileSelectorModel> _Files { get; set; }
        /// <summary>
        /// Коллекция файлов из директории с документом
        /// </summary>
        public AsyncObservableCollection<ExpanderFileSelectorModel> Files
        {
            get
            {
                return this._Files;
            }
            set
            {
                if (this.Files != value)
                {
                    this._Files = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _FilesExpanderIsOpen { get; set; }
        public bool FilesExpanderIsOpen
        {
            get
            {
                return this._FilesExpanderIsOpen;
            }
            set
            {
                if (this.FilesExpanderIsOpen != value)
                {
                    this._FilesExpanderIsOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }
        #endregion

        private bool _IsEdit { get; set; }
        /// <summary>
        /// Редактируется ли в данный момент карточка документа
        /// </summary>
        public bool IsEdit
        {
            get
            {
                return this._IsEdit;
            }
            set
            {
                if (this.IsEdit != value)
                {
                    this._IsEdit = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Метод при изменении выделенного файла в экспандере
        /// </summary>
        /// <param name="value"></param>
        private void SelectedFileChanged()
        {
            DirectoryInfo dir = new DirectoryInfo(System.IO.Path.Combine(Helpers.Paths.PakMedoFolder, SelectedDocument.DirectoryName));
            string filePath = SelectedFile.File.FullName.Remove(0, dir.FullName.Length + 1);
            if (SelectedFile.FileType == FileTypeEnum.Pdf  && filePath != SelectedDocument.DefaultPdf)
            {
                SelectedDocument.DefaultPdf = filePath;
                eventAggregator.GetEvent<SetDefaultPdfFileEvent>().Publish(SelectedDocument);
            }
        }

    }
}
