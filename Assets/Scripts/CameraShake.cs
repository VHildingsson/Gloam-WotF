using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.3f; // Increased for stronger effect

    public Transform camTransform;
    private Vector3 originalPos;
    private bool isShaking = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    public void ShakeCamera()
    {
        if (!isShaking) // Prevents multiple shakes overlapping
        {
            originalPos = camTransform.position; // ? Set originalPos dynamically!
            StartCoroutine(Shake());
        }
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            camTransform.position = originalPos + new Vector3(x, y, 0);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        camTransform.position = originalPos; // ? Reset to the correct position
        isShaking = false;
    }
}




