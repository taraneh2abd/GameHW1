/// Shared contact behavior for anything that costs the player a life.
public abstract class DamagingObstacle : Interactable
{
    public override void OnPlayerHit(PlayerController player)
    {
        var damageable = player.GetComponent<IDamageable>();
        damageable?.TakeDamage();
        Despawn();
    }
}
