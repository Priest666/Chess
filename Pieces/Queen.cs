using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Pieces
{
    internal class Queen : Piece
    {
        public override string Name => "Queen";

        public override string Symbol => Color == PieceColor.White ? "♛" : "♛";

        public override List<Point> GetValidMoves(Board board, int x, int y)
        {
            var moves = new List<Point>();

            int[][] directions = new int[][]
            {
                new[] {1, 0}, 
                new[] {-1, 0}, 
                new[] {0, 1}, 
                new[] {0, -1},
                new[] { 1, 1 },
                new[] { 1, -1 },
                new[] { -1, -1 },
                new[] { -1, 1 }
            };

            foreach (var dir in directions)
            {
                int dx = dir[0], dy = dir[1];
                int nx = x + dx, ny = y + dy;

                while (board.IsInBounds(nx, ny))
                {
                    var target = board.GetPieceAt(nx, ny);

                    if (target == null)
                    {
                        moves.Add(new Point(nx, ny));
                    }
                    else
                    {
                        if (target.Color != this.Color)
                        {
                            moves.Add(new Point(nx, ny));
                        }

                        break;
                    }

                    nx += dx;
                    ny += dy;
                }
            }

            return moves;
        }
    }
}
