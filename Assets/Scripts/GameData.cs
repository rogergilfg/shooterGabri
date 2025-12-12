using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField]
    private float currentLife;
    [SerializeField]
    private float maxLife;
    [SerializeField]
    private List<Weapon> weapons;
    [SerializeField]
    private int weaponIndex;


    public float CurrentLife
    {
        get { return currentLife; } 
        set { currentLife = value; }
    }

    public float MaxLife
    {
        get { return maxLife; }
        set { maxLife = value; }
    }

    public List<Weapon> Weapons
    {
        get { return weapons; }
        set {  weapons = value; }
    }

    public int WeaponIndex
    {
        get { return weaponIndex; }
        set { weaponIndex = value; }
    }
}
