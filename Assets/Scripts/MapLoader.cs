using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [SerializeField] Texture2D mapTexture;
    [SerializeField] Material testMat;
    [SerializeField] GameObject provincePrefab;
    public GameObject square;

    private bool[,] visited;
    private List<List<Vector2Int>> whiteRegions;

    private void Awake()
    {
        DetectWhiteRegions();
        CreateMeshesFromRegions();
    }

    void DetectWhiteRegions()
    {
        int width = mapTexture.width;
        int height = mapTexture.height;

        visited = new bool[width, height];
        whiteRegions = new List<List<Vector2Int>>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!visited[x, y] && mapTexture.GetPixel(x, y) == Color.white)
                {
                    List<Vector2Int> newRegion = new List<Vector2Int>();
                    FloodFillStack(x, y, newRegion);
                    whiteRegions.Add(newRegion);
                }
            }
        }

        Debug.Log($"Znaleziono {whiteRegions.Count} oddzielnych bia³ych obszarów.");
    }

    private void FloodFillStack(int startX, int startY, List<Vector2Int> region)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(startX, startY));

        while (stack.Count > 0)
        {
            Vector2Int p = stack.Pop();
            int x = p.x, y = p.y;

            if (!CheckIfInMap(x, y) || visited[x, y] || mapTexture.GetPixel(x, y) != Color.white)
                continue;

            visited[x, y] = true;
            region.Add(new Vector2Int(x, y));

            stack.Push(new Vector2Int(x - 1, y));
            stack.Push(new Vector2Int(x + 1, y));
            stack.Push(new Vector2Int(x, y - 1));
            stack.Push(new Vector2Int(x, y + 1));
        }
    }

    private bool CheckIfInMap(int x, int y)
    {
        return x >= 0 && x < mapTexture.width && y >= 0 && y < mapTexture.height;
    }

    void CreateMeshesFromRegions()
    {
        foreach (List<Vector2Int> item in whiteRegions)
        {
            GenerateMeshForRegion(item);
        }
    }

    void GenerateMeshForRegion(List<Vector2Int> region)
    {
        GameObject meshObject = Instantiate(provincePrefab, transform);
        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = meshObject.GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        foreach (Vector2Int point in region)
        {
            int index = vertices.Count;

            vertices.Add(new Vector3(point.x, point.y, 0));
            vertices.Add(new Vector3(point.x + 1, point.y, 0));
            vertices.Add(new Vector3(point.x, point.y + 1, 0));
            vertices.Add(new Vector3(point.x + 1, point.y + 1, 0));

            triangles.Add(index);
            triangles.Add(index + 2);
            triangles.Add(index + 1);

            triangles.Add(index + 1);
            triangles.Add(index + 2);
            triangles.Add(index + 3);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        meshRenderer.material = testMat;
        meshCollider.sharedMesh = mesh;
    }



    //void GenerateMeshForRegion(List<Vector2Int> region)
    //{
    //    int minX = int.MaxValue;
    //    int maxX = int.MinValue;
    //    int minY = int.MaxValue;
    //    int maxY = int.MinValue;

    //    foreach (var p in region)
    //    {
    //        if (p.x < minX)
    //        {
    //            minX = p.x;
    //        }
    //        if (p.x > maxX)
    //        {
    //            maxX = p.x;
    //        }
    //        if (p.y < minY)
    //        {
    //            minY = p.y;
    //        }
    //        if (p.y > maxY)
    //        {
    //            maxY = p.y;
    //        }
    //    }

    //    //int[,] grid = new int[maxX - minX, maxY - minY];
    //    //GameObject provinceObject = Instantiate(provincePrefab, new Vector2((minX + maxX) / 2, (minY + maxY) / 2), Quaternion.identity);

    //    //MeshFilter meshFilter = provinceObject.GetComponent<MeshFilter>();
    //    //MeshRenderer meshRenderer = provinceObject.GetComponent<MeshRenderer>();
    //    //MeshCollider meshCollider = provinceObject.GetComponent<MeshCollider>();

    //    //Mesh mesh = new Mesh();
    //    List<Vector3> vertices = new List<Vector3>();
    //    List<int> triangles = new List<int>();
    //    Dictionary<Vector3, int> vertexIndexMap = new Dictionary<Vector3, int>();

    //    for (int x = minX; x < maxX + 1; x++)
    //    {
    //        for (int y = minY; y < maxY + 1; y++)
    //        {

    //        }
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    int minX = int.MaxValue;
    //    int maxX = int.MinValue;
    //    int minY = int.MaxValue;
    //    int maxY = int.MinValue;

    //    foreach (var p in whiteRegions[0])
    //    {
    //        if (p.x < minX)
    //        {
    //            minX = p.x;
    //        }
    //        if (p.x > maxX)
    //        {
    //            maxX = p.x;
    //        }
    //        if (p.y < minY)
    //        {
    //            minY = p.y;
    //        }
    //        if (p.y > maxY)
    //        {
    //            maxY = p.y;
    //        }
    //    }
    //    Gizmos.color = Color.white;

    //    Gizmos.DrawWireSphere(new Vector2(minX, minY), 1f);
    //    Gizmos.DrawWireSphere(new Vector2(maxX, maxY), 1f);

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(new Vector2(0, 0), 1f);
    //    Gizmos.DrawWireSphere(new Vector2(whiteRegions[0][0].x, whiteRegions[0][0].y), 1f);
    //    for (int x = minX - 1; x <= maxX + 1; x++)
    //    {
    //        for (int y = minY - 1; y <= maxY + 1; y++)
    //        {
    //            Gizmos.color = Color.yellow;
    //            Gizmos.DrawWireCube(new Vector2(x, y), Vector3.one);

    //            if (whiteRegions[0].Contains(new Vector2Int(x, y)))
    //            {
    //                print("FOUND");
    //            }
    //        }
    //    }


    //}
}
