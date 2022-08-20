using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager {
    private const int cols = 7;
    private const int rows = 6;

    private const int maxDepth = 2;
    private const int windowLength = 4;

    //TODO: EvaluateBoardPoints(board) - prioritizes maxiumum points (Blitz game mode) instead of connect 4's
    private float evaluateBoard(int[,] board) {
        float score = 0;
        //Horizontal - score potential of horizontal connect 4 in each row
        for (int row = 0; row < rows; row++) {
            var rowArray = GetRow(board, row);
            for (int col = 0; col < cols-3; col++) {
                score += evaluateWindow(rowArray[col..(col+windowLength)]);
            }
        }
        //Vertical Score
        //Diag Score
        return score;
    }

    private int[] GetRow(int[,] board, int row) {
        return Enumerable.Range(0, cols)
                .Select(col => board[row, col])
                .ToArray();
    }

    private float evaluateWindow(int[] window) {
        float score = 0;
        // 4/4 in window
        if (window.Count(x => x == 1) == 4) score = Mathf.Infinity; //AI win
        else if (window.Count(x => x == 2) == 4) score = Mathf.NegativeInfinity; //oppponent win
        // 3/4 in window. TODO: AND is reachable (Figure 7)
        else if (window.Count(x => x == 1) == 3 && window.Count(x => x == 0) == 1) score = 1000;
        else if (window.Count(x => x == 2) == 3 && window.Count(x => x == 0) == 1) score = -1000;
        //TODO: 2/4 AND is reachable
        //TODO: 1/4 AND is reachable
        return score;
    }

    private float miniMax(int[,] board, int depth, bool isMax) {
        float score = evaluateBoard(board);
        //if terminal board, return score
        if (depth == maxDepth) return score; // recursion depth
        if (score == Mathf.Infinity || score == Mathf.NegativeInfinity) return score; //maximizer or minimizer won
        if (!isMovesLeft(board)) return score;// no moves left
        //if maximizer, call minimax recursively on all possible moves with minimizer, vice versa
        if (isMax) {
            float best = Mathf.NegativeInfinity;
            for (int col = 0; col < cols; col++) {
                int newRow = findRow(board, col);
                if (newRow > -1) {
                    board[newRow, col] = 1;
                    best = Mathf.Max(best, miniMax(board, depth + 1, !isMax));
                    board[newRow, col] = 0;
                }
            }
            return best;
        } else {
            float best = Mathf.Infinity;
            for (int col = 0; col < cols; col++) {
                int newRow = findRow(board, col);
                if (newRow > -1) {
                    board[newRow, col] = 2;
                    best = Mathf.Min(best, miniMax(board, depth + 1, !isMax));
                    board[newRow, col] = 0;
                }
            }
            return best;
        }
    }

    private bool isMovesLeft(int[,] board) {
        for (int col = 0; col < cols; col++) {
            if (board[0, col] == 0) return true;
        }
        return false;
    }

    // abstracts dictionary of current chip gameobjects into a 2d array representation
    private int[,] convertBoard(Dictionary<Vector2Int, ChipManager> currentChips) {
        Color opColor = GameManager.Instance.playerColor;
        int[,] board = new int[rows,cols];
        foreach (var chip in currentChips) {
            if (chip.Value.GetColor() == opColor) {
                board[chip.Key.x, chip.Key.y] = 2; //opponent
            } else {
                board[chip.Key.x, chip.Key.y] = 1; //AI
            }
        }
        return board;
    }

    // find lowest empty row in column
    private int findRow(int[,] board, int col) {
        for (int row = 0; row < rows; row++) {
            if (board[row, col] != 0) {
                return row - 1;
            }
        }
        return rows - 1;
    }

    // param board = keys of currentChips dic
    // Returns best possible move (col) for the player
    private int findBestMove(Dictionary<Vector2Int, ChipManager> currentChips) {
        float bestVal = Mathf.NegativeInfinity;
        int bestCol = -1;
        int[,] board = convertBoard(currentChips);

        //Evaluate minimax for all possible moves: iterate over all columns
        for (int col = 0; col < cols; col++) {
            //if column still has space
            //if (board[0, i] == 0) {
            int newRow = findRow(board, col);
            if (newRow > -1) {
                //make the move
                board[newRow, col] = 1;
                //compute evaluation for this move
                float moveVal = miniMax(board, 0, false); //AI always maximizer
                // undo the move
                board[newRow, col] = 0;

                if (moveVal > bestVal) {
                    bestCol = col;
                    bestVal = moveVal;
                }
            }
        }
        Debug.Log("The value of best move: " + bestVal);
        return bestCol;
    }
}
