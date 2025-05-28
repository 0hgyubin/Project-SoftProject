using UnityEngine;

public class CoinManagement : MonoBehaviour
{
    public int coin = 0;
    private static CoinManagement instance = null;

    void Awake()
    {
        if (null == instance)
        {         
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static CoinManagement Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
}
