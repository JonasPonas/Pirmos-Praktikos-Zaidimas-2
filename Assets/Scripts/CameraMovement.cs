using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = new Vector3(0 ,transform.position.y - player.transform.position.y, -10);
    }

    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        Vector3 newPos = new Vector3(0, player.transform.position.y, -10) + offset;
        if(newPos.y <= 0)
            transform.position = new Vector3(0, player.transform.position.y, -10) + offset;
    }
}
