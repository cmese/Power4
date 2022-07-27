using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChip : Chip
{
    [SerializeField] private TMPro.TextMeshProUGUI chipText;

    public void SetText(string text) {
        chipText.text = text;
    }
}
