using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievments : MonoBehaviour
{
    private Text achievmentText;
    public PlayerController player;

    private Coroutine fadeOutRoutine;

    private bool[] achived;
    // Start is called before the first frame update
    void Start()
    {
        achievmentText = GetComponent<Text>();
        achived = new bool[10];
        fadeOutRoutine = StartCoroutine(FadeOutRoutine());
        StopCoroutine(fadeOutRoutine);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < player.inventory.Count; i++)
        {
            switch (player.inventory[i].name)
            {
                case "Ground": if (!achived[0]) StartMessage("New Achievment: First " + "Ground"); achived[0] = true; break;
                case "Copper": if (!achived[1]) StartMessage("New Achievment: First " + "Copper"); achived[1] = true; break;
                case "Iron": if (!achived[2]) StartMessage("New Achievment: First " + "Iron"); achived[2] = true; break;
                case "Uranium": if (!achived[3]) StartMessage("New Achievment: First " + "Uranium"); achived[3] = true; break;
                case "Titanium": if (!achived[4]) StartMessage("New Achievment: First " + "Titanium"); achived[4] = true; break;
                case "BedRock": if (!achived[5]) StartMessage("New Achievment: First " + "BedRock"); achived[5] = true; break;
                default: break;
            }
        }
    }
    void StartMessage(string message)
    {
        achievmentText.text = message;
        achievmentText.color = new Color(achievmentText.color.r, achievmentText.color.g, achievmentText.color.b);
        StopCoroutine(fadeOutRoutine);
        fadeOutRoutine = StartCoroutine(FadeOutRoutine());
    }
    private IEnumerator FadeOutRoutine()
    {
        Color originalColor = achievmentText.color;
        int fadeOutTime = 50;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            achievmentText.color = new Color(achievmentText.color.r, achievmentText.color.g, achievmentText.color.b, Mathf.Lerp(achievmentText.color.a, 0f, Mathf.Min(1, t / fadeOutTime)));
            //messageText.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeOutTime));
            //Debug.Log(m_Fading);
            yield return null;
        }
        //yield break;
    }
}
