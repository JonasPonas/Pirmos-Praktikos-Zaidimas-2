using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Menu : MonoBehaviour
{
    public Button newGame, save, load, scoreBoard, scoreBoardExit;
    public GameObject scores;

    // Start is called before the first frame update
    void Start()
    {
        //SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        newGame.onClick.AddListener(() => SceneManager.LoadScene("Pirma", LoadSceneMode.Single));
        load.onClick.AddListener(Load);
        if (!System.IO.File.Exists("save1.txt")) load.interactable = false;
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Pirma")) save.interactable = false;
        else
        {
            save.onClick.AddListener(Save);
        }

        scoreBoard.onClick.AddListener(() => scores.SetActive(true));
        scoreBoardExit.onClick.AddListener(() => scores.SetActive(false));

        transform.Find("HighScore").GetComponent<Text>().text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0);
    }

    void Save()
    {
        StreamWriter writer = new StreamWriter("save1.txt", false);
        //writer.WriteLine("Saved map:");

        GameObject[] goArray = SceneManager.GetActiveScene().GetRootGameObjects();
        if (goArray.Length > 0)
        {
            Transform player = goArray[3].transform;
            writer.WriteLine("Player pos: " + player.position.x + " " + player.position.y);
            writer.WriteLine("Fuel: " + player.GetComponent<PlayerController>().fuel);
            writer.WriteLine("Cash: " + player.GetComponent<PlayerController>().cash);
            writer.WriteLine("Inventory: ");
            int items = 0;
            Text[] texts = player.GetComponent<PlayerController>().invTexts;
            foreach (Text invText in texts)
                if (invText.gameObject.activeSelf) items++;

            writer.WriteLine(items);
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].gameObject.activeSelf)
                {
                    string[] firstStep = texts[i].text.Split(new string[] { ": " }, System.StringSplitOptions.None);
                    string[] secondStep = firstStep[1].Split(' ');
                    writer.WriteLine(firstStep[0] + " " + secondStep[0]);
                }
            }

            writer.WriteLine("Saved map:");
            GameObject go = goArray[1];
            go = go.transform.GetChild(0).gameObject;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                if (go.transform.GetChild(i).gameObject.activeSelf)
                {
                    GroundTile tile = go.transform.GetChild(i).GetComponent<Tile>().tile;

                    writer.Write(tile.name[0]);
                }
                else
                    writer.Write("-");
                if ((i + 1) % 19 == 0)
                    writer.Write("\n");
            }
        }
        writer.Close();
    }

    void Load()
    {
        if (SceneManager.GetSceneByName("Pirma").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Pirma");
            SceneManager.LoadScene("Pirma", LoadSceneMode.Additive);
        }
        else
            SceneManager.LoadScene("Pirma", LoadSceneMode.Additive);
    }
}
