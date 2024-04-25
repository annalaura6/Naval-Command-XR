using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TileScript tile = other.GetComponent<TileScript>();
        if (tile != null)
        {
            // This is a tile. You can check if it's part of the enemy grid and handle the touch accordingly.
            Debug.Log("Touched tile: " + other.gameObject.name);
        }
    }
}

