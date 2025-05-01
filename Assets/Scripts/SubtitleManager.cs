using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance {get; private set;}
    [SerializeField] private TextMeshProUGUI subtitleText;

    private Coroutine hideCoroutine;

    void Awake()
    {
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    public void ShowSubtitle(string message, float duration){
        subtitleText.text = message;
        subtitleText.gameObject.SetActive(true);

        if(hideCoroutine != null){
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideAfter(duration));
    }

    private IEnumerator HideAfter(float t){
        yield return new WaitForSeconds(t);
        subtitleText.gameObject.SetActive(false);
    }
}
