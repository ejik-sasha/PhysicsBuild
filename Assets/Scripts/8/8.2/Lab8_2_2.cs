using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class Lab8_2_2 : BaseLab
{
    public TMP_InputField angleInput;
    public TMP_InputField thicknessInput;
    public TMP_InputField n2Input;
    public TMP_Text resultText;
    public Transform targetA;

    public LayerMask mirrorLayer;   
    public LayerMask lensLayer;    
    public LayerMask obstacleLayer; 
    public GameObject[] lens;

    public int maxSteps = 30;
    public float maxDistance = 1000f;
    public bool useXZPlane = true;
    public float epsilon = 0.01f;
    public float targetRadius = 10f;
    public bool drawDebugRays = false;
    public float a = 30f;

    [Header("Lens parameters")]
    public float lensRefrIndex = 1.5f; 
    public float airRefrIndex = 1.0f;  
    public float lensThickness = 0.5f; 
    private float k = 1f;

    LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 1f;
        lr.endWidth = 1f;
        lr.positionCount = 0;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        movingObject.transform.rotation = Quaternion.Euler(0f, -90f, 90f);
    }

    void Update()
    {
        TraceAndDraw();
    }

    public override void ExecuteTask()
    {
        if (float.TryParse(angleInput.text, out a) &&
        float.TryParse(thicknessInput.text, out lensThickness) &&
        float.TryParse(n2Input.text, out lensRefrIndex))
        {
            movingObject.transform.rotation = Quaternion.Euler(a, -90f, 90f);
            foreach (var l in lens)
            {
                l.transform.localScale = new Vector3(
                l.transform.localScale.x / k * lensThickness,
                l.transform.localScale.y,
                l.transform.localScale.z);
            }
            k = lensThickness;
            
        }
        else
        {
            Debug.LogWarning("Некорректный ввод!");
        }
    }

    void TraceAndDraw()
    {
        if (movingObject == null)
        {
            Debug.LogWarning("Emitter not assigned");
            return;
        }

        Vector3 dir = DirectionFromAlpha(a, useXZPlane).normalized;
        Vector3 origin = movingObject.transform.position;

        List<Vector3> points = new List<Vector3> { origin };
        bool hitTarget = false;

        for (int i = 0; i < maxSteps; i++)
        {
            int combinedMask = mirrorLayer | lensLayer | obstacleLayer;

            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, maxDistance, combinedMask))
            {
                points.Add(hit.point);

                if (drawDebugRays) Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.green);

                if (targetA != null)
                {
                    float distToTarget = Vector3.Distance(hit.point, targetA.position);
                    if (distToTarget <= targetRadius)
                    {
                        hitTarget = true;
                        break;
                    }
                }

                bool isMirror = ((mirrorLayer.value & (1 << hit.collider.gameObject.layer)) != 0);
                if (isMirror)
                {
                    Vector3 normal = hit.normal;
                    if (Vector3.Dot(dir, normal) > 0f)
                        normal = -normal;

                    dir = Vector3.Reflect(dir, normal).normalized;
                    origin = hit.point + dir * epsilon;
                    continue;
                }

                bool isLens = ((lensLayer.value & (1 << hit.collider.gameObject.layer)) != 0);
                if (isLens)
                {
                    Vector3 normal = hit.normal;
                    if (Vector3.Dot(dir, normal) > 0f)
                        normal = -normal;

                    dir = Refract(dir, normal, airRefrIndex, lensRefrIndex).normalized;

                    Vector3 insideOrigin = hit.point + dir * epsilon;
                    RaycastHit exitHit;
                    if (hit.collider.Raycast(new Ray(insideOrigin, dir), out exitHit, lensThickness * 2f))
                    {
                        points.Add(exitHit.point);

                        Vector3 exitNormal = exitHit.normal;
                        if (Vector3.Dot(dir, exitNormal) > 0f)
                            exitNormal = -exitNormal;

                        dir = Refract(dir, exitNormal, lensRefrIndex, airRefrIndex).normalized;
                        origin = exitHit.point + dir * epsilon;
                        continue;
                    }
                }

                bool isObstacle = ((obstacleLayer.value & (1 << hit.collider.gameObject.layer)) != 0);
                if (isObstacle)
                {
                    break;
                }
            }
            else
            {
                points.Add(origin + dir * maxDistance);
                break;
            }
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());

        if (hitTarget)
        {
            lr.startColor = Color.yellow;
            lr.endColor = Color.yellow;
            resultText.text = "Прогноз: Попал в точку A!";
        }
        else
        {
            lr.startColor = Color.magenta;
            lr.endColor = Color.magenta;
            resultText.text = "Прогноз: Не попал.";
        }

        if (drawDebugRays)
        {
            for (int i = 0; i < points.Count - 1; i++)
                Debug.DrawLine(points[i], points[i + 1], Color.cyan);
        }
    }

    Vector3 DirectionFromAlpha(float alphaDeg, bool xzPlane)
    {
        float a = alphaDeg * Mathf.Deg2Rad;
        if (xzPlane)
            return new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
        else
            return new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f);
    }

    Vector3 Refract(Vector3 incident, Vector3 normal, float n1, float n2)
    {
        incident.Normalize();
        normal.Normalize();

        float ratio = n1 / n2;
        float cosI = -Vector3.Dot(normal, incident);
        float sinT2 = ratio * ratio * (1f - cosI * cosI);

        if (sinT2 > 1f)
            return Vector3.Reflect(incident, normal);

        float cosT = Mathf.Sqrt(1f - sinT2);
        return ratio * incident + (ratio * cosI - cosT) * normal;
    }
}
