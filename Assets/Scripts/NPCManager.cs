// 2025-08-19 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

// 2025-08-19 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Linq 사용을 위해 추가

public class NPCManager : MonoBehaviour
{
    public GameObject[] npcPrefabs;
    public GameObject npcArray;

    // 블록 프리팹들 (Inspector에서 할당 필요)
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
    private Queue<GameObject> activeNPCs = new Queue<GameObject>();
    private SequenceManager sequenceManager;

    private void Start()
    {
        if (npcPrefabs == null || npcPrefabs.Length == 0)
        {
            Debug.LogError("NPC Prefabs are not assigned or empty.");
            return;
        }

        if (npcArray == null)
        {
            Debug.LogError("npcArray is not assigned.");
            return;
        }

        sequenceManager = FindObjectOfType<SequenceManager>();
        if (sequenceManager == null)
        {
            Debug.LogError("SequenceManager is not found in the scene.");
            return;
        }

        // 블록 매핑 초기화
        InitializeBlockMapping();

        // 초기 NPC 생성 및 블록 배치
        GenerateInitialNPCs();
    }

    private void InitializeBlockMapping()
    {
        blockMapping = new Dictionary<int, GameObject>
    {
        { 1, blackBlock }, { 2, redBlock }, { 3, orangeBlock },
        { 4, yellowBlock }, { 5, greenBlock }, { 6, blueBlock },
        { 7, purpleBlock }, { 8, skyBlock }, { 9, whiteBlock }
    };

        // 프리팹 할당 여부를 검사하는 코드 추가
        foreach (var pair in blockMapping)
        {
            if (pair.Value == null)
            {
                // 어떤 블록이 할당되지 않았는지 정확히 알려주는 에러 로그 출력
                Debug.LogError($"Block prefab for key '{pair.Key}' is not assigned in the NPCManager component in the Inspector!");
            }
        }
    }

    private void GenerateInitialNPCs()
    {
        ClearNPCs();

        // 화면에 표시될 모든 수열 리스트를 준비
        List<List<int>> allVisibleSequences = new List<List<int>>();

        // 1. 현재 플레이해야 할 첫번째 수열을 리스트에 추가
        if (sequenceManager.GeneratedSequence != null && sequenceManager.GeneratedSequence.Count > 0)
        {
            allVisibleSequences.Add(sequenceManager.GeneratedSequence);
        }

        // 2. 큐에 대기중인 5개의 수열들을 리스트에 추가
        allVisibleSequences.AddRange(sequenceManager.PreGeneratedSequences.ToList());

        // 총 6개의 NPC를 생성
        for (int i = 0; i < allVisibleSequences.Count; i++)
        {
            GameObject randomNPCPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
            GameObject npcInstance = Instantiate(randomNPCPrefab, npcArray.transform);
            npcInstance.name = $"NPC_{i + 1}";

            // 각 NPC에게 순서에 맞는 수열 블록을 얼굴에 배치
            PlaceBlocksOnNPCFace(npcInstance, allVisibleSequences[i]);

            activeNPCs.Enqueue(npcInstance);
        }

        Debug.Log($"정상적으로 {activeNPCs.Count}개의 초기 NPC가 생성되었습니다.");
    }

    // UpdateNPCs가 새로 생성된 수열을 인자로 받도록 수정
    public void UpdateNPCs(List<int> newSequence)
    {
        if (activeNPCs.Count > 0)
        {
            GameObject firstNPC = activeNPCs.Dequeue();
            Destroy(firstNPC);
        }

        GameObject randomNPCPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
        if (randomNPCPrefab != null)
        {
            GameObject newNPC = Instantiate(randomNPCPrefab, npcArray.transform);
            newNPC.name = $"NPC_{Time.frameCount}";

            // 새로 생성된 NPC에는 새로 생성된 수열의 블록을 배치
            if (newSequence != null)
            {
                PlaceBlocksOnNPCFace(newNPC, newSequence);
            }

            activeNPCs.Enqueue(newNPC);
        }
    }

    /// <summary>
    /// NPC의 'face' 자식 오브젝트에 수열에 맞는 블록을 배치합니다.
    /// </summary>
    /// <param name="npc">블록을 배치할 NPC 게임 오브젝트</param>
    /// <param name="sequence">배치할 블록의 수열</param>
    private void PlaceBlocksOnNPCFace(GameObject npc, List<int> sequence)
    {
        Transform faceTransform = npc.transform.Find("face");
        if (faceTransform == null)
        {
            Debug.LogError($"NPC {npc.name} does not have a child object named 'face'.");
            return;
        }

        int gridSize;
        Vector3 blockScale;

        // mainCharacterBoard의 블록 스케일과 동일한 비율 적용
        if (sequence.Count == 9)
        {
            gridSize = 3;
            blockScale = new Vector3(0.04084672f, 0.04084672f, 0.04084672f);
        }
        else if (sequence.Count == 4)
        {
            gridSize = 2;
            blockScale = new Vector3(0.067f, 0.067f, 0.067f);
        }
        else // sequence.Count == 1
        {
            gridSize = 1;
            blockScale = new Vector3(0.1145668f, 0.1145668f, 0.1145668f);
        }

        // face 오브젝트의 위치와 크기를 기준으로 블록 배치 (face 오브젝트의 스케일이 1,1,1이라고 가정)
        Vector3 faceOrigin = faceTransform.position - new Vector3(0.5f, -0.5f, 0); // 임의의 로컬 원점 (가운데 상단 기준)
        float cellSize = 1.0f / gridSize; // face 오브젝트의 가로/세로를 1 unit으로 가정

        for (int i = 0; i < sequence.Count; i++)
        {
            int value = sequence[i];
            int row = i / gridSize;
            int col = i % gridSize;

            // 로컬 위치 계산
            Vector3 localPosition = new Vector3(
                (col + 0.5f) * cellSize - 0.5f,
                -(row + 0.5f) * cellSize + 0.5f,
                0
            );

            if (blockMapping.TryGetValue(value, out GameObject blockPrefab))
            {
                // 월드 위치로 변환하여 블록 생성
                GameObject block = Instantiate(blockPrefab, faceTransform.position + localPosition, Quaternion.identity);
                block.transform.localScale = blockScale;
                block.transform.SetParent(faceTransform);
            }
        }
    }

    private void ClearNPCs()
    {
        foreach (GameObject npc in activeNPCs)
        {
            if (npc != null)
            {
                Destroy(npc);
            }
        }
        activeNPCs.Clear();
    }
}