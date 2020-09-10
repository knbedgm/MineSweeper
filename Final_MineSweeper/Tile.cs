using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace Final_MineSweeper.GameLogic
{
    public class Tile
    {
        /// <summary>
        /// Initalises the tile object
        /// </summary>
        /// <param name="x">horisontal location</param>
        /// <param name="y">vertical location</param>
        public Tile(int x, int y)
        {
            // set the location
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Possible tile states
        /// </summary>
        public enum TileState
        {
            hidden,
            revealed,
            flagged
        }

        #region Global Variables

        public List<Tile> Neighbours { get; private set; } = new List<Tile>(); // list of neighbour tiles
        public TileState State { get; set; } = TileState.hidden; // default clicked state is unclicked
        public bool IsBomb { get; set; } = false; // is the tile a bomb
        public int Danger { get { return Neighbours.Count(t => t.IsBomb); } } // number of bombs surounding the tile
        public int FlagsNear { get => Neighbours.Count(t => t.State == TileState.flagged); }


        public int X { get; private set; }
        public int Y { get; private set; }
        // tile's location on the game board 

        #endregion

        #region Functions

        /// <summary>
        /// adds a neighbour to the tile and re-calculates the danger level
        /// </summary>
        /// <param name="t"> the tile to add as a neighbour</param>
        public void AddNeighbour(Tile t)
        {
            Neighbours.Add(t);
        }

        /// <summary>
        /// Handles click event on the tile
        /// </summary>
        /// <param name="e">MouseEventArgs contining the mouse button pressed, location, press count and </param>
        public void Click(MouseEventArgs e)
        {
            if (State == TileState.hidden && e.Button == MouseButtons.Left) // if state of mouse click is left mouse click
            {
                State = TileState.revealed; // state of the clicked state is click
                if (Danger == 0) // if bombs near is 0
                {
                    Neighbours.ForEach(
                        (Tile t) =>
                        {
                            t.Click(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        }
                    );
                }
            }
            else if (State == TileState.hidden && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click
            {
                State = TileState.flagged; // state is clicked and is flagged
            }
            else if (State == TileState.flagged && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click on a flagged tile
            {
                State = TileState.hidden; // state is unclicked and flag is removed
            }
            else if (State == TileState.revealed && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click on a tile
            {
                // open up minefields
                int flagsNear = 0; // flags nearby is 0 
                Neighbours.ForEach((Tile t) => { if (t.State == TileState.flagged) flagsNear++; }); // count how many flags are on the tiles surrounding this one
                if (flagsNear == Danger) // if number of flags is equailvalent to bombs
                {
                    Neighbours.ForEach((Tile t) => t.Click(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0))); // open up more tiles
                }
            }
        }

        public TileData ExtractData(bool ShowBomb)
        {
            return new TileData()
            {
                Danger = Danger,
                IsBomb = ShowBomb ? IsBomb : false,
                FlagsNear = FlagsNear,
                State = State,
                X = X,
                Y = Y
            };
        }

        #endregion

    }
}

