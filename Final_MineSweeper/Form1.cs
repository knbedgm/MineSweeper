using Final_MineSweeper.GameLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_MineSweeper.Forms
{
	public partial class MainWindow : Form
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		#region Global Variables


		/// <summary>
		/// list of a list of the tiles in the game
		/// </summary>
		MineField field;

		// ints
#pragma warning disable IDE0044 // Add readonly modifier

		int height = 9;			// height of game board
		int width = 9;			// width of game board
		int mineCount = 10;		// number of mines in the game

#pragma warning restore IDE0044 // Add readonly modifier

		TimeSpan highscore = new TimeSpan(long.MaxValue); // highscore, set to long.MaxValue when not set

		DateTime startTime = DateTime.MinValue; // game start time, is set to MinValue when clock not running

		#endregion

		#region Functions

		/// <summary>
		/// Setup a new game
		/// </summary>
		public void NewGame()
		{
			NewMinefield(); // Creates a blank minefield

			field.GameStateChangedEventHandler += OnGameOver; // add gameover event handler

			canvas.Invalidate(); // updates canvas
			startTime = DateTime.MinValue; // reset the start time
			timer1.Start(); // start the timer
		}

		/// <summary>
		/// Creates a new minefield
		/// </summary>
		public void NewMinefield()
		{
			field = new MineField(height, width, mineCount);
		}

		#endregion

		#region Events

		/// <summary>
		/// Game State Change Handler
		/// </summary>
		public void OnGameOver(Object Sender, EventArgs e)
		{
			canvas.Invalidate(); // updates canvas

			if (field.State == MineField.GameState.Lose)
			{
				// open every tile to show user the correct positions
				field.ForEachTile(dat => {
					if (dat.State == Tile.TileState.flagged && dat.IsBomb) return;
					field.SetTileState(dat, Tile.TileState.revealed); 
				});

				canvas.Invalidate(); // update canvas

				timer1.Stop(); // stop timer
				DateTime now = DateTime.Now; // get current time
				TimeSpan time = now.Subtract(startTime); // subtract start time from current time

				MessageBox.Show(String.Format("You lost the game after {0:0.###} seconds", time.TotalSeconds), "You Lost", MessageBoxButtons.OK, MessageBoxIcon.Error); // Message displaying you loss and your time
			}
			else if (field.State == MineField.GameState.Win)
			{
				field.ForEachTile(dat => { if (dat.IsBomb)field.SetTileState(dat, Tile.TileState.flagged); });

				timer1.Stop(); // stop timer
				DateTime now = DateTime.Now; // get current time
				TimeSpan time = now.Subtract(startTime); // subtract start time from current time

				bool recordSet = false;
				if (time.CompareTo(highscore) == -1) // if time is less than highscore
				{
					recordSet = true;
					highscore = time;
				}
				// this lines a little confusing. the input for {1} in string.format comes from a turnary operator that either tells you the current record or that you set a new one based on if you set a record.
				MessageBox.Show(String.Format("You won the game in {0:0.###} seconds!\r\n{1}", time.TotalSeconds, recordSet ? "That's a new highscore!" : $"The current highscore is {highscore.TotalSeconds.ToString("0.###")} seconds."), "You Won!", MessageBoxButtons.OK, MessageBoxIcon.Information); // Message displaying you won and your time
			}
		}


		/// <summary>
		/// on Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender, EventArgs e)
		{
			// setup the minefield array so it doesn't fail when trying to draw
			NewGame();
		}

		/// <summary>
		/// Canvas on paint listener
		/// </summary>
		/// <param name="sender">sender object</param>
		/// <param name="e">Event Args</param>
		private void Canvas_Paint(object sender, PaintEventArgs e)
		{
			field.ForEachTile(dat => dat.Draw(e.Graphics));
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
		}

		private void Canvas_MouseClick(object sender, MouseEventArgs e)
		{
			if (field.State != MineField.GameState.Running) return;

			//find tile clicked
			int x, y;
			x = e.X / (TileVisuals.size);
			y = e.Y / (TileVisuals.size);

			// tell the tile it was clicked
			//mineField[x][y].Click(e);
			TileData data = field.GetTileData(x, y);
			Tile.TileState state = data.State;

			if (state == Tile.TileState.hidden && e.Button == MouseButtons.Left) // if state of mouse click is left mouse click
			{
				field.OpenTile(data); // open tile

			}
			else if (state == Tile.TileState.hidden && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click
			{
				field.FlagTile(data); // state is clicked and is flagged
			}
			else if (state == Tile.TileState.flagged && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click on a flagged tile
			{
				field.UnFlagTile(data); // state is unclicked and flag is removed
			}
			else if (state == Tile.TileState.revealed && e.Button == MouseButtons.Right) // if state of mouse click is right mouse click on a tile
			{
				// open up minefields

				if (field.GetTileData(x, y).FlagsNear == field.GetTileData(x, y).Danger) // if number of flags is equailvalent to bombs
				{
					field.ForTileNeighbours(data, (newData) => field.OpenTile(newData) );// open up more tiles
				}
			}

			//calculate the number of flags remaining and display that to the user
			lblFlags.Text = field.FlagsRemaining.ToString();

			// re-draw the screen
			canvas.Invalidate();
			//if the timer hasn't been started then start it;
			if (startTime.CompareTo(DateTime.MinValue) == 0) startTime = DateTime.Now;
		}

		/// <summary>
		/// Called every 0.1 seconds by the timer to update the timer display
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Timer1_Tick(object sender, EventArgs e)
		{
			if (startTime.CompareTo(DateTime.MinValue) == 0) return;
			if (field.State != MineField.GameState.Running) return;

			DateTime now = DateTime.Now;
			TimeSpan time = now.Subtract(startTime);
			lblTimer.Text = Math.Round(time.TotalSeconds).ToString();
		}
		private void ShowHighscoreToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (highscore.Ticks == long.MaxValue)
				MessageBox.Show("No score has been set yet.", "Highscore", MessageBoxButtons.OK, MessageBoxIcon.Information);
			else
				MessageBox.Show($"The current highscore is {highscore.TotalSeconds.ToString("0.###")}", "Highscore", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		#endregion

	}

	public static class TileVisuals
	{
		public static int size = 30;  // size of the tile (30x30) when drawn
		public static void Draw(this TileData tile, Graphics g)
		{
			if (tile.State.Equals(Tile.TileState.hidden)) // if clicked state is unclicked
			{
				// Fill
				SolidBrush b = new SolidBrush(Color.FromArgb(192, 192, 192));
				g.FillRectangle(b, tile.X * size, tile.Y * size, size, size);

				// Border
#pragma warning disable IDE0017 // Simplify object initialization
				Pen p = new Pen(Color.Black, 2);
#pragma warning restore IDE0017 // Simplify object initialization

				p.Color = Color.FromArgb(126, 126, 126);
				g.DrawLine(p, tile.X * size + size - 1, tile.Y * size, tile.X * size + size - 1, tile.Y * size + size); // draw right edge
				g.DrawLine(p, tile.X * size, tile.Y * size + size - 1, tile.X * size + size, tile.Y * size + size - 1); // draw bottom edge

				p.Color = Color.White;
				g.DrawLine(p, tile.X * size + 1, tile.Y * size, tile.X * size + 1, tile.Y * size + size); // draw left edge
				g.DrawLine(p, tile.X * size, tile.Y * size + 1, tile.X * size + size, tile.Y * size + 1); // draw top edge

			}
			else if (tile.State.Equals(Tile.TileState.revealed)) // if clicked state is clicked
			{
				// Fill
				SolidBrush b = new SolidBrush(Color.FromArgb(192, 192, 192));
				g.FillRectangle(b, tile.X * size, tile.Y * size, size, size);

				// Border
				Pen p = new Pen(Color.FromArgb(133, 133, 133), 2);

				g.DrawLine(p, tile.X * size + 1, tile.Y * size, tile.X * size + 1, tile.Y * size + size); // draw left edge
				g.DrawLine(p, tile.X * size, tile.Y * size + 1, tile.X * size + size, tile.Y * size + 1); // draw top edge

				if (tile.IsBomb)
				{
					// Draw Bomb
					b.Color = Color.Black;
					g.FillEllipse(b, tile.X * size + 5, tile.Y * size + 5, size - 10, size - 10);
				}
				else
				{
					switch (tile.Danger) // make text correct color
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

					g.DrawString(tile.Danger.ToString(), new Font("Arial", 18), b, new RectangleF(tile.X * size, tile.Y * size, size - 1, size - 1));
				}
			}
			else if (tile.State.Equals(Tile.TileState.flagged)) // if clicked state is flagged
			{
				// fill
				SolidBrush b = new SolidBrush(Color.FromArgb(192, 192, 192));
				g.FillRectangle(b, tile.X * size, tile.Y * size, size, size);

				// Border
#pragma warning disable IDE0017 // Simplify object initialization
				Pen p = new Pen(Color.Black, 2);
#pragma warning restore IDE0017 // Simplify object initialization

				p.Color = Color.FromArgb(126, 126, 126);
				g.DrawLine(p, tile.X * size + size - 1, tile.Y * size, tile.X * size + size - 1, tile.Y * size + size); // draw right edge
				g.DrawLine(p, tile.X * size, tile.Y * size + size - 1, tile.X * size + size, tile.Y * size + size - 1); // draw bottom edge

				p.Color = Color.White;
				g.DrawLine(p, tile.X * size + 1, tile.Y * size, tile.X * size + 1, tile.Y * size + size); // draw left edge
				g.DrawLine(p, tile.X * size, tile.Y * size + 1, tile.X * size + size, tile.Y * size + 1); // draw top edge

				// Flags
				b.Color = Color.Red;
				g.FillEllipse(b, tile.X * size + 5, tile.Y * size + 5, size - 10, size - 10);


			}
		}
	}

}
