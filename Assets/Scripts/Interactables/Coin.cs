using System;

public class Coin : Interactable
{
    public static event Action Collected;

    public override void OnPlayerHit(PlayerController player)
    {
        Collected?.Invoke();
        Despawn();
    }
}
