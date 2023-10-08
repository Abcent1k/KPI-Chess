using System.Drawing;

namespace Chess.Pieces
{ 
    internal class Rook : Chessman, ICastlePiece
	{
		public bool canCastle { get; set; }

		public Rook(Color color) : base(color) 
		{
			canCastle = true;
			CalculateMoves();
		}

		public Rook(Color color, bool CanCastle) : base(color)
		{
			canCastle = CanCastle;
			CalculateMoves();
		}


		public override Chessman CalculateMoves()
		{
			availableMoves = new Point[4][];
			availableMoves[0] = GetMovementArray(MAX_DISTANCE, Direction.FORWARD);
			availableMoves[1] = GetMovementArray(MAX_DISTANCE, Direction.BACKWARD);
			availableMoves[2] = GetMovementArray(MAX_DISTANCE, Direction.LEFT);
			availableMoves[3] = GetMovementArray(MAX_DISTANCE, Direction.RIGHT);
			availableAttacks = availableMoves;
			return this;
		}

	}

    
}

