namespace Final_MineSweeper
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.canvas = new System.Windows.Forms.PictureBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.btnStart = new System.Windows.Forms.Button();
			this.txtTimer = new System.Windows.Forms.TextBox();
			this.txtMines = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
			this.SuspendLayout();
			// 
			// canvas
			// 
			this.canvas.Location = new System.Drawing.Point(61, 59);
			this.canvas.Name = "canvas";
			this.canvas.Size = new System.Drawing.Size(270, 270);
			this.canvas.TabIndex = 0;
			this.canvas.TabStop = false;
			this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.Canvas_Paint);
			// 
			// timer1
			// 
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(61, 335);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(270, 34);
			this.btnStart.TabIndex = 1;
			this.btnStart.Text = "Start Game";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.BtnStart_Click);
			// 
			// txtTimer
			// 
			this.txtTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTimer.Location = new System.Drawing.Point(282, 22);
			this.txtTimer.Name = "txtTimer";
			this.txtTimer.ReadOnly = true;
			this.txtTimer.Size = new System.Drawing.Size(49, 29);
			this.txtTimer.TabIndex = 2;
			this.txtTimer.TabStop = false;
			this.txtTimer.Text = "0";
			this.txtTimer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtMines
			// 
			this.txtMines.Location = new System.Drawing.Point(61, 22);
			this.txtMines.Multiline = true;
			this.txtMines.Name = "txtMines";
			this.txtMines.ReadOnly = true;
			this.txtMines.Size = new System.Drawing.Size(49, 31);
			this.txtMines.TabIndex = 3;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(428, 415);
			this.Controls.Add(this.txtMines);
			this.Controls.Add(this.txtTimer);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.canvas);
			this.Name = "Form1";
			this.Text = "Minesweeper";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtTimer;
        private System.Windows.Forms.TextBox txtMines;
    }
}

