#if UNITY_EDITOR
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
    // 같은 시드면 같은 배치가 생성됨 (배리에이션 비교용). 창을 다시 열어도 유지되도록 EditorPrefs에 저장
    private const string SeedPrefsKey = "Quench.GrassSpawner.Seed";
    private const string MaxSlopePrefsKey = "Quench.GrassSpawner.MaxSlope";
    private int seed = 0;
    // 이 각도보다 가파른 면(절벽, 아랫면)에는 배치하지 않음
    private float maxSlope = 60f;

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

    private void OnEnable()
    {
        seed = EditorPrefs.GetInt(SeedPrefsKey, 0);
        maxSlope = EditorPrefs.GetFloat(MaxSlopePrefsKey, 60f);
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
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            seed = EditorGUILayout.IntField("Seed", seed);
            if (GUILayout.Button("Random", GUILayout.Width(70)))
            {
                seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                GUI.FocusControl(null); // 입력 중인 필드에 새 값이 바로 보이도록
                EditorPrefs.SetInt(SeedPrefsKey, seed);
            }
            EditorGUILayout.EndHorizontal();
            maxSlope = EditorGUILayout.Slider("Max Slope", maxSlope, 0f, 90f);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(SeedPrefsKey, seed);
                EditorPrefs.SetFloat(MaxSlopePrefsKey, maxSlope);
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(10);

        // 밀도 설정 (청크당)
        EditorGUILayout.LabelField("Density Settings (Per Chunk)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("지면 표면 넓이 Chunk Size² 당 개수입니다. 청크 안 지면 표면 넓이에 비례해 배치됩니다.", MessageType.None);
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

        SurfaceSampler[,] chunkSamplers = BuildChunkSamplers(meshObjects, startPos, chunksX, chunksZ);

        // 종류 순서는 시드 파생에 쓰이므로 고정 (Grass, Rock, Cactus, Big Rock)
        SpawnGroup[] spawnGroups =
        {
            new SpawnGroup { prefabs = grassPrefabs.FindAll(p => p != null), density = grassDensity },
            new SpawnGroup { prefabs = rockPrefabs.FindAll(p => p != null), density = rockDensity },
            new SpawnGroup { prefabs = cactusPrefabs.FindAll(p => p != null), density = cactusDensity },
            new SpawnGroup { prefabs = bigRockPrefabs.FindAll(p => p != null), density = bigRockDensity },
        };

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

                // 이 청크 영역으로 잘린 지면 삼각형
                SurfaceSampler sampler = chunkSamplers[x, z];
                if (sampler.TriangleCount == 0) continue;

                // 청크 안 지면 넓이에 비례한 배치 개수 (Density는 지면이 꽉 찬 청크 기준)
                int[] counts = new int[spawnGroups.Length];
                int totalCount = 0;
                for (int i = 0; i < spawnGroups.Length; i++)
                {
                    counts[i] = spawnGroups[i].prefabs.Count > 0 ? ScaledCount(spawnGroups[i].density, sampler.TotalArea) : 0;
                    totalCount += counts[i];
                }
                if (totalCount == 0) continue;

                // 청크 오브젝트 생성
                GameObject chunkObject = new GameObject($"Chunk_{x}_{z}");
                chunkObject.transform.SetParent(parentTransform);
                chunkObject.transform.position = chunkCenter;

                // GrassChunk 컴포넌트 추가
                GrassChunk chunkComponent = chunkObject.AddComponent<GrassChunk>();
                chunkComponent.chunkBounds = chunkBounds;

                Undo.RegisterCreatedObjectUndo(chunkObject, "Create Chunk");

                // 오브젝트 스폰. 청크·종류마다 시드에서 파생한 난수를 써서
                // 다른 종류의 설정을 바꿔도 이 청크의 이 종류 배치는 그대로 재현됨 (지면 메시 구성이 같을 때)
                for (int i = 0; i < spawnGroups.Length; i++)
                {
                    if (counts[i] == 0) continue;
                    System.Random random = new System.Random(DeriveSeed(x, z, i));
                    totalSpawned += SpawnType(chunkObject.transform, spawnGroups[i].prefabs, counts[i], sampler, random);
                }
                chunkCount++;

                // 청크 비활성화 (런타임에 로더가 활성화)
                chunkObject.SetActive(false);
            }
        }

        Debug.Log($"Generated {chunkCount} chunks with {totalSpawned} total objects (seed {seed})");
    }

    private struct SpawnGroup
    {
        public List<GameObject> prefabs;
        public int density;
    }

    private int ScaledCount(int density, double groundArea)
    {
        return (int)System.Math.Round(density * groundArea / (chunkSize * chunkSize));
    }

    /// <summary>
    /// 각 메시의 정점을 한 번만 읽어 월드 좌표로 바꾸고, 삼각형을 겹치는 청크마다 청크 영역으로 잘라 분배
    /// </summary>
    private SurfaceSampler[,] BuildChunkSamplers(List<GameObject> meshObjects, Vector3 startPos, int chunksX, int chunksZ)
    {
        SurfaceSampler[,] samplers = new SurfaceSampler[chunksX, chunksZ];
        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                samplers[x, z] = new SurfaceSampler();
            }
        }

        // 0도일 때 회전 오차로 평평한 면까지 빠지지 않도록 약간 여유
        float minNormalY = Mathf.Min(Mathf.Cos(maxSlope * Mathf.Deg2Rad), 1f - 1e-4f);
        foreach (GameObject meshObject in meshObjects)
        {
            MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null) continue;

            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            int[] triangles = meshFilter.sharedMesh.triangles;
            Transform meshTransform = meshObject.transform;
            // 음수 스케일(반전)이면 월드 좌표에서 감기 순서가 뒤집혀 법선 부호도 반대
            float normalSign = meshTransform.localToWorldMatrix.determinant < 0f ? -1f : 1f;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = meshTransform.TransformPoint(vertices[i]);
            }

            for (int i = 0; i + 2 < triangles.Length; i += 3)
            {
                Vector3 a = vertices[triangles[i]];
                Vector3 b = vertices[triangles[i + 1]];
                Vector3 c = vertices[triangles[i + 2]];

                // 너무 가파르거나 아래를 향한 면은 제외
                Vector3 normal = Vector3.Cross(b - a, c - a).normalized * normalSign;
                if (normal.y < minNormalY) continue;

                float triMinX = Mathf.Min(a.x, Mathf.Min(b.x, c.x));
                float triMaxX = Mathf.Max(a.x, Mathf.Max(b.x, c.x));
                float triMinZ = Mathf.Min(a.z, Mathf.Min(b.z, c.z));
                float triMaxZ = Mathf.Max(a.z, Mathf.Max(b.z, c.z));
                int minX = Mathf.Max(0, Mathf.FloorToInt((triMinX - startPos.x) / chunkSize));
                int maxX = Mathf.Min(chunksX - 1, Mathf.FloorToInt((triMaxX - startPos.x) / chunkSize));
                int minZ = Mathf.Max(0, Mathf.FloorToInt((triMinZ - startPos.z) / chunkSize));
                int maxZ = Mathf.Min(chunksZ - 1, Mathf.FloorToInt((triMaxZ - startPos.z) / chunkSize));
                for (int x = minX; x <= maxX; x++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        float chunkMinX = startPos.x + x * chunkSize;
                        float chunkMinZ = startPos.z + z * chunkSize;
                        samplers[x, z].AddTriangleClippedXZ(a, b, c, chunkMinX, chunkMinX + chunkSize, chunkMinZ, chunkMinZ + chunkSize);
                    }
                }
            }
        }
        return samplers;
    }

    // 시드, 청크 좌표, 종류 인덱스를 섞어 청크·종류별 시드 생성
    private int DeriveSeed(int x, int z, int groupIndex)
    {
        unchecked
        {
            uint hash = (uint)seed;
            hash = (hash ^ (uint)x * 0x9E3779B1u) * 0x85EBCA6Bu;
            hash = (hash ^ (uint)z * 0xC2B2AE35u) * 0x27D4EB2Fu;
            hash = (hash ^ (uint)groupIndex * 0x165667B1u) * 0x85EBCA6Bu;
            hash ^= hash >> 16;
            return (int)(hash & 0x7FFFFFFF);
        }
    }

    /// <summary>
    /// 청크 안에 count개를 배치. 샘플러가 청크 영역으로 잘린 삼각형만 가지므로 모든 점이 청크 안에 있음
    /// </summary>
    private int SpawnType(Transform chunkTransform, List<GameObject> prefabs, int count, SurfaceSampler sampler, System.Random random)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPoint = sampler.Sample(random);

            GameObject prefab = prefabs[random.Next(prefabs.Count)];
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = randomPoint;
            instance.transform.rotation = Quaternion.Euler(0, random.Next(0, 360), 0);
            instance.transform.SetParent(chunkTransform);
        }

        return count;
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

        // FindObjectsOfType 순서는 보장되지 않으므로 하이어라키 순서로 정렬 (같은 시드 = 같은 배치)
        Dictionary<GameObject, string> keys = new Dictionary<GameObject, string>();
        foreach (GameObject meshObject in meshObjects)
        {
            keys[meshObject] = HierarchyKey(meshObject.transform);
        }
        meshObjects.Sort((left, right) => string.CompareOrdinal(keys[left], keys[right]));
        return meshObjects;
    }

    // 루트부터 각 단계의 형제 순서를 이어 붙인 정렬 키 (씬 경로 포함)
    private static string HierarchyKey(Transform target)
    {
        string key = "";
        for (Transform current = target; current != null; current = current.parent)
        {
            key = current.GetSiblingIndex().ToString("D6") + "/" + key;
        }
        return target.gameObject.scene.path + ":" + key;
    }
}
#endif
