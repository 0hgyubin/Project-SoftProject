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

        //싹다 지워야함 그래서 다 지웠다.

        // 1) 기존 컨트롤러를 없애기 .. << 이 방식이 틀렸을 수도
        if (MapController.Instance != null)
            Destroy(MapController.Instance.gameObject);
        if (MapController.Instance.Map != null)
            Destroy(MapController.Instance.mapParent.gameObject);        
        if (MapController.Instance.player != null) //플레이어를 없애기
            Destroy(MapController.Instance.player.gameObject);

        SceneManager.LoadScene("MapTest");
    }
}
