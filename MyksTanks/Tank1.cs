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

            return new Command(Command.Move.Right, false);
            //return null; // Return the command “Do nothing at all!”
        }

        public GridSquare GetMyLocation(PlayerWorldState state)
        {
            Debug.WriteLine(state.MyGridSquare);
            return null;
        }
    }
}