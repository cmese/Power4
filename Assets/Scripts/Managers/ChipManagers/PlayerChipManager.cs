using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChipManager : ChipManager {
    private Vector3 mOffset;
    private float mZCoord;
    private int prevCol = -1;
    public int currentCol {get; private set;}

    private Vector3 startPos;

    void Start() {
        stateMachine.ChangeState(new IdleState(this));
        //stateQueue.Enqueue(new IdleState(this));
        //stateMachine.ChangeState(stateQueue.Dequeue());
        startPos = gameObject.transform.position;
        currentCol = -1;
    }

    void Update() {
        stateMachine.Update();
    }

    void OnMouseDown() {
        stateMachine.ChangeState(new DragState(this));
        //TODO:move position so that finger/mouse is always directly in the center
        //TODO:move position back a little bit
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint() {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void OnMouseDrag() {
        transform.position = GetMouseAsWorldPoint() + mOffset;
        //TODO: because dividing the screen width into 1/7ths below, make sure gameboard is always within viewport (camera.fov)?
        if (Input.mousePosition.y > Screen.height*0.25f) {
            currentCol = (int)(Input.mousePosition.x/Screen.width*7); //good enough for now
        } else {
            currentCol = -1;
        }
        if (prevCol != currentCol) {
            //update preview chip
            //update panel info if menu
            prevCol = currentCol;
        }
    }

    void OnMouseUp() {
        //TODO: remove previewchip
        //try to insert
        int newRow = GameManager.Instance.AddToBoard(currentCol, this); //returns -1 if not players turn or invalid move
        if (newRow > -1) {
            gameObject.GetComponent<Collider>().enabled = false;
            stateQueue.Enqueue(new MoveState(this, new Vector3(currentCol+0.5f, 1-0.5f, -0.75f), 1.0f)); //move to top
            stateQueue.Enqueue(new MoveState(this, new Vector3(currentCol+0.5f, -newRow-0.5f, -0.75f), 1.0f)); //move down board
            stateQueue.Enqueue(new InBoardState(this));
        } else {
            stateQueue.Enqueue(new MoveState(this, startPos, 1.0f));
            stateQueue.Enqueue(new IdleState(this));
        }
        stateMachine.ChangeState(stateQueue.Dequeue());
    }
}
