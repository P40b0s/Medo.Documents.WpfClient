using NLog;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.ComponentModel;
using System.Windows;

namespace Medo.Controls.PdfRecognitionViewer.Behaviors
{
    public class CanvasBehavior : Behavior<Canvas>
    {
        private enum HitType
        {
            None, Body, UL, UR, LR, LL, L, R, T, B
        };
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.MouseDown -= AssociatedObject_MouseDown;
            base.AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            base.AssociatedObject.MouseUp -= AssociatedObject_MouseUp;

        }
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            base.AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            base.AssociatedObject.MouseUp += AssociatedObject_MouseUp;
            
        }
        
        #region Управление прямоугольниками мышкой
        //Заканчиваем перетаскивание
        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = false;
        }

        // Если перетаскивание в процессе, продолжаем
        // Иначе показываем обычный курсор
        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas can = (sender as Canvas);
            var source = e.OriginalSource;
            if (source.GetType() == typeof(Rectangle))
            {
                Rectangle r = ((Rectangle)source);
                if (DropDownRectangle == null || r != DropDownRectangle)
                {
                    r.Fill = new SolidColorBrush(Color.FromArgb(60, 255, 120, 120));
                    r.Effect = new BlurEffect
                    {
                        Radius = 10
                    };
                }
                r.MouseLeave += (s1, e1) =>
                {
                    if (DropDownRectangle == null || r != DropDownRectangle)
                    {
                        r.Fill = new SolidColorBrush(Color.FromArgb(60, 181, 241, 108));
                        r.Effect = null;
                    }
                };

            }
            can.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!DragInProgress)
                {
                    if (DropDownRectangle != null)
                    {
                        MouseHitType = SetHitType(DropDownRectangle, Mouse.GetPosition(can));
                        SetMouseCursor();
                    }
                }
                else
                {
                    if (DropDownRectangle != null)
                    {
                        // See how much the mouse has moved.
                        System.Windows.Point point = Mouse.GetPosition(can);
                        double offset_x = point.X - LastPoint.X;
                        double offset_y = point.Y - LastPoint.Y;

                        // Получение текущей позиции прямоугольника
                        double new_x = Canvas.GetLeft(DropDownRectangle);
                        double new_y = Canvas.GetTop(DropDownRectangle);
                        double new_width = DropDownRectangle.Width;
                        double new_height = DropDownRectangle.Height;

                        // обновление прямоугольника
                        switch (MouseHitType)
                        {
                            case HitType.Body:
                                new_x += offset_x;
                                new_y += offset_y;
                                break;
                            case HitType.UL:
                                new_x += offset_x;
                                new_y += offset_y;
                                new_width -= offset_x;
                                new_height -= offset_y;
                                break;
                            case HitType.UR:
                                new_y += offset_y;
                                new_width += offset_x;
                                new_height -= offset_y;
                                break;
                            case HitType.LR:
                                new_width += offset_x;
                                new_height += offset_y;
                                break;
                            case HitType.LL:
                                new_x += offset_x;
                                new_width -= offset_x;
                                new_height += offset_y;
                                break;
                            case HitType.L:
                                new_x += offset_x;
                                new_width -= offset_x;
                                break;
                            case HitType.R:
                                new_width += offset_x;
                                break;
                            case HitType.B:
                                new_height += offset_y;
                                break;
                            case HitType.T:
                                new_y += offset_y;
                                new_height -= offset_y;
                                break;
                        }

                        // Используем только положительные длинну и ширину
                        if ((new_width > 0) && (new_height > 0))
                        {
                            // Обновляем прямоугольники
                            Canvas.SetLeft(DropDownRectangle, new_x);
                            Canvas.SetTop(DropDownRectangle, new_y);
                            DropDownRectangle.Width = new_width;
                            DropDownRectangle.Height = new_height;

                            // Сохраняем текущую позицию мышки
                            LastPoint = point;
                        }
                    }
                }
            }));
        }


        /// <summary>
        /// Начинаем перетаскивать
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas can = (sender as Canvas);
            can.Dispatcher.BeginInvoke(new Action(() =>
            {
                var source = e.OriginalSource;
                if (source.GetType() == typeof(Rectangle))
                {
                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        can.Children.Remove((System.Windows.UIElement)source);
                    }
                    else
                    {
                        if (DropDownRectangle != null)
                        {
                            DropDownRectangle.Fill = new SolidColorBrush(Color.FromArgb(60, 181, 241, 108));
                        }
                        DropDownRectangle = source as Rectangle;
                        if (DropDownRectangle != null)
                        {
                            ((Rectangle)source).Fill = new SolidColorBrush(Color.FromArgb(60, 14, 232, 33));
                            ((Rectangle)source).Effect = null;
                            MouseHitType = SetHitType(DropDownRectangle, Mouse.GetPosition(can));
                            SetMouseCursor();
                            if (MouseHitType == HitType.None) return;
                            LastPoint = Mouse.GetPosition(can);
                            DragInProgress = true;
                            if(e.ClickCount == 2)
                            {
                                PdfViewer data = can.DataContext as PdfViewer;
                                data.RectangleForRecognition = ((Rectangle)source);
                                Mouse.OverrideCursor = null;
                            }
                            
                        }
                    }
                }

            }));
        }
        // True if a drag is in progress.
        private bool DragInProgress = false;

        // The drag's last point.
        private System.Windows.Point LastPoint;

        // The part of the rectangle under the mouse.
        HitType MouseHitType = HitType.None;
        Rectangle DropDownRectangle { get; set; }

        // Return a HitType value to indicate what is at the point.
        private HitType SetHitType(System.Windows.Shapes.Rectangle rect, System.Windows.Point point)
        {
            double left = Canvas.GetLeft(rect);
            double top = Canvas.GetTop(rect);
            double right = left + rect.Width;
            double bottom = top + rect.Height;
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP) return HitType.UL;
                if (bottom - point.Y < GAP) return HitType.LL;
                return HitType.L;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP) return HitType.UR;
                if (bottom - point.Y < GAP) return HitType.LR;
                return HitType.R;
            }
            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }

        // Устанавливаем курсор мыши в соотвествии с текущей операцией
        private void SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = null;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = null;
                    break;
                case HitType.Body:
                    if (double.IsNaN(DropDownRectangle.Height) | double.IsNaN(DropDownRectangle.Width))
                    {
                        desired_cursor = null;
                    }
                    else
                    {
                        desired_cursor = Cursors.ScrollAll;
                    }
                    break;
                case HitType.UL:
                case HitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (Mouse.OverrideCursor != desired_cursor) Mouse.OverrideCursor = desired_cursor;
        }

        #endregion

        #region Свойства зависимости
        [Bindable(true)]
        public bool DocumentIsEdit
        {
            get { return (bool)GetValue(DocumentIsEditProperty); }
            set { SetValue(DocumentIsEditProperty, value); }
        }

        public static readonly DependencyProperty DocumentIsEditProperty
          = DependencyProperty.Register("DocumentIsEdit",
              typeof(bool),
              typeof(CanvasBehavior),
              new FrameworkPropertyMetadata(false, new PropertyChangedCallback(DocumentIsEditChanged)));

        private static void DocumentIsEditChanged(DependencyObject dep, DependencyPropertyChangedEventArgs e)
        {
            var item = (CanvasBehavior)dep;
            var newitem = e.NewValue;
            var olditem = e.OldValue;
            if (newitem != null)
            {
                item.DropDownRectangle = null;
            }
        }
        #endregion


    }
}
