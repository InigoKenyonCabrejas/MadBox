using System;
using UnityEngine;

[Serializable]
public class Enemy
{
    public EnemyType enemyType;
    public GameObject enemyPrefab;
    public int health = 1;
}

public enum EnemyType
{
    Bee = 0
}