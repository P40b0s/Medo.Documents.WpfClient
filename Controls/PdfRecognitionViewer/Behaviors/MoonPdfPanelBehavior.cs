using MoonPdfLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;

namespace Medo.Controls.PdfRecognitionViewer.Behaviors
{
    class MoonPdfPanelBehavior : Behavior<MoonPdfPanel>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            base.AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
        }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MoonPdfPanel pdf = (sender as MoonPdfPanel);
            pdf.Dispatcher.BeginInvoke(new Action(() =>
            {
                var data = pdf.DataContext as PdfViewer;
                double x = e.GetPosition(pdf).X;
                double y = e.GetPosition(pdf).Y;
                if (x<0 || x>pdf.ActualWidth || y<0 || y > pdf.ActualHeight)
                data.ToolButtonsIsVisible = false;
            }));
        }

        private void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MoonPdfPanel pdf = (sender as MoonPdfPanel);
            pdf.Dispatcher.BeginInvoke(new Action(() =>
            {
                var data = pdf.DataContext as PdfViewer;
                data.ToolButtonsIsVisible = true;
            }));
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
            base.AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }
    }
}
