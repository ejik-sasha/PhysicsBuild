using UnityEngine;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Lab5_1 : BaseLab
{
    public TMP_InputField v0Input;
    public TMP_InputField angleInput;
    public TMP_InputField heightInput;

    public LineRenderer lineRenderer;
    private List<Vector3> trailPoints = new List<Vector3>();
    

    public TMP_Text timeOutput;
    public TMP_Text distanceOutput;
    private Vector3 startPosition;

    private float v0, angleDeg, h;
    private float timeOfFlight, distance;
    private float g = 9.81f;

    private float startTime;
    private bool isFlying = false;

    public override void ExecuteTask()
    {
        if (float.TryParse(v0Input.text, out v0) &&
            float.TryParse(angleInput.text, out angleDeg) &&
            float.TryParse(heightInput.text, out h))
        {
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float v0x = v0 * Mathf.Cos(angleRad);
            float v0y = v0 * Mathf.Sin(angleRad);

   
            float discriminant = v0y * v0y + 2 * g * h;
            timeOfFlight = (v0y + Mathf.Sqrt(discriminant)) / g;


            distance = v0x * timeOfFlight;

            //timeOutput.text = timeOfFlight.ToString("F2");
            //distanceOutput.text = distance.ToString("F2");

            startPosition = new Vector3(-90,10,96);;
            startTime = Time.time;
            isFlying = true;
        }
        else
        {
            Debug.LogError("Некорректный ввод данных!");
        }
    }

    void Start()
    {
    lineRenderer.positionCount = 0;
    trailPoints.Clear();
    }

    void Update()
    {
        if (isFlying)
        {
            float t = Time.time - startTime;


            if (t > timeOfFlight)
            {
                isFlying = false;
                return;
            }

            float angleRad = angleDeg * Mathf.Deg2Rad;
            float v0x = v0 * Mathf.Cos(angleRad);
            float v0y = v0 * Mathf.Sin(angleRad);

            float x = v0x * t;
            float y = h + v0y * t - 0.5f * g * t * t;

            movingObject.transform.position = startPosition + new Vector3(x, y, 0);


            timeOutput.text = t.ToString("F2");
            distanceOutput.text = Mathf.Abs(x).ToString("F2");

            trailPoints.Add(movingObject.transform.position);
            lineRenderer.positionCount = trailPoints.Count;
            lineRenderer.SetPositions(trailPoints.ToArray());
        }
        
    }
}
