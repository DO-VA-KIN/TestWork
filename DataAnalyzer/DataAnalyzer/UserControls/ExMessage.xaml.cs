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

namespace DataAnalyzer.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ExMessage.xaml
    /// </summary>
    public partial class ExMessage : Window
    {
        private static Exception ThisException { get; set; }

        public ExMessage(Exception exception)
        {
            InitializeComponent();
            ThisException = exception;
        }

        private void WMessage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ThisException == null)
                TBText.Text = "Нет информации";
            else TBText.Text = ThisException.Message;
        }


        private void TVTree_ItemSelected(object sender, RoutedEventArgs e)
        {
            if (ThisException == null) return;

            TreeViewItem item = sender as TreeViewItem;
            switch (item.Header)
            {
                case "HelpLink":
                    TBText.Text = ThisException.HelpLink;
                    break;
                case "HResult":
                    TBText.Text = ThisException.HResult.ToString();
                    break;
                case "Message":
                    TBText.Text = ThisException.Message;
                    break;
                case "Source":
                    TBText.Text = ThisException.Source;
                    break;
                case "StackTrace":
                    TBText.Text = ThisException.StackTrace;
                    break;
            }
        }
    }
}
