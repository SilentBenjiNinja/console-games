using System;
using System.Collections.Generic;

namespace Challenges.MazeGeneration
{
    static class ColorConfig
    {
        public static Dictionary<NodeColorKey, ConsoleColor> colorDictionary = new Dictionary<NodeColorKey, ConsoleColor>() {
            { NodeColorKey.EmptyNode, ConsoleColor.DarkGray },
            { NodeColorKey.Tunnel, ConsoleColor.White },
            { NodeColorKey.OpenDirection, ConsoleColor.DarkCyan },
            { NodeColorKey.TunnelerNode, ConsoleColor.Cyan },
            { NodeColorKey.PathfindStartNode, ConsoleColor.Red },
            { NodeColorKey.PathfindTargetNode, ConsoleColor.DarkGreen },
            { NodeColorKey.PathfinderOpenNode, ConsoleColor.DarkYellow },
            { NodeColorKey.PathfinderClosedNode, ConsoleColor.Yellow },
            { NodeColorKey.PathfindBackPropagation, ConsoleColor.Green },
        };
    }
}
