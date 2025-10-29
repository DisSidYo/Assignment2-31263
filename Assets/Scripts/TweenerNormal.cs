using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenerNormal : MonoBehaviour
{
    private TweenNormal activeTween;

    void Update()
    {
        if (activeTween == null) return;

        // If the target was destroyed, Unity lets it compare equal to null.
        if (activeTween.Target == null)
        {
            activeTween = null;
            return;
        }

        float elapsedTime = Time.time - activeTween.StartTime;
        float t = activeTween.Duration <= 0f ? 1f : elapsedTime / activeTween.Duration;

        if (t >= 1.0f)
        {
            // final snap
            activeTween.Target.position = activeTween.EndPos;
            activeTween = null;
        }
        else
        {
            activeTween.Target.position = Vector3.Lerp(activeTween.StartPos, activeTween.EndPos, t);
        }
    }

    public bool AddTween(Transform target, Vector3 startPos, Vector3 endPos, float duration)
    {
        if (target == null) return false;
        if (activeTween == null)
        {
            activeTween = new TweenNormal(target, startPos, endPos, Time.time, duration);
            return true;
        }
        return false;
    }

    // Call this before destroying a GameObject to avoid keeping a dangling reference.
    public void RemoveTweenFor(Transform target)
    {
        if (activeTween != null && activeTween.Target == target)
        {
            activeTween = null;
        }
    }

    public bool isTweening()
    {
        return activeTween != null;
    }
    public void CancelAllTweens()
    {
        activeTween = null;
    }
}

