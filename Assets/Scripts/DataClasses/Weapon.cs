using System;
using UnityEngine;

[Serializable]
public class Weapon
{
    public WeaponType weaponType;
    public GameObject weaponPrefab;
    public float attackSpeedMultiplier;
    public float movementSpeedMultiplier;
    public float attackRange;
    public int attackDamage;
}