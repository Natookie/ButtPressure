using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("SHAKE SETTINGS")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.2f;

    private Vector3 originalPosition;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void Start(){
        if (targetTransform) originalPosition = targetTransform.localPosition;
    }

    public void ShakeCamera(bool intense){
        if(intense) StartCoroutine(ShakeCoroutine(shakeDuration * 2f, shakeMagnitude * 2f));
        else StartCoroutine(ShakeCoroutine(shakeDuration, shakeMagnitude));
    }

    public void ShakeCamera(float duration, float magnitude){
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude){
        Vector3 startPosition = targetTransform.localPosition;
        float elapsed = 0f;

        while(elapsed < duration){
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            targetTransform.localPosition = startPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetTransform.localPosition = startPosition;
    }
}