using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CombineFiles
{
    class Program
    {
        private static string _inputDirectory;
        private static StreamWriter _outputFile;

        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage : InputDirectory InputFileName OutputDirectory OutputFileName");
                return;
            }

            _inputDirectory = args[0];
            string inputFileName = args[1];
            string outputDirectory = args[2];
            string outputFileName = args[3];

            _outputFile = new StreamWriter(outputDirectory + outputFileName);
            WriteToFile(inputFileName);
            _outputFile.Close();
        }

        private static void WriteToFile(string inputFileName)
        {
            string line;
            StreamReader inputFile = new StreamReader(_inputDirectory + inputFileName);
            while ((line = inputFile.ReadLine()) != null)
            {
                if (line.Contains("#include"))
                {
                    int startIndex;
                    if ((startIndex = line.IndexOf("\"", 9)) > 0)
                    {
                        int lastIndex = line.LastIndexOf("\"", line.Length);
                        WriteToFile(line.Substring(startIndex + 1, lastIndex - startIndex - 1));
                    }
                    else
                    {
                        _outputFile.WriteLine(line);
                    }
                }
                else
                {
                    _outputFile.WriteLine(line);
                }
            }

            _outputFile.WriteLine();
        }
    }
}
