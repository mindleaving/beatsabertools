# beatsabertools
Level generator for VR-game Beat Saber

**What this is NOT**

A replacement for handcrafted Beat Saber levels/charts

**What this is**

A tool for creating custom songs for Beat Saber with a barely tolerable quality for listening to music while moving your body.
You need to mod Beat Saber using this project: https://github.com/xyonico/BeatSaberSongLoader/releases

This tool comes "as-is" with no support or warrenty (see LICENSE). If something doesn't work, feel free to report it as an issue, but the chance that I will do something about it is slim as I'm working on other projects. If you need it fixed and are comfortable programming C#/.NET, you are welcome to fork this project and modify it however you like or submit pull requests :)

**How to use it**

- Start BeatSaberSongGenerator.exe
- Find the song file and cover image for the song you like to import (I tested with MP3, other file formats might work as well. The tool will tell you if it can't handle a selected audio file)
- Fill in song name and author
- Click "Generate"

The generated song and corresponding levels are stored in the same directory as the audio file within a directory of the same name. That directy must then be placed in the "CustomSongs" folder in the Beat Saber directory.


