using System;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
	public partial class Chessboard : Form
	{
		public int buttonSize = 90;//������ ������ (������)	
		public int frameSize = 25;

		public Bitmap dot;
		public Bitmap circle;
		public Bitmap cross;
		public Bitmap arrowLeft;
		public Bitmap arrowRight;

		public Color lightCell = Color.FromArgb(255, 237, 252, 248);//������� ���� ��� ����� ������
		public Color darkCell = Color.FromArgb(255, 0, 87, 62);//������ ���� ��� ����� ������
		public Color frame = Color.FromArgb(255, 0, 43, 29);//���� ����� ������ ��������� �����

		public Color lightPushedCell = Color.LightGreen;//���� ������� ������, ���� ������ ��� ������� �������
		public Color darkPushedCell = Color.MediumSeaGreen;//���� ������� ������, ���� ������ ��� ������� ������

		public Match currentMatch;

		//��������� �������� �����
		private int oldWidth, oldHeight;
		private float formSidesProportion; // ����������� ������ �����


		public string[,] defaultMap = new string[8, 8]//����� ��������� ������� �����
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

		public Color[,] colorMap = new Color[8, 8];//����� ������ ��������� �����

		public Button? prevBttn;

		public Button[,] buttons = new Button[8, 8];//������ � ��������

		public Label labelWKnokout;
		public Label labelBKnokout;

		public Label[] WKnokoutlabels = new Label[16];
		public Label[] BKnokoutlabels = new Label[16];

		public Label labelFrame;

		public Label[] VLLabels = new Label[8];
		public Label[] VRLabels = new Label[8];
		public Label[] HTLabels = new Label[8];
		public Label[] HBLabels = new Label[8];

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
						chess[i, j].chessSprite = new Bitmap(chess[i, j].chessSprite, new Size(buttonSize - 10, buttonSize - 10));
					buttons[i, j].BackgroundImage = chess[i, j]?.chessSprite;					
					buttons[i, j].BackgroundImageLayout = ImageLayout.Center;					
				}
			}
		}

		//����� ��� �������� ��������� UI ��� ��������� �������� ����
		private void RedrawingUI(bool showElement)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					buttons[i, j].Visible = showElement;
					if ((i * 8 + j)<16)
					{
						WKnokoutlabels[i * 8 + j].Visible = showElement;
						BKnokoutlabels[i * 8 + j].Visible = showElement;
					}
				}
				VLLabels[i].Visible = VRLabels[i].Visible = HTLabels[i].Visible = HBLabels[i].Visible = showElement;
			}
		}

		public void CreateChessboard()//�������� �������� �����
		{

			dot = new Bitmap(new Bitmap($"Sprites\\dot.png"), new Size(buttonSize, buttonSize));
			circle = new Bitmap(new Bitmap($"Sprites\\circle.png"), new Size(buttonSize - 1, buttonSize - 1));
			cross = new Bitmap($"Sprites\\cross.png");
			arrowLeft = new Bitmap(new Bitmap($"Sprites\\arrowLeft.png"), new Size(buttonSize - 1, buttonSize - 1));
			arrowRight = new Bitmap(new Bitmap($"Sprites\\arrowRight.png"), new Size(buttonSize - 1, buttonSize - 1));

			formSidesProportion = ((float)(buttonSize * 12 + frameSize * 4) / (float)(buttonSize * 8 + frameSize * 2));

			CreateColorMap(lightCell, darkCell);

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					buttons[i, j] = new Button();//������ ������

					Button button = new Button();
					button.Size = new Size(buttonSize, buttonSize);//������ ������ ������
					button.FlatAppearance.BorderSize = 0;//��e���� ����� � ������
					button.FlatStyle = FlatStyle.Flat;//������ ������ �������, ��� ����������
					button.Location = new Point(frameSize + j * buttonSize, frameSize + i * buttonSize);//����������� ������
					button.BackColor = colorMap[i, j];
					button.FlatAppearance.MouseDownBackColor = colorMap[i, j];//�����, ����� �� ������� ���� ������ � ������ ������� ������ (�� ������� ���� ��� �������, ���������)

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
							chess[i, j].chessSprite = new Bitmap(chess[i, j].chessSprite, new Size(buttonSize - 10, buttonSize - 10));
						button.BackgroundImage = chess[i, j].chessSprite;
						button.BackgroundImageLayout = ImageLayout.Center;
					}
				}
			}

			//������� ������ ��������� �����
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

				labelVL.Location = new Point(0, (i * buttonSize) + buttonSize/2 + frameSize);
				labelVR.Location = new Point((8 * buttonSize) + frameSize, (i * buttonSize) + buttonSize / 2 + frameSize/2);

				labelHT.Location = new Point((i * buttonSize) + buttonSize / 2 + frameSize / 2, 0);				
				labelHB.Location = new Point((i * buttonSize) + buttonSize / 2 + frameSize / 2, (8 * buttonSize) + 2 * frameSize/2);

				labelVL.Font = labelVR.Font = labelHT.Font = labelHB.Font = new Font("Lucida Console", 12.0F, FontStyle.Regular);

				labelVL.ForeColor = labelVR.ForeColor = labelHT.ForeColor = labelHB.ForeColor = lightCell;

				labelVL.Size = labelVR.Size = labelHT.Size = labelHB.Size = new Size(frameSize, frameSize);

				labelVL.BackColor = labelVR.BackColor = labelHT.BackColor = labelHB.BackColor = frame;

				labelVL.TextAlign = labelVR.TextAlign = labelHT.TextAlign = labelHB.TextAlign = ContentAlignment.MiddleCenter;

				Controls.Add(labelVL);
				Controls.Add(labelVR);
				Controls.Add(labelHT);
				Controls.Add(labelHB);
			}

			//����� ������ ��������� �����
			labelFrame = new Label();

			labelFrame.Location = new Point(0, 0);

			labelFrame.Size = new Size(buttonSize * 8 + frameSize * 2, buttonSize * 8 + frameSize * 2);

			labelFrame.BackColor = frame;

			Controls.Add(labelFrame);

			//����� ��� ������������ ������� ������
			for (int i = 0; i < 16; i++)
			{
				Label labelKnokoutW = new Label();
				Label labelKnokoutB = new Label();

				labelKnokoutW.Location = new Point((8 * buttonSize) + frameSize * 3 + ((i % 4) * buttonSize), frameSize + (i / 4) * buttonSize);
				labelKnokoutB.Location = new Point((8 * buttonSize) + frameSize * 3 + ((i % 4) * buttonSize), (7 * buttonSize) + frameSize - (i / 4) * buttonSize);

				labelKnokoutB.Size = labelKnokoutW.Size = new Size(buttonSize, buttonSize);

				labelKnokoutW.BackColor = darkCell;
				labelKnokoutB.BackColor = lightCell;

				Controls.Add(labelKnokoutW);
				Controls.Add(labelKnokoutB);

				WKnokoutlabels[i] = labelKnokoutW;
				BKnokoutlabels[i] = labelKnokoutB;
			}
		}


		private void OnFigurePress(object sender, EventArgs e)//����� ������� �� ������
		{
			Button pressBttn = sender as Button;
			Chessman pressChess = chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize];

			//������� �� �������������� ������
			if (pressBttn.BackColor == lightPushedCell || pressBttn.BackColor == darkPushedCell)
			{
				ResetBacklightChessboard();
				prevBttn = null;
				return;
			}

			if (pressBttn.Image != null)//������� ��������� ���, ������� �������������� 
			{
				//�� ����������� �� ������� ��������
				if (pressBttn.Image == cross)
					return;

				//���������
				if (pressBttn.Image == arrowLeft || pressBttn.Image == arrowRight)
				{
					//��������� ����������� ���������
					if (chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize].color == 'b')
						currentMatch.BCastling = false;
					else
						currentMatch.WCastling = false;

					//�������� ����������
					StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{currentMatch.safeFile}.txt", FileMode.Append));

					if (currentMatch.roundW == true)
						sw.Write('\n' + currentMatch.currentStep.ToString() + ". ");

					sw.Write("0-0");
					//

					//�������� 0-0
					if (pressBttn.Image == arrowLeft)
					{
						if (chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize].type == 'R')
						{
							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize - 2] = chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize];
							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize] = null;

							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize - 1] = chess[pressBttn.Location.Y / buttonSize, 4];
							chess[pressBttn.Location.Y / buttonSize, 4] = null;
						}
						//������� 0-0-0
						else if (chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize].type == 'K')
						{
							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize - 2] = chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize];
							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize] = null;

							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize - 1] = chess[pressBttn.Location.Y / buttonSize, 0];
							chess[pressBttn.Location.Y / buttonSize, 0] = null;

							//�������� ����������
							sw.Write("-0");
							//
						}
					}
					else
					{
						//������� 0-0-0
						if (chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize].type == 'R')
						{
							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize + 3] = chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize];
							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize] = null;

							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize + 2] = chess[pressBttn.Location.Y / buttonSize, 4];
							chess[pressBttn.Location.Y / buttonSize, 4] = null;

							//�������� ����������
							sw.Write("-0");
							//
						}
						//�������� 0-0
						else if (chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize].type == 'K')
						{
							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize + 2] = chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize];
							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize] = null;

							chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize + 1] = chess[pressBttn.Location.Y / buttonSize, 7];
							chess[pressBttn.Location.Y / buttonSize, 7] = null;
						}
					}
					//�������� ����������
					sw.Write(" ");
					sw.Close();
					//
				}

				// En passant
				else if (pressBttn.Image == circle && ((pressBttn.Location.Y / buttonSize - 1 >= 0 && (buttons[pressBttn.Location.Y / buttonSize - 1, pressBttn.Location.X / buttonSize].Image == cross)) ||
					(pressBttn.Location.Y / buttonSize + 1 <= 7 && buttons[pressBttn.Location.Y / buttonSize + 1, pressBttn.Location.X / buttonSize].Image == cross)))
				{
					try
					{
						//�������� ����������
						StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{currentMatch.safeFile}.txt", FileMode.Append));

						if (currentMatch.roundW == true)
							sw.Write('\n' + currentMatch.currentStep.ToString() + ". ");

						sw.Write(chess[prevBttn.Location.Y / buttonSize, prevBttn.Location.X / buttonSize].type + ((char)(97 + prevBttn.Location.X / buttonSize)).ToString() +
							(8 - prevBttn.Location.Y / buttonSize).ToString());
						sw.Write("/" + ((char)(97 + pressBttn.Location.X / buttonSize)).ToString() + (8 - pressBttn.Location.Y / buttonSize).ToString() + " ");

						sw.Close();
						//

						if (buttons[(pressBttn.Location.Y / buttonSize) + 1, pressBttn.Location.X / buttonSize].Image == cross)
						{
							currentMatch.BKnockedOutChessman.Add(chess[(pressBttn.Location.Y / buttonSize) + 1, pressBttn.Location.X / buttonSize]);
							int iter = currentMatch.BKnockedOutChessman.Count() - 1;
							BKnokoutlabels[iter].Image = currentMatch.BKnockedOutChessman[iter].chessSprite;

							chess[(pressBttn.Location.Y / buttonSize) + 1, pressBttn.Location.X / buttonSize] = null;
						}
						else if (buttons[(pressBttn.Location.Y / buttonSize) - 1, pressBttn.Location.X / buttonSize].Image == cross)
						{
							currentMatch.WKnockedOutChessman.Add(chess[(pressBttn.Location.Y / buttonSize) - 1, pressBttn.Location.X / buttonSize]);
							int iter = currentMatch.WKnockedOutChessman.Count() - 1;
							WKnokoutlabels[iter].Image = currentMatch.WKnockedOutChessman[iter].chessSprite;

							chess[(pressBttn.Location.Y / buttonSize) - 1, pressBttn.Location.X / buttonSize] = null;
						}

						//������������ �����
						chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize] = chess[prevBttn.Location.Y / buttonSize, prevBttn.Location.X / buttonSize];
						chess[prevBttn.Location.Y / buttonSize, prevBttn.Location.X / buttonSize] = null;

					}
					catch (IndexOutOfRangeException) { }
				}

				//���������� ���� ��� ������
				else
				{
					//�������� ����������
					StreamWriter sw = new StreamWriter(new FileStream($"Saves\\{currentMatch.safeFile}.txt", FileMode.Append));

					if (currentMatch.roundW == true)
						sw.Write('\n' + currentMatch.currentStep.ToString() + ". ");

					sw.Write(chess[prevBttn.Location.Y / buttonSize, prevBttn.Location.X / buttonSize].type + ((char)(97 + prevBttn.Location.X / buttonSize)).ToString() +
						(8 - prevBttn.Location.Y / buttonSize).ToString());
					//

					if (pressBttn.Image == circle)
					{
						if (chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize]?.color == 'w')
						{
							currentMatch.WKnockedOutChessman.Add(chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize]);
							int iter = currentMatch.WKnockedOutChessman.Count() - 1;
							WKnokoutlabels[iter].Image = currentMatch.WKnockedOutChessman[iter].chessSprite;
						}
						else if (chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize]?.color == 'b')
						{ 
							currentMatch.BKnockedOutChessman.Add(chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize]);
							int iter = currentMatch.BKnockedOutChessman.Count() - 1;
							BKnokoutlabels[iter].Image = currentMatch.BKnockedOutChessman[iter].chessSprite;
						}
						//�������� ����������
						sw.Write(":");
						//
					}
					//�������� ����������
					else
						sw.Write("-");
					sw.Write(((char)(97 + pressBttn.Location.X / buttonSize)).ToString() + (8 - pressBttn.Location.Y / buttonSize).ToString() + " ");
					sw.Close();
					//

					//������������ �����
					chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize] = chess[prevBttn.Location.Y / buttonSize, prevBttn.Location.X / buttonSize];
					chess[prevBttn.Location.Y / buttonSize, prevBttn.Location.X / buttonSize] = null;
				}

				//����� ���� + ���� ����
				if(currentMatch.roundW == true)
					currentMatch.currentStep ++;
				currentMatch.roundW = !currentMatch.roundW;

				//��������� ����������� ���������
				try
				{
					if (chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize].type == 'K' ||
						chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize].type == 'R')
					{
						if (chess[pressBttn.Location.Y / buttonSize, pressBttn.Location.X / buttonSize].color == 'b')
							currentMatch.BCastling = false;
						else
							currentMatch.WCastling = false;
					}
				}
				catch (NullReferenceException) { }

				//��������� ��������
				for (int i = 0; i < 8; i++)
				{
					for(int j = 0; j < 8; j++)
					{
						if(chess[i, j] != null)
							chess[i, j].posStepCalculated = false;
					}
				}				

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
					pressChess.posStepCalculated = true;//��������� �������
				}

				if (pressBttn.BackColor == lightCell)//��������� �������� ������
					pressBttn.BackColor = lightPushedCell;
				else 
					pressBttn.BackColor = darkPushedCell;
				
				BacklightChessboard(pressChess);

				prevBttn = pressBttn;
			}
		}

		//��������� ��������� �����
		private void BacklightChessboard(Chessman chessman)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (chessman?.posSteps[i, j] == 1)
						buttons[i, j].Image = dot;
					else if (chessman?.posSteps[i, j] == 2)
						buttons[i, j].Image = circle;
					else if (chessman?.posSteps[i, j] == 3)
						buttons[i, j].Image = cross;
					else if (chessman?.posSteps[i, j] == 4)
						buttons[i, j].Image = arrowLeft;
					else if (chessman?.posSteps[i, j] == 5)
						buttons[i, j].Image = arrowRight;
					buttons[i, j].ImageAlign = ContentAlignment.MiddleCenter;
				}
			}
		}

		//����� ��������� �� ��������� �����
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

			// ���� ���������� ������ � �������� ����
			if (oldWidth != Width)
			{				
				ClientSize = new Size(width , (int)(width * 1f / formSidesProportion));
				height = Height - 47;
			}

			// ���� ���������� ������ � �������� ����
			if (oldHeight != Height)
			{
				ClientSize = new Size((int)(height * formSidesProportion) , height);				
				width = Width - 18;
			}

			buttonSize = (frameSize * -1 / 4) + (width - height - frameSize) / 4;//������ ������� ������ ��� ��������� ������� ����

			ClientSize = new Size(buttonSize * 12 + frameSize * 4, buttonSize * 8 + frameSize * 2);//������������ ������ ������ ����					

			labelFrame.Size = new Size(buttonSize * 8 + frameSize * 2, buttonSize * 8 + frameSize * 2);//������ �����

			dot = new Bitmap(dot, new Size(buttonSize, buttonSize));
			circle = new Bitmap(circle, new Size(buttonSize - 1, buttonSize - 1));
			cross = new Bitmap(cross, new Size(buttonSize, buttonSize));
			arrowLeft = new Bitmap(arrowLeft, new Size(buttonSize - 1, buttonSize - 1));
			arrowRight = new Bitmap(arrowRight, new Size(buttonSize - 1, buttonSize - 1));

			//�������� ���������� ����������
			RedrawingUI(false);

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{	
					buttons[i, j].Location = new Point(frameSize + j * buttonSize, frameSize + i * buttonSize);
					buttons[i, j].Size = new Size(buttonSize, buttonSize);

					if (buttons[i, j].BackgroundImage!= null)
					buttons[i, j].BackgroundImage = new Bitmap(buttons[i, j].BackgroundImage, new Size(buttonSize - 10, buttonSize - 10));
				}

				VLLabels[i].Location = new Point(0 , (i * buttonSize) + buttonSize / 2 + frameSize);
				VRLabels[i].Location = new Point((8 * buttonSize) + frameSize, (i * buttonSize) + buttonSize / 2 + frameSize / 2);

				HTLabels[i].Location = new Point((i * buttonSize) + buttonSize / 2 + frameSize / 2, 0);
				HBLabels[i].Location = new Point((i * buttonSize) + buttonSize / 2 + frameSize / 2, (8 * buttonSize) + 2 * frameSize / 2);
			}

			for(int i = 0; i < 16; i++)
			{
				WKnokoutlabels[i].Size = BKnokoutlabels[i].Size = new Size(buttonSize, buttonSize);

				WKnokoutlabels[i].Location = new Point((8 * buttonSize) + frameSize * 3 + ((i % 4) * buttonSize), frameSize + (i / 4) * buttonSize);
				BKnokoutlabels[i].Location = new Point((8 * buttonSize) + frameSize * 3 + ((i % 4) * buttonSize), (7 * buttonSize) + frameSize - (i / 4) * buttonSize);

				if (WKnokoutlabels[i].Image != null)
					WKnokoutlabels[i].Image = new Bitmap(WKnokoutlabels[i].Image, new Size(buttonSize - 10, buttonSize - 10));
				if (BKnokoutlabels[i].Image != null)
					BKnokoutlabels[i].Image = new Bitmap(BKnokoutlabels[i].Image, new Size(buttonSize - 10, buttonSize - 10));
			}

			//�������� ���������� ��������
			RedrawingUI(true);

		}

	}

}