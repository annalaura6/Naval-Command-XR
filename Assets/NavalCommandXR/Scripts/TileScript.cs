using System;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    private bool isOccupiedByEnemyShip;
    private bool isHit; 
    
    private Renderer _renderer;
    public static event Action<TileScript> OnMissileHit;
    
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void OnHit() {
        IsHit = true; 

        if (IsOccupiedByEnemyShip) {
            ChangeColor(Color.red); 
        }
        else {
            ChangeColor(Color.gray); 
        }
        
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
    
    public bool IsOccupiedByEnemyShip {
        get { return isOccupiedByEnemyShip; }
        set { isOccupiedByEnemyShip = value; }
    }

    public bool IsHit {
        get { return isHit; }
        set { isHit = value; }
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
        IsHit = false; 
    }
}