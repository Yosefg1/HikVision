using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Serilog;

namespace HikVisionRunner
{
    internal class Program
    {
        private static string? _solutionDirectory;

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();
  
            _solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.FullName;
            Thread thread1 = new(HikeVisionConverter);
            Log.Information("HikeVisionConverter Thread Created...");
            Thread thread2 = new(HikeVisionApiInterface);
            Log.Information("HikeVisionApiInterface Thread Created...");
            // Start the threads
            thread1.Start();
            Log.Information("HikeVisionConverter Thread Running!");
            thread2.Start();
            Log.Information("HikeVisionApiInterface Thread Running!");

            // Wait for the threads to finish
            thread1.Join();
            thread2.Join();

            Log.Information("Both processes have finished.");
            Log.CloseAndFlush();
        }

        private static void HikeVisionConverter()
        {
            Process HikeVisionConverter = new();
            ProcessStartInfo startInfo = new()
            {
                FileName = Path.Combine(_solutionDirectory, @"HikeVisionConverter\bin\Debug\net7.0\HikeVisionConverter.exe"),
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            HikeVisionConverter.StartInfo = startInfo;

            // Start the process
            HikeVisionConverter.Start();

            // Read the output
            string output = HikeVisionConverter.StandardOutput.ReadToEnd();
            HikeVisionConverter.OutputDataReceived += OnDataRecived;

            // Wait for the process to exit
            HikeVisionConverter.WaitForExit();

            Log.Information(output);
        }

        private static void HikeVisionApiInterface()
        {
            Process HikeVisionApiInterface = new();
            ProcessStartInfo startInfo = new()
            {
                FileName = Path.Combine(_solutionDirectory, @"HikeVisionApiInterface\bin\Debug\net7.0\HikeVisionApiInterface.exe"),
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            HikeVisionApiInterface.StartInfo = startInfo;

            // Start the process
            HikeVisionApiInterface.Start();

            // Read the output
            string output = HikeVisionApiInterface.StandardOutput.ReadToEnd();
            HikeVisionApiInterface.OutputDataReceived += OnDataRecived;

            // Wait for the process to exit
            HikeVisionApiInterface.WaitForExit();

            Log.Information(output);
        }

        private static void OnDataRecived(object sender, DataReceivedEventArgs e)
        {
            Log.Information(e.Data!);
        }
    }
}
