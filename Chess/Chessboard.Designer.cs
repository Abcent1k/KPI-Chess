namespace Chess
{
	partial class Chessboard
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(231, 298);
			this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(262, 123);
			this.button1.TabIndex = 0;
			this.button1.Text = "Start the match";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Chessboard
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(726, 720);
			this.Controls.Add(this.button1);
			this.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "Chessboard";
			this.Text = "Chess";
			this.Deactivate += new System.EventHandler(this.Chessboard_Deactivate);
			this.Load += new System.EventHandler(this.Chessboard_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private Button button1;
	}
}