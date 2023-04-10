using System;
using System.Collections.Generic;
using System.Linq;

namespace Challenges.MazeGeneration
{
    class CrossNodeSet
    {
        CrossNode[,] nodeArray;
        public List<CrossNode> ActiveNodes { get; private set; }
        public List<CrossNode> ClosedNodes { get; private set; }

        public List<CrossNode> OpenNodes =>
            ClosedNodes.Where(closedNode => GetOpenDirections(closedNode.Position).Count > 0).ToList();

        public CrossNode this[Int2 nodePosition]
        {
            get => nodeArray[nodePosition.X, nodePosition.Y];
        }

        public bool GetNodeInDirection(Int2 nodePosition, Direction direction, out CrossNode node)
        {
            node = null;

            switch (direction)
            {
                case Direction.Up:
                    nodePosition.Y -= 1;
                    if (nodePosition.Y < 0) return false;
                    break;
                case Direction.Right:
                    nodePosition.X += 1;
                    if (nodePosition.X >= nodeArray.GetLength(0)) return false;
                    break;
                case Direction.Down:
                    nodePosition.Y += 1;
                    if (nodePosition.Y >= nodeArray.GetLength(1)) return false;
                    break;
                case Direction.Left:
                    nodePosition.X -= 1;
                    if (nodePosition.X < 0) return false;
                    break;
            }

            node = this[nodePosition];

            return !node.Closed && !ActiveNodes.Contains(node);
        }

        public List<Direction> GetOpenDirections(Int2 nodePosition)
        {
            List<Direction> openDirections = new List<Direction>();

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                if (GetNodeInDirection(nodePosition, direction, out _))
                    openDirections.Add(direction);

            return openDirections;
        }

        public void CloseNode(CrossNode node)
        {
            ActiveNodes.Remove(node);
            node.Closed = true;
            ClosedNodes.Add(node);
        }

        public CrossNodeSet(int width, int height)
        {
            nodeArray = new CrossNode[width, height];
            ActiveNodes = new List<CrossNode>();
            ClosedNodes = new List<CrossNode>();

            for (int xPosition = 0; xPosition < nodeArray.GetLength(0); xPosition++)
                for (int yPosition = 0; yPosition < nodeArray.GetLength(1); yPosition++)
                    nodeArray[xPosition, yPosition] = new CrossNode(xPosition, yPosition);
        }
    }
}
