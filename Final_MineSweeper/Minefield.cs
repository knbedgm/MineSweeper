using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_MineSweeper.GameLogic
{
    public class MineField
    {
        #region Globals
        public readonly int Height;
        public readonly int Width;

        private readonly Tile[,] tiles;

        private bool generateMines = false;
        private int generateMineCount = 0;

        private GameState _state = GameState.Setup;
        public GameState State { get => _state; private set { _state = value; HandleStateChange(); } }
        #endregion

        public enum GameState
        {
            Setup,
            Running,
            Win,
            Lose
        }

        #region Getters and Setters
        public Tile.TileState GetTileState(int x, int y) => tiles[x, y].State;
        public void SetTileState(int x, int y, Tile.TileState state)
        {
            tiles[x, y].State = state;
            HandleTileStateChange(tiles[x, y]);
        }
        public int GetTileDanger(int x, int y) => tiles[x, y].State == Tile.TileState.revealed ? tiles[x, y].BombsNear : -1;

        //public Tile.TileState[,] TileStates
        //{
        //	get
        //	{
        //		var val = new Tile.TileState[tiles.GetLength(0), tiles.GetLength(1)];
        //		for (int i = 0; i < val.GetLength(0); i++)
        //		{
        //			for (int j = 0; j < val.GetLength(1); j++)
        //			{
        //				val[i, j] = tiles[i, j].State;
        //			}
        //		}
        //		return val;
        //	}
        //}
        public int MineCount { get => tiles.Cast<Tile>().Count(t => t.IsBomb); }
        public int FlagCount { get => tiles.Cast<Tile>().Count(t => t.State == Tile.TileState.flagged); }
        #endregion

        public MineField(int height, int width)
        {
            // setup vars
            Height = height;
            Width = width;
            tiles = new Tile[Width, Height];

            // initalise array
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = new Tile(i, j);
                }
            }

            // add tile neighbours
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles.Cast<Tile>().Where(t =>
                    {
                        return (t.X >= i - 1 &&
                                t.X <= i + 1 &&
                                t.Y >= j - 1 &&
                                t.Y <= j + 1 &&
                                (t.X != i && t.Y != j));
                    })
                        .ToList().ForEach(t => tiles[i, j].AddNeighbour(t));
                }
            }
        }
        public MineField(int h, int w, int bombs) : this(h, w)
        {
            generateMines = true;
            generateMineCount = bombs;
            _state = GameState.Running;

        }

        #region Methods
        private void ForEachTile(Action<Tile> del)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    del(tiles[i, j]);
                }
            }
        }

        private void HandleStateChange()
        {
            OnGameStateChanged();
        }

        private void HandleTileStateChange(Tile t)
        {
            if (generateMines && State == GameState.Running)
            {
                generateMines = false;

                t.State = Tile.TileState.revealed;
                t.IsBomb = false;
                t.Neighbours.ForEach(x =>
                {
                    x.State = Tile.TileState.revealed;
                    x.IsBomb = false;
                });

                Random r = new Random();

                while (generateMineCount > MineCount)
                {
                    Tile rtile = tiles[r.Next(tiles.GetLength(0)), r.Next(tiles.GetLength(1))];
                    if (rtile.State != Tile.TileState.revealed)
                    {
                        rtile.IsBomb = true;
                    }
                }
            }

            IsGameOver();
            if (State == GameState.Running)
            {
                OnTileStateChanged(new TileStateChangedEventArgs() { X = t.X, Y = t.Y, newState = t.State });
            }

        }

        private void IsGameOver()
        {
            bool loss = false;
            bool win = true;
            tiles.Cast<Tile>().ToList().ForEach((Tile t) =>
            {
                if (t.IsBomb && t.State == Tile.TileState.revealed)
                { // if a bomb is clicked
                    loss = true; // you have loss

                }
                if (!t.IsBomb && t.State != Tile.TileState.revealed)
                { // if the tile is not a bomb and has not been opened
                    win = false; // you haven't won yet
                }
            });
            if (loss) { State = GameState.Lose; }
            else if (win) { State = GameState.Win; }
        }
        #endregion

        #region Events

        #region GameStateChanged Event
        public event EventHandler GameStateChangedEventHandler;

        private void OnGameStateChanged()
        {
            GameStateChangedEventHandler?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region TileStateChanged Event
        public event EventHandler<TileStateChangedEventArgs> TileStateChangedEventHandler;

        public class TileStateChangedEventArgs : EventArgs
        {
            
            public int X;
            public int Y;
            public Tile.TileState newState;
        }

        private void OnTileStateChanged(TileStateChangedEventArgs args)
        {
            TileStateChangedEventHandler?.Invoke(this, args);
        }
        #endregion

        #endregion
    }

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
        public int BombsNear { get { return Neighbours.Count(t => t.IsBomb); } } // number of bombs surounding the tile


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
                if (BombsNear == 0) // if bombs near is 0
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
                if (flagsNear == BombsNear) // if number of flags is equailvalent to bombs
                {
                    Neighbours.ForEach((Tile t) => t.Click(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0))); // open up more tiles
                }
            }
        }



        #endregion

    }
}

