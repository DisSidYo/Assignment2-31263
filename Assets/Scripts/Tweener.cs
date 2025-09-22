using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Tweener : MonoBehaviour
{
    // Start is called before the first frame update
    private Tween activeTween;
    void Start()
    {
        activeTween = null;

    }

    // Update is called once per frame
    void Update()
    {
        if (activeTween != null)
        {
            float distance = Vector3.Distance(activeTween.Target.position, activeTween.EndPos);
            float elapsedTime = Time.time - activeTween.StartTime;
            float t = elapsedTime / activeTween.Duration;

            if (t >= 1.0f)
            {
                activeTween.Target.position = activeTween.EndPos;
                activeTween = null; // Tween finished
            }
            else
            {
                // Lerp between StartPos and EndPos based on fraction
                activeTween.Target.position = Vector3.Lerp(
                        activeTween.StartPos,
                        activeTween.EndPos,
                        t
                );
            }
        }

    }
    public bool AddTween(Transform target, Vector3 startPos, Vector3 endPos, float duration)
    {
        if (activeTween == null)
        {
            activeTween = new Tween(target, startPos, endPos, Time.time, duration);
            return true;
        }
        return false;

    }
    public bool isTweening()
    {
        return activeTween != null;
    }
}
        
