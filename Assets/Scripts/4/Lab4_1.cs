using UnityEngine;
using TMPro;

public class Lab4_1 : BaseLab
{
    public TMP_InputField speedInput; 
    public TMP_InputField radiusInput; 
    public TMP_InputField delayInput; 
    public TMP_InputField inclineAngleInput;
    public TMP_Text timeOutput;
    public TMP_Text distanceOutput; 

    private float speed; 
    private float radius; 
    private float delay;     
    private float inclineAngle; 
    private float startTime;
    private bool isMoving = false;
    private bool isDelayed = true;
    private float totalDistance = 0f; 
    private Vector3 lastPosition; 

    public override void ExecuteTask()
    {
        if (float.TryParse(speedInput.text, out speed) &&
            float.TryParse(radiusInput.text, out radius) &&
            float.TryParse(delayInput.text, out delay) &&
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

            float angularVelocity = speed / radius;
            float angle = angularVelocity * currentTime;

            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            float y = speed * currentTime * Mathf.Tan(inclineAngle * Mathf.Deg2Rad); 

            x = WrapPosition(x);
            y = WrapPosition(y);
            z = WrapPosition(z);

            Vector3 newPosition = new Vector3(x, -y, z);

            totalDistance += Vector3.Distance(lastPosition, newPosition);
            lastPosition = newPosition;

            timeOutput.text = (currentTime + delay).ToString("F2"); 
            distanceOutput.text = totalDistance.ToString("F2"); 

            movingObject.transform.position = newPosition;
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