using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoverButtonController : MonoBehaviour
{
    [Header("Click Sound")]
    public AudioClip clickSound;
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void OnButtonClicked()
    {
        StartCoroutine(PlayClickAndLoad());
    }

    IEnumerator PlayClickAndLoad()
    {
        audioSource.PlayOneShot(clickSound);

        yield return new WaitForSeconds(clickSound.length);

        // 플레이어 스탯 초기화
        if (PlayerStatsManager.Instance != null)
        {
            // 모든 스탯을 기본값으로 초기화
            PlayerStatsManager.Instance.InitializeDefaultStats();
        }

        // 기존 오브젝트들 제거
        if (MapController.Instance != null)
        {
            if (MapController.Instance.Map != null)
                Destroy(MapController.Instance.mapParent.gameObject);
            if (MapController.Instance.player != null)
                Destroy(MapController.Instance.player.gameObject);
            Destroy(MapController.Instance.gameObject);
        }

        SceneManager.LoadScene("MapTest");
    }
}
