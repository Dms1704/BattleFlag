using UnityEngine;

namespace Models
{
    public class MoveStepLO
    {
        public Vector3 targetPos { get; set; }
        public int moveCost { get; set; }

        public MoveStepLO(Vector3 targetPos, int moveCost)
        {
            this.targetPos = targetPos;
            this.moveCost = moveCost;
        }
    }
}