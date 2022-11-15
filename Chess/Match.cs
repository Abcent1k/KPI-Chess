
namespace Chess
{
	public partial class Match
	{
		public bool roundW;		
		public string safeFile = ("SF " + (DateTime.Now).ToString().Replace(":", "."));
		public int currentStep = 1;

		public Match(Chessboard cb)
		{			
			FileStream fs = new FileStream($"{safeFile}.txt", FileMode.Create);
			fs.Close();

			roundW = true;
			cb.Init();
		}

		


	}
}
