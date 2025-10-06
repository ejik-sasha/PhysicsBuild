using UnityEngine;
using TMPro;

public class Lab1_3 : BaseLab
{
    public TMP_InputField speedInput;
    public TMP_InputField accelerationInput;
    public TMP_InputField startPositionInput;
    public TMP_Text positionOutput;
    public TMP_Text timeOutput;
    public TMP_Text distanceOutput;
    public TMP_Text speedOutput;

    private float initialSpeed;
    private float acceleration;
    private float startTime;
    private float startPosition;
    private bool isMoving = false;
    private float totalDistance = 0f;
    private float lastPosition = 0f; 
    public override void ExecuteTask()
    {
        if (float.TryParse(speedInput.text, out initialSpeed) &&
            float.TryParse(accelerationInput.text, out acceleration) &&
            float.TryParse(startPositionInput.text, out startPosition))
        {
            startTime = Time.time;
            isMoving = true;
            totalDistance = 0f;
            lastPosition = startPosition; 

            movingObject.transform.position = Vector3.right * startPosition;
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
            float currentSpeed = initialSpeed + acceleration * currentTime; 
            float distance = initialSpeed * currentTime + 0.5f * acceleration * currentTime * currentTime;
            float currentPosition = startPosition + distance; 

            while (currentPosition > 60 || currentPosition < -60)
            {
                if (currentPosition > 60)
                    currentPosition -= 120;
                else if (currentPosition < -60)
                    currentPosition += 120; 
            }

            totalDistance += Mathf.Abs(currentPosition - lastPosition);
            lastPosition = currentPosition;

            timeOutput.text = currentTime.ToString("F2");
            positionOutput.text = currentPosition.ToString("F2");
            distanceOutput.text = totalDistance.ToString("F2");
            speedOutput.text = currentSpeed.ToString("F2");

            movingObject.transform.position = Vector3.right * currentPosition;
        }
    }
}