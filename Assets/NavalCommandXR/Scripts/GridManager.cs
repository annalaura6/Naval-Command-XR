using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float tileSize = 0.001f;
    public float tileSpacing = 0.06f;
    public Vector3 gridStartPosition = Vector3.zero;

    protected GameObject[,] gridArray;
    protected bool[,] occupied;

    protected virtual void Awake()
    {
        occupied = new bool[gridWidth, gridHeight];
        CreateGrid();
    }

    protected void CreateGrid()
    {
        gridArray = new GameObject[gridWidth, gridHeight];
        float totalTileSize = tileSize + tileSpacing;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 position = gridStartPosition + new Vector3(x * totalTileSize, 0, z * totalTileSize) - new Vector3(tileSpacing / 2, 0, tileSpacing / 2);
                GameObject newTile = Instantiate(tilePrefab, position, Quaternion.identity);
                newTile.name = $"Tile_{x}_{z}";
                newTile.transform.parent = transform;

                TileScript tileScript = newTile.GetComponent<TileScript>();
                gridArray[x, z] = newTile;
            }
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
        return x >= 0 && y >= 0 && x < gridWidth && y < gridHeight && occupied[x, y];
    }

    public GameObject GetTileAt(int x, int y)
    {
        return gridArray[x, y];
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
    public void OccupyTile(int x, int y)
    {
        occupied[x, y] = true;
    }

    public void HighlightTile(int x, int y, bool highlight)
    {
        TileScript tileScript = gridArray[x, y].GetComponent<TileScript>();
        if(tileScript != null)
        {
            tileScript.HighlightTile(highlight);
        }
    }
}