using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
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
using ICSharpCode.AvalonEdit.Document;

namespace AvallonEditTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<DateTime> dt = new List<DateTime>();

        public MainWindow()
        {
            InitializeComponent();
            editor.TextArea.TextView.LineTransformers.Add(new ColorizeAvalonEdit());
            editor.Text = "Об утверждении Порядка расчета нормативных затрат на обеспечение функций Федеральной службы исполнения наказаний, территориальных органов Федеральной службы исполнения наказаний и федеральных казенных учреждений уголовно-исполнительной системы в части приобретения вычислительной техники, периферийных устройств к ней, персональных электронно-вычислительных машин в защищенном исполнении, средств связи, телекоммуникационного и климатического оборудования, а также нормативов количества и цены закупаемых товаров";
            dt.Add(DateTime.Now.AddDays(1));
            dt.Add(DateTime.Now.AddDays(3));
            dt.Add(DateTime.Now.AddDays(5));
            dt.Add(DateTime.Now.AddDays(2));
            dt.Add(DateTime.Now.AddDays(4));
            dt.Add(DateTime.Now.AddDays(8));
            BinarySorter<DateTime> bs = new BinarySorter<DateTime>();

        }
    }
}
