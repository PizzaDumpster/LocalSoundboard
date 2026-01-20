# LocalSoundboard

A Windows Forms desktop application that provides a 9-button soundboard for playing audio clips with individual volume controls and audio normalization.

## Features

- **9 Sound Buttons**: Load and play up to 9 different audio clips simultaneously
- **Multiple Audio Formats**: Supports WAV and MP3 files
- **Individual Volume Control**: Each button has its own vertical slider (0-200%) with boost capability
- **Audio Normalization**: Built-in normalize function to balance volume levels across clips
- **Real-time Output Meter**: Visual feedback showing current audio output levels with color indicators
- **Persistent Settings**: All sound selections and volume levels are saved and restored on startup
- **Easy Controls**:
  - **Left-click** to play a sound
  - **Right-click** to load a sound file
  - **Double-click slider** to reset volume to 100%
  - **Click "N" button** to normalize audio

## Requirements

- .NET 8.0 or later
- Windows OS
- NAudio library (automatically installed via NuGet)

## Installation

1. Clone this repository
2. Open `LocalSoundboard.sln` in Visual Studio
3. Build and run the project

Or run directly:
```bash
dotnet build
dotnet run --project LocalSoundboard/LocalSoundboard.csproj
```

## Usage

1. **Load Sounds**: Right-click any button to select an audio file
2. **Play Sounds**: Left-click a button to play the loaded sound
3. **Adjust Volume**: Use the vertical slider next to each button (0-200%)
4. **Normalize Audio**: Click the orange "N" button to normalize a clip to 95% peak volume
5. **Monitor Output**: Watch the top meter for real-time audio level feedback

## Settings

Settings are automatically saved to `soundboard_settings.json` in the application directory and include:
- Sound file paths
- Individual volume levels

## Technologies Used

- C# / .NET 8.0
- Windows Forms
- NAudio for audio playback and processing
- Newtonsoft.Json for settings persistence