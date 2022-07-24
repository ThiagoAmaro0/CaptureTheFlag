using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class Bullet : EntityBehaviour<IBullet>
{
    private Vector3 startPos;
    public override void Attached()
    {
        base.Attached();
        startPos = transform.position;
        state.SetTransforms(state.BulletTransform, transform);
    }

    public override void SimulateOwner()
    {
        base.SimulateOwner();
        if (Vector3.Distance(transform.position, startPos) > 100)
        {
            BoltNetwork.Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerCombatHandler combatHandler = other.transform.GetComponentInParent<PlayerCombatHandler>();
        if (combatHandler)
        {
            print("TRIGGER");
            combatHandler.state.Hit();
            StartCoroutine(DestroyBullet());
            return;
        }
        StartCoroutine(DestroyBullet());

    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(0.1f);
        BoltNetwork.Destroy(gameObject);
    }
}
