using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class Lab8_2_1 : BaseLab
{
    public TMP_InputField angleInput; 
    public TMP_Text resultText;
    public Transform targetA;        
    public LayerMask mirrorLayer;   
    public LayerMask obstacleLayer; 
    public int maxReflections = 20;
    public float maxDistance = 1000f;
    public bool useXZPlane = true; 
    public float epsilon = 0.01f; 
    public float targetRadius = 0.5f; 
    public bool drawDebugRays = false;
    public float a = 30f;

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
        if (
            float.TryParse(angleInput.text, out a)
        )
        { 
            movingObject.transform.rotation = Quaternion.Euler(a, -90f, 90f);
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
            Debug.LogWarning("Target not assigned");
            return;
        }

        Vector3 dir = DirectionFromAlpha(a, useXZPlane).normalized;

        Vector3 origin = movingObject.transform.position;
        List<Vector3> points = new List<Vector3> { origin };

        bool hitTarget = false;
        int reflections = 0;

        int combinedMask = mirrorLayer | obstacleLayer;

        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, maxDistance, combinedMask))
            {
                points.Add(hit.point);

                if (drawDebugRays) Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.green);

                if (targetA != null && hit.collider != null && hit.collider.transform == targetA)
                {
                    hitTarget = true;
                    reflections = i;
                    break;
                }

                if (targetA != null && hit.collider != targetA.GetComponent<Collider>())
                {
                    float distToTarget = Vector3.Distance(hit.point, targetA.position);
                    if (distToTarget <= targetRadius)
                    {
                        hitTarget = true;
                        reflections = i;
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

                    reflections = i + 1;
                    continue;
                }
                else
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
            Debug.Log($"Луч попал в точку A после {reflections} отражений.");
            lr.startColor = Color.yellow;
            lr.endColor = Color.yellow;
            resultText.text = "Прогноз: Попал в точку A!";
        }
        else
        {
            lr.startColor = Color.magenta;
            lr.endColor = Color.magenta;
        }

        if (drawDebugRays)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Debug.DrawLine(points[i], points[i + 1], Color.cyan);
            }
        }
    }

    Vector3 DirectionFromAlpha(float alphaDeg, bool xzPlane)
    {
        float a = alphaDeg * Mathf.Deg2Rad;
        if (xzPlane)
        {
            return new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
        }
        else
        {
            return new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f);
        }
    }
}
