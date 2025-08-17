// 2025-08-11 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

// 2025-08-04 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerProgressTracker : MonoBehaviour
{
    public SequenceManager sequenceManager; // SequenceManager 참조
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText; // 콤보 TMP 텍스트 컴포넌트 참조
    public TextMeshProUGUI gameOverText; // "Game Over" 텍스트 TMP 컴포넌트
    public TextMeshProUGUI remainingText; // 남은 수열 개수 TMP 텍스트 컴포넌트
    public Image screenOverlay; // 화면 어둡게 할 이미지 (검은색 투명 이미지)

    public Image[] hpIcons; // HP 아이콘 배열 (Inspector에서 설정)
    private int currentHp; // 현재 HP

    private int correctSequenceCount = 0; // 플레이어가 맞춘 수열의 개수
    private int comboStack = 0; // 콤보 스택
    private int remainingSequences; // 다음 단계로 넘어가기 위해 남은 수열 개수
    private int currentStage = 0; // 현재 단계
    public int score = 0; // 점수 변수

    private readonly int[] sequencesToAdvance = { 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 10, 10 }; // 단계별 필요한 수열 개수

    public int CorrectSequenceCount => correctSequenceCount; // 외부에서 읽기 가능
    public int ComboStack => comboStack; // 외부에서 읽기 가능

    private bool isGameOver = false; // 게임 오버 상태

    public Image screenOverlay_red;

    private void Start()
    {
        if (sequenceManager == null)
        {
            sequenceManager = FindObjectOfType<SequenceManager>();
            if (sequenceManager == null)
            {
                Debug.LogError("SequenceManager is not assigned or found in the scene.");
            }
        }

        if (comboText != null)
        {
            UpdateComboText();
        }
        else
        {
            Debug.LogWarning("Combo Text (TextMeshProUGUI) is not assigned in the inspector.");
        }

        if (remainingText != null)
        {
            UpdateRemainingText();
        }
        else
        {
            Debug.LogWarning("Remaining Text (TextMeshProUGUI) is not assigned in the inspector.");
        }

        // HP 초기화
        currentHp = hpIcons.Length;

        // "Game Over" 텍스트와 화면 어둡게 하는 이미지 초기화
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false); // 초기에는 비활성화
        }

        if (screenOverlay != null)
        {
            screenOverlay.color = new Color(0, 0, 0, 0); // 초기에는 투명
        }

        // 초기 진행 상태 설정
        InitializeProgress();

        ResetScreenOverlay();
    }

    // 초기 진행 상태 설정
    private void InitializeProgress()
    {
        correctSequenceCount = 0; // 맞춘 수열 개수 초기화
        comboStack = 0; // 콤보 스택 초기화
        currentStage = 0; // 현재 단계 초기화
        remainingSequences = sequencesToAdvance[currentStage]; // 초기 남은 수열 개수 설정

        // TMP 텍스트 업데이트
        UpdateComboText();
        UpdateRemainingText();
    }

    // 플레이어가 수열을 맞췄을 때 호출
    public void OnCorrectSequence()
    {
        if (isGameOver) return;

        correctSequenceCount++;
        comboStack++; // 콤보 스택 증가
        remainingSequences--; // 남은 수열 개수 감소

        // 점수 증가 로직
        if (sequenceManager != null && sequenceManager.GeneratedSequence != null)
        {
            int sequenceLength = sequenceManager.GeneratedSequence.Count;
            if (sequenceLength == 1)
            {
                score += 1; // 숫자가 1개인 수열
            }
            else if (sequenceLength == 4)
            {
                score += 4; // 숫자가 4개인 수열
            }
            else if (sequenceLength == 9)
            {
                score += 9; // 숫자가 9개인 수열
            }
            else
            {
                Debug.LogWarning("Unexpected sequence length. No score added.");
            }
        }
        else
        {
            Debug.LogError("SequenceManager or GeneratedSequence is null. Cannot calculate score.");
        }

        // 점수 텍스트 업데이트
        UpdateScoreText();

        // 콤보 텍스트 및 남은 수열 텍스트 업데이트
        UpdateComboText();
        UpdateRemainingText();

        // 콤보가 5 스택에 도달하면 HP 회복 시도
        if (comboStack % 5 == 0)
        {
            RecoverHp();
        }

        // 남은 수열 개수가 0이 되면 다음 단계로 이동
        if (remainingSequences <= 0)
        {
            AdvanceToNextStage();
        }
    }

    // 다음 단계로 이동
    private void AdvanceToNextStage()
    {
        if (currentStage < sequencesToAdvance.Length - 1)
        {
            currentStage++;
            remainingSequences = sequencesToAdvance[currentStage];
            sequenceManager.UpdateProbabilitiesBasedOnProgress();
            UpdateRemainingText();
        }
        else
        {
            Debug.Log("Maximum stage reached. No further progression.");
        }
    }

    // 플레이어가 잘못된 입력을 했을 때 호출
    public void OnIncorrectInput()
    {
        if (isGameOver) return;

        comboStack = 0; // 콤보 스택 초기화
        UpdateComboText();

        // HP 감소
        DecreaseHp();
    }

    // HP 감소 처리
    private void DecreaseHp()
    {
        if (currentHp > 0)
        {
            currentHp--;

            // HP 아이콘 색상 변경
            hpIcons[currentHp].color = new Color32(0x5A, 0x5A, 0x5A, 0xFF); // 5A5A5A 색상

            if (currentHp == 0)
            {
                GameOver();
            }
        }
    }

    // HP 회복 처리
    private void RecoverHp()
    {
        if (currentHp < hpIcons.Length)
        {
            // 가장 나중에 닳은 HP를 회복
            hpIcons[currentHp].color = new Color32(0xFF, 0xDC, 0xB8, 0xFF); // FFDCB8 색상으로 복구
            currentHp++;
            Debug.Log("HP Recovered!");
        }
    }

    // 게임 오버 처리
    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over! All HP is depleted.");

        // 게임 진행 멈춤
        Time.timeScale = 0f;

        // 화면 어둡게 하기
        if (screenOverlay != null)
        {
            screenOverlay.color = new Color(0, 0, 0, 0.5f); // 반투명 검은색
        }

        // 화면 붉어짐 효과 초기화
        if (screenOverlay_red != null)
        {
            screenOverlay_red.color = new Color(0, 0, 0, 0); // 투명하게 설정
        }

        // "Game Over" 텍스트 표시
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "Game Over";
        }
    }

    // TMP 텍스트 업데이트 메서드
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}"; // 점수를 업데이트
        }
        else
        {
            Debug.LogWarning("Score Text (TextMeshProUGUI) is not assigned in the inspector.");
        }
    }

    private void UpdateComboText()
    {
        if (comboText != null)
        {
            comboText.text = $"Combo: {comboStack}";
        }
    }

    private void UpdateRemainingText()
    {
        if (remainingText != null)
        {
            remainingText.text = $"Remaining: {remainingSequences}";
        }
    }

    // 게임 오버 상태 확인
    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void UpdateScreenOverlayColor(float progress)
    {
        if (screenOverlay_red != null)
        {
            // 진행 비율에 따라 화면이 붉어짐 (0: 투명, 1: 완전 붉음)
            screenOverlay_red.color = new Color(1f, 0f, 0f, Mathf.Lerp(0f, 0.5f, progress));
        }
    }

    public void ResetScreenOverlay()
    {
        if (screenOverlay_red != null)
        {
            screenOverlay_red.color = new Color(0, 0, 0, 0); // 초기화 (투명)
        }
    }
}