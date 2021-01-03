using UnityEngine;

/// <summary> Useful functions will live here until I find a better place for them.</summary>
public class Utils : MonoBehaviour
{
    public static T InstantiateOffScreen<T>(T prefab) where T : Object
    {
        Vector3 offScreen = Camera.main.ViewportToWorldPoint(new Vector3(-1f, -1f, 0f));
        return Instantiate(prefab, offScreen, Quaternion.identity);
    }

    public static T InstantiateOffScreen<T>(T prefab, Vector3 position, Quaternion quaternion) where T : Object
    {
        Vector3 offScreen = Camera.main.ViewportToWorldPoint(new Vector3(-1f, -1f, position.z));
        return Instantiate(prefab, offScreen, quaternion);
    }
}
