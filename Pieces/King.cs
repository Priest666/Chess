using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Pieces
{
    internal class King : Piece
    {
        public override string Name => "King";

        public override string Symbol => Color == PieceColor.White ? "♔" : "♚";

        public override List<Point> GetValidMoves(Board board, int x, int y)
        {
            var moves = new List<Point>();

            int[][] directions = new int[][]
            {
                new[] { -1, -1 }, 
                new[] {  0, -1 },
                new[] {  1, -1 },
                new[] { -1,  0 },                 
                new[] {  1,  0 },
                new[] { -1,  1 }, 
                new[] {  0,  1 }, 
                new[] {  1,  1 }
            };

            foreach (var dir in directions)
            {
                int dx = dir[0], dy = dir[1];
                int nx = x + dx, ny = y + dy;

                if (!board.IsInBounds(nx, ny))
                    continue;

                var target = board.GetPieceAt(nx, ny);

                // Move is valid if the target square is empty or contains enemy
                if (target == null || target.Color != this.Color)
                {
                    moves.Add(new Point(nx, ny));
                }
            }

            return moves;
        }
    }
}
