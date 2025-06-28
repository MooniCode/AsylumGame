using UnityEngine;

public class TerrainDigger : MonoBehaviour
{
    public Terrain terrain;
    public float digRadius = 2f;
    public float digDepth = 0.1f;
    public LayerMask terrainLayer = -1;

    private TerrainData terrainData;
    private TerrainData originalTerrainData; // Keep reference to original
    private int heightmapWidth;
    private int heightmapHeight;

    void Start()
    {
        // Get terrain reference if not assigned
        if (terrain == null)
            terrain = Terrain.activeTerrain;

        // Store reference to original terrain data
        originalTerrainData = terrain.terrainData;

        // Create a runtime copy that we can modify freely
        terrainData = Instantiate(originalTerrainData);
        terrain.terrainData = terrainData;

        heightmapWidth = terrainData.heightmapResolution;
        heightmapHeight = terrainData.heightmapResolution;

        Debug.Log("Terrain runtime copy created - original will remain unchanged!");
    }

    void OnDestroy()
    {
        // Restore original terrain data when this component is destroyed
        if (terrain != null && originalTerrainData != null)
        {
            terrain.terrainData = originalTerrainData;
        }

        // Clean up the runtime copy
        if (terrainData != null && terrainData != originalTerrainData)
        {
            DestroyImmediate(terrainData);
        }
    }

    void Update()
    {
        // Check for left mouse click
        if (Input.GetMouseButton(0))
        {
            DigAtMousePosition();
        }
    }

    void DigAtMousePosition()
    {
        // Cast ray from camera through mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))
        {
            // Check if we hit the terrain
            if (hit.collider.GetComponent<Terrain>() == terrain)
            {
                DigAtWorldPosition(hit.point);
            }
        }
    }

    void DigAtWorldPosition(Vector3 worldPosition)
    {
        // Convert world position to terrain coordinates
        Vector3 terrainPosition = worldPosition - terrain.transform.position;

        // Convert to heightmap coordinates
        int hmapX = Mathf.RoundToInt((terrainPosition.x / terrainData.size.x) * heightmapWidth);
        int hmapZ = Mathf.RoundToInt((terrainPosition.z / terrainData.size.z) * heightmapHeight);

        // Calculate the area to modify based on dig radius
        int radiusInHeightmapUnits = Mathf.RoundToInt((digRadius / terrainData.size.x) * heightmapWidth);

        // Get current heights
        int startX = Mathf.Max(0, hmapX - radiusInHeightmapUnits);
        int startZ = Mathf.Max(0, hmapZ - radiusInHeightmapUnits);
        int width = Mathf.Min(heightmapWidth - startX, radiusInHeightmapUnits * 2);
        int height = Mathf.Min(heightmapHeight - startZ, radiusInHeightmapUnits * 2);

        float[,] heights = terrainData.GetHeights(startX, startZ, width, height);

        // Modify heights in a circular pattern
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Calculate distance from center
                int actualX = startX + x;
                int actualZ = startZ + z;
                float distance = Vector2.Distance(new Vector2(hmapX, hmapZ), new Vector2(actualX, actualZ));

                // Only modify if within radius
                if (distance <= radiusInHeightmapUnits)
                {
                    // Calculate falloff (smooth edges)
                    float falloff = 1f - (distance / radiusInHeightmapUnits);
                    falloff = Mathf.Pow(falloff, 2); // Square for smoother falloff

                    // Calculate depth adjustment
                    float depthAdjustment = (digDepth / terrainData.size.y) * falloff;

                    // Apply the digging (subtract height)
                    heights[z, x] = Mathf.Max(0, heights[z, x] - depthAdjustment);
                }
            }
        }

        // Apply the modified heights back to terrain
        terrainData.SetHeights(startX, startZ, heights);
    }

    // Optional: Method to dig at a specific world position (useful for scripted digging)
    public void DigAtPosition(Vector3 worldPosition, float radius, float depth)
    {
        float originalRadius = digRadius;
        float originalDepth = digDepth;

        digRadius = radius;
        digDepth = depth;

        DigAtWorldPosition(worldPosition);

        digRadius = originalRadius;
        digDepth = originalDepth;
    }

    // Reset terrain to original state
    [ContextMenu("Reset Terrain")]
    public void ResetTerrainToOriginal()
    {
        if (originalTerrainData != null)
        {
            // Destroy current runtime copy
            if (terrainData != null && terrainData != originalTerrainData)
            {
                DestroyImmediate(terrainData);
            }

            // Create fresh copy from original
            terrainData = Instantiate(originalTerrainData);
            terrain.terrainData = terrainData;

            Debug.Log("Terrain reset to original state!");
        }
    }
}