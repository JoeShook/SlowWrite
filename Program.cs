using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
            
            var file = new StreamReader(_options.InputFilePath);

            using (var outputStream = new StreamWriter(_options.OutputFilePath))
            {
                CopyFile(file, outputStream);
            }
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
