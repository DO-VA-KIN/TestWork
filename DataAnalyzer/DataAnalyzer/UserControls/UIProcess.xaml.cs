using System;
using System.Collections.Generic;
using System.Data;
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

namespace DataAnalyzer
{
    /// <summary>
    /// Логика взаимодействия для UIProcess.xaml
    /// </summary>
    public partial class UIProcess : UserControl
    {

        private System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        static TimeSpan TimerInterval = new TimeSpan(0, 0, 1);

        private TimeSpan TSTimeOver = new TimeSpan(0, 0, 0);
        long Position = 0;
        long Size = 0;


        public UIProcess()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Timer.Interval = TimerInterval;
            Timer.Tick += Timer_Tick;
        }






        private void Timer_Tick(object sender, EventArgs e)
        {
            TSTimeOver += TimerInterval;
            LbTimeOver.Content = TSTimeOver;
        }


        public void Start(long size)
        {
            Size = size;
            Position = 0;
            TSTimeOver = new TimeSpan(0, 0, 0);
            Timer.Start();
        }


    }




}
