using UnityEngine;
using TMPro;

public class Lab1_5_3 : BaseLab
{
    public TMP_InputField speedXInput; 
    public TMP_InputField speedYInput; 
    public TMP_InputField speedZInput;
    public TMP_InputField accelerationXInput; 
    public TMP_InputField accelerationYInput; 
    public TMP_InputField accelerationZInput; 
    public TMP_InputField startPositionXInput;
    public TMP_InputField startPositionYInput; 
    public TMP_InputField startPositionZInput; 
    public TMP_Text positionOutput;
    public TMP_Text timeOutput;
    public TMP_Text distanceOutput;
    public TMP_Text speedOutput;

    private float initialSpeedX;
    private float initialSpeedY;
    private float initialSpeedZ;
    private float accelerationX;
    private float accelerationY;
    private float accelerationZ;
    private float startTime;
    private float startPositionX;
    private float startPositionY;
    private float startPositionZ;
    private bool isMoving = false;

    public override void ExecuteTask()
    {
        if (float.TryParse(speedXInput.text, out initialSpeedX) &&
            float.TryParse(speedYInput.text, out initialSpeedY) &&
            float.TryParse(speedZInput.text, out initialSpeedZ) &&
            float.TryParse(accelerationXInput.text, out accelerationX) &&
            float.TryParse(accelerationYInput.text, out accelerationY) &&
            float.TryParse(accelerationZInput.text, out accelerationZ) &&
            float.TryParse(startPositionXInput.text, out startPositionX) &&
            float.TryParse(startPositionYInput.text, out startPositionY) &&
            float.TryParse(startPositionZInput.text, out startPositionZ))
        {
            startTime = Time.time;
            isMoving = true;

            movingObject.transform.position = new Vector3(startPositionX, startPositionY, startPositionZ);
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

            float currentSpeedX = initialSpeedX + accelerationX * currentTime;
            float currentSpeedY = initialSpeedY + accelerationY * currentTime;
            float currentSpeedZ = initialSpeedZ + accelerationZ * currentTime;

            float distanceX = initialSpeedX * currentTime + 0.5f * accelerationX * currentTime * currentTime;
            float distanceY = initialSpeedY * currentTime + 0.5f * accelerationY * currentTime * currentTime;
            float distanceZ = initialSpeedZ * currentTime + 0.5f * accelerationZ * currentTime * currentTime;

            float currentPositionX = startPositionX + distanceX;
            float currentPositionY = startPositionY + distanceY;
            float currentPositionZ = startPositionZ + distanceZ;

            float totalDistance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY + distanceZ * distanceZ);

            timeOutput.text = currentTime.ToString("F2");
            positionOutput.text =  currentPositionX.ToString("F2") + ", " + currentPositionY.ToString("F2") + ", " + currentPositionZ.ToString("F2");
            distanceOutput.text = totalDistance.ToString("F2") ;
            speedOutput.text = currentSpeedX.ToString("F2") + ", " + currentSpeedY.ToString("F2") + ", " + currentSpeedZ.ToString("F2") ;

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

            float newPositionZ = currentPositionZ % 12000;
            if (newPositionZ > 6000)
            {
                newPositionZ -= 12000;
            }

            movingObject.transform.position = new Vector3(newPositionX, newPositionY, newPositionZ);
        }
    }
}