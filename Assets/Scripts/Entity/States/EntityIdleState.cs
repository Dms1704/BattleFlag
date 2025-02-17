namespace States
{
    public class EntityIdleState : EntityState
    {
        public EntityIdleState(Entity entity, EntityStateMachine _stateMachine, string _animBoolName) : 
            base(entity, _stateMachine, _animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            entity.SetPositionUseOddQ(entity.tilemap.WorldToCell(rb.position));
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
            
            BoardManager.instance.RemoveEntityPosition(entity.GetGridPosition());
        }
    }
}