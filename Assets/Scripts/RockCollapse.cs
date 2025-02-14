using UnityEngine;
using System.Collections;

public class RockCollapse : MonoBehaviour
{
    private Animator animator;
    private bool hasFallen = false;

    void Start()
    {
        animator = GetComponent<Animator>(); // ? Get the Animator component
    }

    public void TriggerRockFall()
    {
        if (!hasFallen)
        {
            hasFallen = true;

            // ? Play the Rock Fall animation
            animator.SetTrigger("FallTrigger");

            // ? Ensure camera shake only runs if CameraShake exists
            if (CameraShake.instance != null)
            {
                CameraShake.instance.ShakeCamera();
            }
            else
            {
                Debug.LogWarning("?? CameraShake instance not found!");
            }
        }
    }
}





