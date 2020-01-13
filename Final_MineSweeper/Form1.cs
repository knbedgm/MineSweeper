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
		

        private void Form1_Load(object sender, EventArgs e)
        {
            NewMinefield();
        }

        public void NewGame()
		{
			NewMinefield();
			GenerateMines();
			CalculateDanger();
			flagsRemaining = mineCount;
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
			timer1.Start();
			running = true;
		}

		private void canvas_MouseClick(object sender, MouseEventArgs e)
		{
			if (!running) return;

			//find block clicked
			int x, y;
			x = e.X/(Tile.size);
			y = e.Y/(Tile.size);
			mineField[x][y].Click(e);
			flagsRemaining = CalculateRemainingFlags();
			lblFlags.Text = flagsRemaining.ToString();
			canvas.Invalidate();
		}

		private void Timer1_Tick(object sender, EventArgs e)
        {
           lblTimer.Text = (Convert.ToInt32(lblTimer.Text) + 1).ToString();
        }

		public void CalculateDanger()
		{
			mineField.ForEach((List<Tile> col) => {
				col.ForEach((Tile t) => {
					t.bombsNear = 0;
					ForNeighbors(t, t.AddBomb);
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

		public void AddBomb(Tile t)
		{
			if (t.isBomb)
			{
				bombsNear++;
			}
		}

		public void Click(MouseEventArgs e)
		{
			if (state == ClickedState.unClicked && e.Button == MouseButtons.Left)
			{
				state = Tile.ClickedState.clicked;
			}
			else if (state == ClickedState.unClicked && e.Button == MouseButtons.Right)
			{
				state = Tile.ClickedState.flagged;
			}
			else if (state == ClickedState.flagged && e.Button == MouseButtons.Right)
			{
				state = Tile.ClickedState.unClicked;
			}
		}

		public void Draw(Graphics g)
		{
			if (state.Equals(ClickedState.unClicked))
			{
				// fill
				SolidBrush b = new SolidBrush(Color.Gray);
				g.FillRectangle(b, x * size, y * size, size, size);
				
				// Border
				Pen p = new Pen(Color.DarkGray, 2);
				g.DrawLine(p, x * size + size - 1, y * size, x * size + size - 1, y * size + size);
				g.DrawLine(p, x * size, y * size + size - 1, x * size + size, y * size + size - 1);
				p.Color = Color.LightGray;
				g.DrawLine(p, x * size, y * size, x * size, y * size + size);
				g.DrawLine(p, x * size, y * size, x * size + size, y * size);

			}
			else if (state.Equals(ClickedState.clicked))
			{
				// fill
				SolidBrush b = new SolidBrush(Color.Gray);
				g.FillRectangle(b, x * size, y * size, size, size);


				g.DrawString(bombsNear.ToString(), new Font("Arial", 10), b, x * size, y * size);

			}
			else if (state.Equals(ClickedState.flagged))
			{
				// fill
				SolidBrush b = new SolidBrush(Color.Gray);
				g.FillRectangle(b, x * size, y * size, size, size);

				// Border
				Pen p = new Pen(Color.DarkGray, 2);
				g.DrawLine(p, x * size + size - 1, y * size, x * size + size - 1, y * size + size);
				g.DrawLine(p, x * size, y * size + size - 1, x * size + size, y * size + size - 1);
				p.Color = Color.LightGray;
				g.DrawLine(p, x * size, y * size, x * size, y * size + size);
				g.DrawLine(p, x * size, y * size, x * size + size, y * size);

				g.DrawString(bombsNear.ToString(), new Font("Arial", 10), b, x * size, y * size);

				// Flags
				b.Color = Color.Red;
				g.FillEllipse(b, x * size + 5 , y * size + 5, size - 10 , size - 10);

				// On Right Click

			}
		}	
	}
   
}
