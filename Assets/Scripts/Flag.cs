using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
public class Flag : EntityBehaviour<IFlag>
{
    [SerializeField] GameObject mesh;
    private Vector3 _basePos;
    public override void Attached()
    {
        base.Attached();
        _basePos = transform.position;
        state.SetTransforms(state.FlagTransform, transform);
    }


    public bool OutBase()
    {
        return _basePos != transform.position;
    }

    public void ToBase()
    {
        transform.position = _basePos;
    }
    public bool PickUp()
    {
        if (mesh.active)
        {
            mesh.SetActive(false);
            return true;
        }
        return false;
    }

    public void Drop(Vector3 pos)
    {
        transform.position = pos;
        mesh.SetActive(true);
    }
}
