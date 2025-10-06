using UnityEngine;
using TMPro;

public class Lab2_3 : BaseLab
{
    public TMP_InputField frequencyInput; 
    public TMP_InputField radiusInput;
    public TMP_Text timeOutput;
    public TMP_Text angleOutput; 
    public TMP_Text coordinateOutput; 
    public TMP_Text linearVelocityOutput; 
    public Transform centerPoint;

    private float frequency; 
    private float radius;
    private float startTime;
    private bool isMoving = false; 

    public override void ExecuteTask()
    {
        if (float.TryParse(frequencyInput.text, out frequency) &&
            float.TryParse(radiusInput.text, out radius))
        {
            startTime = Time.time;

            movingObject.transform.position = centerPoint.position + Vector3.right * radius;

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

            timeOutput.text = currentTime.ToString("F2") ;

            // Угловая скорость: ω₀ = 2πν
            float angularVelocity = 2 * Mathf.PI * frequency;

            // Угол поворота: θ = ω₀ * t 
            float angle = angularVelocity * currentTime + 0.5f;

            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            // Линейная скорость: v = ω * R
            float linearVelocity = angularVelocity * radius;

            angleOutput.text = angle.ToString("F2");
            coordinateOutput.text = x.ToString("F2") + ", " + y.ToString("F2");
            linearVelocityOutput.text =  linearVelocity.ToString("F2") ;

            movingObject.transform.position = centerPoint.position + new Vector3(x, y, 0);
        }
    }
}