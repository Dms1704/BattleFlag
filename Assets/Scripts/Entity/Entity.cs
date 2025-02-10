using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using States;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour
{
    // Unity数据
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float knockbackDuration;
    
    [Header("移动速度")]
    [SerializeField] protected float moveSpeed;

    // 数据
    protected bool isKnocked;
    protected bool isBusy;
    public bool isOperating;
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;
    public string lastAnimBoolName { get; private set; }
    protected IList<MoveStepLO> moveStepLos;
    
    // 事件
    public Action onFliped;
    public event Action OnOperateOverChanged;
    
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

    public virtual void AssignLastAnimName(string lastAnimBoolName)
    {
        this.lastAnimBoolName = lastAnimBoolName;
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
        
        BoardManager.instance.AddEntity(this);
        
        stateMachine.Initialize(idleState);
    }

    public void ClearMoveSteps()
    {
        moveStepLos.Clear();
    }

    public void CostActionPoint(int cost)
    {
        stats.actionPoint.AddModifier(-cost);
    }

    protected virtual void Update()
    {
        stateMachine.currentState.Update();
    }

    public delegate void MoveOver();
    public IEnumerator MoveSequentially(IList<MoveStepLO> moveSteps, MoveOver moveOver)
    {
        isBusy = true;
        moveSteps.Insert(0, new MoveStepLO(rb.position, 0));
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
            rb.position = currentPos;
 
            // 等待下一帧
            yield return null;
        }
        
        rb.position = targetStep.targetPos;
        stats.CostActionPoint(targetStep.moveCost);
    }

    public virtual void SlowEntityBy(float slowPercentage, float slowDuration)
    {

    }

    public virtual void DamageImpact()
    {
        StartCoroutine("HitKnockback");
    }

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        rb.velocity = new Vector2(knockbackDirection.x * -facingDir, knockbackDirection.y);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
    }

    #region 速度
    public void SetZeroVelocity()
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(0, 0); 
    }

    public void SetVelocity(float xInput, float yInput)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(xInput, yInput);
    }

    public void SetVelocityAndFlip(float xInput, float yInput)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(xInput, yInput);
        FlipController(xInput);
    }
    #endregion

    #region 翻转
    public virtual void Flip()
    {
        if (isKnocked)
        {
            return;
        }

        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.localRotation = facingRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        //transform.Rotate(0, 180, 0);

        onFliped?.Invoke();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }
    #endregion

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
}
