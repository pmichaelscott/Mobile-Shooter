using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public SquadSoldierAdder Squad { get; private set; }
    
    private bool _gameOver;

    private void Awake()
    {
        Debug.Log("GameManager Awake: " + gameObject.scene.name, this);
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

    public void RegisterSquad(SquadSoldierAdder squad)
    {
        Squad = squad;
    }
}
