using System.IO;
using System.Windows;
using System.Windows.Input;
using BeatSaberSongGenerator.Generators;
using BeatSaberSongGenerator.IO;

namespace BeatSaberSongGenerator.ViewModels
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        public MainViewModel()
        {
            GenerateCommand = new RelayCommand(GenerateSong, CanGenerateSong);
        }

        public float SkillLevel { get; set; } = 5;

        private string audioFilePath;
        public string AudioFilePath
        {
            get => audioFilePath;
            set
            {
                if (Path.GetExtension(value)?.ToLowerInvariant() != ".ogg")
                    MessageBox.Show("Song must be an OGG sound file");
                else
                    audioFilePath = value;
                OnPropertyChanged();
            }
        }

        private string coverFilePath;
        public string CoverFilePath
        {
            get => coverFilePath;
            set
            {
                if (Path.GetExtension(value)?.ToLowerInvariant() != ".jpg")
                    MessageBox.Show("Cover must be a JPG-image");
                else
                    coverFilePath = value;
                OnPropertyChanged();
            }
        }


        public ICommand GenerateCommand { get; }

        private bool CanGenerateSong()
        {
            return true;
        }

        private void GenerateSong()
        {
            var songGenerator = new SongGenerator(new SongGeneratorSettings
            {
                SkillLevel = SkillLevel,
            });
            var song = songGenerator.Generate(AudioFilePath, CoverFilePath);
            var songStorer = new SongStorer();
            var outputDirectory = Path.Combine(
                Path.GetDirectoryName(AudioFilePath),
                Path.GetFileNameWithoutExtension(AudioFilePath));
            songStorer.Store(song, outputDirectory);
        }
    }
}
