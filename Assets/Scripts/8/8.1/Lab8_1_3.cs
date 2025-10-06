using UnityEngine;
using TMPro;

public class Lab8_1_3 : BaseLab
{
    public TMP_InputField angleInput;
    public TMP_InputField thicknessInput;
    public TMP_InputField n2Input;
    public GameObject cccube;

    public float maxDistance = 100f;

    public float thickness = 50f;
    public float n1 = 1f;
    public float n2 = 1.5f;
    public float a = 30f;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        transform.Rotate(0, -90, 0);

        ExecuteTask();
    }

    void Update()
    {
        Vector3 dir = -transform.up;
        Ray ray = new Ray(transform.position, dir);
        Debug.DrawRay(transform.position, dir * 1000f, Color.yellow);

        if (Physics.Raycast(ray, out RaycastHit hitIn, maxDistance) && n2 > 1)
        {
            Vector3 entryPoint = hitIn.point;
            Vector3 normalIn = hitIn.normal.normalized;
            Debug.DrawRay(entryPoint, normalIn * 30f, Color.green);

            Vector3 refractedDir = Refract(dir.normalized, normalIn, n1, n2);
            if (refractedDir == Vector3.zero) return;

            Vector3 oppositeStart = entryPoint + refractedDir * 100f;
            Vector3 oppositeDir = -refractedDir;

            if (Physics.Raycast(oppositeStart, oppositeDir, out RaycastHit hitOut, maxDistance))
            {
                if (hitOut.collider == hitIn.collider)
                {
                    Vector3 exitPoint = hitOut.point;
                    Vector3 normalOut = hitOut.normal.normalized;
                    Debug.DrawRay(exitPoint, normalOut * 30f, Color.blue);

                    Vector3 exitDir = Refract(refractedDir, normalOut, n2, n1);
                    exitDir = exitDir;
                    if (exitDir == Vector3.zero)
                    {
                        exitDir = Vector3.Reflect(refractedDir, normalOut);
                    }
                    if (Vector3.Dot(exitDir, Vector3.down) < 0)
                    {
                        exitDir = new Vector3(exitDir.x , -exitDir.y, 0f).normalized;

                    }

                    lineRenderer.positionCount = 4;
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, entryPoint);
                    lineRenderer.SetPosition(2, exitPoint);
                    lineRenderer.SetPosition(3, exitPoint + exitDir.normalized * 200f);
                }
            }
        }
        else
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + dir * 1000f);
        }
    }




    public override void ExecuteTask()
    {
        if (
            float.TryParse(thicknessInput.text, out thickness) &&
            float.TryParse(n2Input.text, out n2) &&
            float.TryParse(angleInput.text, out a)
        )
        {
            transform.Rotate(0, 90, a);
            cccube.transform.localScale = new Vector3(
                100f,
                thickness * 2f,
                100f
                
            );
        }
        else
        {
            Debug.LogWarning("Некорректный ввод!");
        }
    }
    Vector3 Refract(Vector3 I, Vector3 N, float n1, float n2)
    {
        float eta = n1 / n2;
        float cosi = -Vector3.Dot(N, I);
        float k = 1 - eta * eta * (1 - cosi * cosi);
        if (k < 0) return Vector3.zero;
        return eta * I + (eta * cosi - Mathf.Sqrt(k)) * N;
    }
}
