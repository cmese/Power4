using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChip : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI chipText;
    [SerializeField] private Color chipColor;
    [SerializeField] private MeshRenderer chipRenderer;

    public void Init(string text) {
        chipRenderer.material.color = chipColor;
        chipText.text = text;
    }
}
