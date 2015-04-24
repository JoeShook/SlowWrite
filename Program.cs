using System;
using System.Collections.Generic;
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

            EnsureFolders();

            if (File.Exists(InputPath()))
            {
                CopySingleFile();
            }
            else
            {
                CopyManyFiles();
            }
        }

        private static void EnsureFolders()
        {
            if (!string.IsNullOrEmpty(_options.BackupFilePath) && !Directory.Exists(_options.BackupFilePath))
            {
                Directory.CreateDirectory(_options.BackupFilePath);
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

            var files = inputDirectoryInfo
                .EnumerateFiles(GetFilePattern(), SearchOption.TopDirectoryOnly)
                .OrderBy(x => x.LastWriteTime);
            
            var po = new ParallelOptions
            {
                MaxDegreeOfParallelism = _options.Threads 
            };

            Parallel.ForEach(files, po, (file) =>
            {
                var outFile = Path.Combine(OutputPath(), file.Name);
                using (var inputStream = new StreamReader(file.FullName))
                using (var outFileStream = new FileStream(Path.Combine(OutputPath(), file.Name), FileMode.Create, FileAccess.Write, FileShare.Write))
                using (var outputStream = new StreamWriter(outFileStream))
                {
                    CopyFile(inputStream, outputStream);
                }

                if (file.Extension == ".eml")
                {
                    
                    try
                    {
                        // Touch the file
                        for (int i = 0; i < 1000; i++)
                        {
                            using (var outFileStream = new FileStream(Path.Combine(OutputPath(), file.Name), FileMode.Open, FileAccess.Read, FileShare.Write))
                            {
                                File.SetLastWriteTime(outFile, DateTime.Now.Add(TimeSpan.FromSeconds(-i)));
                                Thread.Sleep(TimeSpan.FromMilliseconds(2));
                            }
                        }
                        
                    }
                    catch
                    {
                        Console.WriteLine("Error");
                    }
                }

                if (!string.IsNullOrEmpty(_options.BackupFilePath))
                {
                    file.MoveTo(Path.Combine(_options.BackupFilePath, file.Name));
                }
                else if (_options.DeleteSourceFile)
                {
                    file.Delete();
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
                Thread.Sleep(_options.LineWait);
                Console.Write(".");
                output.WriteLine(line);
            }

            Console.WriteLine(sw.Elapsed);
        }

        protected IEnumerable<FileInfo> GetSortedFiles(string searchPattern, string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            return directoryInfo.EnumerateFiles("*" + searchPattern, SearchOption.TopDirectoryOnly)
                .OrderBy(x => x.LastWriteTime);
        }
    }
}
