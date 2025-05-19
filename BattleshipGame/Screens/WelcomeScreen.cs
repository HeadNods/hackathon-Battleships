namespace BattleshipGame;

public partial class WelcomeScreen : Form
{
    public WelcomeScreen()
    {
        InitializeComponent();
        InitializeWelcomeScreen();
    }

    private void InitializeWelcomeScreen()
    {
        // Set form properties
        this.Text = "Battleship Game - Welcome";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        // Create welcome label
        Label welcomeLabel = new Label
        {
            Text = "Welcome to Battleship",
            Font = new Font("Arial", 24, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(400, 50),
            Location = new Point((this.ClientSize.Width - 400) / 2, 50)
        };

        // Create start game button
        Button startGameButton = new Button
        {
            Text = "Start New Game",
            Size = new Size(200, 50),
            Location = new Point((this.ClientSize.Width - 200) / 2, 200),
            Font = new Font("Arial", 12)
        };
        startGameButton.Click += StartGameButton_Click;

        // Create exit button
        Button exitButton = new Button
        {
            Text = "Exit",
            Size = new Size(200, 50),
            Location = new Point((this.ClientSize.Width - 200) / 2, 300),
            Font = new Font("Arial", 12)
        };
        exitButton.Click += (s, e) => this.Close();

        // Add controls to form
        this.Controls.AddRange(new Control[] { welcomeLabel, startGameButton, exitButton });
    }    private void StartGameButton_Click(object? sender, EventArgs e)
    {
        var gameSetupForm = new GameSetupForm();
        this.Hide();
        gameSetupForm.ShowDialog();
        this.Close();
    }
}
