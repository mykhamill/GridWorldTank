using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace GridWorld
{
    public class mhamill: BasePlayer
    {
        PlayerWorldState myWorldState;
        Dictionary<Point, GridSquare> memory;

        public mhamill() : base()
        {
            this.Name = "Tank1";
            memory = new Dictionary<Point, GridSquare>();
        }

        public override ICommand GetTurnCommands(IPlayerWorldState igrid)
        {
            myWorldState = (PlayerWorldState)igrid;
            updateMemory(myWorldState);
            Debug.WriteLine(memoryToString(memory));
            Debug.WriteLine(myWorldState);
            mloc();
            findEnemies(myWorldState);
            Trace.WriteLine(String.Format("Move {0}, Fire? {1}", Command.Move.Right, false ? "Yes" : "No"));
            return new Command(Command.Move.Right, false);
        }

        public void updateMemory(PlayerWorldState state)
        {
            foreach (var square in state.MyVisibleSquares.ToList())
            {
                var s = new Point(square.X, square.Y);
                if (memory.ContainsKey(s))
                {
                    memory.Remove(s);
                }
                memory.Add(s, new GridSquare(square.X, square.Y, square.Contents, square.Player));
            }
        }

        public String memoryToString(Dictionary<Point, GridSquare> mem)
        {
            String result = "";
            var mx = this.myWorldState.GridWidthInSquares;
            var my = this.myWorldState.GridHeightInSquares;
            for (var y = 0; y < my; y++)
            {
                for (var x = 0; x < mx; x++)
                {
                    var p = new Point(x, y);
                    if (mem.ContainsKey(p))
                    {
                        result += mem.SingleOrDefault(kv => kv.Key == p).Value.ContentString;
                    }
                    else
                    {
                        result += " ";
                    }
                }
                result += "\r\n";
            }
            return result;
        }

        public GridSquare mloc()
        {
            if (myWorldState != null)
            {
                Debug.WriteLine(myWorldState.MyGridSquare);
                return myWorldState.MyGridSquare;
            }
            return null;
        }

        public List<GridSquare> findEnemies(PlayerWorldState state)
        {
            var visibleEnemySquares = state.MyVisibleSquares.Where(square => square.Player > 0 && square.Player != this.ID).ToList();
            foreach (var square in visibleEnemySquares)
            {
                Debug.WriteLine(String.Format("Player: {0} at {1}", square.Player, square));
            }
            return visibleEnemySquares;
        }
    }
}