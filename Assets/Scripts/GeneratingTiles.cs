using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class GeneratingTiles : MonoBehaviour
{

    //public Texture2D tex;
    public Sprite ground_1, ground_2, ground_3, bedrock, lava;
    public GroundTile[] tiles;
    private Camera cam;

    public GameObject ground;
    public GameObject groundTilesGroup;

    private int pixelsToUnit = 100;

    public int tileCount = 10;

    private StreamWriter writer;
    private StreamReader reader;

    private bool loadSave = false;

    // Use this for initialization
    void Start()
    {

        cam = Camera.main;

        //new GameObject("Ground").AddComponent<SpriteRenderer>().sprite = ground_1;
        Sprite currentSprite = ground_1;
        Vector2 spriteSize = new Vector2((float)ground_1.rect.width / pixelsToUnit, (float)ground_1.rect.width / pixelsToUnit);
        Vector3 offset = new Vector3(spriteSize.x / 2, spriteSize.y / 2, 0) + new Vector3(0, spriteSize.y * 2, 0);

        Vector3 newPos = cam.ScreenToWorldPoint(new Vector3(0, 0, 10)) + offset;

        writer = new StreamWriter("map1.txt", false);
        writer.WriteLine("Map:");

        reader = new StreamReader("save1.txt");
        for (int i = 0; i < 4; i++)
            reader.ReadLine();
        int times = int.Parse(reader.ReadLine());
        //Debug.Log(times);
        for (int i = 0; i < times + 1; i++)
            reader.ReadLine();
        //if (reader.ReadLine() != "Save map:") break;

        if (SceneManager.GetSceneByName("Menu").isLoaded) loadSave = true;
        string line = "";

        for (int i = 0; i <= tileCount + 10; i++)
        {
            if (loadSave) line = reader.ReadLine();
            int j = 0;
            while (newPos.x + spriteSize.x / 2 < Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x)
            {
                newPos = cam.ScreenToWorldPoint(new Vector3(0, 0, 10)) + offset;

                if (!loadSave)
                {
                    GroundTile tmp = null;
                    if (i > tileCount)
                        tmp = GenerateTile(new GroundTile(lava, "Lava"), newPos).tile;
                    else if (i == tileCount)
                        tmp = GenerateTile(new GroundTile(bedrock, "BedRock"), newPos).tile;
                    else
                        tmp = GenerateTile(GenerateSprite(i), newPos).tile;

                    switch (tmp.name)
                    {
                        case "Ground": writer.Write("G"); break;
                        case "Lava": writer.Write("L"); break;
                        case "BedRock": writer.Write("B"); break;
                        default: break;
                    }
                }
                else
                {
                    SceneManager.UnloadSceneAsync("Menu");
                    bool exists = false;
                    foreach (GroundTile tileInArray in tiles)
                    {
                        if (tileInArray.name[0] == line[j])
                        {
                            GenerateTile(tileInArray, newPos);
                            exists = true;
                        }
                    }
                    if (!exists)
                    {
                        switch (line[j])
                        {
                            case 'G':
                                {
                                    if (i == 0) GenerateTile(new GroundTile(ground_1, "Ground"), newPos);
                                    else if (i == 1) GenerateTile(new GroundTile(ground_2, "Ground"), newPos);
                                    else GenerateTile(new GroundTile(ground_3, "Ground"), newPos);
                                    break;
                                }
                            case 'L': GenerateTile(new GroundTile(lava, "Lava"), newPos); break;
                            case 'B': GenerateTile(new GroundTile(bedrock, "BedRock"), newPos); break;
                            case '-': GenerateTile(new GroundTile(ground_3, "Ground"), newPos).gameObject.SetActive(false); break;
                            default: break;
                        }
                    }
                    j++;
                }


                offset.x += spriteSize.x;
            }
            writer.Write("\n");

            offset.x = spriteSize.x / 2;
            offset.y -= spriteSize.y;
            newPos = cam.ScreenToWorldPoint(new Vector3(0, 0, 10)) + offset;
        }
        writer.Close();
        reader.Close();
    }

    Tile GenerateTile(GroundTile tile, Vector3 pos)
    {
        GameObject go = Instantiate(ground);
        go.GetComponent<SpriteRenderer>().sprite = tile.sprite;
        go.GetComponent<Tile>().tile = tile;
        go.transform.SetParent(groundTilesGroup.transform);

        //Debug.Log("width = " + (float)ground_1.rect.width / pixelsToUnit);

        go.transform.position = pos;

        return go.GetComponent<Tile>();
    }

    GroundTile GenerateSprite(int depth)
    {
        GroundTile tile = new GroundTile(ground_3, "Ground");

        if (depth < 3)
        {
            switch (depth)
            {
                case 0: tile.sprite = ground_1; break;
                case 1: tile.sprite = ground_2; break;
                case 2: tile.sprite = ground_3; break;
                default: break;
            }
        }
        else
        {
            foreach (GroundTile tileInArray in tiles)
            {
                if (tileInArray.minDepth <= depth)
                {
                    if (tileInArray.percentage >= Random.Range(0.0f, 1.0f))
                    {
                        tile = tileInArray;
                        writer.Write(tileInArray.name[0]);
                        break;
                    }
                }
            }
        }

        return tile;
    }
}
