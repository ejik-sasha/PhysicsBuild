using UnityEngine;
using TMPro;

public class Lab2_2 : BaseLab
{
    public TMP_InputField frequencyInput;
    public TMP_InputField radiusInput;
    public TMP_Text timeOutput; 
    public TMP_Text angleOutput; 
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

            timeOutput.text = currentTime.ToString("F2");

            // Начальная угловая скорость: ω₀ = 2πν
            float angularVelocity = 2 * Mathf.PI * frequency;

            // Угол поворота: θ = ω₀ * t
            float angle = angularVelocity * currentTime ;

            angleOutput.text = angle.ToString("F2");

            movingObject.transform.RotateAround(centerPoint.position, Vector3.up, angularVelocity * Time.deltaTime * Mathf.Rad2Deg);
        }
    }
}