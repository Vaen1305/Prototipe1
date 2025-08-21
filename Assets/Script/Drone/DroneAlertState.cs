using UnityEngine;

public class DroneAlertState : State
{
    private DroneController droneController;
    private float alertTimer;

    private void Awake()
    {
        typestate = TypeState.DroneAlert;
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
        alertTimer = 0f;
    }

    public override void Execute()
    {
        base.Execute();
        alertTimer += Time.deltaTime;

        if (droneController.Player == null || !droneController.Player.gameObject.activeInHierarchy)
        {
            _StateMachine.ChangeState(TypeState.DronePatrol);
            return;
        }
        
        Vector3 directionToPlayer = (droneController.Player.position - transform.position).normalized;
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, droneController.rotationSpeed * Time.deltaTime);
        }

        if(droneController.IsPlayerInAttackRange())
        {
            _StateMachine.ChangeState(TypeState.DroneAttack);
        }
        else if(droneController.IsPlayerInChaseRange())
        {
            _StateMachine.ChangeState(TypeState.DroneChase);
        }
        else if(alertTimer > droneController.alertDuration)
        {
            _StateMachine.ChangeState(TypeState.DronePatrol);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}