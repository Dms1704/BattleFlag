using System;
using System.Collections.Generic;
using Models;
using Sylves;
using UnityEngine;

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
    private Vector3Int currentMoveToCell;
    private SkillManager skillManager;
    
    protected override void Start()
    {
        base.Start();
        camera = Camera.main;
        board = BoardManager.instance;
        skillManager = SkillManager.instance;

        TurnOrderManager.instance.StartTurn();
    }

    protected override void Update()
    {
        base.Update();

        if (isOperating)
        {
            // 移动判定
            if (Input.GetKeyDown(KeyCode.Mouse0) && !isBusy)
            {
                // 技能键按下
                if (attackReady)
                {
                    Vector3 screenToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
                    Entity entity = board.GetEntity(HexGridUtil.IgnoreZ(tilemap.WorldToCell(screenToWorldPoint)));
                    if (entity != null)
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
                    board.GenerateAttackHexMasks(transform, skill.scope);
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
            StartCoroutine(nameof(BusyFor), .2f);
        }

        attackReady = false;
        board.ClearHexMasks();
    }

    private void MoveLogic()
    {
        // 第二次点击相同位置：移动
        Vector3Int clickCell = GetMouseInputCell();
        if (clickCell.Equals(currentMoveToCell))
        {
            Move();
            currentMoveToCell = new Vector3Int(Int32.MaxValue, 0, 0);
            return;
        }

        // 第一次点击：生成脚印+移动距离判定：基于角色的行动点
        currentMoveToCell = GetMouseInputCell();
        ClearFootprints();

        IGrid grid = HexGridUtil.GenerateGrid();
        CellPath path = HexGridUtil.FindPath(transform, grid);
                
        if (path != null)
        {
            IList<Step> steps = path.Steps;
            moveStepLos = new List<MoveStepLO>();
            int actionPoint = stats.GetStat(StatType.actionPoint).GetValue();
            for (int i = 0; i < steps.Count && actionPoint > 1; i++)
            {
                // 基于行动点判断角色能够走到哪
                Step step = steps[i];
                Vector3Int cellPos = tilemap.WorldToCell(grid.GetCellCenter(step.Dest));
                HexTerrainTile tile = tilemap.GetTile<HexTerrainTile>(cellPos);
                Vector3 end = transform.TransformPoint(grid.GetCellCenter(step.Dest));
                GenerateFootprints(step, end);

                if (tile != null)
                {
                    actionPoint -= tile.moveCost;
                    moveStepLos.Add(new MoveStepLO(end, tile.moveCost));
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
        stats.RecoveryActionPoint();
        Debug.Log("轮到你了");
    }
}
