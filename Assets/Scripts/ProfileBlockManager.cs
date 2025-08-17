// 2025-08-01 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

// 2025-07-31 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System.Collections.Generic;
using UnityEngine;

public class ProfileBlockManager : MonoBehaviour
{
    public GameObject profile; // Profile 오브젝트

    // 블록 프리팹들
    public GameObject blackBlock;
    public GameObject redBlock;
    public GameObject orangeBlock;
    public GameObject yellowBlock;
    public GameObject greenBlock;
    public GameObject blueBlock;
    public GameObject purpleBlock;
    public GameObject skyBlock;
    public GameObject whiteBlock;

    private Dictionary<int, GameObject> blockMapping; // 숫자와 블록 프리팹 매핑
    private SequenceManager sequenceManager;

    private void Start()
    {
        // SequenceManager 인스턴스 찾기
        sequenceManager = FindObjectOfType<SequenceManager>();
        if (sequenceManager == null)
        {
            Debug.LogError("SequenceManager is not found in the scene.");
            return;
        }

        // 블록 매핑 초기화
        InitializeBlockMapping();

        // 수열을 기반으로 블록 생성
        GenerateBlocksBasedOnSequence();
    }

    private void InitializeBlockMapping()
    {
        blockMapping = new Dictionary<int, GameObject>
        {
            { 1, blackBlock },
            { 2, redBlock },
            { 3, orangeBlock },
            { 4, yellowBlock },
            { 5, greenBlock },
            { 6, blueBlock },
            { 7, purpleBlock },
            { 8, skyBlock },
            { 9, whiteBlock }
        };

        // 블록 매핑 확인
        foreach (var pair in blockMapping)
        {
            if (pair.Value == null)
            {
                Debug.LogError($"Block prefab for value {pair.Key} is not assigned in the inspector.");
            }
        }
    }

    private void GenerateBlocksBasedOnSequence()
    {
        // SequenceManager에서 수열 가져오기
        List<int> generatedSequence = sequenceManager.GeneratedSequence;

        if (generatedSequence == null || generatedSequence.Count == 0)
        {
            Debug.LogWarning("Generated sequence is empty or null.");
            return;
        }

        Debug.Log($"Generated Sequence: {string.Join(", ", generatedSequence)}");

        // 수열의 길이에 따라 그리드 크기와 블록 크기 설정
        int gridSize;
        Vector3 blockScale;

        if (generatedSequence.Count == 9)
        {
            gridSize = 3;
            blockScale = new Vector3(0.05909031f, 0.05909031f, 0.05909031f);
        }
        else if (generatedSequence.Count == 4)
        {
            gridSize = 2;
            blockScale = new Vector3(0.08906015f, 0.08906015f, 0.08906015f);
        }
        else // generatedSequence.Count == 1
        {
            gridSize = 1;
            blockScale = new Vector3(0.1776078f, 0.1776078f, 0.1776078f);
        }

        // Profile의 SpriteRenderer를 기준으로 블록 배치
        SpriteRenderer profileRenderer = profile.GetComponent<SpriteRenderer>();
        if (profileRenderer == null)
        {
            Debug.LogError("Profile does not have a SpriteRenderer component.");
            return;
        }

        Vector3 profileOrigin = profileRenderer.bounds.min;
        Vector2 cellSize = profileRenderer.bounds.size / gridSize;

        // 수열의 각 숫자에 따라 블록 배치
        for (int i = 0; i < generatedSequence.Count; i++)
        {
            int value = generatedSequence[i];
            int row = i / gridSize;
            int col = i % gridSize;

            Vector3 position = new Vector3(
                profileOrigin.x + (col + 0.5f) * cellSize.x,
                profileOrigin.y + (gridSize - row - 0.5f) * cellSize.y,
                profile.transform.position.z
            );

            Debug.Log($"Placing block for value {value} at position {position}");

            // 숫자에 해당하는 블록 프리팹 가져오기
            if (blockMapping.TryGetValue(value, out GameObject blockPrefab))
            {
                if (blockPrefab == null)
                {
                    Debug.LogError($"Block prefab for value {value} is not assigned.");
                    continue;
                }

                GameObject block = Instantiate(blockPrefab, position, Quaternion.identity);
                block.transform.localScale = blockScale;

                // Profile의 자식으로 설정
                block.transform.SetParent(profile.transform);
            }
            else
            {
                Debug.LogError($"No block prefab found for value: {value}");
            }
        }
    }

    /// <summary>
    /// Profile에 배치된 블록을 모두 제거합니다.
    /// </summary>
    public void ClearProfileBlocks()
    {
        foreach (Transform child in profile.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 플레이어가 올바른 수열을 입력했을 때 호출됩니다.
    /// Profile의 블록을 제거하고 새로운 수열을 받아와 블록을 배치합니다.
    /// </summary>
    public void OnCorrectSequence()
    {
        // Profile의 블록 제거
        ClearProfileBlocks();

        // 새로운 수열을 기반으로 블록 배치
        GenerateBlocksBasedOnSequence();
    }
}