using System.Collections.Generic;
using Models;

namespace States
{
    public class EntityMoveState : EntityState
    {
        // 角色要移到的地方
        private IList<MoveStepLO> moveSteps;
        
        public EntityMoveState(Entity entity, EntityStateMachine _stateMachine, string _animBoolName) : 
            base(entity, _stateMachine, _animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            entity.StartCoroutine(entity.MoveSequentially(moveSteps, () =>
            {
                stateMachine.ChangeState(entity.idleState);
                if (entity.stats.GetStat(StatType.actionPoint).GetValue() <= 1)
                {
                    entity.OperateOver();
                }
            }));
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
            
            entity.ClearFootprints();
        }

        public void SetMoveStepLOs(IList<MoveStepLO> moveSteps)
        {
            this.moveSteps = moveSteps;
        }
    }
}