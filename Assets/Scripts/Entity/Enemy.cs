using UnityEngine;

public class Enemy : Entity
{
    protected override void Start()
    {
        base.Start();

        isAlly = false;
        faceRight = -1;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Operate()
    {
        base.Operate();
        
        Debug.Log("轮到敌方了");
        Debug.Log("敌方攻击了");
        OperateOver();
    }
}
