using BattleshipGame.Models;
using System.Drawing.Drawing2D;

namespace BattleshipGame
{
    public partial class GameSetupForm : Form
    {
        private const int GRID_SIZE = 10;
        private const int CELL_SIZE = 40;
        private const int GRID_OFFSET = 70; // Increased offset to accommodate coordinates
        
        private readonly Ship[] ships;
        private Ship? selectedShip;
        private Point? hoverCell;
        private int currentPlayer = 1;
        private readonly bool[,] player1Grid = new bool[GRID_SIZE, GRID_SIZE];
        private readonly bool[,] player2Grid = new bool[GRID_SIZE, GRID_SIZE];
        private readonly Dictionary<Point, Ship> placedShips = new();

        public GameSetupForm()
        {
            InitializeComponent();
            ships = new Ship[]
            {
                new Ship("Carrier", 5),
                new Ship("Battleship", 4),
                new Ship("Cruiser", 3),
                new Ship("Submarine", 3),
                new Ship("Destroyer", 2)
            };
            InitializeGame();
        }

        private void InitializeGame()
        {
            this.Text = $"Battleship - Player {currentPlayer} Setup";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create ship selection buttons
            CreateShipButtons();

            // Add rotation button
            Button rotateButton = new Button
            {
                Text = "Rotate Ship (R)",
                Size = new Size(120, 30),
                Location = new Point(GRID_OFFSET + (GRID_SIZE * CELL_SIZE) + 50, GRID_OFFSET + 250)
            };
            rotateButton.Click += (s, e) => RotateSelectedShip();
            this.Controls.Add(rotateButton);

            // Add instructions label
            Label instructionsLabel = new Label
            {
                Text = "Instructions:\n1. Select a ship from the right\n2. Press 'R' to rotate\n3. Click on the grid to place\nPress ESC to deselect",
                Location = new Point(GRID_OFFSET + (GRID_SIZE * CELL_SIZE) + 50, GRID_OFFSET + 300),
                Size = new Size(200, 100),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(instructionsLabel);

            // Set up mouse and keyboard events
            this.Paint += GameSetupForm_Paint;
            this.MouseMove += GameSetupForm_MouseMove;
            this.MouseClick += GameSetupForm_MouseClick;
            this.KeyPreview = true;
            this.KeyDown += GameSetupForm_KeyDown;
        }

        private void CreateShipButtons()
        {
            for (int i = 0; i < ships.Length; i++)
            {
                Button shipButton = new Button
                {
                    Text = $"{ships[i].Name} ({ships[i].Length})",
                    Tag = ships[i],
                    Size = new Size(120, 30),
                    Location = new Point(GRID_OFFSET + (GRID_SIZE * CELL_SIZE) + 50, GRID_OFFSET + (i * 40))
                };
                shipButton.Click += ShipButton_Click;
                this.Controls.Add(shipButton);
            }
        }

        private void ShipButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is Ship ship && !ship.IsPlaced)
            {
                selectedShip = ship;
            }
        }

        private void GameSetupForm_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw coordinate labels
            using (var font = new Font("Arial", 12))
            {
                // Draw column labels (A-J)
                for (int i = 0; i < GRID_SIZE; i++)
                {
                    string colLabel = ((char)('A' + i)).ToString();
                    var size = e.Graphics.MeasureString(colLabel, font);
                    e.Graphics.DrawString(colLabel, font, Brushes.Black,
                        GRID_OFFSET + (i * CELL_SIZE) + (CELL_SIZE - size.Width) / 2,
                        GRID_OFFSET - 25);
                }

                // Draw row labels (1-10)
                for (int i = 0; i < GRID_SIZE; i++)
                {
                    string rowLabel = (i + 1).ToString();
                    var size = e.Graphics.MeasureString(rowLabel, font);
                    e.Graphics.DrawString(rowLabel, font, Brushes.Black,
                        GRID_OFFSET - 25 - size.Width,
                        GRID_OFFSET + (i * CELL_SIZE) + (CELL_SIZE - size.Height) / 2);
                }
            }

            // Draw grid background
            using (var brush = new SolidBrush(Color.FromArgb(240, 240, 255)))
            {
                e.Graphics.FillRectangle(brush, 
                    GRID_OFFSET, GRID_OFFSET, 
                    GRID_SIZE * CELL_SIZE, GRID_SIZE * CELL_SIZE);
            }

            // Draw grid with thicker lines
            using (var pen = new Pen(Color.Navy, 1.5f))
            {
                for (int i = 0; i <= GRID_SIZE; i++)
                {
                    // Vertical lines
                    e.Graphics.DrawLine(pen,
                        GRID_OFFSET + (i * CELL_SIZE), GRID_OFFSET,
                        GRID_OFFSET + (i * CELL_SIZE), GRID_OFFSET + (GRID_SIZE * CELL_SIZE));

                    // Horizontal lines
                    e.Graphics.DrawLine(pen,
                        GRID_OFFSET, GRID_OFFSET + (i * CELL_SIZE),
                        GRID_OFFSET + (GRID_SIZE * CELL_SIZE), GRID_OFFSET + (i * CELL_SIZE));
                }
            }

            // Draw placed ships
            foreach (var shipPoint in placedShips)
            {
                DrawShipCell(e.Graphics, shipPoint.Key.X, shipPoint.Key.Y, Color.DarkSlateGray);
            }

            // Draw hover preview
            if (selectedShip != null && hoverCell.HasValue && !selectedShip.IsPlaced)
            {
                var previewCells = GetShipPreviewCells(hoverCell.Value.X, hoverCell.Value.Y);
                if (previewCells != null)
                {
                    bool isValidPlacement = previewCells.All(cell => IsValidPlacement(cell.Item1, cell.Item2));
                    Color previewColor = isValidPlacement ? Color.FromArgb(128, Color.LightGreen) : Color.FromArgb(128, Color.Red);
                    
                    foreach (var cell in previewCells)
                    {
                        DrawShipCell(e.Graphics, cell.Item1, cell.Item2, previewColor);
                    }
                }
            }
        }

        private void DrawShipCell(Graphics g, int gridX, int gridY, Color color)
        {
            var rect = new Rectangle(
                GRID_OFFSET + (gridX * CELL_SIZE) + 1,
                GRID_OFFSET + (gridY * CELL_SIZE) + 1,
                CELL_SIZE - 1,
                CELL_SIZE - 1);
            
            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }

            // Add a slight 3D effect
            using (var pen = new Pen(Color.FromArgb(color.R / 2, color.G / 2, color.B / 2)))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        private (int, int)[]? GetShipPreviewCells(int gridX, int gridY)
        {
            if (selectedShip == null) return null;

            var cells = new (int, int)[selectedShip.Length];
            for (int i = 0; i < selectedShip.Length; i++)
            {
                int x = selectedShip.IsHorizontal ? gridX + i : gridX;
                int y = selectedShip.IsHorizontal ? gridY : gridY + i;
                cells[i] = (x, y);
            }
            return cells;
        }

        private bool IsValidPlacement(int x, int y)
        {
            if (x < 0 || x >= GRID_SIZE || y < 0 || y >= GRID_SIZE)
                return false;

            var currentGrid = currentPlayer == 1 ? player1Grid : player2Grid;
            return !currentGrid[x, y];
        }

        private void GameSetupForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (selectedShip != null && !selectedShip.IsPlaced)
            {
                int gridX = (e.X - GRID_OFFSET) / CELL_SIZE;
                int gridY = (e.Y - GRID_OFFSET) / CELL_SIZE;

                if (gridX >= 0 && gridX < GRID_SIZE && gridY >= 0 && gridY < GRID_SIZE)
                {
                    hoverCell = new Point(gridX, gridY);
                }
                else
                {
                    hoverCell = null;
                }
                this.Invalidate();
            }
        }

        private void GameSetupForm_MouseClick(object? sender, MouseEventArgs e)
        {
            if (selectedShip == null || selectedShip.IsPlaced) return;

            int gridX = (e.X - GRID_OFFSET) / CELL_SIZE;
            int gridY = (e.Y - GRID_OFFSET) / CELL_SIZE;

            if (gridX >= 0 && gridX < GRID_SIZE && gridY >= 0 && gridY < GRID_SIZE)
            {
                var previewCells = GetShipPreviewCells(gridX, gridY);
                if (previewCells != null && previewCells.All(cell => IsValidPlacement(cell.Item1, cell.Item2)))
                {
                    // Place the ship
                    foreach (var cell in previewCells)
                    {
                        var currentGrid = currentPlayer == 1 ? player1Grid : player2Grid;
                        currentGrid[cell.Item1, cell.Item2] = true;
                        placedShips[new Point(cell.Item1, cell.Item2)] = selectedShip;
                    }

                    selectedShip.IsPlaced = true;
                    selectedShip.Coordinates = previewCells;
                    
                    // Update button state
                    foreach (Control control in this.Controls)
                    {
                        if (control is Button button && button.Tag == selectedShip)
                        {
                            button.Enabled = false;
                            break;
                        }
                    }

                    // Check if all ships are placed
                    if (ships.All(s => s.IsPlaced))
                    {
                        if (currentPlayer == 1)
                        {
                            // Switch to player 2
                            currentPlayer = 2;
                            ResetShipPlacement();
                            this.Text = $"Battleship - Player {currentPlayer} Setup";
                            MessageBox.Show("Player 1's ships have been placed. Player 2's turn to place ships.", "Next Player");
                        }
                        else
                        {
                            // Both players have placed their ships, start the game
                            MessageBox.Show("All ships placed! Ready to start the game.", "Setup Complete");
                            // TODO: Create and show the main game form
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }

                    selectedShip = null;
                    this.Invalidate();
                }
            }
        }

        private void ResetShipPlacement()
        {
            placedShips.Clear();
            foreach (var ship in ships)
            {
                ship.IsPlaced = false;
            }
            foreach (Control control in this.Controls)
            {
                if (control is Button button && button.Tag is Ship)
                {
                    button.Enabled = true;
                }
            }
        }

        private void GameSetupForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                RotateSelectedShip();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                selectedShip = null;
                this.Invalidate();
            }
        }

        private void RotateSelectedShip()
        {
            if (selectedShip != null && !selectedShip.IsPlaced)
            {
                selectedShip.Rotate();
                this.Invalidate();
            }
        }
    }
}
