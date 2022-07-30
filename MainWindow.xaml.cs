using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileCopyProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<SourcePath> sourceFiles = new ObservableCollection<SourcePath>();
        private ICommand sourceFileCommand;
        private ICommand addCommand;
        private ICommand deleteCommand;
        private ICommand destinationFileCommand;
        private ICommand destinationFileValidationCommand;
        private ICommand copyCommand;
        private ICommand dataGridCommand;


        /// <summary>
        /// Property <c>DataGridCommand</c> creates cammand handler
        /// </summary>
        public ICommand DataGridValidationCommand
        {
            get
            {
                return dataGridCommand ?? (dataGridCommand = new CommandHandler(() => SourcePathValidation(), () => CanExecute()));
            }
        }

        /// <summary>
        /// Property <c>DestinationFileValidationCommand</c> creates cammand handler
        /// </summary>
        public ICommand DestinationFileValidationCommand
        {
            get
            {
                return destinationFileValidationCommand ?? (destinationFileValidationCommand = new CommandHandler(() => DestinationPathValidation(), () => CanExecute()));
            }
        }

        /// <summary>
        /// Property <c>DotsClickCommand</c> creates cammand handler
        /// </summary>
        public ICommand SourceFileCommand
        {
            get
            {
                return sourceFileCommand ?? (sourceFileCommand = new CommandHandler(() => SourceDialogBox(), () => CanExecute()));
            }
        }

        /// <summary>
        /// Property <c>AddClickCommand</c> creates cammand handler
        /// </summary>
        public ICommand AddCommand
        {
            get
            {
                return addCommand ?? (addCommand = new CommandHandler(() => AddRow(), () => CanExecute()));
            }
        }

        /// <summary>
        /// Property <c>MinusClickCommand</c> creates cammand handler
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new CommandHandler(() => DeleteRow(), () => CanExecute()));
            }
        }

        /// <summary>
        /// Property <c>DestinationDotsClickCommand</c> creates cammand handler
        /// </summary>
        public ICommand DestinationPathCommand
        {
            get
            {
                return destinationFileCommand ?? (destinationFileCommand = new CommandHandler(() => DestinationDialogBox(), () => CanExecute()));
            }
        }

        /// <summary>
        /// Property <c>CopyClickCommand</c> creates cammand handler
        /// </summary>
        public ICommand CopyCommand
        {
            get
            {
                return copyCommand ?? (copyCommand = new CommandHandler(() => Copy(), () => CanExecute()));
            }
        }

        /// <summary>
        /// Constructor <c>MainWindow</c> initialize the components of xml file
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            sourceFiles.Add(new SourcePath() { SourceFilePath = string.Empty });
            DataGrid.ItemsSource = sourceFiles;
        }

        /// <summary>
        /// Method <c>CanExecute</c> returns true
        /// </summary>
        public bool CanExecute()
        {
            return true;
        }

        /// <summary>
        /// Mehtod <c>SourceDialogBox</c> open dialog box to choose source file path
        /// </summary>
        public void SourceDialogBox()
        {
            OpenFileDialog openDialogBox = new OpenFileDialog();
            if (openDialogBox.ShowDialog() == true)
            {
                if (sourceFiles.Where(item => Path.GetFileName(item.SourceFilePath) == Path.GetFileName(openDialogBox.FileName)).Any())
                {
                    MessageBox.Show(Constants.FileNameError);
                }
                else
                {
                    sourceFiles[DataGrid.SelectedIndex].SourceFilePath = openDialogBox.FileName;
                }
            }
        }

        /// <summary>
        /// Method <c>AddRow</c> Add a another row in data grid
        /// </summary>
        private void AddRow()
        {
            sourceFiles.Add(new SourcePath() { SourceFilePath = string.Empty });
        }

        /// <summary>
        /// Method <c>MinusClick</c> delete a another row in data grid
        /// </summary>
        private void DeleteRow()
        {
            if (sourceFiles.Count > 1)
            {
                sourceFiles.Remove(DataGrid.SelectedItem as SourcePath);
            }
            else
            {
                sourceFiles[DataGrid.SelectedIndex].SourceFilePath = string.Empty;
            }
        }

        /// <summary>
        /// Method <c>DestinationPathDots</c> open dialog box to choose the fileDestinationPath
        /// </summary>
        private void DestinationDialogBox()
        {
            System.Windows.Forms.FolderBrowserDialog openDialogBox = new System.Windows.Forms.FolderBrowserDialog();
            if (openDialogBox.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DestinationPath.Text = openDialogBox.SelectedPath;
            }
        }

        /// <summary>
        /// Method <c>CopyClick</c> calls the do work and work progress changed function
        /// </summary>
        private void Copy()
        {
            if (sourceFiles.Where(item => item.SourceFilePath == string.Empty).Any())
            {
                MessageBox.Show(Constants.SourceFileError);
            }
            else if (DestinationPath.Text == string.Empty)
            {
                MessageBox.Show(Constants.DestinationError);
            }
            else if (sourceFiles.Where(item => Path.GetDirectoryName(item.SourceFilePath) == DestinationPath.Text).Any())
            {
                MessageBox.Show(Constants.DirectorySameError);
            }
            else
            {
                ResultBox result = new ResultBox(DestinationPath.Text, sourceFiles.Select(item => item.SourceFilePath).ToList(), (bool)Yes.IsChecked);
                result.ShowDialog();
                DestinationPath.Text = string.Empty;
                sourceFiles.Clear();
                sourceFiles.Add(new SourcePath() { SourceFilePath = string.Empty });
            }
        }

        /// <summary>
        /// Method <c>DestinationPathValidation</c> validates the destination path 
        /// </summary>
        private void DestinationPathValidation()
        {
            if (!Directory.Exists(DestinationPath.Text))
            {
                MessageBox.Show(Constants.DestinationPathError);
                DestinationPath.Text = string.Empty;
            }
        }

        /// <summary>
        /// Method <c>SourcePathValidation</c> validates the source path 
        /// </summary>
        private void SourcePathValidation()
        {
            if (sourceFiles.Where(item => !File.Exists(item.SourceFilePath)).Any())
            {
                MessageBox.Show(Constants.SourcePathError);
                DestinationPath.Text = string.Empty;
            }
        }
    }
}   