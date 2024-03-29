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

        //Score center column
        score += getCenterScore(board);

        //Horizontal - score potential of horizontal connect 4 in each row
        for (int row = 0; row < rows; row++) {
            //var rowArray = GetRow(board, row);
            for (int col = 0; col < cols-3; col++) {
                //score += evaluateWindow(board, rowArray[col..(col+windowLength)]);

                //Vector2Int[] window = getHorizontalWindow(board, row, col);
                //score += evaluateWindow(board, window);
                score += getHorizontalScore(board, row, col);
            }
        }

        //Vertical Score
        for (int col = 0; col < cols; col++) {
            for (int row = 0; row < rows/2; row++) {
                score += getVerticalScore(board, row, col);
            }
        }

        // Diag Scores - checks bottom diags before top ones
        //Diag Positive Slope. checked right to left. row >=3, col <= 3
        for (int row = rows/2-1; row >= 0; row--) {
            for (int col = cols-1; col >= cols/2; col--) {
                score += getDiagPosScore(board, row, col);
            }
        }

        //Diag Negative Slope. checked left to right. row <= 2, col <= 3
        for (int row = rows/2-1; row >= 0; row--) {
            for (int col = 0; col <= cols/2; col++) {
                score += getDiagPosScore(board, row, col);
            }
        }
        return score;
    }

    private float getCountScore(int emptySpace, int aiCount, int oppCount) {
        float score = 0;
        // 4/4 in window
        if (aiCount == 4) score = Mathf.Infinity; //AI win
        else if (oppCount == 4) score = Mathf.NegativeInfinity; //oppponent win

        // 3/4 in window.
        else if (aiCount == 3 && emptySpace == 1) score = 1000;
        else if (oppCount == 3 && emptySpace == 1) score = -1000;

        // 2/4
        else if (aiCount == 2 && emptySpace == 2) score = 100;
        else if (oppCount == 2 && emptySpace == 2) score = -100;

        return score;
    }

    private float getCenterScore(int[,] board) {
        float score = 0;
        int aiCount = 0, oppCount = 0;
        for (int row = 0; row < rows; row++) {
            switch (board[row, cols/2]) {
                case 0:
                    break;
                case 1:
                    aiCount++; break;
                case 2:
                    oppCount++; break;
            }
        }
        score += aiCount * 6;
        score -= oppCount * 6;
        return score;
    }

    private float getHorizontalScore(int[,] board, int row, int col) {
        int emptySpace = 0, aiCount = 0, oppCount = 0;
        for (int i = 0; i < windowLength; i++) {
            switch (board[row, col + i]) {
                case 0:
                    emptySpace++;
                    if (!isReachable(board, row, col + i)) return 0;
                    break;
                case 1:
                    aiCount++;
                    break;
                case 2:
                    oppCount++;
                    break;
            }
        }
        return getCountScore(emptySpace, aiCount, oppCount);
    }

    private float getVerticalScore(int[,] board, int row, int col) {
        int emptySpace = 0, aiCount = 0, oppCount = 0;
        for (int i = 0; i < windowLength; i++) {
            switch (board[row + i, col]) {
                case 0:
                    emptySpace++;
                    break;
                case 1:
                    aiCount++;
                    break;
                case 2:
                    oppCount++;
                    break;
            }
        }
        return getCountScore(emptySpace, aiCount, oppCount);
    }

    //positive slope left to right. checked from top to bottom
    private float getDiagPosScore(int[,] board, int row, int col) {
        int emptySpace = 0, aiCount = 0, oppCount = 0;
        for (int i = 0; i < windowLength; i++) {
            switch (board[row + i, col - i]) {
                case 0:
                    emptySpace++;
                    break;
                case 1:
                    aiCount++;
                    break;
                case 2:
                    oppCount++;
                    break;
            }
        }
        return getCountScore(emptySpace, aiCount, oppCount);
    }

    //negative slope left to right. checked from top to bottom
    private float getDiagNegScore(int[,] board, int row, int col) {
        int emptySpace = 0, aiCount = 0, oppCount = 0;
        for (int i = 0; i < windowLength; i++) {
            switch (board[row + i, col + i]) {
                case 0:
                    emptySpace++;
                    break;
                case 1:
                    aiCount++;
                    break;
                case 2:
                    oppCount++;
                    break;
            }
        }
        return getCountScore(emptySpace, aiCount, oppCount);
    }

    private Vector2Int[] getHorizontalWindow(int[,] board, int row, int col) {
        Vector2Int[] window = new Vector2Int[4];
        for (int i = 0; i < windowLength; i++)
            window[i] = new Vector2Int(row, col + i);
        return window;
    }

    private int[] GetRow(int[,] board, int row) {
        return Enumerable.Range(0, cols)
                .Select(col => board[row, col])
                .ToArray();
    }

    // returns true if empty cells in window are reachable
    private bool isReachable(int[,] board, int row, int col) {
        if (row + 1 == rows || board[row + 1, col] != 0)
            return true;
        else
            return false;
    }

    private Tuple<int, int, int> getCounts(int[,] board, Vector2Int[] window) {
        int emptySpace = 0, aiCount = 0, oppCount = 0;
        for (int i = 0; i < windowLength; i++) {
            switch (board[window[i].x, window[i].y]) {
                case 0:
                    emptySpace++;
                    break;
                case 1:
                    aiCount++;
                    break;
                case 2:
                    oppCount++;
                    break;
            }
        }
        return new Tuple<int, int, int>(emptySpace, aiCount, oppCount);
    }

    private float evaluateWindow(int[,] board, Vector2Int[] window) {
        float score = 0;
        int aiCount = window.Count(space => board[space.x, space.y] == 1);
        int oppCount = window.Count(space => board[space.x, space.y] == 2);
        int emptySpace = window.Count(space => board[space.x, space.y] == 0);

        // 4/4 in window
        if (aiCount == 4) score = Mathf.Infinity; //AI win
        else if (oppCount == 4) score = Mathf.NegativeInfinity; //oppponent win

        // 3/4 in window. TODO: AND is reachable (Figure 7)
        else if (aiCount == 3 && emptySpace == 1) score = 1000;
        else if (oppCount == 3 && emptySpace == 1) score = -1000;

        // 2/4 TODO: AND is reachable
        //else if (window.Count(x => x == 1) == 2 && window.Count(x => x == 0) == 2) score = 1000;
        //else if (window.Count(x => x == 2) == 2 && window.Count(x => x == 0) == 2) score = -1000;
        return score;
    }


    private float miniMax(int[,] board, int depth, bool isMax, float alpha, float beta) {
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
                    best = Mathf.Max(best, miniMax(board, depth + 1, !isMax, alpha, beta));
                    alpha = Mathf.Max(alpha, best);
                    board[newRow, col] = 0;
                    if (beta <= alpha)
                        break;
                }
            }
            return best;
        } else {
            float best = Mathf.Infinity;
            for (int col = 0; col < cols; col++) {
                int newRow = findRow(board, col);
                if (newRow > -1) {
                    board[newRow, col] = 2;
                    best = Mathf.Min(best, miniMax(board, depth + 1, !isMax, alpha, beta));
                    beta = Mathf.Min(beta, best);
                    board[newRow, col] = 0;
                    if (beta <= alpha)
                        break;
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
                float moveVal = miniMax(board, 0, false, Mathf.NegativeInfinity, Mathf.Infinity); //AI always maximizer
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
