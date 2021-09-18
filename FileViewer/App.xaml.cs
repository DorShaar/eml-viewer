using System;
using System.Windows;

namespace FileViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                MainViewModel viewModel = new MainViewModel();
                Current.MainWindow = new FileViewerWindow(viewModel);
                Current.MainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error message");
            }
        }
    }
}
