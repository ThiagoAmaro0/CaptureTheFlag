using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using System;
using UnityEngine.SceneManagement;

public class PlayerManager : EntityBehaviour<IPlayerState>
{
    [SerializeField] private GameObject _dead;
    [SerializeField] private GameObject _mesh;
    [SerializeField] protected GameObject _flag;
    [SerializeField] protected PlayerCombatHandler _combatHandler;
    [SerializeField] protected Animator anim;
    protected Flag flagT1;
    protected Flag flagT2;
    private Vector3 _startPos;
    bool initialized;
    public Action DieAction;
    public Action RespawnAction;
    private bool _gameover;
    public override void Attached()
    {
        base.Attached();
        state.SetTransforms(state.PlayerTransform, transform);
        DieAction += Die;
        RespawnAction += Respawn;
        state.AddCallback("IsWalking", WalkingCallback);
        _startPos = transform.position;
        _combatHandler.manager = this;

    }

    public override void Detached()
    {
        base.Detached();
        DieAction -= Die;
        RespawnAction -= Respawn;
    }
    private void WalkingCallback()
    {
        anim.SetBool("Walking", state.IsWalking);
    }

    protected void Die()
    {
        TeamManager.instance.Score(state.IsBlue);
        _mesh.SetActive(false);
        _dead.SetActive(true);
        // state.Die();
        if (TeamManager.instance.state.Started)
        {
            GetFlags();
            if (_flag.active)
            {
                if (state.IsBlue)
                {
                    flagT2.Drop(transform.position);
                }
                else
                {
                    flagT1.Drop(transform.position);
                }
                _flag.SetActive(false);
            }
        }

        _combatHandler.enabled = false;
    }
    protected void GetFlags()
    {
        if (flagT1 == null || flagT2 == null)
        {
            flagT1 = GameObject.Find("FlagT1(Clone)").GetComponent<Flag>();
            flagT2 = GameObject.Find("FlagT2(Clone)").GetComponent<Flag>();
        }
    }
    protected void Respawn()
    {

        state.Health = 100;
        _mesh.SetActive(true);
        _dead.SetActive(false);
        _combatHandler.enabled = true;
        transform.position = _startPos;
    }

    protected void Init()
    {
        // _rb = GetComponent
        initialized = true;
    }



    private void OnTriggerStay(Collider other)
    {
        if (state.Health > 0)
        {
            if (other.TryGetComponent<Flag>(out Flag flag))
            {
                if (flag.state.IsBlue == state.IsBlue)
                {
                    if (flag.OutBase())
                    {
                        if (!flag.state.IsPicked)
                            flag.ToBase();
                    }
                    else if (_flag.active)
                    {
                        if (!_gameover)
                        {
                            _gameover = true;
                            Debug.Log(state.IsBlue ? "Blue Team Wins!" : "Red Team Wins!");
                            TeamManager.instance.GameOver(state.IsBlue ? 0 : 1);
                        }
                    }
                }
                else
                {
                    if (!flag.state.IsPicked)
                    {
                        if (flag.PickUp())
                            _flag.SetActive(true);
                    }
                }
            }
        }
    }
}
