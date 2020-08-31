using UnityEngine;

/// <summary>
/// Rotator for game pick ups.
/// </summary>
public class Rotator : MonoBehaviour
{
    public void Update()
    {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }
}

