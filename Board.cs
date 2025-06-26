using Chess.Pieces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Board
    {
        // 8x8 grid: each square can hold a Piece or be empty (null)
        private Piece[,] squares = new Piece[8, 8];

        // Properties for board dimensions
        public int Rows => 8;
        public int Columns => 8;

        //Gets the piece at the specified coordinates.   
        public Piece GetPieceAt(int x, int y)
        {
            if (IsInBounds(x, y))
            {
                return squares[x, y];
            }
            return null;
        }

        //Places a piece at the specified coordinates.
        public void SetPieceAt(int x, int y, Piece piece)
        {
            if (IsInBounds(x, y))
            {
                squares[x, y] = piece;
            }
        }
    
        //Moves a piece from one square to another.    
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

        //Checks if the given coordinates are within the bounds of the board.    
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Columns && y >= 0 && y < Rows;
        }

        //Finds the position of the king of the specified color.    
        public Point FindKing(PieceColor color)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = GetPieceAt(x, y);
                    if (piece is King && piece.Color == color)
                    {
                        return new Point(x, y);
                    }
                }
            }
            throw new Exception("King not found");
        }

        //Gets all positions containing pieces of the specified color.

        public List<Point> GetAllPiecesOfColor(PieceColor color)
        {
            var list = new List<Point>();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = GetPieceAt(x, y);
                    if (piece != null && piece.Color == color)
                    {
                        list.Add(new Point(x, y));
                    }
                }
            }

            return list;
        }

        //Simulates a move and checks if it would leave the king in check.
        public bool WouldBeInCheckAfterMove(PieceColor color, int fromX, int fromY, int toX, int toY)
        {
            // Temporarily make the move
            Piece originalPiece = GetPieceAt(fromX, fromY);
            Piece targetPiece = GetPieceAt(toX, toY);

            SetPieceAt(toX, toY, originalPiece);
            SetPieceAt(fromX, fromY, null);

            bool isInCheck = false;
            isInCheck = IsInCheck(color);

            // Undo the temporary move
            SetPieceAt(fromX, fromY, originalPiece);
            SetPieceAt(toX, toY, targetPiece);

            return isInCheck;
        }

        // Checks if the king of the specified color is in check.
        public bool IsInCheck(PieceColor color)
        {
            Point kingPos = FindKing(color);
            var opponentColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;

            foreach (var pos in GetAllPiecesOfColor(opponentColor))
            {
                var piece = GetPieceAt(pos.X, pos.Y);

                // Special handling for king to avoid recursion
                if (piece is King)
                {
                    // Kings can only attack adjacent squares
                    if (Math.Abs(pos.X - kingPos.X) <= 1 && Math.Abs(pos.Y - kingPos.Y) <= 1)
                        return true;
                }
                else
                {
                    var moves = piece.GetValidMoves(this, pos.X, pos.Y);
                    if (moves.Contains(kingPos))
                        return true;
                }
            }
            return false;
        }

        //Checks if the specified color is in checkmate.
        public bool IsCheckmate(PieceColor color)
        {
            if (!IsInCheck(color)) return false;

            // Check if any legal move exists that would get the king out of check
            foreach (var pos in GetAllPiecesOfColor(color))
            {
                var piece = GetPieceAt(pos.X, pos.Y);
                foreach (var move in piece.GetValidMoves(this, pos.X, pos.Y))
                {
                    if (!WouldBeInCheckAfterMove(color, pos.X, pos.Y, move.X, move.Y))
                        return false;
                }
            }
            return true;
        }

        //Checks if the specified color is in stalemate.   
        public bool IsStalemate(PieceColor color)
        {
            if (IsInCheck(color)) return false;

            // Check if any legal move exists
            foreach (var pos in GetAllPiecesOfColor(color))
            {
                var piece = GetPieceAt(pos.X, pos.Y);
                if (piece.GetValidMoves(this, pos.X, pos.Y).Any(move =>
                    !WouldBeInCheckAfterMove(color, pos.X, pos.Y, move.X, move.Y)))
                    return false;
            }
            return true;
        }

        //Sets up the standard chess starting position.
        public void InitializeStandardPosition()
        {
            for (int x = 0; x < 8; x++)
            {
                SetPieceAt(x, 6, new Pawn { Color = PieceColor.White });
                SetPieceAt(x, 1, new Pawn { Color = PieceColor.Black });
            }

            // Place white rooks (corners of row 7)
            SetPieceAt(0, 7, new Rook { Color = PieceColor.White });
            SetPieceAt(7, 7, new Rook { Color = PieceColor.White });

            // Place black rooks (corners of row 0)
            SetPieceAt(0, 0, new Rook { Color = PieceColor.Black });
            SetPieceAt(7, 0, new Rook { Color = PieceColor.Black });

            // Place white knights (row 7, columns 1 and 6)
            SetPieceAt(1, 7, new Knight { Color = PieceColor.White });
            SetPieceAt(6, 7, new Knight { Color = PieceColor.White });

            // Place black knights (row 0, columns 1 and 6)
            SetPieceAt(1, 0, new Knight { Color = PieceColor.Black });
            SetPieceAt(6, 0, new Knight { Color = PieceColor.Black });

            // Place white bishops (row 7, columns 2 and 5)
            SetPieceAt(2, 7, new Bishop { Color = PieceColor.White });
            SetPieceAt(5, 7, new Bishop { Color = PieceColor.White });

            // Place black bishops (row 0, columns 2 and 5)
            SetPieceAt(2, 0, new Bishop { Color = PieceColor.Black });
            SetPieceAt(5, 0, new Bishop { Color = PieceColor.Black });

            // Place queens (row 7 column 3 for white, row 0 column 3 for black)
            SetPieceAt(3, 7, new Queen { Color = PieceColor.White });
            SetPieceAt(3, 0, new Queen { Color = PieceColor.Black });

            // Place kings (row 7 column 4 for white, row 0 column 4 for black)
            SetPieceAt(4, 7, new King { Color = PieceColor.White });
            SetPieceAt(4, 0, new King { Color = PieceColor.Black });
        }
    }
}