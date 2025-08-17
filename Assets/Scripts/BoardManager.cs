// 2025-07-28 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardManager : MonoBehaviour
{
    public GameObject woodBoard;
    public GameObject mainCharacterBoard;

    public GameObject blackBlock;
    public GameObject redBlock;
    public GameObject orangeBlock;
    public GameObject yellowBlock;
    public GameObject greenBlock;
    public GameObject blueBlock;
    public GameObject purpleBlock;
    public GameObject skyBlock;
    public GameObject whiteBlock;

    public AudioClip wrongBuzzer; // 잘못된 입력 시 재생할 소리
    public AudioClip woodBlockHit; // 블록이 맞을 때 재생할 소리
    public AudioClip correct_bell; // 올바른 입력 시 재생할 소리
    private AudioSource audioSource;

    public Sprite[] numberSprites; // 1부터 9까지의 숫자 스프라이트 배열

    public float timeLimit; // 제한시간
    private float timeRemaining; // 남은 시간
    private bool isTimerRunning = false;

    private Dictionary<int, GameObject> blockMapping;
    private List<GameObject> mainCharacterBlocks = new List<GameObject>();

    private int currentIndex = 0;
    private List<int> playerInputSequence;

    private SequenceManager sequenceManager;
    private PlayerProgressTracker playerProgressTracker; // PlayerProgressTracker 참조
    private ProfileBlockManager profileBlockManager;

    private void Start()
    {
        InitializeBlockMapping();

        // AudioSource 컴포넌트 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // SequenceManager 인스턴스 찾기
        sequenceManager = FindObjectOfType<SequenceManager>();
        if (sequenceManager == null)
        {
            Debug.LogError("SequenceManager is not found in the scene.");
            return;
        }

        // PlayerProgressTracker 인스턴스 찾기
        playerProgressTracker = FindObjectOfType<PlayerProgressTracker>();
        if (playerProgressTracker == null)
        {
            Debug.LogError("PlayerProgressTracker is not found in the scene.");
            return;
        }

        profileBlockManager = FindObjectOfType<ProfileBlockManager>();
        if (profileBlockManager == null)
        {
            Debug.LogError("profileBlockManager is not found in the scene.");
            return;
        }

        // 초기 수열 생성
        sequenceManager.GenerateRandomSequence();

        // PlayerProgressTracker 초기화는 Start()에서 이미 처리됨
        playerInputSequence = new List<int>();

        // 숫자 스프라이트 배치
        PlaceNumberSprites();

        // 제한시간 초기화
        InitializeTimer();
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
    }

    private void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard == null) return;

        // 숫자 키 입력 처리
        if (keyboard.digit1Key.wasPressedThisFrame) HandlePlayerInput(1);
        if (keyboard.digit2Key.wasPressedThisFrame) HandlePlayerInput(2);
        if (keyboard.digit3Key.wasPressedThisFrame) HandlePlayerInput(3);
        if (keyboard.digit4Key.wasPressedThisFrame) HandlePlayerInput(4);
        if (keyboard.digit5Key.wasPressedThisFrame) HandlePlayerInput(5);
        if (keyboard.digit6Key.wasPressedThisFrame) HandlePlayerInput(6);
        if (keyboard.digit7Key.wasPressedThisFrame) HandlePlayerInput(7);
        if (keyboard.digit8Key.wasPressedThisFrame) HandlePlayerInput(8);
        if (keyboard.digit9Key.wasPressedThisFrame) HandlePlayerInput(9);

        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;
            UpdateScreenOverlay();

            if (timeRemaining <= 0)
            {
                HandleTimeOut();
            }
        }
    }

    private void HandlePlayerInput(int input)
    {
        if (sequenceManager == null)
        {
            Debug.LogError("SequenceManager is null. Ensure it is assigned or present in the scene.");
            return;
        }

        List<int> generatedSequence = sequenceManager.GeneratedSequence;

        if (generatedSequence == null)
        {
            Debug.LogError("Generated sequence is null. Ensure that SequenceManager.GenerateRandomSequence() is called.");
            return;
        }

        if (currentIndex < 0 || currentIndex >= generatedSequence.Count)
        {
            Debug.LogWarning("Input index is out of range. Resetting player input.");
            ResetPlayerInput();
            return;
        }

        if (input == generatedSequence[currentIndex])
        {
            playerInputSequence.Add(input);
            PlaceBlock(input);
            currentIndex++;

            PlaySound(woodBlockHit);

            if (playerInputSequence.Count == generatedSequence.Count)
            {
                Debug.Log("Correct Sequence!");

                playerProgressTracker.OnCorrectSequence();

                // ClearBoard와 다음 수열 생성 전에 딜레이를 추가
                StartCoroutine(HandleCorrectSequence());
            }
        }
        else
        {
            Debug.Log("Incorrect input. Try again.");

            // 게임 오버 상태가 아니면 화면 흔들림 효과 실행
            if (!playerProgressTracker.IsGameOver())
            {
                StopAllCoroutines(); // 기존 흔들림 효과 중단
                StartCoroutine(ShakeCamera());
            }

            PlaySound(wrongBuzzer);

            // 콤보 초기화 및 HP 감소
            playerProgressTracker.OnIncorrectInput();
        }
    }

    private System.Collections.IEnumerator HandleCorrectSequence()
    {
        // Correct bell 사운드 재생
        PlaySound(correct_bell);

        // 0.5초 대기
        yield return new WaitForSeconds(0.5f);

        // 보드 초기화 및 다음 수열로 넘어가기
        ClearBoard();
        MoveToNextSequence();
    }

    private void MoveToNextSequence()
    {
        // 다음 수열 생성
        sequenceManager.GenerateRandomSequence();

        // 플레이어 입력 초기화
        ResetPlayerInput();

        // 제한시간 초기화
        InitializeTimer();
    }

    private void ResetPlayerInput()
    {
        playerInputSequence.Clear();
        currentIndex = 0;

        // 숫자 스프라이트 재배치
        PlaceNumberSprites();
    }

    private void PlaceNumberSprites()
    {
        List<int> generatedSequence = sequenceManager.GeneratedSequence;

        if (generatedSequence == null || generatedSequence.Count == 0)
        {
            Debug.LogError("Generated sequence is null or empty.");
            return;
        }

        if (numberSprites == null || numberSprites.Length < 9)
        {
            Debug.LogError("NumberSprites array is not properly initialized. Ensure it contains 9 sprites.");
            return;
        }

        int currentGridSize = Mathf.CeilToInt(Mathf.Sqrt(generatedSequence.Count)); // 1x1, 2x2, 3x3 결정

        // 숫자 개수에 따른 스프라이트 크기 설정
        Vector3 spriteScale;
        switch (generatedSequence.Count)
        {
            case 1:
                spriteScale = new Vector3(0.65f, 0.65f, 0.65f);
                break;
            case 4:
                spriteScale = new Vector3(0.35f, 0.35f, 0.35f);
                break;
            case 9:
                spriteScale = new Vector3(0.24f, 0.24f, 0.24f);
                break;
            default:
                Debug.LogWarning("Unexpected sequence count. Using default scale.");
                spriteScale = new Vector3(0.35f, 0.35f, 0.35f); // 기본 크기
                break;
        }

        SpriteRenderer boardRenderer = woodBoard.GetComponent<SpriteRenderer>();
        Vector3 boardOrigin = boardRenderer.bounds.min;

        Vector2 cellSizeDynamic = boardRenderer.bounds.size / currentGridSize;

        for (int i = 0; i < generatedSequence.Count; i++)
        {
            int row = i / currentGridSize;
            int col = i % currentGridSize;

            Vector3 position = new Vector3(
                boardOrigin.x + (col + 0.5f) * cellSizeDynamic.x,
                boardOrigin.y + (currentGridSize - row - 0.5f) * cellSizeDynamic.y,
                woodBoard.transform.position.z
            );

            int number = generatedSequence[i];
            if (number < 1 || number > 9)
            {
                Debug.LogWarning($"Invalid number in sequence: {number}. Skipping this number.");
                continue;
            }

            GameObject numberObject = new GameObject($"Number_{number}");
            numberObject.transform.position = position;
            numberObject.transform.localScale = spriteScale; // 설정된 스프라이트 크기 적용
            numberObject.transform.SetParent(woodBoard.transform);

            SpriteRenderer spriteRenderer = numberObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = numberSprites[number - 1]; // 1부터 9까지의 스프라이트 매핑

            // Sorting Layer와 Sorting Order 설정
            spriteRenderer.sortingLayerName = "ui"; // UI 레이어로 설정
            spriteRenderer.sortingOrder = 10; // 필요한 경우 정렬 순서를 조정
        }
    }

    private void InitializeTimer()
    {
        if (sequenceManager.GeneratedSequence.Count == 1)
        {
            timeLimit = 2f;
        }
        else if (sequenceManager.GeneratedSequence.Count == 4)
        {
            timeLimit = 3f;
        }
        else if (sequenceManager.GeneratedSequence.Count == 9)
        {
            timeLimit = 4f;
        }
        else
        {
            Debug.LogWarning("Unexpected sequence count. Using default time limit.");
            timeLimit = 3f; // 기본 제한시간
        }

        timeRemaining = timeLimit;
        isTimerRunning = true;

        // 화면 붉어짐 초기화
        playerProgressTracker.ResetScreenOverlay();
    }

    private void HandleTimeOut()
    {
        isTimerRunning = false;
        Debug.Log("Time's up! Player failed to input the sequence.");

        // 잘못된 입력 처리
        playerProgressTracker.OnIncorrectInput();

        // 이전에 생성된 숫자 스프라이트와 블록 제거
        ClearBoard();

        // 다음 수열로 넘어가기
        MoveToNextSequence();
    }

    private void UpdateScreenOverlay()
    {
        if (playerProgressTracker != null)
        {
            float progress = 1 - (timeRemaining / timeLimit); // 제한시간 진행 비율 (0~1)
            playerProgressTracker.UpdateScreenOverlayColor(progress);
        }
    }

    private void ClearBoard()
    {
        foreach (Transform child in woodBoard.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject block in mainCharacterBlocks)
        {
            if (block != null)
            {
                Destroy(block);
            }
        }

        mainCharacterBlocks.Clear();
    }

    private void PlaceBlock(int blockType)
    {
        // 블록 크기와 위치 계산
        Vector3 blockScale;
        Vector3 characterBlockScale;
        int currentGridSize;

        List<int> generatedSequence = sequenceManager.GeneratedSequence;

        if (generatedSequence.Count == 9)
        {
            currentGridSize = 3;
            blockScale = new Vector3(0.1154311f, 0.1154311f, 0.1154311f);
            characterBlockScale = new Vector3(0.04084672f, 0.04084672f, 0.04084672f);
        }
        else if (generatedSequence.Count == 4)
        {
            currentGridSize = 2;
            blockScale = new Vector3(0.1777869f, 0.1777869f, 0.1777869f);
            characterBlockScale = new Vector3(0.05808794f, 0.05808794f, 0.05808794f);
        }
        else // generatedSequence.Count == 1
        {
            currentGridSize = 1;
            blockScale = new Vector3(0.3496134f, 0.3496134f, 0.3496134f);
            characterBlockScale = new Vector3(0.1145668f, 0.1145668f, 0.1145668f);
        }

        // 블록의 row와 col 계산
        int row = currentIndex / currentGridSize;
        int col = currentIndex % currentGridSize;

        // woodBoard에 블록 배치
        if (blockMapping.TryGetValue(blockType, out GameObject blockPrefab))
        {
            SpriteRenderer boardRenderer = woodBoard.GetComponent<SpriteRenderer>();
            Vector3 boardOrigin = boardRenderer.bounds.min;

            Vector2 cellSizeDynamic = boardRenderer.bounds.size / currentGridSize;

            Vector3 position = new Vector3(
                boardOrigin.x + (col + 0.5f) * cellSizeDynamic.x,
                boardOrigin.y + (currentGridSize - row - 0.5f) * cellSizeDynamic.y,
                woodBoard.transform.position.z
            );

            GameObject block = Instantiate(blockPrefab, position, Quaternion.identity);
            block.transform.localScale = blockScale;

            // woodBoard의 자식으로 설정
            block.transform.SetParent(woodBoard.transform);
        }

        // mainCharacterBoard에 블록 배치
        if (blockMapping.TryGetValue(blockType, out GameObject mainBlockPrefab))
        {
            SpriteRenderer mainBoardRenderer = mainCharacterBoard.GetComponent<SpriteRenderer>();
            Vector3 mainBoardOrigin = mainBoardRenderer.bounds.min;

            Vector2 cellSizeDynamicMain = mainBoardRenderer.bounds.size / currentGridSize;

            Vector3 mainPosition = new Vector3(
                mainBoardOrigin.x + (col + 0.5f) * cellSizeDynamicMain.x,
                mainBoardOrigin.y + (currentGridSize - row - 0.5f) * cellSizeDynamicMain.y,
                mainCharacterBoard.transform.position.z
            );

            GameObject mainBlock = Instantiate(mainBlockPrefab, mainPosition, Quaternion.identity);
            mainBlock.transform.localScale = characterBlockScale;

            // mainCharacterBoard의 자식으로 설정
            mainBlock.transform.SetParent(mainCharacterBoard.transform);

            // mainCharacterBlocks 리스트에 추가
            mainCharacterBlocks.Add(mainBlock);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private System.Collections.IEnumerator ShakeCamera()
    {
        if (playerProgressTracker.IsGameOver()) yield break; // 게임 오버 상태라면 흔들림 효과 중단

        Transform cameraTransform = Camera.main.transform;
        Vector3 originalPosition = cameraTransform.position;

        float duration = 0.2f; // 흔들림 지속 시간
        float magnitude = 0.1f; // 흔들림 강도

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (playerProgressTracker.IsGameOver()) break; // 게임 오버 상태라면 흔들림 효과 중단

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cameraTransform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraTransform.position = originalPosition;
    }
}