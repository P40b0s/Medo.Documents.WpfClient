using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Controls.PdfRecognitionViewer
{
    /// <summary>
    /// Расчет пропорций элементов к файлу PDF и наоборот
    /// </summary>
    class Proportions
    {
        #region Перерасчет пропорций PDF<->Control
        /// <summary>
        /// Расчет пропорций (Высота или ширина - После поиска блоков текста на изображении оригинального размера)
        /// </summary>
        /// <param name="coordinatsForChange">Исходные координаты</param>
        /// <param name="elementSize">Размер элемента в котором будет распологатся Rectangle</param>
        /// <param name="originalBitmapSize">Размер оригинального изображения на котором отмечались текстовые блоки (Rectangle)</param>
        /// <returns>Возвращает координату после перерасчета пропорций</returns>
        public static double ToElementProportions(double coordinatsForChange, double elementSize, double originalBitmapSize)
        {
            //y = a*b/c
            //a = y*c/b->
            return coordinatsForChange * elementSize / originalBitmapSize;
        }
        /// <summary>
        /// Расчет пропорций (Высота или Ширина - Получение координат с оригинального изображения)
        /// </summary>
        /// <param name="coordinats">Исходные координаты</param>
        /// <param name="elementSize">Размер элемента в котором располагается Rectangle</param>
        /// <param name="originalBitmapSize">Размер оригинального изображения на котором отмечались текстовые блоки (Rectangle)</param>
        /// <returns>Возвращает координату после перерасчета пропорций</returns>
        public static double ToImageProportions(double coordinats, double elementSize, double originalBitmapSize)
        {
            //y = a*b/c
            //a = y*c/b->
            return coordinats * originalBitmapSize / elementSize;
        }
        #endregion
    }
}
