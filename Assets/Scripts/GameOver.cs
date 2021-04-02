using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private Button menu;
    // Start is called before the first frame update
    void Start()
    {
        menu = transform.GetChild(1).GetComponent<Button>();
        menu.onClick.AddListener(() => SceneManager.LoadScene("Menu", LoadSceneMode.Single));
    }

    private void Awake()
    {
        Debug.Log("GameOver was activated!");
    }
}
