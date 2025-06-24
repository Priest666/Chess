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

        public bool HasMoved { get; set; }

        public override string Name => "King";

        public override string Symbol => Color == PieceColor.White ? "♔" : "♚";

        public override List<Point> GetValidMoves(Board board, int x, int y)
        {
            var moves = new List<Point>();
            KingMoves(board, x, y, moves);

            if (!HasMoved)
            {
                AddCastlingMoves(board, x, y, moves);
            }

            return moves.Where(move =>
                !board.WouldBeInCheckAfterMove(Color, x, y, move.X, move.Y)).ToList();
        }


        private void KingMoves (Board board, int x, int y, List<Point> moves)
        {
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
        }

        private void AddCastlingMoves(Board board, int x, int y, List<Point> moves)
        {
            //King side
            CheckCastling(board, x, y, 7, moves);

            //Queen side
            CheckCastling(board, x, y, 0, moves);
        }

        private void CheckCastling(Board board, int kingX, int kingY, int rookX, List<Point> moves)
        {
            var rook = board.GetPieceAt(rookX, kingY) as Rook;

            if (rook == null || rook.HasMoved || rook.Color != Color)
            {
                return;
            }

            int step = rookX > kingX ? 1 : -1;

            for (int x = kingX + step; x != rookX; x += step)
            {
                if (board.GetPieceAt(x, kingY) != null)
                {
                    return;
                }
            }

            int castleX = kingX + 2 * (rookX > kingX ? 1 : -1);
            if (!board.WouldBeInCheckAfterMove(Color, kingX, kingY, castleX, kingY) &&
                !board.WouldBeInCheckAfterMove(Color, kingX, kingY, kingX + step, kingY))
            {
                moves.Add(new Point(castleX, kingY));   
            }
        }
    }
}
