using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Diagnostics;

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
            var edgeSquares = edgeOfSeen();
            for (var y = 0; y < maxHeight; y++)
            {
                result.Add("");
                for (var x = 0; x < maxWidth; x++)
                {
                    var p = new Point(x, y);
                    if (mem.ContainsKey(p))
                    {
                        var sq = mem.SingleOrDefault(kv => kv.Key == p).Value;
                        if (edgeSquares.Contains(sq))
                        {
                            result[result.ToArray().Length - 1] += "@"; 
                        }
                        else
                        {
                            result[result.ToArray().Length - 1] += convertContent(sq);
                        }
                    }
                    else
                    {
                        result[result.ToArray().Length - 1] += " ";
                    }
                }
            }
            return result.Aggregate((a, b) => b + "\r\n" + a);
        }

        public int distance(GridSquare g1, GridSquare g2)
        {
            Point p1 = new Point(g1.X, g1.Y);
            Point p2 = new Point(g2.X, g2.Y);
            var x = (p2.X - p1.X);
            var y = (p2.Y - p1.Y);
            int d = (x+y);
            return d;
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
                }
                else if (sq.Y < maxHeight - 1)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                }
                else
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                }
            }
            else if (sq.X < maxWidth - 1)
            {
                if (sq.Y == 0)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                }
                else if (sq.Y < maxHeight - 1)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                }
                else
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X + 1 && kv.Key.Y == sq.Y).Value));
                }
            }
            else
            {
                if (sq.Y == 0)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                }
                else if (sq.Y < maxHeight - 1)
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y + 1).Value));
                }
                else
                {
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X - 1 && kv.Key.Y == sq.Y).Value));
                    result.Add(convertContent(mem.SingleOrDefault(kv => kv.Key.X == sq.X && kv.Key.Y == sq.Y - 1).Value));
                }
            } 
            return result;
        }

        public List<GridSquare> edgeOfSeen()
        {
            var seenSquares = mem.Select(kv => kv.Value).Where(square => square.Contents == GridSquare.ContentType.Empty && surrounding(square).Any(s => s == " ")).ToList();
            
            return seenSquares;
        }
    }
}
