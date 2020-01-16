﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_MineSweeper
{
	public partial class Form1 : Form
	{
		/// <summary>
		/// delegate function that is called with a tile input
		/// </summary>
		/// <param name="t">Tile</param>
		public delegate void tileCallback(Tile t);
		public Form1()
		{
			InitializeComponent();
		}

		// list of a list of tiles 
		List<List<Tile>> mineField = new List<List<Tile>>();
		
		// ints
		int height = 9, // height of game board
			width = 9, // width of game board
			mineCount = 10, // number of mines in the game
			flagsRemaining = 99; // number of flags that are left

		TimeSpan highscore = new TimeSpan(999, 9999, 999);

		bool running = false; // is the game running

		DateTime startTime = DateTime.MinValue; // game start time, is set to MinValue when clock not running
		/// <summary>
		/// on Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender, EventArgs e)
		{
			// setup the minefield array so it doesn't fail when trying to draw
			NewMinefield();
		}

		/// <summary>
		/// setup a new game
		/// </summary>
		public void NewGame()
		{
			NewMinefield(); // Creates a blank minefield
			GenerateMines(); // Adds mines to the minefield
			AddNeighbours(); // allow tiles to ineract with their neighbours
			flagsRemaining = mineCount; // initialise the flag count
			canvas.Invalidate(); // updates canvas
			startTime = DateTime.MinValue; // reset the start time
			timer1.Start(); // start the timer
		}

		/// <summary>
		/// Creates a new minefield
		/// </summary>
		public void NewMinefield()
		{
			mineField.Clear(); // clear minefield
			for (int i = 0; i < width; i++) // for each collumn
			{
				List<Tile> col = new List<Tile>(); // create a new list for the column
				for (int j = 0; j < height; j++) // for each row
				{
					col.Add(new Tile(i, j)); // add a tile in the collumn
				}
				mineField.Add(col); // add the collumn to the field
			}
		}

		/// <summary>
		/// Canvas on paint listener
		/// </summary>
		/// <param name="sender">sender object</param>
		/// <param name="e">Event Args</param>
		private void Canvas_Paint(object sender, PaintEventArgs e)
		{
			// loops over each tile and tells it to draw it self with the given graphics
			mineField.ForEach((List<Tile> col) => {
				col.ForEach((Tile t) => {
					t.Draw(e.Graphics);
				});
			});

		}

		/// <summary>
		/// Places the correct number of mines on the board
		/// Does not clear previously placed mines from the field
		/// </summary>
		public void GenerateMines() {
			int bombsAdded = 0; // counter for number of bombs placed
			Random r = new Random(); 

			// while the number of mines placed is less than the number that should be placed
			while (bombsAdded < mineCount) {
				// get new random position
				int x = r.Next(0,height);
				int y = r.Next(0,width);

				// if it isnt already a bomb
				if (!mineField[x][y].isBomb) {
					// set the tile as a bomb
					mineField[x][y].isBomb = true;
					bombsAdded++;
				}
			}
		}

		/// <summary>
		/// on Start button pressed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnStart_Click(object sender, EventArgs e)
		{
			NewGame();
			lblTimer.Text = "0";
			lblFlags.Text = mineCount.ToString();
			running = true;
		}

		private void Canvas_MouseClick(object sender, MouseEventArgs e)
		{
			if (!running) return;

			//find tile clicked
			int x, y;
			x = e.X/(Tile.size);
			y = e.Y/(Tile.size);

			// tell the tile it was clicked
			mineField[x][y].Click(e);
			
			//calculate the number of flags remaining and display that to the user
			flagsRemaining = CalculateRemainingFlags();
			lblFlags.Text = flagsRemaining.ToString();

			// re-draw the screen
			canvas.Invalidate();
			//if the timer hasn't been started then start it;
			if (startTime.CompareTo(DateTime.MinValue) == 0) startTime = DateTime.Now;

			//check if the game is over
			CheckGameOver();
		}

		/// <summary>
		/// Called every 0.1 seconds to update the timer display
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Timer1_Tick(object sender, EventArgs e)
		{
			if (startTime.CompareTo(DateTime.MinValue) == 0) return;
			if (!running) return;

			DateTime now = DateTime.Now;
			TimeSpan time = now.Subtract(startTime);
			lblTimer.Text = Math.Round(time.TotalSeconds).ToString();
		}

		/// <summary>
		/// loops over each tile in the field and adds its neighbours to the tiles neighbours array
		/// </summary>
		public void AddNeighbours()
		{
			mineField.ForEach((List<Tile> col) => {
				col.ForEach((Tile t) => {
					// for each tile add the tiles neighbours
					ForNeighbors(t, t.AddNeighbour);
				});
			});
		}

		/// <summary>
		/// Loops over the minefield array and counts the number of flags that havent been used
		/// </summary>
		/// <returns></returns>
		public int CalculateRemainingFlags()
		{
			int flags = mineCount; // set the flag counter to max value (number of mines)
			mineField.ForEach((List<Tile> col) => {
				col.ForEach((Tile t) => { // loop over each tile
					// if the tile is flaged
					if (t.state == Tile.ClickedState.flagged)
						flags--; // decrement flag counter
				});
			});
			return flags;
		}

		/// <summary>
		/// checks the passed tile and executes passed callback with each neighbour tile as parameter
		/// </summary>
		/// <param name="tile">Center Tile</param>
		/// <param name="callback">function called for each of tile's neighbours</param>
		public void ForNeighbors(Tile tile, tileCallback callback)
		{
			// in this case there is 9 different possibilites

			// if the tile is in the top right corner
			if (tile.x == 0 && tile.y == 0) 
			{
				// run calbacks
				callback(mineField[tile.x + 1][tile.y    ]);
				callback(mineField[tile.x + 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y+1]);
			}
			else if (tile.x == 0 && tile.y == height - 1) // tile is in the bottom right corner
			{
				// run calbacks
				callback(mineField[tile.x + 1][tile.y    ]);
				callback(mineField[tile.x + 1][tile.y - 1]);
				callback(mineField[tile.x    ][tile.y - 1]);
			}
			else if (tile.x == width - 1 && tile.y == 0) // tile is in the top left corner
			{
				// run calbacks
				callback(mineField[tile.x - 1][tile.y    ]);
				callback(mineField[tile.x - 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y + 1]);
			}
			else if (tile.x == width - 1 && tile.y == height - 1) // tile is in the bottom left corner
			{
				// run calbacks
				callback(mineField[tile.x - 1][tile.y    ]);
				callback(mineField[tile.x - 1][tile.y - 1]);
				callback(mineField[tile.x    ][tile.y - 1]);
			}
			else if (tile.x == 0) // tile is in the top row
			{
				// run calbacks
				callback(mineField[tile.x    ][tile.y - 1]);
				callback(mineField[tile.x + 1][tile.y - 1]);
				callback(mineField[tile.x + 1][tile.y    ]);
				callback(mineField[tile.x + 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y + 1]);
			}
			else if (tile.x == width - 1) // tile is in the bottom row
			{
				// run calbacks
				callback(mineField[tile.x    ][tile.y - 1]);
				callback(mineField[tile.x - 1][tile.y - 1]);
				callback(mineField[tile.x - 1][tile.y    ]);
				callback(mineField[tile.x - 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y + 1]);
			}
			else if (tile.y == 0) // tile is in the right collumn
			{
				// run calbacks
				callback(mineField[tile.x - 1][tile.y    ]);
				callback(mineField[tile.x - 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y + 1]);
				callback(mineField[tile.x + 1][tile.y + 1]);
				callback(mineField[tile.x + 1][tile.y    ]);
			}
			else if (tile.y == height - 1) // tile is in the left collumn
			{
				// run calbacks
				callback(mineField[tile.x - 1][tile.y    ]);
				callback(mineField[tile.x - 1][tile.y - 1]);
				callback(mineField[tile.x    ][tile.y - 1]);
				callback(mineField[tile.x + 1][tile.y - 1]);
				callback(mineField[tile.x + 1][tile.y    ]);
			}
			else // tile is in the center
			{
				// run calbacks
				callback(mineField[tile.x - 1][tile.y - 1]);
				callback(mineField[tile.x    ][tile.y - 1]);
				callback(mineField[tile.x + 1][tile.y - 1]);
				callback(mineField[tile.x + 1][tile.y    ]);
				callback(mineField[tile.x + 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y + 1]);
				callback(mineField[tile.x - 1][tile.y + 1]);
				callback(mineField[tile.x - 1][tile.y    ]);
			}
		}

		/// <summary>
		/// Checks if the game should be ended
		/// </summary>
		public void CheckGameOver()
		{
			bool loss = false;
			bool win = true;
			mineField.ForEach((List<Tile> col) => {
				col.ForEach((Tile t) => {
					if (t.isBomb && t.state == Tile.ClickedState.clicked){ // if a bomb is clicked
						loss = true; // you have loss

					} 
					if (!t.isBomb && t.state != Tile.ClickedState.clicked){ // if the tile is not a bomb and has not been opened
						win = false; // you haven't won yet
					}
				});
			});

			if (loss) {
				// click each tile to open them all
				mineField.ForEach((List<Tile> col) => {
					col.ForEach((Tile t) => {
						t.Click(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));	
					});
				});
				
				canvas.Invalidate(); // update canvas

				running = false; // stop running game

				timer1.Stop(); // stop timer
				DateTime now = DateTime.Now; // get current time
				TimeSpan time = now.Subtract(startTime); // subtract start time from current time

				MessageBox.Show(String.Format("You lost the game in {0:0.###} seconds", time.TotalSeconds), "You Lost", MessageBoxButtons.OK, MessageBoxIcon.Error); // Message displaying you loss and your time
			}
			else if (win){ 
				running = false; // stop running game

				timer1.Stop(); // stop timer
				DateTime now = DateTime.Now; // get current time
				TimeSpan time = now.Subtract(startTime); // subtract start time from current time

				MessageBox.Show(String.Format("You won the game in {0:0.###} seconds!", time.TotalSeconds), "You Won!", MessageBoxButtons.OK, MessageBoxIcon.Information); // Message displaying you won and your time
			}
		}

	}

	/// <summary>
	/// Tile Class to hold data and methods for each tile
	/// </summary>
	public class Tile
	{
		public enum ClickedState 
		{
			// 3 clicked state types
			unClicked,
			clicked,
			flagged
		}

		public List<Tile> neighbours = new List<Tile>(); // list of neighbour tiles

		public ClickedState state = ClickedState.unClicked; // default clicked state is unclicked
		public bool isBomb = false; // is the tile a bomb
		public byte bombsNear = 0; // number of bombs surounding the tile

		public static int size = 30;  // size of the tile (30x30) when drawn
		public int x, y; // tile's location on the game board 

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
        /// Calculates the danger lever (mines around the tile) with the neighbour tiles
        /// </summary>
		public void CalculateDanger()
		{
			bombsNear = 0;
			neighbours.ForEach((Tile t) => {
				if (t.isBomb)
					bombsNear++;
			});
		}

		/// <summary>
        /// adds a neighbour to the tile and re-calculates the danger level
        /// </summary>
        /// <param name="t"> the tile to add as a neighbour</param>
		public void AddNeighbour(Tile t)
		{
			neighbours.Add(t); 
			CalculateDanger();
		}

		/// <summary>
        /// Handles click event on the tile
        /// </summary>
        /// <param name="e">MouseEventArgs contining the mouse button pressed, location, press count and </param>
		public void Click(MouseEventArgs e)
		{
			if (state == ClickedState.unClicked && e.Button == MouseButtons.Left) // if state of mouse click is left mouse click
			{
				state = ClickedState.clicked; // state of the clicked state is click
				if (bombsNear == 0) // if bombs near is 0
				{
					neighbours.ForEach(
						(Tile t) => {
							t.Click(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
						}
					);
				}
			}
			else if (state == ClickedState.unClicked && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click
			{
				state = ClickedState.flagged; // state is clicked and is flagged
			}
			else if (state == ClickedState.flagged && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click on a flagged tile
			{
				state = ClickedState.unClicked; // state is unclicked and flag is removed
			}
			else if (state == ClickedState.clicked && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click on a tile
			{
				// open up minefields
				int flagsNear = 0; // flags nearby is 0 
				neighbours.ForEach((Tile t) => { if (t.state == ClickedState.flagged) flagsNear++; }); // count how many flags are on the tiles surrounding this one
				if (flagsNear == bombsNear) // if number of flags is equailvalent to bombs
				{
					neighbours.ForEach((Tile t) => t.Click(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0))); // open up more tiles
				}
			}
		}

		public void Draw(Graphics g)
		{
			if (state.Equals(ClickedState.unClicked)) // if clicked state is unclicked
			{
				// Fill
				SolidBrush b = new SolidBrush(Color.FromArgb(192, 192, 192));
				g.FillRectangle(b, x * size, y * size, size, size);

				// Border
				Pen p = new Pen(Color.Black, 2);

				p.Color = Color.FromArgb(126, 126, 126);
				g.DrawLine(p, x * size + size - 1, y * size, x * size + size - 1, y * size + size); // draw right edge
				g.DrawLine(p, x * size, y * size + size - 1, x * size + size, y * size + size - 1); // draw bottom edge

				p.Color = Color.White;
				g.DrawLine(p, x * size + 1, y * size, x * size + 1, y * size + size); // draw left edge
				g.DrawLine(p, x * size, y * size + 1, x * size + size, y * size + 1); // draw top edge

			}
			else if (state.Equals(ClickedState.clicked)) // if clicked state is clicked
			{
				// Fill
				SolidBrush b = new SolidBrush(Color.FromArgb(192, 192, 192));
				g.FillRectangle(b, x * size, y * size, size, size);

				// Border
				Pen p = new Pen(Color.FromArgb(133, 133, 133), 2);

				g.DrawLine(p, x * size + 1, y * size, x * size + 1, y * size + size); // draw left edge
				g.DrawLine(p, x * size, y * size + 1, x * size + size, y * size + 1); // draw top edge

				if (isBomb)
				{
					// Draw Bomb
					b.Color = Color.Black;
					g.FillEllipse(b, x * size + 5, y * size + 5, size - 10, size - 10);
				}
				else
				{
					switch (bombsNear) // make text correct color
					{
						case 1:
							b.Color = Color.Blue;
							break;
						case 2:
							b.Color = Color.Green;
							break;
						case 3:
							b.Color = Color.Red;
							break;
						case 4:
							b.Color = Color.Navy;
							break;
						case 5:
							b.Color = Color.FromArgb(128, 0, 0);
							break;
						case 6:
							b.Color = Color.FromArgb(0, 128, 128);
							break;
						case 7:
							b.Color = Color.Black;
							break;
						case 8:
							b.Color = Color.FromArgb(128, 128, 128);
							break;
						default:
							break;
					}

					g.DrawString(bombsNear.ToString(), new Font("Arial", 18), b, new RectangleF(x * size, y * size, size - 1, size - 1));
				}
			}
			else if (state.Equals(ClickedState.flagged)) // if clicked state is flagged
			{
				// fill
				SolidBrush b = new SolidBrush(Color.FromArgb(192, 192, 192));
				g.FillRectangle(b, x * size, y * size, size, size);

				// Border
				Pen p = new Pen(Color.Black, 2);

				p.Color = Color.FromArgb(126, 126, 126);
				g.DrawLine(p, x * size + size - 1, y * size, x * size + size - 1, y * size + size); // draw right edge
				g.DrawLine(p, x * size, y * size + size - 1, x * size + size, y * size + size - 1); // draw bottom edge

				p.Color = Color.White;
				g.DrawLine(p, x * size + 1, y * size, x * size + 1, y * size + size); // draw left edge
				g.DrawLine(p, x * size, y * size + 1, x * size + size, y * size + 1); // draw top edge

				// Flags
				b.Color = Color.Red;
				g.FillEllipse(b, x * size + 5, y * size + 5, size - 10, size - 10);


			}
		}	
	}
   
}
