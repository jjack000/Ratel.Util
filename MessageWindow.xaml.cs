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
    /// MessageWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageWindow : Window
    {
        MessageBoxResult result = MessageBoxResult.None;
        public MessageBoxResult Result
        {
            get
            {
                return result;
            }
        }
        public MessageWindow()
        {
            InitializeComponent();
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
        }

        public static MessageBoxResult Show(string message, string caption = "Message", MessageBoxButton button = MessageBoxButton.OK)
        {
            var msgWindow = new MessageWindow();
            msgWindow.Title = caption;
            switch (button)
            {
                case MessageBoxButton.OK:
                    msgWindow.okButton.Visibility = Visibility.Visible;
                    msgWindow.cancelButton.Visibility = Visibility.Collapsed;
                    msgWindow.yesButton.Visibility = Visibility.Collapsed;
                    msgWindow.noButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    msgWindow.okButton.Visibility = Visibility.Visible;
                    msgWindow.cancelButton.Visibility = Visibility.Visible;
                    msgWindow.yesButton.Visibility = Visibility.Collapsed;
                    msgWindow.noButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    msgWindow.okButton.Visibility = Visibility.Collapsed;
                    msgWindow.cancelButton.Visibility = Visibility.Collapsed;
                    msgWindow.yesButton.Visibility = Visibility.Visible;
                    msgWindow.noButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    msgWindow.okButton.Visibility = Visibility.Collapsed;
                    msgWindow.cancelButton.Visibility = Visibility.Visible;
                    msgWindow.yesButton.Visibility = Visibility.Visible;
                    msgWindow.noButton.Visibility = Visibility.Visible;
                    break;
            }
            msgWindow.messageText.Text = message;

            msgWindow.ShowDialog();
            return msgWindow.Result;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Yes;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.No;
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Cancel;
            Close();
        }
    }
}
