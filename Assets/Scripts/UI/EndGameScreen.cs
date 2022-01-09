using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameScreen : UIScreen
{
    [SerializeField]
    private Text _victoryText = null;
    [SerializeField]
    private Text _timeText = null;

    public override void Show()
    {
        float time = GameManager.Instance.RemainingTime;
        if (time > 0.0f)  // Victoire
        {
            _victoryText.text = "VICTOIRE";
            _timeText.text = string.Format("Temps restant: {0:f1}s", time);
        }
        else    // Défaite
        {
            _victoryText.text = "DEFAITE";
            _timeText.text = "";
        }

        base.Show();
    }

    public void Play()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
