using System.Collections.Generic;
using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    public List<GameObject> meshObjects = new List<GameObject>();
    public List<GameObject> grassPrefab;
    public List<GameObject> rockPrefab;
    public List<GameObject> cactusPrefab;
    public List<GameObject> bigRockPrefab;
    public int grassDensity = 200;
    public int rockDensity = 50;
    public int cactusDensity = 20;
    public int bigRockDensity = 5;
    private GameObject _grassAndRocks;


    private void Start()
    {
        _grassAndRocks = GameObject.Find("GrassAndRocks");
        FindGroundMeshObjects();
        PlaceRandomGrass();
    }
    private void FindGroundMeshObjects()
    {
        // 모든 MeshRenderer를 가진 오브젝트 찾기
        MeshRenderer[] allMeshRenderers = FindObjectsOfType<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in allMeshRenderers)
        {
            GameObject obj = meshRenderer.gameObject;

            // 오브젝트가 Ground 레이어에 속하는지 확인
            if (obj.layer == LayerMask.NameToLayer("Ground"))
            {
                meshObjects.Add(obj);
            }
        }
    }
    private void PlaceRandomGrass()
    {
        foreach (GameObject meshObject in meshObjects)
        {
            Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            for (int i = 0; i < grassDensity; i++)
            {
                int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;
                Vector3 vert1 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex]]);
                Vector3 vert2 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
                Vector3 vert3 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);

                Vector3 randomPoint = RandomPointInTriangle(vert1, vert2, vert3);

                Instantiate(grassPrefab[Random.Range(0, grassPrefab.Count)], randomPoint, Quaternion.Euler(0, Random.Range(0, 360), 0)).transform.SetParent(_grassAndRocks.transform);
            }
            for (int i = 0; i < rockDensity; i++)
            {
                int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;
                Vector3 vert1 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex]]);
                Vector3 vert2 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
                Vector3 vert3 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);

                Vector3 randomPoint = RandomPointInTriangle(vert1, vert2, vert3);

                Instantiate(rockPrefab[Random.Range(0, rockPrefab.Count)], randomPoint, Quaternion.Euler(0, Random.Range(0, 360), 0)).transform.SetParent(_grassAndRocks.transform);
            }
            for (int i = 0; i < cactusDensity; i++)
            {
                int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;
                Vector3 vert1 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex]]);
                Vector3 vert2 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
                Vector3 vert3 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);

                Vector3 randomPoint = RandomPointInTriangle(vert1, vert2, vert3);

                Instantiate(cactusPrefab[Random.Range(0, cactusPrefab.Count)], randomPoint, Quaternion.Euler(0, Random.Range(0, 360), 0)).transform.SetParent(_grassAndRocks.transform);
            }
            for (int i = 0; i < bigRockDensity; i++)
            {
                int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;
                Vector3 vert1 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex]]);
                Vector3 vert2 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
                Vector3 vert3 = meshObject.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);

                Vector3 randomPoint = RandomPointInTriangle(vert1, vert2, vert3);

                Instantiate(bigRockPrefab[Random.Range(0, bigRockPrefab.Count)], randomPoint, Quaternion.Euler(0, Random.Range(0, 360), 0)).transform.SetParent(_grassAndRocks.transform);
            }
        }
    }

    private Vector3 RandomPointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        float r1 = Random.value;
        float r2 = Random.value;

        // barycentric 좌표계 사용
        if (r1 + r2 > 1)
        {
            r1 = 1 - r1;
            r2 = 1 - r2;
        }

        float r3 = 1 - r1 - r2;

        return r1 * a + r2 * b + r3 * c;
    }
}