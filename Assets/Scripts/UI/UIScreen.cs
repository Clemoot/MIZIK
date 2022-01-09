using System.Collections;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float FadingTime = 0.5f;

    private CanvasGroup _canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Show()
    {
        _canvasGroup.enabled = true;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        StartCoroutine(Fade(0.0f, 1.0f));
    }

    public virtual void Hide()
    {
        _canvasGroup.enabled = false;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        StartCoroutine(Fade(1.0f, 0.0f));
    }


    IEnumerator Fade(float start, float end)
    {
        float time = 0;
        while (time <= FadingTime)
        {
            time += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(start, end, time / FadingTime);
            yield return null;
        }
    }
}
