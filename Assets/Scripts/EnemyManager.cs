using System;
using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject modelContainer;
    [SerializeField] private HealthBar healthBar;

    private Enemy enemyData;
    private int initialHealth;

    public Action<EnemyManager> EnemyDeathAction;

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

    #region Init&Mono
    public void Init(Enemy _enemyData)
    {
        enemyData = _enemyData;
        initialHealth = enemyData.health;
    }
    #endregion

    #region Logic
    public void HitEnemy(int damage)
    {
        enemyData.health -= damage;
        healthBar.ChangeHealth((float) enemyData.health / (float) initialHealth);

        if(enemyData.health <= 0)
        {
            Kill();
        }
    }

    private void Kill()
    {
        StartCoroutine(KillProcess());
    }

    private IEnumerator KillProcess()
    {
        EnemyDeathAction?.Invoke(this);

        //Wait for death anim if it had one

        modelContainer.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
    #endregion
}