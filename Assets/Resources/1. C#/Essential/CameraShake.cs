using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("SHAKE SETTINGS")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.2f;

    private Transform cameraTransform;
    private Vector3 originalPosition;
    private bool isShaking;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void Start(){
        cameraTransform = Camera.main.transform;
        originalPosition = cameraTransform.localPosition;
    }

    void Update(){
        if(isShaking) originalPosition = cameraTransform.localPosition;
    }

    public void ShakeCamera(bool intense){
        if(intense) StartCoroutine(ShakeCoroutine(shakeDuration * 2f, shakeMagnitude * 2f));
        else StartCoroutine(ShakeCoroutine(shakeDuration, shakeMagnitude));
    }

    public void ShakeCamera(float duration, float magnitude){
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude){
        isShaking = true;
        float elapsed = 0f;

        while(elapsed < duration){
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cameraTransform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalPosition;
        isShaking = false;
        //Shake my booty
    }
}