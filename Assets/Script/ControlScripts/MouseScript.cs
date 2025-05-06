using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static TerrainGenerator;

public class MouseScript : MonoBehaviour
{
    public static bool selected = false;
    public static List<GameObject> selectedList;
    private int previous = 0;
    public static bool MouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    void Start()
    {
        selectedList = new List<GameObject>();
    }
    public static void ClearList()
    {
        foreach (GameObject obj in selectedList.ToList())
        {
            if (obj != false)
                obj.GetComponent<Selectable>().Deselect();
            else
                selectedList.Remove(obj);
        }
        selectedList.Clear();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
        if (selectedList.Count > 1 && Input.GetMouseButtonDown(0) && ShiftAndCtrl() && !MouseOverUI())
        {
            ClearList();
        }
        else if (selectedList.Count == 1 && Input.GetMouseButtonDown(0) && ShiftAndCtrl() && !MouseOverUI())
        {
            if (previous > 0)
            {
                selectedList[0].GetComponent<Selectable>().Deselect();
                selectedList.Clear();
            }
        }
        previous = selectedList.Count;
        selected = selectedList.Count == 1;
    }
    public static Vector2 MouseToGrid() => new Vector2((int)Camera.main.ScreenToWorldPoint(Input.mousePosition.ConvertTo<Vector3>()).ConvertTo<Vector2>().x, (int)Camera.main.ScreenToWorldPoint(Input.mousePosition.ConvertTo<Vector3>()).ConvertTo<Vector2>().y);
    public static bool ShiftAndCtrl()
    {
        return !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl);
    }
}
