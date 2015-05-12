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
        
        public mhamill() : base()
        {
            this.Name = "Tank1";
            memory = new Memory();
            lastCommand = null;
            lastDirection = GridSquare.ContentType.Empty;
            travelDirection = PlayerWorldState.Facing.Right;
        }

        public override ICommand GetTurnCommands(IPlayerWorldState igrid)
        {
            myWorldState = (PlayerWorldState)igrid;
            memory.update(myWorldState);
            Debug.WriteLine(memory);
            Debug.WriteLine(myWorldState);
            Command command = followRightwall(myWorldState);
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
            Command result = null;
            if (lastCommand == null)
            {
                lastCommand = new Command(Command.Move.RotateRight, true);
            }
            GridSquare front = null;
            Command forward = null;
            GridSquare rightSide = null;
            GridSquare leftSide = null;
            GridSquare loc = mloc();
            switch (travelDirection) {
                case PlayerWorldState.Facing.Right:
                    front = borderWall(loc, true, 1, state.GridWidthInSquares);
                    leftSide = borderWall(loc, false, 1, state.GridHeightInSquares);
                    rightSide = borderWall(loc, false, -1, state.GridHeightInSquares);
                    forward = new Command(Command.Move.Right, true);
                    break;
                case PlayerWorldState.Facing.Down:
                    front = borderWall(loc, false, -1, state.GridHeightInSquares);
                    leftSide = borderWall(loc, true, 1, state.GridWidthInSquares);
                    rightSide = borderWall(loc, true, -1, state.GridWidthInSquares);
                    forward = new Command(Command.Move.Down, true);
                    break;
                case PlayerWorldState.Facing.Left:
                    front = borderWall(loc, true, -1, state.GridWidthInSquares);
                    leftSide = borderWall(loc, false, -1, state.GridHeightInSquares);
                    rightSide = borderWall(loc, false, 1, state.GridHeightInSquares);
                    forward = new Command(Command.Move.Left, true);
                    break;
                case PlayerWorldState.Facing.Up:
                    front = borderWall(loc, false, 1, state.GridHeightInSquares);
                    leftSide = borderWall(loc, true, -1, state.GridWidthInSquares);
                    rightSide = borderWall(loc, true, 1, state.GridWidthInSquares);
                    forward = new Command(Command.Move.Up, true);
                    break;
            }

            if (leftSide == null)
            {
                leftSide = new GridSquare(0, 0, GridSquare.ContentType.Empty);
            }
            if (rightSide == null)
            {
                rightSide = new GridSquare(0, 0, GridSquare.ContentType.Empty);
            }
            if (front == null)
            {
                return new Command(Command.Move.RotateRight, true);
            }

            bool turnedLastMove = (lastCommand.CommandMove == Command.Move.RotateLeft || lastCommand.CommandMove == Command.Move.RotateRight);

            if (turnedLastMove && front.Contents == GridSquare.ContentType.Empty) result = forward;
            if (turnedLastMove && front.Contents != GridSquare.ContentType.Empty) result = new Command(Command.Move.RotateLeft, true);
            if (!turnedLastMove && rightSide.Contents == GridSquare.ContentType.Empty) result = new Command(Command.Move.RotateRight, true);
            if (!turnedLastMove && rightSide.Contents != GridSquare.ContentType.Empty && front.Contents == GridSquare.ContentType.Empty) result = forward;
            if (!turnedLastMove && rightSide.Contents != GridSquare.ContentType.Empty && front.Contents != GridSquare.ContentType.Empty) result = new Command(Command.Move.RotateLeft, true);

            if (result.CommandMove == Command.Move.RotateRight)
            {
                switch (travelDirection)
                {
                    case PlayerWorldState.Facing.Right: travelDirection = PlayerWorldState.Facing.Down; break;
                    case PlayerWorldState.Facing.Up: travelDirection = PlayerWorldState.Facing.Right; break;
                    case PlayerWorldState.Facing.Left: travelDirection = PlayerWorldState.Facing.Up; break;
                    case PlayerWorldState.Facing.Down: travelDirection = PlayerWorldState.Facing.Left; break;
                }
                lastCommand = result;
                result = followRightwall(state);
            }
            else if (result.CommandMove == Command.Move.RotateLeft)
            {
                switch (travelDirection)
                {
                    case PlayerWorldState.Facing.Right: travelDirection = PlayerWorldState.Facing.Up; break;
                    case PlayerWorldState.Facing.Up: travelDirection = PlayerWorldState.Facing.Left; break;
                    case PlayerWorldState.Facing.Left: travelDirection = PlayerWorldState.Facing.Down; break;
                    case PlayerWorldState.Facing.Down: travelDirection = PlayerWorldState.Facing.Right; break;
                }
                lastCommand = result;
                result = followRightwall(state);
            } 
             
            return result;
        }

        public GridSquare borderWall(GridSquare g, bool XorY, int vX, int maxX)
        {
            GridSquare borderWallSquare = new GridSquare(0, 0, GridSquare.ContentType.Rock);
            if (vX < 0)
            {
                if (XorY)
                {
                    return g.X > 0 ? memory.SingleOrDefault(sq => sq.X == g.X + vX && sq.Y == g.Y) : borderWallSquare;
                }
                else
                {
                    return g.Y > 0 ? memory.SingleOrDefault(sq => sq.X == g.X && sq.Y == g.Y + vX) : borderWallSquare;
                }
            }
            if (vX > 0)
            {
                if (XorY)
                {
                    return g.X < (maxX - 1) ? memory.SingleOrDefault(sq => sq.X == g.X + vX && sq.Y == g.Y) : borderWallSquare;
                }
                else
                {
                    return g.Y < (maxX - 1)? memory.SingleOrDefault(sq => sq.X == g.X && sq.Y == g.Y + vX) : borderWallSquare;
                }
            }
            if (XorY)
            {
                return memory.SingleOrDefault(sq => sq.X == g.X + vX && sq.Y == g.Y);
            }
            else
            {
                return memory.SingleOrDefault(sq => sq.X == g.X && sq.Y == g.Y + vX);
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