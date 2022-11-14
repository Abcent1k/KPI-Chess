
namespace Chess
{
	public partial class Match
	{
		public bool roundW;
		public string path = "save.txt";

		public Match(Chessboard cb)
		{
			roundW = true;
			cb.Init();
		}

		


	}
}
