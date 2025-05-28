using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI text;
    public static GameManager insatnce = null;
    public int coin = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(insatnce == null){
            insatnce = this;
        }

        coin = CoinManagement.Instance.coin;
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText(coin.ToString());
        coin = CoinManagement.Instance.coin;
    }

    public void IncreaseCoin(){
        CoinManagement.Instance.coin += 1;
        text.SetText(coin.ToString());
    }
}
