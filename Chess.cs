using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
    public partial class Chess : Form
    {
        private const int TileSize = 64; // Size of each tile on the board
        private PictureBox[,] tileControls = new PictureBox[8, 8]; // 8x8 grid of UI tiles
        private Board board = new Board(); 
        private Point? selectedSquare = null; // The currently selected piece (if any)
        private List<Point> highlightedMovesEmpty = new List<Point>(); // Valid non-capture moves
        private List<Point> highlightedMovesCaptures = new List<Point>(); // Valid capture moves
        private PieceColor currentTurn = PieceColor.White; 

        public Chess()
        {
            InitializeComponent();
        }

        private void Chess_Load(object sender, EventArgs e)
        {
            board.InitializeStandardPosition(); 
            CreateBoardUI(); 
            DrawBoard(); 
        }

        // Create the 8x8 grid of PictureBoxes and place them centered on the form
        private void CreateBoardUI()
        {
            int boardPixelSize = 8 * TileSize;

            // Calculate offset to center the board
            int offsetX = (this.ClientSize.Width - boardPixelSize) / 2;
            int offsetY = (this.ClientSize.Height - boardPixelSize) / 2;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var tile = new PictureBox
                    {
                        Width = TileSize,
                        Height = TileSize,
                        Location = new Point(offsetX + x * TileSize, offsetY + y * TileSize),
                        BorderStyle = BorderStyle.FixedSingle,
                        Tag = new Point(x, y) // Store board coordinates in the Tag
                    };

                    tile.Click += Tile_Click; // Wire up click handler
                    tileControls[x, y] = tile;
                    this.Controls.Add(tile);
                }
            }
        }

        // Draw the entire board and pieces
        private void DrawBoard()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = board.GetPieceAt(x, y); // Get piece at this position
                    var tile = tileControls[x, y];
                    var pos = new Point(x, y);

                    // Base color of the tile
                    Color baseColor = (x + y) % 2 == 0 ? Color.SandyBrown : Color.Brown;

                    // Highlighting logic
                    if (selectedSquare.HasValue && selectedSquare.Value == pos)
                        tile.BackColor = Color.Gold; // Selected piece
                    else if (highlightedMovesCaptures.Contains(pos))
                        tile.BackColor = Color.Red; // Valid capture
                    else if (highlightedMovesEmpty.Contains(pos))
                        tile.BackColor = Color.Green; // Valid non-capture move
                    else
                        tile.BackColor = baseColor; // Default color

                    // Draw the piece symbol
                    Bitmap bmp = new Bitmap(TileSize, TileSize);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(tile.BackColor);

                        if (piece != null)
                        {
                            var sf = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };

                            using (Font font = new Font("Segoe UI Symbol", 28))
                            using (Brush brush = new SolidBrush(piece.Color == PieceColor.White ? Color.White : Color.Black))
                            {
                                g.DrawString(piece.Symbol, font, brush, new RectangleF(0, 0, TileSize, TileSize), sf);
                            }
                        }
                    }

                    tile.Image = bmp;
                }
            }
        }

        // Handle when a tile is clicked
        private void Tile_Click(object sender, EventArgs e)
        {
            var tile = sender as PictureBox;
            var clickedPos = (Point)tile.Tag;
            var clickedPiece = board.GetPieceAt(clickedPos.X, clickedPos.Y);

            // First click: select a piece
            if (selectedSquare == null)
            {
                if (clickedPiece != null && clickedPiece.Color == currentTurn)
                {
                    selectedSquare = clickedPos;

                    // Get possible moves for this piece
                    var allMoves = clickedPiece.GetValidMoves(board, clickedPos.X, clickedPos.Y);
                    highlightedMovesEmpty.Clear();
                    highlightedMovesCaptures.Clear();

                    foreach (var move in allMoves)
                    {
                        var targetPiece = board.GetPieceAt(move.X, move.Y);
                        if (targetPiece == null)
                            highlightedMovesEmpty.Add(move); // Empty move
                        else if (targetPiece.Color != clickedPiece.Color)
                            highlightedMovesCaptures.Add(move); // Capture
                    }
                }
            }
            // Second click: attempt to move
            else
            {
                Point from = selectedSquare.Value;
                Piece selectedPiece = board.GetPieceAt(from.X, from.Y);

                if (selectedPiece != null && selectedPiece.Color == currentTurn)
                {
                    var validMoves = selectedPiece.GetValidMoves(board, from.X, from.Y);

                    if (validMoves.Contains(clickedPos))
                    {
                        // Move piece and switch turn
                        board.MovePiece(from.X, from.Y, clickedPos.X, clickedPos.Y);
                        currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                    }
                }

                // Clear selection and highlights
                selectedSquare = null;
                highlightedMovesEmpty.Clear();
                highlightedMovesCaptures.Clear();
            }

            // Redraw the board with updated state
            DrawBoard();
        }
    }
}
