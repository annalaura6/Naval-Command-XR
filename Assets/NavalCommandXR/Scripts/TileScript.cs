using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    private GameManager _gameManager;
    private Renderer _renderer;
    
    void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        if (_gameManager == null) {
            Debug.LogError("GameManager not found in the scene!");
        }
        _renderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship"))
        {
            ShipScript ship = other.GetComponent<ShipScript>();
            if (ship != null)
            {
                // This method will handle which tiles should be highlighted
                List<GameObject> tilesToHighlight = ship.GetHighlightTiles(gameObject);

                foreach (GameObject tile in tilesToHighlight)
                {
                    // Assuming you have a method in TileScript to highlight itself.
                    tile.GetComponent<TileScript>().HighlightTile(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ship"))
        {
            Debug.Log("Ship exited tile: " + gameObject.name);
            HighlightTile(false);
        }
    }

    public void HighlightTile(bool highlight)
    {
        Color color = highlight ? Color.green : Color.white;
        _renderer.material.color = color;
        Debug.Log("Tile " + gameObject.name + (highlight ? " highlighted." : " unhighlighted."));
    }
}