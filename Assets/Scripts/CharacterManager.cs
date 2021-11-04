using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private CharacterSO characterData;
    [SerializeField] private WorldSO worldData;
    [SerializeField] private DamageDisplayer damageDisplayer;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private AnimationClip attackAnimation;
    [SerializeField] private Transform weaponContainer;

    private Joystick joystick;
    private Vector3 movement;
    private Vector3 lookAtPoint;
    private GameObject currentWeaponObj;
    private bool isRunning = false;
    private float attackAnimationDuration;
    private Coroutine attackCoroutine;

    private const string RUN_BOOL = "Running";
    private const string DEAD_BOOL = "Dead";
    private const string ATTACK_BOOL = "Attack";
    private const string DAMAGED_TRIGGER = "Damaged";
    private const string CANCEL_ATTACK_TRIGGER = "CancelAttack";
    private const string ATTACK_STATE = "Attack";

    public bool IsRunning
    {
        get { return isRunning; }
        set
        {
            if(isRunning != value && !value)
            {
                TryToAttack();
            }

            isRunning = value;

            if(isRunning)
            {
                currentWeaponObj.SetActive(false);
                characterAnimator.SetBool(RUN_BOOL, true);
                CancelAttack();
                characterAnimator.speed = 1f * characterData.EquipedWeapon.movementSpeedMultiplier;
            }
            else
            {
                currentWeaponObj.SetActive(true);
                characterAnimator.SetBool(RUN_BOOL, false);
            }
        }
    }

    #region Init&Mono
    private void Awake()
    {
        joystick = FindObjectOfType<Joystick>().GetComponent<Joystick>();
        movement = Vector3.zero;
        lookAtPoint = Vector3.zero;
        attackAnimationDuration = attackAnimation.length;

        joystick.MovementUpdateAction += OnMovementUpdate;
        characterData.WeaponChangedAction += OnWeaponChanged;
    }

    private void Start()
    {
        CreateWeapon();
    }

    private void OnDestroy()
    {
        joystick.MovementUpdateAction -= OnMovementUpdate;
        characterData.WeaponChangedAction -= OnWeaponChanged;
    }
    #endregion

    #region Handlers
    private void OnTriggerEnter(Collider other)
    {
        PickUpLoot(other.gameObject);
    }

    private void OnMovementUpdate(Vector2 dir, float movementStrength)
    {
        MoveCharacter(dir, movementStrength);
    }

    private void OnWeaponChanged()
    {
        ChangeWeapon();
    }
    
    private void OnEnemyDeath(EnemyManager enemy)
    {
        TryToCancelAttack(false);
        
        enemy.EnemyDeathAction -= OnEnemyDeath;
    }
    #endregion

    #region Logic
    private void MoveCharacter(Vector2 dir, float movementStrength)
    {
        if(movementStrength > 0)
        {
            IsRunning = true;
        }
        else
        {
            IsRunning = false;
        }

        lookAtPoint.x = transform.position.x + dir.x;
        lookAtPoint.y = 0f;
        lookAtPoint.z = transform.position.z + dir.y;

        transform.LookAt(lookAtPoint);

        dir = dir * (characterData.movementSpeed * characterData.EquipedWeapon.movementSpeedMultiplier * movementStrength);
        movement = new Vector3(dir.x, 0f, dir.y);
        transform.position += movement;
    }

    private void ChangeWeapon()
    {
        if(currentWeaponObj != null)
        {
            Destroy(currentWeaponObj);
        }

        CreateWeapon();
        TryToCancelAttack(true);
        TryToAttack();
    }

    private void CreateWeapon()
    {
        currentWeaponObj = Instantiate(characterData.EquipedWeapon.weaponPrefab, weaponContainer);
    }

    private void TryToAttack()
    {
        EnemyManager enemy = null;
        EnemyManager lastInRangeEnemy = null;
        float enemyDistance = -1f;
        float lastInRangeEnemyDistance = -1f;

        for(int i = 0; i < worldData.AliveEnemies.Count; i++)//We iterate the full loop looking for closest enemy
        {
            enemy = worldData.AliveEnemies[i];
            enemyDistance = GetDistanceToEnemyInRange(enemy.transform);
            if(enemyDistance != -1f && (lastInRangeEnemyDistance == -1 || enemyDistance < lastInRangeEnemyDistance))
            {
                lastInRangeEnemyDistance = enemyDistance;
                lastInRangeEnemy = enemy;
            }
        }

        if(lastInRangeEnemyDistance != -1f && lastInRangeEnemy.IsAlive)
        {
            AttackEnemy(lastInRangeEnemy);
        }
    }

    private void AttackEnemy(EnemyManager enemy)
    {
        transform.LookAt(enemy.transform);
        characterAnimator.speed = 1f * characterData.EquipedWeapon.attackSpeedMultiplier;
        characterAnimator.SetBool(ATTACK_BOOL, true);

        enemy.EnemyDeathAction += OnEnemyDeath;

        attackCoroutine = StartCoroutine(AttackDelayToDamageEnemy(enemy));
    }

    private IEnumerator AttackDelayToDamageEnemy(EnemyManager enemy)
    {
        yield return new WaitForSeconds(attackAnimationDuration / characterData.EquipedWeapon.attackSpeedMultiplier);

        if(enemy == null)
        {
            yield break;
        }
        
        if(GetDistanceToEnemyInRange(enemy.transform) != -1)
        {
            enemy.HitEnemy(characterData.EquipedWeapon.attackDamage);
            damageDisplayer.ShowDamageFloater(enemy, characterData.EquipedWeapon.attackDamage);

            if(enemy.IsAlive)
            {
                AttackEnemy(enemy);
            }
        }
        else
        {
            TryToCancelAttack(false);
        }

        attackCoroutine = null;
    }

    private void TryToCancelAttack(bool isInstantCancelation)
    {
        if(!IsRunning &&  characterAnimator.GetCurrentAnimatorStateInfo(0).IsName(ATTACK_STATE) && isInstantCancelation)
        {
            characterAnimator.SetTrigger(CANCEL_ATTACK_TRIGGER);
        }

        CancelAttack();
    }

    private void CancelAttack()
    {
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        
        characterAnimator.SetBool(ATTACK_BOOL, false);
        characterAnimator.speed = 1f;
    }

    private void PickUpLoot(GameObject lootObj)
    {
        Loot loot = lootObj.GetComponent<Loot>();

        if(loot == null)
        {
            return;
        }
        
        characterData.AddNewWeapon(loot.weaponType);
        Destroy(lootObj);
    }
    #endregion

    #region Helpers
    private float GetDistanceToEnemyInRange(Transform enemyTransform)
    {
        float rangeToEnemy = (enemyTransform.position - transform.position).magnitude;
        
        if(rangeToEnemy <= characterData.EquipedWeapon.attackRange)
        {
            return rangeToEnemy;
        }

        return -1;//-1 represents not in range
    }
    #endregion
}