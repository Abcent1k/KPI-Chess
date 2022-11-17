
namespace Chess
{
	public partial class Match
	{
		public bool roundW;		
		public string safeFile = ("SF " + (DateTime.Now).ToString().Replace(":", "."));
		public int currentStep = 2;

		public bool WCastling;
		public bool BCastling;

		public List<Chessman> WKnockedOutChessman;
		public List<Chessman> BKnockedOutChessman;

		public Match(Chessboard cb)
		{			
			FileStream fs = new FileStream($"Saves\\{safeFile}.txt", FileMode.Create);
			fs.Close();

			WKnockedOutChessman = new List<Chessman>();
			BKnockedOutChessman = new List<Chessman>();

			roundW = true;

			WCastling = true;
			BCastling= true;

			cb.Init();
		}

		


	}
}
