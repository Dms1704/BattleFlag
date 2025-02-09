using System;
using UnityEngine;

public class EntityStateMachine : MonoBehaviour
{
    public EntityState currentState { get; private set; }

    public void Initialize(EntityState _currentState)
    {
        currentState = _currentState;
        currentState.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}