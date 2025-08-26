using UnityEngine;
using UnityEngine.InputSystem; // 새로운 Input System 사용을 위해 추가

public class GamePauseManager : MonoBehaviour
{
    // 게임이 현재 일시정지 상태인지 추적하는 변수
    private bool isPaused = false;

    void Update()
    {
        // 키보드의 ESC 키가 이번 프레임에 눌렸는지 확인
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // isPaused 값에 따라 게임을 재개하거나 일시정지
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
    /// 게임을 일시정지하는 함수
    /// </summary>
    public void PauseGame()
    {
        // 게임 속도를 0으로 만들어 모든 시간 기반 액션을 멈춤
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Game Paused");
    }

    /// <summary>
    /// 게임을 다시 재개하는 함수
    /// </summary>
    public void ResumeGame()
    {
        // 게임 속도를 1로 되돌려 정상 속도로 진행
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game Resumed");
    }
}