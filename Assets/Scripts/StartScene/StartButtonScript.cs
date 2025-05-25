using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartButtonScript : MonoBehaviour
{
    [Header("Ŭ�� ����")]
    public AudioClip clickSound;
    AudioSource audioPlayer;

    void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
        audioPlayer.playOnAwake = false;
        audioPlayer.loop = false;
    }

    public void OnButtonClicked()
    {
        StartCoroutine(PlayClickAndLoad());
    }

    IEnumerator PlayClickAndLoad()
    {
        audioPlayer.PlayOneShot(clickSound);

        yield return new WaitForSeconds(clickSound.length);

        SceneManager.LoadScene("MapTest");
    }
}
