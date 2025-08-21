using UnityEngine;

public class DronePatrolState : State
{
    private DroneController droneController;
    private float patrolTimer;

    private void Awake()
    {
        typestate = TypeState.DronePatrol;
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
        patrolTimer = droneController.patrolCheckInterval;
        droneController.StartPatrolMovement();
        if (droneController.Player != null)
        {
            droneController.SetTarget(null);
        }
    }

    public override void Execute()
    {
        base.Execute();
        
        patrolTimer -= Time.deltaTime;
        if(patrolTimer <= 0)
        {
            patrolTimer = droneController.patrolCheckInterval;
            
            Transform potentialTarget = droneController.FindBestTarget();
            if (potentialTarget != null)
            {
                droneController.SetTarget(potentialTarget);
                _StateMachine.ChangeState(TypeState.DroneAlert);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        droneController.StopMovement();
    }
}