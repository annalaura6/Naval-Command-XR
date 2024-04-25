using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    public int Length => shipLength; 

    [SerializeField]
    private int shipLength = 2; 
    
}
