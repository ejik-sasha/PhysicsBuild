using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Lab5_3 : BaseLab
{
    public TMP_InputField accelerationInput; 
    public TMP_InputField angleInput;       
    public TMP_InputField heightInput;      

    public TMP_Text timeOutput;
    public TMP_Text distanceOutput;
    public TMP_Text averageSpeedOutput;
    public TMP_Text landingSpeedOutput;
    public TMP_Text totalPathOutput;

    public LineRenderer lineRenderer;

    private List<Vector3> trailPoints = new List<Vector3>();
    private Vector3 startPosition;

    private float A, angleDeg, h;
    private float g = 9.81f;

    private float ax, ay;

    private float timeOfFlight, distance, averageSpeed, landingSpeed, totalPath;

    private float startTime;
    private bool isFlying = false;

    private float lastX = 0;
    private float lastY = 0;
    private float accumulatedDistance = 0;
    
    private bool isSliding = false;
    private float fallEndedTime;
    private Vector3 landingPosition;


    public override void ExecuteTask()
    {
        if (float.TryParse(accelerationInput.text, out A) &&
            float.TryParse(angleInput.text, out angleDeg) &&
            float.TryParse(heightInput.text, out h))
        {
            float angleRad = angleDeg * Mathf.Deg2Rad;

            ax = A * Mathf.Cos(angleRad);
            ay = A * Mathf.Sin(angleRad);


            float a = 0.5f * (ay - g);
            float b = h;

            if (a >= 0)
            {
                Debug.LogError("Объект никогда не упадёт — ускорение слишком велико вверх.");
                return;
            }

            timeOfFlight = Mathf.Sqrt(-2 * b / (ay - g));


            distance = 0.5f * ax * timeOfFlight * timeOfFlight;


            float vx = ax * timeOfFlight;
            float vy = ay * timeOfFlight - g * timeOfFlight;
            landingSpeed = Mathf.Sqrt(vx * vx + vy * vy);

            averageSpeed = distance / timeOfFlight;

            startPosition = new Vector3(-90, 10, 96);
            startTime = Time.time;
            isFlying = true;

            lineRenderer.positionCount = 0;
            trailPoints.Clear();

            lastX = 0;
            lastY = h;
            accumulatedDistance = 0;



            landingSpeedOutput.text = landingSpeed.ToString("F2") + " м/с";
        }
        else
        {
            Debug.LogError("Некорректный ввод данных!");
        }
    }

    void Update()
{
    if (isFlying)
    {
        float t = Time.time - startTime;

        float angleRad = angleDeg * Mathf.Deg2Rad;
        float ax = A * Mathf.Cos(angleRad);
        float ay = A * Mathf.Sin(angleRad);
        float aYnet = ay - g;

        if (t > timeOfFlight)
        {
            isFlying = false;
            isSliding = true;
            fallEndedTime = Time.time;

            float x = 0.5f * ax * timeOfFlight * timeOfFlight;
            float y = h + 0.5f * aYnet * timeOfFlight * timeOfFlight;
            landingPosition = startPosition + new Vector3(x, y, 0);
            return;
        }

        float xFlight = 0.5f * ax * t * t;
        float yFlight = h + 0.5f * aYnet * t * t;

        Vector3 newPosition = startPosition + new Vector3(xFlight, yFlight, 0);
        movingObject.transform.position = newPosition;

        trailPoints.Add(newPosition);
        lineRenderer.positionCount = trailPoints.Count;
        lineRenderer.SetPositions(trailPoints.ToArray());

        timeOutput.text = t.ToString("F2") + " с";
        distanceOutput.text = xFlight.ToString("F2") + " м";
    }
    else if (isSliding)
    {
        float tSlide = Time.time - fallEndedTime;

        float angleRad = angleDeg * Mathf.Deg2Rad;
        float ax = A * Mathf.Cos(angleRad);

        float xSlide = 0.5f * ax * tSlide * tSlide;

        Vector3 newPosition = landingPosition + new Vector3(xSlide, 0, 0);
        movingObject.transform.position = newPosition;

        trailPoints.Add(newPosition);
        lineRenderer.positionCount = trailPoints.Count;
        lineRenderer.SetPositions(trailPoints.ToArray());

        timeOutput.text = (timeOfFlight + tSlide).ToString("F2") + " с";
        distanceOutput.text = (distance + xSlide).ToString("F2") + " м";
    }
}
}
