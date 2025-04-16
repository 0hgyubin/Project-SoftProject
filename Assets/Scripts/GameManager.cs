using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI text;
    public static GameManager insatnce = null;
    private int coin = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(insatnce == null){
            insatnce = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseCoin(){
        coin += 1;
        text.SetText(coin.ToString());
    }
}
