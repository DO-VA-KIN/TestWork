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
        
        private struct SpeedStruct
        {
            public long Position1;
            public long Position2;
            public int TimeSpent;
        }//структура для измерения скорости 
        SpeedStruct Speed;


        public UIProcess()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Timer.Interval = TimerInterval;
            Timer.Tick += Timer_Tick;

            Speed = new SpeedStruct()
            {
                Position1 = 0,
                Position2 = 0,
                TimeSpent = 0,
            };
        }



        private void Timer_Tick(object sender, EventArgs e)
        {
            TSTimeOver += TimerInterval;
            LbTimeOver.Content = TSTimeOver;

            //времени осталось
            if (PBProgress.Value != 0)
            {
                int hours, minutes, seconds = 0;
                ulong timeSec = Convert.ToUInt64(TSTimeOver.TotalSeconds / PBProgress.Value * 100 - TSTimeOver.TotalSeconds);
                hours = Convert.ToInt32(timeSec / 3600);
                timeSec %= 3600;
                minutes = Convert.ToInt32(timeSec / 60);
                timeSec %= 60;
                seconds = Convert.ToInt32(timeSec);
                LbTimeLeft.Content = new TimeSpan(hours, minutes, seconds);
            }

            //скорость
            Speed.TimeSpent += 1;
            long change = Speed.Position1 - Speed.Position2;
            if (change > 0 && Speed.TimeSpent != 0)
            {
                LbSpeedMBS.Content = Math.Round((change / Speed.TimeSpent / Math.Pow(2, 20)), 2);
                Speed.TimeSpent = 0;
                Speed.Position2 = Speed.Position1;
            }
        }


        public void Start()
        {
            TSTimeOver = new TimeSpan(0, 0, 0);
            Timer.Start();
        }

        public void Update(int percentage, long position)
        {
            PBProgress.Value = percentage;
            Speed.Position1 = position;
        }

        public void Stop(bool isCanceled)
        {
            if (!isCanceled)
            {
                PBProgress.Value = PBProgress.Maximum;
                LbTimeLeft.Content = new TimeSpan(0, 0, 0);
            }

            Timer.Stop();
        }

    }




}
