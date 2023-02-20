using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using API;

namespace DataAnalyzer
{
    public class BackProcess
    {
        public BackProcess(UIProcess statusBar)
        {
            StatusBar = statusBar;
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Worker.DoWork += Worker_DoWork;
            Worker.ProgressChanged += Worker_ProgressChanged;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        public static UIProcess StatusBar { get; set; }
        public static BackgroundWorker Worker = new BackgroundWorker();
        public static bool BWCancel = false;
        public static string WayFile { get; set; }

        private Exception LastException = new Exception();
        public Exception GetException()
        { return LastException; }

        /// <summary>
        /// Занят ли поток
        /// </summary>
        /// <returns></returns>
        public bool IsBusy()
        {
            return Worker.IsBusy;
        }

        public bool StartWork()
        {
            if (IsBusy())
            {
                LastException = new Exception("Процесс уже запущен.");
                return false;
            }

            Worker.RunWorkerAsync();
            return true;
        }



        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            StatusBar.Start(new FileInfo(WayFile).Length);
            FrameAnalize.WayFile = WayFile;
            FrameAnalize.Back = Worker;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }



    }
}
