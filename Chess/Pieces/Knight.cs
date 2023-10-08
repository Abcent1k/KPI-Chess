using System.Drawing;

namespace Chess.Pieces
{
	internal class Night : Chessman
	{

		private const int DIST_ONE_WAY = 1;
		private const int DIST_PERPENDICULAR = 2;

		public Night(Color color) : base(color)
		{
			CalculateMoves();
		}

		public override Chessman CalculateMoves()
		{
			availableMoves = new Point[8][] {
				new[] {new Point(DIST_ONE_WAY, DIST_PERPENDICULAR) },
				new[] {new Point(DIST_PERPENDICULAR, DIST_ONE_WAY) },
				new[] {new Point(-DIST_ONE_WAY, -DIST_PERPENDICULAR) },
				new[] {new Point(-DIST_PERPENDICULAR, -DIST_ONE_WAY) },
				new[] {new Point(-DIST_ONE_WAY, DIST_PERPENDICULAR) },
				new[] {new Point(-DIST_PERPENDICULAR, DIST_ONE_WAY) },
				new[] {new Point(DIST_ONE_WAY, -DIST_PERPENDICULAR) },
				new[] {new Point(DIST_PERPENDICULAR, -DIST_ONE_WAY) } 
			};
			availableAttacks = availableMoves;
			return this;
		}
	}

}
