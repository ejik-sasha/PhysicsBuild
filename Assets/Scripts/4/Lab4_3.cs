using UnityEngine;
using TMPro;

public class Lab4_3 : BaseLab
{
    public TMP_InputField initialSpeedInput;
    public TMP_InputField AInput;
    public TMP_InputField BInput;
    public TMP_InputField radiusInput;
    public TMP_InputField delayInput;
    public TMP_InputField timeT1Input;
    public TMP_InputField inclineAngleInput;
    public TMP_Text timeOutput;
    public TMP_Text distanceOutput;

    private float initialSpeed;
    private float A;
    private float B;
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
            float.TryParse(AInput.text, out A) &&
            float.TryParse(BInput.text, out B) &&
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

            float currentAcceleration = A + B * currentTime;
            float currentSpeed = initialSpeed + A * currentTime + 0.5f * B * currentTime * currentTime;
            float angularVelocity = currentSpeed / radius;
            float angle = angularVelocity * currentTime;

            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            float y = (initialSpeed * currentTime + 0.5f * A * currentTime * currentTime + (1f / 6f) * B * currentTime * currentTime * currentTime) * Mathf.Tan(inclineAngle * Mathf.Deg2Rad);

            x = WrapPosition(x);
            y = WrapPosition(y);
            z = WrapPosition(z);

            Vector3 newPosition = new Vector3(x, -y, z);

            totalDistance += Vector3.Distance(lastPosition, newPosition);
            lastPosition = newPosition;

            timeOutput.text = (currentTime + delay).ToString("F2");
            distanceOutput.text = totalDistance.ToString("F2");

            movingObject.transform.position = newPosition;

            if (currentTime >= timeT1)
            {
                float t1 = timeT1;
                float speedT1 = initialSpeed + A * t1 + 0.5f * B * t1 * t1;
                float angleT1 = (speedT1 / radius) * t1;
                float xT1 = radius * Mathf.Cos(angleT1);
                float zT1 = radius * Mathf.Sin(angleT1);
                float yT1 = (initialSpeed * t1 + 0.5f * A * t1 * t1 + (1f / 6f) * B * t1 * t1 * t1) * Mathf.Tan(inclineAngle * Mathf.Deg2Rad);

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