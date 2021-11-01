using System;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private CharacterSO characterData;
    [SerializeField] private WorldSO worldData;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Transform weaponContainer;

    private Joystick joystick;
    private Vector3 movement;
    private Vector3 lookAtPoint;
    private GameObject currentWeaponObj;
    private bool isRunning = false;

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
                characterAnimator.speed = 1f;
                characterAnimator.SetBool(RUN_BOOL, true);
                characterAnimator.SetBool(ATTACK_BOOL, false);
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

        joystick.DirectionAction += OnDirectionChanged;
        characterData.WeaponChangedAction += OnWeaponChanged;
    }

    private void Start()
    {
        CreateWeapon();
    }

    private void OnDestroy()
    {
        joystick.DirectionAction -= OnDirectionChanged;
        characterData.WeaponChangedAction -= OnWeaponChanged;
    }
    #endregion

    #region Handlers
    private void OnDirectionChanged(Vector2 dir, float movementStrength)
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

        dir = dir * (characterData.EquipedWeapon.heroMovementSpeed * movementStrength);
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
        EnemyManager enemy;

        for(int i = 0; i < worldData.AliveEnemies.Count; i++)
        {
            enemy = worldData.AliveEnemies[i];
            if(IsEnemyInRangeToAttack(enemy.transform))
            {
                AttackEnemy(enemy);
            }
        }
    }

    private void AttackEnemy(EnemyManager enemy)
    {
        transform.LookAt(enemy.transform);
        characterAnimator.speed = characterData.EquipedWeapon.attackSpeedMultiplier;
        characterAnimator.SetBool(ATTACK_BOOL, true);

        enemy.EnemyDeathAction += OnEnemyDeath;
    }

    private void TryToCancelAttack(bool isInstantCancelation)
    {
        if(!IsRunning &&  characterAnimator.GetCurrentAnimatorStateInfo(0).IsName(ATTACK_STATE) && isInstantCancelation)
        {
            characterAnimator.SetTrigger(CANCEL_ATTACK_TRIGGER);
        }
        
        characterAnimator.SetBool(ATTACK_BOOL, false);
        characterAnimator.speed = 1;
    }
    #endregion

    #region Helpers
    private bool IsEnemyInRangeToAttack(Transform enemyTransform)
    {
        if((enemyTransform.position - transform.position).magnitude <= characterData.EquipedWeapon.attackRange)
        {
            return true;
        }

        return false;
    }
    #endregion
}