using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FoxingVideo;

public class FFmpegTask
{
    public static readonly List<FFmpegTask> RunningTasks = new();

    public string InputFile { get; set; }

    public string OutputFile { get; set; }

    public string Arguments { get; set; }

    public string Status { get; set; } = "";

    public FFmpegTask(string inputFile, string outputFile, string arguments)
    {
        InputFile = inputFile;
        OutputFile = outputFile;
        Arguments = arguments;
    }

    public async Task Run()
    {
        if (File.Exists(InputFile))
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-i '{InputFile}' {Arguments} '{OutputFile}'",
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
            Status = e.Data ?? "";
        };

        process.Start();

        while (!process.HasExited)
        {
            await Task.Delay(500); // Update every 500 ms. Adjust this value to your needs.
        }

        if (process.ExitCode != 0)
        {
            throw new Exception($"ffmpeg failed with exit code {process.ExitCode}.");
        }
    }
}
