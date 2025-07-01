using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Pieces
{
    internal class Pawn : Piece
    {
        public override string Name => "Pawn";

        public override string Symbol => Color == PieceColor.White ? "♟" : "♟";

        public override List<Point> GetValidMoves(Board board, int x, int y)
        {
            var moves = new List<Point>();

            int direction = Color == PieceColor.White ? -1 : 1; // white moves up, black down
            int startRow = Color == PieceColor.White ? 6 : 1;

            int oneStepY = y + direction;
            int twoStepY = y + 2 * direction;

            // Move forward one step if the square is empty
            if (board.IsInBounds(x, oneStepY) && board.GetPieceAt(x, oneStepY) == null)
            {
                moves.Add(new Point(x, oneStepY));

                // Move two steps if on starting row and both squares are empty
                if (y == startRow && board.GetPieceAt(x, twoStepY) == null)
                {
                    moves.Add(new Point(x, twoStepY));
                }
            }

            // Diagonal captures (left and right)
            foreach (int dx in new[] {-1, 1})
            {
                int newX = x + dx;
                if (board.IsInBounds(newX, oneStepY))
                {
                    Piece target = board.GetPieceAt(newX, oneStepY);
                    if (target != null && target.Color != this.Color)
                    {
                        moves.Add(new Point(newX, oneStepY));
                    } 
                }
            }

            return moves;
        }
    }
}
