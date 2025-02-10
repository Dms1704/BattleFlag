namespace States
{
    public class EntityAttackState : EntityState
    {
        public EntityAttackState(Entity entity, EntityStateMachine _stateMachine, string _animBoolName) : 
            base(entity, _stateMachine, _animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();

            if (triggerCalled)
            {
                stateMachine.ChangeState(entity.idleState);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}