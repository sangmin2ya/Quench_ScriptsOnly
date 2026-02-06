using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 위치에 따라 청크를 활성화/비활성화하는 런타임 로더입니다.
/// 씬에 빈 오브젝트를 만들고 이 컴포넌트를 추가하세요.
/// </summary>
public class GrassChunkLoader : MonoBehaviour
{
    [Header("References")]
    [Tooltip("청크들의 부모 오브젝트")]
    [SerializeField] private Transform chunksParent;

    [Tooltip("플레이어 Transform (비워두면 자동으로 Player 태그 검색)")]
    [SerializeField] private Transform player;

    [Header("Settings")]
    [Tooltip("플레이어로부터 이 거리 내의 청크만 활성화됩니다")]
    [SerializeField] private float loadDistance = 50f;

    [Tooltip("청크 체크 주기 (초)")]
    [SerializeField] private float updateInterval = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = false;

    private List<GrassChunk> allChunks = new List<GrassChunk>();
    private float nextUpdateTime;

    private void Start()
    {
        // 플레이어 자동 검색
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("GrassChunkLoader: Player를 찾을 수 없습니다. Player 태그를 확인하거나 직접 할당해주세요.");
            }
        }

        // 모든 청크 수집
        if (chunksParent != null)
        {
            CollectChunks();
        }
        else
        {
            Debug.LogError("GrassChunkLoader: Chunks Parent가 할당되지 않았습니다.");
        }
    }

    private void CollectChunks()
    {
        allChunks.Clear();

        foreach (Transform child in chunksParent)
        {
            GrassChunk chunk = child.GetComponent<GrassChunk>();
            if (chunk != null)
            {
                allChunks.Add(chunk);
            }
        }

        Debug.Log($"GrassChunkLoader: {allChunks.Count}개의 청크를 수집했습니다.");
    }

    private void Update()
    {
        if (player == null || Time.time < nextUpdateTime) return;

        nextUpdateTime = Time.time + updateInterval;
        UpdateChunks();
    }

    private void UpdateChunks()
    {
        Vector3 playerPos = player.position;
        float loadDistanceSqr = loadDistance * loadDistance;

        foreach (GrassChunk chunk in allChunks)
        {
            if (chunk == null) continue;

            // 청크 중심과 플레이어 거리 계산 (Y축 무시)
            Vector3 chunkCenter = chunk.chunkBounds.center;
            float distX = playerPos.x - chunkCenter.x;
            float distZ = playerPos.z - chunkCenter.z;
            float distanceSqr = distX * distX + distZ * distZ;

            bool shouldBeActive = distanceSqr <= loadDistanceSqr;

            if (chunk.gameObject.activeSelf != shouldBeActive)
            {
                chunk.gameObject.SetActive(shouldBeActive);
            }
        }
    }

    /// <summary>
    /// 런타임에 청크를 다시 수집합니다.
    /// </summary>
    public void RefreshChunks()
    {
        CollectChunks();
        UpdateChunks();
    }

    /// <summary>
    /// 로드 거리를 런타임에 변경합니다.
    /// </summary>
    public void SetLoadDistance(float distance)
    {
        loadDistance = distance;
        UpdateChunks();
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        // 로드 거리 시각화
        if (player != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawWireSphere(player.position, loadDistance);
        }

        // 청크 바운드 시각화
        if (chunksParent != null)
        {
            foreach (Transform child in chunksParent)
            {
                GrassChunk chunk = child.GetComponent<GrassChunk>();
                if (chunk != null)
                {
                    Gizmos.color = chunk.gameObject.activeSelf ? Color.green : Color.red;
                    Gizmos.DrawWireCube(chunk.chunkBounds.center, chunk.chunkBounds.size);
                }
            }
        }
    }
}
