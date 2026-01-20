using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Newtonsoft.Json;

namespace LocalSoundboard
{
    public class SoundButtonData
    {
        public int ButtonIndex { get; set; }
        public string SoundFilePath { get; set; } = string.Empty;
        public float Volume { get; set; } = 1.0f;
    }

    public partial class Form1 : Form
    {
        private class ButtonControl
        {
            public Button Button { get; set; }
            public TrackBar VolumeSlider { get; set; }
            public Label VolumeLabel { get; set; }
            public Button NormalizeButton { get; set; }
            public Button EjectButton { get; set; }
            public string SoundPath { get; set; } = string.Empty;
            public float Volume { get; set; } = 1.0f;
        }

        private List<ButtonControl> buttonControls = new List<ButtonControl>();
        private string settingsPath = Path.Combine(Application.StartupPath, "soundboard_settings.json");
        private ProgressBar outputLevelMeter;
        private Label outputLevelLabel;
        private System.Windows.Forms.Timer levelUpdateTimer;
        private WaveOutEvent currentOutputDevice;
        private AudioFileReader currentAudioFile;
        private float currentPeakLevel = 0f;
        private ToolTip toolTip;

        public Form1()
        {
            InitializeComponent();
            
            levelUpdateTimer = new System.Windows.Forms.Timer();
            levelUpdateTimer.Interval = 50;
            levelUpdateTimer.Tick += UpdateLevelMeter;
            levelUpdateTimer.Start();
            
            LoadSettings();
        }

        private void UpdateLevelMeter(object sender, EventArgs e)
        {
            currentPeakLevel *= 0.95f;
            
            int levelPercent = (int)(currentPeakLevel * 100);
            outputLevelMeter.Value = Math.Min(levelPercent, 100);
            
            if (levelPercent > 90)
                outputLevelMeter.ForeColor = Color.Red;
            else if (levelPercent > 70)
                outputLevelMeter.ForeColor = Color.Orange;
            else
                outputLevelMeter.ForeColor = Color.LimeGreen;
                
            outputLevelLabel.Text = $"Output: {levelPercent}%";
        }

        private void SoundButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            var control = buttonControls.FirstOrDefault(bc => bc.Button == btn);
            
            if (control != null && !string.IsNullOrEmpty(control.SoundPath) && File.Exists(control.SoundPath))
            {
                Task.Run(() => PlaySound(control.SoundPath, control.Volume));
            }
        }

        private void PlaySound(string filePath, float volume)
        {
            try
            {
                currentAudioFile?.Dispose();
                currentOutputDevice?.Dispose();

                currentAudioFile = new AudioFileReader(filePath);
                currentOutputDevice = new WaveOutEvent();
                
                currentAudioFile.Volume = Math.Min(volume, 2.0f);
                
                var meterStream = new MeteringSampleProvider(currentAudioFile);
                meterStream.StreamVolume += (s, args) =>
                {
                    currentPeakLevel = Math.Max(currentPeakLevel, Math.Max(args.MaxSampleValues[0], args.MaxSampleValues.Length > 1 ? args.MaxSampleValues[1] : 0));
                };
                
                currentOutputDevice.Init(meterStream);
                currentOutputDevice.Play();
                
                while (currentOutputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate {
                    MessageBox.Show($"Error playing sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        private void SoundButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button btn = (Button)sender;
                var control = buttonControls.FirstOrDefault(bc => bc.Button == btn);
                
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Audio Files (*.wav;*.mp3)|*.wav;*.mp3|All Files (*.*)|*.*";
                    ofd.Title = "Select a Sound File";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        control.SoundPath = ofd.FileName;
                        btn.Text = Path.GetFileNameWithoutExtension(ofd.FileName);
                        btn.BackColor = Color.LightGreen;
                        control.NormalizeButton.Enabled = true;
                        control.EjectButton.Enabled = true;
                        SaveSettings();
                    }
                }
            }
        }

        private void NormalizeButton_Click(object sender, EventArgs e)
        {
            Button normBtn = (Button)sender;
            var control = buttonControls.FirstOrDefault(bc => bc.NormalizeButton == normBtn);

            if (control != null && !string.IsNullOrEmpty(control.SoundPath) && File.Exists(control.SoundPath))
            {
                try
                {
                    normBtn.Enabled = false;
                    normBtn.Text = "...";
                    
                    Task.Run(() =>
                    {
                        string normalizedPath = NormalizeAudio(control.SoundPath);
                        
                        this.Invoke((MethodInvoker)delegate
                        {
                            control.SoundPath = normalizedPath;
                            control.Button.Text = Path.GetFileNameWithoutExtension(normalizedPath);
                            normBtn.Text = "N";
                            normBtn.Enabled = true;
                            SaveSettings();
                            MessageBox.Show("Audio normalized successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        });
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error normalizing audio: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    normBtn.Text = "N";
                    normBtn.Enabled = true;
                }
            }
        }

        private string NormalizeAudio(string inputPath)
        {
            string outputPath = Path.Combine(
                Path.GetDirectoryName(inputPath) ?? "",
                Path.GetFileNameWithoutExtension(inputPath) + "_normalized.wav"
            );

            using (var reader = new AudioFileReader(inputPath))
            {
                float max = 0;
                var buffer = new float[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
                int samplesRead;
                
                while ((samplesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < samplesRead; i++)
                    {
                        max = Math.Max(max, Math.Abs(buffer[i]));
                    }
                }

                if (max == 0) max = 1;
                float normalizationFactor = 0.95f / max;

                reader.Position = 0;
                
                using (var writer = new WaveFileWriter(outputPath, reader.WaveFormat))
                {
                    while ((samplesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        for (int i = 0; i < samplesRead; i++)
                        {
                            buffer[i] *= normalizationFactor;
                        }
                        writer.WriteSamples(buffer, 0, samplesRead);
                    }
                }
            }

            return outputPath;
        }

        private void VolumeSlider_ValueChanged(object sender, EventArgs e)
        {
            TrackBar slider = (TrackBar)sender;
            var control = buttonControls.FirstOrDefault(bc => bc.VolumeSlider == slider);
            
            if (control != null)
            {
                control.Volume = slider.Value / 50f;
                control.VolumeLabel.Text = $"{slider.Value * 2}%";
                SaveSettings();
            }
        }

        private void VolumeSlider_DoubleClick(object sender, EventArgs e)
        {
            TrackBar slider = (TrackBar)sender;
            slider.Value = 50;
        }

        private void EjectButton_Click(object sender, EventArgs e)
        {
            Button ejectBtn = (Button)sender;
            var control = buttonControls.FirstOrDefault(bc => bc.EjectButton == ejectBtn);

            if (control != null)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to remove this sound?",
                    "Confirm Eject",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    control.SoundPath = string.Empty;
                    control.Button.Text = $"Sound {buttonControls.IndexOf(control) + 1}\n(Right-click\nto load)";
                    control.Button.BackColor = Color.LightBlue;
                    control.NormalizeButton.Enabled = false;
                    control.EjectButton.Enabled = false;
                    SaveSettings();
                }
            }
        }

        private void SaveSettings()
        {
            var settings = buttonControls.Select((bc, index) => new SoundButtonData
            {
                ButtonIndex = index,
                SoundFilePath = bc.SoundPath,
                Volume = bc.Volume
            }).ToList();

            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        private void LoadSettings()
        {
            if (File.Exists(settingsPath))
            {
                try
                {
                    var settings = JsonConvert.DeserializeObject<List<SoundButtonData>>(File.ReadAllText(settingsPath));
                    
                    foreach (var setting in settings)
                    {
                        if (setting.ButtonIndex < buttonControls.Count)
                        {
                            var control = buttonControls[setting.ButtonIndex];
                            control.SoundPath = setting.SoundFilePath;
                            control.Volume = setting.Volume;
                            
                            if (!string.IsNullOrEmpty(setting.SoundFilePath) && File.Exists(setting.SoundFilePath))
                            {
                                control.Button.Text = Path.GetFileNameWithoutExtension(setting.SoundFilePath);
                                control.Button.BackColor = Color.LightGreen;
                                control.NormalizeButton.Enabled = true;
                                control.EjectButton.Enabled = true;
                            }
                            
                            control.VolumeSlider.Value = (int)(setting.Volume * 50);
                            control.VolumeLabel.Text = $"{(int)(setting.Volume * 100)}%";
                        }
                    }
                }
                catch { }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            levelUpdateTimer?.Stop();
            levelUpdateTimer?.Dispose();
            currentAudioFile?.Dispose();
            currentOutputDevice?.Dispose();
            toolTip?.Dispose();
            SaveSettings();
            base.OnFormClosing(e);
        }
    }
}
