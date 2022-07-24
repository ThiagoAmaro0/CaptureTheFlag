using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : PlayerManager
{
    [SerializeField] private FirstPersonController _fpsController;

    public override void Attached()
    {
        base.Attached();
        if (entity.IsOwner)
        {
            _fpsController.Init();
            DieAction += PlayerDie;
            RespawnAction += PlayerRespawn;
        }
    }
    public override void SimulateOwner()
    {
        base.SimulateOwner();
        if (state.Health <= 0)
            return;
        _fpsController.Move();
        _fpsController.UpdateMovement();
        state.IsWalking = _fpsController.isWalking;
        if (Input.GetMouseButton(0))
        {
            _combatHandler.Fire(GetCamera().transform);
        }
    }
    private void PlayerDie()
    {
        _fpsController.cameraCanMove = false;
        _fpsController.playerCanMove = false;
        _fpsController.rb.velocity = new Vector3();
        _fpsController.rb.useGravity = false;
    }

    private void PlayerRespawn()
    {
        _fpsController.rb.useGravity = true;
        _fpsController.cameraCanMove = true;
        _fpsController.playerCanMove = true;
    }

    public Camera GetCamera()
    {
        return _fpsController.playerCamera;
    }
}
