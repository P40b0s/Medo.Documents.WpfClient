using Medo.Client.SedRegistration;
using System;
using System.Collections.Generic;
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

namespace SedRegistrationTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SedOperations reg;
        public MainWindow()
        {
            InitializeComponent();
            init();
        }

        private async void init()
        {
            try
            {
                SedModel sm = new SedModel();
                reg = new SedOperations();
                // reg.ChangeSedComment(new Guid("6D8C1EF5-A5EA-4DD9-A97D-5EE80F0663B1"), "310-р");

              
                sm.SGuid = new Guid("7C960FC6-2745-4655-80EB-FED1A7905325");
                sm.DocGuid = new Guid("E7241D3E-AA36-45D4-B4F7-24934F0A69AD");
                sm.DocumentName = "Тестовая регистрация";
                sm.Number = "619/17-НПА";
                sm.PagesCount = 44;
                sm.SignDate = DateTime.Now;
                sm.RefuseStatus = "Тестовое отклонение документа, документ будет опубликован";
                sm.RefuseComment = "ЗДЕСЬ МОЖЕТ БЫТЬ КОМЕНТАРИЙ";
                sm.Operation = SedOperationEnum.Refuse;
                await reg.Register(sm);               
            }
            catch (System.Exception ex)
            {
                string s = ex.Message;
            }
        }
    }
}
