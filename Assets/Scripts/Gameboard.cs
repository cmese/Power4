using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gameboard : MonoBehaviour
{
    private const int rows = 6;
    private const int cols = 7;
    //[SerializeField] private GameObject menuPiecePrefab;
    [SerializeField] private MenuChip menuChipPrefab;

    [SerializeField] private PreviewChip previewChipPrefab;
    private List<PreviewChip> previewChips;

    private Quaternion chipRotation = Quaternion.Euler(new Vector3(-90, 0, 0));

    //private char[,] currentBoardArray; //2d array representation of current gameboard
    private Dictionary<Vector2, GameObject> currentChips; //dict representation of current gameboard
    private List<GameObject> movedChips;

    void Awake() {
        currentChips = new Dictionary<Vector2, GameObject>();
        CreatePreviewChips();
    }
    void Start() {
        //currentBoardArray = new char[6,7];
        //CreatePreviewChips();
    }

    //method takes each column entry in dictionary and populates the board with the given string
    //assumes column string lengths <= 6 characters long
    public void DictToBoard(Dictionary<int, string[]> menuDict) {
        foreach (KeyValuePair<int, string[]> menuItem in menuDict) {
            int rowIndex = rows-1;
            for (int i = menuItem.Value[0].Length - 1; i >= 0; i--) {
                var menuChip = Instantiate(menuChipPrefab, new Vector3(menuItem.Key+0.5f, -rowIndex-0.5f, -0.75f), chipRotation);
                menuChip.Init(menuItem.Value[0][i].ToString());
                currentChips[new Vector2(rowIndex, menuItem.Key)] = menuChip.gameObject;
                rowIndex--;
            }
        }
    }

    private void CreatePreviewChips() {
        previewChips = new List<PreviewChip>();
        for (int i = 0; i < cols; i++) {
            var previewChip = Instantiate(previewChipPrefab, new Vector3(i+0.5f, 1-0.5f, -0.75f), chipRotation);
            previewChip.gameObject.SetActive(false);
            previewChips.Add(previewChip);
        }
    }

    public List<PreviewChip> GetPreviewChips() {
        return previewChips;
    }

    public void ClearBoard() {
        //drop the chips, destroy them when they go passed a certain height
        currentChips.Clear();
    }

    public int insert(int col, GameObject insertedChip) {
        if (col < 0) return -1;
        int newRow = -1;
        for (int i = rows - 1; i >= 0; i--) {
            Vector2 pos = new Vector2(i, col);
            Debug.Log(pos);
            if (!currentChips.ContainsKey(pos)) {
                //Debug.Log(pos);
                currentChips.Add(pos, insertedChip);

                newRow = i;
                break;
            }
        }
        return newRow;
    }

    /*public int insert(int col) {
        if (col < 0) return -1;
        int numRows = currentBoardArray.GetLength(0);
        int newRow = -1;
        for (int i=0; i<numRows; i++) {
            if (i == numRows - 1) {
                currentBoardArray[i, col] = 'X';
                newRow = i;
                break;
            }
            if (currentBoardArray[i+1, col] != '.') {
                currentBoardArray[i, col] = 'X';
                newRow = i;
                break;
            }
        }
        return newRow;
    }*/
    // Method takes a 2d array and produces Gameboard
    /*public void CreateMenuBoardArray(char[,] gameboard) {
        currentBoardArray = gameboard;
        for (int row=0; row<gameboard.GetLength(0); row++) {
            for (int col=0; col<gameboard.GetLength(1); col++) {
                //init with a letter
                var menuChip = gameboard[row, col] != '.' ? Instantiate(menuChipPrefab, new Vector3(col+.5f, -row-.5f, -.75f), chipRotation) : null;
                if (menuChip) {
                    menuChip.Init(gameboard[row, col].ToString());
                    currentChips[new Vector2(row, col)] = menuChip.gameObject;
                }
                //if (menuPiece) menuPiece.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gameboard[row, col].ToString();
            }
        }
    }*/

    //inserts char in gameboard array at given column down to the row with first empty space
    //return new row or  -1 if there is no space in column
    /*public int insert(int col) {
        if (col < 0) return -1;
        int numRows = currentBoardArray.GetLength(0);
        int newRow = -1;
        for (int i=0; i<numRows; i++) {
            if (i == numRows - 1) {
                currentBoardArray[i, col] = 'X';
                newRow = i;
                break;
            }
            if (currentBoardArray[i+1, col] != '.') {
                currentBoardArray[i, col] = 'X';
                newRow = i;
                break;
            }
        }
        return newRow;
    }*/
}
