using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
	public partial class Chessboard : Form
	{
		public int buttonSize = 90;//Размер клетки (кнопки)	
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

		public Button? prevBttn;

		public Button[,] buttons = new Button[8, 8];//Массив с кнопками

		public Chessman[,] chess = new Chessman[8, 8];//Массив с фигурами


		public Label[] WlabelsKnockout = new Label[16];
		public Label[] BlabelsKnockout = new Label[16];

		public Label labelFrame;

		public Label turnLabel;

		public Label[] VLLabels = new Label[8];
		public Label[] VRLabels = new Label[8];
		public Label[] HTLabels = new Label[8];
		public Label[] HBLabels = new Label[8];


		public Chessman[] PromotionChess = new Chessman[4];
		public Button[] PromotionButtons = new Button[4];

		public Chessboard()
		{
			InitializeComponent();
		}

		public void Init()
		{
			CreateChessboard();
		}

		public void CreateColorMap(Color light, Color dark)//Создание карты цветов шахматной доски
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

		public void ShowChessboard()//Перерисовка шахматной доски
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (chess[i, j] != null)
						buttons[i, j].BackgroundImage = new Bitmap(chess[i, j].chessSprite, new Size(buttonSize - 10, buttonSize - 10));
					else
						buttons[i, j].BackgroundImage = null;

					buttons[i, j].BackgroundImageLayout = ImageLayout.Center;
				}
			}
		}

		//Нужен для красивой отрисовки UI при изменении размеров окна
		private void RedrawingUI(bool showElement)
		{
			turnLabel.Visible = showElement;
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					buttons[i, j].Visible = showElement;

					if ((i * 8 + j) < 16)
					{
						WlabelsKnockout[i * 8 + j].Visible = showElement;
						BlabelsKnockout[i * 8 + j].Visible = showElement;
					}
				}
				VLLabels[i].Visible = VRLabels[i].Visible = HTLabels[i].Visible = HBLabels[i].Visible = showElement;
			}
		}

		public void CreateChessboard()//Создание шахманой доски
		{
			dot = new Bitmap(new Bitmap($"Sprites\\dot.png"), new Size(buttonSize, buttonSize));
			circle = new Bitmap(new Bitmap($"Sprites\\circle.png"), new Size(buttonSize - 1, buttonSize - 1));
			cross = new Bitmap($"Sprites\\cross.png");
			arrowLeft = new Bitmap(new Bitmap($"Sprites\\arrowLeft.png"), new Size(buttonSize - 1, buttonSize - 1));
			arrowRight = new Bitmap(new Bitmap($"Sprites\\arrowRight.png"), new Size(buttonSize - 1, buttonSize - 1));

			frameSize = (int)(buttonSize / 3.6);
			labelSize = (int)(buttonSize / 1.5);

			formSidesProportion = (float)(Width - 18) / (float)(Height - 47);

			CreateColorMap(lightCell, darkCell);

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					buttons[i, j] = new Button();//Массив кнопок

					Button button = new Button();
					button.Size = new Size(buttonSize, buttonSize);//Задаем размер кнопки
					button.FlatAppearance.BorderSize = 0;//Убeраем рамку у кнопок
					button.FlatStyle = FlatStyle.Flat;//Делаем кнопку плоской, без скгруления
					button.Location = new Point(frameSize + j * buttonSize, frameSize + i * buttonSize);//Расставляем кнопки
					button.BackColor = colorMap[i, j];
					button.FlatAppearance.MouseDownBackColor = colorMap[i, j];//Нужно, чтобы не менялся цвет клетки в момент зажатия кнопки (по дефолту цвет был голубым, некрасиво)

					button.Click += new EventHandler(OnFigurePress);//Добавляем ивент

					Controls.Add(button);
					buttons[i, j] = button;

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
							chess[i, j].chessSprite = new Bitmap(chess[i, j].chessSprite, new Size(buttonSize - 10, buttonSize - 10));

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

				labelVL.Location = new Point(0, (i * buttonSize) + buttonSize / 2 + frameSize);
				labelVR.Location = new Point((8 * buttonSize) + frameSize, (i * buttonSize) + buttonSize / 2 + frameSize / 2);

				labelHT.Location = new Point((i * buttonSize) + buttonSize / 2 + frameSize / 2, 0);
				labelHB.Location = new Point((i * buttonSize) + buttonSize / 2 + frameSize / 2, (8 * buttonSize) + 2 * frameSize / 2);

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

			labelFrame.Size = new Size(buttonSize * 8 + frameSize * 2, buttonSize * 8 + frameSize * 2);

			labelFrame.BackColor = frame;

			Controls.Add(labelFrame);

			//Место где показываются выбитые фигуры
			for (int i = 0; i < 16; i++)
			{
				Label labelKnokoutW = new Label();
				Label labelKnokoutB = new Label();

				labelKnokoutW.Location = new Point((8 * buttonSize) + frameSize * 3 + ((i % 4) * labelSize), frameSize + (i / 4) * labelSize);
				labelKnokoutB.Location = new Point((8 * buttonSize) + frameSize * 3 + ((i % 4) * labelSize), (7 * labelSize) + frameSize - (i / 4) * labelSize);

				labelKnokoutB.Size = labelKnokoutW.Size = new Size(labelSize, labelSize);

				labelKnokoutW.BackColor = darkCell;
				labelKnokoutB.BackColor = lightCell;

				Controls.Add(labelKnokoutW);
				Controls.Add(labelKnokoutB);

				WlabelsKnockout[i] = labelKnokoutW;
				BlabelsKnockout[i] = labelKnokoutB;
			}

			turnLabel = new Label();

			turnLabel.Location = new Point((8 * buttonSize + frameSize * 3), labelSize * 8 + frameSize * 2);

			turnLabel.Size = new Size(labelSize * 4, frameSize * 2);

			turnLabel.BackColor = Color.White;
			turnLabel.Text = "White";
			turnLabel.Font = new Font("Lucida Console", frameSize, FontStyle.Regular);
			turnLabel.TextAlign = ContentAlignment.MiddleCenter;

			Controls.Add(turnLabel);
		}

		private void OnFigurePress(object sender, EventArgs e)//Ивент нажатия на фигуру
		{
			Button pressBttn = sender as Button;

			int Y = (pressBttn.Location.Y - frameSize) / buttonSize;
			int X = (pressBttn.Location.X - frameSize) / buttonSize;

			int prevY = (prevBttn?.Location.Y ?? frameSize - frameSize) / buttonSize;
			int prevX = (prevBttn?.Location.X ?? frameSize - frameSize) / buttonSize;

			Chessman pressChess = chess[Y, X];

			//Нажатие на подсвечиваемую фигуру
			if (pressBttn.BackColor == lightPushedCell || pressBttn.BackColor == darkPushedCell)
			{
				ResetBacklightChessboard();
				prevBttn = null;
				return;
			}

			//Сделать возможный ход, который подсвечивается 
			else if (pressBttn.Image != null)
			{
				//Механика сохранения
				StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{currentMatch.safeFile}.txt", FileMode.Append));
				//
				if (currentMatch.roundW == true)
				{
					if (currentMatch.currentStep != 1)
						sw.Write('\n');
					sw.Write($"{currentMatch.currentStep}.");
				}

				//

				//Не реагировать на нажатие крестика
				if (pressBttn.Image == cross)
					return;

				//Рокировка
				if (pressBttn.Image == arrowLeft || pressBttn.Image == arrowRight)
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

					if (pressBttn.Image == arrowLeft)
					{
						//Короткая 0-0
						if (chess[Y, X].type == 'R')
						{
							chess[Y, 5] = chess[Y, X];
							chess[Y, X] = null;

							chess[Y, 6] = chess[Y, 4];
							chess[Y, 4] = null;
						}
						//Длинная 0-0-0
						else if (chess[Y, X].type == 'K')
						{
							chess[Y, 2] = chess[Y, X];
							chess[Y, X] = null;

							chess[Y, 3] = chess[Y, 0];
							chess[Y, 0] = null;

							//
							sw.Write("-0");
							//
						}
					}
					else
					{
						//Длинная 0-0-0
						if (chess[Y, X].type == 'R')
						{
							chess[Y, 3] = chess[Y, X];
							chess[Y, X] = null;

							chess[Y, 2] = chess[Y, 4];
							chess[Y, 4] = null;

							//
							sw.Write("-0");
							//
						}
						//Короткая 0-0
						else if (chess[Y, X].type == 'K')
						{
							chess[Y, 6] = chess[Y, X];
							chess[Y, X] = null;

							chess[Y, 5] = chess[Y, 7];
							chess[Y, 7] = null;
						}
					}

					//					
					sw.Close();
					//
				}

				// En passant
				else if (pressBttn.Image == circle && ((Y - 1 >= 0 && (buttons[Y - 1, X].Image == cross)) || (Y + 1 <= 7 && buttons[Y + 1, X].Image == cross)))
				{
					try
					{
						//
						sw.Write(" ");
						sw.Write($"{chess[prevY, prevX].type}{(char)(97 + prevX)}{8 - prevY}");
						sw.Write("/" + ((char)(97 + X)).ToString() + (8 - Y).ToString());

						sw.Close();
						//

						int plus_mn = -1;
						var KnockoutChess = currentMatch.WKnockoutChess;
						var labelsKnockout = WlabelsKnockout;

						if (chess[prevY, prevX].color == 'w')
						{
							plus_mn = 1;
							KnockoutChess = currentMatch.BKnockoutChess;
							labelsKnockout = BlabelsKnockout;
						}

						KnockoutChess.Add(chess[Y + plus_mn, X]);
						int iter = KnockoutChess.Count() - 1;
						labelsKnockout[iter].Image = new Bitmap(KnockoutChess[iter].chessSprite, new Size(labelSize, labelSize));
						chess[Y + plus_mn, X] = null;

						//Перестановка фигур
						chess[Y, X] = chess[prevY, prevX];
						chess[prevY, prevX] = null;

					}
					catch (IndexOutOfRangeException) { }
				}

				//Реализация бить или ходить
				else
				{
					//
					sw.Write(" ");
					sw.Write($"{chess[prevY, prevX].type}{(char)(97 + prevX)}{8 - prevY}");
					//

					if (pressBttn.Image == circle)
					{
						List<Chessman> KnockoutChess = currentMatch.WKnockoutChess;
						Label[] labelsKnockout = WlabelsKnockout;

						if (chess[Y, X]?.color == 'b')
						{
							KnockoutChess = currentMatch.BKnockoutChess;
							labelsKnockout = BlabelsKnockout;
						}

						KnockoutChess.Add(chess[Y, X]);
						int iter = KnockoutChess.Count() - 1;
						labelsKnockout[iter].Image = new Bitmap(KnockoutChess[iter].chessSprite, new Size(labelSize, labelSize));

						//
						sw.Write(":");
						//
					}
					//
					else
						sw.Write("-");

					sw.Write($"{(char)(97 + X)}{8 - Y}");
					sw.Close();
					//

					//Перестановка фигур
					chess[Y, X] = chess[prevY, prevX];
					chess[prevY, prevX] = null;
				}

				//Превращение пешки в другую фигуру
				if (chess[Y, X]?.type == 'P' && (Y == 7 || Y == 0))
				{
					chess[Y, X].PossibleSteps(this);

					BlockChessboard(true);
				}

				//Смена хода + счет хода + подсветка чей ход
				if (currentMatch.roundW == true)
				{
					currentMatch.currentStep++;

					turnLabel.BackColor = Color.Black;
					turnLabel.Text = "Black";					
					turnLabel.ForeColor = Color.White;					
				}
				else
				{					
					turnLabel.BackColor = Color.White;
					turnLabel.Text = "White";					
					turnLabel.ForeColor = Color.Black;				
				}

				currentMatch.roundW = !currentMatch.roundW;

				//Обнуление возможности рокировки
				if (chess[Y, X] != null && (chess[Y, X].type == 'K' || chess[Y, X].type == 'R'))
				{
					if (chess[Y, X].color == 'b')
						currentMatch.BCastling = false;
					else
						currentMatch.WCastling = false;
				}

				//Обнуление маркеров
				foreach (Chessman chessman in chess)
				{
					if (chessman != null)
						chessman.posStepCalculated = false;
				}

				ResetBacklightChessboard();
				prevBttn = null;//Обнуление предыдущей кнопки
				ShowChessboard();
				return;
			}

			//Нажимаем на клетку без подсветки
			else
			{
				prevBttn = pressBttn;
				ResetBacklightChessboard();

				if (pressChess != null)
				{
					if ((currentMatch.roundW && pressChess.color == 'b') || (!currentMatch.roundW && pressChess.color == 'w'))//Проверка на то, чей ход
						return;

					if (pressChess.posStepCalculated == false)//Расчет возможных ходов выбраной фигуры
					{
						pressChess.PossibleSteps(this);
						pressChess.posStepCalculated = true;//Установка маркера
					}

					if (pressBttn.BackColor == lightCell)//Подсветка выбраной фигуры
						pressBttn.BackColor = lightPushedCell;
					else
						pressBttn.BackColor = darkPushedCell;

					BacklightChessboard(pressChess);

					prevBttn = pressBttn;
				}

			}
									
		}

		//Отображение меню превращение пешки 
		public void PawnPromotion(object sender, EventArgs e)
		{
			Button pressBttn = sender as Button;

			int X = (pressBttn.Location.X - frameSize) / buttonSize;


			int iter = 0;
			for (int i = 0; i < 4; i++)
			{
				if (PromotionButtons[i] == pressBttn)
					iter = i;
			}
			Chessman pressChess = PromotionChess[iter];


			int y;
			if (pressChess.color == 'w')
				y = 0;
			else
				y = 7;


			chess[y, X] = pressChess;
			buttons[y, X].BackgroundImage = new Bitmap(chess[y, X].chessSprite, new Size(buttonSize - 10, buttonSize - 10));


			foreach (Button button in PromotionButtons)
			{
				Controls.Remove(button);
			}

			//Механика сохранения
			StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{currentMatch.safeFile}.txt", FileMode.Append));
			sw.Write($"={pressChess.type}");
			sw.Close();
			//

			BlockChessboard(false);
		}

		//Заблокировать возможность взаимодействовать с шахматной доской
		private void BlockChessboard(bool block)
		{
			foreach (Button button in buttons)
			{
				button.Enabled = !block;
			}

			if (block)
				FormBorderStyle = FormBorderStyle.FixedSingle;
			else
				FormBorderStyle = FormBorderStyle.Sizable;
		}

		//Подсветка возможных ходов
		private void BacklightChessboard(Chessman chessman)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					switch (chessman?.posSteps[i, j])
					{
						case 1:
							buttons[i, j].Image = dot;
							break;
						case 2:
							buttons[i, j].Image = circle;
							break;
						case 3:
							buttons[i, j].Image = cross;
							break;
						case 4:
							buttons[i, j].Image = arrowLeft;
							break;
						case 5:
							buttons[i, j].Image = arrowRight;
							break;
					}

					buttons[i, j].ImageAlign = ContentAlignment.MiddleCenter;
				}
			}
		}

		//Сброс подсветок на шахматной доске
		private void ResetBacklightChessboard()
		{
			if (prevBttn != null)
			{
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						buttons[i, j].BackColor = colorMap[i, j];
						buttons[i, j].Image = null;
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
			if (buttons[0, 0] == null || (oldHeight == Height && oldWidth == Width))
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

			buttonSize = (int)(3.6 / 30.8 * height);//Размер стороны кнопки при изменении размера окна

			labelSize = (int)(buttonSize / 1.5);
			frameSize = (int)(buttonSize / 3.6);

			ClientSize = new Size(buttonSize * 8 + labelSize * 4 + frameSize * 4, buttonSize * 8 + frameSize * 2);//Окончательно меняем размер окна					

			labelFrame.Size = new Size(buttonSize * 8 + frameSize * 2, buttonSize * 8 + frameSize * 2);//Размер рамки

			dot = new Bitmap(new Bitmap($"Sprites\\dot.png"), new Size(buttonSize - 1, buttonSize - 1));
			circle = new Bitmap(new Bitmap($"Sprites\\circle.png"), new Size(buttonSize - 1, buttonSize - 1));
			cross = new Bitmap(new Bitmap($"Sprites\\cross.png"), new Size(buttonSize - 1, buttonSize - 1));
			arrowLeft = new Bitmap(new Bitmap($"Sprites\\arrowLeft.png"), new Size(buttonSize - 1, buttonSize - 1));
			arrowRight = new Bitmap(new Bitmap($"Sprites\\arrowRight.png"), new Size(buttonSize - 1, buttonSize - 1));

			//Элементы становятся невидимыми
			RedrawingUI(false);

			turnLabel.Location = new Point((8 * buttonSize + frameSize * 3), labelSize * 8 + frameSize * 2);
			turnLabel.Size = new Size(labelSize * 4, frameSize * 2);
			turnLabel.Font = new Font("Lucida Console", frameSize, FontStyle.Regular);

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					buttons[i, j].Location = new Point(frameSize + j * buttonSize, frameSize + i * buttonSize);
					buttons[i, j].Size = new Size(buttonSize, buttonSize);

					if (buttons[i, j].Image != null)
						buttons[i, j].Image = new Bitmap(buttons[i, j].Image, new Size(buttonSize - 1, buttonSize - 1));

					if (buttons[i, j].BackgroundImage != null)
						buttons[i, j].BackgroundImage = new Bitmap(chess[i, j].chessSprite, new Size(buttonSize - 10, buttonSize - 10));
				}

				VLLabels[i].Location = new Point(0, (i * buttonSize) + buttonSize / 2 + frameSize);
				VRLabels[i].Location = new Point((8 * buttonSize) + frameSize, (i * buttonSize) + buttonSize / 2 + frameSize / 2);

				HTLabels[i].Location = new Point((i * buttonSize) + buttonSize / 2 + frameSize / 2, 0);
				HBLabels[i].Location = new Point((i * buttonSize) + buttonSize / 2 + frameSize / 2, (8 * buttonSize) + 2 * frameSize / 2);

				VLLabels[i].Size = VRLabels[i].Size = HTLabels[i].Size = HBLabels[i].Size = new Size(frameSize, frameSize);

				VLLabels[i].Font = VRLabels[i].Font = HTLabels[i].Font = HBLabels[i].Font = new Font("Lucida Console", frameSize / 2, FontStyle.Regular);
			}

			for (int i = 0; i < 16; i++)
			{
				WlabelsKnockout[i].Size = BlabelsKnockout[i].Size = new Size(labelSize, labelSize);

				WlabelsKnockout[i].Location = new Point((8 * buttonSize) + frameSize * 3 + ((i % 4) * labelSize), frameSize + (i / 4) * labelSize);
				BlabelsKnockout[i].Location = new Point((8 * buttonSize) + frameSize * 3 + ((i % 4) * labelSize), (7 * labelSize) + frameSize - (i / 4) * labelSize);

				if (WlabelsKnockout[i].Image != null)
					WlabelsKnockout[i].Image = new Bitmap(currentMatch.WKnockoutChess[i].chessSprite, new Size(labelSize, labelSize));
				if (BlabelsKnockout[i].Image != null)
					BlabelsKnockout[i].Image = new Bitmap(currentMatch.BKnockoutChess[i].chessSprite, new Size(labelSize, labelSize));
			}

			//Элементы становятся видимыми
			RedrawingUI(true);
		}

	}

}