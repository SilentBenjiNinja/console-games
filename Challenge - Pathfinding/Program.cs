using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinding
{
    class Program
    {
        static Program instance = new Program();

        const int size = 15;
        int sizeSq => size * size;
        float Sqrt2 => (float)Math.Sqrt(2);

        static void Main(string[] args)
        {
            instance.SomeMethod();
        }

        // TODO: rewrite grid generation method
        // TODO: restore other stuff
        public void SomeMethod()
        {
            for (int i = 0; i < sizeSq; i++)
            {
                // top center
                if (i > size - 1)
                    Nodes[i].NeighborNodes.Add(
                        new CellNodePath(Nodes[i] as CellNode, Nodes[i - size] as CellNode, Sqrt2));

                // top right
                if (i > size - 1 && i % size != size - 1)
                    Nodes[i].NeighborNodes.Add(
                        new CellNodePath(Nodes[i] as CellNode, Nodes[i - size + 1] as CellNode, Sqrt2));

                // center right
                if (i % size != size - 1)
                    Nodes[i].NeighborNodes.Add(
                        new CellNodePath(Nodes[i] as CellNode, Nodes[i + 1] as CellNode, Sqrt2));

                // bottom right
                if (i < sizeSq - size && i % size != size - 1)
                    Nodes[i].NeighborNodes.Add(
                        new CellNodePath(Nodes[i] as CellNode, Nodes[i + size + 1] as CellNode, Sqrt2));

                // bottom center
                if (i < sizeSq - size)
                    Nodes[i].NeighborNodes.Add(
                        new CellNodePath(Nodes[i] as CellNode, Nodes[i + size] as CellNode, Sqrt2));

                // bottom left
                if (i < sizeSq - size && i % size != 0)
                    Nodes[i].NeighborNodes.Add(
                        new CellNodePath(Nodes[i] as CellNode, Nodes[i + size - 1] as CellNode, Sqrt2));

                // center left
                if (i % size != 0)
                    Nodes[i].NeighborNodes.Add(
                        new CellNodePath(Nodes[i] as CellNode, Nodes[i - 1] as CellNode));

                // top left
                if (i > size - 1 && i % size != 0)
                    Nodes[i].NeighborNodes.Add(
                        new CellNodePath(Nodes[i] as CellNode, Nodes[i - size - 1] as CellNode, Sqrt2));
            }

            //Random r = new Random();
            //for (int i = 1; i < sizeSq - 1; i++)
            //    if (r.Next(5) < 1)
            //        (Nodes[i] as CellNode).Blocked = true;
        }

        public void Draw()
        {
            Console.SetWindowSize(size * 2 + 4, size + 2);
            Console.CursorVisible = false;
            Console.Clear();

            for (int i = 0; i < sizeSq; i++)
            {
                if (i % size == 0) Console.Write("\n  ");
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("ST");
                    Console.ResetColor();
                    continue;
                }
                else { }
            }
        }
    }
}