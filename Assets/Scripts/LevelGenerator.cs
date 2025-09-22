using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LevelGenerator:
/// - Provide a quadrant-level 2D int array called levelMap (top-left quadrant).
/// - Script builds a symmetric full map by mirroring horizontally, vertically and both,
///   using "2*dim - 1" sizing so the central row/column is not duplicated.
/// - Instantiates prefabs and rotates them based on neighbour connectivity.
/// - Adjusts an orthographic camera to fit the generated level.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    // Prefabs (assign in inspector)
    [Header("Prefabs (assign)")]
    public GameObject outsideCornerPrefab; // straight piece (horizontal orientation default)
    public GameObject outsideWallPrefab;       // corner piece (L-shape)
    public GameObject tPrefab;            // T junction

    public GameObject insideCornerPrefab; // straight piece (horizontal orientation default)

    public GameObject insideWallPrefab; 

    public GameObject ghostWallPrefab;        // cross junction
    public GameObject powerPelletPrefab;           // dead-end single wall
    public GameObject pelletPrefab;       // pellet or dot
    // If you use a single "wall" prefab, you can set straightWallPrefab and ignore others.

    [Header("Settings")]
    public float cellSize = 1.0f;
    public Vector2 spawnOffset = Vector2.zero; // offset to shift the whole level
    // public Camera targetCamera; // optional; if null, will use Camera.main
    public float cameraPadding = 1.0f;

    // Example levelMap for top-left quadrant (you'll replace this with your own or CSV loader)
    // Legend: 0 = empty, 1 = wall, 2 = pellet/item
    private int[,] levelMap = new int[,]
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0}
    };

    // runtime parent to contain generated objects (so we can Destroy at Start during Play)
    private GameObject generatedRoot;

    void Start()
    {
        // Ensure we don't keep previous generated instance (useful when restarting Play in editor)
        if (generatedRoot != null)
            DestroyImmediate(generatedRoot);

        generatedRoot = new GameObject("GeneratedLevel");
        generatedRoot.transform.SetParent(this.transform, false);

        // Build a full (mirrored) map from the top-left quadrant
        int[,] full = BuildFullMap(levelMap);

        // Instantiate the full map
        InstantiateFullMap(full);

        // Fit camera
        // FitCameraToMap(full.GetLength(1), full.GetLength(0)); // cols, rows
    }

    /// <summary>
    /// Builds full map by mirroring the quadrant provided in quadrantMap.
    /// Uses full rows = rows*2 - 1 and cols*2 - 1 to avoid duplicating center lines.
    /// </summary>
    private int[,] BuildFullMap(int[,] quadrantMap)
    {
        int rows = quadrantMap.GetLength(0);
        int cols = quadrantMap.GetLength(1);

        int fullRows = rows * 2 - 1;
        int fullCols = cols * 2 - 1;

        int[,] full = new int[fullRows, fullCols];

        // Fill top-left (direct copy)
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                full[r, c] = quadrantMap[r, c];

        // Fill top-right (mirror horizontally)
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int targetCol = fullCols - 1 - c;
                full[r, targetCol] = quadrantMap[r, c];
            }
        }

        // Fill bottom-left (mirror vertically)
        for (int r = 0; r < rows; r++)
        {
            int targetRow = fullRows - 1 - r;
            for (int c = 0; c < cols; c++)
            {
                full[targetRow, c] = quadrantMap[r, c];
            }
        }

        // Fill bottom-right (mirror both)
        for (int r = 0; r < rows; r++)
        {
            int targetRow = fullRows - 1 - r;
            for (int c = 0; c < cols; c++)
            {
                int targetCol = fullCols - 1 - c;
                full[targetRow, targetCol] = quadrantMap[r, c];
            }
        }

        return full;
    }

    /// <summary>
    /// Instantiate pieces for every cell of fullMap.
    /// Coordinates: row 0 = top, row increases downward.
    /// We place world Y = (fullRows - 1 - row) so top row is highest Y.
    /// </summary>
    private void InstantiateFullMap(int[,] fullMap)
    {
        int rows = fullMap.GetLength(0);
        int cols = fullMap.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int value = fullMap[r, c];

                Vector2 worldPos2D = new Vector2(c * cellSize, (rows - 1 - r) * cellSize);
                Vector3 worldPos = new Vector3(worldPos2D.x + spawnOffset.x, worldPos2D.y + spawnOffset.y, 0);

                switch (value)
                {
                    case 0:
                        // Empty space (optional: instantiate background/floor sprite)
                        break;
                    case 1:
                        // Outside corner (double lined corner piece)
                        if (outsideCornerPrefab != null)
                            Spawn(outsideCornerPrefab, worldPos, Quaternion.identity);
                        break;
                    case 2:
                        // Outside wall (double line)
                        if (outsideWallPrefab != null)
                            Spawn(outsideWallPrefab, worldPos, Quaternion.identity);
                        break;
                    case 3:
                        // Inside corner (single lined corner piece)
                        if (insideCornerPrefab != null)
                            Spawn(insideCornerPrefab, worldPos, Quaternion.identity);
                        break;
                    case 4:
                        // Inside wall (single line)
                        if (insideWallPrefab != null)
                            Spawn(insideWallPrefab, worldPos, Quaternion.identity);
                        break;
                    case 5:
                        // Standard pellet
                        if (pelletPrefab != null)
                            Spawn(pelletPrefab, worldPos, Quaternion.identity);
                        break;
                    case 6:
                        // Power pellet
                        if (powerPelletPrefab != null)
                            Spawn(powerPelletPrefab, worldPos, Quaternion.identity);
                        break;
                    case 7:
                        // T junction piece
                        if (tPrefab != null)
                            Spawn(tPrefab, worldPos, Quaternion.identity);
                        break;
                    case 8:
                        // Ghost exit wall (pink line)
                        if (ghostWallPrefab != null)
                            Spawn(ghostWallPrefab, worldPos, Quaternion.identity);
                        break;
                    default:
                        // Unknown type, do nothing
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Instantiate and parent to generatedRoot.
    /// </summary>
    private void Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (prefab == null) return;
        GameObject go = Instantiate(prefab, pos, rot, generatedRoot.transform);
        go.name = prefab.name + "_" + go.GetInstanceID();
    }

    /// <summary>
    /// Given the fullMap and a wall cell at (r,c), decide which wall prefab to use and what rotation.
    /// Neighbour order: up, right, down, left.
    /// We test for walls (value == 1).
    /// </summary>
    // private void DecideWallShapeAndRotation(int[,] fullMap, int r, int c, out GameObject prefab, out Quaternion rotation)
    // {
    //     prefab = null;
    //     rotation = Quaternion.identity;

    //     bool up = IsWall(fullMap, r - 1, c);
    //     bool right = IsWall(fullMap, r, c + 1);
    //     bool down = IsWall(fullMap, r + 1, c);
    //     bool left = IsWall(fullMap, r, c - 1);

    //     int neighbourCount = (up ? 1 : 0) + (right ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0);

    //     // Decide shape
    //     if (neighbourCount == 4)
    //     {
    //         prefab = crossPrefab ?? straightWallPrefab; // fallback
    //         rotation = Quaternion.identity;
    //         return;
    //     }
    //     if (neighbourCount == 3)
    //     {
    //         prefab = tPrefab ?? straightWallPrefab;
    //         // determine which side is missing -> rotate so T opens that direction
    //         if (!up) rotation = Quaternion.Euler(0, 0, 180f);       // T open up => missing up => rotate so stem points up -> that's 180
    //         else if (!right) rotation = Quaternion.Euler(0, 0, 90f);
    //         else if (!down) rotation = Quaternion.Euler(0, 0, 0f);
    //         else if (!left) rotation = Quaternion.Euler(0, 0, 270f);
    //         return;
    //     }
    //     if (neighbourCount == 2)
    //     {
    //         // Could be straight (opposite neighbours) or corner (adjacent neighbours)
    //         if ((up && down) && !left && !right)
    //         {
    //             // vertical straight
    //             prefab = straightWallPrefab;
    //             rotation = Quaternion.Euler(0, 0, 90f); // rotate 90 so it's vertical
    //             return;
    //         }
    //         if ((left && right) && !up && !down)
    //         {
    //             // horizontal straight
    //             prefab = straightWallPrefab;
    //             rotation = Quaternion.Euler(0, 0, 0f);
    //             return;
    //         }

    //         // adjacent -> corner
    //         prefab = cornerPrefab ?? straightWallPrefab;
    //         // up+right -> rotation 0 (corner connecting up and right)
    //         if (up && right) rotation = Quaternion.Euler(0, 0, 0f);
    //         else if (right && down) rotation = Quaternion.Euler(0, 0, 270f);
    //         else if (down && left) rotation = Quaternion.Euler(0, 0, 180f);
    //         else if (left && up) rotation = Quaternion.Euler(0, 0, 90f);
    //         return;
    //     }
    //     if (neighbourCount == 1)
    //     {
    //         // end piece (dead-end) pointing to the neighbour
    //         prefab = endPrefab ?? straightWallPrefab;
    //         if (up) rotation = Quaternion.Euler(0, 0, 180f);
    //         else if (right) rotation = Quaternion.Euler(0, 0, 270f);
    //         else if (down) rotation = Quaternion.Euler(0, 0, 0f);
    //         else if (left) rotation = Quaternion.Euler(0, 0, 90f);
    //         return;
    //     }
    //     if (neighbourCount == 0)
    //     {
    //         // isolated single block - treat as end piece (no rotation)
    //         prefab = endPrefab ?? straightWallPrefab;
    //         rotation = Quaternion.identity;
    //         return;
    //     }
    // }

    // private bool IsWall(int[,] map, int r, int c)
    // {
    //     if (r < 0 || c < 0 || r >= map.GetLength(0) || c >= map.GetLength(1)) return false;
    //     return map[r, c] == 1;
    // }

    // /// <summary>
    // /// Adjust an orthographic camera to see the whole map.
    // /// For orthographic camera: size = half of required height in world units.
    // /// For width: requiredHalfWidth = (mapCols * cellSize) / 2
    // /// Then account for camera.aspect to choose correct size.
    // /// </summary>
    // private void FitCameraToMap(int mapCols, int mapRows)
    // {
    //     Camera cam = targetCamera ?? Camera.main;
    //     if (cam == null)
    //     {
    //         Debug.LogWarning("No camera found to fit to map.");
    //         return;
    //     }

    //     float worldWidth = mapCols * cellSize;
    //     float worldHeight = mapRows * cellSize;

    //     if (cam.orthographic)
    //     {
    //         float halfHeight = (worldHeight / 2f) + cameraPadding;
    //         float halfWidth = (worldWidth / 2f) + cameraPadding;

    //         // orthographic size is half height in world units. Need to ensure width fits with aspect.
    //         float sizeBasedOnWidth = halfWidth / cam.aspect;
    //         cam.orthographicSize = Mathf.Max(halfHeight, sizeBasedOnWidth);

    //         // position camera at center of world (in XY plane)
    //         float centerX = (worldWidth / 2f - cellSize / 2f) + spawnOffset.x;
    //         float centerY = (worldHeight / 2f - cellSize / 2f) + spawnOffset.y;

    //         cam.transform.position = new Vector3(centerX, centerY, cam.transform.position.z);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Camera is not orthographic. Script only adjusts orthographic cameras automatically.");
    //     }
    // }

#if UNITY_EDITOR
    // Allow replacing levelMap in editor via inspector debugging (not serialized here) - optional helper methods could be added.
#endif
}
