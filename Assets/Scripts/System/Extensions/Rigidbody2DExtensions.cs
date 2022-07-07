using UnityEngine;

public static class Rigidbody2DExtensions
{
    public static void SetVelocity(this Rigidbody2D rigidbody2D, Vector2 velocity)
    {
        rigidbody2D.velocity = velocity;
    }
}
