using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace SlowWrite
{
    class Program
    {
        private static CmdLineOptions _options;

        static void Main(string[] args)
        {
            _options = new CmdLineOptions();
            if (!Parser.Default.ParseArguments(args, _options))
            {
                Console.ReadLine();
                Environment.Exit(Parser.DefaultExitCodeFail);
            }
            
            if (File.Exists(InputPath()))
            {
                CopySingleFile();
            }
            else
            {
                CopyManyFiles();
            }
        }

        private static void CopySingleFile()
        {
            using(var inputStream = new StreamReader(_options.InputFilePath))
            using (var outputStream = new StreamWriter(_options.OutputFilePath))
            {
                CopyFile(inputStream, outputStream);
            }
        }

        private static void CopyManyFiles()
        {
            var inputDirectoryInfo = new DirectoryInfo(GetDirectory(InputPath()));
            var files = inputDirectoryInfo.EnumerateFiles(GetFilePattern(), SearchOption.TopDirectoryOnly);

            var po = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(files, po, (file) =>
            {
                using (var inputStream = new StreamReader(file.FullName))
                using (var outputStream = new StreamWriter(Path.Combine(OutputPath(), file.Name)))
                {
                    CopyFile(inputStream, outputStream);
                }
            });
        }

        private static string InputPath()
        {
            string inputPath = Path.Combine(Directory.GetCurrentDirectory(), _options.InputFilePath);

            return inputPath;
        }

        private static string OutputPath()
        {
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), _options.OutputFilePath);

            return outputPath;
        }

        private static string GetDirectory(string path)
        {
            var parts = path.Split('\\').ToList();
            parts.Remove(parts.Last());

            return string.Join(@"\", parts);
        }

        private static string GetFilePattern()
        {
            var parts = InputPath().Split('\\').ToList();

            return parts.Last();
        }

        private static void CopyFile(StreamReader input, StreamWriter output)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            string line;
            while ((line = input.ReadLine()) != null)
            {
                Console.Write(".");
                output.WriteLine(line);
                Thread.Sleep(_options.LineWait);
            }

            Console.WriteLine(sw.Elapsed);
        }
    }
}
