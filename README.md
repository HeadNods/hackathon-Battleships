# Battleship Game

A modern Windows Forms implementation of the classic Battleship board game where two players try to sink each other's fleet of ships.

## Features

- 🎮 Interactive graphical user interface
- 🚢 Classic Battleship gameplay for two players
- 🎯 Visual grid displays for ship placement and targeting
- 🔊 Sound effects for hits, misses, and ship sinking
- ⌨️ Keyboard shortcuts for enhanced gameplay
- 📊 Real-time status panel showing ship conditions

## Requirements

- Windows operating system
- .NET 9.0 or later
- Visual Studio 2022 or later (for development)
- Windows system sounds (for audio feedback)
  - Windows Notify.wav
  - Windows Navigation Start.wav
  - Windows Exclamation.wav
  - tada.wav

## Getting Started

1. Clone or download this repository
2. Open the solution file `Battleship-Copilot-Challenge.sln` in Visual Studio
3. Build and run the application
4. Alternatively, run the compiled executable from:
   ```
   bin/Debug/net9.0-windows/BattleshipGame.exe
   ```

## How to Play

### Game Setup

1. When the game starts, each player will take turns placing their ships
2. For each ship:
   - Click on the grid to place the ship
   - Press `R` or right-click to rotate the ship before placement
   - Ships cannot overlap or extend beyond the grid
   - Click "Reset" to start over with ship placement

### Available Ships

- Carrier (5 spaces)
- Battleship (4 spaces)
- Cruiser (3 spaces)
- Submarine (3 spaces)
- Destroyer (2 spaces)

### Gameplay

1. Players take turns attacking their opponent's grid
2. On your turn:
   - Hover over the opponent's grid to select a target
   - Click to fire at the selected position
   - Red X marks a hit, blue O marks a miss
   - If you hit a ship, you get another turn
   - The game continues until all ships of one player are sunk

### Controls

- **Mouse**: Click to place ships or fire at targets
- **N**: Start a new game
- **Escape**: Cancel target selection
- **Right-click** or **R**: Rotate ship during placement

### Visual Feedback

- Yellow highlight: Valid target position
- Red X: Hit
- Blue O: Miss
- Red flash: Hit feedback
- Blue flash: Miss feedback
- Status panel: Shows conditions of your ships

## Development

The game is built using:
- C# WinForms for the UI
- Object-oriented design for game logic
- Custom drawing for the game board
- System sound integration for effects

## License

This project is available under the MIT License. Feel free to use and modify the code for your own projects.
