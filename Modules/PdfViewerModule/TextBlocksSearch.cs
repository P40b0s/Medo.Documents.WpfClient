using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Medo.Core.Collections;
using Medo.Core.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Prism.Commands;
using System.Windows.Input;
using System.Windows.Controls;
using Prism.Events;
using Medo.Core.EventsAggregator;
using Medo.Core.Structures;
using Medo.Core.Enums;
using System.Diagnostics;
using Emgu.CV.CvEnum;

namespace Medo.Modules.PdfViewerModule
{
    class TextBlocksSearch : Base
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        readonly Logger Coordinatelogger = LogManager.GetLogger("recognitionCoordinatesLogger");
        Mat ChImage { get; set; }
        Mat image { get; set; }
        public AsyncObservableCollection<RectanglesCoordinates> RectanglesCollection { get; set; }
        IEventAggregator eventAggregator;
        public TextBlocksSearch(IEventAggregator _eventAggregator) : base(_eventAggregator)
        {
            try
            {
                eventAggregator = _eventAggregator;
                ChImage = new Mat();
                image = new Mat();
                RectanglesCollection = new AsyncObservableCollection<RectanglesCoordinates>();
                RectanglesCollection.CollectionChanged += RectanglesCollection_CollectionChanged;
                this.PropertyChanged += TextBlocksSearch_PropertyChanged;
                CommandsInitialization();

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private void TextBlocksSearch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedFile")
                RectanglesCollection.Clear();
        }

        private void RectanglesCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (RectanglesCollection.Count == 0)
            {
                StaticProperty.RecognitionMode = false;
            }
            else
            {
                StaticProperty.RecognitionMode = true;
            }
            ClearRectanglesCommand.RaiseCanExecuteChanged();
            AddLayerBlockCommand.RaiseCanExecuteChanged();
        }


        #region Commands
        public DelegateCommand ClearRectanglesCommand { get; set; }
        public DelegateCommand StartSearchBlocksCommand { get; set; }
        public DelegateCommand AddLayerBlockCommand { get; set; }
        public DelegateCommand<object> GetBitmapForCurrentBlockCommand { get; set; }
        public DelegateCommand<object> DeleteCurrentBlockCommand { get; set; }


        private void CommandsInitialization()
        {
            ClearRectanglesCommand = new DelegateCommand(ClearRectangles, () => StaticProperty.RecognitionMode);
            StartSearchBlocksCommand = DelegateCommand.FromAsyncHandler(GetPageForRecognition);
            AddLayerBlockCommand = new DelegateCommand(() => AddLayerBlock(), () => StaticProperty.RecognitionMode);
            GetBitmapForCurrentBlockCommand = new DelegateCommand<object>(GetBitmapForCurrentBlock);
            DeleteCurrentBlockCommand = new DelegateCommand<object>(DeleteCurrentBlock);
        }



        #endregion

        private void DeleteCurrentBlock(object rect)
        {
            try
            {
                var rec = (RectanglesCoordinates)rect;
                RectanglesCollection.Remove(rec);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        private void GetBitmapForCurrentBlock(object rect)
        {
            try
            {
                var rec = (RectanglesCoordinates)rect;
                CropImage(rec);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        public RectanglesCoordinates AddLayerBlock(double X = 0, double Y = 0, double W = 0, double H = 0)
        {
            try
            {
                RectanglesCoordinates rect = new RectanglesCoordinates(StaticProperty.BitmapHeight,
                    StaticProperty.BitmapWidth,
                    StaticProperty.DocumentControlHeight,
                    StaticProperty.DocumentControlWidth);

                if (X == 0 && Y == 0)
                {
                    rect.ControlRectangleWidth = 200;
                    rect.ControlRectangleHeight = 100;
                    rect.ControlXCoords = 100;
                    rect.ControlYCoords = 100;
                }
                else
                {
                    rect.ControlRectangleWidth = W;
                    rect.ControlRectangleHeight = H;
                    rect.ControlXCoords = X;
                    rect.ControlYCoords = Y - RectanglesCoordinates.correction;
                }

                RectanglesCollection.Insert(0, rect);
                return rect;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return null;
            }
        }

        public void ClearRectangles()
        {
            RectanglesCollection.Clear();
        }
        public bool AutoRecognitionMode { get; set; }
        public async Task<bool> GetPageForRecognition()
        {
            try
            {
                return await Task<bool>.Factory.StartNew(() =>
                {
                    ClearRectangles();
                    switch (SelectedFile.FileType)
                    {
                        case FileTypeEnum.Pdf:
                            {
                                SearchInProgress = true;
                                SearchProgressMaximum = 1;
                                SearchProgressValue = 0;
                                SearchProgressText = String.Format("Поиск блоков на странице {0} файла {1}", StaticProperty.CurrentPageNumber, SelectedFile.File.Name);
                                using (Bitmap b = MoonPdfLib.MuPdf.MuPdfWrapper.ExtractPage(SelectedFile.File.FullName, StaticProperty.CurrentPageNumber, 5f))
                                {
                                    using (MemoryStream memoryBitmap = new MemoryStream())
                                    {
                                        b.Save(memoryBitmap, System.Drawing.Imaging.ImageFormat.Png);
                                        SearchProgressValue = 1;
                                        GetContoursArray(memoryBitmap.ToArray());
                                    }
                                };
                                break;
                            }
                        case FileTypeEnum.Image:
                            {
                                SearchInProgress = true;
                                SearchProgressMaximum = 1;
                                SearchProgressValue = 0;
                                SearchProgressText = String.Format("Преобразование файла {0}", SelectedFile.File.Name);
                                using (Bitmap b = new Bitmap(SelectedFile.File.FullName))
                                {
                                    using (MemoryStream memoryBitmap = new MemoryStream())
                                    {
                                        b.Save(memoryBitmap, System.Drawing.Imaging.ImageFormat.Png);
                                        SearchProgressValue = 1;
                                        GetContoursArray(memoryBitmap.ToArray());
                                    }
                                }
                                break;
                            }
                    }
                    AutoRecognitionMode = false;
                    return true;
                });

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                ClearRectangles();
                return false;
            }
        }

        #region Обработка с помощью OpenCV

        private void RectangleDetection(Mat image)
        {
            //Image<Gray, byte> imageGrey = image.Convert<Gray, byte>();

            List<Triangle2DF> triangleList = new List<Triangle2DF>();
            List<RotatedRect> boxList = new List<RotatedRect>();

            UMat cannyEdges = new UMat();
            double cannyThreshold = 180.0;
            double cannyThresholdLinking = 120.0;
            //CvInvoke.Imwrite("D:\\IMAGE.jpg", image);
            CvInvoke.GaussianBlur(image, image, new Size(5, 5), 0);
            //CvInvoke.Imwrite("D:\\GAUSSIAN.jpg", image);
            CvInvoke.Canny(image, cannyEdges, cannyThreshold, cannyThresholdLinking);
            //CvInvoke.Imwrite("D:\\CANNY.jpg", cannyEdges);
            var kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new System.Drawing.Size(2, 2), new System.Drawing.Point(-1, -1));
            var sc = new MCvScalar(0);
            CvInvoke.Dilate(cannyEdges, cannyEdges, kernel, new System.Drawing.Point(-1, -1), 16, Emgu.CV.CvEnum.BorderType.Default, sc);
            //CvInvoke.Imwrite("D:\\AFTERDILATE.jpg", cannyEdges);
            //LineSegment2D[] lines = CvInvoke.HoughLinesP(
            //                                                cannyEdges,
            //                                                1, //Distance resolution in pixel-related units
            //                                                Math.PI / 45.0, //Angle resolution measured in radians.
            //                                                20, //threshold
            //                                                100, //min Line width
            //                                                1); //gap between lines

            using (VectorOfVectorOfPoint countours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, countours, null, RetrType.Ccomp, ChainApproxMethod.ChainApproxSimple);
                int count = countours.Size;
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint kontur = countours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(kontur, approxContour, CvInvoke.ArcLength(kontur, true) * 0.1, true);
                        if (CvInvoke.ContourArea(approxContour, false) > 350) //only consider contours with area greater than 250
                        {
                            //if (approxContour.Size == 3) //the countour has 3 vertices. it is triangle
                            //{
                            //    System.Drawing.Point[] pts = approxContour.ToArray();
                            //    triangleList.Add(new Triangle2DF(pts[0], pts[1], pts[2]));
                            //}
                            //else if (approxContour.Size == 4) //rectangle
                            if (approxContour.Size >= 3 && approxContour.Size <= 6) //rectangle
                            {
                                //determine if allthe angles in the contour are within [80,100] degree
                                bool isRectangle = true;
                                System.Drawing.Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = Emgu.CV.PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(edges[(j + i) % edges.Length].GetExteriorAngleDegree(edges[j]));

                                    if (angle < 80 || angle > 100)
                                    {
                                        isRectangle = false;
                                        break;
                                    }

                                }
                                if (isRectangle)
                                    boxList.Add(CvInvoke.MinAreaRect(approxContour));

                            }
                           
                           

                        }
                    }
                }
            }
            //Поиск линий
            //UMat LinescannyEdges = new UMat();
            //LineSegment2D[] lines = CvInvoke.HoughLinesP(
            //                                             cannyEdges,
            //                                             1, //Distance resolution in pixel-related units
            //                                             Math.PI / 10.0, //Angle resolution measured in radians.
            //                                             30, //threshold
            //                                             200, //min Line width
            //                                             55); //gap between lines

            // рисование линий для полученных геометрических фигур
            Image<Bgr, Byte> triRectImage =
               new Image<Bgr, byte>(image.Bitmap);
            //foreach (Triangle2DF triangle in triangleList)
            //    triRectImage.Draw(triangle, new Bgr(0, 255, 0), 5);
            //foreach (LineSegment2D line in lines)
            //    triRectImage.Draw(line, new Bgr(0, 255, 0), 5);
            foreach (RotatedRect box in boxList)
                triRectImage.Draw(box, new Bgr(0, 0, 255), 5);
            //CvInvoke.Imwrite("D:\\TESTIMG.jpg", triRectImage);
        }

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
                
                StaticProperty.BitmapHeight = image.Bitmap.Height;
                StaticProperty.BitmapWidth = image.Bitmap.Width;
                CvInvoke.MedianBlur(image, ChImage, 15);
                RectangleDetection(image);
                //CvInvoke.BoxFilter(ChImage, ChImage, Emgu.CV.CvEnum.DepthType.Cv8U, new System.Drawing.Size(5, 5), new System.Drawing.Point(-1, -1));         
                //CvInvoke.Imwrite(@"D:\test\1238test\blur.tiff", ChImage);
                CvInvoke.Threshold(ChImage, ChImage, 0, 255, Emgu.CV.CvEnum.ThresholdType.BinaryInv | Emgu.CV.CvEnum.ThresholdType.Otsu);
                
                //CvInvoke.Imwrite(@"D:\test\1238test\Thresold.tiff", ChImage);
                var kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, new System.Drawing.Size(8, 5), new System.Drawing.Point(-1, -1));
                var sc = new MCvScalar(0);
                CvInvoke.Dilate(ChImage, ChImage, kernel, new System.Drawing.Point(-1, -1), 13, Emgu.CV.CvEnum.BorderType.Default, sc);
               
                //CvInvoke.Imwrite(@"D:\test\1238test\Dilate.tiff", ChImage);
                CvInvoke.FindContours(ChImage, vp, Hierarchy, Emgu.CV.CvEnum.RetrType.Ccomp, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                var contoursArray = new List<VectorOfPoint>();
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
                    double bitmapWidth = RectanglesCoordinates.ToControlProportions(StaticProperty.BitmapWidth, StaticProperty.DocumentControlWidth, StaticProperty.BitmapWidth);
                    double W = RectanglesCoordinates.ToControlProportions(rct.Width, StaticProperty.DocumentControlWidth, StaticProperty.BitmapWidth);
                    double H = RectanglesCoordinates.ToControlProportions(rct.Height, StaticProperty.DocumentControlHeight, StaticProperty.BitmapHeight);
                    double Y = RectanglesCoordinates.ToControlProportions(rct.Y, StaticProperty.DocumentControlHeight, StaticProperty.BitmapHeight) - RectanglesCoordinates.correction;
                    double X = RectanglesCoordinates.ToControlProportions(rct.X, StaticProperty.DocumentControlWidth, StaticProperty.BitmapWidth);
                    
                    // максамальная и минимальная высота контура
                    if ((H > 20 && H < 700) &&
                        //максимальная и минимальная ширина контура
                        (W > 200 && W < 600) &&
                        //точка расположения координаты Y (подбиралась для наименования документа)
                        (Y > 200  && Y < 420) &&
                        //Точка расположения координаты X
                        (X < (bitmapWidth / 2.1F)))
                    {

                        //CvInvoke.Rectangle(image, rct, new MCvScalar(121, 236, 56)); добавляем прямоугольники к исходному изображению
                        SearchProgressText = "Создание массива текстовых блоков";
                        RectanglesCoordinates r = null;
                        
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            r = getRectangle(rct);
                        }));
                        //Делаем insert а не add потому, что при Add каждый 
                        //следующий блук перекрывает предыдущий (по zindex) 
                        //и нажать на нем кнопки становиться невозможно
                        //При insert перекрытие идет в обратном порядке и кнопки доступны
                        //но только если панель с кнопками распологается сверху
                        RectanglesCollection.Insert(0, r);
                        SearchProgressText = String.Format("Создание массива текстовых блоков {0}", i);
                        SearchProgressValue = i;
                    }
                }
                ContainsCheck();
                //Если не найдено ни одного контура, добавляем контур вручную в рандомном месте
                if (RectanglesCollection.Count == 0)
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        AddLayerBlock(100, 100, 100, 100);
                    }));
               
                if(AutoRecognitionMode && RectanglesCollection.Count > 0)
                {
                    CropImage(RectanglesCollection[0]);
                }
                SearchInProgress = false;

                //CvInvoke.Imwrite(@"D:\test\1238test\WR.tiff", image);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        /// <summary>
        /// Рекурсивное удаление объектов которые целиком находятся в границах других объектов
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public  void ContainsCheck()
        {
            try
            {
                for (int c = RectanglesCollection.Count - 1; c >= 0; c--)
                {
                    foreach (var r in RectanglesCollection)
                    {
                       if (r.Contains(RectanglesCollection[c]))
                            RectanglesCollection.RemoveAt(c);
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private RectanglesCoordinates getRectangle(Rectangle rct)
        {
            RectanglesCoordinates rect = new RectanglesCoordinates(StaticProperty.BitmapHeight,
                    StaticProperty.BitmapWidth,
                    StaticProperty.DocumentControlHeight,
                    StaticProperty.DocumentControlWidth);
            try
            {
                rect.OriginalRectangleWidth = rct.Width;
                rect.OriginalRectangleHeight = rct.Height;
                rect.XCoords = rct.X;
                rect.YCoords = rct.Y;
                return rect;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return rect;
            }

        }

        /// <summary>
        /// Обрезание изображения по координатам заданного прямоугольника
        /// </summary>
        /// <param name="rec">Прямоугольник на изображении, из которого будет вырезана часть изображения (сохраняется вырезанная часть)</param>
        /// <returns></returns>
        private Task CropImage(RectanglesCoordinates rec)
        {
            return Task.Factory.StartNew(() =>
             {
                 try
                 {
                     System.Drawing.Rectangle rectangle = new Rectangle();
                     rectangle.Height = Convert.ToInt16(rec.OriginalRectangleHeight);
                     rectangle.Width = Convert.ToInt16(rec.OriginalRectangleWidth);
                     rectangle.X = Convert.ToInt16(rec.XCoords);
                     rectangle.Y = Convert.ToInt16(rec.YCoords);
                     Coordinatelogger.Info($"X:{rec.ControlXCoords} Y:{rec.ControlYCoords} W:{rec.ControlRectangleWidth} H:{rec.ControlRectangleHeight} X%:{rec.ControlXCoordsPercent} Y%:{rec.ControlYCoordsPercent}");
                     System.Drawing.Bitmap bit = null;
                     using (Mat afterFilter = new Mat())
                     {
                         //CvInvoke.BilateralFilter(image, afterFilter, 25, 80, 100);
                         CvInvoke.MedianBlur(image, afterFilter, 5);
                         using (Mat cropImage = new Mat(afterFilter, rectangle))
                         {
                             //CvInvoke.Imwrite(@"D:\test\1238test\CROP.tiff", cropImage);
                             bit = cropImage.Bitmap;
                         }
                     }
                     if (bit != null)
                     {
                         RecognitionTypeEnum recognitionEnum = RecognitionTypeEnum.Наименование;
                         Enum.TryParse(rec.RectangleRecognitionType[rec.RecognitionTypeSelectedIndex], out recognitionEnum);
                         RecognitionTypeStruct r = new RecognitionTypeStruct() { CroppedImage = bit, RecognitionType = recognitionEnum };
                         eventAggregator.GetEvent<RecognizeCroppedImageEvent>().Publish(r);
                     }
                 }
                 catch (System.Exception ex)
                 {
                     logger.Fatal(ex);
                 }
             });
        }


        #endregion
    }
}
