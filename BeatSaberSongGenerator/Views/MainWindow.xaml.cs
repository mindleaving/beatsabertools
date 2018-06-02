using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BeatSaberSongGenerator.ViewModels;

namespace BeatSaberSongGenerator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", 
            typeof(MainViewModel), typeof(MainWindow), new PropertyMetadata(default(MainViewModel)));

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainViewModel ViewModel
        {
            get { return (MainViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            var textBox = sender as TextBox;
            if(textBox == null)
                return;
            if(!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            if(files.Length != 1)
                return;
            textBox.Text = files.Single();
            e.Effects = DragDropEffects.None;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}
