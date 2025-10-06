using UnityEngine;
using TMPro;

public class Lab1_1 : BaseLab
{
    public TMP_InputField speedInput;
    public TMP_Text timeOutput;
    public TMP_Text distanceOutput;

    private float speed;
    private float startTime;
    private bool isMoving = false;
    private float totalDistance = 0f;
    private float lastPosition = 0f;

    public override void ExecuteTask()
    {
        if (float.TryParse(speedInput.text, out speed))
        {
            startTime = Time.time;
            totalDistance = 0f;
            lastPosition = 0f;
            isMoving = true;
        }
        else
        {
            Debug.LogError("Некорректный ввод скорости!");
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            float currentTime = Time.time - startTime;
            float distance = speed * currentTime;

            while (distance > 60 || distance < -60)
            {
                if (distance > 60)
                    distance -= 120;
                else if (distance < -60)
                    distance += 120;
            }

            totalDistance += Mathf.Abs(distance - lastPosition);
            lastPosition = distance;

            timeOutput.text = currentTime.ToString("F2");
            distanceOutput.text = totalDistance.ToString("F2");

            movingObject.transform.position = Vector3.right * distance;
        }
    }
}