using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmberManager : MonoBehaviour
{
    public GameObject ember1;
    public GameObject ember2;
    public GameObject ember3;

    public Animator ember1Animator;
    public Animator ember2Animator;
    public Animator ember3Animator;

    void Start()
    {
        UpdateEmbers(GameManager.instance.GetCurrentLives());
    }

    public void UpdateEmbers(int livesLeft)
    {
        // If livesLeft is 2, Ember3 is dead
        if (livesLeft <= 2)
        {
            ember3Animator.SetTrigger("Die");
        }
        // If livesLeft is 1, Ember3 and Ember2 are dead
        if (livesLeft <= 1)
        {
            ember2Animator.SetTrigger("Die");
        }
        // If livesLeft is 0, all embers are dead
        if (livesLeft == 0)
        {
            ember1Animator.SetTrigger("Die");
        }
    }
}

