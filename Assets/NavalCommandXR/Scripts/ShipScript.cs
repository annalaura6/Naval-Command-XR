using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    public int shipLength = 2;
    public bool isVertical = true;
    public float xOffset = 0;
    public float zOffset = 0;

    private List<GameObject> _touchingTiles = new List<GameObject>();

    public void ClearTileList()
    {
        _touchingTiles.Clear();
        Debug.Log("Cleared tile list for ship: " + gameObject.name);
    }

    public Vector3 GetOffsetVector(Vector3 tilePosition)
    {
        Vector3 offsetVector = new Vector3(tilePosition.x + xOffset, 0, tilePosition.z + zOffset);
        Debug.Log("Offset vector calculated for " + gameObject.name + ": " + offsetVector);
        return offsetVector;
    }

    public List<GameObject> GetHighlightTiles(GameObject currentTile)
    {
        List<GameObject> tilesToHighlight = new List<GameObject>();

        // Get the position of the current tile
        Vector3 currentPosition = currentTile.transform.position;

        // Add the current tile to the list
        tilesToHighlight.Add(currentTile);

        // Calculate positions of subsequent tiles based on shipLength and isVertical
        for (int i = 1; i < shipLength; i++)
        {
            Vector3 offset = isVertical ? Vector3.forward * i : Vector3.right * i;
            Vector3 nextPosition = currentPosition + offset;
            GameObject nextTile = GetTileAtPosition(nextPosition);
            if (nextTile != null)
            {
                tilesToHighlight.Add(nextTile);
            }
        }

        return tilesToHighlight;
    }

    private GameObject GetTileAtPosition(Vector3 position)
    {
        // Perform a raycast to find the tile at the given position
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Tile"))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    public void AddTouchingTile(GameObject tile)
    {
        if (!_touchingTiles.Contains(tile))
        {
            _touchingTiles.Add(tile);
            Debug.Log("Tile " + tile.name + " added to touching tiles list for ship: " + gameObject.name);
        }
    }

    public void RemoveTouchingTile(GameObject tile)
    {
        if (_touchingTiles.Contains(tile))
        {
            _touchingTiles.Remove(tile);
            Debug.Log("Tile " + tile.name + " removed from touching tiles list for ship: " + gameObject.name);
        }
    }
}
