﻿
using Microsoft.VisualBasic;
using System.Windows.Forms;

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

			int y1 = 1,//Для обычного хода
					y2 = 2,//Для битья
					y16 = 1,//Для хода на две клеточки
					yEnd = 7;//Для смены пешки при достижении края доски

			if (color == 'w')
			{
				y1 *= -1;
				y2 *= -1;
				y16 = 6;
				yEnd = 0;				
			}
			//Превращение пешки
			if (Y == yEnd)
			{
				cb.PawnChess[0] = new Queen(color);
				cb.PawnChess[1] = new Rook(color);
				cb.PawnChess[2] = new Bishop(color);
				cb.PawnChess[3] = new Night(color);

				for (int i = 0; i < 4; i++)
				{
					int y;
					if(color == 'w')
						y = i + yEnd;
					else
						y = yEnd - i;

					cb.PawnButtons[i] = new Button();

					cb.PawnChess[i].chessSprite = new Bitmap(cb.PawnChess[i].chessSprite, new Size(cb.buttonSize - 10, cb.buttonSize - 10));

					cb.PawnButtons[i].FlatAppearance.BorderSize = 0;					
					cb.PawnButtons[i].BackgroundImage = cb.PawnChess[i].chessSprite;
					cb.PawnButtons[i].BackgroundImageLayout = ImageLayout.Center;
					cb.PawnButtons[i].Size = new Size(cb.buttonSize, cb.buttonSize);
					cb.PawnButtons[i].Location = new Point(cb.frameSize + X * cb.buttonSize, cb.frameSize + y * cb.buttonSize);

					cb.Controls.Add(cb.PawnButtons[i]);
					cb.PawnButtons[i].BringToFront();

					cb.PawnButtons[i].Click += new EventHandler(cb.PawnPromotion);
				}
			}

			for (int n1 = 1; n1 <= 2; n1++)
			{
				try
				{
					if (cb.chess[Y + y1, X] == null)//Ход
					{
						posSteps[Y + y1, X] = 1;
						if (Y == y16 && cb.chess[Y + y2, X] == null)//Ход на две клеточки
							posSteps[Y + y2, X] = 1;						
					}

					if ((cb.chess[Y + y1, X + plus_or_minus]?.color ?? color) != color)//Бить
						posSteps[Y + y1, X + plus_or_minus] = 2;

					if(((Y == 4 && color == 'b') || (Y == 3 && color == 'w')) && 
						((X - 1 >= 0 && cb.chess[Y, X - 1]?.type == 'P') || (X + 1 <= 7 && cb.chess[Y, X + 1]?.type == 'P')))
					{														
						if(((char)(X + plus_or_minus + 97) == (str[len-3])) && ((char)(X + plus_or_minus + 97) == (str[len - 6])))
						{
							if (str[len - 5] == (char)(7 + 48))
							{
								posSteps[3, X + plus_or_minus] = 3;
								posSteps[2, X + plus_or_minus] = 2;
							}
							else if (str[len - 5] == (char)(2 + 48))
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

			if((cb.chess[Y, X].color == 'b' && cb.currentMatch.BCastling == true) ||
				(cb.chess[Y, X].color == 'w' && cb.currentMatch.WCastling == true))
			{
				if (X == 0)
				{
					if (cb.chess[Y, 1] == null && cb.chess[Y, 2] == null && cb.chess[Y, 3] == null)
						posSteps[Y, 4] = 4;
				}
				else
				{
					if (cb.chess[Y, 5] == null && cb.chess[Y, 6] == null)
						posSteps[Y, 4] = 5;
				}				
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

			if ((cb.chess[Y, X].color == 'b' && cb.currentMatch.BCastling == true) ||
				(cb.chess[Y, X].color == 'w' && cb.currentMatch.WCastling == true))
			{
				if (cb.chess[Y, 0].type == 'R')
				{					
					if (cb.chess[Y, 1] == null && cb.chess[Y, 2] == null && cb.chess[Y, 3] == null)					
						posSteps[Y, 0] = 5;											
				}
				if(cb.chess[Y, 7].type == 'R')
				{					
					if (cb.chess[Y, 5] == null && cb.chess[Y, 6] == null)						
						posSteps[Y, 7] = 4;								
				}
			}

			return posSteps;
		}

	}
}

