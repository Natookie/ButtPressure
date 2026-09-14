using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocationController : MonoBehaviour
{
    public UnityEvent<string> LocationChanged;

    public static LocationController Instance { get; private set; }

    [Header("LOCATION SETTINGS")]
    [SerializeField] private List<Location> locationList = new List<Location>();

    private Dictionary<string, Location> locationDictionary = new Dictionary<string, Location>();
    private Location currentLocation;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy() => Instance = null;

    void Start()
    {
        // Assertion check
        Debug.Assert(locationList.Count > 0, "locationList is empty");
        // Initialize
        currentLocation = locationList[0];
        foreach (Location location in locationList)
        {
            locationDictionary.Add(location.locationName, location);
            if (location == currentLocation) location.ShowLocation();
            else location.HideLocation();
        }
    }
    #endregion

    // ====================================================================================================
    //                     Location Functions
    // ====================================================================================================
    #region Location
    public void ChangeLocation(string locationName)
    {
        if (!locationDictionary.TryGetValue(locationName, out Location newLocation))
        {
            Debug.LogWarning($"LocationController: No location found with name '{locationName}'");
            return;
        }
        ChangeLocation(newLocation);
    }

    public void ChangeLocation(Location location)
    {
        if (location == null)
        {
            Debug.LogWarning("LocationController: ChangeLocation called with a null location");
            return;
        }
        if (!locationDictionary.ContainsValue(location))
        {
            Debug.LogWarning($"LocationController: '{location.name}' is not in locationDictionary");
            return;
        }
        if (location == currentLocation) return;
        currentLocation.HideLocation();
        location.ShowLocation();
        currentLocation = location;
        LocationUI.Instance.SetLocation(location.locationDisplayName);
        LocationChanged?.Invoke(currentLocation.locationName);
    }
    #endregion
}