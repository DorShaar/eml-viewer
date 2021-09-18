using Factory;
using Microsoft.Win32;
using Parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Utilities;

namespace FileViewer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private TreeView mTreeView;
        private TextBox mTextBoxContent;
        private TextBox mTextBoxView;
        private Grid mImageGrid;
        private ListView mListView;
        private ContextMenu mRrightClickOnTreeViewItemMenu;

        private Image mDisplayImage;

        private ParsableHandlers mHandlers;
        private IParsable mFile;
        private IParsable mBackupFile;

        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand SaveItemCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel()
        {
            // Subscribing to mediator.
            Mediator.GetIntance().TreeViewItemLeftClicked += MainWindow_TreeViewItemLeftClicked;

            OpenFileCommand = new DelegateCommand(OpenFile);
            SaveFileCommand = new DelegateCommand(SaveFile);
            SaveItemCommand = new DelegateCommand(() => SaveItem());
        }

        public void ShareProperties(TreeView treeView, TextBox textBoxContent, TextBox textBoxView, Grid imageGrid, ListView listView, ContextMenu rightClickOnTreeViewItemMenu)
        {
            mTreeView = treeView;
            mTextBoxContent = textBoxContent;
            mTextBoxView = textBoxView;
            mImageGrid = imageGrid;
            mListView = listView;
            mRrightClickOnTreeViewItemMenu = rightClickOnTreeViewItemMenu;

            SetControlsProperties();
        }

        private void SetControlsProperties()
        {
            mTextBoxView.IsReadOnly = true;

            mDisplayImage = new Image()
            {
                Stretch = System.Windows.Media.Stretch.Fill,
                Visibility = Visibility.Visible
            };
        }

        private void MTextBoxContent_KeyUp(object sender, KeyEventArgs e)
        {
            if (mTreeView.SelectedItem is TreeViewItem treeViewItem)
            {
                if (treeViewItem.Tag is ILeafNode leafNode && !string.IsNullOrEmpty(mTextBoxContent.Text))
                    leafNode.Text = mTextBoxContent.Text;
            }
        }

        private async Task OpenFile()
        {
            mTreeView.Items.Clear();

            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    if (!UpdateReaders(fileDialog.FileName))
                        return;

                    TreeViewBuilder treeViewBuilder = new TreeViewBuilder(mTreeView);

                    mFile = mHandlers.Parser.Parse(fileDialog.FileName);
                    mFile.FilePath = fileDialog.FileName;

                    mBackupFile = mFile.Clone();
                    mBackupFile.FilePath = CreateNewFilePathWithPrefix(mBackupFile.FilePath, "backup");

                    treeViewBuilder.UpdateTreeView(
                        mFile as ITreeNode,
                        mFile.FilePath,
                        mRrightClickOnTreeViewItemMenu);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Could not open file");
                }
            }
        }

        private bool UpdateReaders(string filePath)
        {
            mHandlers = new ParserFactory().GetProduct(filePath);
            return mHandlers != null;
        }

        /// <summary>
        /// Saves the file to a chosen path. If the file was changed, saves the back up.
        /// </summary>
        /// <returns></returns>
        private async Task SaveFile()
        {
            if (mFile != null && mFile.FilePath != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = CreateNewFilePathWithPrefix(mFile.FilePath, "resaved")
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string directorySavedFile =
                        await Task<string>.Factory.StartNew(() => new FileSaver().Save(mFile, saveFileDialog.FileName));

                    // Opens output file directory.
                    System.Diagnostics.Process.Start(directorySavedFile);

                    MessageBox.Show($"{mFile.FilePath} saved", "Save success");
                }
            }
            else
            {
                MessageBox.Show("No file to save", "Save failed");
            }
        }

        private string CreateNewFilePathWithPrefix(string fileName, string prefix)
        {
            string directoryPath = Path.GetDirectoryName(fileName);
            string currentFileName = Path.GetFileName(fileName);
            string newFileName = prefix + "_" + currentFileName;

            return Path.Combine(directoryPath, currentFileName, newFileName);
        }

        private void SaveItem()
        {
            if (mTreeView.SelectedItem is TreeViewItem treeViewItem)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = mHandlers.DataExtractor.GetName(treeViewItem.Tag)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    mHandlers.DataExtractor.Extract(treeViewItem.Tag, saveFileDialog.FileName);
                    MessageBox.Show($"{saveFileDialog.FileName} saved", "Save succeed");
                }
            }
        }

        private void MainWindow_TreeViewItemLeftClicked(object sender, TreeViewItemClickedEventArgs e)
        {
            UpdateTextBoxContent(e.TreeViewItem.Tag);
            UpdateTextBoxView(e.TreeViewItem.Tag);
            UpdateListView(e.TreeViewItem.Tag);
            UpdateImageGrid(e.TreeViewItem.Tag);
        }

        private void UpdateTextBoxContent(object item)
        {
            if (item is ILeafNode leafNode)
                mTextBoxContent.Text = leafNode.Text;
            else if (item is ITreeNode)
                mTextBoxContent.Text = string.Empty;
        }

        private void UpdateTextBoxView(object item)
        {
            if (item is ILeafNode leafNode)
                mTextBoxView.Text = mHandlers.TextEncoderDecoder.Decode(leafNode);
            else if (item is ITreeNode)
                mTextBoxView.Text = string.Empty;
        }

        /// <summary>
        /// Updates list view.
        /// Key and Value are data binded in the xml. Those property names are important.
        /// </summary>
        private void UpdateListView(object item)
        {
            mListView.Items.Clear();
            if (item is ITreeNode node)
            {
                foreach (KeyValuePair<string, List<string>> header in node.KeyValuePair)
                {
                    foreach (string value in header.Value)
                    {
                        mListView.Items.Add(new
                        {
                            header.Key,
                            value
                        });
                    }
                }
            }
        }

        private void UpdateImageGrid(object item)
        {
            mImageGrid.Children.Remove(mDisplayImage);

            if (item is ILeafNode leafNode)
            {
                if (leafNode.Name.Contains("image"))
                    HandleImageInTextBoxView(leafNode);
            }
        }

        private void HandleImageInTextBoxView(ILeafNode leafNode)
        {
            using (Stream imageStream = new MemoryStream(Convert.FromBase64String(leafNode.Text)))
            {
                mDisplayImage.Source = BitmapFrame.Create(imageStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                mImageGrid.Children.Add(mDisplayImage);
            }
        }

        private void Test()
        {
            // TODO changed
            ListBox listBox = new ListBox();
            listBox.Items.Add(new object());

            ListBoxItem listBoxItem = new ListBoxItem();
        }

        private void AddLeafNode(TreeViewItem treeViewItem)
        {
            // TODO changed.
            //ILeafNode leafNode = new EmlLeafNode

            TreeViewItem newItem = new TreeViewItem();
            //newItem.Tag = 

            treeViewItem.Items.Add(newItem);
        }
    }
}