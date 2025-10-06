using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Lab5_2 : BaseLab
{
    public TMP_InputField accelerationInput;
    public TMP_InputField angleInput;       
    public TMP_InputField heightInput;     

    public TMP_Text timeOutput;
    public TMP_Text distanceOutput;
    public TMP_Text averageSpeedOutput;
    public TMP_Text landingSpeedOutput;

    public LineRenderer lineRenderer;
   

    private List<Vector3> trailPoints = new List<Vector3>();
    private Vector3 startPosition;

    private float A, angleDeg, h;
    private float V0x, V0y;
    private float timeOfFlight, distance, averageSpeed, landingSpeed, fullPath;
    private float g = 9.81f;

    private float startTime;
    private bool isFlying = false;

    public override void ExecuteTask()
    {
        if (float.TryParse(accelerationInput.text, out A) &&
            float.TryParse(angleInput.text, out angleDeg) &&
            float.TryParse(heightInput.text, out h))
        {
            float angleRad = angleDeg * Mathf.Deg2Rad;

            V0x = A * Mathf.Cos(angleRad);
            V0y = A * Mathf.Sin(angleRad);

            float a = 0.5f * g;
            float b = -V0y;
            float c = -h;

            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                Debug.LogError("Объект не достигнет земли (нет корней).");
                return;
            }

            float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            timeOfFlight = Mathf.Max(t1, t2); 

            distance = V0x * timeOfFlight;
            float vyFinal = V0y - g * timeOfFlight;
            landingSpeed = Mathf.Sqrt(V0x * V0x + vyFinal * vyFinal);
            fullPath = A * timeOfFlight;
            averageSpeed = fullPath / timeOfFlight;

            timeOutput.text = timeOfFlight.ToString("F2") + " с";
            distanceOutput.text = distance.ToString("F2") + " м";
            averageSpeedOutput.text = averageSpeed.ToString("F2") + " м/с";
            landingSpeedOutput.text = landingSpeed.ToString("F2") + " м/с";

            startPosition = new Vector3(-90, h, 96);
            movingObject.transform.position = startPosition;

            startTime = Time.time;
            isFlying = true;

            lineRenderer.positionCount = 0;
            trailPoints.Clear();
        }
        else
        {
            Debug.LogError("Ошибка ввода данных!");
        }
    }

    void Update()
    {
        if (!isFlying) return;

        float t = Time.time - startTime;

        if (t > timeOfFlight)
        {
            isFlying = false;
            return;
        }

        float x = V0x * t;
        float y = h + V0y * t - 0.5f * g * t * t;

        Vector3 newPosition = new Vector3(startPosition.x + x, y, startPosition.z);
        Debug.Log(startPosition.x);
        Debug.Log(y);
        Debug.Log(startPosition.z);
        movingObject.transform.position = newPosition;


        trailPoints.Add(newPosition);
        lineRenderer.positionCount = trailPoints.Count;
        lineRenderer.SetPositions(trailPoints.ToArray());


        timeOutput.text = t.ToString("F2") + " с";
        distanceOutput.text = x.ToString("F2") + " м";
    }

}
