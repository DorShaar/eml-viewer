using System;
using System.Windows.Controls;

namespace Utilities
{
    public class Mediator
    {
        private static readonly Mediator mInstance = new Mediator();
        private Mediator() { }

        public static Mediator GetIntance()
        {
            return mInstance;
        }

        public event EventHandler<TreeViewItemClickedEventArgs> TreeViewItemLeftClicked;

        public void OnTreeViewItemLeftClicked(object sender, TreeViewItem treeViewItem)
        {
            if (TreeViewItemLeftClicked is EventHandler<TreeViewItemClickedEventArgs> itemViewClickedDelegate)
            {
                itemViewClickedDelegate.Invoke(
                    sender,
                    new TreeViewItemClickedEventArgs
                    {
                        TreeViewItem = treeViewItem
                    });
            }
        }
    }
}
