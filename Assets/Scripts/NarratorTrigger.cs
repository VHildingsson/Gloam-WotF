using System.Collections;
using UnityEngine;
using TMPro;

public class NarratorTrigger : MonoBehaviour
{
    public TextMeshProUGUI displayText; // Reference to UI text
    public string message = "Default message."; // Custom text for each trigger
    public float typeSpeed = 0.05f; // Speed of typewriter effect

    private bool isTyping = false;
    private Coroutine typewriterCoroutine;

    void Start()
    {
        if (displayText != null)
        {
            displayText.gameObject.SetActive(false); // Start hidden
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTyping)
        {
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);

            displayText.gameObject.SetActive(true); // Ensure text is visible
            typewriterCoroutine = StartCoroutine(TypeText());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);

            // Run the coroutine on this script to prevent deactivation issues
            typewriterCoroutine = StartCoroutine(EraseText());
        }
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        displayText.text = "";

        foreach (char letter in message.ToCharArray())
        {
            displayText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    IEnumerator EraseText()
    {
        isTyping = true;

        while (displayText.text.Length > 0)
        {
            displayText.text = displayText.text.Substring(0, displayText.text.Length - 1);
            yield return new WaitForSeconds(typeSpeed);
        }

        // Ensure this happens AFTER erasing is complete
        displayText.gameObject.SetActive(false);

        isTyping = false;
    }
}


