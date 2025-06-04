using UnityEngine;

public abstract class DroppedItemController : MonoBehaviour
{

    protected bool isOnGroun = false;

    protected GameObject playerObject;
    protected Player player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        player = playerObject.GetComponent<Player>();
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (isOnGroun)
        {
            if (distance <= 0.5f)
            {
                Debug.Log("아이템 먹을 수 있음");
                getItem();
            }
        }
    }


    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("OneWayGround"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            isOnGroun = true;
        }
    }

    protected abstract void getItem();

}
