using Labyrinth.Models;
using Labyrinth.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Labyrinth.App.Core
{
    public class Representation
    {
        private Canvas _canvas;
        private List<LabyrinthUIElement> _elements = new List<LabyrinthUIElement>();
        private LabyrinthUIElement _currentActiveStep;

        public Representation(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void DrawLabyrinth(LabyrinthModel model)
        {
            foreach (var element in model.Elements)
                DrawElement(element);
        }

        public void SetActiveStep(Point point)
        {
            var elementToActivate = _elements.FirstOrDefault(e => e.Point.X == point.X && e.Point.Y == point.Y);

            var stepSet = elementToActivate.TextBlock.Dispatcher.BeginInvoke((Action)(() =>
            {
                elementToActivate.TextBlock.Foreground = new SolidColorBrush(Colors.Green);
                if (elementToActivate.Type == ElementType.Empty)
                    elementToActivate.TextBlock.Text = "O";
            }));

            stepSet.Completed += (s, e) =>
            {
                if (_currentActiveStep != null)
                {
                    _currentActiveStep.TextBlock.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        _currentActiveStep.TextBlock.Foreground = new SolidColorBrush(Colors.Blue);
                    })).Completed += (cs, ce) =>
                    {
                        _currentActiveStep = elementToActivate;
                    };
                }
                else
                {
                    _currentActiveStep = elementToActivate;
                }
            };
        }

        private void DrawElement(LabyrinthElement element)
        {
            var elementText = string.Empty;
            switch (element.Type)
            {
                case ElementType.Empty:
                    elementText = " ";
                    break;
                case ElementType.Start:
                    elementText = "S";
                    break;
                case ElementType.Finish:
                    elementText = "F";
                    break;
                case ElementType.Wall:
                    elementText = "#";
                    break;
            }

            var labyrinthUIElement = new LabyrinthUIElement
            {
                Type = element.Type,
                Point = new Point(element.Point.X, element.Point.Y),
                TextBlock = new TextBlock
                {
                    Text = elementText,
                    FontSize = 22
                }
            };

            _elements.Add(labyrinthUIElement);

            Canvas.SetLeft(labyrinthUIElement.TextBlock, element.Point.X * 20);
            Canvas.SetBottom(labyrinthUIElement.TextBlock, element.Point.Y * 20);

            _canvas.Children.Add(labyrinthUIElement.TextBlock);
        }
    }
}
