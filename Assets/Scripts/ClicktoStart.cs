// 2025-07-19 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Import the Input System namespace

public class FadeTextLoop : MonoBehaviour
{
    public TextMeshProUGUI text;           // ������ TextMeshPro �ؽ�Ʈ
    public float fadeDuration = 1f;      // �� �� ���̵� ��/�ƿ��ϴ� �� �ɸ��� �ð�

    private bool fadingOut = true;
    private float timer = 0f;

    void Reset()
    {
        text = GetComponent<TextMeshProUGUI>(); // �ڵ� ����
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