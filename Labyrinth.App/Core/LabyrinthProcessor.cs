using Labyrinth.Models;
using Labyrinth.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Labyrinth.App.Core
{
    public class LabyrinthProcessor
    {
        public event EventHandler<StepChangeEventArgs> StepChanged;
        private bool _finishFound = false;

        public List<Point> ProccessLabyrinth(LabyrinthModel model)
        {
            var startPoint = model.Elements.FirstOrDefault(e => e.Type == ElementType.Start);
            var proccessedElements = new List<LabyrinthElement>();

            ProcessLabyrinth(startPoint, proccessedElements, model.Elements);

            var startElement = proccessedElements.FirstOrDefault(e => e.Type == ElementType.Start);
            var finishElement = proccessedElements.FirstOrDefault(e => e.Type == ElementType.Finish);

            var chains = new List<LabyrinthElementChain>();
            SearchPathToProccessedElement(startElement,
                                           finishElement.Point,
                                           null,
                                           chains);

            var chainElements = GetChainElements(chains.FirstOrDefault());
            chainElements.Reverse();

            var result = chainElements.Select(e => e.Point).ToList();

            return result;
        }

        private void ProcessLabyrinth(LabyrinthElement elementToProccess,
                                    List<LabyrinthElement> proccessedElements,
                                    List<LabyrinthElement> existingElements)
        {
            if (_finishFound)
                return;

            var x = elementToProccess.Point.X;
            var y = elementToProccess.Point.Y;

            Thread.Sleep(80);

            OnStepChanged(new StepChangeEventArgs(elementToProccess.Point));

            var closeElements = existingElements.Where(e => (e.Point.X == x - 1 && e.Point.Y == y)
                                                            || (e.Point.X == x + 1 && e.Point.Y == y)
                                                            || (e.Point.X == x && e.Point.Y == y + 1)
                                                            || (e.Point.X == x && e.Point.Y == y - 1))
                                                 .Where(e => !elementToProccess.CloseElements.Any(c => c.Equals(e))).ToList();

            elementToProccess.CloseElements.AddRange(closeElements);

            foreach (var closeElement in elementToProccess.CloseElements)
            {
                if (!closeElement.CloseElements.Any(e => e.Equals(elementToProccess)))
                {
                    closeElement.CloseElements.Add(elementToProccess);
                }
            }

            elementToProccess.Proccessed = true;
            proccessedElements.Add(elementToProccess);

            if (elementToProccess.Type == ElementType.Finish)
            {
                _finishFound = true;
                return;
            }

            var closestFinishElement = elementToProccess.CloseElements.FirstOrDefault(e => e.Type == ElementType.Finish);
            if (closestFinishElement != null)
                ProcessLabyrinth(closestFinishElement, proccessedElements, existingElements);

            var closestNotProccessedEmptyElement = elementToProccess.CloseElements.FirstOrDefault(e => e.Type == ElementType.Empty && !e.Proccessed);
            if (closestNotProccessedEmptyElement != null)
                ProcessLabyrinth(closestNotProccessedEmptyElement, proccessedElements, existingElements);

            var elemntsListChecked = new List<LabyrinthElement>();
            var emptyNotProccessedElement = SearchForEmptyElementToProccess(elementToProccess, elemntsListChecked);
            if (emptyNotProccessedElement == null)
                return;

            var chains = new List<LabyrinthElementChain>();
            SearchPathToProccessedElement(elementToProccess,
                                           emptyNotProccessedElement.Point,
                                           null,
                                           chains);

            var chainElements = GetChainElements(chains.FirstOrDefault());
            chainElements.Reverse();

            foreach (var element in chainElements)
            {
                if (_finishFound)
                    break;

                Thread.Sleep(80);
                OnStepChanged(new StepChangeEventArgs(element.Point));
            }

            ProcessLabyrinth(emptyNotProccessedElement, proccessedElements, existingElements);
        }

        private void GoToNotProccessedElement(LabyrinthElement currentElement,
                                              LabyrinthElement elementToGetTo,
                                              List<LabyrinthElement> existingElements)
        {

        }

        private LabyrinthElement SearchForEmptyElementToProccess(LabyrinthElement elementToCheck, List<LabyrinthElement> checkedElements)
        {
            checkedElements.Add(elementToCheck);

            if (elementToCheck.Type == ElementType.Empty && !elementToCheck.Proccessed)
                return elementToCheck;

            foreach (var closeElement in elementToCheck.CloseElements)
            {
                if (checkedElements.Any(e => e.Point.X == closeElement.Point.X && e.Point.Y == closeElement.Point.Y))
                    continue;

                if (closeElement.Type != ElementType.Wall)
                {
                    if (closeElement.Proccessed)
                    {
                        var result = SearchForEmptyElementToProccess(closeElement, checkedElements);
                        if (result != null)
                            return result;
                    }
                    else
                    {
                        return closeElement;
                    }
                }
            }

            return null;
        }

        private void SearchPathToProccessedElement(LabyrinthElement currentElement,
                                                            Point pointToGetTo,
                                                            LabyrinthElementChain parentChain,
                                                            List<LabyrinthElementChain> foundChains)
        {
            if (parentChain == null)
                parentChain = new LabyrinthElementChain(currentElement);
            else
                parentChain = new LabyrinthElementChain(currentElement, parentChain);

            foreach (var elementToCheck in currentElement.CloseElements.Where(e => e.Type != ElementType.Wall))
            {
                if (GetChainElements(parentChain).Any(e => e.Point.X == elementToCheck.Point.X && e.Point.Y == elementToCheck.Point.Y))
                    continue;

                if (elementToCheck.Point.X == pointToGetTo.X && elementToCheck.Point.Y == pointToGetTo.Y)
                {
                    foundChains.Add(new LabyrinthElementChain(elementToCheck, parentChain));
                }
                else
                {
                    SearchPathToProccessedElement(elementToCheck, pointToGetTo, parentChain, foundChains);
                }
            }
        }

        private List<LabyrinthElement> GetChainElements(LabyrinthElementChain chain)
        {
            var result = new List<LabyrinthElement>();
            var chainToCheck = chain;
            while (chainToCheck != null)
            {
                result.Add(chainToCheck.LabyrinthElement);
                chainToCheck = chainToCheck.Parent;
            }

            return result;
        }

        protected virtual void OnStepChanged(StepChangeEventArgs e)
        {
            StepChanged?.Invoke(this, e);
        }
    }
}
