using UnityEngine;

/// <summary>
/// Camera controller.
/// </summary>
public class CameraController : MonoBehaviour
{
    public GameObject Ball;

    private Vector3 offset;

    public void Start()
    {
        this.offset = transform.position - this.Ball.transform.position;
    }

    public void LateUpdate()
    {
        this.transform.position = this.Ball.transform.position + this.offset;
    }
}
