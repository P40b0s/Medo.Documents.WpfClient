using BitMiracle.LibTiff.Classic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Medo.Client.Notifications.Interfaces;
using MuPDFLib;
using NLog;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client.Notifications.Models
{
    public class PdfConverterModel : Confirmation, INotifyPropertyChanged, IPdfConverterInterface
    {

        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private const string TemplateFolder = "TemplateConverterFolder";
        public FileInfo PdfForConvert { get; set; }
        public bool IsColor { get; set; }
        public async Task<bool> ConvertPdf()
        {
           return await Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    var stream = File.ReadAllBytes(PdfForConvert.FullName);
                    {
                        using (MuPDF pdf = new MuPDF(stream, ""))
                        {
                            RenderType rend = RenderType.Monochrome;
                            int count = pdf.PageCount;
                            pdf.Initialize();
                            if (IsColor)
                            {
                                rend = RenderType.RGB;
                            }
                            if (Directory.Exists(TemplateFolder))
                            {
                                Directory.Delete(TemplateFolder, true);
                            }
                            Directory.CreateDirectory(TemplateFolder);
                            ConvertedPagesCount = 0;
                            PagesMaximum = count;
                            for (int i = 1; i <= count; i++)
                            {
                                pdf.Page = i;
                                var h = pdf.Height;
                                var w = pdf.Width;
                                int height = 3608;
                                int weight = 2550;
                                if (w > h)
                                {
                                    height = 2550;
                                    weight = 3608;
                                }
                                Bitmap b = pdf.GetBitmap(weight, height, 300, 300, 0, rend, false, false, 50);
                                b.Save(Path.Combine(TemplateFolder, i.ToString(format) + ".tiff"));
                                ConvertedPagesCount++;
                                Status = string.Format($"Извлечено {ConvertedPagesCount} изображений из {PagesMaximum}");

                            }
                        }
                    }
                    CreatePdfFromImages(Path.Combine(Helpers.Paths.PakMedoFolder, doc.DirectoryName, Path.GetFileNameWithoutExtension(PdfForConvert.FullName) + "_converted.pdf"));
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex);
                    return false;
                }
            });
        }

        private void CreatePdfFromImages(string path)
        {
            ConvertedPagesCount = 0;
            Document PdfDoc = new Document();
            try
            {
                PdfDoc.AddAuthor("УОПИ Спцсвязи ФСО России");
                PdfDoc.SetMargins(0, 0, 0, 0);
                PdfWriter writer = PdfWriter.GetInstance(PdfDoc, new FileStream(path, FileMode.Create));
                writer.PdfVersion = PdfWriter.VERSION_1_5;
                iTextSharp.text.Rectangle rec1 = new iTextSharp.text.Rectangle(iTextSharp.text.PageSize.A4.Width, iTextSharp.text.PageSize.A4.Height);
                iTextSharp.text.Rectangle rec2 = new iTextSharp.text.Rectangle(iTextSharp.text.PageSize.A4.Height, iTextSharp.text.PageSize.A4.Width);
                rec1.BackgroundColor = new BaseColor(Color.White);
                rec2.BackgroundColor = new BaseColor(Color.White);
                PdfDoc.Open();
                List<FileInfo> files = new DirectoryInfo(TemplateFolder).GetFiles().OrderBy(x => x.Name).ToList();
                PagesMaximum = files.Count();
                iTextSharp.text.Image tif = null;
                foreach (FileInfo PageBitmap in files)
                {
                    if (IsColor == true)
                    {
                        tif = iTextSharp.text.Image.GetInstance(ConvertToColorTiff(PageBitmap.FullName).ToArray());
                    }
                    else
                    {
                        tif = iTextSharp.text.Image.GetInstance(ConvertToBitonal(PageBitmap.FullName).ToArray());
                    }

                    tif.BackgroundColor = new BaseColor(Color.Transparent);
                    if (tif.Width > tif.Height)
                    {
                        PdfDoc.SetPageSize(rec2);
                    }
                    else
                    {
                        PdfDoc.SetPageSize(rec1);
                    }
                    var w = tif.Width;
                    var h = tif.Height;
                    tif.SetDpi(300, 300);
                    tif.ScalePercent(100 * 73 / 300);
                    PdfDoc.NewPage();
                    PdfDoc.Add(tif);
                    ConvertedPagesCount++;
                    Status = string.Format($"Формирование PDF файла... {ConvertedPagesCount} из {PagesMaximum}");
                }
            }
            catch (Exception e)
            {
                if (e.HResult != int.Parse("-2146233079"))
                {
                    logger.Fatal(e);
                }
            }

            finally
            {
                PdfDoc.Close();
                new DirectoryInfo(TemplateFolder).Delete(true);
            }
        }





        #region Конвертация TIFF
        private MemoryStream ConvertToColorTiff(string path)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    Bitmap bmp = new Bitmap(stream);
                    TiffStream TifStream = new TiffStream();
                    using (Tiff tif = Tiff.ClientOpen(string.Empty, "w", ms, TifStream))
                    {
                        byte[] raster = getImageRasterBytes(bmp, PixelFormat.Format24bppRgb);
                        tif.SetField(TiffTag.IMAGEWIDTH, bmp.Width);
                        tif.SetField(TiffTag.IMAGELENGTH, bmp.Height);
                        tif.SetField(TiffTag.COMPRESSION, Compression.LZW);
                        tif.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);

                        tif.SetField(TiffTag.ROWSPERSTRIP, bmp.Height);

                        tif.SetField(TiffTag.XRESOLUTION, bmp.HorizontalResolution);
                        tif.SetField(TiffTag.YRESOLUTION, bmp.VerticalResolution);

                        tif.SetField(TiffTag.BITSPERSAMPLE, 8);
                        tif.SetField(TiffTag.SAMPLESPERPIXEL, 3);

                        tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

                        int stride = raster.Length / bmp.Height;
                        convertSamples(raster, bmp.Width, bmp.Height);

                        for (int i = 0, offset = 0; i < bmp.Height; i++)
                        {
                            tif.WriteScanline(raster, offset, i, 0);
                            offset += stride;
                        }
                    }
                }
                return ms;
            }
            catch (Exception e)
            {
                logger.Fatal(e);
                return null;
            }
        }

        private MemoryStream ConvertToBitonal(string src)
        {
            MemoryStream ms = new MemoryStream();
            using (var stream = new FileStream(src, FileMode.Open, FileAccess.Read))
            {
                Bitmap original = new Bitmap(stream);
                Bitmap source = null;

                // Если рисунок не в формате 32 BPP, ARGB тогда конвертируем
                if (original.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                {
                    source = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                    using (Graphics g = Graphics.FromImage(source))
                    {
                        g.DrawImageUnscaled(original, 0, 0);
                    }
                }
                else
                {
                    source = original;
                }

                // Лочим рисунок в память
                BitmapData sourceData = source.LockBits(new System.Drawing.Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // копируем рисунок в массив байт
                int imageSize = sourceData.Stride * sourceData.Height;
                byte[] sourceBuffer = new byte[imageSize];
                Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

                // анлочим битмап
                source.UnlockBits(sourceData);

                // создаем битмап для сохранения
                Bitmap destination = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                destination.SetResolution(original.HorizontalResolution, original.VerticalResolution);

                // лочим битмап для сохранения в память
                BitmapData destinationData = destination.LockBits(new System.Drawing.Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);

                // создаем буфер назначения
                imageSize = destinationData.Stride * destinationData.Height;
                byte[] destinationBuffer = new byte[imageSize];

                int sourceIndex = 0;
                int destinationIndex = 0;
                int pixelTotal = 0;
                byte destinationValue = 0;
                int pixelValue = 128;
                int height = source.Height;
                int width = source.Width;
                int threshold = 500;

                // линии
                for (int y = 0; y < height; y++)
                {
                    sourceIndex = y * sourceData.Stride;
                    destinationIndex = y * destinationData.Stride;
                    destinationValue = 0;
                    pixelValue = 128;

                    // пиксели
                    for (int x = 0; x < width; x++)
                    {
                        // Вычисляем цвета пикселей (красный синий зеленый)
                        //                           B                             G                              R
                        pixelTotal = sourceBuffer[sourceIndex] + sourceBuffer[sourceIndex + 1] + sourceBuffer[sourceIndex + 2];
                        if (pixelTotal > threshold)
                        {
                            destinationValue += (byte)pixelValue;
                        }
                        if (pixelValue == 1)
                        {
                            destinationBuffer[destinationIndex] = destinationValue;
                            destinationIndex++;
                            destinationValue = 0;
                            pixelValue = 128;
                        }
                        else
                        {
                            pixelValue >>= 1;
                        }
                        sourceIndex += 4;
                    }
                    if (pixelValue != 128)
                    {
                        destinationBuffer[destinationIndex] = destinationValue;
                    }
                }

                // Copy binary image data to destination bitmap
                Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);

                // Unlock destination bitmap
                destination.UnlockBits(destinationData);

                // Dispose of source if not originally supplied bitmap
                if (source != original)
                {
                    source.Dispose();
                }
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/tiff");
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Compression;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                //сохраняем tif
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionCCITT4);
                myEncoderParameters.Param[0] = myEncoderParameter;
                destination.Save(ms, myImageCodecInfo, myEncoderParameters);
            }
            return ms;
        }

        #region вспомогательные функции


        private int CalculateTiffPagesCount(string FilePath)
        {
            int count = 0;
            using (Tiff image = Tiff.Open(FilePath, "r"))
            {
                short i = image.NumberOfDirectories();
                if (image != null)
                {
                    do
                    {
                        ++count;
                    } while (image.ReadDirectory());

                }
            }
            return count;
        }







        private byte[] getImageRasterBytes(Bitmap bmp, PixelFormat format)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            byte[] bits = null;

            try
            {
                // Lock the managed memory
                BitmapData bmpdata = bmp.LockBits(rect, ImageLockMode.ReadWrite, format);

                // Declare an array to hold the bytes of the bitmap.
                bits = new byte[bmpdata.Stride * bmpdata.Height];

                // Copy the values into the array.
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, bits, 0, bits.Length);

                // Release managed memory
                bmp.UnlockBits(bmpdata);
            }
            catch
            {
                return null;
            }

            return bits;
        }
        private byte[] GetBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        private ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;

        }
        /// <summary>
        /// Converts BGR samples into RGB samples
        /// </summary>
        private void convertSamples(byte[] data, int width, int height)
        {
            int stride = data.Length / height;
            const int samplesPerPixel = 3;

            for (int y = 0; y < height; y++)
            {
                int offset = stride * y;
                int strideEnd = offset + width * samplesPerPixel;

                for (int i = offset; i < strideEnd; i += samplesPerPixel)
                {
                    byte temp = data[i + 2];
                    data[i + 2] = data[i];
                    data[i] = temp;
                }
            }
        }
        #endregion
        #endregion



        #region Properties
        public Medo.Core.Models.Document doc
        {
            get
            {
                return this._doc;
            }
            set
            {
                if (this.doc != value)
                {
                    this._doc = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private Medo.Core.Models.Document _doc;
        public int ConvertedPagesCount
        {
            get
            {
                return this._ConvertedPagesCount;
            }
            set
            {
                if (this.ConvertedPagesCount != value)
                {
                    this._ConvertedPagesCount = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private int _ConvertedPagesCount;
        public string Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                if (this.Status != value)
                {
                    this._Status = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _Status;
        public int PagesMaximum
        {
            get
            {
                return this._PagesMaximum;
            }
            set
            {
                if (this.PagesMaximum != value)
                {
                    this._PagesMaximum = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private int _PagesMaximum;
        private string format = "00000";
        #endregion
    }
}
