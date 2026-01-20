namespace LocalSoundboard
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            toolTip = new ToolTip();
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 100;
            
            int buttonWidth = 120;
            int buttonHeight = 100;
            int padding = 10;
            int startX = 20;
            int startY = 60;

            outputLevelLabel = new Label();
            outputLevelLabel.Text = "Output: 0%";
            outputLevelLabel.Size = new System.Drawing.Size(100, 20);
            outputLevelLabel.Location = new System.Drawing.Point(startX, 10);
            outputLevelLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.Controls.Add(outputLevelLabel);

            outputLevelMeter = new ProgressBar();
            outputLevelMeter.Minimum = 0;
            outputLevelMeter.Maximum = 100;
            outputLevelMeter.Value = 0;
            outputLevelMeter.Size = new System.Drawing.Size(700, 25);
            outputLevelMeter.Location = new System.Drawing.Point(startX, 30);
            outputLevelMeter.ForeColor = Color.LimeGreen;
            outputLevelMeter.Style = ProgressBarStyle.Continuous;
            this.Controls.Add(outputLevelMeter);

            for (int i = 0; i < 9; i++)
            {
                int col = i % 3;
                int row = i / 3;
                int xPos = startX + col * (buttonWidth + 100 + padding * 2);
                int yPos = startY + row * (buttonHeight + padding * 2);

                Button btn = new Button();
                btn.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
                btn.Location = new System.Drawing.Point(xPos, yPos);
                btn.Text = $"Sound {i + 1}\n(Right-click\nto load)";
                btn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
                btn.BackColor = System.Drawing.Color.LightBlue;
                btn.FlatStyle = FlatStyle.Flat;
                btn.Click += new System.EventHandler(this.SoundButton_Click);
                btn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SoundButton_MouseDown);
                this.Controls.Add(btn);

                TrackBar slider = new TrackBar();
                slider.Minimum = 0;
                slider.Maximum = 100;
                slider.Value = 50;
                slider.TickFrequency = 10;
                slider.Orientation = Orientation.Vertical;
                slider.Size = new System.Drawing.Size(45, buttonHeight);
                slider.Location = new System.Drawing.Point(xPos + buttonWidth + padding, yPos);
                slider.ValueChanged += new System.EventHandler(this.VolumeSlider_ValueChanged);
                slider.DoubleClick += new System.EventHandler(this.VolumeSlider_DoubleClick);
                this.Controls.Add(slider);

                Label volLabel = new Label();
                volLabel.Text = "100%";
                volLabel.Size = new System.Drawing.Size(45, 20);
                volLabel.Location = new System.Drawing.Point(xPos + buttonWidth + padding + 50, yPos + 10);
                volLabel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
                volLabel.TextAlign = ContentAlignment.MiddleLeft;
                this.Controls.Add(volLabel);

                Button normBtn = new Button();
                normBtn.Text = "N";
                normBtn.Size = new System.Drawing.Size(30, 30);
                normBtn.Location = new System.Drawing.Point(xPos + buttonWidth + padding + 50, yPos + 35);
                normBtn.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
                normBtn.BackColor = System.Drawing.Color.Orange;
                normBtn.FlatStyle = FlatStyle.Flat;
                normBtn.Enabled = false;
                normBtn.Click += new System.EventHandler(this.NormalizeButton_Click);
                this.Controls.Add(normBtn);
                
                toolTip.SetToolTip(normBtn, "Normalize Audio\n\nAnalyzes the audio and adjusts it to 95% peak volume.\nCreates a new normalized WAV file.\nUseful for matching volume levels across clips.");

                buttonControls.Add(new ButtonControl
                {
                    Button = btn,
                    VolumeSlider = slider,
                    VolumeLabel = volLabel,
                    NormalizeButton = normBtn,
                    Volume = 1.0f
                });
            }

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 440);
            this.Text = "Local Soundboard";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ResumeLayout(false);
        }

        #endregion
    }
}
