using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : MonoBehaviour
{
    [SerializeField] private MeshRenderer chipRenderer;

    public void Init(Color chipColor) {
        chipRenderer.material.color = chipColor;
    }

    public Color GetColor() {
        return chipRenderer.material.color;
    }
}
