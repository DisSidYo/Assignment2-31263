using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{
[SerializeField] private GameObject cherryPrefab;
    private GameObject currentCherry;
    private float spawnDelay = 5f;
    private float nextSpawnTime;
    private TweenerNormal tweener;
    private Vector3 center;
    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        tweener = GetComponent<TweenerNormal>();
        // Assuming this script is attached to the LevelMask (which covers the level)
        Vector3 scale = transform.localScale;
        Vector3 position = transform.position;

        // Compute half extents
        halfWidth = scale.x / 18f;
        halfHeight = scale.y / 18f;

        // The LevelMask's position is its center
        center = position;

        // Start the spawn timer so first spawn occurs spawnDelay seconds after scene start
        nextSpawnTime = Time.time + spawnDelay;
    }

    void Update()
    {
        // Spawn when timer elapsed and there's no current cherry
        if (currentCherry == null && Time.time >= nextSpawnTime)
        {
            SpawnCherry();
        }

        if (currentCherry != null)
        {
            Vector3 pos = currentCherry.transform.position;

            // Level bounds-based destruction (keep your existing margin)
            bool outsideBounds =
                Mathf.Abs(pos.x - center.x) > halfWidth + 2f ||
                Mathf.Abs(pos.y - center.y) > halfHeight + 2f;

            // Also destroy if outside camera viewport (guarantees off-screen removal)
            bool outsideCamera = false;
            if (Camera.main != null)
            {
                Vector3 vp = Camera.main.WorldToViewportPoint(pos);
                if (vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f) outsideCamera = true;
            }

            if (outsideBounds || outsideCamera  || !tweener.isTweening())
            {
                Destroy(currentCherry);
                currentCherry = null;
                // Set next spawn relative to destruction time (5s after destroyed)
                nextSpawnTime = Time.time + spawnDelay;
            }
        }
    }

    private void SpawnCherry()
    {
        // Get random spawn location just outside bounds
        Vector3 spawnPos = GetRandomSpawnPosition();

        // Move toward and past the camera view on the opposite side of spawn
        Vector3 dir = (center - spawnPos).normalized;
        Camera cam = Camera.main;
        Vector3 endPos;
        // if (cam != null && cam.orthographic)
        // {
        //     Vector3 camCenter = cam.transform.position;
        //     float halfH = cam.orthographicSize;
        //     float halfW = halfH * cam.aspect;

        //     // compute t where camCenter + dir * t hits a camera edge along x or y
        //     float tx = (Mathf.Approximately(dir.x, 0f)) ? float.PositiveInfinity : (halfW / Mathf.Abs(dir.x));
        //     float ty = (Mathf.Approximately(dir.y, 0f)) ? float.PositiveInfinity : (halfH / Mathf.Abs(dir.y));

        //     // choose the smaller t so the point is just beyond whichever edge is hit first
        //     float t = Mathf.Min(tx, ty);
        //     // place end position 1 unit beyond that edge along the same line
        //     endPos = camCenter + dir * (t + 1f);
        //     // keep z consistent with level (0)
        //     endPos.z = 0f;
        // }
        // else
        // {
            // fallback: mirror spawn through center (previous behavior)
        endPos = center + (center - spawnPos);
        // }

        // Instantiate the cherry
        currentCherry = Instantiate(cherryPrefab, spawnPos, Quaternion.identity);

        // Move the cherry toward the center of the level
        tweener.AddTween(currentCherry.transform, spawnPos, endPos, 15f);

        nextSpawnTime = Time.time + spawnDelay;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 pos = Vector3.zero;
        float spawnOffset = 1f; // distance outside the bounds to spawn
        
        // Randomly choose one side
        int side = Random.Range(0, 4);
        
        switch (side)
        {
            case 0: // Left side
                pos = transform.position + new Vector3(-halfWidth - spawnOffset, 
                    Random.Range(-halfHeight, halfHeight), 
                    0);
                break;
            case 1: // Right side
                pos = transform.position + new Vector3(halfWidth + spawnOffset, 
                    Random.Range(-halfHeight, halfHeight), 
                    0);
                break;
            case 2: // Top side
                pos = transform.position + new Vector3(
                    Random.Range(-halfWidth, halfWidth),
                    halfHeight + spawnOffset, 
                    0);
                break;
            case 3: // Bottom side
                pos = transform.position + new Vector3(
                    Random.Range(-halfWidth, halfWidth),
                    -halfHeight - spawnOffset, 
                    0);
                break;
        }

        return pos;
    }
}
