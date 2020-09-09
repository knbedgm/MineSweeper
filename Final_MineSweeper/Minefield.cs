using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (t.BombsNear == 0)
            {
                t.Neighbours.ForEach(x => SetTileState(x.X, x.Y, Tile.TileState.revealed));
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
}

