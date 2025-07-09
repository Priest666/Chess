using Chess.Pieces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
    public partial class Chess : Form
    {
        private const int TileSize = 64; // Size of each square on the board
        private readonly PictureBox[,] tileControls = new PictureBox[8, 8]; // UI elements for board tiles
        private readonly Board board = new Board(); // Logical board representation
        private Point? selectedSquare = null; // Currently selected square (if any)
        private readonly List<Point> highlightedMovesEmpty = new List<Point>(); // Valid non-capture moves
        private readonly List<Point> highlightedMovesCaptures = new List<Point>(); // Valid capture moves
        private PieceColor currentTurn = PieceColor.White; // Tracks whose turn it is
        private readonly Label statusLabel = new Label(); // Label to display game status (e.g., check, checkmate)

        public Chess()
        {
            InitializeComponent();
            InitializeStatusLabel();
        }

        // Sets up the label that displays the current game status
        private void InitializeStatusLabel()
        {
            statusLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(10, 10);
            this.Controls.Add(statusLabel);
        }

        // Updates the status label depending on the current board state
        private void UpdateGameStatus()
        {
            if (board.IsCheckmate(currentTurn))
            {
                statusLabel.Text = $"CHECKMATE! {currentTurn} loses";
                statusLabel.ForeColor = Color.Red;
            }
            else if (board.IsStalemate(currentTurn))
            {
                statusLabel.Text = "STALEMATE! Game drawn";
                statusLabel.ForeColor = Color.Blue;
            }
            else if (board.IsInCheck(currentTurn))
            {
                statusLabel.Text = $"{currentTurn} is in CHECK";
                statusLabel.ForeColor = Color.OrangeRed;
            }
            else
            {
                statusLabel.Text = $"{currentTurn}'s turn";
                statusLabel.ForeColor = Color.Black;
            }
        }

        // Event triggered when the form loads
        private void Chess_Load(object sender, EventArgs e)
        {
            board.InitializeStandardPosition(); // Place pieces in starting positions
            CreateBoardUI(); // Generate tile UI
            DrawBoard(); // Draw initial board
            UpdateGameStatus(); // Show initial game state
        }

        // Creates the 8x8 chessboard UI using PictureBoxes
        private void CreateBoardUI()
        {
            int boardPixelSize = 8 * TileSize;
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
                        Tag = new Point(x, y) // Store position for easy access
                    };
                    tile.Click += Tile_Click;
                    tileControls[x, y] = tile;
                    this.Controls.Add(tile);
                }
            }
        }

        // Renders the board state visually
        private void DrawBoard()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = board.GetPieceAt(x, y);
                    var tile = tileControls[x, y];
                    var pos = new Point(x, y);

                    // Set base tile color
                    Color baseColor = (x + y) % 2 == 0 ? Color.SandyBrown : Color.Brown;

                    // Override color for highlights
                    if (selectedSquare.HasValue && selectedSquare.Value == pos)
                        tile.BackColor = Color.Gold; // Selected tile
                    else if (highlightedMovesCaptures.Contains(pos))
                        tile.BackColor = Color.Red; // Valid capture move
                    else if (highlightedMovesEmpty.Contains(pos))
                        tile.BackColor = Color.Green; // Valid empty move
                    else
                        tile.BackColor = baseColor;

                    // Highlight the king if in check
                    if (piece is King && board.IsInCheck(piece.Color))
                    {
                        tile.BackColor = Color.Orange;
                    }

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

        // Handles tile click events for selecting/moving pieces
        private void Tile_Click(object sender, EventArgs e)
        {
            // If game is over, ignore clicks
            if (board.IsCheckmate(currentTurn) || board.IsStalemate(currentTurn))
                return;

            var tile = sender as PictureBox;
            var clickedPos = (Point)tile.Tag;
            var clickedPiece = board.GetPieceAt(clickedPos.X, clickedPos.Y);

            if (selectedSquare == null)
            {
                // Select piece if it's the player's own
                if (clickedPiece != null && clickedPiece.Color == currentTurn)
                {
                    selectedSquare = clickedPos;
                    UpdateHighlights(clickedPos, clickedPiece);
                }
            }
            else
            {
                // Try to move selected piece to new position
                Point from = selectedSquare.Value;
                Piece selectedPiece = board.GetPieceAt(from.X, from.Y);

                if (selectedPiece != null && selectedPiece.Color == currentTurn)
                {
                    var validMoves = selectedPiece.GetValidMoves(board, from.X, from.Y);

                    // Check if clicked target is valid and doesn’t result in self-check
                    if (validMoves.Contains(clickedPos) &&
                        !board.WouldBeInCheckAfterMove(currentTurn, from.X, from.Y, clickedPos.X, clickedPos.Y))
                    {
                        board.MovePiece(from.X, from.Y, clickedPos.X, clickedPos.Y);
                        currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                    }
                }

                // Clear selection and highlights
                selectedSquare = null;
                highlightedMovesEmpty.Clear();
                highlightedMovesCaptures.Clear();
            }

            DrawBoard(); // Refresh the board
            UpdateGameStatus(); // Refresh status label
        }

        // Highlights all valid moves for the selected piece
        private void UpdateHighlights(Point position, Piece piece)
        {
            highlightedMovesEmpty.Clear();
            highlightedMovesCaptures.Clear();

            var allMoves = piece.GetValidMoves(board, position.X, position.Y);

            foreach (var move in allMoves)
            {
                // Only highlight moves that don’t leave the king in check
                if (!board.WouldBeInCheckAfterMove(currentTurn, position.X, position.Y, move.X, move.Y))
                {
                    var targetPiece = board.GetPieceAt(move.X, move.Y);
                    if (targetPiece == null)
                        highlightedMovesEmpty.Add(move);
                    else if (targetPiece.Color != piece.Color)
                        highlightedMovesCaptures.Add(move);
                }
            }
        }
    }
}
