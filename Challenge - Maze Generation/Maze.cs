using System;
using System.Collections.Generic;

// TODO: multi-genesis?
// TODO: loops?
// TODO: rectangular rooms?
// TODO: start and target are always a min distance apart
// TODO: sample multiple rounds of generating and solving
namespace Challenges.MazeGeneration
{
    class Maze
    {
        CrossNode genesisNode;
        CrossNode pathfindStartNode;
        CrossNode pathfindTargetNode;

        CrossNodeSet nodeSet;

        bool[,] pathNodes;

        int pathNodesTotal = 0;

        public void Setup()
        {
            Helper.DebugStat(1, "State", "Setup...");
            Helper.ResetStopwatch();

            pathNodes = new bool[MazeConfig.WIDTH * 2 - 1, MazeConfig.HEIGHT * 2 - 1];
            pathNodesTotal = 0;

            for (int x = 0; x < MazeConfig.WIDTH; x++)
                for (int y = 0; y < MazeConfig.HEIGHT; y++)
                    Helper.DrawTile(new Int2 { X = x * 2, Y = y * 2 }, NodeColorKey.EmptyNode);

            nodeSet = new CrossNodeSet(MazeConfig.WIDTH, MazeConfig.HEIGHT);

            genesisNode = nodeSet[MazeConfig.GenesisPosition];
            CreateTunnel(genesisNode.AbsolutePosition, NodeColorKey.TunnelerNode);
        }

        #region Maze Generation
        public void Generate()
        {
            Helper.DebugStat(1, "State", "Generating...");

            // add genesis point to active cross nodes
            nodeSet.ActiveNodes.Add(genesisNode);

            // repeat until all cross nodes are handled:
            while (nodeSet.ClosedNodes.Count < MazeConfig.WIDTH * MazeConfig.HEIGHT)
            {
                // exit loop when no more tunnels need to be made
                if (nodeSet.ActiveNodes.Count == 0 && !MazeConfig.FILL_ALL_NODES) break;

                // pick a random node from active list; pick from closed if it's empty
                List<CrossNode> nodeList = nodeSet.ActiveNodes.Count > 0 ? nodeSet.ActiveNodes : nodeSet.OpenNodes;

                CrossNode currentNode = nodeList[Helper.Randy.Next(nodeList.Count)];

                foreach (Direction openDirection in nodeSet.GetOpenDirections(currentNode.Position))
                    if (MazeConfig.FILL_ALL_NODES && nodeSet.GetNodeInDirection(currentNode.Position, openDirection, out CrossNode node))
                        Helper.DrawTile(node.AbsolutePosition, NodeColorKey.OpenDirection);

                // release at least one tunneler in an open direction
                foreach (Tunneler tunneler in GetTunnelersForNode(currentNode.Position))
                {
                    // propagate if possible
                    if (nodeSet.GetNodeInDirection(tunneler.startNodePosition, tunneler.direction, out CrossNode node))
                    {
                        // open cross node for next iteration
                        nodeSet.ActiveNodes.Add(node);

                        // create a tunnel to next node
                        CreateTunnel(tunneler.GetTunnel(), NodeColorKey.Tunnel);

                        // create new cross node at destination
                        CreateTunnel(node.AbsolutePosition, NodeColorKey.TunnelerNode);
                    }
                }

                // set open cross node to closed if there are no neighboring empty nodes
                if (!currentNode.Closed)
                {
                    nodeSet.CloseNode(currentNode);

                    Helper.DrawTile(currentNode.AbsolutePosition, NodeColorKey.Tunnel);
                }

                Helper.DeltaTimeGenerator();

                int totalCrossNodes = MazeConfig.WIDTH * MazeConfig.HEIGHT;
                float progress = (float)nodeSet.ClosedNodes.Count / totalCrossNodes;

                Helper.DebugStat(2, "Generation settings", Helper.GetGenerationSettings());
                Helper.DebugStat(3, "Generation status", $"{Helper.GetPercentageString(progress)} ({nodeSet.ClosedNodes.Count}/{totalCrossNodes})");
                Helper.DebugStat(4, "Open nodes", $"{nodeSet.OpenNodes.Count}");
                Helper.DebugStat(5, "Active tunnelers", $"{nodeSet.ActiveNodes.Count}");
            }

            Helper.DebugStat(1, "State", $"Generation done ({Helper.StopwatchTimeString})");
            Helper.DebugStat(6, "Path nodes populated", $"{pathNodesTotal}");
        }

        List<Tunneler> GetTunnelersForNode(Int2 nodePosition)
        {
            List<Tunneler> tunnelers = new List<Tunneler>();
            List<Direction> directions = nodeSet.GetOpenDirections(nodePosition);

            for (int i = 0; i < MazeConfig.BranchingChances.Length; i++)
            {
                double chance = Helper.Randy.NextDouble();

                if (directions.Count > 0 && chance <= MazeConfig.BranchingChances[i])
                {
                    Direction direction = directions[Helper.Randy.Next(directions.Count)];
                    tunnelers.Add(new Tunneler
                    {
                        startNodePosition = nodePosition,
                        direction = direction
                    });
                    directions.Remove(direction);
                }
                else break;
            }

            return tunnelers;
        }
        #endregion

        #region Solving A*
        public void Solve()
        {
            // generate and draw start and target nodes
            pathfindStartNode = nodeSet[MazeConfig.PathfindStartPosition];
            Helper.DrawTile(pathfindStartNode.AbsolutePosition, NodeColorKey.PathfindStartNode);

            pathfindTargetNode = nodeSet[MazeConfig.PathfindTargetPosition];
            Helper.DrawTile(pathfindTargetNode.AbsolutePosition, NodeColorKey.PathfindTargetNode);

            Helper.Pause();
            Helper.DebugStat(1, "State", "Solving...");
            Helper.ResetStopwatch();

            PathNode startNode = new PathNode { position = pathfindStartNode.AbsolutePosition };
            PathNode targetNode = new PathNode { position = pathfindTargetNode.AbsolutePosition };

            startNode.GCost = 0;

            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                PathNode currentNode = openList[0];
                for (int i = 0; i < openList.Count; i++)
                    if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost))
                        currentNode = openList[i];

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                Helper.DeltaTimeSolver();

                string euclideanDistance = Helper.GetDecimalString(startNode.position.Dist(targetNode.position));
                Helper.DebugStat(2, "Algorithm", "A*");
                Helper.DebugStat(3, "Euclidean distance", $"{euclideanDistance}");
                Helper.DebugStat(4, "Search distance (G cost)", $"{currentNode.GCost}");
                Helper.DebugStat(5, "Visited (closed)", $"{closedList.Count}");
                Helper.DebugStat(6, "Active seekers (open)", $"{openList.Count}");
                Helper.DebugStat(7, "G cost / visited (efficiency)", $"{Helper.GetPercentageString((float)currentNode.GCost / closedList.Count)} ({currentNode.GCost} / {closedList.Count})");
                Helper.DebugStat(8, "Euclidean / G cost (directness)", $"{Helper.GetPercentageString((float)startNode.position.Dist(targetNode.position) / currentNode.GCost)} ({euclideanDistance} / {currentNode.GCost})");
                Helper.DebugStat(9, "Visited / total (coverage)", $"{Helper.GetPercentageString((float)closedList.Count / pathNodesTotal)} ({closedList.Count} / {pathNodesTotal})");

                if (!currentNode.Equals(startNode))
                    Helper.DrawTile(currentNode.position, NodeColorKey.PathfinderClosedNode);

                if (currentNode.Equals(targetNode))
                {
                    // solved
                    targetNode = currentNode;

                    Helper.DebugStat(1, "State", $"Solution found! Tracing...");
                    //Helper.ResetStopwatch();

                    Helper.DrawTile(currentNode.position, NodeColorKey.PathfindTargetNode);

                    while (currentNode.parent != null)
                    {
                        Helper.DeltaTimeTracer();
                        Helper.DrawTile(currentNode.parent.position, NodeColorKey.PathfindBackPropagation);
                        currentNode = currentNode.parent;
                    }

                    Helper.DrawTile(currentNode.position, NodeColorKey.PathfindStartNode);

                    Helper.DebugStat(1, "State", $"Solved! ({Helper.StopwatchTimeString})");
                    Helper.Pause();
                    Helper.Pause();
                    Helper.Pause();

                    return;
                }

                foreach (PathNode pathNode in currentNode.Neighbors)
                {
                    if (!IsPathAtPosition(pathNode.position) || closedList.Contains(pathNode)) continue;

                    int newGCost = currentNode.GCost + 1;

                    if (newGCost < pathNode.GCost || !openList.Contains(pathNode))
                    {
                        pathNode.GCost = newGCost;
                        pathNode.parent = currentNode;

                        if (!openList.Contains(pathNode))
                        {
                            openList.Add(pathNode);

                            Helper.DrawTile(pathNode.position, NodeColorKey.PathfinderOpenNode);
                        }
                    }
                }
            }

            // lands here if while loop is over, which means the maze is not solvable
            // however, if FILL_EMPTY_SPACES == true, the maze generated will always be solvable
        }

        class PathNode : IEquatable<PathNode>
        {
            public Int2 position;

            // G and H cost are measured in Euclidian distance in this case
            public int FCost => GCost + HCost * MazeConfig.HEURISTICWEIGHT;
            public int GCost { get; set; }
            public int HCost => (int)position.Dist(Program.instance.maze.pathfindTargetNode.AbsolutePosition);

            public PathNode parent;
            public List<PathNode> Neighbors
            {
                get
                {
                    List<PathNode> neighbors = new List<PathNode>();

                    if (position.X > 0)
                        neighbors.Add(new PathNode { position = new Int2 { X = position.X - 1, Y = position.Y } });
                    if (position.Y > 0)
                        neighbors.Add(new PathNode { position = new Int2 { X = position.X, Y = position.Y - 1 } });
                    if (position.X < MazeConfig.WIDTH * 2 - 2)
                        neighbors.Add(new PathNode { position = new Int2 { X = position.X + 1, Y = position.Y } });
                    if (position.Y < MazeConfig.HEIGHT * 2 - 2)
                        neighbors.Add(new PathNode { position = new Int2 { X = position.X, Y = position.Y + 1 } });

                    return neighbors;
                }
            }

            public bool Equals(PathNode other) => position.Equals(other.position);
        }
        #endregion

        #region Helpers
        private void CreateTunnel(Int2 position, NodeColorKey colorKey)
        {
            SetPathAtPosition(position);

            Helper.DrawTile(position, colorKey);
        }

        bool IsPathAtPosition(Int2 position) => pathNodes[position.X, position.Y];

        void SetPathAtPosition(Int2 position)
        {
            if (!pathNodes[position.X, position.Y])
                pathNodesTotal += 1;

            pathNodes[position.X, position.Y] = true;
        }
        #endregion
    }
}
