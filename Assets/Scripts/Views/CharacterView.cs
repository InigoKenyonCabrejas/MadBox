using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private CharacterSO characterData;
    [SerializeField] private WorldSO worldData;
    [SerializeField] private NumbersFloater numbersFloater;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private AnimationClip attackAnimation;
    [SerializeField] private Transform weaponContainer;
    [SerializeField] private Rigidbody rigidbody;

    private bool isRunning = false;
    private bool isAttacking = false;
    private bool isDead = false;
    private Joystick joystick;
    private Vector3 movement;
    private Vector3 lookAtPoint;
    private GameObject currentWeaponObj;
    private float attackAnimationDuration;
    private Coroutine attackCoroutine;
    private EnemyView targetEnemy;

    private const string RUN_BOOL = "Running";
    private const string DEAD_BOOL = "Dead";
    private const string ATTACK_BOOL = "Attack";
    private const string DAMAGED_TRIGGER = "Damaged";
    private const string CANCEL_ATTACK_TRIGGER = "CancelAttack";
    private const string ATTACK_STATE = "Attack";

    public static CharacterView Character;

    public bool IsRunning
    {
        get { return isRunning; }
        set
        {
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
        Character = this;
        
        joystick = FindObjectOfType<Joystick>().GetComponent<Joystick>();
        movement = Vector3.zero;
        lookAtPoint = Vector3.zero;
        attackAnimationDuration = attackAnimation.length;

        joystick.MovementUpdateAction += OnMovementUpdate;
        characterData.WeaponChangedAction += OnWeaponChanged;
        characterData.CharacterDeadAction += OnCharacterDead;

    }

    private void Start()
    {
        CreateWeapon();
    }

    private void LateUpdate()
    {
        TryToAttack();
    }

    private void OnDestroy()
    {
        joystick.MovementUpdateAction -= OnMovementUpdate;
        characterData.WeaponChangedAction -= OnWeaponChanged;
        characterData.CharacterDeadAction -= OnCharacterDead;
    }
    #endregion

    #region Handlers
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.gameObject.name);
        TryToPickUpLoot(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("OnCollisionEnter: " + other.gameObject.name);
        CheckAndReceiveEnemyAttacked(other.gameObject);
    }

    private void OnMovementUpdate(Vector2 dir, float movementStrength)
    {
        MoveCharacter(dir, movementStrength);
    }

    private void OnWeaponChanged()
    {
        ChangeWeapon();
    }
    
    private void OnEnemyDeath(EnemyView enemy)
    {
        characterData.CurrentScore += enemy.ScoreValue;
        TryToCancelAttack(false);
        
        enemy.EnemyDeathAction -= OnEnemyDeath;
    }
    
    private void OnCharacterDead()
    {
        isDead = true;
        characterAnimator.SetBool(DEAD_BOOL, true);
    }
    #endregion

    #region Logic
    private void MoveCharacter(Vector2 dir, float movementStrength)
    {
        if(isDead)
        {
            return;
        }
        
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
        
        //rigidbody.MovePosition(transform.position + movement);
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
        if(isAttacking || isDead)
        {
            return;
        }
        
        EnemyView enemy = null;
        EnemyView lastInRangeEnemy = null;
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
            targetEnemy = lastInRangeEnemy;
            targetEnemy.EnemyDeathAction += OnEnemyDeath;
            AttackEnemy(targetEnemy);
        }
    }

    private void AttackEnemy(EnemyView enemy)
    {
        isAttacking = true;
        
        transform.LookAt(enemy.transform);
        characterAnimator.speed = 1f * characterData.EquipedWeapon.attackSpeedMultiplier;
        characterAnimator.SetBool(ATTACK_BOOL, true);
        
        attackCoroutine = StartCoroutine(AttackDelayToDamageEnemy(enemy));
    }

    private IEnumerator AttackDelayToDamageEnemy(EnemyView enemy)
    {
        float animDuration = attackAnimationDuration / characterData.EquipedWeapon.attackSpeedMultiplier;
        
        yield return new WaitForSeconds((animDuration/3f)*2f);//Wait 2/3 of anim to hit

        if(enemy == null)
        {
            yield break;
        }
        
        if(GetDistanceToEnemyInRange(enemy.transform) != -1)
        {
            enemy.ReceiveHit(characterData.EquipedWeapon.attackDamage);
            numbersFloater.ShowDamageFloater(enemy.transform.position, characterData.EquipedWeapon.attackDamage, true);

            if(enemy.IsAlive)
            {
                yield return new WaitForSeconds(animDuration/3f);//Wait 1/3 remaining before attacking again
                AttackEnemy(enemy);//This is were attacks chain with eachother
            }
        }
        else
        {
            yield return new WaitForSeconds(animDuration/3f);//Wait 1/3 remaining to let the animation finish
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

        if(targetEnemy != null)
        {
            targetEnemy.EnemyDeathAction -= OnEnemyDeath;
            targetEnemy = null;
        }
        
        characterAnimator.SetBool(ATTACK_BOOL, false);
        characterAnimator.speed = 1f;
        isAttacking = false;
    }

    private void TryToPickUpLoot(GameObject lootObj)
    {
        Loot loot = lootObj.GetComponent<Loot>();

        if(loot == null)
        {
            return;
        }
        
        characterData.AddNewWeapon(loot.weaponType);
        Destroy(lootObj);
    }

    private bool CheckAndReceiveEnemyAttacked(GameObject enemyObj)
    {
        EnemyView enemy = enemyObj.GetComponent<EnemyView>();

        if(enemy == null)
        {
            return false;
        }

        characterData.CurrentHealth -= enemy.DamageOfAttack;
        numbersFloater.ShowDamageFloater(transform.position, enemy.DamageOfAttack, false);

        return true;
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