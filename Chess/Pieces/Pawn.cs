using System.Drawing;

namespace Chess.Pieces
{
	internal class Pawn : Chessman, IDoubleStep, IEnPassant
	{
		public bool canDoubleStep { get; set; }
		public bool canEnPassantRight { get; set; }
		public bool canEnPassantLeft { get; set; }

		private const int DIST_DOUBLE_STEP = 2;
		private const int DIST_STEP = 1;

		public Pawn(Color color) : base(color) 
		{
			canDoubleStep = true;
			CalculateMoves();
		}

		public Pawn(Color color, bool CanDoubleStep) : base(color)
		{
			canDoubleStep = CanDoubleStep;
			CalculateMoves();
		}

		public override Chessman CalculateMoves()
		{
			Direction forward;
			DiagnalDirection forwardLeft, forwardRight;
			if (color == Color.BLACK)
			{
				forward = Direction.BACKWARD;
				forwardLeft = DiagnalDirection.BACKWARD_LEFT;
				forwardRight = DiagnalDirection.BACKWARD_RIGHT;
			}
			else
			{
				forward = Direction.FORWARD;
				forwardLeft = DiagnalDirection.FORWARD_LEFT;
				forwardRight = DiagnalDirection.FORWARD_RIGHT;
			}

			availableMoves = new Point[1][];
			if (canDoubleStep)
			{
				availableMoves[0] = GetMovementArray(DIST_DOUBLE_STEP, forward);
			}
			else
			{
				availableMoves[0] = GetMovementArray(1, forward);
			}
			availableAttacks = new Point[2][];
			availableAttacks[0] = GetDiagnalMovementArray(DIST_STEP, forwardLeft);
			availableAttacks[1] = GetDiagnalMovementArray(DIST_STEP, forwardRight);
			return this;
		}

	}
}

