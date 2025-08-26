using UnityEngine;
using UnityEngine.InputSystem; // ���ο� Input System ����� ���� �߰�

public class GamePauseManager : MonoBehaviour
{
    // ������ ���� �Ͻ����� �������� �����ϴ� ����
    private bool isPaused = false;

    void Update()
    {
        // Ű������ ESC Ű�� �̹� �����ӿ� ���ȴ��� Ȯ��
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // isPaused ���� ���� ������ �簳�ϰų� �Ͻ�����
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// ������ �Ͻ������ϴ� �Լ�
    /// </summary>
    public void PauseGame()
    {
        // ���� �ӵ��� 0���� ����� ��� �ð� ��� �׼��� ����
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Game Paused");
    }

    /// <summary>
    /// ������ �ٽ� �簳�ϴ� �Լ�
    /// </summary>
    public void ResumeGame()
    {
        // ���� �ӵ��� 1�� �ǵ��� ���� �ӵ��� ����
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game Resumed");
    }
}