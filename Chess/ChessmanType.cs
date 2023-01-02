using System.Drawing;

namespace Chess
{
	internal class Pawn : Chessman
	{
		public Pawn(char color) : base(color) { }

		public override object Clone() { return new Pawn(color); }

		public override byte[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_mn = 1;

			StreamReader sr = new StreamReader($"Saves\\{cb.safeFile}.txt");
			string str = sr.ReadToEnd();
			sr.Close();
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
				cb.PromotionChess[0] = new Queen(color);
				cb.PromotionChess[1] = new Rook(color);
				cb.PromotionChess[2] = new Bishop(color);
				cb.PromotionChess[3] = new Night(color);

				for (int i = 0; i < 4; i++)
				{
					int y = color == 'w' ? i + yEnd : yEnd - i;

					cb.PromotionChess[i].chessSprite = new Bitmap(cb.PromotionChess[i].chessSprite, new Size(cb.cellSize - 10, cb.cellSize - 10));

					cb.PromotionCells[i] = new Button();
					cb.PromotionCells[i].FlatAppearance.BorderSize = 0;
					cb.PromotionCells[i].BackgroundImage = cb.PromotionChess[i].chessSprite;
					cb.PromotionCells[i].BackgroundImageLayout = ImageLayout.Center;
					cb.PromotionCells[i].Size = new Size(cb.cellSize, cb.cellSize);
					cb.PromotionCells[i].Location = new Point(cb.frameSize + X * cb.cellSize, cb.frameSize + y * cb.cellSize);

					cb.Controls.Add(cb.PromotionCells[i]);
					cb.PromotionCells[i].BringToFront();

					cb.PromotionCells[i].Click += new EventHandler(cb.PawnPromotion);
				}
			}

			for (int n1 = 1; n1 <= 2; n1++)
			{
				try
				{
					//Ход
					if (cb.chess[Y + y1, X] == null)
					{
						posSteps[Y + y1, X] = 1;

						//Ход на две клеточки
						if (Y == y16 && cb.chess[Y + y2, X] == null)
							posSteps[Y + y2, X] = 1;
					}

					//Бить
					if ((cb.chess[Y + y1, X + plus_mn]?.color ?? color) != color)
						posSteps[Y + y1, X + plus_mn] = 2;

					//En passant
					if (((Y == 4 && color == 'b') || (Y == 3 && color == 'w')) &&
						((X - 1 >= 0 && cb.chess[Y, X - 1]?.type == 'P') || (X + 1 <= 7 && cb.chess[Y, X + 1]?.type == 'P')))
					{
						if (((char)(X + plus_mn + 97) == (str[len - 2])) && ((char)(X + plus_mn + 97) == (str[len - 5])))
						{
							if (str[len - 4] == (char)(7 + 48))
							{
								posSteps[3, X + plus_mn] = 3;
								posSteps[2, X + plus_mn] = 2;
							}
							else if (str[len - 4] == (char)(2 + 48))
							{
								posSteps[4, X + plus_mn] = 3;
								posSteps[5, X + plus_mn] = 2;
							}
						}
					}
				}
				catch (IndexOutOfRangeException) { }
				plus_mn *= -1;
			}

			return posSteps;
		}

	}

	internal class Rook : Chessman
	{
		public Rook(char color) : base(color) { }

		public override object Clone() { return new Rook(color); }

		public override byte[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_mn = 1;

			for (int n1 = 0; n1 <= 1; n1++)
			{
				for (int n2 = 0; n2 <= 1; n2++)
				{
					for (int i = 1; i <= 7; i++)
					{
						int y = i * plus_mn * n2;
						int x;

						if (y == 0)
							x = i * plus_mn;
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
				plus_mn *= -1;
			}
			return posSteps;
		}

	}

	internal class Bishop : Chessman
	{
		public Bishop(char color) : base(color) { }

		public override object Clone() { return new Bishop(color); }

		public override byte[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_mn_x = 1;
			int plus_mn_y = 1;

			for (int n1 = 1; n1 <= 2; n1++)
			{
				for (int n2 = 1; n2 <= 2; n2++)
				{
					for (int i = 1; i <= 7; i++)
					{
						int y = i * plus_mn_x;
						int x = i * plus_mn_y;

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
					plus_mn_y *= -1;
				}
				plus_mn_x *= -1;
			}

			return posSteps;
		}

	}

	internal class Queen : Chessman
	{
		public Queen(char color) : base(color) { }

		public override object Clone() { return new Queen(color); }

		public override byte[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_mn = 1;

			for (int n1 = 0; n1 <= 1; n1++)
			{
				for (int n2 = 0; n2 <= 1; n2++)
				{
					for (int i = 1; i <= 7; i++)
					{
						int y = i * plus_mn * n2;
						int x;

						if (y == 0)
							x = i * plus_mn;
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
				plus_mn *= -1;
			}

			int plus_mn_x = 1;
			int plus_mn_y = 1;

			for (int n1 = 0; n1 <= 1; n1++)
			{
				for (int n2 = 0; n2 <= 1; n2++)
				{
					for (int i = 1; i <= 7; i++)
					{
						int y = i * plus_mn_x;
						int x = i * plus_mn_y;

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
					plus_mn_y *= -1;
				}
				plus_mn_x *= -1;
			}

			return posSteps;
		}

	}

	internal class Night : Chessman
	{
		public Night(char color) : base(color) { }

		public override object Clone() { return new Night(color); }

		public override byte[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			int y = 2;
			int x = 1;
			int tmp;

			GetIter(cb, ref X, ref Y);

			for (int n1 = 0; n1 <= 1; n1++)
			{
				for (int n2 = 0; n2 <= 1; n2++)
				{
					for (int n3 = 0; n3 <= 1; n3++)
					{
						try
						{
							if (cb.chess[Y + y, X + x] == null)
								posSteps[Y + y, X + x] = 1;
							else if (cb.chess[Y + y, X + x].color != color)
								posSteps[Y + y, X + x] = 2;
						}
						catch (IndexOutOfRangeException) { }

						tmp = y;
						y = x;
						x = tmp;
					}
					x *= -1;
				}
				y *= -1;
			}
			return posSteps;
		}

	}

	internal class King : Chessman
	{
		public King(char color) : base(color) { }

		public override object Clone() { return new King(color); }	

		public override byte[,] PossibleSteps(Chessboard cb)
		{
			ResetSteps();

			int X = 0;
			int Y = 0;

			GetIter(cb, ref X, ref Y);

			int plus_mn_x = 1;
			int plus_mn_y = 1;

			for (int ny = 0; ny <= 1; ny++)
			{
				for (int nx = 0; nx <= 1; nx++)
				{
					for (int n1 = 0; n1 <= 1; n1++)
					{
						for (int n2 = 0; n2 <= 1; n2++)
						{
							int y = n1 * plus_mn_x;
							int x = n2 * plus_mn_y;

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
					plus_mn_x *= -1;
				}
				plus_mn_y *= -1;
			}

			if (cb.chess[Y, X].color == 'b')
			{
				if (cb.currentMatch.BLeftCastling == true && cb.chess[Y, 0]?.type == 'R')
				{
					if (cb.chess[Y, 1] == null && cb.chess[Y, 2] == null && cb.chess[Y, 3] == null)
						posSteps[Y, 2] = 4;
				}
				if (cb.currentMatch.BRightCastling == true && cb.chess[Y, 7]?.type == 'R')
				{
					if (cb.chess[Y, 5] == null && cb.chess[Y, 6] == null)
						posSteps[Y, 6] = 4;
				}
			}
			else
			{
				if (cb.currentMatch.WLeftCastling == true && cb.chess[Y, 0]?.type == 'R')
				{
					if (cb.chess[Y, 1] == null && cb.chess[Y, 2] == null && cb.chess[Y, 3] == null)
						posSteps[Y, 2] = 4;
				}
				if (cb.currentMatch.WRightCastling == true && cb.chess[Y, 7]?.type == 'R')
				{
					if (cb.chess[Y, 5] == null && cb.chess[Y, 6] == null)
						posSteps[Y, 6] = 4;
				}

			}
			return posSteps;
		}

	}
}

