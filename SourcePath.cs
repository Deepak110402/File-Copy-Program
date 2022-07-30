using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileCopyProgram
{
    /// <summary>
    /// class <c>SourcePath</c> contains all column details of data grid
    /// </summary>
    public class SourcePath : INotifyPropertyChanged
    {
        private string path;
        
        public string SourceFilePath
        {
            get { return path; }
            set { path = value; OnPropertyChanged(); }
        }

        public string PassInfo { get; set; }

        public string ReasonToFail { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
