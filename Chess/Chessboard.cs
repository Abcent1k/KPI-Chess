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
		public Bitmap arrowRight;

		public Color lightCell = Color.FromArgb(255, 237, 252, 248);//Светлый цвет для карты цветов
		public Color darkCell = Color.FromArgb(255, 0, 87, 62);//Темный цвет для карты цветов
		public Color frame = Color.FromArgb(255, 0, 43, 29);//Цвет рамки вокруг шахматной доски

		public Color lightPushedCell = Color.LightGreen;//Цвет нажатой кнопки, если клетка под фигурой светлая
		public Color darkPushedCell = Color.MediumSeaGreen;//Цвет нажатой кнопки, если клетка под фигурой темная

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

		public Color[,] colorMap = new Color[8, 8];//Карта цветов шахматной доски

		public Button? prevCell;

		public Button[,] cells = new Button[8, 8];//Массив с кнопками

		public Chessman[,] chess = new Chessman[8, 8];//Массив с фигурами

		public Label[] WLabelsKnockout = new Label[16];
		public Label[] BLabelsKnockout = new Label[16];

		public Label labelFrame;

		public Label LabelTurn;

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

		public void Init()
		{
			CreateChessboard();
		}

		/// <summary>
		/// Создание карты цветов шахматной доски
		/// </summary>
		/// <param name="light">Светлая клетка</param>
		/// <param name="dark">Темная клетка</param>
		public void CreateColorMap(Color light, Color dark)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
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
			LabelTurn.Visible = showElement;
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
			arrowRight = new Bitmap(new Bitmap($"Sprites\\arrowRight.png"), new Size(cellSize - 1, cellSize - 1));

			frameSize = (int)(cellSize / 3.6);
			labelSize = (int)(cellSize / 1.5);

			formSidesProportion = (float)(Width - 18) / (float)(Height - 47);

			CreateColorMap(lightCell, darkCell);

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
					button.BackColor = colorMap[i, j];
					button.FlatAppearance.MouseDownBackColor = colorMap[i, j];//Нужно, чтобы не менялся цвет клетки в момент зажатия кнопки (по дефолту цвет был голубым, некрасиво)

					button.Click += new EventHandler(OnFigurePress);//Добавляем ивент

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

			LabelTurn = new Label();

			LabelTurn.Location = new Point((8 * cellSize + frameSize * 3), labelSize * 8 + frameSize * 2);

			LabelTurn.Size = new Size(labelSize * 4, frameSize * 2);

			LabelTurn.BackColor = Color.White;
			LabelTurn.Text = "White";
			LabelTurn.Font = new Font("Lucida Console", frameSize, FontStyle.Regular);
			LabelTurn.TextAlign = ContentAlignment.MiddleCenter;

			Controls.Add(LabelTurn);
		}

		/// <summary>
		/// Ивент нажатия на фигуру
		/// </summary>
		/// <param name="sender">Кнопка</param>
		/// <param name="e"></param>
		private void OnFigurePress(object sender, EventArgs e)
		{
			Button pressCell = sender as Button;

			int Y = (pressCell.Location.Y - frameSize) / cellSize;
			int X = (pressCell.Location.X - frameSize) / cellSize;

			int prevY = (prevCell?.Location.Y ?? frameSize - frameSize) / cellSize;
			int prevX = (prevCell?.Location.X ?? frameSize - frameSize) / cellSize;

			Chessman pressChess = chess[Y, X];

			//Нажатие на подсвечиваемую фигуру
			if (pressCell.BackColor == lightPushedCell || pressCell.BackColor == darkPushedCell)
			{
				ResetBacklightChessboard();
				prevCell = null;
				return;
			}

			//Сделать возможный ход, который подсвечивается 
			else if (pressCell.Image != null)
			{
				//Механика сохранения
				StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{currentMatch.safeFile}.txt", FileMode.Append));
				//
				if (currentMatch.witeTurn == true)
				{
					if (currentMatch.currentStep != 1)
						sw.Write('\n');
					sw.Write($"{currentMatch.currentStep}.");
				}
				//

				//Не реагировать на нажатие крестика
				if (pressCell.Image == cross)
					return;

				//Рокировка
				if (pressCell.Image == arrowLeft || pressCell.Image == arrowRight)
				{
					//Обнуление возможности рокировки
					if (chess[Y, X].color == 'b')
						currentMatch.BCastling = false;
					else
						currentMatch.WCastling = false;

					//
					sw.Write(" ");
					sw.Write("0-0");
					//

					//Короткая 0-0
					if ((pressCell.Image == arrowLeft && chess[Y, X].type == 'R') || (pressCell.Image == arrowRight && chess[Y, X].type == 'K'))
					{
						FiguresPermutation(cells[Y, 7], cells[Y, 5]);
						FiguresPermutation(cells[Y, 4], cells[Y, 6]);
					}
					//Длинная 0-0-0
					else
					{
						FiguresPermutation(cells[Y, 0], cells[Y, 3]);
						FiguresPermutation(cells[Y, 4], cells[Y, 2]);
						//
						sw.Write("-0");
						//
					}					

					//					
					sw.Close();
					//
				}
				//En passant
				else if (pressCell.Image == circle && ((Y - 1 >= 0 && (cells[Y - 1, X].Image == cross)) || (Y + 1 <= 7 && cells[Y + 1, X].Image == cross)))
				{
					//
					sw.Write(" ");
					sw.Write($"{chess[prevY, prevX].type}{(char)(97 + prevX)}{8 - prevY}");
					sw.Write($"/{(char)(97 + X)}{8 - Y}");
					sw.Close();
					//

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
					//
					sw.Write(" ");
					sw.Write($"{chess[prevY, prevX].type}{(char)(97 + prevX)}{8 - prevY}");
					sw.Write($"{(pressCell.Image == circle ? ':' : '-')}{(char)(97 + X)}{8 - Y}");
					sw.Close();
					//

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
				{
					chess[Y, X].PossibleSteps(this);

					BlockChessboard(true);
				}

				//Смена хода + счет хода + подсветка чей ход
				if (currentMatch.witeTurn == true)
				{
					currentMatch.currentStep++;

					LabelTurn.BackColor = Color.Black;
					LabelTurn.Text = "Black";
					LabelTurn.ForeColor = Color.White;
				}
				else
				{
					LabelTurn.BackColor = Color.White;
					LabelTurn.Text = "White";
					LabelTurn.ForeColor = Color.Black;
				}

				currentMatch.witeTurn = !currentMatch.witeTurn;

				//Обнуление возможности рокировки
				if (chess[Y, X] != null && (chess[Y, X].type == 'K' || chess[Y, X].type == 'R'))
				{
					if (chess[Y, X].color == 'b')
						currentMatch.BCastling = false;
					else
						currentMatch.WCastling = false;
				}

				//Обнуление маркеров
				posStepsCalculated = false;

				ResetBacklightChessboard();
				prevCell = null;//Обнуление предыдущей кнопки
				ShowChessboard();
				return;
			}

			//Нажимаем на клетку без подсветки
			else
			{
				prevCell = pressCell;
				ResetBacklightChessboard();

				if (pressChess != null)
				{
					//Проверка на то, чей ход
					if ((currentMatch.witeTurn && pressChess.color == 'b') || (!currentMatch.witeTurn && pressChess.color == 'w'))
						return;

					//Посчитать возможные ходы
					if (posStepsCalculated == false)
					{
						foreach (var chessman in chess)
						{
							if (chessman != null)
								chessman.PossibleSteps(this);
							posStepsCalculated = true;
						}
					}

					//Подсветка выбраной фигуры
					if (pressCell.BackColor == lightCell)
						pressCell.BackColor = lightPushedCell;
					else
						pressCell.BackColor = darkPushedCell;

					BacklightChessboard(pressChess);

					prevCell = pressCell;
				}
			}
		}

		public void FiguresPermutation(Button cellFrom, Button cellTo)
		{
			int Y = (cellTo.Location.Y - frameSize) / cellSize;
			int X = (cellTo.Location.X - frameSize) / cellSize;

			int prevY = (cellFrom.Location.Y - frameSize) / cellSize;
			int prevX = (cellFrom.Location.X - frameSize) / cellSize;

			chess[Y, X] = chess[prevY, prevX];
			chess[prevY, prevX] = null;
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

			foreach (Button button in PromotionCells)
			{
				Controls.Remove(button);
			}

			//
			StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{currentMatch.safeFile}.txt", FileMode.Append));
			sw.Write($"={pressChess.type}");
			sw.Close();
			//

			BlockChessboard(false);
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
		/// Подсветка возможных ходов
		/// </summary>
		/// <param name="chessman">шахматная фигура на доске</param>
		private void BacklightChessboard(Chessman chessman)
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
						case 5:
							cells[i, j].Image = arrowRight;
							break;
					}

					cells[i, j].ImageAlign = ContentAlignment.MiddleCenter;
				}
			}
		}

		/// <summary>
		/// Сброс подсветок на шахматной доске
		/// </summary>
		private void ResetBacklightChessboard()
		{
			if (prevCell != null)
			{
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						cells[i, j].BackColor = colorMap[i, j];
						cells[i, j].Image = null;
					}
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Button pressBttn = sender as Button;
			pressBttn.Visible = false;

			currentMatch = new Match(this);
		}

		private void Chessboard_ResizeBegin(object sender, EventArgs e)
		{
			oldWidth = Width;
			oldHeight = Height;
		}

		private void Chessboard_Load(object sender, EventArgs e)
		{

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
			arrowRight = new Bitmap(new Bitmap($"Sprites\\arrowRight.png"), new Size(cellSize - 1, cellSize - 1));

			//Элементы становятся невидимыми
			RedrawingUI(false);

			LabelTurn.Location = new Point((8 * cellSize + frameSize * 3), labelSize * 8 + frameSize * 2);
			LabelTurn.Size = new Size(labelSize * 4, frameSize * 2);
			LabelTurn.Font = new Font("Lucida Console", frameSize, FontStyle.Regular);

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

	}

}