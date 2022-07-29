using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gameboard : MonoBehaviour
{
    private const int rows = 6;
    private const int cols = 7;
    [SerializeField] private MenuChip menuChipPrefab;

    [SerializeField] private Chip previewChipPrefab;
    private List<Chip> previewChips;

    private Quaternion chipRotation = Quaternion.Euler(new Vector3(-90, 0, 0));

    private Dictionary<Vector2Int, Chip> currentChips; //dict representation of current gameboard
    private Queue<Vector2Int> movedChips;

    void Awake() {
        currentChips = new Dictionary<Vector2Int, Chip>();
        movedChips = new Queue<Vector2Int>();
        CreatePreviewChips();
    }
    void Start() {
    }

    //method takes each column entry in dictionary and populates the board with the given string
    //assumes column string lengths <= 6 characters long
    public void DictToBoard(Dictionary<int, string[]> menuDict) {
        foreach (KeyValuePair<int, string[]> menuItem in menuDict) {
            int rowIndex = rows-1;
            for (int i = menuItem.Value[0].Length - 1; i >= 0; i--) {
                var menuChip = Instantiate(menuChipPrefab, new Vector3(menuItem.Key+0.5f, -rowIndex-0.5f, -0.75f), chipRotation);
                menuChip.Init(GameManager.Instance.GetPlayerChipColor());
                menuChip.SetText(menuItem.Value[0][i].ToString());
                currentChips[new Vector2Int(rowIndex, menuItem.Key)] = menuChip;
                rowIndex--;
            }
        }
    }

    private void CreatePreviewChips() {
        previewChips = new List<Chip>();
        for (int i = 0; i < cols; i++) {
            var previewChip = Instantiate(previewChipPrefab, new Vector3(i+0.5f, 1-0.5f, -0.75f), chipRotation);
            previewChip.gameObject.SetActive(false);
            previewChips.Add(previewChip);
        }
    }

    public List<Chip> GetPreviewChips() {
        return previewChips;
    }

    public void ClearBoard() {
        //TODO: drop the chips, destroy them when they go passed a certain height
        currentChips.Clear();
    }

    //TODO: change to moveDownInsert
    public int insert(int col, Chip movedChip) {
        if (col < 0) return -1;
        int newRow = -1;
        for (int i = rows - 1; i >= 0; i--) {
            Vector2Int pos = new Vector2Int(i, col);
            if (!currentChips.ContainsKey(pos)) {
                currentChips.Add(pos, movedChip);
                movedChips.Enqueue(pos);
                newRow = i;
                break;
            }
        }
        return newRow;
    }

    public void CheckForWins() {
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
                            currentChips[newLocation] = chip;
                            movedChips.Enqueue(newLocation);
                        }
                    }
                }
            }
        }
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
        for (int row = pos.x+1; row < rows; row++) { //bottom-left diag
            for (int col = pos.y-1; col >= 0; col--) {
                if (currentChips.TryGetValue(new Vector2Int(row, col), out var neighborChip)) {
                    if (neighborChip.GetColor() == sameColor) {
                        matchingChips.Add(new Vector2Int(row, col));
                    } else {
                        break; //different color neighbor chip
                    }
                } else {
                    break; //no neighboring chips
                }
            }
        }
        for (int row = pos.x-1; row >= 0; row--) { //top-right diag
            for (int col = pos.y+1; col < cols; col++) {
                if (currentChips.TryGetValue(new Vector2Int(row, col), out var neighborChip)) {
                    if (neighborChip.GetColor() == sameColor) {
                        matchingChips.Add(new Vector2Int(row, col));
                    } else {
                        break; //different color neighbor chip
                    }
                } else {
                    break; //no neighboring chips
                }
            }
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
        for (int row = pos.x+1; row < rows; row++) { //bottom-right diag
            for (int col = pos.y+1; col < cols; col++) {
                if (currentChips.TryGetValue(new Vector2Int(row, col), out var neighborChip)) {
                    if (neighborChip.GetColor() == sameColor) {
                        matchingChips.Add(new Vector2Int(row, col));
                    } else {
                        break; //different color neighbor chip
                    }
                } else {
                    break; //no neighboring chips
                }
            }
        }
        for (int row = pos.x-1; row >= 0; row--) { //top-left diag
            for (int col = pos.y-1; col >= 0; col--) {
                if (currentChips.TryGetValue(new Vector2Int(row, col), out var neighborChip)) {
                    if (neighborChip.GetColor() == sameColor) {
                        matchingChips.Add(new Vector2Int(row, col));
                    } else {
                        break; //different color neighbor chip
                    }
                } else {
                    break; //no neighboring chips
                }
            }
        }
        if (matchingChips.Count >= 4) {
            return matchingChips;
        }
        matchingChips.Clear(); //change
        return matchingChips;
    }
}
