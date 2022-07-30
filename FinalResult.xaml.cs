using System.Collections.ObjectModel;
using System.Windows;

namespace FileCopyProgram
{
    /// <summary>
    /// Interaction logic for FinalResult.xaml
    /// </summary>
    public partial class FinalResult : Window
    {
        public FinalResult(ObservableCollection<SourcePath> resultList)
        {
            InitializeComponent();
            ResultDataGrid.ItemsSource = resultList;
        }
    }
}