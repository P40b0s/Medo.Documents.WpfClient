using NLog;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Shapes;
using System.Linq;
using Medo.Core.Models;
using System.Windows;
using System.Collections.Generic;

namespace Medo.Modules.PdfViewerModule.Behaviors
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
            base.AssociatedObject.MouseWheel -= AssociatedObject_MouseWheel;

        }
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            base.AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            base.AssociatedObject.MouseUp += AssociatedObject_MouseUp;
            base.AssociatedObject.MouseWheel += AssociatedObject_MouseWheel;

        }

        private void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DropDownRectangle != null)
            {
                if (e.Delta > 0)
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        DropDownRectangle.ControlRectangleWidth = DropDownRectangle.ControlRectangleWidth + 2;
                    }
                    else
                    {
                        DropDownRectangle.ControlRectangleHeight = DropDownRectangle.ControlRectangleHeight + 2;
                    }
                }
                else
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        DropDownRectangle.ControlRectangleWidth = DropDownRectangle.ControlRectangleWidth - 2;
                    }
                    else
                    {
                        DropDownRectangle.ControlRectangleHeight = DropDownRectangle.ControlRectangleHeight - 2;
                    }
                }
            }
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
            Point p = Mouse.GetPosition(can);
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
                        double new_x = DropDownRectangle.ControlXCoords;
                        double new_y = DropDownRectangle.ControlYCoords + RectanglesCoordinates.correction;
                        double new_width = DropDownRectangle.ControlRectangleWidth;
                        double new_height = DropDownRectangle.ControlRectangleHeight;

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
                        if ((new_width > 0) && (new_height > 0) && (new_x > 0) && (new_y > 0))
                        {

                            // Обновляем прямоугольники
                            DropDownRectangle.ControlXCoords = new_x;
                            DropDownRectangle.ControlYCoords = new_y - RectanglesCoordinates.correction;
                            DropDownRectangle.ControlRectangleWidth = new_width;
                            DropDownRectangle.ControlRectangleHeight = new_height;

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
                    DropDownRectangle = (RectanglesCoordinates)(source as Rectangle).DataContext;
                    if (DropDownRectangle != null)
                    {
                        //Меняем ZIndex у данного итема они почему то идут вверх по мере добавления в коллекцию RectanglesCollection
                        //уже ненадо - починил все поменяв add на insert при добавлении в коллекцию
                        //var datacontext = ((TextBlocksSearch)can.DataContext);
                        //var item = ((RectanglesCoordinates)((Rectangle)source).DataContext);
                        //int oldIndex = datacontext.RectanglesCollection.IndexOf(item);
                        //datacontext.RectanglesCollection.Move(oldIndex, 0);

                        MouseHitType = SetHitType(DropDownRectangle, Mouse.GetPosition(can));
                        SetMouseCursor();
                        if (MouseHitType == HitType.None) return;
                        LastPoint = Mouse.GetPosition(can);
                        DragInProgress = true;
                    }
                }
                else
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        var t = (TextBlocksSearch)can.DataContext;
                        if (LastPoint != null)
                        {
                            LastPoint = Mouse.GetPosition(can);
                            DropDownRectangle = t.AddLayerBlock(LastPoint.X, LastPoint.Y, 0, 0);
                            MouseHitType = SetHitType(DropDownRectangle, Mouse.GetPosition(can));
                            SetMouseCursor();
                            if (MouseHitType == HitType.None) return;
                            LastPoint = Mouse.GetPosition(can);
                            DragInProgress = true;
                        }
                    }
                    else
                    {
                        DropDownRectangle = null;
                        Mouse.OverrideCursor = null;
                    }
                }

            }));
        }
        // true если процесс перетаскивания активен
        private bool DragInProgress = false;

        // Пследняя позиция перетаскивания
        private System.Windows.Point LastPoint;

        // The part of the rectangle under the mouse.
        HitType MouseHitType = HitType.None;
        RectanglesCoordinates DropDownRectangle { get; set; }

        // Return a HitType value to indicate what is at the point.
        private HitType SetHitType(RectanglesCoordinates rect, System.Windows.Point point)
        {
            double left = rect.ControlXCoords;
            double top = rect.ControlYCoords + RectanglesCoordinates.correction;
            double right = left + rect.ControlRectangleWidth;
            double bottom = top + rect.ControlRectangleHeight;
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
            // Какой курсор отображать
            Cursor desired_cursor = null;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = null;
                    break;
                case HitType.Body:
                    if (double.IsNaN(DropDownRectangle.ControlRectangleHeight) | double.IsNaN(DropDownRectangle.ControlRectangleWidth))
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

            // Отображаем курсор
            if (Mouse.OverrideCursor != desired_cursor) Mouse.OverrideCursor = desired_cursor;
        }

        #endregion
    }
}
