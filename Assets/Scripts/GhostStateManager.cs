using UnityEngine;

public enum GhostState { Normal, Scared, Recovering, Dead }

public class GhostStateManager : MonoBehaviour
{
    public GhostState State { get; private set; } = GhostState.Normal;
    private Animator animator;

    // stored at Awake for respawn
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    // movement pause flag (other movement scripts should check this)
    public bool IsFrozen { get; private set; } = false;

    private void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // helper to play per-state animation safely
    void PlayStateAnimation(GhostState s)
    {
        if (animator == null) return;
        string name = s == GhostState.Normal ? "Normal"
                    : s == GhostState.Scared ? "Ghost_Scared_Up"
                    : s == GhostState.Recovering ? "Ghost_Recovery"
                    : "Ghost_Death";
        // only try to Play if state name likely exists; caller can override names per-ghost if needed
        if (!string.IsNullOrEmpty(name)) animator.Play(name);
    }

    public void FreezeMovement(bool freeze)
    {
        IsFrozen = freeze;
        if (animator != null) animator.enabled = !freeze;
    }

    public void ResetToInitial()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        IsFrozen = false;
        if (animator != null) animator.enabled = true;
        State = GhostState.Normal;
        PlayStateAnimation(State);
    }

    public void SetScared()
    {
        if (State == GhostState.Dead) return;
        State = GhostState.Scared;
        PlayStateAnimation(State);
        // adjust behaviour: slow, flee, etc.
    }

    public void SetRecovering()
    {
        if (State == GhostState.Dead) return;
        State = GhostState.Recovering;
        PlayStateAnimation(State);
        // adjust behaviour if needed
    }

    public void SetNormal()
    {
        if (State == GhostState.Dead) return;
        State = GhostState.Normal;
        PlayStateAnimation(State);
        // restore normal behaviour
    }

    // When ghost dies you must set State = Dead and handle animation separately.
    public void SetDead()
    {
        State = GhostState.Dead;
        PlayStateAnimation(State);
        // freeze movement visually until respawn
        FreezeMovement(true);
    }
}