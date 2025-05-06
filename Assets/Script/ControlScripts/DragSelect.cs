using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
using static TerrainGenerator;
using static MouseScript;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DragSelect : MonoBehaviour
{
    private bool isDragSelect = false;

    private Vector2 mousePositionInitial;
    private Vector2 mousePositionInitialWorld;
    private Vector2 mousePositionEnd;
    private Vector2 mousePositionEndWorld;

    private GameObject[,] selectedWalls;

    public RectTransform selectionBox;

    private void Start()
    {
        selectedWalls = new GameObject[mapWidth, mapHeight];
    }
    // Update is called once per frame
    void Update()
    {
        mousePositionEnd = Input.mousePosition.ConvertTo<Vector2>();
        if (Input.GetMouseButtonDown(0))
        {
            mousePositionInitial = Mouse.current.position.ReadValue();
            isDragSelect = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (/*!isDragSelect &&*/ (mousePositionInitial - Input.mousePosition.ConvertTo<Vector2>()).magnitude > 30)
            {
                isDragSelect = true;
                UpdateSelectionBox();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragSelect)
            {
                isDragSelect = false;
                UpdateSelectionBox();
            }
        }
    }
    private void ToWorldPoint()
    {
        mousePositionInitialWorld = Camera.main.ScreenToWorldPoint(mousePositionInitial.ConvertTo<Vector3>()).ConvertTo<Vector2>();
        mousePositionEndWorld = Camera.main.ScreenToWorldPoint(mousePositionEnd.ConvertTo<Vector3>()).ConvertTo<Vector2>();

        mousePositionInitialWorld.x = Mathf.RoundToInt(mousePositionInitialWorld.x);
        mousePositionInitialWorld.y = Mathf.RoundToInt(mousePositionInitialWorld.y);

        mousePositionEndWorld.x = Mathf.RoundToInt(mousePositionEndWorld.x);
        mousePositionEndWorld.y = Mathf.RoundToInt(mousePositionEndWorld.y);

        if (mousePositionEndWorld.x < mousePositionInitialWorld.x)
            MathBr.Swap(ref mousePositionEndWorld.x, ref mousePositionInitialWorld.x);
        if (mousePositionEndWorld.y < mousePositionInitialWorld.y)
            MathBr.Swap(ref mousePositionEndWorld.y, ref mousePositionInitialWorld.y);
    }
    void UpdateSelectionBox()
    {
        //mousePositionEnd = Input.mousePosition.ConvertTo<Vector2>();
        mousePositionEnd = Mouse.current.position.ReadValue();
        selectionBox.gameObject.SetActive(isDragSelect);

        float width = mousePositionEnd.x - mousePositionInitial.x;
        float height = mousePositionEnd.y - mousePositionInitial.y;
        ToWorldPoint();

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height)) / gameObject.GetComponent<Canvas>().scaleFactor;
        selectionBox.anchoredPosition = (mousePositionInitial + new Vector2(width / 2, height / 2)) / gameObject.GetComponent<Canvas>().scaleFactor;

        /*Vector2 size = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.sizeDelta = size;
        Vector2 center = mousePositionInitial + new Vector2(width / 2, height / 2);
        selectionBox.position = center;*/


        SelectBlocksInBox();
    }
    private void SelectBlocksInBox()
    {
        if (ShiftAndCtrl())
            ClearList();
        for (int x = Mathf.RoundToInt(mousePositionInitialWorld.x); x < mousePositionEndWorld.x; x++)
        {
            for (int y = Mathf.RoundToInt(mousePositionInitialWorld.y); y < mousePositionEndWorld.y; y++)
            {
                if (CheckArrayLimits(x, y))
                    continue;

                selectedWalls[x, y] = walls[x, y];

                selectedList.Add(selectedWalls[x,y]);
                if (!ReferenceEquals(selectedWalls[x, y], null))
                    selectedWalls[x, y].GetComponent<WallScript>().Select();
            }
        }
    }
}