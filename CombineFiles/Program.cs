using System;
using System.Collections.Generic;
using System.IO;

namespace CombineFiles
{
    class Program
    {
        private static StreamWriter _outputFile;
        private static HashSet<string> _includedFiles;

        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage : InputDirectory InputFileName OutputDirectory OutputFileName");
                return;
            }

            string inputDirectory = args[0];
            string inputFileName = args[1];
            string outputDirectory = args[2];
            string outputFileName = args[3];

            _outputFile = new StreamWriter(outputDirectory + outputFileName);
            _includedFiles = new HashSet<string>();
            WriteToFile(inputFileName, inputDirectory);
            _outputFile.Close();
        }

        private static void WriteToFile(string inputFileName, string inputDirectory)
        {
            string line;
            if (!File.Exists(inputDirectory + inputFileName))
            {
                return;
            }

            StreamReader inputFile = new StreamReader(inputDirectory + inputFileName);
            bool avoidNextLines = false;
            while ((line = inputFile.ReadLine()) != null)
            {
                if (avoidNextLines)
                {
                    if (line.Contains("#endif  // _LOCAL"))
                    {
                        avoidNextLines = false;
                    }
                }
                else
                {
                    if (line.Contains("#include"))
                    {
                        int startIndexOfPath;
                        if ((startIndexOfPath = line.IndexOf("\"", 9)) > 0)
                        {
                            int lastIndexOfPath = line.LastIndexOf("\"", line.Length);
                            string includeFile = line.Substring(startIndexOfPath + 1, lastIndexOfPath - startIndexOfPath - 1);
                            int startIndexOfFile = includeFile.LastIndexOf("\\");
                            string fileName = includeFile;
                            string newDirectory = inputDirectory;
                            if (startIndexOfFile >= 0)
                            {
                                fileName = includeFile.Substring(startIndexOfFile + 1, includeFile.Length - startIndexOfFile - 1);
                                newDirectory += includeFile.Substring(0, includeFile.Length - fileName.Length);
                            }
                            
                            if (!(_includedFiles.Contains(fileName)))
                            {
                                _includedFiles.Add(fileName);
                                WriteToFile(fileName, newDirectory);
                                fileName = fileName.Replace(".h", ".cpp");
                                WriteToFile(fileName, newDirectory);
                            }
                            
                        }
                        else
                        {
                            _outputFile.WriteLine(line);
                        }
                    }
                    else if (line.Contains("#ifdef _LOCAL"))
                    {
                        avoidNextLines = true;
                    }
                    else if (!line.StartsWith("//"))
                    {
                        _outputFile.WriteLine(line);
                    }
                }
            }

            _outputFile.WriteLine();
        }
    }
}
