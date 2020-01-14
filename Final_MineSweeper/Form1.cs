using System;
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
		public delegate void tileCallback(Tile t);
		public Form1()
		{
			InitializeComponent();
		}

		List<List<Tile>> mineField = new List<List<Tile>>();
		int height = 9, 
			width = 9, 
			mineCount = 10,
			flagsRemaining = 99;
		bool running = false;
		DateTime startTime = DateTime.MinValue;
		

		private void Form1_Load(object sender, EventArgs e)
		{
			NewMinefield();
			timer1.Start();
		}

		public void NewGame()
		{
			NewMinefield();
			GenerateMines();
			AddNeighbours();
			flagsRemaining = mineCount;
			canvas.Invalidate();
			startTime = DateTime.MinValue;
		}

		public void NewMinefield()
		{
			mineField.Clear();
			for (int i = 0; i < height; i++)
			{
				List<Tile> col = new List<Tile>();
				for (int j = 0; j < width; j++)
				{
					col.Add(new Tile(i, j));
				}
				mineField.Add(col);
			}
		}

		private void Canvas_Paint(object sender, PaintEventArgs e)
		{
			mineField.ForEach((List<Tile> col) => {
				col.ForEach((Tile t) => {
					t.Draw(e.Graphics);
				});
			});

		}

		public void GenerateMines() {
			int bombsAdded = 0;
			Random r = new Random();

			while (bombsAdded < mineCount) {
				int x = r.Next(0,height);
				int y = r.Next(0,width);

				if (!mineField[x][y].isBomb) {
					mineField[x][y].isBomb = true;
					bombsAdded++;
				}
			}
		}

		private void BtnStart_Click(object sender, EventArgs e)
		{
			NewGame();
			lblTimer.Text = "0";
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
		}

		private void Timer1_Tick(object sender, EventArgs e)
		{
			if (startTime.CompareTo(DateTime.MinValue) == 0) return;
			if (!running) return;

			DateTime now = DateTime.Now;
			TimeSpan time = now.Subtract(startTime);
			lblTimer.Text = Math.Round(time.TotalSeconds).ToString();
		}

		public void AddNeighbours()
		{
			mineField.ForEach((List<Tile> col) => {
				col.ForEach((Tile t) => {
					ForNeighbors(t, t.AddNeighbour);
				});
			});
		}

		public int CalculateRemainingFlags()
		{
			int flags = mineCount;
			mineField.ForEach((List<Tile> col) => {
				col.ForEach((Tile t) => {
					if (t.state == Tile.ClickedState.flagged)
						flags--;
				});
			});
			return flags;
		}

		public void ForNeighbors(Tile tile, tileCallback callback)
		{
			if (tile.x == 0 && tile.y == 0)
			{
				callback(mineField[tile.x + 1][tile.y    ]);
				callback(mineField[tile.x + 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y+1]);
			}
			else if (tile.x == 0 && tile.y == height - 1)
			{
				callback(mineField[tile.x + 1][tile.y    ]);
				callback(mineField[tile.x + 1][tile.y - 1]);
				callback(mineField[tile.x    ][tile.y - 1]);
			}
			else if (tile.x == width - 1 && tile.y == 0)
			{
				callback(mineField[tile.x - 1][tile.y    ]);
				callback(mineField[tile.x - 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y + 1]);
			}
			else if (tile.x == width - 1 && tile.y == height - 1)
			{
				callback(mineField[tile.x - 1][tile.y    ]);
				callback(mineField[tile.x - 1][tile.y - 1]);
				callback(mineField[tile.x    ][tile.y - 1]);
			}
			else if (tile.x == 0)
			{
				callback(mineField[tile.x    ][tile.y - 1]);
				callback(mineField[tile.x + 1][tile.y - 1]);
				callback(mineField[tile.x + 1][tile.y    ]);
				callback(mineField[tile.x + 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y + 1]);
			}
			else if (tile.x == width - 1)
			{
				callback(mineField[tile.x    ][tile.y - 1]);
				callback(mineField[tile.x - 1][tile.y - 1]);
				callback(mineField[tile.x - 1][tile.y    ]);
				callback(mineField[tile.x - 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y + 1]);
			}
			else if (tile.y == 0)
			{
				callback(mineField[tile.x - 1][tile.y    ]);
				callback(mineField[tile.x - 1][tile.y + 1]);
				callback(mineField[tile.x    ][tile.y + 1]);
				callback(mineField[tile.x + 1][tile.y + 1]);
				callback(mineField[tile.x + 1][tile.y    ]);
			}
			else if (tile.y == height - 1)
			{
				callback(mineField[tile.x - 1][tile.y    ]);
				callback(mineField[tile.x - 1][tile.y - 1]);
				callback(mineField[tile.x    ][tile.y - 1]);
				callback(mineField[tile.x + 1][tile.y - 1]);
				callback(mineField[tile.x + 1][tile.y    ]);
			}
			else
			{
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

	}

	public class Tile
	{
		public enum ClickedState
		{
			unClicked,
			clicked,
			flagged
		}

		public List<Tile> neighbours = new List<Tile>();

		public ClickedState state = ClickedState.unClicked;
		public bool isBomb = false;
		public byte bombsNear = 0;

		public static int size = 30;
		public int x, y;

		public Tile(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public void CalculateDanger()
		{
			bombsNear = 0;
			neighbours.ForEach((Tile t) => {
				if (t.isBomb)
					bombsNear++;
			});
		}

		public void AddNeighbour(Tile t)
		{
			neighbours.Add(t);
			CalculateDanger();
		}

		public void Click(MouseEventArgs e)
		{
			if (state == ClickedState.unClicked && e.Button == MouseButtons.Left)
			{
				state = ClickedState.clicked;
				if (bombsNear == 0)
				{
					neighbours.ForEach((Tile t) => t.Click(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0)));
				}
			}
			else if (state == ClickedState.unClicked && e.Button == MouseButtons.Right)
			{
				state = ClickedState.flagged;
			}
			else if (state == ClickedState.flagged && e.Button == MouseButtons.Right)
			{
				state = ClickedState.unClicked;
			}
			else if (state == ClickedState.clicked && e.Button == MouseButtons.Right)
			{
				int flagsNear = 0;
				neighbours.ForEach((Tile t) => { if (t.state == ClickedState.flagged) flagsNear++; });
				if (flagsNear == bombsNear)
				{
					neighbours.ForEach((Tile t) => t.Click(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
				}
			}
		}

		public void Draw(Graphics g)
		{
			if (state.Equals(ClickedState.unClicked))
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

			}
			else if (state.Equals(ClickedState.clicked))
			{
				// fill
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
			else if (state.Equals(ClickedState.flagged))
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
