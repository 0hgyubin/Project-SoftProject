using UnityEngine;

public class DroppedWingController : DroppedItemController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
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
        player.AddMaxJumpCnt(1);
    }
}
