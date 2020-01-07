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
		int height = 9, length = 9, mineCount = 10;

		private void Form1_Load(object sender, EventArgs e)
		{
			NewGame();
		}

		public void NewGame()
		{
			mineField.Clear();
			for (int i = 0; i < height; i++)
			{
				List<Tile> row = new List<Tile>();
				for (int j = 0; i < length; j++)
				{
					row.Add(new Tile(i, j));
				}
				mineField.Add(row);
			}
			mineField.ForEach((List<Tile> row) => {
				row.ForEach((Tile t) => {
					ForNeighbors(t, t.AddBomb);
				});
			});
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

		public int size = 15;
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
			// draw tile
		}
	}
}
