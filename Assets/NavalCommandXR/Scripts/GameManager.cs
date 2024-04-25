using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] shipPrefabs;
    [SerializeField] private GridManager gridManager;
    private List<GameObject> placedShips = new List<GameObject>();
    
    [Header("Ship Placement Settings")]
    [SerializeField] private float yOffset = 0.04f; 
    [SerializeField] private float evenLengthAdditionalOffset = 0.05f; 
    [SerializeField] private float oddLengthAdditionalOffset = 0.1f; 

    void Start()
    {
        PlaceShipsRandomly();
    }

    public void PlaceShipsRandomly()
    {
        foreach (GameObject ship in placedShips)
        {
            Destroy(ship);
        }
        placedShips.Clear();
        
        gridManager.ResetGrid();
        gridManager.ResetTileColors();

        foreach (GameObject shipPrefab in shipPrefabs)
        {
            PlaceShip(shipPrefab);
        }
    }

    private void PlaceShip(GameObject shipPrefab)
{
    bool shipPlaced = false;
    int maxAttempts = 100;

    while (!shipPlaced && maxAttempts > 0)
    {
        maxAttempts--;
        int shipLength = shipPrefab.GetComponent<ShipScript>().Length;
        bool isVertical = Random.value > 0.5f;

        int maxStartX = gridManager.gridWidth - (isVertical ? 1 : shipLength);
        int maxStartY = gridManager.gridHeight - (isVertical ? shipLength : 1);

        int startX = Random.Range(0, maxStartX + 1);
        int startY = Random.Range(0, maxStartY + 1);

        if (CanPlaceShip(shipLength, startX, startY, isVertical))
        {
            List<Vector2Int> occupiedTiles = new List<Vector2Int>();
            for (int i = 0; i < shipLength; i++)
            {
                int tileX = startX + (isVertical ? 0 : i);
                int tileY = startY + (isVertical ? i : 0);
                gridManager.OccupyTile(tileX, tileY);
                occupiedTiles.Add(new Vector2Int(tileX, tileY));
                gridManager.HighlightTile(tileX, tileY, true);
            }
            
            float middleOffset = (shipLength % 2 == 0) ? (gridManager.tileSize * 0.5f) : 0f;
            float additionalOffset = (shipLength % 2 == 0) ? evenLengthAdditionalOffset : oddLengthAdditionalOffset;

            Vector3 positionOffset = isVertical
                ? new Vector3(0, yOffset, middleOffset - additionalOffset)
                : new Vector3(middleOffset - additionalOffset, yOffset, 0);

            Vector3 shipPosition = gridManager.GetTileAt(startX, startY).transform.position + positionOffset;

            Quaternion shipRotation = isVertical ? Quaternion.Euler(-90, 0, 0) : Quaternion.Euler(-90, 90, 0);
            GameObject shipInstance = Instantiate(shipPrefab, shipPosition, shipRotation);
            shipInstance.transform.localScale = new Vector3(4, 4, 1 * shipLength);
            shipInstance.transform.parent = this.transform;
            placedShips.Add(shipInstance);

            Debug.Log($"Placed {shipPrefab.name} at tiles: {string.Join(", ", occupiedTiles)}");

            shipPlaced = true;
        }
    }
}



    private bool CanPlaceShip(int length, int startX, int startY, bool isVertical)
    {
        int bufferZone = 1;
        int startBufferX = isVertical ? startX - bufferZone : startX - bufferZone;
        int endBufferX = isVertical ? startX + bufferZone : startX + length + bufferZone;
        int startBufferY = isVertical ? startY - bufferZone : startY - bufferZone;
        int endBufferY = isVertical ? startY + length + bufferZone : startY + bufferZone;
        
        for (int x = startBufferX; x <= endBufferX; x++)
        {
            for (int y = startBufferY; y <= endBufferY; y++)
            {
                if (x < 0 || x >= gridManager.gridWidth || y < 0 || y >= gridManager.gridHeight || !gridManager.IsTileFree(x, y))
                {
                    return false; 
                }
            }
        }
        return true; 
    }
}
