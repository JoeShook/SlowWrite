using System;
using CommandLine;
using CommandLine.Text;

namespace SlowWrite
{
    public class CmdLineOptions
    {
        [Option('l', "LineWait", Required = false, DefaultValue = 10, HelpText = "Milliseconds per line")]
        public int LineWait { get; set; }

        [Option('t', "Threads", Required = false, DefaultValue = 5, HelpText = "Threads per file")]
        public int Threads { get; set; }

        [Option('i', "Input", Required = true, HelpText = "Input file path")]
        public string InputFilePath { get; set; }

        [Option('o', "Output", Required = true, HelpText = "Output file path")]
        public string OutputFilePath { get; set; }

        [Option('b', "Backup", Required = false, HelpText = "Backup path for source path")]
        public string BackupFilePath { get; set; }

        [Option('d', "DeleteSourceFile", Required = false, DefaultValue = false, HelpText = "Delete source if file")]
        public bool DeleteSourceFile { get; set; }


        [HelpOption(HelpText = @"Example: SlowWrite -i Data\In\license.txt -o Data\Out\license.txt -l 500  or   SlowWrite.exe -i Data\In\*.txt -o Data\Out -l 500")]
        public string GetUsage()
        {
            var helpText = HelpText.AutoBuild(this);
            helpText.AdditionalNewLineAfterOption = true;
            return helpText;
        }
    }
}
