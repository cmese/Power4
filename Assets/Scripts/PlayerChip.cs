using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChip : Chip
{
    private Gameboard currentBoard;

    [SerializeField] private CapsuleCollider currentChipCollider;
    private Chip previewChip;

    [SerializeField] private float speedToTop = 120.0f;
    [SerializeField] private float speedDownBoard = 2.0f;
    [SerializeField] private float speedBackToStart = 0.8f;

    //[SerializeField] private DragObject currentChip;
    private float target=1;
    private float current;

    private Vector3 startPos;

    void Awake() {
        startPos = gameObject.transform.position;
        currentBoard = GameManager.Instance.GetGameboard();
    }

    public void UpdatePreviewChip(int currentCol) {
        if (previewChip) previewChip.gameObject.SetActive(false);
        if (currentCol > -1) {
            previewChip = currentBoard.GetPreviewChips()[currentCol];
            previewChip.Init(this.GetColor());
            previewChip.gameObject.SetActive(true);
        }
    }

    public void MoveChip(int currentCol) {
        StartCoroutine(MoveChipCoroutine(currentCol));
    }

    //TODO: Add duration to all coroutines. Calculate speed based on duration given
    IEnumerator MoveChipCoroutine(int currentCol) {
        if (GameManager.Instance.State == GameState.PlayerTurn) {
            int newRow = currentBoard.insert(currentCol, this);
            if (newRow > -1) {
                yield return StartCoroutine(MoveToGameBoard(newRow, currentCol));
                GameManager.Instance.UpdateGameState(GameState.Decide);
                MenuManager.Instance.UpdatePanel(-1);
                yield break;
            }
        }
        yield return StartCoroutine(MoveToStart());
    }

    IEnumerator MoveToGameBoard(int newRow, int currentCol) {
        currentChipCollider.enabled = false;
        GetComponent<DragObject>().enabled = false;
        yield return StartCoroutine(MoveToTop(currentCol));
        yield return StartCoroutine(MoveDownBoard(newRow));
    }

    IEnumerator MoveToTop(int currentCol) {
        Vector3 startLerpPos = gameObject.transform.position;
        Vector3 finalLerpPos = new Vector3(currentCol+0.5f, 1-0.5f, -0.75f);
        while (current < 1) {
            current = Mathf.MoveTowards(current, target, speedToTop * Time.deltaTime);
            gameObject.transform.position = Vector3.Lerp(startLerpPos, finalLerpPos, current);
            yield return null;
        }
        current = 0;
    }

    IEnumerator MoveDownBoard(int newRow) {
        Vector3 startLerpPos = gameObject.transform.position;
        Vector3 finalLerpPos =  new Vector3(startLerpPos.x, -newRow-0.5f, startLerpPos.z);
        while (current < 1) {
            current = Mathf.MoveTowards(current, target, speedDownBoard * Time.deltaTime);
            gameObject.transform.position = Vector3.Lerp(startLerpPos, finalLerpPos, current);
            yield return null;
        }
        current = 0;
    }

    //should also get called when gamestate is not players turn
    IEnumerator MoveToStart() {
        Vector3 startLerpPos = gameObject.transform.position;
        Vector3 finalLerpPos = startPos;
        while (current < 1) {
            current = Mathf.MoveTowards(current, target, speedBackToStart * Time.deltaTime);
            gameObject.transform.position = Vector3.Lerp(startLerpPos, finalLerpPos, current);
            yield return null;
        }
        current = 0;
    }
}
