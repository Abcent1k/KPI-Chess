using System;

namespace Chess
{
	public partial class Chessboard : Form
	{
		public static int sideSize = 90;//Размер клетки (кнопки)
		public Bitmap dot = new Bitmap (new Bitmap($"Sprites\\1dot.png"),new Size(sideSize - 3, sideSize - 3));
		public Bitmap border = new Bitmap (new Bitmap($"Sprites\\1border.png"),new Size(sideSize - 3, sideSize - 3));
		public Bitmap x = new Bitmap($"Sprites\\x.png");

		public Color lightCell = Color.FromArgb(255, 237, 252, 248);//Светлый цвет для карты цветов
		public Color darkCell = Color.FromArgb(255, 0, 87, 62);//Темный цвет для карты цветов
		public Color contour = Color.FromArgb(255, 0, 43, 29);

		public Color lightPushedCell = Color.LightGreen;//Цвет нажатой кнопки, если клетка под фигурой светлая
		public Color darkPushedCell = Color.MediumSeaGreen;//Цвет нажатой кнопки, если клетка под фигурой темная

		public Match currentMatch;		

		public string[,] defaultMap = new string[8, 8]//Карта начальных позиций фигур
		{
			{"bR","bH","bB","bK","bQ","bB","bH","bR"},
			{"bP","bP","bP","bP","bP","bP","bP","bP"},
			{"","","","","","","",""},
			{"","","","","","","",""},
			{"","","","","","","",""},
			{"","","","","","","",""},
			{"wP","wP","wP","wP","wP","wP","wP","wP"},
			{"wR","wH","wB","wK","wQ","wB","wH","wR"},
		};

		public Color[,] colorMap = new Color[8, 8];//Карта цветов шахматной доски

		public Button? prevBttn;

		public Button[,] buttons = new Button[8, 8];//Массив с кнопками

		public Label[] labels1 = new Label[8];

		public Chessman[,] chess = new Chessman[8, 8];//Массив с фигурами

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
						chess[i, j].chessSprite = new Bitmap(chess[i, j].chessSprite, new Size(sideSize - 10, sideSize - 10));
					buttons[i, j].BackgroundImage = chess[i, j]?.chessSprite;					
					buttons[i, j].BackgroundImageLayout = ImageLayout.Center;					
				}
			}
		}

		public void CreateChessboard()//Создание шахманой доски
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					buttons[i, j] = new Button();//Массив кнопок

					Button button = new Button();
					button.Size = new Size(sideSize, sideSize);//Задаем размер кнопки
					button.FlatAppearance.BorderSize = 0;//Убeраем рамку у кнопок
					button.FlatStyle = FlatStyle.Flat;//Делаем кнопку плоской, без скгруления
					button.Location = new Point(20 + j * sideSize, 20 + i * sideSize);//Расставляем кнопки
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
							chess[i, j].chessSprite = new Bitmap(chess[i, j].chessSprite, new Size(sideSize - 10, sideSize - 10));
						button.BackgroundImage = chess[i, j].chessSprite;
						button.BackgroundImageLayout = ImageLayout.Center;
					}

					CreateColorMap(lightCell, darkCell);

					buttons[i, j].FlatAppearance.MouseDownBackColor = colorMap[i, j];//Нужно, чтобы не менялся цвет клетки в момент зажатия кнопки (по дефолту цвет был голубым, некрасиво)			 
					buttons[i, j].BackColor = colorMap[i, j];
				}
			}
			//Обводка вокруг шахматной доски
			for (int i = 0; i < 8; i++)
			{
				Label labelVL = new Label();
				Label labelHT = new Label();
				Label labelVR = new Label();
				Label labelHB = new Label();

				labelVR.Text = labelVL.Text = (8 - i).ToString();
				labelHB.Text = labelHT.Text = ((char)(65 + i)).ToString();

				labelVL.Location = new Point(0, (i * sideSize) + sideSize/2 + 10);
				labelVR.Location = new Point((8 * sideSize) + 21, (i * sideSize) + sideSize / 2 + 10);

				labelHT.Location = new Point((i * sideSize) + sideSize / 2 + 10, 1);				
				labelHB.Location = new Point((i * sideSize) + sideSize / 2 + 10, (8 * sideSize) + 21);

				labelVR.Font = labelHB.Font = labelVL.Font = labelHT.Font = new Font("Lucida Console", 14.0F, FontStyle.Regular);

				labelVR.ForeColor = labelHB.ForeColor = labelVL.ForeColor = labelHT.ForeColor = lightCell;

				labelVR.Size = labelHB.Size = labelVL.Size = labelHT.Size = new Size(18, 18);

				labelVR.BackColor = labelHB.BackColor = labelVL.BackColor = labelHT.BackColor = contour;

				Controls.Add(labelVL);
				Controls.Add(labelVR);
				Controls.Add(labelHT);
				Controls.Add(labelHB);
			}
			Label label = new Label();

			label.Location = new Point(0, 0);

			label.Size = new Size(sideSize * 8 + 40, sideSize * 8 + 40);

			label.BackColor = contour;

			Controls.Add(label);			
		}



		private void OnFigurePress(object sender, EventArgs e)//Ивент нажатия на фигуру
		{
			Button pressBttn = sender as Button;
			Chessman pressChess = chess[pressBttn.Location.Y / sideSize, pressBttn.Location.X / sideSize];

			if (pressBttn.BackColor == lightPushedCell || pressBttn.BackColor == darkPushedCell)//Нажатие на подсвечиваемую фигуру
			{				
				ResetBacklightChessboard();
				prevBttn = null;
				return;
			}


			if (pressBttn.Image == dot || pressBttn.Image == border)//Сделать возможный ход, подсвечивается точкой или обводкой фигуры
			{
				chess[pressBttn.Location.Y / sideSize, pressBttn.Location.X / sideSize] = chess[prevBttn.Location.Y / sideSize, prevBttn.Location.X / sideSize];
				chess[prevBttn.Location.Y / sideSize, prevBttn.Location.X / sideSize] = null;

				if(buttons[(pressBttn.Location.Y / sideSize) + 1, pressBttn.Location.X / sideSize].Image == x)
					chess[(pressBttn.Location.Y / sideSize) + 1, pressBttn.Location.X / sideSize] = null;
				else if(buttons[(pressBttn.Location.Y / sideSize) - 1, pressBttn.Location.X / sideSize].Image == x)
					chess[(pressBttn.Location.Y / sideSize) - 1, pressBttn.Location.X / sideSize] = null;

				//Механика сохранения
				StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{currentMatch.safeFile}.txt", FileMode.Append));
				sw.Write(" " + (prevBttn.Location.Y / sideSize).ToString() + (prevBttn.Location.X / sideSize).ToString() + "-");//Сохраниние результатов
				sw.Write((pressBttn.Location.Y / sideSize).ToString() + (pressBttn.Location.X / sideSize).ToString());//Сохраниние результатов
				sw.Close();

				//currentMatch.currentStep ++;
				currentMatch.roundW = !currentMatch.roundW;//Смена хода

				for(int i = 0; i < 8; i++)
				{
					for(int j = 0; j < 8; j++)
					{
						if(chess[i, j] != null)
							chess[i, j].posStepCalculated = false;//Обнуление маркера
					}
				}				

				ResetBacklightChessboard();
				prevBttn = null;//Обнуление предыдущей кнопки
				ShowChessboard();
				return;
			}
			else//Нажимаем на клетку без подсветки
			{				
				prevBttn = pressBttn;
				ResetBacklightChessboard();
			}
			

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

				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						if (pressChess?.posSteps[i, j] == 1)							
								buttons[i, j].Image = dot;
						else if (pressChess?.posSteps[i, j] == 2)
							buttons[i, j].Image = border;
						else if (pressChess?.posSteps[i, j] == 3)
							buttons[i, j].Image = x;
						buttons[i, j].ImageAlign = ContentAlignment.MiddleCenter;
					}
				}//Подсветка точками возможных ходов

				prevBttn = pressBttn;
			}
		}
		public void ResetBacklightChessboard()//Сброс подсветок на шахматной доске
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

		private void Chessboard_Load(object sender, EventArgs e)
		{			
		}

		private void Chessboard_Deactivate(object sender, EventArgs e)
		{			
		}
	}
}