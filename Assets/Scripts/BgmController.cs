using UnityEngine;

public class BgmController : MonoBehaviour { 
    public AudioClip BattleBGM;
    public AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.PlayOneShot(BattleBGM);
    }

}
