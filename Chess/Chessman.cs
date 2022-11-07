using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
	public abstract class Chessman
	{		
		public Chessman(char color)
		{				
			type = GetType().ToString()[6] ;
			if (color == 'w' || color == 'W')
				this.color = 'w';
			else if (color == 'b' || color == 'B')
				this.color = 'b';
			else
				throw new ArgumentException();

			chessSprite = Image.FromFile($"Sprites\\{this.color}{type}.png");

			posSteps = new bool[8, 8];
		}

		public bool[,]ResetSteps()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					posSteps[i, j] = false;
				}
			}
			return posSteps;
		}
		public abstract bool[,] PossibleSteps(Chessboard cb);

		public void GetIter(Chessboard cb, ref int X, ref  int Y)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (cb.chess[i, j] == this)
					{
						X = j;
						Y = i;
					}
				}
			}
		}

		public bool[,] posSteps;

		public Image chessSprite;

		public char type { get;}

		public char color { get; }
	}
}
