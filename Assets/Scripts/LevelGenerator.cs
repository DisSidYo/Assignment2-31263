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

    public GameObject Empty;        // cross junction
    public GameObject powerPelletPrefab;    
    
    public GameObject TileMap_Manual; // Add this line for straight wall pieces
    public GameObject pelletPrefab;   
    
        // pellet or dot
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

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        Transform manualParent = null;

        if (TileMap_Manual != null)
        {
            // Save transform before destroying
            spawnPos = TileMap_Manual.transform.position;
            spawnRot = TileMap_Manual.transform.rotation;
            manualParent = TileMap_Manual.transform.parent;

            // Destroy manual tilemap
            Destroy(TileMap_Manual);
        }

        // Create a fresh container for generated level
        generatedRoot = new GameObject("GeneratedLevel");
        generatedRoot.transform.SetParent(manualParent, false);
        generatedRoot.transform.position = spawnPos;
        generatedRoot.transform.rotation = spawnRot;
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
        int fullCols = cols * 2;

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
                Vector3 localPos = new Vector3(c * cellSize, (rows - 1 - r) * cellSize, 0);

                GameObject prefab = null;
                Quaternion rotation = Quaternion.identity;
                Vector3 localScale = Vector3.one;

                switch (value)
                {
                    case 0: // Empty
                        continue;

                    case 1: // Outside corner
                        prefab = outsideCornerPrefab;
                        rotation = DecideRotation(fullMap, r, c, value);
                        break;

                    case 2: // Outside wall
                        prefab = outsideWallPrefab;
                        rotation = DecideRotation(fullMap, r, c, value);
                        break;

                    case 3: // Inside corner
                        prefab = insideCornerPrefab;
                        rotation = DecideRotation(fullMap, r, c, value);
                        break;

                    case 4: // Inside wall
                        prefab = insideWallPrefab;
                        rotation = DecideRotation(fullMap, r, c, value);
                        break;

                    case 5: // Pellet
                        prefab = pelletPrefab;
                        break;

                    case 6: // Power pellet
                        // Lay the empty prefab first
                        if (Empty != null)
                        {
                            GameObject emptyGo = Instantiate(Empty, generatedRoot.transform);
                            emptyGo.name = Empty.name + "_" + emptyGo.GetInstanceID();
                            emptyGo.transform.localPosition = new Vector3(c * cellSize, (rows - 1 - r) * cellSize, 0.5f) + (Vector3)spawnOffset;
                            emptyGo.transform.localRotation = Quaternion.identity;
                            emptyGo.transform.localScale = Vector3.one;
                        }
                        // Then lay the power pellet on top
                        prefab = powerPelletPrefab;
                        break;

                    case 7: // T junction
                        prefab = tPrefab;
                        rotation = DecideRotation(fullMap, r, c, value);

                        // Determine flipping
                        bool up = IsWallLike(fullMap, r - 1, c);
                        bool right = IsWallLike(fullMap, r, c + 1);
                        bool down = IsWallLike(fullMap, r + 1, c);
                        bool left = IsWallLike(fullMap, r, c - 1);

                        // Example: flip X for (left and down), flip Y for (up and right)
                        if (left && up)
                            localScale = new Vector3(1, -1, 1); // flip X
                        else if (down && right)
                            localScale = new Vector3(-1, 1, 1); // flip Y
                        // Add more cases if needed
                        break;

                    case 8: // Ghost wall
                        prefab = ghostWallPrefab;
                        rotation = DecideRotation(fullMap, r, c, value);
                        break;
                }

                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab, generatedRoot.transform);
                    go.name = prefab.name + "_" + go.GetInstanceID();
                    go.transform.localPosition = new Vector3(c * cellSize, (rows - 1 - r) * cellSize, 0) + (Vector3)spawnOffset;
                    go.transform.localRotation = rotation;
                    go.transform.localScale = localScale;

                }
            }
        }
    }

    /// <summary>
    /// Instantiate and parent to generatedRoot.
    /// </summary>
    // private void Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    // {
    //     if (prefab == null) return;
    //     GameObject go = Instantiate(prefab, pos, rot, generatedRoot.transform);
    //     go.name = prefab.name + "_" + go.GetInstanceID();
    //     GameObject go = Instantiate(prefab, generatedRoot.transform);
    //     go.name = prefab.name + "_" + go.GetInstanceID();
    //     go.transform.localPosition = new Vector3(c * cellSize, (rows - 1 - r) * cellSize, 0) + (Vector3)spawnOffset;
    //     go.transform.localRotation = rotation;
    //     go.transform.localScale = localScale;

    // }

    /// <summary>
    /// Given the fullMap and a wall cell at (r,c), decide which wall prefab to use and what rotation.
    /// Neighbour order: up, right, down, left.
    /// We test for walls (value == 1).
    /// </summary>
    private Quaternion DecideRotation(int[,] fullMap, int r, int c, int type)
    {
        bool up = IsWallLike(fullMap, r - 1, c);
        bool right = IsWallLike(fullMap, r, c + 1);
        bool down = IsWallLike(fullMap, r + 1, c);
        bool left = IsWallLike(fullMap, r, c - 1);

        bool tUp = IsT(fullMap, r - 1, c);
        bool tRight = IsT(fullMap, r, c + 1);
        bool tDown = IsT(fullMap, r + 1, c);
        bool tLeft = IsT(fullMap, r, c - 1);

        bool cUp = IsCorner(fullMap, r - 1, c);
        bool cRight = IsCorner(fullMap, r, c + 1);
        bool cDown = IsCorner(fullMap, r + 1, c);
        bool cLeft = IsCorner(fullMap, r, c - 1);
        
        bool upWC = IsWallOrCorner(fullMap, r - 1, c);
        bool rightWC = IsWallOrCorner(fullMap, r, c + 1);
        bool downWC = IsWallOrCorner(fullMap, r + 1, c);
        bool leftWC = IsWallOrCorner(fullMap, r, c - 1);

        bool upRight  = IsWallOrCorner(fullMap, r - 1, c + 1) || IsT(fullMap, r - 1, c + 1);
        bool downRight= IsWallOrCorner(fullMap, r + 1, c + 1) || IsT(fullMap, r + 1, c + 1);
        bool downLeft = IsWallOrCorner(fullMap, r + 1, c - 1) || IsT(fullMap, r + 1, c - 1);
        bool upLeft = IsWallOrCorner(fullMap, r - 1, c - 1) || IsT(fullMap, r - 1, c - 1);

        // Handle by type
        switch (type)
        {
            case 1: // Outside corner


                // Check L-shaped connections
                if (upWC && rightWC) return Quaternion.Euler(0, 0, 90);
                if (rightWC && downWC) return Quaternion.Euler(0, 0, 0);
                if (downWC && leftWC) return Quaternion.Euler(0, 0, 270);
                if (leftWC && upWC) return Quaternion.Euler(0, 0, 180);
                break;

            case 3: // Inside corner



                // Check diagonals to handle concave maze edges


             int wallCount = (up ? 1 : 0) + (right ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0);

                // 3+ walls → this is not an inside corner, leave it for T/wall logic
                // if (wallCount >= 3)
                //     return null;

                // Try wall pairs first
                if (wallCount == 2)
                {
                    if (up && right) return Quaternion.Euler(0, 0, 90);   // opening down-left
                    if (right && down) return Quaternion.Euler(0, 0, 0);  // opening up-left
                    if (down && left) return Quaternion.Euler(0, 0, 270); // opening up-right
                    if (left && up) return Quaternion.Euler(0, 0, 180);   // opening down-right
                }
                else if (wallCount == 3)
                {
                    if (up && left && down && !right) return Quaternion.Euler(0, 0, 270); // bend into left-down
                    if (up && right && down && !left) return Quaternion.Euler(0, 0, 0);   // bend into right-down
                    if (left && up && right && !down) return Quaternion.Euler(0, 0, 90);  // bend into up-right
                    if (left && down && right && !up) return Quaternion.Euler(0, 0, 180); // bend into down-left       // opening right
                    
                    }
                

                
                

                // Fallback: corners acting as walls (only if no wall pairs exist)
                if ((cUp && right) || (cRight && up) || (cUp && cRight))
                    return Quaternion.Euler(0, 0, 90);

                if ((cRight && down) || (cDown && right) || (cRight && cDown))
                    return Quaternion.Euler(0, 0, 0);

                if ((cDown && left) || (cLeft && down) || (cDown && cLeft))
                    return Quaternion.Euler(0, 0, 270);

                if ((cLeft && up) || (cUp && left) || (cLeft && cUp))
                    return Quaternion.Euler(0, 0, 180);

                break;


            case 2: // Outside wall
                if ((up && down) || (tUp || tDown) || (up && cDown) || (down && cUp) || (cDown && cUp)) return Quaternion.Euler(0, 0, 90); // vertical
                return Quaternion.identity; // horizontal

            case 4: // Inside wall
                if ((up && down) || (tUp || tDown) || (up && cDown) || (down && cUp) || (cDown && cUp)) return Quaternion.Euler(0, 0, 90); // vertical
                return Quaternion.identity; // horizontal
            case 8: // Ghost wall
                if (up && down) return Quaternion.Euler(0, 0, 90); // vertical
                return Quaternion.identity; // horizontal

            case 7: // T junction
                if (up && right) return Quaternion.Euler(0, 0, 180);
                // if (right && down) return Quaternion.Euler(0, 0, 180);
                if (down && left) return Quaternion.Euler(0, 0, 0);
                // if (left && up) return Quaternion.Euler(0, 0, 180);
                break;
        }

        return Quaternion.identity;
    }


    private bool IsWallLike(int[,] map, int r, int c)
    {
        if (r < 0 || c < 0 || r >= map.GetLength(0) || c >= map.GetLength(1)) return false;
        int v = map[r, c];
        return v == 2 || v == 4 || v == 8;
    }
    private bool IsT(int[,] map, int r, int c)
    {
        if (r < 0 || c < 0 || r >= map.GetLength(0) || c >= map.GetLength(1)) return false;
        int v = map[r, c];
        return v == 7;
    }
    private bool IsCorner(int[,] map, int r, int c)
    {
        if (r < 0 || c < 0 || r >= map.GetLength(0) || c >= map.GetLength(1)) return false;
        int v = map[r, c];
        return v == 1 || v == 3;
    }
    private bool IsWallOrCorner(int[,] map, int r, int c)
{
    return IsWallLike(map, r, c) || IsCorner(map, r, c);
}

    


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
