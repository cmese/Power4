using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer chipRenderer;

    public void SetColor(Color color) {
        chipRenderer.material.color = color;
    }
    public Color GetColor() {
        return chipRenderer.material.color;
    }
}
