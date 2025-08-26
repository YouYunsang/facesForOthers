// 2025-08-05 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

// 2025-08-01 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    private List<int> generatedSequence;
    public List<int> GeneratedSequence => generatedSequence; // 현재 수열
    private Queue<List<int>> preGeneratedSequences = new Queue<List<int>>(); // 미리 생성된 수열 큐

    public Queue<List<int>> PreGeneratedSequences => preGeneratedSequences;

    private readonly int maxPreGeneratedSequences = 6;

    private float[] probabilities = { 70f, 30f, 0f };
    private int probabilityIndex = 0;

    private readonly float[][] probabilityTable = new float[][]
    {
        new float[] { 70f, 30f, 0f },
        new float[] { 65f, 35f, 0f },
        new float[] { 60f, 35f, 5f },
        new float[] { 55f, 35f, 10f },
        new float[] { 50f, 40f, 10f },
        new float[] { 40f, 45f, 15f },
        new float[] { 35f, 50f, 15f },
        new float[] { 25f, 55f, 20f },
        new float[] { 15f, 60f, 25f },
        new float[] { 10f, 65f, 25f },
        new float[] { 5f, 70f, 25f },
        new float[] { 0f, 65f, 35f },
        new float[] { 0f, 60f, 40f },
        new float[] { 0f, 55f, 45f },
        new float[] { 0f, 50f, 50f },
        new float[] { 0f, 45f, 55f },
        new float[] { 0f, 40f, 60f },
        new float[] { 0f, 30f, 70f },
        new float[] { 0f, 20f, 80f },
        new float[] { 0f, 10f, 90f },
        new float[] { 0f, 0f, 100f }
    };

    private NPCManager npcManager;

    private void Awake()
    {
        probabilities = probabilityTable[probabilityIndex];
        npcManager = FindObjectOfType<NPCManager>();
        if (npcManager == null)
        {
            Debug.LogError("NPCManager is not found in the scene.");
            return;
        }

        // 1. 게임 시작에 필요한 6개의 수열을 미리 생성
        for (int i = 0; i < maxPreGeneratedSequences; i++)
        {
            List<int> newSequence = GenerateRandomSequenceInternal();
            preGeneratedSequences.Enqueue(newSequence);
        }
        Debug.Log($"초기 6개 수열 생성 완료. 큐에 {preGeneratedSequences.Count}개 있음.");

        // 2. 큐에서 첫번째 수열을 꺼내 '현재 플레이할 수열'로 설정
        // 이제 큐에는 앞으로 나올 5개의 수열이 남게 됨
        if (preGeneratedSequences.Count > 0)
        {
            generatedSequence = preGeneratedSequences.Dequeue();
            Debug.Log("첫번째 플레이 수열 설정 완료: " + string.Join(", ", generatedSequence));
        }
    }

    // 함수의 이름을 역할에 맞게 변경 (GenerateRandomSequence -> AdvanceToNextSequence)
    public void AdvanceToNextSequence()
    {
        // 1. 큐에서 다음 수열을 꺼내 현재 플레이할 수열로 설정
        if (preGeneratedSequences.Count > 0)
        {
            generatedSequence = preGeneratedSequences.Dequeue();
        }
        else
        {
            Debug.LogError("Sequence queue is empty! 비상용 수열을 생성합니다.");
            generatedSequence = GenerateRandomSequenceInternal();
        }

        // 2. 큐의 맨 뒤에 추가할 새로운 수열을 생성
        List<int> newSequenceToAdd = GenerateRandomSequenceInternal();
        preGeneratedSequences.Enqueue(newSequenceToAdd);

        // 3. NPC를 업데이트하며, 새로 생성된 수열 정보를 전달
        npcManager.UpdateNPCs(newSequenceToAdd);
    }

    private List<int> GenerateRandomSequenceInternal()
    {
        List<int> sequence = new List<int>();
        int randomCount = GetRandomCount();

        for (int i = 0; i < randomCount; i++)
        {
            int randomNumber = UnityEngine.Random.Range(1, 10);
            sequence.Add(randomNumber);
        }
        return sequence;
    }

    private int GetRandomCount()
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        if (randomValue < probabilities[0]) return 1;
        else if (randomValue < probabilities[0] + probabilities[1]) return 4;
        else return 9;
    }

    public void UpdateProbabilitiesBasedOnProgress()
    {
        if (probabilityIndex < probabilityTable.Length - 1)
        {
            probabilityIndex++;
            probabilities = probabilityTable[probabilityIndex];
            Debug.Log($"Updated probabilities: {probabilities[0]}%, {probabilities[1]}%, {probabilities[2]}%");
        }
        else
        {
            Debug.Log("Final probabilities reached. No further updates.");
        }
    }
}