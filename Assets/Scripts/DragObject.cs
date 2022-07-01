using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    [SerializeField]
    private GameObject gameBoard; //SWITCH THIS TO GAMEBOARD MANAGER NEXT, chips should become children of gameboard because we need to call enumerator for all chips at once when updating the board, also so we can instantiate this bitch from there

    private Vector3 mOffset;
    private float mZCoord;

    private int prevCol, currentCol = -1;
    [SerializeField]
    private GameObject previewChipPrefab;
    private Quaternion chipRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    private GameObject previewChip;
    private Vector3 columnStart;

    [SerializeField]
    private float speedToTop = 0.5f;
    [SerializeField]
    private float speedDownBoard = 0.7f;
    [SerializeField]
    private float speedBackToStart = 0.7f;

    private float target = 1;
    private float current;

    private Vector3 startPos;

    void Start() {
        startPos = gameObject.transform.position;
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
        if (prevCol != currentCol) {
            if (previewChip) Destroy(previewChip);
            if (currentCol > -1) {
                previewChip = Instantiate(previewChipPrefab, new Vector3(currentCol+0.5f, 1-0.5f, -0.75f), chipRotation);
                previewChip.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;
            }
            prevCol = currentCol;
        }
    }

    void OnMouseUp() {
        if (previewChip) Destroy(previewChip);
        StartCoroutine(MoveChip());
    }

    IEnumerator MoveChip() {
        int newRow = gameBoard.GetComponent<Gameboard>().insert(currentCol);
        if (newRow > -1) {
            yield return StartCoroutine(MoveToGameBoard(newRow));
        } else {
            yield return StartCoroutine(MoveToStart());
        }
    }

    IEnumerator MoveToGameBoard(int newRow) {
        print("Going towards preview chip");
        yield return StartCoroutine(MoveToTop());
        print("Going down the board");
        yield return StartCoroutine(MoveDownBoard(newRow));
        print("Finished moving chip........for now");
    }

    IEnumerator MoveToTop() {
        Vector3 startLerpPos = gameObject.transform.position;
        Vector3 finalLerpPos = new Vector3(currentCol+0.5f, 1-0.5f, -0.75f);
        while (current < 1) {
            current = Mathf.MoveTowards(current, target, speedToTop * Time.deltaTime);
            gameObject.transform.position = Vector3.Lerp(startLerpPos, finalLerpPos, current);
            yield return null;
        }
        current = 0;
    }

    IEnumerator MoveDownBoard(int newRow) {
        Vector3 startLerpPos = gameObject.transform.position;
        Vector3 finalLerpPos =  new Vector3(startLerpPos.x, -newRow-0.5f, startLerpPos.z);
        while (current < 1) {
            current = Mathf.MoveTowards(current, target, speedDownBoard * Time.deltaTime);
            gameObject.transform.position = Vector3.Lerp(startLerpPos, finalLerpPos, current);
            yield return null;
        }
        current = 0;
    }

    IEnumerator MoveToStart() {
        Vector3 startLerpPos = gameObject.transform.position;
        Vector3 finalLerpPos = startPos;
        while (current < 1) {
            current = Mathf.MoveTowards(current, target, speedBackToStart * Time.deltaTime);
            gameObject.transform.position = Vector3.Lerp(startLerpPos, finalLerpPos, current);
            yield return null;
        }
        current = 0;
    }
}
