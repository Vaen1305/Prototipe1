using UnityEngine;

public class DroneAttackState : State
{
    private DroneController droneController;
    private float attackCooldown;

    private void Awake()
    {
        typestate = TypeState.DroneAttack;
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
        attackCooldown = 0f;
    }

    public override void Execute()
    {
        base.Execute();
        attackCooldown -= Time.deltaTime;

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
        
        if(attackCooldown <= 0)
        {
            droneController.PerformAttack();
            attackCooldown = 1f / droneController.attackRate;
        }
        
        if(!droneController.IsPlayerInAttackRange())
        {
            _StateMachine.ChangeState(TypeState.DroneAlert);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}