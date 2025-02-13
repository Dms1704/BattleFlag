using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnOrderManager : MonoBehaviour
{
    public static TurnOrderManager instance;
    private List<Entity> sortedEntityList;

    // 当前操作的角色下标
    private int index = 0;
    
    // 回合次数
    private bool started = false;
    private int turnOrder = 0;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 确保只有一个实例
        }
    }

    public Entity GetCurrentEntity()
    {
        return sortedEntityList[index];
    }
    
    public void EndOperate()
    {
        Entity entity = sortedEntityList[index];
        entity.OnOperateOverChanged -= EndOperate;

        Debug.Log("操作结束");
        index++;
        StartNextOperate();
    }

    public void StartNextOperate()
    {
        if (index >= sortedEntityList.Count)
        {
            StartTurnWithIn();
            return;
        }
        
        Debug.Log("轮到" + sortedEntityList[index].name);
        Entity entity = sortedEntityList[index];
        entity.OnOperateOverChanged += EndOperate;
        entity.Operate();
    }

    // 开始一个新的回合
    public void StartTurn()
    {
        if (started)
        {
            return;
        }

        StartTurnWithIn();
    }

    private void StartTurnWithIn()
    {
        started = true;
        Debug.Log("回合次数：" + turnOrder);
        turnOrder++;
        index = 0;
        
        // 根据速度排序角色列表
        sortedEntityList = BoardManager.instance.GetEntities().OrderByDescending(e => e.stats.velocity.GetValue()).ToList();
        StartNextOperate();
    }
}
