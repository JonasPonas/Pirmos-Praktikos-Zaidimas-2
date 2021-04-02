using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundTile
{
    public Sprite sprite;
    public string name;

    public int minDepth = 0;
    public float percentage = 1;

    public int value = 1;

    public GroundTile() { }
    public GroundTile(Sprite sprite, string name){
        this.sprite = sprite;
        this.name = name;
    }
}

public class Tile : MonoBehaviour
{
    public GroundTile tile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
