using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float RemainingTime { get => GameTime; }

    public static GameManager Instance { get; private set; }

    private float GameTime = 5.0f;

    [SerializeField]
    private PlaneHoleMesh _planeHoleMesh = null;

    [SerializeField]
    private InGameScreen _inGameScreen = null;

    [SerializeField]
    private EndGameScreen _endGameScreen = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float distance = _planeHoleMesh.HoleCenter.magnitude;
        PlayerControls controls = GameObject.FindObjectOfType<PlayerControls>();
        GameTime = distance / controls.PlayerSpeed * Random.Range(1.5f, 2.5f) * 5;

        _inGameScreen.Show();
    }

    // Update is called once per frame
    void Update()
    {
        GameTime -= Time.deltaTime;
        _inGameScreen.setRemainingTime(GameTime);

        if (GameTime <= 0.0f)
        {
            GameTime = 0.0f;
            EndGame();
        }
    }

    public void EndGame()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        PlayerControls controls = GameObject.FindObjectOfType<PlayerControls>();
        controls.InputEnabled = false;

        _inGameScreen.Hide();
        _endGameScreen.Show();

        gameObject.SetActive(false);
    }
}
