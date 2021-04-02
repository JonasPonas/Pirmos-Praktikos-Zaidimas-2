using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBars : MonoBehaviour
{
    public Transform foregroundFuel;
    public GameObject player;
    
    private PlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(1).transform.localScale = new Vector2(controller.fuel, transform.GetChild(1).transform.localScale.y);
    }
}
