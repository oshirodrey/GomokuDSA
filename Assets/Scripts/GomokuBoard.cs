using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GomokuBoard : MonoBehaviour
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

    [Header("AI Reference")]
    [SerializeField] private GomokuAI aiController;

    private int[,] boardMatrix = new int[15, 15];
    private bool isGreenTurn = true;
    private bool isPlayerTurn = true; // Locks input during AI turn
    private bool isGameOver = false;

    private void Start()
    {
        if (hoverIndicatorRenderer != null) hoverIndicatorRenderer.gameObject.SetActive(false);
        if (lastMoveIndicator != null) lastMoveIndicator.gameObject.SetActive(false);

        // 1. Random Coin Toss
        isGreenTurn = (Random.Range(0, 2) == 0);
        isPlayerTurn = isGreenTurn; // Player is Green (1), AI is Red (2)

        Debug.Log(isPlayerTurn ? "Player goes first!" : "AI goes first!");

        if (!isPlayerTurn)
        {
            StartCoroutine(AITurnRoutine());
        }
    }

    private void Update()
    {
        if (isGameOver || !isPlayerTurn)
        {
            if (hoverIndicatorRenderer != null) hoverIndicatorRenderer.gameObject.SetActive(false);
            return;
        }

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        Vector2 localPos = transform.InverseTransformPoint(worldPos);

        int x = Mathf.RoundToInt(localPos.x);
        int y = Mathf.RoundToInt(localPos.y);

        bool isWithinBounds = x >= 0 && x < 15 && y >= 0 && y < 15;
        bool isSlotEmpty = isWithinBounds && boardMatrix[x, y] == 0;

        UpdateHoverPreview(x, y, isSlotEmpty);

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
            hoverIndicatorRenderer.transform.localPosition = new Vector3(x, y, -0.05f);
            hoverIndicatorRenderer.sprite = greenStoneSprite; // Player is always Green
        }
        else
        {
            hoverIndicatorRenderer.gameObject.SetActive(false);
        }
    }

    public void PlaceStone(int x, int y)
    {
        int currentPlayer = isGreenTurn ? 1 : 2;
        boardMatrix[x, y] = currentPlayer;

        GameObject prefab = isGreenTurn ? greenStonePrefab : redStonePrefab;
        GameObject stone = Instantiate(prefab, transform);
        stone.transform.localPosition = new Vector3(x, y, -0.1f);

        if (lastMoveIndicator != null)
        {
            lastMoveIndicator.gameObject.SetActive(true);
            lastMoveIndicator.transform.localPosition = new Vector3(x, y, -0.2f);
        }

        if (CheckWinCondition(x, y, currentPlayer))
        {
            isGameOver = true;
            Debug.Log($"GAME OVER! {(isGreenTurn ? "Green" : "Red")} Wins!");
            return;
        }

        // Toggle Turns
        isGreenTurn = !isGreenTurn;
        isPlayerTurn = isGreenTurn;

        if (!isPlayerTurn && !isGameOver)
        {
            StartCoroutine(AITurnRoutine());
        }
    }

    private IEnumerator AITurnRoutine()
    {
        yield return new WaitForSeconds(0.6f); // Natural pause
        Vector2Int aiMove = aiController.GetBestMove(boardMatrix, 2); // 2 = Red (AI)
        PlaceStone(aiMove.x, aiMove.y);
    }

    #region Win Logic (O(1))
    private bool CheckWinCondition(int startX, int startY, int playerID)
    {
        if (CountStones(startX, startY, 1, 0, playerID) + CountStones(startX, startY, -1, 0, playerID) >= 4) return true;
        if (CountStones(startX, startY, 0, 1, playerID) + CountStones(startX, startY, 0, -1, playerID) >= 4) return true;
        if (CountStones(startX, startY, 1, 1, playerID) + CountStones(startX, startY, -1, -1, playerID) >= 4) return true;
        if (CountStones(startX, startY, 1, -1, playerID) + CountStones(startX, startY, -1, 1, playerID) >= 4) return true;
        return false;
    }

    private int CountStones(int startX, int startY, int dirX, int dirY, int playerID)
    {
        int count = 0;
        int currentX = startX + dirX;
        int currentY = startY + dirY;
        while (currentX >= 0 && currentX < 15 && currentY >= 0 && currentY < 15 && boardMatrix[currentX, currentY] == playerID)
        {
            count++;
            currentX += dirX;
            currentY += dirY;
        }
        return count;
    }
    #endregion
}
