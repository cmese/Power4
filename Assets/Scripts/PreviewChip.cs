using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewChip : MonoBehaviour
{
    [SerializeField] private Color chipColor;
    [SerializeField] private MeshRenderer chipRenderer;

    public void Init(Color chipColor) {
        chipRenderer.material.color = chipColor;
    }
}
