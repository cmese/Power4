using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public MenuStateMachine stateMachine {get; private set;}
    [SerializeField] private TMPro.TextMeshProUGUI infoText;
    [SerializeField] private MenuChipManager menuChipPrefab;
    [SerializeField] private ChipManager dummyChipPrefab;

    public void Init() {
        stateMachine = new MenuStateMachine();
        stateMachine.ChangeState(new MainMenuState(this));
    }

    public void MenuDictToBoard(Dictionary<int, string[]> menuDict) {
        Gameboard currentBoard = GameManager.Instance.GetGameboard();
        currentBoard.ClearBoard();
        for (int col = 0; col < 7; col++) {
            int rowIndex = 5;
            if (menuDict.TryGetValue(col, out string[] menuStrings)) {
                for (int i = menuStrings[0].Length - 1; i >= 0; i--) {
                    var menuChip = Instantiate(menuChipPrefab, new Vector3(col+0.5f, -rowIndex-0.5f, -0.75f), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    menuChip.SetColor(GameManager.Instance.GetPlayerChipColor());
                    menuChip.SetText(menuStrings[0][i].ToString());
                    currentBoard.AddChip(new Vector2Int(rowIndex, col), menuChip);
                    rowIndex--;
                }
            } else {
                for (int row = rowIndex; row >= 0; row--) {
                    var dummyChip = Instantiate(dummyChipPrefab, new Vector3(col+0.5f, -row-0.5f, -0.75f), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    dummyChip.SetColor(Color.black);
                    currentBoard.AddChip(new Vector2Int(row, col), dummyChip);
                }
            }
        }
    }
}
