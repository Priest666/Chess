using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Pieces
{
    internal class Knight : Piece
    {
        public override string Name => "Knight";

        public override string Symbol => Color == PieceColor.White ? "♘" : "♞";

        public override List<Point> GetValidMoves(Board board, int x, int y)
        {
            var moves = new List<Point>();

            // All 8 possible L-shaped moves for a knight
            int[][] directions = new int[][]
            {
                new[] { 2, 1 }, new[] { 1, 2 },
                new[] { -1, 2 }, new[] { -2, 1 },
                new[] { -2, -1 }, new[] { -1, -2 },
                new[] { 1, -2 }, new[] { 2, -1 }
            };

            foreach (var dir in directions)
            {
                int newX = x + dir[0];
                int newY = y + dir[1];

                // Skip if out of bounds
                if (!board.IsInBounds(newX, newY))
                    continue;

                var target = board.GetPieceAt(newX, newY);

                // A knight can move to an empty square or capture an opponent's piece
                if (target == null || target.Color != this.Color)
                {
                    moves.Add(new Point(newX, newY));
                }
            }

            return moves;
        }
    }   
}
