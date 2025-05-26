using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartButtonScript : MonoBehaviour
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

        SceneManager.LoadScene("MapTest");
    }
}
