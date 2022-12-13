using Medo.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using NLog;
using System.Collections.ObjectModel;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Interactivity;

namespace Medo.Controls.RectangleCanvas
{
    /// <summary>
    /// Логика взаимодействия для RectangleCanvas.xaml
    /// </summary>
    public partial class Canvas : System.Windows.Controls.Canvas, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public Canvas()
        {
            InitializeComponent();           
        }

        #region Свойства зависимостей
        [Bindable(true)]
        public IEnumerable ContoursList
        {
            get { return (IEnumerable)GetValue(ContoursListProperty); }
            set { SetValue(ContoursListProperty, value); }
        }

        public static readonly DependencyProperty ContoursListProperty
          = DependencyProperty.Register("ContoursList",
              typeof(IEnumerable),
              typeof(Canvas),
              new FrameworkPropertyMetadata(new List<RectangleWithCoordinates>(), new PropertyChangedCallback(ContoursListPropertyChanged)));

        private static void ContoursListPropertyChanged(DependencyObject dep, DependencyPropertyChangedEventArgs e)
        {
            var item = (Canvas)dep;
            var newitem = e.NewValue;
            var olditem = e.OldValue;
            if (newitem != null)
            {
                item.getContours((IEnumerable<RectangleWithCoordinates>)e.NewValue);
            }
        }

        [Bindable(true)]
        public string SourcePdf
        {
            get { return (string)GetValue(SourcePdfProperty); }
            set { SetValue(SourcePdfProperty, value); }
        }

        public static readonly DependencyProperty SourcePdfProperty
          = DependencyProperty.Register("SourcePdf",
              typeof(string),
              typeof(Canvas), new PropertyMetadata(null, new PropertyChangedCallback(SourcePdfChanged)));

        private static void SourcePdfChanged(DependencyObject dep, DependencyPropertyChangedEventArgs e)
        {
            var item = (Canvas)dep;
            var newitem = e.NewValue;
            var olditem = e.OldValue;
            if (newitem != null)
            {
                item.ClearAllChildrens();
            }
        }
        #endregion
        /// <summary>
        /// Удаляем всех чилдренов кроме 0-вого 0-вой это грид с пдфвьювером
        /// </summary>
        private void ClearAllChildrens()
        {
            for (int i = can.Children.Count - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    can.Children.RemoveAt(i);
                }
            }
        }
        private void getContours(IEnumerable<RectangleWithCoordinates> collection)
        {
            try
            {
                foreach (RectangleWithCoordinates rect in collection)
                {
                    double heightCoords = Proportions.ToElementProportions(rect.YCoords, can.ActualHeight, rect.BitmapHeight);
                    double widthCoords = Proportions.ToElementProportions(rect.XCoords, can.ActualWidth, rect.BitmapWidth);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        rect.rectangle.Height = Proportions.ToElementProportions(rect.rectangle.Height, can.ActualHeight, rect.BitmapHeight);
                        rect.rectangle.Width = Proportions.ToElementProportions(rect.rectangle.Width, can.ActualWidth, rect.BitmapWidth);
                        System.Windows.Controls.Canvas.SetTop(rect.rectangle, heightCoords);
                        System.Windows.Controls.Canvas.SetLeft(rect.rectangle, widthCoords);
                        System.Windows.Controls.Canvas.SetZIndex(rect.rectangle, 2);
                        
                        //rect.rectangle.MouseEnter += Rectangle_MouseEnter;
                        //rect.rectangle.MouseLeave += Rectangle_MouseLeave;
                        //rect.rectangle.MouseDown += Rectangle_MouseDown;
                        //rect.rectangle.MouseLeftButtonDown += Rectangle_MouseLeftButtonDown;
                        //rect.rectangle.MouseRightButtonDown += Rectangle_MouseRightButtonDown;
                        can.Children.Add(rect.rectangle); 
                    }));
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
    }
}
