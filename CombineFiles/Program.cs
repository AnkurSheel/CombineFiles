﻿using System;
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
        private static HashSet<string> _includedFiles;

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
            _includedFiles = new HashSet<string>();
            WriteToFile(inputFileName);
            _outputFile.Close();
        }

        private static void WriteToFile(string inputFileName)
        {
            string line;
            if (!File.Exists(_inputDirectory + inputFileName))
            {
                return;
            }

            StreamReader inputFile = new StreamReader(_inputDirectory + inputFileName);
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
                        int startIndex;
                        if ((startIndex = line.IndexOf("\"", 9)) > 0)
                        {
                            int lastIndex = line.LastIndexOf("\"", line.Length);
                            string includeFile = line.Substring(startIndex + 1, lastIndex - startIndex - 1);
                            if (!(_includedFiles.Contains(includeFile)))
                            {
                                _includedFiles.Add(includeFile);
                                WriteToFile(includeFile);
                                includeFile = includeFile.Replace(".h", ".cpp");
                                WriteToFile(includeFile);
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
                    else
                    {
                        _outputFile.WriteLine(line);
                    }
                }
            }

            _outputFile.WriteLine();
        }
    }
}
