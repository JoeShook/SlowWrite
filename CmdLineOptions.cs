using CommandLine;
using CommandLine.Text;

namespace SlowWrite
{
    public class CmdLineOptions
    {
        [Option('l', "LineWait", Required = false, DefaultValue = 10, HelpText = "Milliseconds per line")]
        public int LineWait { get; set; }

        [Option('i', "Input", Required = true, HelpText = "Input file path")]
        public string InputFilePath { get; set; }

        [Option('o', "Output", Required = true, HelpText = "Output file path")]
        public string OutputFilePath { get; set; }

        [HelpOption(HelpText = @"Example: SlowWrite -i Data\In\license.txt -o Data\Out\license.txt -l 500  or   SlowWrite.exe -i Data\In\*.txt -o Data\Out -l 500")]
        public string GetUsage()
        {
            var helpText = HelpText.AutoBuild(this);
            helpText.AdditionalNewLineAfterOption = true;
            return helpText;
        }
    }
}
