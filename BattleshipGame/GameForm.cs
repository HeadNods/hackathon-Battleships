using BattleshipGame.Models;
using System.Drawing.Drawing2D;

namespace BattleshipGame
{    public partial class GameForm : Form
    {
        private const int GRID_SIZE = 10;
        private const int CELL_SIZE = 40;
        private const int GRID_OFFSET = 70;
        private const int GRID_SPACING = 100; // Space between the two grids

        private readonly Font coordinateFont = new Font("Arial", 12, FontStyle.Bold);
        private readonly Font shipFont = new Font("Arial", 8, FontStyle.Bold);
        private readonly StringFormat centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        private readonly Ship[] player1Ships;
        private readonly Ship[] player2Ships;
        private readonly bool[,] player1Grid;
        private readonly bool[,] player2Grid;
        private readonly bool[,] player1Shots = new bool[GRID_SIZE, GRID_SIZE];
        private readonly bool[,] player2Shots = new bool[GRID_SIZE, GRID_SIZE];
        private int currentPlayer = 1;
        private Point? hoverCell;
        private bool gameOver;

        public GameForm(Ship[] player1Ships, Ship[] player2Ships, bool[,] player1Grid, bool[,] player2Grid)
        {
            InitializeComponent();
            this.player1Ships = player1Ships;
            this.player2Ships = player2Ships;
            this.player1Grid = player1Grid;
            this.player2Grid = player2Grid;
            InitializeGame();
        }

        private void InitializeGame()
        {
            this.Text = $"Battleship - Player {currentPlayer}'s Turn";
            this.Size = new Size(1200, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Add player labels
            Label player1Label = new Label
            {
                Text = "Your Grid",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(GRID_OFFSET, 20),
                Size = new Size(GRID_SIZE * CELL_SIZE, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(player1Label);            Label player2Label = new Label
            {
                Text = "Opponent's Grid",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(GRID_OFFSET + GRID_SIZE * CELL_SIZE + GRID_SPACING, 20),
                Size = new Size(GRID_SIZE * CELL_SIZE, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(player2Label);            // Add New Game button
            Button newGameButton = new Button
            {
                Text = "New Game (N)",
                Size = new Size(120, 30),
                Location = new Point(2 * GRID_OFFSET + 2 * GRID_SIZE * CELL_SIZE + GRID_SPACING, 20)
            };
            newGameButton.Click += (s, e) => StartNewGame();
            this.Controls.Add(newGameButton);

            // Add turn indicator and ship status panel
            Panel statusPanel = new Panel
            {
                Location = new Point(2 * GRID_OFFSET + 2 * GRID_SIZE * CELL_SIZE + GRID_SPACING, GRID_OFFSET),
                Size = new Size(200, 400),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(statusPanel);            // Set up event handlers
            this.Paint += GameForm_Paint;
            this.MouseMove += GameForm_MouseMove;
            this.MouseClick += GameForm_MouseClick;
            this.KeyPreview = true;
            UpdateStatusPanel();
        }

        private void GameForm_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw both grids
            DrawGrid(e.Graphics, GRID_OFFSET, true); // Player's grid
            DrawGrid(e.Graphics, GRID_OFFSET + GRID_SIZE * CELL_SIZE + GRID_SPACING, false); // Opponent's grid

            // Draw hover effect on opponent's grid
            if (hoverCell.HasValue && !gameOver)
            {
                var targetGrid = currentPlayer == 1 ? player2Grid : player1Grid;
                var shotsGrid = currentPlayer == 1 ? player1Shots : player2Shots;
                
                int gridX = hoverCell.Value.X;
                int gridY = hoverCell.Value.Y;
                
                if (!shotsGrid[gridX, gridY])
                {
                    int xOffset = GRID_OFFSET + GRID_SIZE * CELL_SIZE + GRID_SPACING;
                    using var brush = new SolidBrush(Color.FromArgb(64, Color.Yellow));
                    e.Graphics.FillRectangle(brush,
                        xOffset + gridX * CELL_SIZE + 1,
                        GRID_OFFSET + gridY * CELL_SIZE + 1,
                        CELL_SIZE - 1,
                        CELL_SIZE - 1);
                }
            }
        }

        private void DrawGrid(Graphics g, int xOffset, bool isPlayerGrid)
        {            // Draw coordinate labels
            // Draw column labels (A-J)
            for (int i = 0; i < GRID_SIZE; i++)
            {
                string colLabel = ((char)('A' + i)).ToString();
                var size = g.MeasureString(colLabel, coordinateFont);
                g.DrawString(colLabel, coordinateFont, Brushes.Black,
                    xOffset + (i * CELL_SIZE) + (CELL_SIZE - size.Width) / 2,
                    GRID_OFFSET - 25);
            }

            // Draw row labels (1-10)
            for (int i = 0; i < GRID_SIZE; i++)
            {
                string rowLabel = (i + 1).ToString();
                var size = g.MeasureString(rowLabel, coordinateFont);
                g.DrawString(rowLabel, coordinateFont, Brushes.Black,
                    xOffset - 25 - size.Width,
                    GRID_OFFSET + (i * CELL_SIZE) + (CELL_SIZE - size.Height) / 2);
            }

            // Draw grid background
            using (var brush = new SolidBrush(Color.FromArgb(240, 240, 255)))
            {
                g.FillRectangle(brush,
                    xOffset, GRID_OFFSET,
                    GRID_SIZE * CELL_SIZE, GRID_SIZE * CELL_SIZE);
            }

            // Draw grid lines
            using (var pen = new Pen(Color.Navy, 1.5f))
            {
                for (int i = 0; i <= GRID_SIZE; i++)
                {
                    g.DrawLine(pen,
                        xOffset + (i * CELL_SIZE), GRID_OFFSET,
                        xOffset + (i * CELL_SIZE), GRID_OFFSET + (GRID_SIZE * CELL_SIZE));

                    g.DrawLine(pen,
                        xOffset, GRID_OFFSET + (i * CELL_SIZE),
                        xOffset + (GRID_SIZE * CELL_SIZE), GRID_OFFSET + (i * CELL_SIZE));
                }
            }

            var currentGrid = isPlayerGrid ? 
                (currentPlayer == 1 ? player1Grid : player2Grid) :
                (currentPlayer == 1 ? player2Grid : player1Grid);

            var shotsGrid = isPlayerGrid ?
                (currentPlayer == 1 ? player2Shots : player1Shots) :
                (currentPlayer == 1 ? player1Shots : player2Shots);

            var ships = isPlayerGrid ?
                (currentPlayer == 1 ? player1Ships : player2Ships) :
                (currentPlayer == 1 ? player2Ships : player1Ships);

            // Draw ships on player's grid
            if (isPlayerGrid)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    for (int y = 0; y < GRID_SIZE; y++)
                    {
                        if (currentGrid[x, y])
                        {
                            DrawCell(g, xOffset, x, y, Color.DarkSlateGray);
                        }
                    }
                }
            }

            // Draw shots
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    if (shotsGrid[x, y])
                    {
                        bool isHit = currentGrid[x, y];
                        DrawShot(g, xOffset, x, y, isHit);
                    }
                }
            }
        }

        private void DrawCell(Graphics g, int xOffset, int x, int y, Color color)
        {
            var rect = new Rectangle(
                xOffset + (x * CELL_SIZE) + 1,
                GRID_OFFSET + (y * CELL_SIZE) + 1,
                CELL_SIZE - 1,
                CELL_SIZE - 1);

            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }

            using (var pen = new Pen(Color.FromArgb(color.R / 2, color.G / 2, color.B / 2)))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        private void DrawShot(Graphics g, int xOffset, int x, int y, bool isHit)
        {
            int centerX = xOffset + (x * CELL_SIZE) + CELL_SIZE / 2;
            int centerY = GRID_OFFSET + (y * CELL_SIZE) + CELL_SIZE / 2;
            int radius = CELL_SIZE / 4;

            if (isHit)
            {
                // Draw X for hit
                using var pen = new Pen(Color.Red, 3);
                g.DrawLine(pen,
                    centerX - radius, centerY - radius,
                    centerX + radius, centerY + radius);
                g.DrawLine(pen,
                    centerX - radius, centerY + radius,
                    centerX + radius, centerY - radius);
            }
            else
            {
                // Draw O for miss
                using var pen = new Pen(Color.Blue, 3);
                g.DrawEllipse(pen,
                    centerX - radius, centerY - radius,
                    radius * 2, radius * 2);
            }
        }

        private void GameForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (gameOver) return;

            int targetGridX = GRID_OFFSET + GRID_SIZE * CELL_SIZE + GRID_SPACING;
            if (e.X >= targetGridX && e.X < targetGridX + GRID_SIZE * CELL_SIZE &&
                e.Y >= GRID_OFFSET && e.Y < GRID_OFFSET + GRID_SIZE * CELL_SIZE)
            {
                int gridX = (e.X - targetGridX) / CELL_SIZE;
                int gridY = (e.Y - GRID_OFFSET) / CELL_SIZE;

                var shotsGrid = currentPlayer == 1 ? player1Shots : player2Shots;
                if (!shotsGrid[gridX, gridY])
                {
                    hoverCell = new Point(gridX, gridY);
                    this.Invalidate();
                    return;
                }
            }

            if (hoverCell.HasValue)
            {
                hoverCell = null;
                this.Invalidate();
            }
        }

        private void GameForm_MouseClick(object? sender, MouseEventArgs e)
        {
            if (gameOver || !hoverCell.HasValue) return;

            int gridX = hoverCell.Value.X;
            int gridY = hoverCell.Value.Y;

            var targetGrid = currentPlayer == 1 ? player2Grid : player1Grid;
            var shotsGrid = currentPlayer == 1 ? player1Shots : player2Shots;
            var targetShips = currentPlayer == 1 ? player2Ships : player1Ships;

            if (!shotsGrid[gridX, gridY])
            {                // Record the shot
                shotsGrid[gridX, gridY] = true;                // Check if it's a hit
                bool isHit = targetGrid[gridX, gridY];
                if (isHit)
                {
                    // Find which ship was hit
                    foreach (var ship in targetShips)
                    {
                        if (ship.TryHit(gridX, gridY))
                        {                            if (ship.IsSunk)
                            {
                                SoundManager.PlaySink();
                                MessageBox.Show($"You sunk the opponent's {ship.Name}!", "Ship Sunk!");
                                
                                // Check for game over
                                if (targetShips.All(s => s.IsSunk))
                                {                                    gameOver = true;
                                    SoundManager.PlayVictory();
                                    MessageBox.Show($"Player {currentPlayer} wins!", "Game Over");
                                    return;
                                }
                            }
                            break;
                        }
                    }
                }

                // Switch turns if it's a miss
                if (!isHit)
                {
                    currentPlayer = currentPlayer == 1 ? 2 : 1;
                    this.Text = $"Battleship - Player {currentPlayer}'s Turn";
                    MessageBox.Show($"Miss! Player {currentPlayer}'s turn", "Turn Change");
                }

                ProvideFeedback(isHit);
                UpdateStatusPanel();
                this.Invalidate();
            }
        }

        private void StartNewGame()
        {
            var result = MessageBox.Show(
                "Are you sure you want to start a new game?",
                "New Game",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Create and show a new setup form
                var setupForm = new GameSetupForm();
                this.Hide();
                setupForm.ShowDialog();
                this.Close();
            }
        }        // Add a sound feedback method
        private async void ProvideFeedback(bool isHit)
        {
            if (isHit)
            {
                this.BackColor = Color.FromArgb(255, 255, 240, 240); // Slight red flash
                SoundManager.PlayHit();
            }
            else
            {
                this.BackColor = Color.FromArgb(255, 240, 240, 255); // Slight blue flash
                SoundManager.PlayMiss();
            }

            try
            {
                // Reset background color after a short delay
                await Task.Delay(100);
                if (!IsDisposed && IsHandleCreated)
                {
                    this.BackColor = SystemColors.Control;
                }
            }
            catch (ObjectDisposedException)
            {
                // Form was closed, ignore
            }
        }

        private void UpdateStatusPanel()
        {
            // Clear existing controls
            var statusPanel = this.Controls.OfType<Panel>().First();
            statusPanel.Controls.Clear();

            // Add turn indicator
            Label turnLabel = new Label
            {
                Text = $"Player {currentPlayer}'s Turn",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(180, 30)
            };
            statusPanel.Controls.Add(turnLabel);

            // Add ship status for current player
            var ships = currentPlayer == 1 ? player1Ships : player2Ships;
            int yOffset = 50;

            Label shipsLabel = new Label
            {
                Text = "Your Ships:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, yOffset),
                Size = new Size(180, 20)
            };
            statusPanel.Controls.Add(shipsLabel);
            yOffset += 30;

            foreach (var ship in ships)
            {
                var shipStatus = $"{ship.Name}: {(ship.IsSunk ? "Sunk" : $"{ship.Hits.Count(h => h)}/{ship.Length} hits")}";
                Label shipLabel = new Label
                {
                    Text = shipStatus,
                    ForeColor = ship.IsSunk ? Color.Red : Color.Black,
                    Location = new Point(20, yOffset),
                    Size = new Size(160, 20)
                };
                statusPanel.Controls.Add(shipLabel);
                yOffset += 25;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (gameOver)
            {
                if (keyData == Keys.N)
                {
                    StartNewGame();
                    return true;
                }
            }
            else if (!gameOver)
            {
                // Allow Escape to exit targeting mode
                if (keyData == Keys.Escape)
                {
                    hoverCell = null;
                    this.Invalidate();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }    }
}
