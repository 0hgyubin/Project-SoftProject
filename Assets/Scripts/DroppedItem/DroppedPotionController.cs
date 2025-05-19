using UnityEngine;

public class DroppedPotionController : MonoBehaviour
{

    private bool canGetItem = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Player player = playerObject.GetComponent<Player>();
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= 0.5f)
        {
            Debug.Log("포션 먹을 수 있음");
            canGetItem = true;
        }
        else
        {
            canGetItem = false;
        }


        if(Input.GetKeyDown(KeyCode.W) && canGetItem)
        {
            player.hpUI.TakeDamaged(-40);
            Destroy(gameObject);
        }
    }
}
