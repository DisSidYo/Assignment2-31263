using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class TweenerNormal : MonoBehaviour
{
    // Start is called before the first frame update
    private TweenNormal activeTween;
    void Start()
    {
        activeTween = null;

    }

    // Update is called once per frame
// ...existing code...
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
                activeTween = null; // TweenNormal finished
            }
            else
            {
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
            activeTween = new TweenNormal(target, startPos, endPos, Time.time, duration);
            return true;
        }
        return false;
    }
// ...existing code...
    public bool isTweening()
    {
        return activeTween != null;
    }
}
        
