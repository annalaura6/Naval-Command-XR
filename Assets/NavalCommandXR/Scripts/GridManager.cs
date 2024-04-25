using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab; 
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float tileSize = 0.001f;
    public float tileSpacing = 0.06f; 
    public Vector3 gridStartPosition = Vector3.zero;

    private GameObject[,] gridArray;

    private bool[,] occupied;
    
    private bool isPlayerTurn = true;
    
    void Awake()
    {
        occupied = new bool[gridWidth, gridHeight];
        CreateGrid();
    }
    
    void Update()
    {
        if (isPlayerTurn)
        {
            // Waiting for the player to drop the missile
        }
        else
        {
            // Process the AI's turn
            AIChooseTile();
        }
    }
    
    public void OnMissileHit(TileScript hitTile)
    {
        if (hitTile.IsOccupiedByEnemyShip)
        {
            hitTile.ChangeColor(Color.red);
            CheckShipDestruction(hitTile); // If all tiles of a ship are hit, change color to black
            // Player gets another turn, so no need to change isPlayerTurn
        }
        else
        {
            hitTile.ChangeColor(Color.grey);
            EndPlayerTurn();
        }
    }
    
    private void CheckShipDestruction(TileScript hitTile)
    {
        // Check if the ship has been fully hit
        // If so, call a method to change all involved tiles to black
    }
    
    private void EndPlayerTurn()
    {
        // Player's turn ends, so switch to AI's turn
        isPlayerTurn = false;
    }
    
    private void AIChooseTile()
    {
        // AI chooses a tile at random
        // Apply the same logic as the player's missile hit
        // Change isPlayerTurn to true at the end of AI's turn
    }
    
    public void DebugTurn()
    {
        // Simple method to log whose turn it is
        Debug.Log(isPlayerTurn ? "Player's turn" : "AI's turn");
    }
    
    public void ResetGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                occupied[x, z] = false;
            }
        }
    }
    
    public bool IsTileFree(int x, int y)
    {
        return !occupied[x, y];
    }

    public void OccupyTile(int x, int y)
    {
        occupied[x, y] = true;
    }

    public GameObject GetTileAt(int x, int y)
    {
        return gridArray[x, y];
    }

    void CreateGrid()
    {
        gridArray = new GameObject[gridWidth, gridHeight];
        float totalTileSize = tileSize + tileSpacing;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 position = gridStartPosition + new Vector3(x * totalTileSize, 0, z * totalTileSize);
                position -= new Vector3(tileSpacing / 2, 0, tileSpacing / 2);

                GameObject newTile = Instantiate(tilePrefab, position, Quaternion.identity);
                newTile.name = $"Tile_{x}_{z}";
                newTile.transform.parent = transform;
                
                TileScript tileScript = newTile.GetComponent<TileScript>();
                
                gridArray[x, z] = newTile;
            }
        }
    }
    public void HighlightTile(int x, int y, bool highlight)
    {
        TileScript tileScript = gridArray[x, y].GetComponent<TileScript>();
        if(tileScript != null)
        {
            tileScript.HighlightTile(highlight);
        }
    }
    
    public void ResetTileColors()
    {
        foreach (GameObject tile in gridArray)
        {
            tile.GetComponent<TileScript>().ResetColor(); 
        }
    }
    
    public bool IsTileOccupied(int x, int y)
    {
        // Check if the coordinates are within the grid bounds before accessing the array
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
        {
            return occupied[x, y];
        }
        else
        {
            // If the coordinates are out of bounds, return true to indicate the tile can't be used
            return true;
        }
    }

    
}