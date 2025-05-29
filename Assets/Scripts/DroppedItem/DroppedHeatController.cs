using UnityEngine;

public class DroppedHeatController : DroppedItemController
{
    [SerializeField]
    float addMaxHP = 1f;

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
        player.AddMaxHP(addMaxHP);
    }
}
