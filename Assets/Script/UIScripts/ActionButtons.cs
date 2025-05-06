using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static MouseScript;
using static SelectsArrays;

public class ActionButtons : MonoBehaviour
{
    private Sprite mineSprite;


    [SerializeField] private Button mineButton;

    [SerializeField] private Button[] downBarButtons;
    [SerializeField] private GameObject[] panels;

    [SerializeField] private Button[] architectButtons;
    [SerializeField] private GameObject[] architectPanels;

    public delegate void ButtonDelegate(int index);

    private void Awake()
    {
        
    }
    void Start()
    {
        mineSprite = Resources.Load<Sprite>("Sprites/Tiles/Orders/MineSprite");
        mineButton.onClick.AddListener(MineSelected);
        for (int i = 0; i < panels.Length; i++)
        {
            var i1 = i;
            downBarButtons[i].onClick.AddListener(new UnityAction(() => { ActivatePanelByIndex(i1); }));
        }
        for (int i = 0; i < architectPanels.Length; i++)
        {
            var i1 = i;
            architectButtons[i].onClick.AddListener(new UnityAction(()=> { ActivateArchitectPanelByIndex(i1); }));
        }
    }
    void MineSelected()
    {
        foreach (GameObject obj in selectedList)
        {
            if (obj != null)
            {
                if (mineSelected.Contains(obj) || obj.GetComponent<WallScript>().block.mineable == false)
                    continue;

                GameObject selectSprite = new();
                selectSprite.name = "MineThis";
                selectSprite.AddComponent<SpriteRenderer>();
                selectSprite.GetComponent<SpriteRenderer>().sprite = mineSprite;
                selectSprite.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";
                selectSprite.transform.parent = mineListObject.transform;
                selectSprite.transform.position = obj.transform.position;

                mineSprites.Add(selectSprite.gameObject);
                mineSelected.Add(obj);
            }
        }
    }
    public void ActivatePanelByIndex(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == index)
                continue;
            panels[i].SetActive(false);
        }
        panels[index].gameObject.SetActive(!panels[index].gameObject.activeSelf);
    }
    public void ActivateArchitectPanelByIndex(int index)
    {
        Debug.Log(index);
        for (int i = 0; i < architectPanels.Length; i++)
        {
            architectPanels[i].SetActive(false);
        }
        architectPanels[index].gameObject.SetActive(true);
    }
    public void ButtonClickGeneric(GameObject obj)
    {

    }
    void Update()
    {
        
    }
}
