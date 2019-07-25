using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace battlepong_game.Settings {
    public class Controls {
        public Key playerOneUp = Key.Up;
        public Key playerOneDown = Key.S;
        public Key playerTwoUp = Key.Up;
        public Key playerTwoDown = Key.Down;
        public Key playerOneQuitKey = Key.Escape;
        public Key playerTwoQuitKey = Key.End;
        public Key pauseKey = Key.P; //opens option menu
        public Key testKey = Key.T;
        public Key startKey = Key.Enter;
        public Key player1Enable = Key.LeftShift;
        public Key player2Enable = Key.RightShift;
        public Key restart = Key.R;

        public Key KeyUp   // This is your property
        {
            get => playerOneUp;
            set => playerOneDown = value;
        }
    }

}
