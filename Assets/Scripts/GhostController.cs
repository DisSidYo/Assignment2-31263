// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// public class GhostController : MonoBehaviour
// {
//     public enum MovementStyle { AwayFromPac, TowardPac, Random, ClockwisePerimeter }

//     [SerializeField] MovementStyle movementStyle = MovementStyle.Random;
//     [SerializeField] float tickDelay = 0.01f;

//     Tweener tweener;
//     GhostStateManager gsm;
//     GhostVisuals visuals;
//     Vector2Int gridPos;
//     Vector2Int lastDir = Vector2Int.right;
//     Queue<Vector2Int> pathQueue = new Queue<Vector2Int>();
//     bool onPerimeter;
//     float nextTick;
//     TilemapLevel level;
//     static readonly Vector2Int[] dirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

//     void Awake()
//     {
//         tweener = FindFirstObjectByType<Tweener>();
//         gsm = GetComponent<GhostStateManager>();
//         visuals = GetComponent<GhostVisuals>();
//         level = TilemapLevel.I;
//     }

//     void Start()
//     {
//         gridPos = level.WorldToGrid(transform.position);
//         onPerimeter = level.IsOutsidePerimeter(gridPos);
//         UpdateVisualDir(lastDir);
//     }
//     void Update()
//     {
//         if (gsm != null && gsm.IsFrozen) return;
//         if (gsm.MovementOverrideActive) return;
//         if (Time.time < nextTick) return;
//         nextTick = Time.time + tickDelay;

//         if (!tweener.TweenExists(transform))
//             gridPos = level.WorldToGrid(transform.position);

//         if (gsm.CurrentState == GhostStateManager.GhostState.Dead) return;
//         if (tweener.TweenExists(transform)) return;

//         var style = EffectiveStyle();

//         switch (style)
//         {
//             case MovementStyle.ClockwisePerimeter:
//                 onPerimeter = level.IsOutsidePerimeter(gridPos);
//                 HandleClockwisePerimeter();
//                 break;

//             case MovementStyle.Random:
//                 HandleRandomMovement();
//                 break;

//             case MovementStyle.AwayFromPac:
//                 pathQueue.Clear();
//                 HandleDirectionalMovement(awayFromPac: true);
//                 break;

//             case MovementStyle.TowardPac:
//                 HandleDirectionalMovement(awayFromPac: false);
//                 break;

//             default:
//                 HandleRandomMovement();
//                 break;
//         }
//     }
//     // private MovementStyle EffectiveStyle()
//     // {
//     //     if (gsm.CurrentState == GhostStateManager.GhostState.Scared)
//     //         return MovementStyle.Random;
//     //     return movementStyle;


//     // }
// }
