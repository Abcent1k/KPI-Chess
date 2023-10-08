namespace Chess
{
	interface IMaxDistance8x8
	{
		internal const int MAX_DISTANCE = 7;
	}

	interface ICastlePiece
	{
		protected bool canCastle { get; set; }
	}

	interface IDoubleStep
	{
		protected bool canDoubleStep { get; set; }
	}

	interface IEnPassant
	{
		protected bool canEnPassantRight { get; set; }
		protected bool canEnPassantLeft { get; set; }
	}

	public abstract class Chessman
	{

		protected const int MAX_DISTANCE = IMaxDistance8x8.MAX_DISTANCE;

		internal Bitmap chessSprite { get; }

		internal Color color { get; }

		internal Point[][]? availableMoves { get; set; }

		internal Point[][]? availableAttacks { get; set; }


		internal Chessman(Color color)
		{

			this.color = color;

			try
			{
				chessSprite = new Bitmap($"Sprites\\ChessSprites\\{this.color}{this.GetType()}.png");
			}
			catch (ArgumentException)
			{
				throw new ArgumentException($"Chessman sprite not found {this.color}{this.GetType()}.png");
			}

		}

		public abstract Chessman CalculateMoves();

		//public abstract object Clone();


		/// <summary>
		/// Get relative horizontal or virtical movement coordinates
		/// Used by: King, Queen, Pawn, Rook
		/// </summary>
		/// <param name="distance">How far in the given direction.</param>
		/// <param name="direction">Direction relative to player</param>
		/// <returns>Return an array for horizontal or virtical movment</returns>
		public static Point[] GetMovementArray(int distance, Direction direction)
		{
			Point[] movement = new Point[distance];
			int xPosition = 0;
			int yPosition = 0;

			for (int i = 0; i < distance; i++)
			{
				switch (direction)
				{
					case Direction.FORWARD:
						yPosition++;
						break;
					case Direction.BACKWARD:
						yPosition--;
						break;
					case Direction.LEFT:
						xPosition++;
						break;
					case Direction.RIGHT:
						xPosition--;
						break;
					default:
						break;
				}
				movement[i] = new Point(xPosition, yPosition);
			}
			return movement;
		}

		/// <summary>
		/// Get relative diagnal movement coordinates
		/// Used by: King, Queen, Pawn, Bishop
		/// </summary>
		/// <param name="distance">How far in the given direction</param>
		/// <param name="direction">Direction relative to player</param>
		/// <returns>Return an array for diagnal movement</returns>
		public static Point[] GetDiagnalMovementArray(int distance, DiagnalDirection direction)
		{
			Point[] attack = new Point[distance];
			int xPosition = 0;
			int yPosition = 0;

			for (int i = 0; i < distance; i++)
			{
				switch (direction)
				{
					case DiagnalDirection.FORWARD_LEFT:
						xPosition--;
						yPosition++;
						break;
					case DiagnalDirection.FORWARD_RIGHT:
						xPosition++;
						yPosition++;
						break;
					case DiagnalDirection.BACKWARD_LEFT:
						xPosition--;
						yPosition--;
						break;
					case DiagnalDirection.BACKWARD_RIGHT:
						xPosition++;
						yPosition--;
						break;
					default:
						break;
				}
				attack[i] = new Point(xPosition, yPosition);
			}
			return attack;
		}

		///// <summary>
		///// Получить местоположение фигуры
		///// </summary>
		///// <param name="cb">Шахматная доска</param>
		///// <param name="X"></param>
		///// <param name="Y"></param>
		//public void GetIter(Chessboard cb, ref int X, ref int Y)
		//{
		//	for (int i = 0; i < 8; i++)
		//	{
		//		for (int j = 0; j < 8; j++)
		//		{
		//			if (cb.chess[i, j] == this)
		//			{
		//				X = j;
		//				Y = i;
		//			}
		//		}
		//	}
		//}
	}
}
