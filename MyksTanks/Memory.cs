using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GridWorld
{
    class Memory
    {
        Dictionary<Point, GridSquare> mem;
        int maxWidth;
        int maxHeight;

        public Memory()
        {
            mem = new Dictionary<Point, GridSquare>();
            maxWidth = 0;
            maxHeight = 0;
        }

        public String convertContent(GridSquare.ContentType ct)
        {
            switch(ct)
            {
                case GridSquare.ContentType.DestroyedTank: return "#";
                case GridSquare.ContentType.Empty: return ".";
                case GridSquare.ContentType.Rock: return "*";
                case GridSquare.ContentType.TankDown: return "v";
                case GridSquare.ContentType.TankLeft: return "<";
                case GridSquare.ContentType.TankRight: return ">";
                case GridSquare.ContentType.TankUp: return "^";
                default: return " ";
            }
        }

        public void update(PlayerWorldState state)
        {
            maxWidth = state.GridWidthInSquares;
            maxHeight = state.GridHeightInSquares;
            var s = state.MyVisibleSquares;
            foreach (var sq in s)
            {
                var p = new Point(sq.X, sq.Y);
                mem[p] = sq;
            }
        }

        public override string ToString()
        {
            var result = new List<String>();
            for (var y = 0; y < maxHeight; y++)
            {
                result.Add("");
                for (var x = 0; x < maxWidth; x++)
                {
                    var p = new Point(x, y);
                    if (mem.ContainsKey(p))
                    {
                        result[result.ToArray().Length-1] += convertContent(mem.SingleOrDefault(kv => kv.Key == p).Value.Contents);
                    }
                    else
                    {
                        result[result.ToArray().Length - 1] += " ";
                    }
                }
            }
            return result.Aggregate((a, b) => b + "\r\n" + a);
        }
    }
}
