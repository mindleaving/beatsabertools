using System.Windows;
using BeatSaberSongGenerator.ViewModels;
using BeatSaberSongGenerator.Views;

namespace BeatSaberSongGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var mainViewModel = new MainViewModel();
            var mainWindow = new MainWindow
            {
                ViewModel = mainViewModel
            };
            mainWindow.ShowDialog();
        }
    }
}
