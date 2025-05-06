using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class SelectsArrays : MonoBehaviour
{
    public static List<GameObject> mineSprites;
    public static List<GameObject> mineSelected;

    public static List<GameObject> deconstructSprites;
    public static List<GameObject> deconstructSelected;

    public static List<GameObject> buildSprites;
    public static List<GameObject> buildSelected;

    [SerializeField] private GameObject _mineListObject, _deconstructListObject, _buildListObject;

    public static GameObject mineListObject { get; private set; }
    public static GameObject deconstructListObject { get; private set; }
    public static GameObject buildListObject { get; private set; }

    void Start()
    {

    }

    private void OnValidate()
    {
        mineListObject = _mineListObject;
        deconstructListObject = _deconstructListObject;
        buildListObject = _buildListObject;

        mineSprites = new List<GameObject>();
        mineSelected = new List<GameObject>();
        deconstructSprites = new List<GameObject>();
        deconstructSelected = new List<GameObject>();
        buildSprites = new List<GameObject>();
        buildSelected = new List<GameObject>();
    }

    void Update()
    {
        
    }
}
