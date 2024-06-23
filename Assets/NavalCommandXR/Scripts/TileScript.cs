using System;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    private bool isOccupiedByEnemyShip;
    private bool isOccupiedByPlayerShip;
    private bool isHit; 
    
    private Renderer _renderer;
    public static event Action<TileScript> OnMissileHit;
    
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void OnHit(bool isPlayerAttack) 
    {
        if (IsHit) 
        {
            Debug.Log("Tile already hit: " + name);
            return;
        }

        IsHit = true;
        
        Debug.Log($"{(isPlayerAttack ? "Player" : "AI")} attack on tile {name}");
        
        if (isPlayerAttack)
        {
            bool wasHit = IsOccupiedByEnemyShip;
            ChangeColor(wasHit ? Color.red : Color.gray);
            Debug.Log($"Player attack on tile {name} - Occupied by enemy ship: {wasHit}");
            GameManager.Instance.RegisterHit(this, wasHit, true);
        }
        else
        {
            bool wasHit = IsOccupiedByPlayerShip;
            ChangeColor(wasHit ? Color.red : Color.gray);
            Debug.Log($"AI attack on tile {name} - Occupied by player ship: {wasHit}");
            GameManager.Instance.RegisterHit(this, wasHit, false);
        }
    }


    public void SetOccupiedByEnemyShip(bool occupied)
    {
        IsOccupiedByEnemyShip = occupied;
    }
    
    public void SetOccupiedByPlayerShip(bool occupied)
    {
        IsOccupiedByPlayerShip = occupied;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Missile"))
        {
            OnMissileHit?.Invoke(this);
        }
    }
    
    public bool IsOccupiedByPlayerShip {
        get { return isOccupiedByPlayerShip; }
        set { isOccupiedByPlayerShip = value; }
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