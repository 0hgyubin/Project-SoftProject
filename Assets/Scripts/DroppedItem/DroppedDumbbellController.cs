using UnityEngine;

public class DroppedDumbbellController : DroppedItemController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    float addStrength = 1f;
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
        player.AddStrength(addStrength);
        Destroy(gameObject);
    }
}
