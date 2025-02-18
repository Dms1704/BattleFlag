using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using States;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour
{
    // Unity数据
    [Header("移动速度")]
    [SerializeField] protected float moveSpeed;
    [FormerlySerializedAs("knockbackSpeed")]
    [Header("受击和闪躲")]
    [SerializeField] protected float SmilingMovementSpeed;
    [SerializeField] protected float SmilingMovementOffset;

    // 数据
    protected bool isBusy;
    public bool isOperating;
    protected IList<MoveStepLO> moveStepLos;
    public int faceRight = 1;
    public bool isAlly = true;
    
    // 数据接口（能力组件）
    public IEquippable equipment { get; private set; }
    
    // 事件
    public event Action OnOperateOverChanged;
    public event Action OnInitialized;
    
    // 状态
    protected EntityStateMachine stateMachine { get; private set; }
    public EntityIdleState idleState { get; private set; }
    public EntityMoveState moveState { get; private set; }
    public EntityAttackState attackState { get; private set; }

    // 组件
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    public Tilemap tilemap { get; private set; }
    public GameObject operatingCursor;
    public GameObject mainWeapon;
    public GameObject chessBase;
    [HideInInspector] public SpriteRenderer mainWeaponSr;
    [HideInInspector] public SpriteRenderer playerSpriteSr;
    [HideInInspector] public SpriteRenderer chessBaseSpriteSr;
    
    private void Initialize()
    {
        // 初始化逻辑...
        OnInitialized?.Invoke(); // 触发事件
    }

    protected virtual void Awake()
    {
        stateMachine = new EntityStateMachine();
        idleState = new EntityIdleState(this, stateMachine, "Idle");
        moveState = new EntityMoveState(this, stateMachine, "Move");
        attackState = new EntityAttackState(this, stateMachine, "Attack");
    }

    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
        tilemap = TilemapSingleton.instance.tilemap;
        mainWeaponSr = mainWeapon.GetComponent<SpriteRenderer>();
        playerSpriteSr = animator.gameObject.GetComponent<SpriteRenderer>();
        chessBaseSpriteSr = chessBase.GetComponent<SpriteRenderer>();

        equipment = new EquippableImpl();
        
        BoardManager.instance.AddEntity(this);
        
        OnInitialized += BoardManager.instance.HandleCharacterInitialized;
        Initialize();
        
        stateMachine.Initialize(idleState);
    }
    
    protected virtual void Update()
    {
        stateMachine.currentState.Update();

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (CanEquip() && equipment.IsDirty())
        {
            ItemEquipmentData mainEquipment = equipment.GetEquipment(EquipmentType.MainWeapon);
            mainWeaponSr.sprite = !mainEquipment ? null : mainEquipment.icon;
            equipment.Clean();
        }
    }

    public void ClearMoveSteps()
    {
        moveStepLos.Clear();
    }
    
    public IEnumerator AttackAndDash()
    {
        isBusy = true;
        yield return Knockback(transform.position, transform.position + new Vector3((float)(faceRight * SmilingMovementOffset * 0.4), 0, 0), transform);
        yield return Knockback(transform.position, transform.position + new Vector3((float)(-faceRight * SmilingMovementOffset * 0.4), 0, 0), transform);
        isBusy = false;
    }
    
    public IEnumerator KnockbackAndBack()
    {
        isBusy = true;
        yield return Knockback(transform.position, transform.position + new Vector3(-faceRight * SmilingMovementOffset, 0, 0), transform);
        yield return Knockback(transform.position, transform.position + new Vector3(faceRight * SmilingMovementOffset, 0, 0), transform);
        isBusy = false;
    }

    public IEnumerator Knockback(Vector3 src, Vector3 dst, Transform moveTarget)
    {
        Vector3 currentPos = src;
        float distance = Vector3.Distance(src, dst);
        float timeToMove = distance / SmilingMovementSpeed;
        for (float t = 0; t < timeToMove; t += Time.deltaTime)
        {
            // 计算插值位置
            currentPos = Vector3.Lerp(moveTarget.position, dst, t / timeToMove);
 
            // 设置对象位置
            moveTarget.position = currentPos;
 
            // 等待下一帧
            yield return null;
        }
        moveTarget.position = dst;
    }

    public delegate void MoveOver();
    public IEnumerator MoveSequentially(IList<MoveStepLO> moveSteps, MoveOver moveOver)
    {
        isBusy = true;
        moveSteps.Insert(0, new MoveStepLO(transform.position, 0));
        for (int i = 0; i < moveSteps.Count - 1; i++)
        {
            yield return MoveToPosition(moveSteps[i], moveSteps[i + 1]);
        }
        moveOver?.Invoke();
        isBusy = false;
    }
    
    public IEnumerator MoveToPosition(MoveStepLO srcStep, MoveStepLO targetStep)
    {
        Vector3 currentPos = srcStep.targetPos;
        float distance = Vector3.Distance(srcStep.targetPos, targetStep.targetPos);
        float timeToMove = distance / moveSpeed;
        for (float t = 0; t < timeToMove; t += Time.deltaTime)
        {
            // 计算插值位置
            currentPos = Vector3.Lerp(srcStep.targetPos, targetStep.targetPos, t / timeToMove);
 
            // 设置对象位置
            transform.position = currentPos;
 
            // 等待下一帧
            yield return null;
        }
        
        transform.position = targetStep.targetPos;
        stats.CostActionPoint(targetStep.moveCost);
    }

    #region 网格对齐
    public void SetPositionUseOddQ(Vector3Int oddQVector)
    {
        Vector3 cellToWorld = tilemap.CellToWorld(oddQVector);
        transform.position = cellToWorld;
    }

    /**
     * 设置小人位置（使用轴坐标）
     */
    public void SetPositionUseAxial(Vector3Int axialVector)
    {
        Vector3Int oddQVector = HexGridUtil.AxialToOddQ(axialVector);
        SetPositionUseOddQ(oddQVector);
    }
    #endregion

    public virtual void Die()
    {
        Destroy(gameObject);
        TurnOrderManager.instance.RemoveSortListEntity(this);
        BoardManager.instance.RemoveEntity(this);
        
        int winOrLose = BoardManager.instance.WinOrLose();
        if (winOrLose == 0)
        {
            return;
        }
        if (winOrLose == 1)
        {
            UI.instance.WinOrLose(true);
            return;
        }
        if (winOrLose == 2)
        {
            UI.instance.WinOrLose(false);
        }
    }
    
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();
    
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    /**
     * 轮到你了！
     */
    public virtual void Operate()
    {
        isOperating = true;
        operatingCursor.SetActive(true);
    }

    public void OperateOver()
    {
        isOperating = false;
        operatingCursor.SetActive(false);
        OnOperateOverChanged?.Invoke();
    }

    // todo 问题：地方不需要脚印，所以这个方法不应该放在这里，它应该放在Player
    public virtual void ClearFootprints()
    {
        
    }

    public Vector3Int GetGridPosition()
    {
        return HexGridUtil.IgnoreZ(tilemap.WorldToCell(transform.position));
    }

    public bool CanEquip()
    {
        return equipment != null;
    }
}
