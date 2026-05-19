using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class GomokuBoard2Player : MonoBehaviour
{
    [Header("Stone Prefabs")]
    [SerializeField] private GameObject greenStonePrefab;
    [SerializeField] private GameObject redStonePrefab;

    [Header("Indicators")]
    [SerializeField] private SpriteRenderer hoverIndicatorRenderer;
    [SerializeField] private Transform lastMoveIndicator;

    [Header("Stone Sprites (for Hover)")]
    [SerializeField] private Sprite greenStoneSprite;
    [SerializeField] private Sprite redStoneSprite;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;

    // DSA Layer: 0 = empty, 1 = green, 2 = red
    // Indices 0-14 represent grid intersections
    private int[,] boardMatrix = new int[15, 15];
    private bool isGreenTurn = true;
    private bool isGameOver = false;

    private void Start()
    {
        // Ensure indicators and panels are hidden at start
        if (hoverIndicatorRenderer != null) hoverIndicatorRenderer.gameObject.SetActive(false);
        if (lastMoveIndicator != null) lastMoveIndicator.gameObject.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        UpdateStatusUI();
    }

    private void UpdateStatusUI()
    {
        if (statusText == null) return;
        
        if (isGreenTurn)
            statusText.text = "Player 1's Turn (Green)";
        else
            statusText.text = "Player 2's Turn (Red)";
    }

    private void Update()
    {
        if (isGameOver)
        {
            if (hoverIndicatorRenderer != null) hoverIndicatorRenderer.gameObject.SetActive(false);
            return;
        }

        // 1. Calculate grid position from mouse
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        Vector2 localPos = transform.InverseTransformPoint(worldPos);

        // 2. Math Shift: Round to nearest intersection (0-14)
        int x = Mathf.RoundToInt(localPos.x);
        int y = Mathf.RoundToInt(localPos.y);

        // 3. Logic checks
        bool isWithinBounds = x >= 0 && x < 15 && y >= 0 && y < 15;
        bool isSlotEmpty = isWithinBounds && boardMatrix[x, y] == 0;

        // 4. Update Hover Preview
        UpdateHoverPreview(x, y, isSlotEmpty);

        // 5. Handle Click (New Input System)
        if (Mouse.current.leftButton.wasPressedThisFrame && isSlotEmpty)
        {
            PlaceStone(x, y);
        }
    }

    private void UpdateHoverPreview(int x, int y, bool isVisible)
    {
        if (hoverIndicatorRenderer == null) return;

        if (isVisible)
        {
            hoverIndicatorRenderer.gameObject.SetActive(true);
            // Local Z: -0.05f (Between board and stones)
            hoverIndicatorRenderer.transform.localPosition = new Vector3(x, y, -0.05f);
            hoverIndicatorRenderer.sprite = isGreenTurn ? greenStoneSprite : redStoneSprite;
        }
        else
        {
            hoverIndicatorRenderer.gameObject.SetActive(false);
        }
    }

    private void PlaceStone(int x, int y)
    {
        int currentPlayer = isGreenTurn ? 1 : 2;
        boardMatrix[x, y] = currentPlayer;

        // Visual Instantiation
        GameObject prefab = isGreenTurn ? greenStonePrefab : redStonePrefab;
        GameObject stone = Instantiate(prefab, transform);
        
        // Exact integer local coordinates for intersection snapping
        // Local Z: -0.1f
        stone.transform.localPosition = new Vector3(x, y, -0.1f);

        // Update Last Move Indicator
        if (lastMoveIndicator != null)
        {
            lastMoveIndicator.gameObject.SetActive(true);
            // Local Z: -0.2f (In front of everything)
            lastMoveIndicator.transform.localPosition = new Vector3(x, y, -0.2f);
        }

        // Check Win Condition
        if (CheckWinCondition(x, y, currentPlayer))
        {
            TriggerGameOver(isGreenTurn ? "Player 1 (Green)" : "Player 2 (Red)");
        }
        else
        {
            isGreenTurn = !isGreenTurn;
            UpdateStatusUI();
        }
    }

    private void TriggerGameOver(string winnerName)
    {
        isGameOver = true;
        if (statusText != null) statusText.text = "Game Over";
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (winnerText != null) winnerText.text = winnerName + " Wins!";
        }
    }

    #region DSA Win Detection Logic (Optimized O(1))
    
    private bool CheckWinCondition(int startX, int startY, int playerID)
    {
        // Horizontal check
        if (CountStones(startX, startY, 1, 0, playerID) + CountStones(startX, startY, -1, 0, playerID) >= 4) return true;
        // Vertical check
        if (CountStones(startX, startY, 0, 1, playerID) + CountStones(startX, startY, 0, -1, playerID) >= 4) return true;
        // Diagonal 1 (\)
        if (CountStones(startX, startY, 1, 1, playerID) + CountStones(startX, startY, -1, -1, playerID) >= 4) return true;
        // Diagonal 2 (/)
        if (CountStones(startX, startY, 1, -1, playerID) + CountStones(startX, startY, -1, 1, playerID) >= 4) return true;

        return false;
    }

    private int CountStones(int startX, int startY, int dirX, int dirY, int playerID)
    {
        int count = 0;
        int currentX = startX + dirX;
        int currentY = startY + dirY;

        while (currentX >= 0 && currentX < 15 && currentY >= 0 && currentY < 15 &&
               boardMatrix[currentX, currentY] == playerID)
        {
            count++;
            currentX += dirX;
            currentY += dirY;
        }

        return count;
    }

    #endregion
}
