using Labyrinth.Models.Enums;
using System.Collections.Generic;

namespace Labyrinth.Models
{
    public class LabyrinthElement
    {
        public ElementType Type { get; set; }
        public Point Point { get; set; }
        public List<LabyrinthElement> CloseElements = new List<LabyrinthElement>();

        public bool Proccessed { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as LabyrinthElement;

            if (item == null)
            {
                return false;
            }

            return item.Point.X == Point.X && item.Point.Y == Point.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Point.X.GetHashCode();
                hash = hash * 23 + Point.Y.GetHashCode();
                return hash;
            }
        }
    }
}
