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

    private readonly int maxPreGeneratedSequences = 7; // 미리 생성된 수열의 최대 개수 (7개로 변경)

    private float[] probabilities = { 70f, 30f, 0f }; // 초기 확률 (1개, 4개, 9개)
    private int probabilityIndex = 0; // 현재 확률 단계

    // 확률 변동 테이블
    private readonly float[][] probabilityTable = new float[][]
    {
        new float[] { 70f, 30f, 0f },  // 시작 확률
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
        new float[] { 0f, 0f, 100f }  // 최종 확률
    };

    private void Awake()
    {
        // 초기 확률 설정
        probabilities = probabilityTable[probabilityIndex];
        Debug.Log($"Initial probabilities: {probabilities[0]}%, {probabilities[1]}%, {probabilities[2]}%");

        // 초기 수열 생성
        generatedSequence = new List<int>();

        // 7개의 랜덤 수열을 미리 생성
        for (int i = 0; i < maxPreGeneratedSequences; i++)
        {
            List<int> newSequence = GenerateRandomSequenceInternal();
            preGeneratedSequences.Enqueue(newSequence);

            // 디버그 로그로 생성된 수열 출력
            Debug.Log($"Pre-generated Sequence {i + 1}: {string.Join(", ", newSequence)}");
        }
    }

    public void GenerateRandomSequence()
    {
        // 미리 생성된 수열에서 꺼내 사용
        if (preGeneratedSequences.Count > 0)
        {
            generatedSequence = preGeneratedSequences.Dequeue();
        }

        Debug.Log("Generated Sequence: " + string.Join(", ", generatedSequence));

        // 수열 개수가 6개로 줄어들면 새로운 수열 생성
        if (preGeneratedSequences.Count < maxPreGeneratedSequences - 1)
        {
            List<int> newSequence = GenerateRandomSequenceInternal();
            preGeneratedSequences.Enqueue(newSequence);
            Debug.Log("New Sequence Generated: " + string.Join(", ", newSequence));
        }
    }

    private List<int> GenerateRandomSequenceInternal()
    {
        List<int> sequence = new List<int>();

        // 랜덤 숫자 개수 선택
        int randomCount = GetRandomCount();

        // 선택된 개수만큼 숫자를 랜덤하게 생성
        for (int i = 0; i < randomCount; i++)
        {
            int randomNumber = UnityEngine.Random.Range(1, 10); // UnityEngine.Random 사용
            sequence.Add(randomNumber);
        }

        return sequence;
    }

    private int GetRandomCount()
    {
        // 확률에 따라 숫자 개수 선택
        float randomValue = UnityEngine.Random.Range(0f, 100f); // 0부터 100 사이의 랜덤 값
        if (randomValue < probabilities[0])
        {
            return 1; // 1개 선택
        }
        else if (randomValue < probabilities[0] + probabilities[1])
        {
            return 4; // 4개 선택
        }
        else
        {
            return 9; // 9개 선택
        }
    }

    // 플레이어 진행 상황에 따라 확률 업데이트
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