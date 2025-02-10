using UnityEngine;

public class EntityState
{
    protected EntityStateMachine stateMachine;
    protected Entity entity;
    protected float xInput;
    protected float yInput;
    protected Rigidbody2D rb;
    protected bool triggerCalled;

    private string animBoolName;

    protected float stateTimer;

    public EntityState(Entity entity, EntityStateMachine _stateMachine, string _animBoolName)
    {
        this.stateMachine = _stateMachine;
        this.entity = entity;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        rb = entity.rb;
        xInput = Input.GetAxisRaw("Horizontal");

        entity.animator.SetBool(animBoolName, true);

        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        // entity.animator.SetFloat("yVelocity", rb.velocity.y);
    }

    public virtual void Exit()
    {
        entity.animator.SetBool(animBoolName, false);
    }

    public override string ToString()
    {
        return animBoolName;
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}