using System;
using UnityEngine;

[Serializable]
public class Enemy
{
    public EnemyType enemyType;
    public GameObject enemyPrefab;
    public int health = 1;

    public Enemy(EnemyType _enemyType, int _health)
    {
        enemyType = _enemyType;
        health = _health;
    }
}

public enum EnemyType
{
    Bee = 0
}