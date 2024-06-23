using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    public int Length => shipLength; 

    [SerializeField]
    private int shipLength = 2; 
    
    public List<TileScript> occupiedTiles = new List<TileScript>();
    
    public static event Action<ShipScript> OnShipDestroyed;

    public bool IsDestroyed()
    {
        if (IsDestroyed())
        {
            OnShipDestroyed?.Invoke(this);
        }
        
        foreach (var tile in occupiedTiles)
        {
            if (!tile.IsHit) 
                return false;
        }
        return true;
    }
}
