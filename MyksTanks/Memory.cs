using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace GridWorld
{
    class Memory : IEnumerable<GridSquare>
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

        public String convertContent(GridSquare ct)
        {
            if (ct == null) return " ";
            switch(ct.Contents)
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
                        result[result.ToArray().Length - 1] += convertContent(mem.SingleOrDefault(kv => kv.Key == p).Value);
                    }
                    else
                    {
                        result[result.ToArray().Length - 1] += " ";
                    }
                }
            }
            return result.Aggregate((a, b) => b + "\r\n" + a);
        }

        public IEnumerator<GridSquare> GetEnumerator()
        {
            return mem.Select(v => v.Value).GetEnumerator(); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mem.Select(v => v.Value).GetEnumerator();
        }

        public List<String> surrounding(GridSquare sq)
        {
            List<String> result = new List<String>();
            if (sq.X == 0)
            {
                if (sq.Y == 0)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y + 1).Value));
                }
                else if (sq.Y < maxHeight - 1)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y + 1).Value));
                }
                else
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                }
            }
            else if (sq.X < maxWidth - 1)
            {
                if (sq.Y == 0)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y + 1).Value));
                }
                else if (sq.Y < maxHeight - 1)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y + 1).Value));
                }
                else
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                }
            }
            else
            {
                if (sq.Y == 0)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                }
                else if (sq.Y < maxHeight - 1)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                }
                else
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                }
            } 
            return result;
        }
    }
}
