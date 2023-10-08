using System.Drawing;

namespace Chess.Pieces
{

    internal class Bishop : Chessman
    {
		public Bishop(Color color) : base(color)
		{ 
			CalculateMoves();
		}
		

		public override Chessman CalculateMoves()
		{
			availableMoves = new Point[4][];
			availableMoves[0] = GetDiagnalMovementArray(MAX_DISTANCE, DiagnalDirection.FORWARD_LEFT);
			availableMoves[1] = GetDiagnalMovementArray(MAX_DISTANCE, DiagnalDirection.FORWARD_RIGHT);
			availableMoves[2] = GetDiagnalMovementArray(MAX_DISTANCE, DiagnalDirection.BACKWARD_LEFT);
			availableMoves[3] = GetDiagnalMovementArray(MAX_DISTANCE, DiagnalDirection.BACKWARD_RIGHT);
			availableAttacks = availableMoves;
			return this;
		}
	}

}

