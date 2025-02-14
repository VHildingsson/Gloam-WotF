using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredCreatureTrigger : MonoBehaviour
{
    private Animator anim;
    private bool isScared = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (anim == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isScared)
        {
            isScared = true;
            anim.SetTrigger("Scared");
            StartCoroutine(SetScaredIdleAfterDelay()); // Ensure transition happens
        }
    }

    // ?? Ensures transition to "ScaredIdle" happens with a delay
    private IEnumerator SetScaredIdleAfterDelay()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length); // Waits for Scared animation to finish
        anim.SetBool("isCalm", true); // Transitions to "ScaredIdle"
    }
}
