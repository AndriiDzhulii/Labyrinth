namespace Labyrinth.Models
{
    public class LabyrinthElementChain
    {
        public LabyrinthElementChain Parent { get; private set; }

        public LabyrinthElement LabyrinthElement { get; set; }

        public LabyrinthElementChain(LabyrinthElement element, LabyrinthElementChain parent)
        {
            LabyrinthElement = element;
            Parent = parent;
        }

        public LabyrinthElementChain(LabyrinthElement element)
        {
            LabyrinthElement = element;
        }
    }
}
