using System.Collections;
using UnityEngine;

[System.Serializable]
public class TreeClass
{
    public string name;
    [Range(0, 1)]
    public float rarity;
    [Range(0, 1)]
    public float size;
    [Range(0, 1)]
    public float scattering;
    public Texture2D spreadTexture;
}
