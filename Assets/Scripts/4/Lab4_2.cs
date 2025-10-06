using UnityEngine;
using TMPro;

public class Lab4_2 : BaseLab
{
    public TMP_InputField initialSpeedInput;
    public TMP_InputField accelerationInput; 
    public TMP_InputField radiusInput; 
    public TMP_InputField delayInput; 
    public TMP_InputField timeT1Input; 
    public TMP_InputField inclineAngleInput; // Ввод угла наклона
    public TMP_Text timeOutput;     
    public TMP_Text speedOutput; 
    public TMP_Text distanceOutput;
    public TMP_Text coordinatesOutput; 

    private float initialSpeed; 
    private float acceleration; 
    private float radius; 
    private float delay; 
    private float timeT1; 
    private float inclineAngle;
    private float startTime; 
    private bool isMoving = false; 
    private bool isDelayed = true; 
    private float totalDistance = 0f; 
    private Vector3 lastPosition; 

    public override void ExecuteTask()
    {
        if (float.TryParse(initialSpeedInput.text, out initialSpeed) &&
            float.TryParse(accelerationInput.text, out acceleration) &&
            float.TryParse(radiusInput.text, out radius) &&
            float.TryParse(delayInput.text, out delay) &&
            float.TryParse(timeT1Input.text, out timeT1) &&
            float.TryParse(inclineAngleInput.text, out inclineAngle))
        {
            startTime = Time.time; 
            totalDistance = 0f; 
            lastPosition = movingObject.transform.position; 
            isMoving = true; 
            isDelayed = true;
        } 
        else
        {
            Debug.LogError("Некорректный ввод данных!");
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            float currentTime = Time.time - startTime; 

            if (isDelayed && currentTime < delay)
            {
                timeOutput.text = currentTime.ToString("F2"); 
                distanceOutput.text = totalDistance.ToString("F2"); 
                return; 
            }

            if (isDelayed && currentTime >= delay)
            {
                isDelayed = false; 
                startTime = Time.time; 
                currentTime = 0;
            }

            float currentSpeed = initialSpeed + acceleration * currentTime;
            float angularVelocity = currentSpeed / radius;
            float angle = angularVelocity * currentTime;

            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            float y = (initialSpeed * currentTime + 0.5f * acceleration * currentTime * currentTime) * Mathf.Tan(inclineAngle * Mathf.Deg2Rad);

            x = WrapPosition(x);
            y = WrapPosition(y);
            z = WrapPosition(z);

            Vector3 newPosition = new Vector3(x, -y, z);

            totalDistance += Vector3.Distance(lastPosition, newPosition);
            lastPosition = newPosition;

            timeOutput.text = (currentTime + delay).ToString("F2"); 
            speedOutput.text = currentSpeed.ToString("F2"); 
            distanceOutput.text = totalDistance.ToString("F2"); 

            movingObject.transform.position = newPosition;

            if (currentTime >= timeT1)
            {
                float t1 = timeT1;
                float speedT1 = initialSpeed + acceleration * t1;
                float angleT1 = (speedT1 / radius) * t1;
                float xT1 = radius * Mathf.Cos(angleT1);
                float yT1 = radius * Mathf.Sin(angleT1);
                float zT1 = (initialSpeed * t1 + 0.5f * acceleration * t1 * t1) * Mathf.Tan(inclineAngle * Mathf.Deg2Rad);

                coordinatesOutput.text = $"({xT1:F2}, {yT1:F2}, {zT1:F2})";
            }
        }
    }

    private float WrapPosition(float position)
    {
        while (position > 60 || position < -60)
        {
            if (position > 60)
                position -= 120;
            else if (position < -60)
                position += 120;
        }
        return position;
    }
}