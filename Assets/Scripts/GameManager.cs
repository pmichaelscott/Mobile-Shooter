using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool _gameOver;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Lose()
    {
        if (_gameOver) return;
        _gameOver = true;
        Debug.Log("Game Over!");
        // For now: reload scene
        Invoke(nameof(Reload), 1.0f);
    }

    private void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
