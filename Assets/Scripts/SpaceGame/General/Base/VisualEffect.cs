using System.Collections;
using UnityEngine;
using Zenject;
using SpaceGame.General;

public abstract class VisualEffect : MonoBehaviour, IReset
{
    #region Properties
    [SerializeField] protected float _lifetime;

    private Coroutine _myCoroutine;
    protected GameStateManager GameStateManager { get; private set; }
    protected PoolManager PoolManager { get; private set; }
    #endregion

    #region Merhods
    [Inject]
    public void Construct(GameStateManager gameStateManager, PoolManager poolManager)
    {
        GameStateManager = gameStateManager;
        PoolManager = poolManager;
    }

    private void OnEnable()
    {
        _myCoroutine = StartCoroutine(LifetimeCoroutine());
    }

    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(_lifetime);
        OnLifetimeEnd();
    }
    public virtual void ResetState()
    {
        StopCoroutine(_myCoroutine);
    }

    public abstract void Init(Vector3 startPosition);
    protected abstract void OnLifetimeEnd();
    #endregion 
}