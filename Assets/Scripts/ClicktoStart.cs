// 2025-07-19 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Import the Input System namespace

public class FadeTextLoop : MonoBehaviour
{
    public TextMeshProUGUI text;           // 연결할 TextMeshPro 텍스트
    public float fadeDuration = 1f;      // 한 번 페이드 인/아웃하는 데 걸리는 시간

    private bool fadingOut = true;
    private float timer = 0f;

    void Reset()
    {
        text = GetComponent<TextMeshProUGUI>(); // 자동 연결
    }

    void Update()
    {
        if (text == null) return;

        timer += Time.deltaTime;
        float alpha = Mathf.PingPong(timer / fadeDuration, 1f);

        Color color = text.color;
        color.a = alpha;
        text.color = color;

        // Use the new Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more scenes to load. Make sure the next scene is added in Build Settings.");
        }
    }
}