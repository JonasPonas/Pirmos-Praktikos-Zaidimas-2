using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{

    public GameObject openText;

    // Start is called before the first frame update
    void Start()
    {
        openText = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D Other)
    {
        openText.SetActive(true);
    }
    void OnTriggerExit2D(Collider2D Other)
    {
        openText.SetActive(false);
    }
}
