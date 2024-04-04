using UnityEngine;
using SpaceGame.General;

public class EffectDestroy : VisualEffect
{
    public override void Init(Vector3 startPosition)
    {
        transform.position = startPosition;
    }

    protected override void OnLifetimeEnd()
    {
        PoolManager.Return(this);
    }

    public override void ResetState()
    {
        base.ResetState();
        OnLifetimeEnd();
    }
}