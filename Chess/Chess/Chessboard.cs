using Microsoft.VisualBasic.Devices;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Xml;

namespace Chess
{
	public partial class Chessboard : Form
	{
		public Image dot = Image.FromFile($"Sprites\\1dot.png");
		public Image border = Image.FromFile($"Sprites\\1border.png");
		public int sideSize = 91;

		public Color lightCell = Color.FromArgb(255, 237, 252, 248);
		public Color darkCell = Color.FromArgb(255, 0, 87, 62);

		public Color? nocolor = null;

		public string[,] defaultMap = new string[8, 8]
		{
			{"bR","bH","bB","bK","bQ","bB","bH","bR"},
			{"bP","bP","bP","bP","bP","bP","bP","bP"},
			{"","","","","","","wK",""},
			{"","wR","","","","","bR",""},
			{"","","","bB","","","bH",""},
			{"","","","","","","",""},
			{"wP","wP","wP","wP","wP","wP","wP","wP"},
			{"wR","wH","wB","wK","wQ","wB","wH","wR"},
		};

		public Color[,] colorMap = new Color[8, 8];

		public Button? prevBttn;

		public Button[,] buttons = new Button[8, 8];//Массив с кнопками

		public Chessman[,] chess = new Chessman[8, 8];//Массив с фигурами

		public Chessboard()
		{
			InitializeComponent();

			Init();
		}

		public void Init()
		{
			CreateChessboard();
		}

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

		public void ShowChessboard()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					buttons[i, j].BackgroundImage = null;
				}
			}
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					buttons[i, j].BackgroundImage = chess[i, j]?.chessSprite;
					buttons[i, j].BackgroundImageLayout = ImageLayout.Center;
				}
			}
		}

		public void CreateChessboard()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					buttons[i, j] = new Button();//Массив кнопок

					Button button = new Button();
					button.Size = new Size(sideSize, sideSize);//Задаем размер кнопки
					button.FlatAppearance.BorderSize = 0;//Убираем рамку у кнопок
					button.FlatStyle = FlatStyle.Flat;//Делаем кнопку плоской, без скгруления
					button.Location = new Point(j * sideSize, i * sideSize);//Расставляем кнопки
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
						button.BackgroundImage = chess[i, j].chessSprite;
						button.BackgroundImageLayout = ImageLayout.Center;
					}

					CreateColorMap(lightCell, darkCell);

					buttons[i, j].FlatAppearance.MouseDownBackColor = buttons[i, j].BackColor = colorMap[i, j];					 
				}
			}
		}

		private void OnFigurePress(object sender, EventArgs e)
		{
			Button pressBttn = sender as Button;
			Chessman pressChess = chess[pressBttn.Location.Y / sideSize, pressBttn.Location.X / sideSize];

			if (pressBttn.BackColor == Color.LightGreen || pressBttn.BackColor == Color.MediumSeaGreen)
			{
				ResetColorChessboard();
				prevBttn = null;
				return;
			}

			if (pressBttn.Image == dot || pressBttn.Image == border)
			{
				chess[pressBttn.Location.Y / sideSize, pressBttn.Location.X / sideSize] = chess[prevBttn.Location.Y / sideSize, prevBttn.Location.X / sideSize];
				chess[prevBttn.Location.Y / sideSize, prevBttn.Location.X / sideSize] = null;

				ResetColorChessboard();
				prevBttn = null;
				ShowChessboard();
				return;
			}
			else
				prevBttn = pressBttn;

			ResetColorChessboard();

			if (pressChess != null)
			{
				pressChess.PossibleSteps(this);

				if(pressBttn.BackColor == lightCell)
					pressBttn.BackColor = Color.LightGreen;
				else 
					pressBttn.BackColor = Color.MediumSeaGreen;

				for (int i = 0; i < 8; i++)//Подсветка точками возможных ходов
				{
					for (int j = 0; j < 8; j++)
					{
						if (pressChess?.posSteps[i, j] == true)
						{					
							if (pressChess?.color != (chess[i, j]?.color ?? pressChess?.color))
							{
								buttons[i, j].Image = border;
								buttons[i, j].ImageAlign = ContentAlignment.MiddleCenter;
							}
							else
							{
								buttons[i, j].Image = dot;
								buttons[i, j].ImageAlign = ContentAlignment.MiddleCenter;
							}
						}
					}
				}
			}
		}
		public void ResetColorChessboard()
		{
			if (prevBttn != null)//Возврат цветов клеточек к базовому значению
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
	}
}