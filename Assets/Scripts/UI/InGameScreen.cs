using UnityEngine;
using UnityEngine.UI;

public class InGameScreen : UIScreen
{
    [SerializeField]
    private Text _timeText = null;

    public void setRemainingTime(float time)
    {
        _timeText.text = string.Format("Temps restant: {0:f1}s", time);
    }
}
