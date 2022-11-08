
namespace Chess
{
	public partial class Match
	{		
		public bool roundW;

		public Match(Chessboard cb)
		{
			roundW = true;	
			cb.Init();
		}
	
		
	}
}
