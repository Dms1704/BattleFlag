using System;
using System.Collections.Generic;
using Models;
using Prefabs;
using Sylves;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Entity
{
    // 预制体
    public GameObject footprintPrefab;

    // 组件
    private Camera camera;
    private BoardManager board;
    
    // 常量
    private static readonly float defaultFootprintAngle = -60;

    // 数据
    private bool attackReady = false;
    private IList<GameObject> footprints = new List<GameObject>();
    private Vector3Int currentMoveToCell = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
    private SkillManager skillManager;
    private List<Entity> enemies = new List<Entity>();
    
    // 事件
    
    protected override void Start()
    {
        base.Start();
        camera = Camera.main;
        board = BoardManager.instance;
        skillManager = SkillManager.instance;
    }

    protected override void Update()
    {
        base.Update();

        if (isOperating)
        {
            // 移动判定
            if (Input.GetKeyDown(KeyCode.Mouse0) && !isBusy)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                // 技能键按下
                if (attackReady)
                {
                    Vector3 screenToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
                    Vector3Int cellPos = HexGridUtil.IgnoreZ(tilemap.WorldToCell(screenToWorldPoint));
                    Entity entity = board.GetEntity(cellPos);
                    if (entity != null && enemies.Contains(entity))
                    {
                        UseSkill(entity);
                    }
                }
                else
                {
                    MoveLogic();
                }
            }
            else if (Input.GetMouseButtonDown(1) && !isBusy)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                ClearFootprints();
                board.ClearHexMasks();
                attackReady = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && !isBusy)
            {
                ClearFootprints();
                Skill skill = skillManager.GetSkill(SkillType.Melee);
                if (!attackReady)
                {
                    enemies = new List<Entity>();
                    board.GenerateAttackHexMasks(transform, skill.scope, enemies);
                    attackReady = true;
                }
            }
        }
    }

    private void UseSkill(Entity entity)
    {
        Skill skill = skillManager.GetSkill(SkillType.Melee);
        if (entity is Enemy)
        {
            skill.UseSkill(entity);
            stateMachine.ChangeState(attackState);
            stats.DoDamage(entity.stats);
        }

        attackReady = false;
        board.ClearHexMasks();
    }

    private void MoveLogic()
    {
        // 第二次点击相同位置：移动
        Vector3Int clickCell = GetMouseInputCell();
        if (board.GetEntity(clickCell) != null)
        {
            return;
        }
        if (clickCell.Equals(currentMoveToCell))
        {
            Move();
            currentMoveToCell = new Vector3Int(Int32.MaxValue, 0, 0);
            return;
        }

        // 第一次点击：生成脚印+移动距离判定：基于角色的行动点
        currentMoveToCell = GetMouseInputCell();
        ClearFootprints();

        CellPath path = HexGridUtil.FindPath(this);
                
        if (path != null)
        {
            IList<Step> steps = path.Steps;
            moveStepLos = new List<MoveStepLO>();
            int actionPoint = stats.GetStat(StatType.actionPoint).GetValue();
            for (int i = 0; i < steps.Count && actionPoint > 1; i++)
            {
                // 基于行动点判断角色能够走到哪
                Step step = steps[i];
                Vector3Int cellPos = tilemap.WorldToCell(HexGridUtil.GetCellCenter(step.Dest));
                DataAccessible tile = (DataAccessible)tilemap.GetTile(cellPos);
                Vector3 end = transform.TransformPoint(HexGridUtil.GetCellCenter(step.Dest));
                GenerateFootprints(step, end);

                if (tile != null)
                {
                    actionPoint -= tile.MoveCost();
                    moveStepLos.Add(new MoveStepLO(end, tile.MoveCost()));
                }
                else
                {
                    throw new ArgumentException("step.Dest 计算错误，tilemap 中不存在此坐标点");
                }
            }
        }
    }

    private Vector3Int GetMouseInputCell()
    {
        return HexGridUtil.IgnoreZ(tilemap.WorldToCell(camera.ScreenToWorldPoint(Input.mousePosition)));
    }

    private void Move()
    {
        moveState.SetMoveStepLOs(moveStepLos);
        stateMachine.ChangeState(moveState);
    }

    public override void ClearFootprints()
    {
        for (int i = 0; i < footprints.Count; i++)
        {
            Destroy(footprints[i]);
        }

        footprints.Clear();
    }

    // 脚印生成
    private void GenerateFootprints(Step step, Vector3 end)
    {
        GameObject footprint = Instantiate(footprintPrefab);
        footprint.transform.localRotation = Quaternion.Euler(0, 0, defaultFootprintAngle + Convert.ToInt32(step.Dir) * 60);
        footprint.transform.position = end;
        footprints.Add(footprint);
    }

    public override void Operate()
    {
        base.Operate();
        Debug.Log("轮到你了");
    }
}
