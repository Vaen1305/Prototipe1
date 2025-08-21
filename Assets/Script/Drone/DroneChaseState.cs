using UnityEngine;

public class DroneChaseState : State
{
    private DroneController droneController;

    private void Awake()
    {
        typestate = TypeState.DroneChase;
        LocadComponent();
    }

    public override void LocadComponent()
    {
        base.LocadComponent();
        droneController = GetComponent<DroneController>();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        if (droneController.Player == null || !droneController.Player.gameObject.activeInHierarchy)
        {
            _StateMachine.ChangeState(TypeState.DronePatrol);
            return;
        }

        droneController.UpdateChase();
        
        if(droneController.IsPlayerInAttackRange())
        {
            _StateMachine.ChangeState(TypeState.DroneAttack);
        }
        else if(!droneController.IsPlayerInChaseRange())
        {
            _StateMachine.ChangeState(TypeState.DroneAlert);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}