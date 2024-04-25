using UnityEngine;

public class TileScript : MonoBehaviour
{
    private Renderer _renderer;
    
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void HighlightTile(bool highlight)
    {
        Color color = highlight ? Color.green : Color.white;
        _renderer.material.color = color;
    }

    public void OccupyTile()
    {
        HighlightTile(true); 
    }
    
    public void ResetColor()
    {
        _renderer.material.color = Color.blue; 
    }
}