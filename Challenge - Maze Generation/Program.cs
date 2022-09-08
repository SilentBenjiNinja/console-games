using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


// TODO: generate maze, THEN put in start and finish
// TODO: debug: total node amount, pathfind efficiency (path length / walked nodes), status of maze gen / pathfinding (generating 55/100 (55%), amount of tunnelers, amount of open nodes, nodes walked 5/100 (5%))
namespace Maze_Generation
{
    class Program
    {
        static Program instance = new Program();

        const string PIXEL = "\u2588\u2588";

        // amount of possible crossroads per row/column
        const int W = 75;
        const int H = 35;

        const int DT = 5;   // delta time
        const int PT = 1000;   // pause time

        const bool IGNORE_CHANCES_FOR_SINGLES = true;
        //readonly double[] CHANCES = { 1, 0.2, 0.04 };
        readonly double[] CHANCES = { 1, 0.01, 0.0001 };

        const bool FILL_EMPTY_SPACES = true;

        const bool RANDOMIZE_START = true;
        const bool RANDOMIZE_END = true;
        Int2 startPos = new Int2 { x = 0, y = H - 1 };
        Int2 endPos = new Int2 { x = W - 1, y = 0 };

        CrossNode startNode;
        CrossNode endNode;
        CrossNodeSet set;

        bool[,] halls;

        Random randy = new Random();

        static void Main(string[] args)
        {
            Console.Title = "Maze Generator";
            Console.SetWindowSize(W * 4 + 2, H * 2 + 1);
            Console.CursorVisible = false;

            while (true)
            {
                instance.Setup();
                Thread.Sleep(PT);

                instance.Generate();
                Thread.Sleep(PT);

                instance.Solve();
                Thread.Sleep(PT);

                Console.Clear();
            }
        }

        #region Maze Generation
        private void Setup()
        {
            halls = new bool[W * 2 - 1, H * 2 - 1];

            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                    FillTile(new Int2 { x = i * 2, y = j * 2 }, ConsoleColor.DarkBlue);

            if (RANDOMIZE_START)
                startPos = new Int2 { x = randy.Next(W), y = randy.Next(H) };

            if (RANDOMIZE_END)
                endPos = new Int2 { x = randy.Next(W), y = randy.Next(H) };

            startNode = new CrossNode(startPos);
            FillTile(startNode.AbsPos, ConsoleColor.Green);

            endNode = new CrossNode(endPos);
            FillTile(endNode.AbsPos, ConsoleColor.Magenta);
        }

        private void Generate()
        {
            set = new CrossNodeSet(W, H, startNode, endNode);

            // add start point to open cross nodes
            set.OpenNodes.Add(startNode);

            // repeat until all cross nodes are handled:
            while (set.ClosedNodes.Count < W * H)
            {
                // pick a random node from open list; pick from closed if it's empty
                if (set.OpenNodes.Count == 0 && !FILL_EMPTY_SPACES) break;

                List<CrossNode> nodeList = set.OpenNodes.Count > 0 ? set.OpenNodes : set.ClosedNodes.Where(n => set.GetOpenDirections(n.Pos).Count > 0).ToList();
                CrossNode cn = nodeList[randy.Next(nodeList.Count)];

                foreach (var dir in set.GetOpenDirections(cn.Pos))
                    if (FILL_EMPTY_SPACES && set.GetNodeInDirection(cn.Pos, dir, out CrossNode n))
                        FillTile(n.AbsPos, ConsoleColor.DarkCyan, false);

                // release at least 1 walker in a non-closed direction
                List<Walker> walkers = GetWalkersForNode(cn.Pos);

                // for every walker:
                foreach (var w in walkers)
                {
                    CrossNode n;

                    // propagate if possible
                    if (set.GetNodeInDirection(w.startNodePos, w.dir, out n))
                    {
                        // create new cross node at destination
                        FillTile(n.AbsPos, ConsoleColor.Cyan);

                        // create a hallway to next node
                        FillTile(GetHallFromWalker(w));

                        // open cross node for next iteration
                        set.OpenNodes.Add(n);

                        Thread.Sleep(DT);
                    }
                }

                // set open cross node to closed if there are no neighboring empty nodes
                if (!cn.Closed)
                {
                    set.CloseNode(cn);

                    // overpaint node
                    FillTile(cn.AbsPos);
                }
            }

            // overpaint start and end
            FillTile(startNode.AbsPos, ConsoleColor.Green);
            FillTile(endNode.AbsPos, ConsoleColor.Magenta);
        }

        public Int2 GetHallFromWalker(Walker w)
        {
            Int2 hallwayPos = new Int2 { x = w.startNodePos.x * 2, y = w.startNodePos.y * 2 };

            switch (w.dir)
            {
                case Dir.Up:
                    hallwayPos.y -= 1;
                    break;
                case Dir.Rt:
                    hallwayPos.x += 1;
                    break;
                case Dir.Dn:
                    hallwayPos.y += 1;
                    break;
                case Dir.Lt:
                    hallwayPos.x -= 1;
                    break;
            }

            return hallwayPos;
        }

        public List<Walker> GetWalkersForNode(Int2 nodePos)
        {
            List<Walker> ret = new List<Walker>();
            List<Dir> dirs = set.GetOpenDirections(nodePos);

            for (int i = 0; i < 3; i++)
            {
                double chance = randy.NextDouble();
                if (IGNORE_CHANCES_FOR_SINGLES && i == 0 && set.OpenNodes.Count == 1)
                    chance = CHANCES[0];

                if (dirs.Count > 0 && chance <= CHANCES[i])
                {
                    Dir dir = dirs[randy.Next(dirs.Count)];
                    ret.Add(new Walker
                    {
                        startNodePos = nodePos,
                        dir = dir
                    });
                    dirs.Remove(dir);
                }
                else break;
            }

            return ret;
        }
        #endregion

        #region Solving A*
        private void Solve()
        {
            PathNode startTile = new PathNode { pos = startNode.AbsPos };
            PathNode endTile = new PathNode { pos = endNode.AbsPos };

            startTile.GCost = 0;

            List<PathNode> open = new List<PathNode>();
            List<PathNode> closed = new List<PathNode>();

            open.Add(startTile);

            while (open.Count > 0)
            {
                PathNode currentTile = open[0];
                for (int i = 0; i < open.Count; i++)
                    if (open[i].FCost < currentTile.FCost || (open[i].FCost == currentTile.FCost && open[i].HCost < currentTile.HCost))
                        currentTile = open[i];

                open.Remove(currentTile);
                closed.Add(currentTile);

                Thread.Sleep(DT);
                FillTile(currentTile.pos, ConsoleColor.Green, false);

                if (currentTile.Equals(endTile))
                {
                    // solved
                    endTile = currentTile;

                    Thread.Sleep(DT);
                    FillTile(currentTile.pos, ConsoleColor.DarkMagenta, false);

                    while (currentTile.parent != null)
                    {
                        Thread.Sleep(DT);
                        FillTile(currentTile.parent.pos, ConsoleColor.Magenta, false);
                        currentTile = currentTile.parent;
                    }

                    return;
                }

                foreach (PathNode pn in currentTile.Neighbors)
                {
                    if (!pn.Traversible || closed.Contains(pn)) continue;

                    int newGCost = currentTile.GCost + 1;

                    if (newGCost < pn.GCost || !open.Contains(pn))
                    {
                        pn.GCost = newGCost;
                        pn.parent = currentTile;

                        if (!open.Contains(pn))
                        {
                            open.Add(pn);

                            FillTile(pn.pos, ConsoleColor.DarkYellow, false);
                        }
                    }
                }
            }

            // lands here if while loop is over, which means the maze it is not solvable
            // however, if FILL_EMPTY_SPACES == true, the maze generated will always be solvable
        }

        class PathNode : IEquatable<PathNode>
        {
            public Int2 pos;

            // G and H cost are measured in Euclidian distance in this case
            public int FCost => GCost + 2 * HCost;
            public int GCost { get; set; }
            public int HCost => (int)pos.Dist(instance.endNode.AbsPos);

            public bool Traversible => instance.halls[pos.x, pos.y];

            public PathNode parent;
            public List<PathNode> Neighbors
            {
                get
                {
                    List<PathNode> ret = new List<PathNode>();

                    if (pos.x > 0)
                        ret.Add(new PathNode { pos = new Int2 { x = pos.x - 1, y = pos.y } });
                    if (pos.y > 0)
                        ret.Add(new PathNode { pos = new Int2 { x = pos.x, y = pos.y - 1 } });
                    if (pos.x < W * 2 - 2)
                        ret.Add(new PathNode { pos = new Int2 { x = pos.x + 1, y = pos.y } });
                    if (pos.y < H * 2 - 2)
                        ret.Add(new PathNode { pos = new Int2 { x = pos.x, y = pos.y + 1 } });

                    return ret;
                }
            }

            public bool Equals(PathNode other)
            {
                return pos.Equals(other.pos);
            }
        }
        #endregion

        #region Helpers
        private void FillTile(Int2 pos, ConsoleColor col = ConsoleColor.White, bool makeHall = true)
        {
            if (makeHall)
                halls[pos.x, pos.y] = true;

            Console.ForegroundColor = col;
            pos.SetCursor();
            Console.Write(PIXEL);
        }
        #endregion
    }

    class CrossNode
    {
        Int2 pos;
        public Int2 Pos => pos;
        public Int2 AbsPos => new Int2 { x = pos.x * 2, y = pos.y * 2 };
        public bool Closed { get; set; }

        public CrossNode(int x, int y)
        {
            this.pos = new Int2 { x = x, y = y };
            this.Closed = false;
        }

        public CrossNode(Int2 pos)
        {
            this.pos = new Int2 { x = pos.x, y = pos.y };
            this.Closed = false;
        }
    }

    class CrossNodeSet
    {
        CrossNode[,] nodeArray;
        public List<CrossNode> OpenNodes { get; private set; }
        public List<CrossNode> ClosedNodes { get; private set; }

        public CrossNode this[Int2 nodePos]
        {
            get => nodeArray[nodePos.x, nodePos.y];
            // set => nodeArray[x, y] = value;
        }

        public bool GetNodeInDirection(Int2 nodePos, Dir dir, out CrossNode node)
        {
            node = null;

            switch (dir)
            {
                case Dir.Up:
                    nodePos.y -= 1;
                    if (nodePos.y < 0) return false;
                    break;
                case Dir.Rt:
                    nodePos.x += 1;
                    if (nodePos.x >= nodeArray.GetLength(0)) return false;
                    break;
                case Dir.Dn:
                    nodePos.y += 1;
                    if (nodePos.y >= nodeArray.GetLength(1)) return false;
                    break;
                case Dir.Lt:
                    nodePos.x -= 1;
                    if (nodePos.x < 0) return false;
                    break;
            }

            node = nodeArray[nodePos.x, nodePos.y];

            return !node.Closed && !OpenNodes.Contains(node);
        }

        public List<Dir> GetOpenDirections(Int2 nodePos)
        {
            List<Dir> ret = new List<Dir>();

            foreach (Dir item in Enum.GetValues(typeof(Dir)))
                if (GetNodeInDirection(nodePos, item, out _))
                    ret.Add(item);

            return ret;
        }

        public void CloseNode(CrossNode node)
        {
            OpenNodes.Remove(node);
            node.Closed = true;
            ClosedNodes.Add(node);
        }

        public CrossNodeSet(int w, int h, CrossNode start, CrossNode end)
        {
            nodeArray = new CrossNode[w, h];
            OpenNodes = new List<CrossNode>();
            ClosedNodes = new List<CrossNode>();

            for (int i = 0; i < nodeArray.GetLength(0); i++)
                for (int j = 0; j < nodeArray.GetLength(1); j++)
                    nodeArray[i, j] = new CrossNode(i, j);

            nodeArray[start.Pos.x, start.Pos.y] = start;
            nodeArray[end.Pos.x, end.Pos.y] = end;
        }
    }

    enum Dir { Up, Rt, Dn, Lt, }

    struct Int2
    {
        public int x
        {
            get;
            set;
        }

        public int y
        {
            get;
            set;
        }

        public void SetCursor()
        {
            Console.SetCursorPosition(2 + x * 2, 1 + y);
        }

        public double Dist(Int2 other)
        {
            return Math.Sqrt((x - other.x) * (x - other.x) + (y - other.y) * (y - other.y));
        }

        public bool Equals(Int2 other)
        {
            return x == other.x && y == other.y;
        }
    }

    struct Walker
    {
        public Int2 startNodePos;
        public Dir dir;
    }
}
