using System;
using UnityEngine;

[Serializable]
public class Enemy
{
    public EnemyType enemyType;
    public GameObject enemyPrefab;
    public int health;
    public int retreatAtHealth;
    public float movementSpeed;
    public float rangeToAttack;
    public int damageOfAttack;
    public int scoreValue;

    [HideInInspector] public EnemyAIState enemyAIState;

    public Enemy(Enemy _enemy)
    {
        enemyType = _enemy.enemyType;
        enemyPrefab = _enemy.enemyPrefab;
        health = _enemy.health;
        retreatAtHealth = _enemy.retreatAtHealth;
        movementSpeed = _enemy.movementSpeed;
        rangeToAttack = _enemy.rangeToAttack;
        damageOfAttack = _enemy.damageOfAttack;
        scoreValue = _enemy.scoreValue;
        
        enemyAIState = EnemyAIState.Wandering;
    }
}

public enum EnemyType
{
    Bee = 0
}

public enum EnemyAIState
{
    Wandering = 0,
    Attacking = 1,
    Retreating = 3,
    Dead = 4
}