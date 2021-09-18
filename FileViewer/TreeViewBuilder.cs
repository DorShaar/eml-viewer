using Parser;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;

namespace FileViewer
{
    internal class TreeViewBuilder
    {
        public static int DataPosition { get; } = 0;

        private TreeView mTreeView;

        public TreeViewBuilder(TreeView treeView)
        {
            mTreeView = treeView;
        }

        public void UpdateTreeView(ITreeNode rootNode, string rootNodeName, ContextMenu contextMenu)
        {
            // The root.
            TreeViewItem rootTreeItem = CreateTreeViewItem(rootNode, contextMenu);
            rootTreeItem.Header = rootNodeName;

            // Adding the root node.
            mTreeView.Items.Add(rootTreeItem);
        }

        private TreeViewItem CreateTreeViewItem(ITreeNode dataNode, ContextMenu contextMenu)
        {
            TreeViewItem treeViewItemNode = new TreeViewItem
            {
                Header = dataNode.Name
            };

            // Subscribing double click events.
            treeViewItemNode.PreviewMouseLeftButtonDown += TreeViewerItem_MouseLeftButtonDown;

            // Adding the data node into the tree view node.
            treeViewItemNode.Tag = dataNode;

            if (dataNode is ILeafNode)
            {
                // End case.
                treeViewItemNode.ContextMenu = contextMenu;
            }
            else if (dataNode is ITreeNode)
            {
                foreach (ITreeNode childNode in dataNode.ChildNodes)
                {
                    treeViewItemNode.Items.Add(CreateTreeViewItem(childNode, contextMenu));
                }
            }
            else
            {
                throw new ArgumentException($"Node is not {nameof(ILeafNode)} or {nameof(ITreeNode)}");
            }

            return treeViewItemNode;
        }

        private void TreeViewerItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mediator.GetIntance().OnTreeViewItemLeftClicked(this, sender as TreeViewItem);
        }
    }
}
