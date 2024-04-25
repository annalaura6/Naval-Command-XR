using System;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    private bool isOccupiedByEnemyShip;
    
    private Renderer _renderer;
    public static event Action<TileScript> OnMissileHit;
    
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void OnHit() {
        if (IsOccupiedByEnemyShip) {
            ChangeColor(Color.red); // Hit
            // Optionally, if the ship is destroyed, you could ChangeColor(Color.black);
        }
        else {
            ChangeColor(Color.gray); // Miss
        }

        // Inform the GameManager about the hit
        GameManager.Instance.RegisterHit(this, IsOccupiedByEnemyShip);
    }

    public void SetOccupiedByEnemyShip(bool occupied)
    {
        IsOccupiedByEnemyShip = occupied;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Missile"))
        {
            OnMissileHit?.Invoke(this);
        }
    }
    
    

// Change the method to a property
    public bool IsOccupiedByEnemyShip {
        get { return isOccupiedByEnemyShip; }
        set { isOccupiedByEnemyShip = value; }
    }
    
    public void HighlightTile(bool highlight)
    {
        Color color = highlight ? Color.green : Color.white;
        _renderer.material.color = color;
    }

    public void ChangeColor(Color color)
    {
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