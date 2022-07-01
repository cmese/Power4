using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gameboard : MonoBehaviour
{
    [SerializeField]
    private GameObject menuPiecePrefab;
    private Quaternion chipRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    private Vector3 size;
    private char[,] currentBoard;
    private char[,] menuBoard1 = new char[6,7] {
        { '.', '.', '.', '.', '.', '.', '.' },
        { '.', '.', '.', '.', '.', '.', '.' },
        { '.', 'M', '.', '.', '.', '.', '.' },
        { '.', 'E', '.', '.', '.', '.', '.' },
        { '.', 'N', '.', '.', '.', '.', '.' },
        { '.', 'U', '.', '.', '.', '.', '.' },
    };
    //fuck
    // Start is called before the first frame update
    void Start() {
        size = gameObject.transform.localScale;
        currentBoard = BoardProducer(menuBoard1);
    }

    // Method takes a 2d array and produces Gameboard
    private char[,] BoardProducer(char[,] gameboard) {
        for (int row=0; row<gameboard.GetLength(0); row++) {
            for (int col=0; col<gameboard.GetLength(1); col++) {
                GameObject menuPiece = gameboard[row, col] != '.' ? Instantiate(menuPiecePrefab, new Vector3(col+.5f, -row-.5f, -.75f), chipRotation) : null;
                if (menuPiece) menuPiece.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gameboard[row, col].ToString();
            }
        }
        return gameboard;
    }

    //inserts char in gameboard array at given column down to the row with first empty space
    //return new row or  -1 if there is no space in column
    public int insert(int col) {
        if (col < 0) return -1;
        int numRows = currentBoard.GetLength(0);
        int newRow = -1;
        for (int i=0; i<numRows; i++) {
            if (i == numRows - 1) {
                currentBoard[i, col] = 'X';
                newRow = i;
                break;
            }
            if (currentBoard[i+1, col] != '.') {
                currentBoard[i, col] = 'X';
                newRow = i;
                break;
            }
        }
        return newRow;
    }

    // Update is called once per frame
    void Update() {
    }
}
