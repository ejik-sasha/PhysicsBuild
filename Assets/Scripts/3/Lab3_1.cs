using UnityEngine;
using TMPro;

public class Lab3_1 : BaseLab
{
    public TMP_InputField AInput; 
    public TMP_InputField BInput; 
    public TMP_InputField t1Input; 
    public TMP_InputField t3Input;     
    public TMP_InputField v0Input; 
    public TMP_Text timeOutput;
    public TMP_Text distanceOutput;
    public TMP_Text speedOutput;
    public TMP_Text accelerationOutput;

    private float A, B, t1, t3, v0;
    private float startTime;
    private bool isMoving = false;
    private float totalDistance = 0f;
    private float lastPosition = 0f; 

    public override void ExecuteTask()
    {
        if (float.TryParse(AInput.text, out A) &&
            float.TryParse(BInput.text, out B) &&
            float.TryParse(t1Input.text, out t1) &&
            float.TryParse(t3Input.text, out t3) &&
            float.TryParse(v0Input.text, out v0))
        {
            startTime = Time.time;
            totalDistance = 0f;
            lastPosition = 0f; 

            float acceleration = A + B * t3;
            accelerationOutput.text = acceleration.ToString("F2");

            float speed = v0 + A * (t3 - t1) + 0.5f * B * (Mathf.Pow(t3, 2) - Mathf.Pow(t1, 2));
            speedOutput.text = speed.ToString("F2");

            isMoving = true;
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
            float distance;

            if (currentTime < t1)
            {
                distance = v0 * currentTime;
            }
            else
            {
                float relativeTime = currentTime - t1;
                distance = v0 * t1 + v0 * relativeTime + 0.5f * A * Mathf.Pow(relativeTime, 2) + (1f / 6f) * B * Mathf.Pow(relativeTime, 3);
            }

            distance = WrapPosition(distance);
            float deltaDistance = Mathf.Abs(distance - lastPosition);
            if (deltaDistance > 60)
            {
                deltaDistance = 0;
            }

            totalDistance += deltaDistance;
            lastPosition = distance;

            timeOutput.text = currentTime.ToString("F2");
            distanceOutput.text = totalDistance.ToString("F2");

            movingObject.transform.position = Vector3.right * distance;
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