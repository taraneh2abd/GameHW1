using System;
using UnityEngine;

public class Coin : Interactable
{
    public static event Action<Vector3> Collected;

    public override void OnPlayerHit(PlayerController player)
    {
        Collected?.Invoke(transform.position);
        Despawn();
    }
}
