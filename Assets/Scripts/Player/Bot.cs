using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : PlayerManager
{
    [SerializeField] private Transform _aim;
    [SerializeField] private Rigidbody _rb;
    private BotState _botState;
    private Transform _target;
    private NavMeshAgent _agent;

    public override void Attached()
    {
        base.Attached();
        if (entity.IsOwner)
        {
            _rb = GetComponent<Rigidbody>();
            _agent = GetComponent<NavMeshAgent>();
            _combatHandler.fireDelay = 0.5f;
            DieAction += BotDie;
            RespawnAction += BotRespawn;
        }
    }

    private void GetTarget()
    {
        GetFlags();
        SetState(BotState.Walking);
        if (_flag.active)
        {
            _target = state.IsBlue ? flagT1.transform : flagT2.transform;
        }
        else
        {
            _target = state.IsBlue ? flagT2.transform : flagT1.transform;
        }
    }

    private void FindPlayers()
    {
        List<PlayerManager> players = TeamManager.instance.players;

        foreach (PlayerManager p in players)
        {
            if (p.state.IsBlue != state.IsBlue)
            {
                if (p.state.Health > 0)
                    if (Vector3.Distance(p.transform.position, transform.position) < 20)
                    {
                        SetState(BotState.Firing);
                        _target = p.transform;

                    }
            }
        }
    }


    public override void SimulateOwner()
    {
        base.SimulateOwner();
        switch (_botState)
        {
            case BotState.Idle:
                GetTarget();
                FindPlayers();
                _agent.destination = transform.position;
                break;
            case BotState.Walking:
                GetTarget();
                FindPlayers();
                _agent.acceleration = 8;
                _agent.isStopped = false;
                _agent.destination = _target.position;
                break;
            case BotState.Firing:
                GetTarget();
                FindPlayers();
                _agent.destination = _target.position;
                _agent.isStopped = true;
                _aim.LookAt(_target.transform.position + new Vector3(0, 1.5f, 0));
                transform.LookAt(_target.transform.position);
                _combatHandler.Fire(_aim);
                break;
            case BotState.Dead:
                _agent.destination = transform.position;
                _agent.isStopped = true;
                _rb.velocity = Vector3.zero;
                break;
            default:
                break;
        }
    }

    private void BotDie()
    {
        SetState(BotState.Dead);
    }

    private void BotRespawn()
    {
        SetState(BotState.Idle);
    }

    private void SetState(BotState newState)
    {
        _botState = newState;
        switch (newState)
        {
            case BotState.Idle:
                state.IsWalking = false;
                break;
            case BotState.Walking:
                state.IsWalking = true;
                break;
            case BotState.Firing:
                state.IsWalking = false;
                break;
            case BotState.Dead:
                state.IsWalking = false;
                break;
            default:
                break;
        }
    }

    private enum BotState
    {
        Idle,
        Walking,
        Firing,
        Dead,
    }
}
