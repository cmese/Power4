using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChipManager : ChipManager
{
    [SerializeField] private TMPro.TextMeshProUGUI chipText;

    public void SetText(string text) {
        chipText.text = text;
    }
}
