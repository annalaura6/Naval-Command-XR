using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _ships;
    private bool _setupComplete = false;
    private bool _playerTurn = false;
    private int _shipNum = 0;
    private ShipScript _shipScript;

    void Start()
    {
        _shipScript = _ships[_shipNum].GetComponent<ShipScript>();
    }

    public void TileHighlight(GameObject tile)
    {
        if (_setupComplete && _playerTurn)
        {
            // drop a missile
        }
        else if (!_setupComplete)
        {
            PlaceShip(tile);
        }
    }

    private void PlaceShip(GameObject tile)
    {
        _shipScript = _ships[_shipNum].GetComponent<ShipScript>();
        _shipScript.ClearTileList();
        Vector3 newVector3 = _shipScript.GetOffsetVector(tile.transform.position);
        _ships[_shipNum].transform.localPosition = newVector3;

        // Highlight tiles for ship placement
        List<GameObject> tilesToHighlight = _shipScript.GetHighlightTiles(tile);
        foreach (GameObject highlightedTile in tilesToHighlight)
        {
            highlightedTile.GetComponent<TileScript>().HighlightTile(true);
        }
    }
}