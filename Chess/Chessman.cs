
namespace Chess
{
	public abstract class Chessman
	{
		public Chessman(char color)
		{
			type = GetType().ToString()[6];
			if (color == 'w' || color == 'W')
				this.color = 'w';
			else if (color == 'b' || color == 'B')
				this.color = 'b';
			else
				throw new ArgumentException();
						
			chessSprite = new Bitmap($"Sprites\\{this.color}{type}.png");
			//chessSprite = Image.FromFile($"Sprites\\{this.color}{type}.svg");

			posStepCalculated = false;
			posSteps = new byte[8, 8];
		}

		public byte[,] ResetSteps()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					posSteps[i, j] = 0;
				}
			}
			return posSteps;
		}
		public abstract byte[,] PossibleSteps(Chessboard cb);

		public void GetIter(Chessboard cb, ref int X, ref int Y)
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

		public byte[,] posSteps;

		public Bitmap chessSprite;
		public char type { get; }
		public char color { get; }

		public bool posStepCalculated;//Маркер, посчитаны ли возможные ходы выбраной фигуры
	}
}
