using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using Medo.Core.Models;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Media;
using Prism.Events;

namespace Medo.Controls.PdfRecognitionViewer
{
    public class TextSearch : INotifyPropertyChanged
    {

        readonly Logger logger = LogManager.GetCurrentClassLogger();
        Mat ChImage { get; set; }
        Mat image { get; set; }
        public double BitmapHeight { get; set; }
        public double BitmapWidth { get; set; }
        public TextSearch()
        {
            try
            {
                ChImage = new Mat();
                image = new Mat();
                ContoursCollection = new List<RectanglesCoordinates>();
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
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

        #region Fields
        private List<RectanglesCoordinates> _ContoursCollection { get; set; }
        public List<RectanglesCoordinates> ContoursCollection
        {
            get
            {
                return this._ContoursCollection;
            }
            set
            {
                if (this.ContoursCollection != value)
                {
                    this._ContoursCollection = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private bool _SearchInProgress { get; set; }
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
        #endregion

        public async Task<bool> GetPageForRecognition(int pageNumber, FileInfo pdfFile)
        {
            try
            {
                SearchInProgress = true;
                return await Task<bool>.Factory.StartNew(() =>
                {
                    SearchProgressMaximum = 1;
                    SearchProgressValue = 0;
                    SearchProgressText = String.Format("Извлечение страницы {0} из файла {1}", pageNumber, pdfFile.Name);
                    ContoursCollection.Clear();

                    using (Bitmap b = MoonPdfLib.MuPdf.MuPdfWrapper.ExtractPage(pdfFile.FullName, pageNumber, 5f))
                    {
                        using (MemoryStream memoryBitmap = new MemoryStream())
                        {
                            b.Save(memoryBitmap, System.Drawing.Imaging.ImageFormat.Png);
                            SearchProgressValue = 1;
                            GetContoursArray(memoryBitmap.ToArray());
                        }
                    };
                    return true;
                });

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return false;
            }
        }

        #region Обработка с помощью OpenCV
        /// <summary>
        /// Получение массива контуров текста на выбраном изображении в виде прямоугольников
        /// </summary>
        /// <param name="img">Изображение ня котором будет производиться поиск текста</param>
        private void GetContoursArray(byte[] img)
        {
            try
            {

                Mat Hierarchy = new Mat();
                VectorOfVectorOfPoint vp = new VectorOfVectorOfPoint();
                CvInvoke.Imdecode(img, Emgu.CV.CvEnum.LoadImageType.Grayscale, image);
                BitmapHeight = image.Bitmap.Height;
                BitmapWidth = image.Bitmap.Width;
                CvInvoke.MedianBlur(image, ChImage, 15);
                //CvInvoke.BoxFilter(ChImage, ChImage, Emgu.CV.CvEnum.DepthType.Cv8U, new System.Drawing.Size(5, 5), new System.Drawing.Point(-1, -1));         
                //CvInvoke.Imwrite(@"D:\test\1238test\Filtered.tiff", ChImage);
                CvInvoke.Threshold(ChImage, ChImage, 0, 255, Emgu.CV.CvEnum.ThresholdType.BinaryInv | Emgu.CV.CvEnum.ThresholdType.Otsu);

                //CvInvoke.Imwrite(@"D:\test\1238test\Thresold.tiff", ChImage);
                var kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, new System.Drawing.Size(8, 5), new System.Drawing.Point(-1, -1));
                var sc = new MCvScalar(0);
                CvInvoke.Dilate(ChImage, ChImage, kernel, new System.Drawing.Point(-1, -1), 13, Emgu.CV.CvEnum.BorderType.Default, sc);

                //CvInvoke.Imwrite(@"D:\test\1238test\ChImage.tiff", ChImage);
                //CvInvoke.Imwrite(@"D:\test\1238test\Dilate.tiff", ChImage);
                CvInvoke.FindContours(ChImage, vp, Hierarchy, Emgu.CV.CvEnum.RetrType.Ccomp, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                var contoursArray = new List<VectorOfPoint>();
                var Collection = new List<RectanglesCoordinates>();
                int count = vp.Size;
                SearchProgressMaximum = count;

                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint currCont = vp[i])
                    {
                        contoursArray.Add(currCont);
                        SearchProgressText = String.Format("Определение контуров текста {0}", i);
                        SearchProgressValue = i;
                    }
                }
                for (int i = 0; i < contoursArray.Count; i++)
                {
                    var rct = CvInvoke.BoundingRectangle(contoursArray[i]);

                    if ((rct.Height > 80 && rct.Width > 400) && (rct.Height < 2000 && rct.Width < 2800))
                    {
                        //CvInvoke.Rectangle(image, rct, new MCvScalar(121, 236, 56)); добавляем прямоугольники к исходному изображению
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                            rec.Stroke = System.Windows.Media.Brushes.Transparent;
                            rec.StrokeThickness = 0.5;
                            rec.Height = rct.Height;
                            rec.Width = rct.Width;
                            rec.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(60, 181, 241, 108));
                            Canvas.SetZIndex(rec, 2);
                            Collection.Add(new RectanglesCoordinates(rec, rct.X, rct.Y, BitmapHeight, BitmapWidth));
                            SearchProgressText = String.Format("Создание массива текстовых блоков {0}", i);
                            SearchProgressValue = i;                           
                        }));
                    }
                }
                ContoursCollection = new List<RectanglesCoordinates>(Collection);
                //CvInvoke.Imwrite(@"D:\test\1238test\WR.tiff", image);
                //ContoursList = new ObservableCollection<RectangleWithCoordinates>(ContoursArray);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        /// <summary>
        /// Обрезание изображения по координатам заданного прямоугольника
        /// </summary>
        /// <param name="rec">Прямоугольник на изображении, из которого будет вырезана часть изображения (сохраняется вырезанная часть)</param>
        /// <returns></returns>
        public System.Drawing.Bitmap getCropImage(System.Drawing.Rectangle rec)
        {
            try
            {

                System.Drawing.Bitmap bit = null;
                using (Mat afterFilter = new Mat())
                {
                    //CvInvoke.BilateralFilter(image, afterFilter, 25, 80, 100);
                    CvInvoke.MedianBlur(image, afterFilter, 5);
                    using (Mat cropImage = new Mat(afterFilter, rec))
                    {
                        //CvInvoke.Imwrite(@"D:\test\1238test\CROP.tiff", cropImage);
                        bit = cropImage.Bitmap;
                    }
                }
                return bit;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return null;
            }
        }

      
        #endregion
    }
}
