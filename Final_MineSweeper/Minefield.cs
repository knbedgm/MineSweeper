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
			this.x = x;
			this.y = y;
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

		public List<Tile> neighbours { get; private set; } = new List<Tile>(); // list of neighbour tiles

		public TileState state { get; set; } = TileState.hidden; // default clicked state is unclicked
		public bool isBomb { get;  set; } = false; // is the tile a bomb
		public int bombsNear { get { return neighbours.Where(t => t.isBomb).Count(); }} // number of bombs surounding the tile


		public int x { get; private set; }
		public int y { get; private set; } 
		// tile's location on the game board 

		#endregion

		#region Functions

		/// <summary>
		/// adds a neighbour to the tile and re-calculates the danger level
		/// </summary>
		/// <param name="t"> the tile to add as a neighbour</param>
		public void AddNeighbour(Tile t)
		{
			neighbours.Add(t);
		}

		/// <summary>
		/// Handles click event on the tile
		/// </summary>
		/// <param name="e">MouseEventArgs contining the mouse button pressed, location, press count and </param>
		public void Click(MouseEventArgs e)
		{
			if (state == TileState.hidden && e.Button == MouseButtons.Left) // if state of mouse click is left mouse click
			{
				state = TileState.revealed; // state of the clicked state is click
				if (bombsNear == 0) // if bombs near is 0
				{
					neighbours.ForEach(
						(Tile t) => {
							t.Click(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
						}
					);
				}
			}
			else if (state == TileState.hidden && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click
			{
				state = TileState.flagged; // state is clicked and is flagged
			}
			else if (state == TileState.flagged && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click on a flagged tile
			{
				state = TileState.hidden; // state is unclicked and flag is removed
			}
			else if (state == TileState.revealed && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click on a tile
			{
				// open up minefields
				int flagsNear = 0; // flags nearby is 0 
				neighbours.ForEach((Tile t) => { if (t.state == TileState.flagged) flagsNear++; }); // count how many flags are on the tiles surrounding this one
				if (flagsNear == bombsNear) // if number of flags is equailvalent to bombs
				{
					neighbours.ForEach((Tile t) => t.Click(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0))); // open up more tiles
				}
			}
		}



		#endregion

	}
}

