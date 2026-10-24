using UnityEngine;

/// Gradually speeds the whole game up over a run, so the challenge escalates
/// instead of staying flat. Shares the ScoreTicker GameObject's active
/// lifecycle (on during Playing, off otherwise).
public class DifficultyRamp : MonoBehaviour
{
    [SerializeField] private float speedPerSecond = 0.15f;
    [SerializeField] private float maxSpeed = 14f;

    private void Update()
    {
        float next = GameSpeed.BaseSpeed + speedPerSecond * Time.deltaTime;
        GameSpeed.SetBaseSpeed(Mathf.Min(next, maxSpeed));
    }
}
