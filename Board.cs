using Chess.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Board
    {
        // 8x8 grid: each square can hold a Piece or be empty (null)
        private Piece[,] squares = new Piece[8, 8];

        public int Rows => 8;
        public int Columns => 8;

        // Get the piece at a specific square
        public Piece GetPieceAt(int x, int y)
        {
            if (IsInBounds(x, y))
            {
                return squares[x, y];
            }
            return null;
        }

        // Set a piece on a specific square
        public void SetPieceAt(int x, int y, Piece piece)
        {
            if (IsInBounds(x, y))
            {
                squares[x, y] = piece;
            }
        }

        // Move a piece from one square to another
        public void MovePiece(int fromX, int fromY, int toX, int toY)
        {
            if (!IsInBounds(fromX, fromY) || !IsInBounds(toX, toY))
            {
                return;
            }

            Piece movingPiece = squares[fromX, fromY];
            squares[toX, toY] = movingPiece;
            squares[fromX, fromY] = null;
        }

        // Check if coordinates are within the bounds of the board
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Columns && y >= 0 && y < Rows;
        }

        // Set up the standard chess starting position
        public void InitializeStandardPosition()
        {
            // Place white pawns (row 6)
            for (int x = 0; x < 8; x++)
            {
                SetPieceAt(x, 6, new Pawn { Color = PieceColor.White });
            }

            // Place black pawns (row 1)
            for (int x = 0; x < 8; x++)
            {
                SetPieceAt(x, 1, new Pawn { Color = PieceColor.Black });
            }

            // Place white rooks (corners of row 7)
            SetPieceAt(0, 7, new Rook { Color = PieceColor.White });
            SetPieceAt(7, 7, new Rook { Color = PieceColor.White });

            // Place black rooks (corners of row 0)
            SetPieceAt(0, 0, new Rook{ Color = PieceColor.Black });
            SetPieceAt(7, 0, new Rook{ Color = PieceColor.Black });

            // Place white knights (row 7, columns 1 and 6)
            SetPieceAt(1, 7, new Knight{ Color = PieceColor.White });
            SetPieceAt(6, 7, new Knight { Color = PieceColor.White });

            // Place black knights (row 0, columns 1 and 6)
            SetPieceAt(1, 0, new Knight { Color = PieceColor.Black });
            SetPieceAt(6, 0, new Knight { Color = PieceColor.Black });

            SetPieceAt(2, 7, new Bishop { Color = PieceColor.White });
            SetPieceAt(5, 7, new Bishop { Color = PieceColor.White });

            SetPieceAt(2, 0, new Bishop { Color = PieceColor.Black });
            SetPieceAt(5, 0, new Bishop { Color = PieceColor.Black });


        }
    }
}
