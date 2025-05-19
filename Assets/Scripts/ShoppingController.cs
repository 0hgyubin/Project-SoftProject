using UnityEngine;

public class ShoppingController : MonoBehaviour
{

    [SerializeField]
    private Player player;


    [SerializeField]
    private int howMuch = 0;


    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    GameObject droppedItem;
    
    

    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W) && player.canDialoging)
        {
            Debug.Log("사기");
            if(gameManager.coin > howMuch)
            {
                gameManager.coin = gameManager.coin - howMuch;
                Instantiate(droppedItem, transform.position, Quaternion.identity);
            }
        }
    }
}
