using UnityEngine;
using TMPro;

public class Lab1_4_2 : BaseLab
{
    public TMP_InputField speedXInput;
    public TMP_InputField speedYInput;
    public TMP_InputField startPositionXInput; 
    public TMP_InputField startPositionYInput;
    public TMP_Text positionOutput;
    public TMP_Text timeOutput;
    public TMP_Text distanceOutput;

    private float speedX;
    private float speedY;
    private float startTime;
    private float startPositionX;
    private float startPositionY;
    private bool isMoving = false;

    public override void ExecuteTask()
    {
        if (float.TryParse(speedXInput.text, out speedX) &&
            float.TryParse(speedYInput.text, out speedY) &&
            float.TryParse(startPositionXInput.text, out startPositionX) &&
            float.TryParse(startPositionYInput.text, out startPositionY))
        {
            startTime = Time.time;
            isMoving = true;

            movingObject.transform.position = new Vector3(startPositionX, startPositionY, 0);
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

            float distanceX = speedX * currentTime;
            float distanceY = speedY * currentTime;

            float currentPositionX = startPositionX + distanceX;
            float currentPositionY = startPositionY + distanceY;

            float totalDistance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

            timeOutput.text = currentTime.ToString("F2");
            positionOutput.text = currentPositionX.ToString("F2") + ", " + currentPositionY.ToString("F2");
            distanceOutput.text = totalDistance.ToString("F2");

            float newPositionX = currentPositionX % 120;
            if (newPositionX > 60)
            {
                newPositionX -= 120;
            }

            float newPositionY = currentPositionY % 120;
            if (newPositionY > 60)
            {
                newPositionY -= 120;
            }

            movingObject.transform.position = new Vector3(newPositionX, newPositionY, 0);
        }
    }
}