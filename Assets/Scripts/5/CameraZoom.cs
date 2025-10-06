using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform target;
    public float zoomSpeed = 2f;
    public float minDistance = -10f;
    public float maxDistance = -100f;

    public float xLimit = 100f;
    public float yLimit = 100f;

    public float followSpeed = 2f;

    private Vector3 offset;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Цель не назначена!");
            return;
        }

        offset = transform.position - target.position;
    }

    void Update()
    {
        if (!target) return;

        Vector3 targetPos = target.position;
        Vector3 camPos = transform.position;

       
        float dx = Mathf.Max(0, Mathf.Abs(targetPos.x) - xLimit);
        float dy = Mathf.Max(0, Mathf.Abs(targetPos.y) - yLimit);

        
        float overflow = Mathf.Max(dx, dy);

        
        float t = Mathf.Clamp01(overflow / 100f);  

        float targetZ = Mathf.Lerp(minDistance, maxDistance, t);
        float newZ = Mathf.Lerp(camPos.z, targetZ, Time.deltaTime * zoomSpeed);

        //float newX = Mathf.Lerp(camPos.x, targetPos.x + offset.x, Time.deltaTime * followSpeed);
        //float newY = Mathf.Lerp(camPos.y, targetPos.y + offset.y, Time.deltaTime * followSpeed);

        //transform.position = new Vector3(newX, newY, newZ);

        transform.position = new Vector3(camPos.x,camPos.y, newZ);
    }
}
