using UnityEngine;

public class Location : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private CameraBoundary cameraBoundary;

    [Header("LOCATION SETTINGS")]
    public string locationName;
    public string locationDisplayName;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void Start()
    {
        // Assertion check
        Debug.Assert(cameraBoundary, "cameraBoundary is missing");
        Debug.Assert(locationName != "", "locationName is empty");
        Debug.Assert(locationDisplayName != "", "locationDisplayName is empty");
    }
    #endregion

    // ====================================================================================================
    //                     Location Functions
    // ====================================================================================================
    #region Location
    public void ShowLocation()
    {
        gameObject.SetActive(true);
        CameraController.Instance.cameraBoundary = cameraBoundary;
    }

    public void HideLocation()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
