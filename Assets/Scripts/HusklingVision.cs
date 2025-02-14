using UnityEngine;

public class HusklingVision : MonoBehaviour
{
    private HusklingAI parentAI;

    void Start()
    {
        parentAI = GetComponentInParent<HusklingAI>(); // ? Get AI script from parent
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered vision trigger!");
            parentAI?.SetPlayerInVision(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited vision trigger!");
            parentAI?.SetPlayerInVision(false);
        }
    }
}


