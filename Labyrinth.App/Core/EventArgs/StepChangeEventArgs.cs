using Labyrinth.Models;
using System;

namespace Labyrinth.App.Core
{
    public class StepChangeEventArgs : EventArgs
    {
        public Point Point { get; set; }

        public StepChangeEventArgs(Point point)
        {
            Point = point;
        }
    }
}
