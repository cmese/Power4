using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour
{
    private const int rows = 6;
    private const int cols = 7;

    [SerializeField] private ChipManager previewChipPrefab;
    private List<ChipManager> previewChips;

    private Quaternion chipRotation = Quaternion.Euler(new Vector3(-90, 0, 0));

    private Dictionary<Vector2Int, ChipManager> currentChips; //dict representation of current gameboard
    private Queue<Vector2Int> movedChips;

    private int mostRecentPlay;

    void Awake() {
        currentChips = new Dictionary<Vector2Int, ChipManager>();
        movedChips = new Queue<Vector2Int>();
        previewChips = CreatePreviewChips();
    }
    void Start() {
    }

    public void AddChip(Vector2Int pos, ChipManager chip) {
        currentChips[pos] = chip;
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

    public List<ChipManager> GetPreviewChips() {
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

    public int ProcessGameboard() {
        return 0;
    }
/*
    IEnumerator GameDecideCoroutine(bool menu) {
        yield return StartCoroutine(CheckForWins());
        if (menu) {
            MenuManager.Instance.UpdateMenu(mostRecentPlay);
        } else {
            //update gamestate = player turn again
        }
    }
*/
    IEnumerator CheckForWins() {
        HashSet<Vector2Int> totalPlayerSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> totalEnemySet = new HashSet<Vector2Int>();
        while (movedChips.Count > 0) {
            var chipPos = movedChips.Dequeue();
            Color chipColor = currentChips[chipPos].GetColor();
            if (GameManager.Instance.GetPlayerChipColor() == chipColor) {
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
                        Vector2Int chipAbove = new Vector2Int(row, col);
                        if (!currentChips.ContainsKey(chipAbove))
                            spaceCount++;
                        if (chipsAboveSet.Contains(chipAbove)) {
                            var chip = currentChips[chipAbove];
                            currentChips.Remove(chipAbove);
                            Vector2Int newLocation = new Vector2Int(chipAbove.x + spaceCount, chipAbove.y);
                            chip.MoveDown(newLocation.x);
                            currentChips[newLocation] = chip;
                            movedChips.Enqueue(newLocation);
                        }
                    }
                }
            }
        }
    }

    IEnumerator DebugColorizer(HashSet<Vector2Int> playerWins, HashSet<Vector2Int> enemyWins) {
        Debug.Log("am i here");
        foreach(Vector2Int pos in playerWins) {
            currentChips[pos].SetColor(Color.green);
        }
        foreach(Vector2Int pos in enemyWins) {
            currentChips[pos].SetColor(Color.red);
        }
        yield return new WaitForSeconds(3);
        Debug.Log("waiting");
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
