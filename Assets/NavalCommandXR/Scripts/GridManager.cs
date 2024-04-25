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
    
    void Awake()
    {
        occupied = new bool[gridWidth, gridHeight];
        CreateGrid();
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
}