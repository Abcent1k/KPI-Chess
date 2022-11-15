using System;

namespace Chess
{
	public partial class Chessboard : Form
	{
		public static int sideSize = 80;//������ ������ (������)
		public Bitmap dot = new Bitmap (new Bitmap($"Sprites\\1dot.png"),new Size(sideSize - 3, sideSize - 3));
		public Bitmap border = new Bitmap (new Bitmap($"Sprites\\1border.png"),new Size(sideSize - 3, sideSize - 3));		

		public Color lightCell = Color.FromArgb(255, 237, 252, 248);//������� ���� ��� ����� ������
		public Color darkCell = Color.FromArgb(255, 0, 87, 62);//������ ���� ��� ����� ������

		public Color lightPushedCell = Color.LightGreen;//���� ������� ������, ���� ������ ��� ������� �������
		public Color darkPushedCell = Color.MediumSeaGreen;//���� ������� ������, ���� ������ ��� ������� ������

		public Match currentMatch;

		//public DateTime localDate = DateTime.Now;
		//FileStream fs = new FileStream($"{localDate}.txt", FileMode.Create);

		public string[,] defaultMap = new string[8, 8]//����� ��������� ������� �����
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

		public Color[,] colorMap = new Color[8, 8];//����� ������ ��������� �����

		public Button? prevBttn;

		public Button[,] buttons = new Button[8, 8];//������ � ��������

		public Chessman[,] chess = new Chessman[8, 8];//������ � ��������

		public Chessboard()
		{
			InitializeComponent();
		}

		public void Init()
		{
			CreateChessboard();
		}

		public void CreateColorMap(Color light, Color dark)//�������� ����� ������ ��������� �����
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if ((i % 2 + j % 2) % 2 == 0)//������ ����� �������� �����
						colorMap[i, j] = light;
					else
						colorMap[i, j] = dark;
				}
			}
		}

		public void ShowChessboard()//����������� ��������� �����
		{			
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (chess[i, j] != null)
						chess[i, j].chessSprite = new Bitmap(chess[i, j].chessSprite, new Size(sideSize, sideSize));
					buttons[i, j].BackgroundImage = chess[i, j]?.chessSprite;					
					buttons[i, j].BackgroundImageLayout = ImageLayout.Center;
					
				}
			}
		}

		public void CreateChessboard()//�������� �������� �����
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					buttons[i, j] = new Button();//������ ������

					Button button = new Button();
					button.Size = new Size(sideSize, sideSize);//������ ������ ������
					button.FlatAppearance.BorderSize = 0;//��e���� ����� � ������
					button.FlatStyle = FlatStyle.Flat;//������ ������ �������, ��� ����������
					button.Location = new Point(j * sideSize, i * sideSize);//����������� ������
					button.Click += new EventHandler(OnFigurePress);//��������� �����

					Controls.Add(button);
					buttons[i, j] = button;

					if (defaultMap[i, j].Length != 0)//����������� ����� ������ �� �����, ��������� �������� ����� �� �������
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
							chess[i, j].chessSprite = new Bitmap(chess[i, j].chessSprite, new Size(sideSize, sideSize));
						button.BackgroundImage = chess[i, j].chessSprite;
						button.BackgroundImageLayout = ImageLayout.Center;
					}

					CreateColorMap(lightCell, darkCell);

					buttons[i, j].FlatAppearance.MouseDownBackColor = colorMap[i, j];//�����, ����� �� ������� ���� ������ � ������ ������� ������ (�� ������� ���� ��� �������, ���������)			 
					buttons[i, j].BackColor = colorMap[i, j];
				}
			}
		}



		private void OnFigurePress(object sender, EventArgs e)//����� ������� �� ������
		{
			Button pressBttn = sender as Button;
			Chessman pressChess = chess[pressBttn.Location.Y / sideSize, pressBttn.Location.X / sideSize];

			if (pressBttn.BackColor == lightPushedCell || pressBttn.BackColor == darkPushedCell)//������� �� �������������� ������
			{
				ResetBacklightChessboard();
				prevBttn = null;
				return;
			}


			if (pressBttn.Image == dot || pressBttn.Image == border)//������� ��������� ���, �������������� ������ ��� �������� ������
			{
				chess[pressBttn.Location.Y / sideSize, pressBttn.Location.X / sideSize] = chess[prevBttn.Location.Y / sideSize, prevBttn.Location.X / sideSize];
				chess[prevBttn.Location.Y / sideSize, prevBttn.Location.X / sideSize] = null;

				//save mechanic

				StreamWriter sw = new StreamWriter(new FileStream($"{currentMatch.safeFile}.txt", FileMode.Append));
				sw.Write((prevBttn.Location.Y / sideSize).ToString() + (prevBttn.Location.X / sideSize).ToString() + "-");//���������� �����������
				sw.Write((pressBttn.Location.Y / sideSize).ToString() + (pressBttn.Location.X / sideSize).ToString() + " ");//���������� �����������
				sw.Close();

				currentMatch.currentStep ++;
				currentMatch.roundW = !currentMatch.roundW;//����� ����

				chess[pressBttn.Location.Y / sideSize, pressBttn.Location.X / sideSize].posStepCalculated = false;

				ResetBacklightChessboard();
				prevBttn = null;//��������� ���������� ������
				ShowChessboard();
				return;
			}
			else//�������� �� ������ ��� ���������
			{
				prevBttn = pressBttn;
				ResetBacklightChessboard();
			}
			

			if (pressChess != null)
			{
				if ((currentMatch.roundW && pressChess.color == 'b') || (!currentMatch.roundW && pressChess.color == 'w'))//�������� �� ��, ��� ���
					return;

				if (pressChess.posStepCalculated == false)//������ ��������� ����� �������� ������
				{
					pressChess.PossibleSteps(this);
					pressChess.posStepCalculated = true;
				}

				if (pressBttn.BackColor == lightCell)//��������� �������� ������
					pressBttn.BackColor = lightPushedCell;
				else 
					pressBttn.BackColor = darkPushedCell;

				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						if (pressChess?.posSteps[i, j] == true)
						{
							if (pressChess?.color != (chess[i, j]?.color ?? pressChess?.color))
								buttons[i, j].Image = border;
							else
								buttons[i, j].Image = dot;

							buttons[i, j].ImageAlign = ContentAlignment.MiddleCenter;							
						}
					}
				}//��������� ������� ��������� �����

				prevBttn = pressBttn;
			}
		}
		public void ResetBacklightChessboard()//����� ��������� �� ��������� �����
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
			//sw.Close();
		}
	}
}