// StressManager.cs

using UnityEngine;
using UnityEngine.UI; // UI ��Ҹ� ����ϱ� ���� �߰�

public class StressManager : MonoBehaviour
{
    // ��Ʈ���� ���� ����
    public float maxStress = 100f;
    public float currentStress = 0f;

    // ��Ʈ���� ������ ǥ���� UI �����̴�(������ ��)
    public Slider stressBar;

    private PlayerProgressTracker playerProgressTracker;

    void Start()
    {
        // PlayerProgressTracker�� ã�� ���ӿ��� ó���� ����
        playerProgressTracker = FindObjectOfType<PlayerProgressTracker>();
        if (playerProgressTracker == null)
        {
            Debug.LogError("PlayerProgressTracker is not found in the scene.");
        }

        // ��Ʈ���� �� �ʱ�ȭ
        UpdateStressBar();
    }

    /// <summary>
    /// ��Ʈ���� ������ ������Ű�� �Լ� (������ ������ �� ȣ��)
    /// </summary>
    /// <param name="amount">������ (������ ����)</param>
    public void IncreaseStress(int amount)
    {
        currentStress += amount;
        currentStress = Mathf.Clamp(currentStress, 0, maxStress); // 0 ~ 100 ���̷� �� ����
        Debug.Log($"Stress Increased: {currentStress}");

        UpdateStressBar();

        // ��Ʈ������ 100 �̻��̸� ���� ����
        if (currentStress >= maxStress)
        {
            playerProgressTracker.GameOver();
        }
    }

    /// <summary>
    /// ��Ʈ���� ������ ���ҽ�Ű�� �Լ� (�ð� �ʰ� �� ȣ��)
    /// </summary>
    /// <param name="amount">���ҷ�</param>
    public void DecreaseStress(float amount)
    {
        currentStress -= amount;
        currentStress = Mathf.Clamp(currentStress, 0, maxStress); // 0 ~ 100 ���̷� �� ����
        Debug.Log($"Stress Decreased: {currentStress}");

        UpdateStressBar();
    }

    /// <summary>
    /// UI ��Ʈ���� ���� ���� ������Ʈ�ϴ� �Լ�
    /// </summary>
    private void UpdateStressBar()
    {
        if (stressBar != null)
        {
            stressBar.value = currentStress;
        }
    }
}