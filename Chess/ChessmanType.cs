
namespace Chess
{
	internal class Pawn : Chessman
	{
		public Pawn(char color) : base(color) { }

		public override bool[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_or_minus = 1;

			for (int n1 = 1; n1 <= 2; n1++)
			{
				int y1 = 1,//Для обычного хода
					y2 = 2,//Для битья
					y16 = 1,//Для хода на две клеточки
					yEnd = 7;//Для смены пешки при достижении края доски
				try
				{
					if (color == 'w')
					{
						y1 *= -1;
						y2 *= -1;
						y16 = 6;
						yEnd = 0;
					}

					if (cb.chess[Y + y1, X] == null)//Ход
					{
						posSteps[Y + y1, X] = true;
						if (Y == y16 && cb.chess[Y + y2, X] == null)//Ход на две клеточки
							posSteps[Y + y2, X] = true;						
					}

					if ((cb.chess[Y + y1, X + plus_or_minus]?.color ?? color) != color)//Бить
						posSteps[Y + y1, X + plus_or_minus] = true;
				}
				catch (IndexOutOfRangeException) { }
				plus_or_minus *= -1;
			}
			return posSteps;
		}

	}

	internal class Rook : Chessman
	{
		public Rook(char color) : base(color) { }

		public override bool[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_or_minus = 1;

			for (int n1 = 1; n1 <= 2; n1++)
			{
				for (int n2 = 0; n2 <= 1; n2++)
				{
					for (int i = 1; i < 8; i++)
					{
						int y = i * plus_or_minus * n2;
						int x;

						if (y == 0)
							x = i * plus_or_minus;
						else
							x = 0;

						try
						{
							if (cb.chess[Y + y, X + x]?.color != color)
							{
								posSteps[Y + y, X + x] = true;
								if ((cb.chess[Y + y, X + x]?.color ?? color) != color)
									break;
							}
							else
								break;
						}
						catch (IndexOutOfRangeException) { }
					}
				}
				plus_or_minus *= -1;
			}

			return posSteps;
		}

	}

	internal class Bishop : Chessman
	{
		public Bishop(char color) : base(color) { }

		public override bool[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_or_minus_x = 1;
			int plus_or_minus_y = 1;

			for (int n1 = 1; n1 <= 2; n1++)
			{
				for (int n2 = 1; n2 <= 2; n2++)
				{
					for (int i = 1; i < 8; i++)
					{
						int y = i * plus_or_minus_x;
						int x = i * plus_or_minus_y;

						try
						{
							if (cb.chess[Y + y, X + x]?.color != color)
							{
								posSteps[Y + y, X + x] = true;
								if ((cb.chess[Y + y, X + x]?.color ?? color) != color)
									break;
							}
							else
								break;
						}
						catch (IndexOutOfRangeException) { }
					}
					plus_or_minus_y *= -1;
				}
				plus_or_minus_x *= -1;
			}

			return posSteps;
		}

	}

	internal class Queen : Chessman
	{
		public Queen(char color) : base(color) { }

		public override bool[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_or_minus = 1;

			for (int n1 = 1; n1 <= 2; n1++)
			{
				for (int n2 = 0; n2 <= 1; n2++)
				{
					for (int i = 1; i < 8; i++)
					{
						int y = i * plus_or_minus * n2;
						int x;

						if (y == 0)
							x = i * plus_or_minus;
						else
							x = 0;

						try
						{
							if (cb.chess[Y + y, X + x]?.color != color)
							{
								posSteps[Y + y, X + x] = true;
								if ((cb.chess[Y + y, X + x]?.color ?? color) != color)
									break;
							}
							else
								break;
						}
						catch (IndexOutOfRangeException) { }
					}
				}
				plus_or_minus *= -1;
			}

			int plus_or_minus_x = 1;
			int plus_or_minus_y = 1;

			for (int n1 = 1; n1 <= 2; n1++)
			{
				for (int n2 = 1; n2 <= 2; n2++)
				{
					for (int i = 1; i < 8; i++)
					{
						int y = i * plus_or_minus_x;
						int x = i * plus_or_minus_y;

						try
						{
							if (cb.chess[Y + y, X + x]?.color != color)
							{
								posSteps[Y + y, X + x] = true;
								if ((cb.chess[Y + y, X + x]?.color ?? color) != color)
									break;
							}
							else
								break;
						}
						catch (IndexOutOfRangeException) { }
					}
					plus_or_minus_y *= -1;
				}
				plus_or_minus_x *= -1;
			}

			return posSteps;
		}

	}

	internal class Night : Chessman
	{
		public Night(char color) : base(color) { }

		public override bool[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			int a = 2;
			int b = 1;
			int tmp;

			GetIter(cb, ref X, ref Y);

			for (int n1 = 0; n1 < 2; n1++)
			{
				for (int n2 = 0; n2 < 2; n2++)
				{
					for (int n3 = 0; n3 < 2; n3++)
					{
						try
						{
							if (cb.chess[Y + a, X + b]?.color != color)
								posSteps[Y + a, X + b] = true;
						}
						catch (IndexOutOfRangeException) { }
						tmp = a;
						a = b;
						b = tmp;
					}
					b *= -1;
				}
				a *= -1;
			}
			return posSteps;
		}

	}

	internal class King : Chessman
	{
		public King(char color) : base(color) { }

		public override bool[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_or_minus_x = 1;
			int plus_or_minus_y = 1;
			for (int ny = 0; ny <= 1; ny++)
			{
				for (int nx = 0; nx <= 1; nx++)
				{
					for (int n1 = 0; n1 <= 1; n1++)
					{
						for (int n2 = 0; n2 <= 1; n2++)
						{
							int y = n1 * plus_or_minus_x;
							int x = n2 * plus_or_minus_y;

							try
							{
								if (cb.chess[Y + y, X + x]?.color != color)
									posSteps[Y + y, X + x] = true;
							}
							catch (IndexOutOfRangeException) { }
						}
					}
					plus_or_minus_x *= -1;
				}
				plus_or_minus_y *= -1;
			}
			return posSteps;
		}

	}
}

