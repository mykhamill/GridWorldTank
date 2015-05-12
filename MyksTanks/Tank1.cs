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
        Memory memory;
        Command lastCommand;
        GridSquare.ContentType lastDirection;
        PlayerWorldState.Facing travelDirection;        
        Dictionary<String, Command> movements = new Dictionary<string, Command>();
        Dictionary<String, GridSquare> compassPoints = new Dictionary<string, GridSquare>();
        Queue<Command> commandQueue = new Queue<Command>();

        public mhamill() : base()
        {
            this.Name = "Tank1";
            memory = new Memory();
            lastCommand = new Command(Command.Move.Right, true);
            lastDirection = GridSquare.ContentType.Empty;
            travelDirection = PlayerWorldState.Facing.Right;
            movements.Add("n", new Command(Command.Move.Up, true));
            movements.Add("e", new Command(Command.Move.Left, true));
            movements.Add("s", new Command(Command.Move.Down, true));
            movements.Add("w", new Command(Command.Move.Right, true));
            
        }

        public override ICommand GetTurnCommands(IPlayerWorldState igrid)
        {
            myWorldState = (PlayerWorldState)igrid;
            memory.update(myWorldState);
            Debug.WriteLine(memory);
            Debug.WriteLine(myWorldState);
            commandQueue.Enqueue(followRightwall(myWorldState));
            Command command = (commandQueue.Count == 0) ? followRightwall(myWorldState) : commandQueue.Dequeue();
            lastDirection = mloc().Contents;
            lastCommand = command;
            findEnemies(myWorldState);
            findEnemiesInMemory();
            foreach (var edgeSquare in memory.edgeOfSeen())
            {
                Debug.WriteLine(String.Format("edge of Seen: {0} distance {1}", edgeSquare, memory.distance(mloc(), edgeSquare)));
            }
            Trace.WriteLine(String.Format("Move {0}, Fire? {1}", command.CommandMove, command.CommandShoot ? "Yes" : "No"));
            return command;
        }

        public Command followRightwall(PlayerWorldState state)
        {
            GridSquare loc = mloc();
            compassPoints.Clear();
            compassPoints.Add("n", borderWall(loc, 0, 1, state));
            compassPoints.Add("ne", borderWall(loc, -1, 1, state));
            compassPoints.Add("e", borderWall(loc, -1, 0, state));
            compassPoints.Add("se", borderWall(loc, -1, -1, state));
            compassPoints.Add("s", borderWall(loc, 0, -1, state));
            compassPoints.Add("w", borderWall(loc, 1, 0, state));
            compassPoints.Add("sw", borderWall(loc, 1, -1, state));
            compassPoints.Add("nw", borderWall(loc, 1, 1, state));

            if (compassPoints.Any(kv => kv.Value == null))
            {
                return rotateGun360();
            }

            String compassValue = compassPoints.Select((kv, i) => kv.Value.Contents == GridSquare.ContentType.Empty ? "0" : "1").Aggregate((a, b) => b + a);
            //                      next command
            //                           move up             move left             move down             move right
            // lastcommand move up
            // priority                4                   2                     1                     3
            //     ne  n  nw             x 0 x               x 1 x                 x 1 x                 x x x         
            //     e   ^   w             x ^ x               0 ^ 1                 1 ^ 1                 x ^ 0
            //     se  s  sw             x x x               x x x                 x x x                 x x 1
            //           
            // lastcommand move left
            // priority                3                   3                     2                     1
            //     ne  n  nw             x 0 1               x x x                 x 1 x                 x 1 x                              
            //     e   <   w             x < x               0 < x                 1 < x                 1 < x
            //     se  s  sw             x x x               x x x                 x 0 x                 x 1 x
            //
            // lastcommand move down
            // priority                1                   3                     4                     2
            //     ne  n  nw             x x x               1 x x                 x x x                 x x x
            //     e   V   w             1 V 1               0 V x                 x V x                 1 V 0
            //     se  s  sw             x 1 x               x x x                 x 0 x                 x 1 x
            //
            // lastcommand move right
            // priority                2                   1                     3                     4
            //     ne  n  nw             x 0 x               x 1 x                 x x x                 x x x
            //     e   >   w             x > 1               x > 1                 x > x                 x > 0
            //     se  s  sw             x 1 x               x 1 x                 1 0 x                 x x x
            //
            //               0  1  2  3  4  5  6  7
            //compassValue = n,ne, e,se, s,sw, w,nw => 0,1,1,1,1,1,0,0
            var cVQ = new Queue<Char>(compassValue);
            if (lastCommand.CommandMove == Command.Move.Left)
            {
                for (int i = 0; i < 2; i++) cVQ.Enqueue(cVQ.Dequeue());
            }
            if (lastCommand.CommandMove == Command.Move.Down)
            {
                for (int i = 0; i < 4; i++) cVQ.Enqueue(cVQ.Dequeue());
            }
            if (lastCommand.CommandMove == Command.Move.Right)
            {
                for (int i = 0; i < 6; i++) cVQ.Enqueue(cVQ.Dequeue());
            }

            var cV = new String(cVQ.ToArray());
            if (cV[2] == '1' && cV[0] == '1' && cV[6] == '1')
            {
                return new Command(Command.Move.Down, true);
            }
            else if (cV[2] == '0' && cV[0] == '1' && cV[6] == '1')
            {
                return new Command(Command.Move.Left, true);
            }
            else if (cV[5] == '1' && cV[6] == '0') {
                return new Command(Command.Move.Right,true);
            }
            else
            {
                return new Command(Command.Move.Up, true);
            }
        }

        public Command rotateGun360()
        {
            commandQueue.Enqueue(new Command(Command.Move.RotateRight, true));
            commandQueue.Enqueue(new Command(Command.Move.RotateRight, true));
            commandQueue.Enqueue(new Command(Command.Move.RotateRight, true));
            return new Command(Command.Move.RotateRight, true);
        }

        public GridSquare memSoD(int X, int Y)
        {
            return memory.SingleOrDefault(sq => sq.X == X && sq.Y == Y);
        }

        public GridSquare borderWall(GridSquare g, int X, int Y, PlayerWorldState state)
        {
            Point max = new Point(state.GridWidthInSquares, state.GridHeightInSquares);
            
            GridSquare borderWallSquare = new GridSquare(0, 0, GridSquare.ContentType.Rock);
            if (X < 0)
            {
                if (Y < 0)
                {
                    return (g.X > 0 && g.Y > 0) ? memSoD(g.X + X, g.Y + Y) : borderWallSquare;
                }
                else if (Y > 0)
                {
                    return (g.X > 0 && g.Y < (max.Y - 1)) ? memSoD(g.X + X, g.Y + Y) : borderWallSquare;
                }
                else
                {
                    return (g.X > 0) ? memSoD(g.X + X, g.Y + Y) : borderWallSquare;
                }
            }
            if (X > 0)
            {
                if (Y < 0)
                {
                    return (g.X < (max.X - 1) && g.Y > 0) ? memSoD(g.X + X, g.Y + Y) : borderWallSquare;
                }
                else if (Y > 0)
                {
                    return (g.X < (max.X - 1) && g.Y < (max.Y - 1)) ? memSoD(g.X + X, g.Y + Y) : borderWallSquare;
                }
                else
                {
                    return (g.X < (max.X - 1)) ? memSoD(g.X + X, g.Y + Y) : borderWallSquare;
                }
            }
            if (Y < 0)
            {
                return (g.Y > 0) ? memSoD(g.X + X, g.Y + Y) : borderWallSquare;
            }
            else if (Y > 0)
            {
                return (g.Y < (max.Y - 1)) ? memSoD(g.X + X, g.Y + Y) : borderWallSquare;
            }
            else
            {
                return memSoD(g.X + X, g.Y + Y);
            }
        }
        

        public GridSquare mloc()
        {
            if (myWorldState != null)
            {
                //Debug.WriteLine(myWorldState.MyGridSquare);
                return myWorldState.MyGridSquare;
            }
            return null;
        }

        public List<GridSquare> findEnemies(PlayerWorldState state)
        {
            var visibleEnemySquares = state.MyVisibleSquares.Where(square => square.Player > 0 && square.Player != this.ID).ToList();
            foreach (var square in visibleEnemySquares)
            {
                Debug.WriteLine(String.Format("Visible Player: {0} at {1}", square.Player, square));
            }
            return visibleEnemySquares;
        }

        public List<GridSquare> findEnemiesInMemory()
        {
            var rememberedEnemySquares = memory.Where(square => square.Player > 0 && square.Player != this.ID).ToList();
            foreach (var square in rememberedEnemySquares)
            {
                Debug.WriteLine(String.Format("Remembered Player: {0} at {1}", square.Player, square));
            }
            return rememberedEnemySquares; 
        }
    }
}