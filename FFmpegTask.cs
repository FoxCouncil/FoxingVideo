using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FoxingVideo;

public class FFmpegTask
{
    public static readonly ObservableCollection<FFmpegTask> RunningTasks = new();

    public string InputFile { get; set; }

    public string OutputFile { get; set; }

    public FFmpegProfile Profile { get; set; }

    public string Status { get; set; } = "";

    public FFmpegTask(FFmpegProfile profile, string inputFile, string outputDir)
    {
        Profile = profile;

        InputFile = inputFile;
        OutputFile = Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(inputFile)}_{Profile.Key}");
    }

    public async Task Run()
    {
        if (File.Exists(InputFile))
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(AppContext.BaseDirectory, "ffmpeg.exe"),
                    Arguments = $"-i \"{InputFile}\" {Profile.Arguments} \"{OutputFile}.avi\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            RunningTasks.Add(this);

            await RunProcessAsync(proc);

            RunningTasks.Remove(this);
        }
        else
        {
            throw new FileNotFoundException($"{InputFile} does not exist.");
        }
    }

    private async Task RunProcessAsync(Process process)
    {
        process.ErrorDataReceived += (s, e) =>
        {
            Status += e.Data ?? "";
        };

        process.OutputDataReceived += (s, e) =>
        {
            Status += e.Data ?? "";
        };

        process.Start();

        while (!process.HasExited)
        {
            await Task.Delay(1); // Update every 500 ms. Adjust this value to your needs.
        }

        if (process.ExitCode != 0)
        {
            throw new Exception($"ffmpeg failed with exit code {process.ExitCode}.");
        }
    }
}
