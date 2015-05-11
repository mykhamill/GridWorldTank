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
        bool Hunting;
        
        public mhamill() : base()
        {
            this.Name = "Tank1";
            memory = new Memory();
            lastCommand = null;
            lastDirection = GridSquare.ContentType.Empty;
        }

        public override ICommand GetTurnCommands(IPlayerWorldState igrid)
        {
            myWorldState = (PlayerWorldState)igrid;
            memory.update(myWorldState);
            Debug.WriteLine(memory);
            Debug.WriteLine(myWorldState);
            Command command;
            if (lastDirection == GridSquare.ContentType.Empty || (lastCommand.CommandMove == Command.Move.RotateLeft && lastDirection != GridSquare.ContentType.TankDown))
            {
                command = new Command(Command.Move.RotateLeft, false);
            }
            else
            {
                command = new Command(Command.Move.Right, false);
            }
            lastDirection = mloc().Contents;
            lastCommand = command;
            findEnemies(myWorldState);
            findEnemiesInMemory();
            foreach (var edgeSquare in memory.edgeOfSeen())
            {
                Debug.WriteLine(String.Format("edge of Seen: {0} distance {1}", edgeSquare, memory.distance(mloc(), edgeSquare)));
            }
            Trace.WriteLine(String.Format("Move {0}, Fire? {1}", Command.Move.Right, false ? "Yes" : "No"));
            return command;
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