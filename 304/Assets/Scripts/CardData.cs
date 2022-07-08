using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardData : ScriptableObject
{
    public Suite suite;
    public Number number;
    public int Points;
    public Team currentTeam;
    public Sprite cardSprite; 
}
