using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI text;
    public static GameManager insatnce = null;
    public int coin;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        coin = 0;
        if (insatnce == null)
        {
            insatnce = this;
            // DontDestroyOnLoad(this.gameObject);
            // DontDestroyOnLoad(text);
        }

        coin = CoinManagement.Instance.coin;
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText(coin.ToString());
        coin = CoinManagement.Instance.coin;
    }

    public void IncreaseCoin()
    {
        CoinManagement.Instance.coin += 10;
        text.SetText(coin.ToString());
    }
}
