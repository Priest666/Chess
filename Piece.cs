using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public enum PieceColor
    {
        White,
        Black
    }

    internal abstract class Piece
    {
        public PieceColor Color { get; set; }

        public abstract string Name { get; }

        public abstract string Symbol { get; }

        // Returns a list of valid moves from (x, y) on a given board
        public abstract List<Point> GetValidMoves(Board board, int x, int y);

        public virtual Piece Clone()
        {
            return (Piece)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return Symbol;
        }
    }
}

