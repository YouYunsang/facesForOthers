// StressManager.cs

using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 추가

public class StressManager : MonoBehaviour
{
    // 스트레스 지수 변수
    public float maxStress = 100f;
    public float currentStress = 0f;

    // 스트레스 지수를 표시할 UI 슬라이더(게이지 바)
    public Slider stressBar;

    private PlayerProgressTracker playerProgressTracker;

    void Start()
    {
        // PlayerProgressTracker를 찾아 게임오버 처리를 위임
        playerProgressTracker = FindObjectOfType<PlayerProgressTracker>();
        if (playerProgressTracker == null)
        {
            Debug.LogError("PlayerProgressTracker is not found in the scene.");
        }

        // 스트레스 바 초기화
        UpdateStressBar();
    }

    /// <summary>
    /// 스트레스 지수를 증가시키는 함수 (수열을 맞췄을 때 호출)
    /// </summary>
    /// <param name="amount">증가량 (수열의 길이)</param>
    public void IncreaseStress(int amount)
    {
        currentStress += amount;
        currentStress = Mathf.Clamp(currentStress, 0, maxStress); // 0 ~ 100 사이로 값 제한
        Debug.Log($"Stress Increased: {currentStress}");

        UpdateStressBar();

        // 스트레스가 100 이상이면 게임 오버
        if (currentStress >= maxStress)
        {
            playerProgressTracker.GameOver();
        }
    }

    /// <summary>
    /// 스트레스 지수를 감소시키는 함수 (시간 초과 시 호출)
    /// </summary>
    /// <param name="amount">감소량</param>
    public void DecreaseStress(float amount)
    {
        currentStress -= amount;
        currentStress = Mathf.Clamp(currentStress, 0, maxStress); // 0 ~ 100 사이로 값 제한
        Debug.Log($"Stress Decreased: {currentStress}");

        UpdateStressBar();
    }

    /// <summary>
    /// UI 스트레스 바의 값을 업데이트하는 함수
    /// </summary>
    private void UpdateStressBar()
    {
        if (stressBar != null)
        {
            stressBar.value = currentStress;
        }
    }
}