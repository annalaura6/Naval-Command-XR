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
    [SerializeField] private float evenLengthAdditionalOffset = -0.05f; 
    [SerializeField] private float oddLengthAdditionalOffset = -0.08f; 

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
        ShipScript.OnShipDestroyed += HandleShipDestroyed;
        PlacePlayerShipsRandomly();
        PlaceEnemyShipsRandomly();
    }
    
    public void OnReadyButtonPressed()
    {
        PlaceEnemyShipsRandomly();
        HideUIElements();
        HidePlayerShips();
        ResetEnemyTileColors();
        StartPlayerTurn();
    }
    
    void OnEnable()
    {
        TileScript.OnMissileHit += HandleMissileHit;
    }
    
    void OnDisable()
    {
        TileScript.OnMissileHit -= HandleMissileHit;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        ShipScript.OnShipDestroyed -= HandleShipDestroyed;
    }

    private void HandleShipDestroyed(ShipScript destroyedShip)
    {
        // Handle the ship destruction, maybe by updating the UI or ending the game
        Debug.Log("Ship destroyed: " + destroyedShip.gameObject.name);
    }
    
   private void HandleMissileHit(TileScript tileScript)
   {
       bool hit = tileScript.IsOccupiedByEnemyShip;
       tileScript.ChangeColor(hit ? Color.red : Color.grey);
       Debug.Log(hit ? "Hit!" : "Miss!");
       isPlayerTurn = !hit; // If hit, the player continues, otherwise, it's AI's turn
       
       // Proceed based on hit or miss
       if (hit) {
           if (CheckIfShipIsDestroyed(tileScript)) {
               ChangeShipTilesToDestroyed(tileScript);
               Debug.Log("Enemy ship destroyed!");
           }
           // Invoke StartPlayerTurn to give player another turn
           Invoke(nameof(StartPlayerTurn), 1f);
       } else {
           // Invoke StartEnemyTurn to switch to AI's turn
           Invoke(nameof(StartEnemyTurn), 1f);
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
    
    public void RegisterHit(TileScript tile, bool wasHit, bool isPlayerAttack)
    {
        string attacker = isPlayerAttack ? "Player" : "AI";
        string hitResult = wasHit ? "hit" : "miss";
        Debug.Log($"{attacker} {hitResult} at tile {tile.name}");
        
        if (wasHit)
        {
            Debug.Log((isPlayerAttack ? "Player" : "AI") + " hit a ship!");
            tile.ChangeColor(Color.red); // Red for a hit
        }
        else
        {
            Debug.Log((isPlayerAttack ? "Player" : "AI") + " missed.");
            tile.ChangeColor(Color.gray); // Gray for a miss
        }

        // Update game state
        if (isPlayerAttack)
        {
            // Player's logic
            if (!wasHit)
            {
                // Player missed, so it's AI's turn
                isPlayerTurn = false;
                Invoke(nameof(AIPlayTurn), 1f);
            }
            else
            {
                // Player hit, so they get another turn (handled in the invoking method)
            }
        }
        else
        {
            // AI's logic
            if (!wasHit)
            {
                // AI missed, so it's player's turn
                isPlayerTurn = true;
                Invoke(nameof(StartPlayerTurn), 1f);
            }
            else
            {
                // AI hit, so it gets another turn (handled in the invoking method)
            }
        }
    }
    
    private void AIPlayTurn()
    {
        Debug.Log("AI Play Turn");
        bool validHit = false;
        while (!validHit)
        {
            int x = Random.Range(0, playerGridManager.gridWidth);
            int y = Random.Range(0, playerGridManager.gridHeight);
            TileScript targetTile = playerGridManager.GetTileAt(x, y).GetComponent<TileScript>();

            // Check if the tile has already been hit to prevent hitting the same tile again
            if (!targetTile.IsHit) 
            {
                targetTile.OnHit(false); // Pass false to indicate this is an AI attack
                validHit = true; // We made a valid hit, so we can exit the loop

                // Determine the next action based on whether the AI hit or missed
                if (targetTile.IsOccupiedByPlayerShip) 
                {
                    // If hit, AI gets another turn
                    Debug.Log("AI hit the player's ship!");
                    Invoke(nameof(AIPlayTurn), 1f); // Wait for 1 second before AI's next turn
                } 
                else 
                {
                    // If miss, it's now the player's turn
                    Debug.Log("AI missed!");
                    isPlayerTurn = true; // Change turn to player
                    Invoke(nameof(StartPlayerTurn), 1f); // Wait for 1 second before switching to player's turn
                }
            }
        }
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
        ToggleShipsVisibility(playerPlacedShips, false);
        ToggleShipsVisibility(enemyPlacedShips, false);
        //playerGridManager.gameObject.SetActive(false);
        //enemyGridManager.gameObject.SetActive(true);
    }
    
    public void StartEnemyTurn()
    {
        Debug.Log("Enemy's turn!");
        ToggleShipsVisibility(playerPlacedShips, true);
        ToggleShipsVisibility(enemyPlacedShips, false);
        //enemyGridManager.gameObject.SetActive(false);
        //playerGridManager.gameObject.SetActive(true);
        AIPlayTurn();
    }

    private void ToggleShipsVisibility(List<GameObject> ships, bool visible)
    {
        foreach (var ship in ships)
        {
            ship.SetActive(visible);
        }
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
                
                if (isPlayer)
                {
                    // Add this for player ships
                    foreach (Vector2Int tile in occupiedTiles)
                    {
                        gridManager.GetTileAt(tile.x, tile.y).GetComponent<TileScript>().SetOccupiedByPlayerShip(true);
                    }
                    Debug.Log($"Placed player {shipPrefab.name} at tiles: {string.Join(", ", occupiedTiles)}");
                }
                else
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
