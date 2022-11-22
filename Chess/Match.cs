
using System.IO;

namespace Chess
{
	public partial class Match
	{
		public bool roundW;//Чей ход
		public int currentStep = 1;// Номер хода

		public string savesPath = "Saves";//Путь к папке с сохранениями
		public string safeFile = ("SF " + (DateTime.Now).ToString().Replace(":", "."));//Файл сохранения	

		public bool WCastling;//Возможность совершить рокировку белым
		public bool BCastling;//Возможность совершить рокировку черным

		public List<Chessman> WKnockoutChess;//Белые выбитые фигуры
		public List<Chessman> BKnockoutChess;//Черные выбитые фигуры

		public Match(Chessboard cb)
		{
			Directory.CreateDirectory(savesPath);

			FileStream fs = new FileStream($"{savesPath}\\{safeFile}.txt", FileMode.Create);
			fs.Close();

			cb.ClientSize = new Size(cb.buttonSize * 12 + cb.frameSize * 4, cb.buttonSize * 8 + cb.frameSize * 2);			

			WKnockoutChess = new List<Chessman>();
			BKnockoutChess = new List<Chessman>();

			roundW = true;

			WCastling = true;
			BCastling= true;

			cb.Init();
		}

		


	}
}
