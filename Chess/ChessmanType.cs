
using Microsoft.VisualBasic;

namespace Chess
{
	internal class Pawn : Chessman
	{
		public Pawn(char color) : base(color) { }

		public override byte[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_or_minus = 1;

			StreamReader sr = new StreamReader($"Saves\\{cb.currentMatch.safeFile}.txt");
			string str = sr.ReadToEnd();
			int len = str.Length;

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
						posSteps[Y + y1, X] = 1;
						if (Y == y16 && cb.chess[Y + y2, X] == null)//Ход на две клеточки
							posSteps[Y + y2, X] = 1;						
					}

					if ((cb.chess[Y + y1, X + plus_or_minus]?.color ?? color) != color)//Бить
						posSteps[Y + y1, X + plus_or_minus] = 2;

					if(((Y == 4 && cb.chess[Y, X].color == 'b') || (Y == 3 && cb.chess[Y, X].color == 'w')) && (cb.chess[Y, X + 1]?.type == 'P' || cb.chess[Y, X - 1]?.type == 'P'))
					{														
						if((X + plus_or_minus + 48 == (str[len-1])) && (X + plus_or_minus + 48 == (str[len - 4])))
						{
							if (str[len - 5] == 1 + 48)
							{
								posSteps[3, X + plus_or_minus] = 3;
								posSteps[2, X + plus_or_minus] = 2;
							}
							else if (str[len - 5] == 6 + 48)
							{
								posSteps[4, X + plus_or_minus] = 3;
								posSteps[5, X + plus_or_minus] = 2;
							}
						}
					}
				}
				catch (IndexOutOfRangeException) { }
				plus_or_minus *= -1;
			}

			sr.Close();

			return posSteps;
		}

	}

	internal class Rook : Chessman
	{
		public Rook(char color) : base(color) { }

		public override byte[,] PossibleSteps(Chessboard cb)
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
							if(cb.chess[Y + y, X + x] == null)
								posSteps[Y + y, X + x] = 1;
							
							else if ((cb.chess[Y + y, X + x].color) != color)
							{
								posSteps[Y + y, X + x] = 2;
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

		public override byte[,] PossibleSteps(Chessboard cb)
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
							if (cb.chess[Y + y, X + x] == null)
								posSteps[Y + y, X + x] = 1;

							else if ((cb.chess[Y + y, X + x].color) != color)
							{
								posSteps[Y + y, X + x] = 2;
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

		public override byte[,] PossibleSteps(Chessboard cb)
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
							if (cb.chess[Y + y, X + x] == null)
								posSteps[Y + y, X + x] = 1;

							else if ((cb.chess[Y + y, X + x].color) != color)
							{
								posSteps[Y + y, X + x] = 2;
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
							if (cb.chess[Y + y, X + x] == null)
								posSteps[Y + y, X + x] = 1;

							else if ((cb.chess[Y + y, X + x].color) != color)
							{
								posSteps[Y + y, X + x] = 2;
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

		public override byte[,] PossibleSteps(Chessboard cb)
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
							if (cb.chess[Y + a, X + b] == null)
								posSteps[Y + a, X + b] = 1;
							else if (cb.chess[Y + a, X + b].color != color)
								posSteps[Y + a, X + b] = 2;
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

		public override byte[,] PossibleSteps(Chessboard cb)
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
								if (cb.chess[Y + y, X + x] == null)								
									posSteps[Y + y, X + x] = 1;								
								else if (cb.chess[Y + y, X + x].color != color)
									posSteps[Y + y, X + x] = 2;								
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

