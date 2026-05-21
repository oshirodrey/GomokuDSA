# GomokuDSA: A Strategy Game Built on Advanced Algorithms

GomokuDSA is a professional 2D implementation of the classic game "Five-in-a-Row" (Gomoku), developed in **Unity 6**. This project serves as a practical application of core Data Structures and Algorithms (DSA), specifically focusing on Game Theory and state-space search optimizations.

## 1. Introduction
The game is set on a 15x15 grid where players compete to align five stones in a row. The project features a robust single-player mode powered by a **Minimax AI** with **Alpha-Beta Pruning**, as well as a local 2-player mode.

### Technical Highlights (DSA Focus):
- **Minimax Algorithm**: A decision-making algorithm used for zero-sum games.
- **Alpha-Beta Pruning**: An optimization technique that reduces the number of nodes evaluated by the Minimax algorithm in its search tree.
- **O(1) Win Detection**: An optimized algorithm that checks for victory in constant time relative to the board size by only scanning the axes of the most recent move.
- **Search Space Optimization**: AI branching factor reduction by evaluating only "action-adjacent" cells.

---

## 2. How to Play (Rules)
- **Objective**: Be the first player to form an unbroken line of five stones horizontally, vertically, or diagonally.
- **Turns**: 
  - **Single Player**: A random coin toss decides who goes first. Player is Green, AI is Red.
  - **2-Player**: Players take turns placing stones (Green and Red).
- **Placement**: Clicks snap to the nearest grid intersection. A hover indicator previews your move.
- **Restrictions**: Once a stone is placed, it cannot be moved or removed.

---

## 3. How to Run
### Run Locally (for Windows)
1. Navigate to the `Release` section.
2. Download GomokuDSA_Windows.zip and extract the file
3. Run `GomokuDSA.exe`.
4. Use the Main Menu to select Single-player (and difficulty) or 2-Player mode.

### WebGL (GitHub Pages)
1. Visit the project's [GitHub Pages URL](https://oshirodrey.github.io/GomokuDSA/).
2. The game will load directly in your browser. Ensure your browser supports WebGL 2.0.

---

## 4. How to Compile (Development Setup)
To modify or build the project from source, follow these steps:

### Prerequisites
- **Unity Editor Version**: `6000.3.12f1`
- **Download Link**: [Unity 6000.3.12f1 Official Release](https://unity.com/releases/editor/whats-new/6000.3.12f1#notes)
- **Input System**: This project uses the **New Input System Package** (com.unity.inputsystem).

### Setup Steps
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/oshirodrey/GomokuDSA
   ```
2. **Open in Unity Hub**:
   - Open Unity Hub.
   - Click **Add** > **Add project from disk**.
   - Select the `GomokuDSA` folder.
   - Ensure the version is set to `6000.3.12f1`.
3. **Project Settings**:
   - If prompted to "Change to New Input System," click **Yes**. The editor will restart.
4. **Building the Game**:
   - Go to **File** > **Build Settings**.
   - Select your target platform (PC, Mac & Linux Standalone or WebGL).
   - Click **Build** and choose an output directory.

---

## 5. Project Structure
- `Assets/Scripts/GomokuBoard.cs`: Core logic, World-Space math, and Win Detection.
- `Assets/Scripts/GomokuAI.cs`: AI implementation (Minimax/Alpha-Beta).
- `Assets/Scripts/GameSettings.cs`: Static configuration for difficulty levels.
- `Assets/Scripts/MainMenuController.cs`: UI Navigation logic.
