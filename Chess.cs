using Chess.Pieces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
    public partial class Chess : Form
    {
        private const int TileSize = 64;
        private PictureBox[,] tileControls = new PictureBox[8, 8];
        private Board board = new Board();
        private Point? selectedSquare = null;
        private List<Point> highlightedMovesEmpty = new List<Point>();
        private List<Point> highlightedMovesCaptures = new List<Point>();
        private PieceColor currentTurn = PieceColor.White;
        private Label statusLabel = new Label();

        public Chess()
        {
            InitializeComponent();
            InitializeStatusLabel();
        }

        private void InitializeStatusLabel()
        {
            statusLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(10, 10);
            this.Controls.Add(statusLabel);
        }

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

        private void Chess_Load(object sender, EventArgs e)
        {
            board.InitializeStandardPosition();
            CreateBoardUI();
            DrawBoard();
            UpdateGameStatus();
        }

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
                        Tag = new Point(x, y)
                    };
                    tile.Click += Tile_Click;
                    tileControls[x, y] = tile;
                    this.Controls.Add(tile);
                }
            }
        }

        private void DrawBoard()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = board.GetPieceAt(x, y);
                    var tile = tileControls[x, y];
                    var pos = new Point(x, y);

                    Color baseColor = (x + y) % 2 == 0 ? Color.SandyBrown : Color.Brown;

                    if (selectedSquare.HasValue && selectedSquare.Value == pos)
                        tile.BackColor = Color.Gold;
                    else if (highlightedMovesCaptures.Contains(pos))
                        tile.BackColor = Color.Red;
                    else if (highlightedMovesEmpty.Contains(pos))
                        tile.BackColor = Color.Green;
                    else
                        tile.BackColor = baseColor;

                    // Highlight king if in check
                    if (piece is King && board.IsInCheck(piece.Color))
                    {
                        tile.BackColor = Color.Orange;
                    }

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

        private void Tile_Click(object sender, EventArgs e)
        {
            if (board.IsCheckmate(currentTurn) || board.IsStalemate(currentTurn))
                return;

            var tile = sender as PictureBox;
            var clickedPos = (Point)tile.Tag;
            var clickedPiece = board.GetPieceAt(clickedPos.X, clickedPos.Y);

            if (selectedSquare == null)
            {
                if (clickedPiece != null && clickedPiece.Color == currentTurn)
                {
                    selectedSquare = clickedPos;
                    UpdateHighlights(clickedPos, clickedPiece);
                }
            }
            else
            {
                Point from = selectedSquare.Value;
                Piece selectedPiece = board.GetPieceAt(from.X, from.Y);

                if (selectedPiece != null && selectedPiece.Color == currentTurn)
                {
                    var validMoves = selectedPiece.GetValidMoves(board, from.X, from.Y);

                    if (validMoves.Contains(clickedPos) && !board.WouldBeInCheckAfterMove(currentTurn, from.X, from.Y, clickedPos.X, clickedPos.Y))
                    {
                        board.MovePiece(from.X, from.Y, clickedPos.X, clickedPos.Y);
                        currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                    }
                }

                selectedSquare = null;
                highlightedMovesEmpty.Clear();
                highlightedMovesCaptures.Clear();
            }

            DrawBoard();
            UpdateGameStatus();
        }

        private void UpdateHighlights(Point position, Piece piece)
        {
            highlightedMovesEmpty.Clear();
            highlightedMovesCaptures.Clear();

            var allMoves = piece.GetValidMoves(board, position.X, position.Y);

            foreach (var move in allMoves)
            {
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