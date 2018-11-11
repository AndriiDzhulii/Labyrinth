using Labyrinth.Models;
using Labyrinth.Models.Enums;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Labyrinth.App.Core
{
    public class FileParser
    {
        public static LabyrinthModel ParseLabyrinthFile()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            var directory = new FileInfo(location.AbsolutePath).Directory.FullName;
            var filePath = $"{directory}\\{AppSettings.Default.LabyrinthFileName}";

            if (!File.Exists(filePath))
                throw new Exception("File not exists.");

            var linesList = File.ReadAllLines(filePath).Reverse().ToList();

            var model = new LabyrinthModel();

            short lineNumber = 0;
            foreach (var line in linesList)
            {
                short charNumber = 0;
                foreach (var character in line)
                {
                    var element = new LabyrinthElement
                    {
                        Point = new Point(charNumber, lineNumber)
                    };

                    switch (character)
                    {
                        case ' ':
                            element.Type = ElementType.Empty;
                            break;
                        case 'S':
                            element.Type = ElementType.Start;
                            break;
                        case 'F':
                            element.Type = ElementType.Finish;
                            break;
                        case '#':
                            element.Type = ElementType.Wall;
                            break;
                        default:
                            throw new Exception($"Input file contains unexpected character '{character}'");
                    }

                    model.Elements.Add(element);

                    charNumber++;
                }

                lineNumber++;
            }

            var startPoint = model.Elements.FirstOrDefault(e => e.Type == ElementType.Start);
            if (startPoint == null)
                throw new Exception("Start point not exists in input file.");

            var finishPoint = model.Elements.FirstOrDefault(e => e.Type == ElementType.Finish);
            if (finishPoint == null)
                throw new Exception("Finish point not exists in input file.");

            return model;
        }
    }
}
