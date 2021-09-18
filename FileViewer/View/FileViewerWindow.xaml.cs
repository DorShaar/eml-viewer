using System.Windows;
using System.Windows.Controls;

namespace FileViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileViewerWindow : Window
    {
        public FileViewerWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            ContextMenu rightClickOnTreeViewItemMenu = this.FindResource("SaveMenu") as ContextMenu;
            viewModel.ShareProperties(mTreeView, mTextBoxContent, mTextBoxView, ImageGrid, mListView, rightClickOnTreeViewItemMenu);
            this.DataContext = viewModel;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
