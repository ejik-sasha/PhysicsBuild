using UnityEngine;

public class Gear : MonoBehaviour
{
    public float angularSpeed;
    public bool clockwise = true;

    void Update()
    {
        float sign = clockwise ? -1f : 1f;
        transform.Rotate(Vector3.forward, angularSpeed * Mathf.Rad2Deg * Time.deltaTime * sign);
    }
}
