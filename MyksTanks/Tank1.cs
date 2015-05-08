using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GridWorld
{
    public class mhamill: BasePlayer
    {
        PlayerWorldState myWorldState;

        public mhamill() : base()
        {
            this.Name = "Tank1";
        }

        public override ICommand GetTurnCommands(IPlayerWorldState igrid)
        {
            myWorldState = (PlayerWorldState)igrid;
            Debug.WriteLine(myWorldState);
            mloc();
            findEnemies(myWorldState);
            Trace.WriteLine(String.Format("Move {0}, Fire? {1}", Command.Move.Right, false ? "Yes" : "No"));
            return new Command(Command.Move.Right, false);
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