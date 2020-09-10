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

        #region Tile Data Helpers

        public TileData GetTileData(int x, int y)
        {
            return GetTileData(tiles[x, y]);
        }
        private TileData GetTileData(Tile t)
        {
            return t.ExtractData(State != GameState.Running);
        }
        public void SetTileState(int x, int y, Tile.TileState state)
        {
            //if (State == GameState.Win || State == GameState.Lose) return;
            if (tiles[x, y].State == Tile.TileState.revealed) return;
            tiles[x, y].State = state;
            HandleTileStateChange(tiles[x, y]);
        }
        public void SetTileState(TileData data, Tile.TileState state) => SetTileState(data.X, data.Y, state);

        public void OpenTile(TileData data)
        {
            if (data.State == Tile.TileState.hidden)
                SetTileState(data, Tile.TileState.revealed);
        }

        public void FlagTile(TileData data)
        {
            if (data.State == Tile.TileState.hidden)
                SetTileState(data, Tile.TileState.flagged);
        }

        public void UnFlagTile(TileData data)
        {
            if (data.State == Tile.TileState.flagged)
                SetTileState(data, Tile.TileState.hidden);
        }


        public int MineCount { get {
                if (generateMines) return generateMineCount;
                return tiles.Cast<Tile>().Count(t => t.IsBomb); 
            } }
        public int FlagCount {
            get => tiles.Cast<Tile>().Count(t => t.State == Tile.TileState.flagged); 
        }
        public int FlagsRemaining {
            get => MineCount - FlagCount; 
        }
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
                                t.Y <= j + 1);
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
        public void ForTileNeighbours(int x, int y, Action<int, int> del)
        {
            tiles[x, y].Neighbours.ForEach(t => del(t.X, t.Y));
        }

        public void ForTileNeighbours(TileData t, Action<TileData> del)
        {
            tiles[t.X, t.Y].Neighbours.ForEach(x => del(GetTileData(x)));
        }

        public void ForEachTile(Action<TileData> del)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    del(GetTileData(i, j));
                }
            }
        }

        private void HandleStateChange()
        {
            OnGameStateChanged();
        }

        private void HandleTileStateChange(Tile t)
        {
            if (generateMines && State == GameState.Running && t.State == Tile.TileState.revealed)
            {
                generateMines = false;

                t.State = Tile.TileState.revealed;
                t.IsBomb = false;
                t.Neighbours.ForEach(x =>
                {
                    if (x.State == Tile.TileState.hidden)
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

                t.Neighbours.ForEach(x =>
                {
                    HandleTileStateChange(x);
                });

            }

            if (t.Danger == 0 && t.State == Tile.TileState.revealed)
            {
                t.Neighbours.ForEach(x => OpenTile(GetTileData(x)));
            }

            IsGameOver();
            if (State == GameState.Running)
            {
                OnTileStateChanged(new TileStateChangedEventArgs() { X = t.X, Y = t.Y, newState = t.State });
            }

        }

        private void IsGameOver()
        {
            if (State != GameState.Running) return;
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

    public class TileData
    {
        public Tile.TileState State;
        public int Danger;
        public int FlagsNear;
        public bool IsBomb;
        public int X;
        public int Y;
    }
}

