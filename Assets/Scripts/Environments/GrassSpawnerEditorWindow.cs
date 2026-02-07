using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GrassSpawnerEditorWindow : EditorWindow
{
    // 프리팹 리스트
    private List<GameObject> grassPrefabs = new List<GameObject>();
    private List<GameObject> rockPrefabs = new List<GameObject>();
    private List<GameObject> cactusPrefabs = new List<GameObject>();
    private List<GameObject> bigRockPrefabs = new List<GameObject>();

    // 밀도 설정
    private int grassDensity = 200;
    private int rockDensity = 50;
    private int cactusDensity = 20;
    private int bigRockDensity = 5;

    // 청크 설정
    private float chunkSize = 20f;
    private Transform parentTransform;

    // 스크롤 위치
    private Vector2 scrollPosition;

    // Foldout states
    private bool grassFoldout = true;
    private bool rockFoldout = true;
    private bool cactusFoldout = true;
    private bool bigRockFoldout = true;
    private bool chunkSettingsFoldout = true;

    [MenuItem("Tools/Grass & Rock Spawner")]
    public static void ShowWindow()
    {
        var window = GetWindow<GrassSpawnerEditorWindow>("Grass & Rock Spawner");
        window.minSize = new Vector2(350, 600);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Grass & Rock Spawner Tool (Chunk System)", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Parent 오브젝트 설정
        EditorGUILayout.LabelField("Parent Object", EditorStyles.boldLabel);
        parentTransform = (Transform)EditorGUILayout.ObjectField("Parent Transform", parentTransform, typeof(Transform), true);

        if (parentTransform == null)
        {
            EditorGUILayout.HelpBox("Parent Transform을 설정해주세요. 청크들이 이 Transform 아래에 배치됩니다.", MessageType.Warning);
        }

        EditorGUILayout.Space(10);

        // 청크 설정
        chunkSettingsFoldout = EditorGUILayout.Foldout(chunkSettingsFoldout, "Chunk Settings", true);
        if (chunkSettingsFoldout)
        {
            EditorGUI.indentLevel++;
            chunkSize = EditorGUILayout.Slider("Chunk Size", chunkSize, 5f, 100f);
            EditorGUILayout.HelpBox($"청크 크기: {chunkSize}x{chunkSize} 유닛", MessageType.Info);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(10);

        // 밀도 설정 (청크당)
        EditorGUILayout.LabelField("Density Settings (Per Chunk)", EditorStyles.boldLabel);
        grassDensity = EditorGUILayout.IntSlider("Grass Density", grassDensity, 0, 500);
        rockDensity = EditorGUILayout.IntSlider("Rock Density", rockDensity, 0, 200);
        cactusDensity = EditorGUILayout.IntSlider("Cactus Density", cactusDensity, 0, 100);
        bigRockDensity = EditorGUILayout.IntSlider("Big Rock Density", bigRockDensity, 0, 50);

        EditorGUILayout.Space(10);

        // 프리팹 리스트
        EditorGUILayout.LabelField("Prefab Settings", EditorStyles.boldLabel);

        grassFoldout = EditorGUILayout.Foldout(grassFoldout, $"Grass Prefabs ({grassPrefabs.Count})", true);
        if (grassFoldout)
        {
            DrawPrefabList(grassPrefabs, "Grass");
        }

        rockFoldout = EditorGUILayout.Foldout(rockFoldout, $"Rock Prefabs ({rockPrefabs.Count})", true);
        if (rockFoldout)
        {
            DrawPrefabList(rockPrefabs, "Rock");
        }

        cactusFoldout = EditorGUILayout.Foldout(cactusFoldout, $"Cactus Prefabs ({cactusPrefabs.Count})", true);
        if (cactusFoldout)
        {
            DrawPrefabList(cactusPrefabs, "Cactus");
        }

        bigRockFoldout = EditorGUILayout.Foldout(bigRockFoldout, $"Big Rock Prefabs ({bigRockPrefabs.Count})", true);
        if (bigRockFoldout)
        {
            DrawPrefabList(bigRockPrefabs, "Big Rock");
        }

        EditorGUILayout.Space(20);

        // 버튼들
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear All Chunks", GUILayout.Height(30)))
        {
            if (parentTransform != null)
            {
                if (EditorUtility.DisplayDialog("Confirm Clear",
                    $"정말로 '{parentTransform.name}' 아래의 모든 청크를 삭제하시겠습니까?",
                    "삭제", "취소"))
                {
                    ClearAllChunks();
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Parent Transform을 먼저 설정해주세요.", "OK");
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Generate Chunks", GUILayout.Height(40)))
        {
            if (ValidateSpawn())
            {
                GenerateChunks();
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(10);

        // 정보 표시
        if (parentTransform != null)
        {
            int chunkCount = 0;
            int totalObjects = 0;
            foreach (Transform child in parentTransform)
            {
                if (child.name.StartsWith("Chunk_"))
                {
                    chunkCount++;
                    totalObjects += child.childCount;
                }
            }
            EditorGUILayout.HelpBox($"청크 수: {chunkCount}개\n총 오브젝트 수: {totalObjects}개", MessageType.Info);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawPrefabList(List<GameObject> prefabList, string label)
    {
        EditorGUI.indentLevel++;

        for (int i = 0; i < prefabList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            prefabList[i] = (GameObject)EditorGUILayout.ObjectField(prefabList[i], typeof(GameObject), false);
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                prefabList.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button($"+ Add {label}", GUILayout.Width(120)))
        {
            prefabList.Add(null);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }

    private bool ValidateSpawn()
    {
        if (parentTransform == null)
        {
            EditorUtility.DisplayDialog("Error", "Parent Transform을 먼저 설정해주세요.", "OK");
            return false;
        }

        bool hasAnyPrefab = grassPrefabs.Exists(p => p != null) ||
                           rockPrefabs.Exists(p => p != null) ||
                           cactusPrefabs.Exists(p => p != null) ||
                           bigRockPrefabs.Exists(p => p != null);

        if (!hasAnyPrefab)
        {
            EditorUtility.DisplayDialog("Error", "최소 하나의 프리팹을 설정해주세요.", "OK");
            return false;
        }

        return true;
    }

    private void ClearAllChunks()
    {
        if (parentTransform == null) return;

        Undo.RegisterFullObjectHierarchyUndo(parentTransform.gameObject, "Clear All Chunks");

        int childCount = parentTransform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = parentTransform.GetChild(i);
            if (child.name.StartsWith("Chunk_"))
            {
                Undo.DestroyObjectImmediate(child.gameObject);
            }
        }

        Debug.Log($"Cleared all chunks from {parentTransform.name}");
    }

    private void GenerateChunks()
    {
        List<GameObject> meshObjects = FindGroundMeshObjects();

        if (meshObjects.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "Ground 레이어에 MeshRenderer가 있는 오브젝트를 찾을 수 없습니다.", "OK");
            return;
        }

        // 전체 바운드 계산
        Bounds totalBounds = new Bounds();
        bool boundsInitialized = false;

        foreach (GameObject meshObject in meshObjects)
        {
            MeshRenderer renderer = meshObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                if (!boundsInitialized)
                {
                    totalBounds = renderer.bounds;
                    boundsInitialized = true;
                }
                else
                {
                    totalBounds.Encapsulate(renderer.bounds);
                }
            }
        }

        // 청크 그리드 계산
        int chunksX = Mathf.CeilToInt(totalBounds.size.x / chunkSize);
        int chunksZ = Mathf.CeilToInt(totalBounds.size.z / chunkSize);

        Vector3 startPos = new Vector3(
            totalBounds.min.x,
            totalBounds.center.y,
            totalBounds.min.z
        );

        Undo.RegisterFullObjectHierarchyUndo(parentTransform.gameObject, "Generate Chunks");

        int totalSpawned = 0;
        int chunkCount = 0;

        // 각 청크 생성
        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                Vector3 chunkCenter = new Vector3(
                    startPos.x + (x + 0.5f) * chunkSize,
                    startPos.y,
                    startPos.z + (z + 0.5f) * chunkSize
                );

                Bounds chunkBounds = new Bounds(chunkCenter, new Vector3(chunkSize, totalBounds.size.y, chunkSize));

                // 이 청크에 해당하는 메시 찾기
                List<MeshData> chunkMeshes = new List<MeshData>();
                foreach (GameObject meshObject in meshObjects)
                {
                    MeshRenderer renderer = meshObject.GetComponent<MeshRenderer>();
                    if (renderer != null && renderer.bounds.Intersects(chunkBounds))
                    {
                        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
                        if (meshFilter != null && meshFilter.sharedMesh != null)
                        {
                            chunkMeshes.Add(new MeshData
                            {
                                mesh = meshFilter.sharedMesh,
                                transform = meshObject.transform
                            });
                        }
                    }
                }

                if (chunkMeshes.Count == 0) continue;

                // 청크 오브젝트 생성
                GameObject chunkObject = new GameObject($"Chunk_{x}_{z}");
                chunkObject.transform.SetParent(parentTransform);
                chunkObject.transform.position = chunkCenter;

                // GrassChunk 컴포넌트 추가
                GrassChunk chunkComponent = chunkObject.AddComponent<GrassChunk>();
                chunkComponent.chunkBounds = chunkBounds;

                Undo.RegisterCreatedObjectUndo(chunkObject, "Create Chunk");

                // 오브젝트 스폰
                int spawned = SpawnInChunk(chunkObject.transform, chunkMeshes, chunkBounds);
                totalSpawned += spawned;
                chunkCount++;

                // 청크 비활성화 (런타임에 로더가 활성화)
                chunkObject.SetActive(false);
            }
        }

        Debug.Log($"Generated {chunkCount} chunks with {totalSpawned} total objects");
    }

    private int SpawnInChunk(Transform chunkTransform, List<MeshData> meshDataList, Bounds chunkBounds)
    {
        int spawnedCount = 0;

        List<GameObject> validGrass = grassPrefabs.FindAll(p => p != null);
        List<GameObject> validRocks = rockPrefabs.FindAll(p => p != null);
        List<GameObject> validCactus = cactusPrefabs.FindAll(p => p != null);
        List<GameObject> validBigRocks = bigRockPrefabs.FindAll(p => p != null);

        foreach (MeshData meshData in meshDataList)
        {
            Vector3[] vertices = meshData.mesh.vertices;
            int[] triangles = meshData.mesh.triangles;

            // Grass
            if (validGrass.Count > 0 && grassDensity > 0)
            {
                spawnedCount += SpawnType(chunkTransform, validGrass, grassDensity, meshData, chunkBounds);
            }

            // Rocks
            if (validRocks.Count > 0 && rockDensity > 0)
            {
                spawnedCount += SpawnType(chunkTransform, validRocks, rockDensity, meshData, chunkBounds);
            }

            // Cactus
            if (validCactus.Count > 0 && cactusDensity > 0)
            {
                spawnedCount += SpawnType(chunkTransform, validCactus, cactusDensity, meshData, chunkBounds);
            }

            // Big Rocks
            if (validBigRocks.Count > 0 && bigRockDensity > 0)
            {
                spawnedCount += SpawnType(chunkTransform, validBigRocks, bigRockDensity, meshData, chunkBounds);
            }
        }

        return spawnedCount;
    }

    private int SpawnType(Transform chunkTransform, List<GameObject> prefabs, int density, MeshData meshData, Bounds chunkBounds)
    {
        int spawnedCount = 0;
        Vector3[] vertices = meshData.mesh.vertices;
        int[] triangles = meshData.mesh.triangles;

        for (int i = 0; i < density; i++)
        {
            int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;
            Vector3 vert1 = meshData.transform.TransformPoint(vertices[triangles[triangleIndex]]);
            Vector3 vert2 = meshData.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
            Vector3 vert3 = meshData.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);

            Vector3 randomPoint = RandomPointInTriangle(vert1, vert2, vert3);

            // 청크 범위 내에 있는지 확인
            if (!chunkBounds.Contains(randomPoint)) continue;

            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = randomPoint;
            instance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            instance.transform.SetParent(chunkTransform);

            spawnedCount++;
        }

        return spawnedCount;
    }

    private List<GameObject> FindGroundMeshObjects()
    {
        List<GameObject> meshObjects = new List<GameObject>();
        MeshRenderer[] allMeshRenderers = FindObjectsOfType<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in allMeshRenderers)
        {
            GameObject obj = meshRenderer.gameObject;
            if (obj.layer == LayerMask.NameToLayer("Ground"))
            {
                meshObjects.Add(obj);
            }
        }

        return meshObjects;
    }

    /// <summary>
    /// 삼각형 내부의 랜덤한 점을 반환
    /// </summary>
    private Vector3 RandomPointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        float r1 = Random.value;
        float r2 = Random.value;

        if (r1 + r2 > 1)
        {
            r1 = 1 - r1;
            r2 = 1 - r2;
        }

        float r3 = 1 - r1 - r2;

        return r1 * a + r2 * b + r3 * c;
    }

    private struct MeshData
    {
        public Mesh mesh;
        public Transform transform;
    }
}
