using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer chipRenderer;

    public StateMachine stateMachine {get; private set;}
    public Queue<IState> stateQueue {get; private set;}

    void Awake() {
        stateMachine = new StateMachine();
        stateQueue = new Queue<IState>();
    }

    public void SetColor(Color color) {
        chipRenderer.material.color = color;
    }
    public Color GetColor() {
        return chipRenderer.sharedMaterial.color;
    }

    public void SetOpacity(float alphaVal) {
        var currentColor = GetColor();
        currentColor.a = alphaVal;
        SetColor(currentColor);
    }
}
