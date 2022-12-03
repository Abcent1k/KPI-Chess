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
				throw new ArgumentException("Incorrect color");

			try
			{
				chessSprite = new Bitmap($"Sprites\\ChessSprites\\{this.color}{type}.png");
			}
			catch (ArgumentException)
			{								
				throw new ArgumentException($"Chessman sprite not found {this.color}{type}.png");				
			}
			
			posSteps = new byte[8, 8];
		}

		/// <summary>
		/// Обнулить маркеры подсветки
		/// </summary>
		/// <returns>Обнуленный масив маркеров</returns>
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

		/// <summary>
		/// Логика расчета возможных ходов фигуры
		/// </summary>
		/// <param name="cb">Шахматная доска</param>
		/// <returns>Масив маркеров</returns>
		public abstract byte[,] PossibleSteps(Chessboard cb);

		/// <summary>
		/// Получить местоположение фигуры
		/// </summary>
		/// <param name="cb">Шахматная доска</param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
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

		/// <summary>
		/// Массив возможных ходов фигуры
		/// </summary>
		public byte[,] posSteps;

		public Bitmap chessSprite;

		public char type { get; }

		public char color { get; }
	}
}
