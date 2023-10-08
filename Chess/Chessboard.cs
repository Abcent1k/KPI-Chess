using System.Media;
using Chess.Pieces;

namespace Chess
{
    public partial class Chessboard : Form
	{
		public int cellSize = 90;//Размер клетки (кнопки)	
		public int frameSize = 25;
		public int labelSize = 60;

		public Bitmap dot;
		public Bitmap circle;
		public Bitmap cross;
		public Bitmap arrowLeft;

		public Color frame = Color.FromArgb(255, 0, 43, 29);//Цвет рамки вокруг шахматной доски

		public Color lightCell = Color.FromArgb(255, 237, 252, 248);//Светлый цвет для карты цветов
		public Color darkCell = Color.FromArgb(255, 0, 87, 62);//Темный цвет для карты цветов		

		public Color lightPushedCell = Color.LightGreen;//Цвет нажатой кнопки, если клетка под фигурой светлая
		public Color darkPushedCell = Color.MediumSeaGreen;//Цвет нажатой кнопки, если клетка под фигурой темная

		public Color lightPrevCell = Color.FromArgb(255, 172, 250, 255);//Цвет предыдущей кнопки, если клетка под фигурой светлая
		public Color darkPrevCell = Color.FromArgb(255, 0, 162, 159);//Цвет предыдущей кнопки, если клетка под фигурой темная

		public Color[,] chessboardColorMap = new Color[8, 8];//Карта цветов шахматной доски
		public Color[,] PushedCellColorMap = new Color[8, 8];
		public Color[,] PrevCellColorMap = new Color[8, 8];

		public string savesPath = "Saves";//Путь к папке с сохранениями
		public string safeFile = ("SF " + (DateTime.Now).ToString().Replace(":", "."));//Файл сохранения

		public Match currentMatch;

		//Изменение размеров формы
		private int oldWidth, oldHeight;
		private float formSidesProportion; // соотношение сторон формы

		public string[,] defaultMap = new string[8, 8]//Карта начальных позиций фигур
		{
			{"bR","bH","bB","bQ","bK","bB","bH","bR"},
			{"bP","bP","bP","bP","bP","bP","bP","bP"},
			{"","","","","","","",""},
			{"","","","","","","",""},
			{"","","","","","","",""},
			{"","","","","","","",""},
			{"wP","wP","wP","wP","wP","wP","wP","wP"},
			{"wR","wH","wB","wQ","wK","wB","wH","wR"},
		};

		public Button? prevCell;

		public Button[,] cells = new Button[8, 8];//Массив с кнопками

		public Chessman[,] chess = new Chessman[8, 8];//Массив с фигурами

		public Label[] WLabelsKnockout = new Label[16];
		public Label[] BLabelsKnockout = new Label[16];

		public Label labelFrame;

		public Label[] VLLabels = new Label[8];
		public Label[] VRLabels = new Label[8];
		public Label[] HTLabels = new Label[8];
		public Label[] HBLabels = new Label[8];


		public Chessman[] PromotionChess = new Chessman[4];
		public Button[] PromotionCells = new Button[4];

		public bool posStepsCalculated = false;//Посчитаны ли ходы фигур

		public Chessboard()
		{
			InitializeComponent();
		}

		public Chessboard(Chessboard cb)
		{
			CreateChessboard();
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (cb.chess[i, j] != null)
						chess[i, j] = (Chessman)cb.chess[i, j].Clone();
				}
			}

			safeFile = cb.safeFile;
			currentMatch = (Match)cb.currentMatch.Clone();
		}

		public void Init()
		{
			CreateChessboard();
		}

		/// <summary>
		/// Создание карты цветов шахматной доски
		/// </summary>
		/// <param name="light">Светлая клетка</param>
		/// <param name="dark">Темная клетка</param>
		public void CreateColorMap(Color[,] colorMap, Color light, Color dark)
		{
			for (int i = 0; i < Math.Sqrt(colorMap.Length); i++)
			{
				for (int j = 0; j < Math.Sqrt(colorMap.Length); j++)
				{
					if ((i % 2 + j % 2) % 2 == 0)//Задаем цвета клеточек доски
						colorMap[i, j] = light;
					else
						colorMap[i, j] = dark;
				}
			}
		}

		/// <summary>
		/// Перерисовка шахматной доски
		/// </summary>
		public void ShowChessboard()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (chess[i, j] != null)
						cells[i, j].BackgroundImage = new Bitmap(chess[i, j].chessSprite, new Size(cellSize - 10, cellSize - 10));
					else
						cells[i, j].BackgroundImage = null;

					cells[i, j].BackgroundImageLayout = ImageLayout.Center;
				}
			}
		}

		/// <summary>
		/// Скрыть или показать элементы формы		
		/// </summary>
		/// <param name="showElement">Скрыть или показать</param>
		//Нужен для красивой отрисовки UI при изменении размеров окна
		private void RedrawingUI(bool showElement)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					cells[i, j].Visible = showElement;

					if ((i * 8 + j) < 16)
					{
						WLabelsKnockout[i * 8 + j].Visible = showElement;
						BLabelsKnockout[i * 8 + j].Visible = showElement;
					}
				}
				VLLabels[i].Visible = VRLabels[i].Visible = HTLabels[i].Visible = HBLabels[i].Visible = showElement;
			}
		}

		/// <summary>
		/// Создать шахманую доску
		/// </summary>
		public void CreateChessboard()
		{
			dot = new Bitmap(new Bitmap($"Sprites\\dot.png"), new Size(cellSize, cellSize));
			circle = new Bitmap(new Bitmap($"Sprites\\circle.png"), new Size(cellSize - 1, cellSize - 1));
			cross = new Bitmap($"Sprites\\cross.png");
			arrowLeft = new Bitmap(new Bitmap($"Sprites\\arrowLeft.png"), new Size(cellSize - 1, cellSize - 1));

			frameSize = (int)(cellSize / 3.6);
			labelSize = (int)(cellSize / 1.5);

			formSidesProportion = (float)(Width - 18) / (float)(Height - 47);

			CreateColorMap(chessboardColorMap, lightCell, darkCell);
			CreateColorMap(PushedCellColorMap, lightPushedCell, darkPushedCell);
			CreateColorMap(PrevCellColorMap, lightPrevCell, darkPrevCell);

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					cells[i, j] = new Button();//Массив кнопок

					Button button = new Button();
					button.Size = new Size(cellSize, cellSize);//Задаем размер кнопки
					button.FlatAppearance.BorderSize = 0;//Убeраем рамку у кнопок
					button.FlatStyle = FlatStyle.Flat;//Делаем кнопку плоской, без скгруления
					button.Location = new Point(frameSize + j * cellSize, frameSize + i * cellSize);//Расставляем кнопки
					button.BackColor = chessboardColorMap[i, j];
					button.FlatAppearance.MouseDownBackColor = chessboardColorMap[i, j];//Нужно, чтобы не менялся цвет клетки в момент зажатия кнопки (по дефолту цвет был голубым, некрасиво)

					button.Click += new EventHandler(OnCellPress);//Добавляем ивент

					Controls.Add(button);
					cells[i, j] = button;

					if (defaultMap[i, j].Length != 0)//Расстановка фигур исходя из карты, отрисовка картинок фигур на кнопках
					{
						switch (defaultMap[i, j][1])
						{
							case 'P':
								chess[i, j] = new Pawn(defaultMap[i, j][0]);
								break;
							case 'R':
								chess[i, j] = new Rook(defaultMap[i, j][0]);
								break;
							case 'H':
								chess[i, j] = new Night(defaultMap[i, j][0]);
								break;
							case 'B':
								chess[i, j] = new Bishop(defaultMap[i, j][0]);
								break;
							case 'Q':
								chess[i, j] = new Queen(defaultMap[i, j][0]);
								break;
							case 'K':
								chess[i, j] = new King(defaultMap[i, j][0]);
								break;
						}
						if (chess[i, j] != null)
							chess[i, j].chessSprite = new Bitmap(chess[i, j].chessSprite, new Size(cellSize - 10, cellSize - 10));

						button.BackgroundImage = chess[i, j].chessSprite;
						button.BackgroundImageLayout = ImageLayout.Center;
					}
				}
			}

			//Обводка вокруг шахматной доски
			for (int i = 0; i < 8; i++)
			{
				Label labelVL = new Label();
				Label labelVR = new Label();
				Label labelHT = new Label();
				Label labelHB = new Label();

				VLLabels[i] = labelVL;
				VRLabels[i] = labelVR;
				HTLabels[i] = labelHT;
				HBLabels[i] = labelHB;

				labelVL.Text = labelVR.Text = (8 - i).ToString();
				labelHT.Text = labelHB.Text = ((char)(97 + i)).ToString();

				labelVL.Location = new Point(0, (i * cellSize) + cellSize / 2 + frameSize);
				labelVR.Location = new Point((8 * cellSize) + frameSize, (i * cellSize) + cellSize / 2 + frameSize / 2);

				labelHT.Location = new Point((i * cellSize) + cellSize / 2 + frameSize / 2, 0);
				labelHB.Location = new Point((i * cellSize) + cellSize / 2 + frameSize / 2, (8 * cellSize) + 2 * frameSize / 2);

				labelVL.Font = labelVR.Font = labelHT.Font = labelHB.Font = new Font("Lucida Console", frameSize / 2, FontStyle.Regular);

				labelVL.ForeColor = labelVR.ForeColor = labelHT.ForeColor = labelHB.ForeColor = lightCell;

				labelVL.Size = labelVR.Size = labelHT.Size = labelHB.Size = new Size(frameSize, frameSize);

				labelVL.BackColor = labelVR.BackColor = labelHT.BackColor = labelHB.BackColor = frame;

				labelVL.TextAlign = labelVR.TextAlign = labelHT.TextAlign = labelHB.TextAlign = ContentAlignment.MiddleCenter;

				Controls.Add(labelVL);
				Controls.Add(labelVR);
				Controls.Add(labelHT);
				Controls.Add(labelHB);
			}

			//Рамка вокруг шахматной доски
			labelFrame = new Label();

			labelFrame.Location = new Point(0, 0);

			labelFrame.Size = new Size(cellSize * 8 + frameSize * 2, cellSize * 8 + frameSize * 2);

			labelFrame.BackColor = frame;

			Controls.Add(labelFrame);

			//Место где показываются выбитые фигуры
			for (int i = 0; i < 16; i++)
			{
				Label labelKnokoutW = new Label();
				Label labelKnokoutB = new Label();

				labelKnokoutW.Location = new Point((8 * cellSize) + frameSize * 3 + ((i % 4) * labelSize), frameSize + (i / 4) * labelSize);
				labelKnokoutB.Location = new Point((8 * cellSize) + frameSize * 3 + ((i % 4) * labelSize), (7 * labelSize) + frameSize - (i / 4) * labelSize);

				labelKnokoutB.Size = labelKnokoutW.Size = new Size(labelSize, labelSize);

				labelKnokoutW.BackColor = darkCell;
				labelKnokoutB.BackColor = lightCell;

				Controls.Add(labelKnokoutW);
				Controls.Add(labelKnokoutB);

				WLabelsKnockout[i] = labelKnokoutW;
				BLabelsKnockout[i] = labelKnokoutB;
			}
		}

		/// <summary>
		/// Ивент нажатия на фигуру
		/// </summary>
		/// <param name="sender">Кнопка</param>
		/// <param name="e"></param>
		private void OnCellPress(object sender, EventArgs e)
		{
			StepsCalculate();

			Button pressCell = sender as Button;

			int Y = (pressCell.Location.Y - frameSize) / cellSize;
			int X = (pressCell.Location.X - frameSize) / cellSize;

			int prevY = (prevCell?.Location.Y ?? frameSize - frameSize) / cellSize;
			int prevX = (prevCell?.Location.X ?? frameSize - frameSize) / cellSize;

			Chessman pressChess = chess[Y, X];
			Chessman prevChess = chess[prevY, prevX];

			//Нажатие на подсвечиваемую фигуру
			if (pressCell.BackColor == lightPushedCell || pressCell.BackColor == darkPushedCell)
			{
				ResetBacklightChessboard();
				ResetImageChessboard();
				prevCell = null;
				return;
			}

			//Сделать возможный ход, который подсвечивается 
			else if (pressCell.Image != null)
			{
				if (currentMatch.witeTurn == true)
				{
					if (currentMatch.currentStep != 1)
						SafeFileAppend("\n");
					SafeFileAppend($"{currentMatch.currentStep}.");
				}

				//Не реагировать на нажатие крестика
				if (pressCell.Image == cross)
					return;

				//Рокировка
				if (pressCell.Image == arrowLeft)
				{
					Chessman ShahKing = CheckPotCheck(prevCell, pressCell);

					//Проверка на то, чтобы не поставить себе шах
					if ((ShahKing?.color == 'w' && currentMatch.witeTurn == true)
						|| (ShahKing?.color == 'b' && currentMatch.witeTurn == false))
					{
						return;
					}

					//Обнуление возможности рокировки
					if (chess[prevY, prevX].color == 'b')
					{
						currentMatch.BLeftCastling = false;
						currentMatch.BRightCastling = false;
					}
					else
					{
						currentMatch.WLeftCastling = false;
						currentMatch.WRightCastling = false;
					}

					SafeFileAppend(" 0-0");

					//Короткая 0-0
					if (X == 6)
					{
						FiguresPermutation(cells[Y, 7], cells[Y, 5]);
						FiguresPermutation(prevCell, pressCell);
					}
					//Длинная 0-0-0
					else
					{
						FiguresPermutation(cells[Y, 0], cells[Y, 3]);
						FiguresPermutation(prevCell, pressCell);

						SafeFileAppend("-0");
					}
				}

				//En passant
				else if (pressCell.Image == circle && ((Y - 1 >= 0 && (cells[Y - 1, X].Image == cross)) ||
						(Y + 1 <= 7 && cells[Y + 1, X].Image == cross)))
				{
					SafeFileAppend($" {chess[prevY, prevX].type}{(char)(97 + prevX)}{8 - prevY}/{(char)(97 + X)}{8 - Y}");

					try
					{
						List<Chessman> KnockoutChess = currentMatch.WKnockoutChess;
						Label[] labelsKnockout = WLabelsKnockout;
						int plus_mn = -1;

						if (chess[prevY, prevX]?.color == 'w')
						{
							KnockoutChess = currentMatch.BKnockoutChess;
							labelsKnockout = BLabelsKnockout;
							plus_mn = 1;
						}

						if (cells[Y + plus_mn, X].Image == cross)
						{
							KnockoutChess.Add(chess[Y + plus_mn, X]);
							chess[Y + plus_mn, X] = null;
							int iter = KnockoutChess.Count() - 1;
							labelsKnockout[iter].Image = new Bitmap(KnockoutChess[iter].chessSprite, new Size(labelSize, labelSize));
						}

						FiguresPermutation(prevCell, pressCell);
					}
					catch (IndexOutOfRangeException)
					{
					}
				}

				//Бить или ходить
				else
				{
					Chessman ShahKing = CheckPotCheck(prevCell, pressCell);

					//Проверка на то, чтобы не поставить себе шах
					if ((ShahKing?.color == 'w' && currentMatch.witeTurn == true) ||
						(ShahKing?.color == 'b' && currentMatch.witeTurn == false))
					{
						playExclamation();
						return;
					}

					//Если стоит шах, то нужно его отбить
					if ((currentMatch.witeTurn && currentMatch.WShah) || (!currentMatch.witeTurn && currentMatch.BShah))
					{
						if (ShahKing != null)
							return;

						else
						{
							if (currentMatch.WShah)
								currentMatch.WShah = false;
							else
								currentMatch.BShah = false;							
						}
					}


					SafeFileAppend($" {chess[prevY, prevX].type}{(char)(97 + prevX)}{8 - prevY}{(pressCell.Image == circle ? ':' : '-')}{(char)(97 + X)}{8 - Y}");

					List<Chessman> KnockoutChess = currentMatch.WKnockoutChess;
					Label[] labelsKnockout = WLabelsKnockout;

					if (chess[prevY, prevX]?.color == 'w')
					{
						KnockoutChess = currentMatch.BKnockoutChess;
						labelsKnockout = BLabelsKnockout;
					}

					if (chess[Y, X] != null)
					{
						KnockoutChess.Add(chess[Y, X]);
						int iter = KnockoutChess.Count() - 1;
						labelsKnockout[iter].Image = new Bitmap(KnockoutChess[iter].chessSprite, new Size(labelSize, labelSize));
					}

					FiguresPermutation(prevCell, pressCell);
				}

				//Превращение пешки в другую фигуру
				if (chess[Y, X]?.type == 'P' && (Y == 7 || Y == 0))
					BlockChessboard(true);

				//Cчет хода
				if (currentMatch.witeTurn == true)
					currentMatch.currentStep++;

				//Смена хода
				currentMatch.witeTurn = !currentMatch.witeTurn;

				//Обнуление возможности рокировки
				if (chess[Y, X] != null && chess[Y, X].type == 'K')
				{
					if (chess[Y, X].color == 'b')
					{
						currentMatch.BLeftCastling = false;
						currentMatch.BRightCastling = false;
					}
					else
					{
						currentMatch.WRightCastling = false;
						currentMatch.WLeftCastling = false;
					}
				}
				if (chess[Y, X] != null && chess[Y, X].type == 'R')
				{
					if (prevX == 0)
					{
						if (chess[Y, X].color == 'b')
							currentMatch.BLeftCastling = false;
						else
							currentMatch.WLeftCastling = false;
					}
					else if (prevX == 7)
					{
						if (chess[Y, X].color == 'b')
							currentMatch.BRightCastling = false;
						else
							currentMatch.WRightCastling = false;
					}
				}

				//Обнуление маркеров
				posStepsCalculated = false;
				ResetBacklightChessboard(resetPrev: true);
				ResetImageChessboard();

				//Просчитать возможные ходы
				StepsCalculate();

				//Подсветка предыдущего хода
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						if (chess[i, j] == prevChess)
							cells[i, j].BackColor = PrevCellColorMap[i, j];
					}
				}
				prevCell.BackColor = PrevCellColorMap[prevY, prevX];

				//Обнуление предыдущей кнопки
				prevCell = null;
				ShowChessboard();

				//Проверка на шах
				Chessman King = CheckCheck();

				//Проверкка на мат
				if (King != null)
					CheckMate(ref King);
			}

			//Нажимаем на клетку без подсветки
			else
			{
				prevCell = pressCell;
				ResetBacklightChessboard();
				ResetImageChessboard();

				if (pressChess != null)
				{
					//Проверка на то, чей ход
					if ((currentMatch.witeTurn && pressChess.color == 'b') || (!currentMatch.witeTurn && pressChess.color == 'w'))
						return;

					//Подсветка выбраной фигуры
					pressCell.BackColor = PushedCellColorMap[Y, X];

					BacklightImageChessboard(pressChess);

					prevCell = pressCell;
				}
			}

			BacklightCheck(currentMatch.WShah, 'w');
			BacklightCheck(currentMatch.BShah, 'b');

		}

		public void StepsCalculate()
		{
			if (posStepsCalculated == false)
			{
				foreach (var chessman in chess)
				{
					if (chessman != null)
						chessman.PossibleSteps(this);
				}
				posStepsCalculated = true;
			}
		}

		/// <summary>
		/// Походить фигурой
		/// </summary>
		/// <param name="cellFrom">Откуда ходим</param>
		/// <param name="cellTo">Куда ходим</param>
		public void FiguresPermutation(Button cellFrom, Button cellTo)
		{
			int Y = (cellTo.Location.Y - frameSize) / cellSize;
			int X = (cellTo.Location.X - frameSize) / cellSize;

			int prevY = (cellFrom.Location.Y - frameSize) / cellSize;
			int prevX = (cellFrom.Location.X - frameSize) / cellSize;

			chess[Y, X] = chess[prevY, prevX];
			chess[prevY, prevX] = null;
		}

		public void BacklightCheck(bool Shah, char color)
		{
			int x = -1, y = -1;

			if (Shah == true)
			{
				foreach (var chessman in chess)
				{
					if (chessman!= null && chessman.type == 'K' && chessman.color == color)
					{
						chessman.GetIter(this, ref x, ref y);
						break;
					}
				}
				cells[y, x].BackColor = Color.Red;
			}
		}

		/// <summary>
		/// Взаимодействие с меню превращение пешки
		/// </summary>
		/// <param name="sender">кнопка</param>
		/// <param name="e"></param>
		public void PawnPromotion(object sender, EventArgs e)
		{
			Button pressBttn = sender as Button;

			int X = (pressBttn.Location.X - frameSize) / cellSize;

			Chessman pressChess = PromotionChess[0];

			for (int i = 0; i < 4; i++)
			{
				if (PromotionCells[i] == pressBttn)
					pressChess = PromotionChess[i];
			}

			int y = pressChess.color == 'w' ? 0 : 7;

			chess[y, X] = pressChess;
			cells[y, X].BackgroundImage = new Bitmap(chess[y, X].chessSprite, new Size(cellSize - 10, cellSize - 10));

			foreach (Button cell in PromotionCells)
			{
				Controls.Remove(cell);
			}

			//
			StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{safeFile}.txt", FileMode.Append));
			sw.Write($"={pressChess.type}");
			sw.Close();
			//

			BlockChessboard(false);
		}

		public void SafeFileAppend(string appendstr)
		{
			StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{safeFile}.txt", FileMode.Append));
			sw.Write(appendstr);
			sw.Close();
		}

		/// <summary>
		/// Проверка на то, будет ли шах если походить таким образом
		/// </summary>
		/// <param name="cellFrom">Откуда ходим</param>
		/// <param name="cellTo">Куда ходим</param>
		/// <returns></returns>
		public Chessman CheckPotCheck(Button cellFrom, Button cellTo)
		{
			Chessboard tempCb = new Chessboard(this);

			for (int a = 0; a < 8; a++)
			{
				for (int b = 0; b < 8; b++)
				{
					if (tempCb.chess[a, b] != chess[a, b])
						tempCb.chess[a, b] = (Chessman)chess[a, b]?.Clone();
				}
			}

			int Y = (cellTo.Location.Y - frameSize) / cellSize;
			int X = (cellTo.Location.X - frameSize) / cellSize;

			int prevY = (cellFrom.Location.Y - frameSize) / cellSize;
			int prevX = (cellFrom.Location.X - frameSize) / cellSize;

			tempCb.chess[Y, X] = tempCb.chess[prevY, prevX];
			tempCb.chess[prevY, prevX] = null;

			King whiteKing = new King('w');
			King blackKing = new King('b');

			int wY = 0, wX = 0;
			int bY = 0, bX = 0;

			foreach (var chessman in tempCb.chess)
			{
				if (chessman?.type == 'K')
				{
					if (chessman.color == 'w')
						whiteKing = (King)chessman;
					else
						blackKing = (King)chessman;
				}
			}

			whiteKing.GetIter(tempCb, ref wX, ref wY);
			blackKing.GetIter(tempCb, ref bX, ref bY);

			tempCb.StepsCalculate();

			foreach (var chessman in tempCb.chess)
			{
				if (chessman?.posSteps[wY, wX] == 2)
					return whiteKing;

				else if (chessman?.posSteps[bY, bX] == 2)
					return blackKing;
			}

			return null;
		}

		/// <summary>
		/// Проверка шах ли ?
		/// </summary>
		public Chessman CheckCheck()
		{
			King whiteKing = new King('w');
			King blackKing = new King('b');

			int wY = 0, wX = 0;
			int bY = 0, bX = 0;

			foreach (var chessman in chess)
			{
				if (chessman?.type == 'K')
				{
					if (chessman.color == 'w')
						whiteKing = (King)chessman;
					else
						blackKing = (King)chessman;
				}
			}

			whiteKing.GetIter(this, ref wX, ref wY);
			blackKing.GetIter(this, ref bX, ref bY);

			foreach (var chessman in chess)
			{
				if (chessman?.posSteps[wY, wX] == 2)
				{
					currentMatch.WShah = true;
					cells[wY, wX].BackColor = Color.Red;
					return whiteKing;
				}
				else if (chessman?.posSteps[bY, bX] == 2)
				{
					currentMatch.BShah = true;
					cells[bY, bX].BackColor = Color.Red;
					return blackKing;
				}
			}

			return null;
		}

		/// <summary>
		/// Проверка шах или мат
		/// </summary>
		/// <param name="King">Король</param>
		public void CheckMate(ref Chessman King)
		{
			byte kingPosS1 = 0;
			byte kingPosS2 = 0;

			byte MatChessCount = 0;//Кол-во фигур, поставивших мат
			List<Chessman> MateChessmans = new List<Chessman>();

			int x = 0, y = 0;

			King.GetIter(this, ref x, ref y);

			foreach (var chessman in chess)
			{
				if (chessman?.posSteps[y, x] == 2)
				{
					MatChessCount++;
					MateChessmans.Add(chessman);
				}
			}

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (King.posSteps[i, j] == 1 || King.posSteps[i, j] == 2)
					{
						Chessboard tempCb = new Chessboard(this);

						for (int a = 0; a < 8; a++)
						{
							for (int b = 0; b < 8; b++)
							{
								if (tempCb.chess[a, b] != chess[a, b])
									tempCb.chess[a, b] = (Chessman)chess[a, b]?.Clone();

								if (tempCb.chess[a, b]?.type == 'K')
									tempCb.chess[a, b] = null;
							}
						}

						tempCb.chess[i, j] = (Chessman)King.Clone();

						tempCb.StepsCalculate();

						foreach (var chessman in tempCb.chess)
						{
							if (chessman?.posSteps[i, j] == 2)
							{
								kingPosS1++;
								break;
							}
						}

					}
					else if (King.posSteps[i, j] == 4)
						King.posSteps[i, j] = 0;
				}
			}

			foreach (var step in King.posSteps)
			{
				if (step == 1 || step == 2)
					kingPosS2++;
			}

			if (kingPosS1 == kingPosS2)// Проверка на то, не может ли король сам уйти от шаха
			{
				if (MatChessCount == 1)
				{
					MateChessmans[0].GetIter(this, ref x, ref y);

					foreach (var chessman in chess)
					{
						if (chessman?.posSteps[y, x] == 2)
						{
							SafeFileAppend("+");
							//Если да, то съедаем фигуру.
							return;
						}
					}

					if (MateChessmans[0].type == 'R' || MateChessmans[0].type == 'Q' || MateChessmans[0].type == 'B')
					{
						for (int i = 0; i < 8; i++)
						{
							for (int j = 0; j < 8; j++)
							{
								if (MateChessmans[0].posSteps[i, j] == 1)
								{
									for (int a = 0; a < 8; a++)
									{
										for (int b = 0; b < 8; b++)
										{
											if (chess[a, b] != null && chess[a, b] != MateChessmans[0] && chess[a, b].posSteps[i, j] == 1)
											{
												Chessman CheckKing = CheckPotCheck(cells[a, b], cells[i, j]);
												if (CheckKing == null)
												{
													SafeFileAppend("+");
													//Если да, то закрываем короля.
													return;
												}
											}
										}
									}
								}
							}
						}
					}

					MatActions();
				}

				else
					MatActions();
			}
		}

		/// <summary>
		/// Заблокировать возможность взаимодействовать с шахматной доской
		/// </summary>
		/// <param name="block">заблокировать</param>
		private void BlockChessboard(bool block)
		{
			foreach (Button button in cells)
			{
				button.Enabled = !block;
			}

			if (block)
				FormBorderStyle = FormBorderStyle.FixedSingle;
			else
				FormBorderStyle = FormBorderStyle.Sizable;
		}

		/// <summary>
		/// Подсветка возможных ходов картинками
		/// </summary>
		/// <param name="chessman">шахматная фигура на доске</param>
		private void BacklightImageChessboard(Chessman chessman)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					switch (chessman?.posSteps[i, j])
					{
						case 1:
							cells[i, j].Image = dot;
							break;
						case 2:
							cells[i, j].Image = circle;
							break;
						case 3:
							cells[i, j].Image = cross;
							break;
						case 4:
							cells[i, j].Image = arrowLeft;
							break;
					}

					cells[i, j].ImageAlign = ContentAlignment.MiddleCenter;
				}
			}
		}

		/// <summary>
		/// Сброс подсветок на шахматной доске
		/// </summary>
		/// <param name="resetPrev">Сбросить подсветку предыдущего хода</param>
		private void ResetBacklightChessboard(bool resetPrev = false)
		{
			if (prevCell != null)
			{
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						if (!resetPrev && cells[i, j].BackColor == PrevCellColorMap[i, j])
							continue;
						cells[i, j].BackColor = chessboardColorMap[i, j];
					}
				}
			}
		}

		/// <summary>
		/// Сброс картинок-подсветок
		/// </summary>
		private void ResetImageChessboard()
		{
			foreach (Button cell in cells)
			{
				cell.Image = null;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			foreach (var button in Controls.OfType<Button>())
				button.Visible = false;

			currentMatch = new Match(this);

			Directory.CreateDirectory(savesPath);

			FileStream fs = new FileStream($"{savesPath}\\{safeFile}.txt", FileMode.Create);
			fs.Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Directory.CreateDirectory(savesPath);

			OpenFileDialog OPF = new OpenFileDialog();

			OPF.Filter = "Файлы txt|*.txt";

			OPF.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + savesPath;

			OPF.ShowDialog();

			StreamReader sr;

			try
			{
				sr = new StreamReader(OPF.OpenFile());
			}
			catch (IndexOutOfRangeException)
			{
				return;
			}
			sr.Close();

			foreach (var button in Controls.OfType<Button>())
				button.Visible = false;

			currentMatch = new Match(this);

			safeFile = OPF.FileName;
			int pos = safeFile.LastIndexOf('\\');
			safeFile = safeFile.Substring(pos).Replace("\\", String.Empty).Replace(".txt", String.Empty);

			ReadTheGameFromFile();
		}

		private void ReadTheGameFromFile()
		{
			StreamReader sr = new StreamReader(new FileStream($"Saves\\{safeFile}.txt", FileMode.Open));
			string str = sr.ReadToEnd();
			sr.Close();

			File.Delete($"Saves\\{safeFile}.txt");
			StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{safeFile}.txt", FileMode.Create));
			sw.Close();

			string [] strArray = str.Split('\n');

			for (int a = 0; a < strArray.Length; a++)
			{
				string tempstr = strArray[a].Remove(0, 3);
				string Wstr = "", Bstr = "";
				int pos = tempstr.LastIndexOf(' ');

				if(pos != -1)				
				{
					Wstr = tempstr.Substring(0, pos);
					Bstr = tempstr.Substring(pos + 1);
				}
				else
					Wstr = tempstr;

				SimulatingTheGame(Wstr, 7);

				SimulatingTheGame(Bstr, 0);
			}
		}

		public void playExclamation()
		{
			SystemSounds.Exclamation.Play();
		}

		public void playMateSound()
		{
			SystemSounds.Hand.Play();
		}

		public void MateMessage()
		{
			string str = currentMatch.BShah ? "Білі" : "Чорні";
			MessageBox.Show($"{str} поставили мат", "Кінець гри");

		}

		/// <summary>
		/// Действия при мате
		/// </summary>
		public void MatActions()
		{
			SafeFileAppend("#");
			playMateSound();
			MateMessage();
			Close();
		}

		private void Chessboard_ResizeBegin(object sender, EventArgs e)
		{
			oldWidth = Width;
			oldHeight = Height;
		}

		private void Chessboard_ResizeEnd(object sender, EventArgs e)
		{
			if (cells[0, 0] == null || (oldHeight == Height && oldWidth == Width))
				return;

			int height = Height - 47;
			int width = Width - 18;

			// если изменилась ширина с прошлого раза
			if (oldWidth != Width)
			{
				ClientSize = new Size(width, (int)(width * 1f / formSidesProportion));
				height = Height - 47;
			}

			// если изменилась высота с прошлого раза
			if (oldHeight != Height)
			{
				ClientSize = new Size((int)(height * formSidesProportion), height);
				width = Width - 18;
			}

			cellSize = (int)Math.Round(3.6 / 30.8 * height);//Размер стороны кнопки при изменении размера окна

			labelSize = (int)Math.Round(cellSize / 1.5);
			frameSize = (int)Math.Round(cellSize / 3.6);

			ClientSize = new Size(cellSize * 8 + labelSize * 4 + frameSize * 4, cellSize * 8 + frameSize * 2);//Окончательно меняем размер окна					

			labelFrame.Size = new Size(cellSize * 8 + frameSize * 2, cellSize * 8 + frameSize * 2);//Размер рамки

			dot = new Bitmap(new Bitmap($"Sprites\\dot.png"), new Size(cellSize - 1, cellSize - 1));
			circle = new Bitmap(new Bitmap($"Sprites\\circle.png"), new Size(cellSize - 1, cellSize - 1));
			cross = new Bitmap(new Bitmap($"Sprites\\cross.png"), new Size(cellSize - 1, cellSize - 1));
			arrowLeft = new Bitmap(new Bitmap($"Sprites\\arrowLeft.png"), new Size(cellSize - 1, cellSize - 1));

			//Элементы становятся невидимыми
			RedrawingUI(false);

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					cells[i, j].Location = new Point(frameSize + j * cellSize, frameSize + i * cellSize);
					cells[i, j].Size = new Size(cellSize, cellSize);

					if (cells[i, j].Image != null)
						cells[i, j].Image = new Bitmap(cells[i, j].Image, new Size(cellSize - 1, cellSize - 1));

					if (cells[i, j].BackgroundImage != null)
						cells[i, j].BackgroundImage = new Bitmap(chess[i, j].chessSprite, new Size(cellSize - 10, cellSize - 10));
				}

				VLLabels[i].Location = new Point(0, (i * cellSize) + cellSize / 2 + frameSize);
				VRLabels[i].Location = new Point((8 * cellSize) + frameSize, (i * cellSize) + cellSize / 2 + frameSize / 2);

				HTLabels[i].Location = new Point((i * cellSize) + cellSize / 2 + frameSize / 2, 0);
				HBLabels[i].Location = new Point((i * cellSize) + cellSize / 2 + frameSize / 2, (8 * cellSize) + 2 * frameSize / 2);

				VLLabels[i].Size = VRLabels[i].Size = HTLabels[i].Size = HBLabels[i].Size = new Size(frameSize, frameSize);

				VLLabels[i].Font = VRLabels[i].Font = HTLabels[i].Font = HBLabels[i].Font = new Font("Lucida Console", frameSize / 2, FontStyle.Regular);
			}

			for (int i = 0; i < 16; i++)
			{
				WLabelsKnockout[i].Size = BLabelsKnockout[i].Size = new Size(labelSize, labelSize);

				WLabelsKnockout[i].Location = new Point((8 * cellSize) + frameSize * 3 + ((i % 4) * labelSize), frameSize + (i / 4) * labelSize);
				BLabelsKnockout[i].Location = new Point((8 * cellSize) + frameSize * 3 + ((i % 4) * labelSize), (7 * labelSize) + frameSize - (i / 4) * labelSize);

				if (WLabelsKnockout[i].Image != null)
					WLabelsKnockout[i].Image = new Bitmap(currentMatch.WKnockoutChess[i].chessSprite, new Size(labelSize, labelSize));
				if (BLabelsKnockout[i].Image != null)
					BLabelsKnockout[i].Image = new Bitmap(currentMatch.BKnockoutChess[i].chessSprite, new Size(labelSize, labelSize));
			}

			//Элементы становятся видимыми
			RedrawingUI(true);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void SimulatingTheGame(string str, int Y)
		{
			string letters = "abcdefgh";
			string numbers = "12345678";

			EventArgs e = null;

			int X1 = -1, Y1 = -1;
			int X2 = -1, Y2 = -1;
			int X3 = -1;

			for (int i = 0; i < str.Length; i++)
			{
				if (letters.Contains(str[i]))
				{
					if (X1 == -1)
						X1 = str[i] - 97;
					else
						X2 = str[i] - 97;
				}
				if (numbers.Contains(str[i]))
				{
					if (Y1 == -1)
						Y1 = Math.Abs(str[i] - 48 - 8);
					else
						Y2 = Math.Abs(str[i] - 48 - 8);
				}
				else if (str[i] == '0')
				{
					Y1 = Y;
					Y2 = Y;
					X1 = 4;
					if (str.Length == 3)
					{
						X2 = 6;
					}
					else
					{
						X2 = 2;
					}
					break;
				}
				else if (str[i] == '=')
				{
					switch (str[i + 1])
					{
						case 'Q':
							X3 = 0;
							break;
						case 'R':
							X3 = 1;
							break;
						case 'B':
							X3 = 2;
							break;
						case 'H':
							X3 = 3;
							break;
					}
				}
				
			}
			if (str.Length > 0)
			{
				InvokeOnClick(cells[Y1, X1], e);
				InvokeOnClick(cells[Y2, X2], e);
				if (X3 >= 0)
					InvokeOnClick(PromotionCells[X3], e);
			}
		}

	}

}