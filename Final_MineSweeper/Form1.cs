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
		int height = 9, width = 9, mineCount = 10;

		private void Form1_Load(object sender, EventArgs e)
		{
			NewGame();
		}

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        public void NewGame()
		{
			NewMinefeild();
			GenerateMines();
		}

        public void NewMinefeild()
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
			mineField.ForEach((List<Tile> col) => {
				col.ForEach((Tile t) => {
					ForNeighbors(t, t.AddBomb);
				});
			});
		}

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
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

		public void ForNeighbors(Tile t, tileCallback callback)
		{
           
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

		public int size = 30;
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

		public void Draw(Graphics g)
		{
			
			SolidBrush b = new SolidBrush(Color.LightGray);
			Pen p = new Pen(Color.Black, 1);
            g.FillRectangle(b, x*size, y*size, size, size);
			g.DrawLine(p, x*size+size-1, y*size, x*size+size-1, y*size+size);
			g.DrawLine(p, x*size, y*size+size-1, x*size+size, y*size+size-1);
          
        }
	}
   
}
