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
    public class ImageBehavior : Behavior<Image>
    {

        readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;

        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var img = (Image)sender;
            StaticProperty.DocumentControlHeight = img.ActualHeight;
            StaticProperty.DocumentControlWidth = img.ActualWidth;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;

        }
    }
}
