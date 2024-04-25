using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject randomButton;
    [SerializeField] private GameObject readyButton;
    
    [Header("Player Settings")]
    [SerializeField] private GameObject[] playerShipPrefabs;
    [SerializeField] private GridManager playerGridManager;
    private List<GameObject> playerPlacedShips = new List<GameObject>();

    [Header("Enemy Settings")]
    [SerializeField] private GameObject[] enemyShipPrefabs;
    [SerializeField] private GridManager enemyGridManager;
    private List<GameObject> enemyPlacedShips = new List<GameObject>();

    [Header("Ship Placement Settings")]
    [SerializeField] private float yOffset = 0.04f; 
    [SerializeField] private float evenLengthAdditionalOffset = 0.05f; 
    [SerializeField] private float oddLengthAdditionalOffset = 0.1f; 

    private bool isPlayerTurn = true;
    
    public static GameManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        PlacePlayerShipsRandomly();
    }
    
    void OnEnable()
    {
        TileScript.OnMissileHit += HandleMissileHit;
    }
    
    void OnDisable()
    {
        TileScript.OnMissileHit -= HandleMissileHit;
    }
    
    private void HandleMissileHit(TileScript tileScript)
    {
        if (tileScript.IsOccupiedByEnemyShip)
        {
            tileScript.ChangeColor(Color.red);
            Debug.Log("Hit enemy ship!");
            
            if (CheckIfShipIsDestroyed(tileScript))
            {
                tileScript.ChangeColor(Color.black);
                ChangeShipTilesToDestroyed(tileScript);
                Debug.Log("Enemy ship destroyed!");
            }
            
            isPlayerTurn = true;
            Debug.Log("Player's turn continues.");
        }
        else
        {
            tileScript.ChangeColor(Color.grey);
            Debug.Log("Missed!");
            
            isPlayerTurn = false;
            Invoke(nameof(StartEnemyTurn), 1.0f); 
            Debug.Log("Switching to enemy's turn.");
        }
    }
    
    private void ChangeShipTilesToDestroyed(TileScript hitTile)
    {
        foreach (var tile in GetShipTiles(hitTile))
        {
            tile.ChangeColor(Color.black);
        }
    }


    private IEnumerable<TileScript> GetShipTiles(TileScript hitTile)
    {
        return new List<TileScript>(); 
    }
    
    private bool CheckIfShipIsDestroyed(TileScript hitTile)
    {
        return false;
    }
    
    public void RegisterHit(TileScript hitTile, bool isHit)
    {
        if (isHit)
        {
            Debug.Log("Hit registered on enemy ship!");
        }
        else
        {
            Debug.Log("Missed enemy ship.");
            isPlayerTurn = !isPlayerTurn;
        }
        
        if (!isPlayerTurn)
        {
            Invoke(nameof(AIPlayTurn), 1f);
        }
    }
    private void AIPlayTurn()
    {
        int x = Random.Range(0, playerGridManager.gridWidth);
        int y = Random.Range(0, playerGridManager.gridHeight);
        TileScript targetTile = playerGridManager.GetTileAt(x, y).GetComponent<TileScript>();
        
        targetTile.OnHit();
        
        isPlayerTurn = true;
    }
    
    public void OnReadyButtonPressed()
    {
        PlaceEnemyShipsRandomly();
        HideUIElements();
        HidePlayerShips();
        ResetEnemyTileColors();
        StartPlayerTurn();
    }

    private void HidePlayerShips()
    {
        foreach (var ship in playerPlacedShips)
        {
            ship.SetActive(false);
        }
    }
    
    private void ResetEnemyTileColors()
    {
        enemyGridManager.ResetTileColors();
    }
    
    public void StartPlayerTurn()
    {
        Debug.Log("Player's turn!");
        ToggleShipsVisibility(playerPlacedShips, true);
        ToggleShipsVisibility(enemyPlacedShips, false);
    }
    
    public void StartEnemyTurn()
    {
        Debug.Log("Enemy's turn!");
        ToggleShipsVisibility(playerPlacedShips, false);
        ToggleShipsVisibility(enemyPlacedShips, true);
        AIPlayTurn();
    }
    
    private void ToggleShipsVisibility(List<GameObject> ships, bool visible)
    {
        foreach (var ship in ships)
        {
            ship.SetActive(visible);
        }
    }
    private void LockPlayerShips()
    {
       //
    }

    private void HideUIElements()
    {
        randomButton.SetActive(false);
        readyButton.SetActive(false);
    }

    public void PlacePlayerShipsRandomly()
    {
        PlaceShipsRandomly(playerGridManager, ref playerPlacedShips, playerShipPrefabs, true);
    }

    private void PlaceEnemyShipsRandomly()
    {
        PlaceShipsRandomly(enemyGridManager, ref enemyPlacedShips, enemyShipPrefabs, false);
    }

    private void PlaceShipsRandomly(GridManager gridManager, ref List<GameObject> placedShipsList, GameObject[] shipPrefabsArray, bool isPlayer)
    {
        foreach (GameObject ship in placedShipsList)
        {
            Destroy(ship);
        }
        placedShipsList.Clear();
        gridManager.ResetGrid();
        gridManager.ResetTileColors();

        foreach (GameObject shipPrefab in shipPrefabsArray)
        {
            GameObject newShip = PlaceShip(gridManager, shipPrefab, isPlayer);
            if (newShip != null)
            {
                placedShipsList.Add(newShip);
            }
        }
    }

    private GameObject PlaceShip(GridManager gridManager, GameObject shipPrefab, bool isPlayer)
    {
        int maxAttempts = 100;
        while (maxAttempts > 0)
        {
            maxAttempts--;
            int shipLength = shipPrefab.GetComponent<ShipScript>().Length;
            bool isVertical = Random.value > 0.5f;
            int startX = Random.Range(0, gridManager.gridWidth - (isVertical ? 1 : shipLength));
            int startY = Random.Range(0, gridManager.gridHeight - (isVertical ? shipLength : 1));

            if (CanPlaceShip(gridManager, shipLength, startX, startY, isVertical))
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
                shipInstance.transform.localScale = new Vector3(4, 4, shipLength);
                shipInstance.transform.parent = gridManager.transform;
                
                if (!isPlayer)
                {
                    shipInstance.SetActive(false);
                    foreach (Vector2Int tile in occupiedTiles)
                    {
                        gridManager.GetTileAt(tile.x, tile.y).GetComponent<TileScript>().SetOccupiedByEnemyShip(true);
                    }
                    Debug.Log($"Placed enemy {shipPrefab.name} at tiles: {string.Join(", ", occupiedTiles)}");
                }
                
                return shipInstance;
            }
        }
        return null;
    }

    private bool CanPlaceShip(GridManager gridManager, int length, int startX, int startY, bool isVertical)
    {
        int bufferZone = 1;
        for (int x = startX - bufferZone; x <= startX + (isVertical ? bufferZone : length + bufferZone); x++)
        {
            for (int y = startY - bufferZone; y <= startY + (isVertical ? length + bufferZone : bufferZone); y++)
            {
                if (x < 0 || x >= gridManager.gridWidth || y < 0 || y >= gridManager.gridHeight || gridManager.IsTileOccupied(x, y))
                {
                    return false;
                }
            }
        }
        return true;
    }
}
