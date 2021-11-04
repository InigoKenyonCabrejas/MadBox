using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyView : MonoBehaviour
{
    [SerializeField] private GameObject modelContainer;
    [SerializeField] private HealthBar healthBar;

    private Enemy enemyData;
    private int initialHealth;
    private Vector3 auxRotation;
    private bool isDying = false;

    public Action<EnemyView> EnemyDeathAction;

    public bool IsAlive
    {
        get
        {
            if(enemyData.health > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public int DamageOfAttack
    {
        get
        {
            return enemyData.damageOfAttack;
        }
    }

    public int ScoreValue
    {
        get
        {
            return enemyData.scoreValue;
        }
    }

    #region Init&Mono
    private void Update()
    {
        switch(enemyData.enemyAIState)
        {
            case EnemyAIState.Wandering:

                if(IsPlayerInRange())
                {
                    enemyData.enemyAIState = EnemyAIState.Attacking;
                    break;
                }

                MoveEnemy();

                break;
            case EnemyAIState.Attacking:

                if(enemyData.health <= enemyData.retreatAtHealth)
                {
                    enemyData.enemyAIState = EnemyAIState.Retreating;
                    break;
                }

                FollowPlayer();

                break;
            case EnemyAIState.Retreating:

                if(!IsPlayerInRange())
                {
                    enemyData.enemyAIState = EnemyAIState.Wandering;
                    break;
                }

                RetreatFromPlayer();

                break;
            case EnemyAIState.Dead:
                //Do nothing until object is destroyed
                break;
        }
    }

    public void Init(Enemy _enemyData)
    {
        enemyData = _enemyData;
        initialHealth = enemyData.health;
    }
    #endregion

    #region Logic
    public void ReceiveHit(int damage)
    {
        if(isDying)
        {
            return;
        }
        
        enemyData.health -= damage;
        healthBar.ChangeHealth((float)enemyData.health / (float)initialHealth);

        if(enemyData.health <= 0)
        {
            Kill();
        }
    }

    private void Kill()
    {
        isDying = true;
        StartCoroutine(KillProcess());
    }

    private IEnumerator KillProcess()
    {
        modelContainer.SetActive(false);
        //Wait for death anim, healthBar lerp, etc...
        yield return new WaitForSeconds(0.5f);

        EnemyDeathAction?.Invoke(this);
        Destroy(gameObject);
    }

    private void MoveEnemy()
    {
        auxRotation = transform.eulerAngles;
        auxRotation.y += Random.Range(-5f, 5f);
        transform.eulerAngles = auxRotation;
        transform.position += transform.forward * enemyData.movementSpeed;
    }

    private void FollowPlayer()
    {
        transform.LookAt(CharacterView.Character.transform);
        transform.position += transform.forward * enemyData.movementSpeed;
    }

    private void RetreatFromPlayer()
    {
        transform.LookAt(CharacterView.Character.transform);
        auxRotation.x = 0f;
        auxRotation.y = 180f;
        auxRotation.z = 0f;
        transform.eulerAngles += auxRotation;
        transform.position += transform.forward * enemyData.movementSpeed;
    }

    public void MakeStronger()
    {
        initialHealth++;
        enemyData.health++;
    }
    #endregion
    #region Helpers
    private bool IsPlayerInRange()
    {
        if((transform.position - CharacterView.Character.transform.position).magnitude <= enemyData.rangeToAttack)
        {
            return true;
        }

        return false;
    }
    #endregion
}