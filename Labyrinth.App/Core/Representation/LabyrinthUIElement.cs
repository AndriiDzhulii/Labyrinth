using Labyrinth.Models;
using Labyrinth.Models.Enums;
using System.Windows.Controls;

namespace Labyrinth.App.Core
{
    public class LabyrinthUIElement
    {
        public ElementType Type { get; set; }

        public Point Point { get; set; }

        public TextBlock TextBlock { get; set; }
    }
}
