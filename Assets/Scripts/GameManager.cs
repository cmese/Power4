using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Time for mahfukin GameStates
    public static GameManager Instance;

    void Awake() {
        Instance = this;
    }

    void Start()
    {
    }

    void Update()
    {
    }
}
