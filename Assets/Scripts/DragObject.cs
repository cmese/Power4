using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    private int prevCol, currentCol = -1;

    private PlayerChip playerChipScript;

    void Awake() {
        playerChipScript = GetComponent<PlayerChip>();
    }
    void OnMouseDown() {
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

    //should this be a coroutine itself? dont think it matters
    void OnMouseDrag() {
        transform.position = GetMouseAsWorldPoint() + mOffset;
        //TODO: because dividing the screen width into 1/7ths below, make sure gameboard is always within viewport (camera.fov)?
        if (Input.mousePosition.y > Screen.height*0.25f) {
            currentCol = (int)(Input.mousePosition.x/Screen.width*7); //good enough for now
        } else {
            currentCol = -1;
        }
        if (GameManager.Instance.State == GameState.PlayerTurn && prevCol != currentCol) {
            playerChipScript.UpdatePreviewChip(currentCol);
            MenuManager.Instance.UpdatePanel(currentCol);
            prevCol = currentCol;
        }
    }

    void OnMouseUp() {
        playerChipScript.UpdatePreviewChip(-1);
        playerChipScript.MoveChip(currentCol);
    }
}
