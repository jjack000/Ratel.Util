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
using System.Windows.Shapes;

namespace Ratel.Util
{
    /// <summary>
    /// ErrorWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public string? DetailMessage
        {
            get
            {
                return detailMessageTextBox.Text;
            }
            set
            {
                detailMessageTextBox.Text = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessageTextBox.Text;
            }
            set
            {
                errorMessageTextBox.Text = value;
            }
        }

        public static void Show(string errorMessage, string? detailMessage)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var win = new ErrorWindow();
                win.ErrorMessage = errorMessage;
                win.DetailMessage = detailMessage;
                win.ShowDialog();
            });
        }

        public ErrorWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
