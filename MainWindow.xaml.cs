using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SAMFrontend
{
    public partial class MainWindow : Window
    {
        private string samPath = "sam.exe"; // Path to sam.exe
        private string outputPath = "output.wav"; // Output path for the generated WAV file

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            string inputText = txtInput.Text;
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show("Please enter some text to synthesize.");
                return;
            }

            statusBar.Text = "Audio playing...";
            await GenerateAudioAsync(inputText);
            statusBar.Text = "Ready";
        }

        private async void SaveWav_Click(object sender, RoutedEventArgs e)
        {
            string inputText = txtInput.Text;
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show("Please enter some text to synthesize.");
                return;
            }

            // Create a SaveFileDialog instance
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = "output.wav"; // Default filename
            saveFileDialog.DefaultExt = ".wav"; // Default file extension
            saveFileDialog.Filter = "WAV Files (*.wav)|*.wav"; // Filter to only show WAV files

            // Show the dialog and await user input
            if (saveFileDialog.ShowDialog() == true)
            {
                // Get the selected file name and save audio
                string outputPath = saveFileDialog.FileName;
                await SaveAudioAsync(outputPath, inputText);
            }
        }


        private async Task GenerateAudioAsync(string text)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = samPath;
                        process.StartInfo.Arguments = $"\"{text}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.CreateNoWindow = true;

                        process.Start();
                        process.WaitForExit();
                    }
                });

                statusBar.Text = "Audio generated successfully.";
            }
            catch (Exception ex)
            {
                statusBar.Text = $"Error: {ex.Message}";
            }
        }

        private async Task SaveAudioAsync(string outputPath, string text)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = samPath;
                        process.StartInfo.Arguments = $"-wav \"{outputPath}\" \"{text}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.CreateNoWindow = true;

                        process.Start();
                        process.WaitForExit();
                    }
                });

                statusBar.Text = $"Audio saved to {outputPath}.";
            }
            catch (Exception ex)
            {
                statusBar.Text = $"Error: {ex.Message}";
            }
        }
    }
}
