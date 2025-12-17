using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public float wallSize;
    public float carveDelay;

    public bool[,] visited;
    public List<GameObject> walls = new List<GameObject>();
    public GameObject startMarkerPrefab;
    public GameObject endMarkerPrefab;

    void Start()
    {
        GenerateMaze();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearMaze();
            GenerateMaze();
        }
    }

    void GenerateMaze()
    {
        visited = new bool[width, height];
        CreateGrid();
        StartCoroutine(RunGenerationAndMark(0, 0));
    }

    void CreateGrid()
    {
        ClearMaze();

        float half = wallSize / 2f;
        float thickness = Mathf.Max(0.05f, wallSize * 0.05f);
        float heightY = wallSize;

        for (int vx = 0; vx <= width; vx++)
        {
            float wallX = vx * wallSize - half;
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(wallX, heightY / 2f, y * wallSize);
                Vector3 scale = new Vector3(thickness, heightY, wallSize);
                CreateWall(pos, scale);
            }
        }

        for (int hy = 0; hy <= height; hy++)
        {
            float wallZ = hy * wallSize - half;
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x * wallSize, heightY / 2f, wallZ);
                Vector3 scale = new Vector3(wallSize, heightY, thickness);
                CreateWall(pos, scale);
            }
        }
    }

    IEnumerator RunGenerationAndMark(int sx, int sy)
    {
        yield return StartCoroutine(Generate(sx, sy));

        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = new Vector2Int(Mathf.Max(0, width - 1), Mathf.Max(0, height - 1));

        CreateMarkerAt(start, Color.green);
        CreateMarkerAt(end, Color.red);
    }

    void CreateMarkerAt(Vector2Int cell, Color color)
    {
        Vector3 pos = new Vector3(cell.x * wallSize, 0.01f, cell.y * wallSize);
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.transform.position = pos;
        marker.transform.localScale = Vector3.one * (wallSize * 0.3f);
        marker.transform.parent = this.transform;
        var mr = marker.GetComponent<Renderer>();
        if (mr != null)
        {
            mr.material = new Material(Shader.Find("Standard"));
            mr.material.color = color;
        }
        walls.Add(marker); 
    }

    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.transform.parent = this.transform;
        walls.Add(wall);
    }

    IEnumerator Generate(int x, int y)
    {
        visited[x, y] = true;

        List<Vector2Int> neighbors = GetUnvisitedNeighbors(x, y);

        while (neighbors.Count > 0)
        {
            int idx = Random.Range(0, neighbors.Count);
            Vector2Int nb = neighbors[idx];

            RemoveWallBetween(x, y, nb.x, nb.y);

            if (carveDelay > 0f)
                yield return new WaitForSeconds(carveDelay);
            else
                yield return null;

            yield return StartCoroutine(Generate(nb.x, nb.y));

            neighbors = GetUnvisitedNeighbors(x, y);
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(int x, int y)
    {
        List<Vector2Int> list = new List<Vector2Int>();

        if (x - 1 >= 0 && !visited[x - 1, y]) list.Add(new Vector2Int(x - 1, y));
        if (x + 1 < width && !visited[x + 1, y]) list.Add(new Vector2Int(x + 1, y));
        if (y - 1 >= 0 && !visited[x, y - 1]) list.Add(new Vector2Int(x, y - 1));
        if (y + 1 < height && !visited[x, y + 1]) list.Add(new Vector2Int(x, y + 1));

        return list;
    }

    void RemoveWallBetween(int x1, int y1, int x2, int y2)
    {
        Vector3 a = new Vector3(x1 * wallSize, 0f, y1 * wallSize);
        Vector3 b = new Vector3(x2 * wallSize, 0f, y2 * wallSize);

        Vector3 mid = (a + b) / 2f;
        mid.y = wallSize / 2f;

        GameObject closest = null;
        float best = float.MaxValue;
        foreach (var w in walls)
        {
            if (w == null) continue;
            float d = Vector3.Distance(w.transform.position, mid);
            if (d < best)
            {
                best = d;
                closest = w;
            }
        }

        if (closest != null)
        {
            walls.Remove(closest);
            Destroy(closest);
        }
    }

    void ClearMaze()
    {
        for (int i = walls.Count - 1; i >= 0; i--)
        {
            if (walls[i] != null)
                Destroy(walls[i]);
        }
        walls.Clear();
    }
}