using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour
{
    private const int rows = 6;
    private const int cols = 7;

    [SerializeField] private MenuChipManager menuChipPrefab;
    [SerializeField] private ChipManager dummyChipPrefab;
    [SerializeField] private ChipManager previewChipPrefab;
    public List<ChipManager> previewChips {get; private set;}

    private Quaternion chipRotation = Quaternion.Euler(new Vector3(-90, 0, 0));

    private Dictionary<Vector2Int, ChipManager> currentChips; //dict representation of current gameboard
    private Queue<Vector2Int> movedChips; //queue of chips to process for connect 4's

    void Awake() {
        currentChips = new Dictionary<Vector2Int, ChipManager>();
        movedChips = new Queue<Vector2Int>();
        previewChips = CreatePreviewChips();
    }

    public void MenuDictToBoard(Dictionary<int, string[]> menuDict) {
        ClearBoard();
        for (int col = 0; col < cols; col++) {
            int rowIndex = rows-1;
            if (menuDict.TryGetValue(col, out string[] menuStrings)) {
                for (int i = menuStrings[0].Length - 1; i >= 0; i--) {
                    var menuChip = Instantiate(menuChipPrefab, new Vector3(col+0.5f, -rowIndex-0.5f, -0.75f), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    menuChip.SetColor(GameManager.Instance.playerColor);
                    menuChip.SetText(menuStrings[0][i].ToString());
                    currentChips[new Vector2Int(rowIndex, col)] = menuChip;
                    rowIndex--;
                }
            } else {
                for (int row = rowIndex; row >= 0; row--) {
                    var dummyChip = Instantiate(dummyChipPrefab, new Vector3(col+0.5f, -row-0.5f, -0.75f), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    dummyChip.SetColor(Color.black);
                    currentChips[new Vector2Int(row, col)] = dummyChip;
                }
            }
        }
    }

    private List<ChipManager> CreatePreviewChips() {
        previewChips = new List<ChipManager>();
        for (int i = 0; i < cols; i++) {
            var previewChip = Instantiate(previewChipPrefab, new Vector3(i+0.5f, 1-0.5f, -0.75f), chipRotation);
            previewChip.gameObject.SetActive(false);
            previewChips.Add(previewChip);
        }
        return previewChips;
    }

    public void ClearBoard() {
        //TODO: drop the chips, destroy them when they go passed a certain height
        foreach (ChipManager chip in currentChips.Values) {
            Destroy(chip.gameObject);
        }
        currentChips.Clear();
        movedChips.Clear();
    }

    //TODO: change to moveDownInsert
    public int insert(int col, ChipManager movedChip) {
        if (col < 0) return -1;
        int newRow = -1;
        for (int row = rows - 1; row >= 0; row--) {
            Vector2Int pos = new Vector2Int(row, col);
            if (!currentChips.ContainsKey(pos)) {
                currentChips.Add(pos, movedChip);
                movedChips.Enqueue(pos);
                newRow = row;
                //GameManager.Instance.ChipInserted(col);
                break;
            }
        }
        return newRow;
    }

    //move to processboardstate TODO: switch this entire thing to gameboard MANAGER
    public IEnumerator CheckForWins() {
        HashSet<Vector2Int> totalPlayerSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> totalEnemySet = new HashSet<Vector2Int>();
        while (movedChips.Count > 0) {
            var chipPos = movedChips.Dequeue();
            Color chipColor = currentChips[chipPos].GetColor();
            if (GameManager.Instance.playerColor == chipColor) {
                totalPlayerSet.UnionWith(HorizontalCheck(chipPos, chipColor));
                totalPlayerSet.UnionWith(VerticalCheck(chipPos, chipColor));
                totalPlayerSet.UnionWith(BottomLeftTopRightDiagCheck(chipPos, chipColor)); //can save diag checks if check chipPos.x, chipPos.y is possible for 4 chip diag
                totalPlayerSet.UnionWith(BottomRightTopLeftDiagCheck(chipPos, chipColor));
            } else {
                totalEnemySet.UnionWith(HorizontalCheck(chipPos, chipColor));
                totalEnemySet.UnionWith(VerticalCheck(chipPos, chipColor));
                totalEnemySet.UnionWith(BottomLeftTopRightDiagCheck(chipPos, chipColor));
                totalEnemySet.UnionWith(BottomRightTopLeftDiagCheck(chipPos, chipColor));
            }
            if (movedChips.Count == 0) {
                //TODO: Calculate Points for each set
                //process chips above winners
                //color totalplayerset, color totalenemyset, pause for a second, then delete
                //DebugColorizer(totalPlayerSet, totalEnemySet);
                yield return StartCoroutine(DebugColorizer(totalPlayerSet, totalEnemySet));
                HashSet<Vector2Int> chipsAboveSet = new HashSet<Vector2Int>();
                HashSet<Vector2Int> totalWinSet = new HashSet<Vector2Int>(totalPlayerSet);
                totalWinSet.UnionWith(totalEnemySet);
                foreach (Vector2Int pos in totalWinSet) {
                    for (int row = pos.x-1; row >= 0; row--) {
                        Vector2Int chipAbove = new Vector2Int(row, pos.y);
                        //if nothing above it, break
                        if (!currentChips.ContainsKey(chipAbove))
                            break;
                        //if something above it, and not in totalwinset, add to moved chips
                        if (!totalWinSet.Contains(chipAbove)) {
                            chipsAboveSet.Add(chipAbove);
                            //movedChips.Enqueue(chipAbove);
                        }
                    }
                    //delete the chip
                    Destroy(currentChips[pos].gameObject);
                    currentChips.Remove(pos);
                }

                //better to do it this way so that bottom chips move down before chips above it
                //move to own function
                for (int col = 0; col < cols; col++) {
                    int spaceCount = 0;
                    for (int row = rows-1; row >=0; row--) {
                        Vector2Int pos = new Vector2Int(row, col);
                        if (!currentChips.ContainsKey(pos))
                            spaceCount++;
                        else if (chipsAboveSet.Contains(pos)) {
                            var chip = currentChips[pos];
                            currentChips.Remove(pos);
                            Vector2Int newLocation = new Vector2Int(pos.x + spaceCount, pos.y);
                            chip.stateQueue.Enqueue(new MoveState(chip, new Vector3(newLocation.y + 0.5f, newLocation.x - 0.5f, -0.75f), 1));
                            chip.stateQueue.Enqueue(new InBoardState(chip));
                            chip.stateMachine.ChangeState(chip.stateQueue.Dequeue());
                            currentChips[newLocation] = chip;
                            movedChips.Enqueue(newLocation);
                        }
                    }
                }
            }
        }
    }

    IEnumerator DebugColorizer(HashSet<Vector2Int> playerWins, HashSet<Vector2Int> enemyWins) {
        Debug.Log("Coloring Connect 4's");
        foreach(Vector2Int pos in playerWins) {
            currentChips[pos].SetColor(Color.green);
        }
        foreach(Vector2Int pos in enemyWins) {
            currentChips[pos].SetColor(Color.red);
        }
        yield return new WaitForSeconds(2);
    }

    private HashSet<Vector2Int> HorizontalCheck(Vector2Int pos, Color sameColor) {
        HashSet<Vector2Int> matchingChips = new HashSet<Vector2Int>();
        matchingChips.Add(pos);
        //prev state must be players turn which means match players color to chip
        for (int col = pos.y-1; col >= 0; col--) {
            if (currentChips.TryGetValue(new Vector2Int(pos.x, col), out var neighborChip)) {
                if (neighborChip.GetColor() == sameColor) {
                    matchingChips.Add(new Vector2Int(pos.x, col));
                } else {
                    break; //different color neighbor chip
                }
            } else {
                break; //no neighboring chips
            }
        }
        for (int col = pos.y+1; col < cols; col++) {
            if (currentChips.TryGetValue(new Vector2Int(pos.x, col), out var neighborChip)) {
                if (neighborChip.GetColor() == sameColor) {
                    matchingChips.Add(new Vector2Int(pos.x, col));
                } else {
                    break; //different color neighbor chip
                }
            } else {
                break; //no neighboring chips
            }
        }
        if (matchingChips.Count >= 4) {
            return matchingChips;
        }
        matchingChips.Clear(); //change
        return matchingChips;
    }

    private HashSet<Vector2Int> VerticalCheck(Vector2Int pos, Color sameColor) {
        HashSet<Vector2Int> matchingChips = new HashSet<Vector2Int>();
        matchingChips.Add(pos);
        //prev state must be players turn which means match players color to chip
        for (int row = pos.x-1; row >= 0; row--) {
            if (currentChips.TryGetValue(new Vector2Int(row, pos.y), out var neighborChip)) {
                if (neighborChip.GetColor() == sameColor) {
                    matchingChips.Add(new Vector2Int(row, pos.y));
                } else {
                    break; //different color neighbor chip
                }
            } else {
                break; //no neighboring chips
            }
        }
        for (int row = pos.x+1; row < rows; row++) {
            if (currentChips.TryGetValue(new Vector2Int(row, pos.y), out var neighborChip)) {
                if (neighborChip.GetColor() == sameColor) {
                    matchingChips.Add(new Vector2Int(row, pos.y));
                } else {
                    break; //different color neighbor chip
                }
            } else {
                break; //no neighboring chips
            }
        }
        if (matchingChips.Count >= 4) {
            return matchingChips;
        }
        matchingChips.Clear(); //change
        return matchingChips;
    }

    private HashSet<Vector2Int> BottomLeftTopRightDiagCheck(Vector2Int pos, Color sameColor) {
        HashSet<Vector2Int> matchingChips = new HashSet<Vector2Int>();
        matchingChips.Add(pos);
        int row = pos.x+1;
        int col = pos.y-1;
        while (row < rows && col >= 0) { //bottom-left diag
            if (currentChips.TryGetValue(new Vector2Int(row, col), out var neighborChip)) {
                if (neighborChip.GetColor() == sameColor) {
                    matchingChips.Add(new Vector2Int(row, col));
                } else {
                    break; //neighbor is different color
                }
            } else {
                break; //no neighbor
            }
            row++;
            col--;
        }
        row = pos.x-1;
        col = pos.y+1;
        while (row >= 0 && col < cols) { //top-right diag
            if (currentChips.TryGetValue(new Vector2Int(row, col), out var neighborChip)) {
                if (neighborChip.GetColor() == sameColor) {
                    matchingChips.Add(new Vector2Int(row, col));
                } else {
                    break; //neighbor is different color
                }
            } else {
                break; //no neighbor
            }
            row--;
            col++;
        }
        if (matchingChips.Count >= 4) {
            return matchingChips;
        }
        matchingChips.Clear(); //change
        return matchingChips;
    }

    private HashSet<Vector2Int> BottomRightTopLeftDiagCheck(Vector2Int pos, Color sameColor) {
        HashSet<Vector2Int> matchingChips = new HashSet<Vector2Int>();
        matchingChips.Add(pos);
        int row = pos.x+1;
        int col = pos.y+1;
        while (row < rows && col < cols) { //bottom-right diag
            if (currentChips.TryGetValue(new Vector2Int(row, col), out var neighborChip)) {
                if (neighborChip.GetColor() == sameColor) {
                    matchingChips.Add(new Vector2Int(row, col));
                } else {
                    break; //neighbor is different color
                }
            } else {
                break; //no neighbor
            }
            row++;
            col++;
        }
        row = pos.x-1;
        col = pos.y-1;
        while (row >= 0 && col >= 0) { //top-left diag
            if (currentChips.TryGetValue(new Vector2Int(row, col), out var neighborChip)) {
                if (neighborChip.GetColor() == sameColor) {
                    matchingChips.Add(new Vector2Int(row, col));
                } else {
                    break; //neighbor is different color
                }
            } else {
                break; //no neighbor
            }
            row--;
            col--;
        }
        if (matchingChips.Count >= 4) {
            return matchingChips;
        }
        matchingChips.Clear(); //change
        return matchingChips;
    }
}
