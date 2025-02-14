using UnityEngine;

public class WallCheck : MonoBehaviour
{
    private HusklingAI parentAI;

    void Start()
    {
        parentAI = GetComponentInParent<HusklingAI>(); // ? Find the main AI script
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall")) // ? Make sure walls are tagged correctly
        {
            Debug.Log("Huskling hit a wall!");
            parentAI?.OnWallHit(); // ? Calls function in parent to break mask
        }
    }
}



