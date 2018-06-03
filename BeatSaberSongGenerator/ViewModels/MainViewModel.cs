using System.IO;
using System.Windows;
using System.Windows.Input;
using BeatSaberSongGenerator.Generators;
using BeatSaberSongGenerator.IO;
using Microsoft.Win32;
using NAudio.Wave;

namespace BeatSaberSongGenerator.ViewModels
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        private string lastDirectory;
        public MainViewModel()
        {
            BrowseAudioCommand = new RelayCommand(BrowseAudio);
            BrowseCoverCommand = new RelayCommand(BrowseCover);
            GenerateCommand = new RelayCommand(GenerateSong, CanGenerateSong);
        }

        private float skillLevel = 5;
        public float SkillLevel
        {
            get => skillLevel;
            set
            {
                skillLevel = value;
                OnPropertyChanged();
            }
        }

        private string songName;
        public string SongName
        {
            get => songName;
            set
            {
                songName = value;
                OnPropertyChanged();
            }
        }

        private string author;
        public string Author
        {
            get => author;
            set
            {
                author = value;
                OnPropertyChanged();
            }
        }

        private string audioFilePath;
        public string AudioFilePath
        {
            get => audioFilePath;
            set
            {
                var isReadableAudio = IsSupportedByNAudio(value);// || Path.GetExtension(value)?.ToLowerInvariant() == ".ogg";
                
                if (!isReadableAudio)
                {
                    MessageBox.Show("Song cannot be read");
                }
                else
                {
                    audioFilePath = value;
                    SongName = Path.GetFileNameWithoutExtension(audioFilePath);
                    Author = string.Empty;
                }
                OnPropertyChanged();
            }
        }

        private static bool IsSupportedByNAudio(string audioFilePath)
        {
            try
            {
                using (new AudioFileReader(audioFilePath))
                {
                    return true;
                }
            }
            catch
            {
                return false;
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


        public ICommand BrowseAudioCommand { get; }
        public ICommand BrowseCoverCommand { get; }
        public ICommand GenerateCommand { get; }

        private void BrowseAudio()
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = lastDirectory,
                CheckFileExists = true,
                Title = "Select audio file"
            };
            if(openFileDialog.ShowDialog() != true)
                return;
            AudioFilePath = openFileDialog.FileName;
            lastDirectory = Path.GetDirectoryName(openFileDialog.FileName);
        }

        private void BrowseCover()
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = lastDirectory,
                CheckFileExists = true,
                Title = "Select cover image"
            };
            if(openFileDialog.ShowDialog() != true)
                return;
            CoverFilePath = openFileDialog.FileName;
            lastDirectory = Path.GetDirectoryName(openFileDialog.FileName);
        }

        private bool CanGenerateSong()
        {
            return !string.IsNullOrWhiteSpace(SongName)
                && !string.IsNullOrWhiteSpace(Author)
                && File.Exists(AudioFilePath)
                && File.Exists(CoverFilePath);
        }

        private void GenerateSong()
        {
            var songGenerator = new SongGenerator(new SongGeneratorSettings
            {
                SkillLevel = SkillLevel,
            });
            var song = songGenerator.Generate(SongName, Author, AudioFilePath, CoverFilePath);
            var songStorer = new SongStorer();
            var outputDirectory = Path.Combine(
                Path.GetDirectoryName(AudioFilePath),
                Path.GetFileNameWithoutExtension(AudioFilePath));
            songStorer.Store(song, outputDirectory);
        }
    }
}
