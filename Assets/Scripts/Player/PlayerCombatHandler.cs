using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class PlayerCombatHandler : EntityBehaviour<IPlayerState>
{
    [SerializeField] private int _localHealth;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private int _maxDamage;
    [SerializeField] private int _minDamage;
    [SerializeField] private Rigidbody _bullet;
    [SerializeField] private Transform _bulletOrigin;


    private float _nextFireTime;
    private bool _dead;
    public PlayerManager manager;
    public float fireDelay = 0.1f;

    public override void Attached()
    {
        base.Attached();
        state.Health = _localHealth;
        state.AddCallback("Health", HealthCallback);
        state.OnHit = ReceiveDamage;

        // ReceiveDamage();
    }



    public void Fire(Transform aim)
    {
        if (_nextFireTime < Time.time)
        {
            _nextFireTime = Time.time + fireDelay;

            if (entity.IsOwner)
            {
                Rigidbody newBullet = BoltNetwork.Instantiate(_bullet.gameObject, _bulletOrigin.position, aim.rotation).GetComponent<Rigidbody>();
                newBullet.velocity = aim.TransformDirection(new Vector3(0, 0, _bulletSpeed));
            }
        }
    }

    private void HealthCallback()
    {
        _localHealth = state.Health;
        if (_localHealth == 0)
        {
            _dead = true;
            Die();
        }
    }

    public void ReceiveDamage()
    {
        if (_dead)
            return;
        int damage = UnityEngine.Random.Range(_minDamage, _maxDamage);
        state.Health -= damage;

        if (state.Health <= 0)
        {
            state.Health = 0;
        }
    }

    private void Die()
    {
        //Morre
        StartCoroutine(Respawn());
    }
    private IEnumerator Respawn()
    {
        manager.DieAction?.Invoke();
        yield return new WaitForSeconds(10);
        manager.RespawnAction?.Invoke();
        _dead = false;
    }

}
