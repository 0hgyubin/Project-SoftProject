using UnityEngine;

public class DroppedFeatherController : DroppedItemController
{

    [SerializeField]
    int addMoveSpeed = 1;


    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
    }

    protected override void getItem()
    {
        player.AddMoveSpeed(addMoveSpeed);
        Destroy(gameObject);
    }
}
