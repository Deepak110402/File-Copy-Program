using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace FileCopyProgram
{
    /// <summary>
    /// Interaction logic for ResultBox.xaml
    /// </summary>
    public partial class ResultBox : Window
    {
        readonly DateTime startTime;
        readonly BackgroundWorker work = new BackgroundWorker
        {
            WorkerReportsProgress = true
        };
        private readonly string destination;
        private readonly List<string> sourceFiles;
        private readonly bool overWrite;
        private readonly ObservableCollection<SourcePath> resultList = new ObservableCollection<SourcePath>();

        /// <summary>
        /// Constructor <c>ResultBox</c> initialize the components of xml file
        /// </summary>
        public ResultBox(string destination, List<string> sourceFiles, bool overWrite)
        {
            InitializeComponent();
            this.destination = destination;
            this.sourceFiles = sourceFiles;
            this.overWrite = overWrite;
            startTime = DateTime.Now;
            work.DoWork += WorkDoWork;
            work.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorkerProgressChanged);
            work.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorkerRunWorkerCompleted);
            work.RunWorkerAsync();
        }

        /// <summary>
        /// Mehtod <c>BackgroundWorkerRunWorkerCompleted</c> performs action after completing copying process
        /// </summary>
        /// <param name="sender">object details of button</param>
        /// <param name="e">Run worker completd event arguments</param>
        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
            FinalResult finalResult = new FinalResult(resultList);
            finalResult.ShowDialog();
        }

        /// <summary>
        /// Method <c>BackgroundWorkerProgressChanged</c> called by report progress 
        /// </summary>
        /// <param name="sender">object details of button</param>
        /// <param name="e">Process changed event arguments</param>
        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
            Percentage.Text = e.ProgressPercentage.ToString() + Constants.Percentage;
            TimeSpan timeSpan = DateTime.Now.Subtract(startTime);
            TimeElasped.Text = String.Format(Constants.Time, timeSpan.Hours,timeSpan.Minutes,timeSpan.Seconds);
        }

        /// <summary>
        /// Method <c>WorkDoWork</c> perform copy and append in destination file
        /// </summary>
        /// <param name="sender">object details of button</param>
        /// <param name="e">Do Work Event arguments</param>
        private void WorkDoWork(object sender, DoWorkEventArgs e)
        {
            long length = 0, currentFileLength = 0;
            long totalLength = GetLength();
            foreach (var item in sourceFiles)
            {
                if (overWrite)
                {
                    try
                    {
                        FileStream fileStream = new FileStream(item, FileMode.Open);
                        currentFileLength = fileStream.Length;
                        fileStream.Close();
                        if (File.Exists(Path.Combine(destination, Path.GetFileName(item))))
                        {
                            File.Delete(Path.Combine(destination, Path.GetFileName(item)));
                        }
                        length = CopyPaste(item, length, totalLength);
                    }
                    catch
                    {
                        resultList.Add(new SourcePath() { SourceFilePath = item, PassInfo = Constants.Failed, ReasonToFail = Constants.ReadOnly });
                        ContinueProgressBar(item, length, totalLength, currentFileLength);
                    }
                }
                else
                {
                    try
                    {
                        FileStream fileStream = new FileStream(item, FileMode.Open);
                        currentFileLength = fileStream.Length;
                        fileStream.Close();
                        if (!File.Exists(Path.Combine(destination, Path.GetFileName(item))))
                        {
                            length = CopyPaste(item, length, totalLength);
                        }
                        else
                        {
                            resultList.Add(new SourcePath() { SourceFilePath = item, PassInfo = Constants.Failed, ReasonToFail = Constants.NonOverwriteError });
                            ContinueProgressBar(item, length, totalLength, currentFileLength);
                        }
                    }
                    catch
                    {
                        resultList.Add(new SourcePath() { SourceFilePath = item, PassInfo = Constants.Failed, ReasonToFail = Constants.ReadOnly });
                    }
                }
            }
        }

        /// <summary>
        /// Method <c>CopyPaste</c> copy and paste file form one to another
        /// </summary>
        /// <param name="item">source file path</param>
        long CopyPaste(string item, long length, long totalLength)
        {
            try
            {
                FileStream fileStreamIn = new FileStream(item, FileMode.Open);
                FileStream fileStreamOut = new FileStream(Path.Combine(destination, Path.GetFileName(item)), FileMode.CreateNew);
                byte[] buffer = new byte[1024 * 1024];
                int readByte;
                while ((readByte = fileStreamIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStreamOut.Write(buffer, 0, readByte);
                    work.ReportProgress((int)((length + fileStreamIn.Position) * 100 / totalLength));
                    this.Dispatcher.Invoke(DispatcherPriority.Send, (Action)(() =>
                    {
                        TimeSpan timeSpan = TimeSpan.FromTicks(DateTime.Now.Subtract(startTime).Ticks * (totalLength - (length + fileStreamIn.Position)) / (length + fileStreamIn.Position));
                        TimeRemaining.Text = String.Format(Constants.Time, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                        FileName.Text = item;
                    }));
                }
                length += fileStreamIn.Length;
                fileStreamIn.Close();
                fileStreamOut.Close();
                resultList.Add(new SourcePath() { SourceFilePath = item, PassInfo = Constants.Passed, ReasonToFail = String.Empty });
            }
            catch
            {
                resultList.Add(new SourcePath() { SourceFilePath = item, PassInfo = Constants.Failed, ReasonToFail = Constants.ReadOnly });
            }
            return length;
        }

        /// <summary>
        /// Method <c>GetLength</c> returns the length of all files
        /// </summary>
        /// <returns>returns the length of all files</returns>
        long GetLength()
        {
            long length = 0;
            List<string> errorFileList = new List<string>();
            foreach (var item in sourceFiles)
            {
                try
                {
                    FileStream fileStream = new FileStream(item, FileMode.Open);
                    length += fileStream.Length;
                    fileStream.Close();
                }
                catch
                {
                    errorFileList.Add(item);
                    resultList.Add(new SourcePath() { SourceFilePath = item, PassInfo = Constants.Failed, ReasonToFail = Constants.ReadOnly });
                }
            }
            if (errorFileList.Count > 0)
            {
                sourceFiles.RemoveAll(item => errorFileList.Contains(item));
            }
            return length;
        }

        /// <summary>
        /// Method <c>ContinueProgressBar</c> continue progress bar after a exption
        /// </summary>
        /// <param name="item">path of source file</param>
        /// <param name="length">length</param>
        /// <param name="totalLength">length of all files</param>
        void ContinueProgressBar(string item, long length, long totalLength, long currentFileLength)
        {
            work.ReportProgress((int)((length + currentFileLength) * 100 / totalLength));
            this.Dispatcher.Invoke(DispatcherPriority.Send, (Action)(() =>
            {
                TimeSpan timeSpan = TimeSpan.FromTicks(DateTime.Now.Subtract(startTime).Ticks * (totalLength - (length + currentFileLength)) / (length + currentFileLength));
                TimeRemaining.Text = String.Format(Constants.Time, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                FileName.Text = item;
            }));
        }
    }
}
