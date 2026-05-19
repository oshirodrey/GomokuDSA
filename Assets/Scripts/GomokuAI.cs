using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gomoku AI using Minimax with Alpha-Beta Pruning (DSA focus).
/// </summary>
public class GomokuAI : MonoBehaviour
{
    private int maxDepth;

    /* 
     * [DSA CONCEPT: HEURISTIC EVALUATION]
     * In complex games like Gomoku, we cannot search to the end of the game (the tree is too big).
     * We use a Heuristic function to "guess" how good a board position is.
     * We assign higher values to patterns like "4 in a row" or "3 open ends".
     */
    private const int FIVE_IN_A_ROW = 1000000;
    private const int FOUR_OPEN = 50000;
    private const int FOUR_BLOCKED = 5000;
    private const int THREE_OPEN = 3000;
    private const int THREE_BLOCKED = 500;
    private const int TWO_OPEN = 100;

    public Vector2Int GetBestMove(int[,] board, int aiPlayerID)
    {
        // --- DIFFICULTY SCALING LOGIC ---
        // We vary the search depth and introduce intentional "blunders" for easier modes.
        float randomness = 0f;
        switch (GameSettings.currentDifficulty)
        {
            case GameSettings.GameDifficulty.Easy: 
                maxDepth = 1; 
                randomness = 0.8f; // 80% chance to make a random-ish move
                break;
            case GameSettings.GameDifficulty.Normal: 
                maxDepth = 2; 
                randomness = 0.3f; // 30% chance to be sub-optimal
                break;
            case GameSettings.GameDifficulty.Hard: 
                maxDepth = 3; 
                randomness = 0.1f; // 10% chance to make a less optimal move 
                break;
            case GameSettings.GameDifficulty.Extreme: 
                maxDepth = 4; 
                randomness = 0.01f; // 1% chance to make a non-optimal move for unpredictability 
                break;
        }

        // 1. [DSA: SEARCH SPACE OPTIMIZATION] - Start in center if board is empty
        if (IsBoardEmpty(board)) return new Vector2Int(7, 7);

        List<Vector2Int> candidates = GetCandidateMoves(board);
        
        // --- EASY MODE "BLUNDER" LOGIC ---
        if (Random.value < randomness && candidates.Count > 0)
        {
            return candidates[Random.Range(0, candidates.Count)];
        }

        // 2. [DSA: MINIMAX INITIALIZATION]
        Vector2Int bestMove = new Vector2Int(-1, -1);
        int bestScore = int.MinValue;

        foreach (Vector2Int move in candidates)
        {
            board[move.x, move.y] = aiPlayerID;
            // Start the recursive Minimax search
            // [DSA: ALPHA-BETA PRUNING] - Pass Min/Max values for alpha and beta
            int score = Minimax(board, 0, false, int.MinValue, int.MaxValue, aiPlayerID);
            board[move.x, move.y] = 0; // [DSA: BACKTRACKING] - Reset board state

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return (bestMove.x == -1) ? candidates[0] : bestMove;
    }

    /*
     * [DSA CONCEPT: MINIMAX ALGORITHM]
     * A recursive algorithm for decision-making. 
     * The AI tries to MAXIMIZE its score (Maximizing player), 
     * while assuming the human player will MINIMIZE the AI's score (Minimizing player).
     * 
     * [DSA CONCEPT: ALPHA-BETA PRUNING]
     * An optimization that cuts off branches in the search tree that cannot possibly 
     * influence the final decision. alpha is the best value the maximizer is guaranteed,
     * and beta is the best value the minimizer is guaranteed.
     */
    private int Minimax(int[,] board, int depth, bool isMaximizing, int alpha, int beta, int aiPlayerID)
    {
        int opponentID = (aiPlayerID == 1) ? 2 : 1;
        
        // [DSA: RECURSION TERMINATION]
        // Stop when we reach the depth limit and evaluate the leaf node.
        if (depth == maxDepth) return EvaluateBoard(board, aiPlayerID);

        List<Vector2Int> candidates = GetCandidateMoves(board);
        if (candidates.Count == 0) return 0;

        if (isMaximizing)
        {
            int maxScore = int.MinValue;
            foreach (Vector2Int move in candidates)
            {
                board[move.x, move.y] = aiPlayerID;
                maxScore = Mathf.Max(maxScore, Minimax(board, depth + 1, false, alpha, beta, aiPlayerID));
                board[move.x, move.y] = 0; // Backtrack
                
                // [DSA: ALPHA PRUNING]
                alpha = Mathf.Max(alpha, maxScore);
                if (beta <= alpha) break; 
            }
            return maxScore;
        }
        else
        {
            int minScore = int.MaxValue;
            foreach (Vector2Int move in candidates)
            {
                board[move.x, move.y] = opponentID;
                minScore = Mathf.Min(minScore, Minimax(board, depth + 1, true, alpha, beta, aiPlayerID));
                board[move.x, move.y] = 0; // Backtrack
                
                // [DSA: BETA PRUNING]
                beta = Mathf.Min(beta, minScore);
                if (beta <= alpha) break;
            }
            return minScore;
        }
    }

    /*
     * [DSA CONCEPT: SEARCH SPACE REDUCTION]
     * Checking all 225 cells is too slow (15x15). 
     * Moves made far away from any stones are almost always useless.
     * We only check empty cells within a 1 or 2-cell radius of existing stones.
     */
    private List<Vector2Int> GetCandidateMoves(int[,] board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        bool[,] visited = new bool[15, 15];

        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (board[x, y] != 0)
                {
                    // Use 1-cell radius for speed and simpler behavior on lower difficulties
                    int radius = (maxDepth > 2) ? 2 : 1;
                    for (int dx = -radius; dx <= radius; dx++)
                    {
                        for (int dy = -radius; dy <= radius; dy++)
                        {
                            int nx = x + dx, ny = y + dy;
                            if (IsValid(nx, ny) && board[nx, ny] == 0 && !visited[nx, ny])
                            {
                                moves.Add(new Vector2Int(nx, ny));
                                visited[nx, ny] = true;
                            }
                        }
                    }
                }
            }
        }
        return moves;
    }

    private int EvaluateBoard(int[,] board, int playerID)
    {
        int opponentID = (playerID == 1) ? 2 : 1;
        
        // We calculate the score for the AI, and subtract the opponent's score.
        // Multiplying the opponent's score by 2 makes the AI prioritize defensive blocking 
        // over building its own lines when threats are equal.
        int score = ScoreForPlayer(board, playerID);
        score -= ScoreForPlayer(board, opponentID) * 2; 
        
        return score;
    }

    private int ScoreForPlayer(int[,] board, int playerID)
    {
        int totalScore = 0;
        int[] dx = { 1, 0, 1, -1 };
        int[] dy = { 0, 1, 1, 1 };

        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (board[x, y] == playerID)
                {
                    for (int dir = 0; dir < 4; dir++)
                    {
                        // Check if this is the start of a line to avoid double counting
                        int prevX = x - dx[dir];
                        int prevY = y - dy[dir];
                        if (IsValid(prevX, prevY) && board[prevX, prevY] == playerID) 
                            continue;

                        totalScore += EvaluateLine(board, x, y, dx[dir], dy[dir], playerID);
                    }
                }
            }
        }
        return totalScore;
    }

    private int EvaluateLine(int[,] board, int startX, int startY, int dx, int dy, int playerID)
    {
        int count = 0;
        int x = startX;
        int y = startY;

        while (IsValid(x, y) && board[x, y] == playerID)
        {
            count++;
            x += dx;
            y += dy;
        }

        int openEnds = 0;
        if (IsValid(startX - dx, startY - dy) && board[startX - dx, startY - dy] == 0) openEnds++;
        if (IsValid(x, y) && board[x, y] == 0) openEnds++;

        if (count >= 5) return FIVE_IN_A_ROW;
        if (count == 4) return openEnds == 2 ? FOUR_OPEN : (openEnds == 1 ? FOUR_BLOCKED : 0);
        if (count == 3) return openEnds == 2 ? THREE_OPEN : (openEnds == 1 ? THREE_BLOCKED : 0);
        if (count == 2 && openEnds == 2) return TWO_OPEN;

        return 0;
    }

    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < 15 && y >= 0 && y < 15;
    }

    private bool IsBoardEmpty(int[,] board)
    {
        for (int i = 0; i < 15; i++)
            for (int j = 0; j < 15; j++)
                if (board[i, j] != 0) return false;
        return true;
    }
}
