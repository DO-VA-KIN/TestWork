using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using API;


namespace DataAnalyzer
{
    public class BackProcess
    {
        public BackProcess(UIProcess statusBar, DataGrid dgFrames, Button btnDataFile)
        {
            StatusBar = statusBar;
            DGFrames = dgFrames;
            BtnDataFile = btnDataFile;
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Worker.DoWork += Worker_DoWork;
            Worker.ProgressChanged += Worker_ProgressChanged;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private static bool IsStop { get; set; } = false;
        private static BackgroundWorker Worker = new BackgroundWorker();
        private static string WayFile { get; set; }

        public Button BtnDataFile { get; set; }
        public DataGrid DGFrames { get; set; }
        public UIProcess StatusBar { get; set; }


        private static Exception LastException = new Exception();
        public static Exception GetException()
        { return LastException; }

        /// <summary>
        /// Занят ли поток
        /// </summary>
        /// <returns></returns>
        public static bool IsBusy()
        { return Worker.IsBusy; }

        /// <summary>
        /// Прерывание работы потока
        /// </summary>
        public static void CancelAsync()
        { Worker.CancelAsync(); IsStop = true; }

        /// <summary>
        /// Начало проверки
        /// </summary>
        /// <returns></returns>
        public bool StartWork(string wayFile)
        {
            if (IsBusy())
            {
                LastException = new Exception("Процесс уже запущен.");
                return false;
            }
            WayFile = wayFile;
            IsStop = false;
            Worker.RunWorkerAsync();
            BtnDataFile.Content = "Прервать";
            return true;
        }



        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            StatusBar.Start();
            FrameAnalize.WayFile = WayFile;
            FrameAnalize.Back = Worker;
            FrameAnalize.Analize();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StatusBar.Update(e.ProgressPercentage, (long)e.UserState);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusBar.Stop(IsStop);
            BtnDataFile.Content = "Данные";
            TableFill();
        }

        private void TableFill()
        {
            Dictionary<string, FrameAnalize.Report> r = FrameAnalize.GetErrors();

            var t = r.Select(x => new
            {
                Название = x.Key,
                Количество = x.Value.FrameCount,
                Счётчик = x.Value.ErrorNumberCount,
                CRC = x.Value.ErrorCRCCount
            });

            DGFrames.HeadersVisibility = DataGridHeadersVisibility.All;
            DGFrames.ItemsSource = t;


            DGFrames.Columns[0].Header = "Название кадра";
            DGFrames.Columns[1].Header = "Количество кадров";
            DGFrames.Columns[2].Header = "Ошибок нумерации";
            DGFrames.Columns[3].Header = "Ошибок CRC";
        }

    }
}
