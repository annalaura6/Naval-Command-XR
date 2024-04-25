using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab; // Assign a prefab for the tile in the inspector
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float tileSize = 1.0f;
    public float tileSpacing = 0.1f; // Space between tiles
    public Vector3 gridStartPosition = Vector3.zero;// Set this to the width and height of your tile

    private GameObject[,] gridArray;

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        gridArray = new GameObject[gridWidth, gridHeight];
        // Calculate total size of a tile including spacing
        float totalTileSize = tileSize + tileSpacing;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Calculate the position with spacing on all sides
                Vector3 position = gridStartPosition + new Vector3(x * totalTileSize, 0, z * totalTileSize);
                // Adjust the position by half the spacing to ensure even spacing around tiles
                position -= new Vector3(tileSpacing / 2, 0, tileSpacing / 2);

                GameObject newTile = Instantiate(tilePrefab, position, Quaternion.identity);
                newTile.name = $"Tile_{x}_{z}";
                newTile.transform.parent = transform;

                // You can add a TileScript component to the tile if it's not already attached in the prefab
                //TileScript tileScript = newTile.AddComponent<TileScript>();
                // If the TileScript is already on the prefab, use the following line instead:
                TileScript tileScript = newTile.GetComponent<TileScript>();

                // Store the tile in the grid array
                gridArray[x, z] = newTile;
            }
        }
    }
}