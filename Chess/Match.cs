namespace Chess
{
	public partial class Match : ICloneable
	{
		public bool witeTurn;//Чей ход	
		public int currentStep = 1;//Номер хода


		public bool WRightCastling;//Возможность совершить рокировку белым
		public bool WLeftCastling;
		public bool BRightCastling;//Возможность совершить рокировку черным
		public bool BLeftCastling;


		public bool WShah;
		public bool BShah;

		public List<Chessman> WKnockoutChess;//Белые выбитые фигуры
		public List<Chessman> BKnockoutChess;//Черные выбитые фигуры

		public Match(Chessboard cb)
		{
			cb.ClientSize = new Size(cb.cellSize * 8 + cb.labelSize * 4 + cb.frameSize * 4, cb.cellSize * 8 + cb.frameSize * 2);			

			WKnockoutChess = new List<Chessman>();
			BKnockoutChess = new List<Chessman>();

			witeTurn = true;

			WRightCastling = true;
			WLeftCastling = true;
			BRightCastling = true;
			BLeftCastling= true;

			WShah = false;
			BShah = false;

			cb.Init();		
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
