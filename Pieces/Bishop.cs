using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Pieces
{
    internal class Bishop : Piece
    {
        public override string Name => "Bishop";

        public override string Symbol => Color == PieceColor.White ? "♗" : "♝";

        public override List<Point> GetValidMoves(Board board, int x, int y)
        {
            var moves = new List<Point>();


            return moves;
        }
    }
}
