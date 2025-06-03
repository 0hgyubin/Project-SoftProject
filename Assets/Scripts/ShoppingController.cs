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




    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && player.canDialoging)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= 1)
            {
                Debug.Log("사기");
                if (gameManager.GetCoin() >= howMuch)
                {
                    gameManager.SetCoin(gameManager.GetCoin() - howMuch) ;
                    Instantiate(droppedItem, transform.position, Quaternion.identity);
                }
            }
        }
    }
}
