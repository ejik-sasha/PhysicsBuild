using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class Lab8_1_2 : BaseLab
{
    public TMP_InputField angleInput;
    public TMP_InputField thicknessesInput;
    public TMP_InputField nValuesInput;
    public TMP_InputField countInput;
    public GameObject cubePrefab;

    public float maxDistance = 100f;

    public float thickness = 50f;
    public float nAir = 1f;
    public float n2 = 1.5f;
    public float a = 30f;

    private List<GameObject> cubes = new List<GameObject>();

    private LineRenderer lineRenderer;

    List<float> thicknesses = new List<float>();
    List<float> nValues = new List<float>();

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        transform.Rotate(0, -90, 0);

        ExecuteTask();

    }

    void Update()
    {
        Vector3 emitDir = -transform.up;
        Ray ray = new Ray(transform.position, emitDir);
        Debug.DrawRay(transform.position, emitDir * 1000f, Color.yellow);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            int hitIndex = FindCubeIndex(hit);
            if (hitIndex == -1)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, hit.point);
                return;
            }

            List<Vector3> points = new List<Vector3>();
            points.Add(transform.position);      
            points.Add(hit.point);   

            Vector3 currentPoint = hit.point;
            Vector3 currentDir = ray.direction.normalized; 
            float currentN = nAir;

            for (int i = hitIndex; i >= 0; i--)
            {
                GameObject cube = cubes[i];
                float nThis = nValues[i];
                float thickness = thicknesses[i];

                float topY = cube.transform.position.y + cube.transform.localScale.y * 0.5f;
                float bottomY = cube.transform.position.y - cube.transform.localScale.y * 0.5f;

                Vector3 topNormal = (i == hitIndex) ? hit.normal.normalized : Vector3.up;

                if (!Refract(currentDir, topNormal, currentN, nThis, out Vector3 dirInside))
                {
                    Vector3 reflected = Vector3.Reflect(currentDir, topNormal).normalized;
                    points.Add(currentPoint + reflected * 200f);
                    lineRenderer.positionCount = points.Count;
                    lineRenderer.SetPositions(points.ToArray());
                    return;
                }

                if (Mathf.Abs(dirInside.y) < 1e-6f)
                {
                    points.Add(currentPoint + dirInside.normalized * 200f);
                    lineRenderer.positionCount = points.Count;
                    lineRenderer.SetPositions(points.ToArray());
                    return;
                }

                float t = (bottomY - currentPoint.y) / dirInside.y;
                if (t < 0f)
                {
                    points.Add(currentPoint + dirInside.normalized * 200f);
                    lineRenderer.positionCount = points.Count;
                    lineRenderer.SetPositions(points.ToArray());
                    return;
                }

                Vector3 exitPoint = currentPoint + dirInside * t;
                points.Add(exitPoint); 

                float nextN = (i - 1 >= 0) ? nValues[i - 1] : nAir;
                Vector3 bottomNormal = Vector3.down;

                if (!Refract(dirInside, bottomNormal, nThis, nextN, out Vector3 dirAfter))
                {
                    Vector3 reflectedInside = Vector3.Reflect(dirInside, bottomNormal).normalized;
                    points.Add(exitPoint + reflectedInside * 200f);
                    lineRenderer.positionCount = points.Count;
                    lineRenderer.SetPositions(points.ToArray());
                    return;
                }

                currentPoint = exitPoint + dirAfter.normalized * 0.001f; 
                currentDir = dirAfter.normalized;
                currentN = nextN;

                if (i - 1 >= 0)
                {
                    points.Add(currentPoint);
                }
            }

            Vector3 finalDir = currentDir.normalized;

            if (finalDir.y > 0)
            {
                finalDir = new Vector3(finalDir.x, -finalDir.y, finalDir.z);
            }

            points.Add(currentPoint + finalDir * 200f);

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
            return;
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position - transform.up * 1000f);
    }

    int FindCubeIndex(RaycastHit hit)
    {
        for (int i = 0; i < cubes.Count; i++)
        {
            if (hit.collider.transform == cubes[i].transform || hit.collider.transform.IsChildOf(cubes[i].transform))
                return i;
        }
        return -1;
    }

    bool Refract(Vector3 I, Vector3 N, float nFrom, float nTo, out Vector3 T)
    {
        I = I.normalized;
        N = N.normalized;
        float eta = nFrom / nTo;
        float cosi = Mathf.Clamp(Vector3.Dot(-I, N), -1f, 1f);
        float k = 1f - eta * eta * (1f - cosi * cosi);
        if (k < 0f)
        {
            T = Vector3.zero;
            return false; 
        }
        T = (eta * I) + (eta * cosi - Mathf.Sqrt(k)) * N;
        T = T.normalized;
        return true;
    }


    public override void ExecuteTask()
    {
        foreach (var c in cubes)
            Destroy(c);
        cubes.Clear();
        thicknesses.Clear();
        nValues.Clear();

        if (!float.TryParse(angleInput.text, out a))
        {
            Debug.LogWarning("Некорректный угол!");
            return;
        }
        transform.Rotate(0, 90, a);

        if (!int.TryParse(countInput.text, out int count) || count <= 0)
        {
            Debug.LogWarning("Введите корректное количество материалов!");
            return;
        }

        string[] thicknessStr = thicknessesInput.text.Split(',');
        string[] nStr = nValuesInput.text.Split(',');

        if (thicknessStr.Length != count || nStr.Length != count)
        {
            Debug.LogWarning("Количество параметров не совпадает с количеством материалов!");
            return;
        }



        for (int i = 0; i < count; i++)
        {
            if (float.TryParse(thicknessStr[i], out float t) && float.TryParse(nStr[i], out float n))
            {
                thicknesses.Add(t);
                nValues.Add(n);
            }
            else
            {
                Debug.LogWarning($"Ошибка в параметрах материала №{i + 1}");
                return;
            }
        }

        float zOffset = 0f;
        for (int i = 0; i < count; i++)
        {
            GameObject cube = Instantiate(cubePrefab);
            cube.transform.position = new Vector3(0, (zOffset + thicknesses[i] / 2f) + 30f, 50);
            cube.transform.localScale = new Vector3(250f, thicknesses[i], 55f);
            cube.name = $"Material_{i + 1}_n{nValues[i]}";

            float hue = (float)i / count;
            Color color = Color.HSVToRGB(hue, 0.8f, 0.8f);
            color.a = 0.5f;
            cube.GetComponent<Renderer>().material.color = color;
            cubes.Add(cube);

            zOffset += thicknesses[i];
        }

        Debug.Log($"Создано {count} материалов.");
    }

    void DrawRayPath()
    {
        if (cubes.Count == 0) return;

        List<Vector3> points = new List<Vector3>();
        Vector3 startPos = transform.position;
        points.Add(startPos);

        float currentN = nAir;
        float sign = Mathf.Sign(a);

        float incidentRad = Mathf.Abs(a) * Mathf.Deg2Rad;
        Vector3 rayDir = new Vector3(Mathf.Sin(incidentRad) * sign, 0, -Mathf.Cos(incidentRad));

        Vector3 pos = startPos;

        for (int i = 0; i < cubes.Count; i++)
        {
            float nextN = nValues[i];
            float thickness = thicknesses[i];

            float incidentAngle = Mathf.Abs(Vector3.Angle(rayDir, Vector3.back)) * Mathf.Deg2Rad;

            float refractedAngle = Mathf.Asin(currentN * Mathf.Sin(incidentAngle) / nextN) * sign;

            Vector3 tangent = Vector3.right;
            Vector3 refractedDir = new Vector3(Mathf.Sin(refractedAngle), 0, -Mathf.Cos(refractedAngle));

            Vector3 exitPoint = pos + refractedDir.normalized * thickness;

            points.Add(exitPoint);

            pos = exitPoint;
            rayDir = refractedDir;
            currentN = nextN;
        }

        Vector3 finalDir = rayDir.normalized;
        points.Add(pos + finalDir * 50f);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

}
