using UnityEngine;
using TMPro;

public class Lab2_1 : BaseLab
{
    public TMP_InputField radiusInput;
    public TMP_InputField frequencyInput;
    public TMP_Text timeOutput;
    public TMP_Text revolutionsOutput;
    public TMP_Text angularVelocityOutput;
    public Transform centerPoint;

    private float radius;
    private float frequency;
    private float startTime;
    private float angularVelocity;
    private float totalAngle;
    private bool isMoving = false;

    public override void ExecuteTask()
    {
        if (float.TryParse(frequencyInput.text, out frequency) &&
            float.TryParse(radiusInput.text, out radius))
        {
            // Начальная угловая скорость: ω₀ = 2πν
            angularVelocity = 2 * Mathf.PI * frequency;

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

            // Cуммарный угол поворота: θ = ω * Δt
            totalAngle = angularVelocity * Time.fixedDeltaTime;

            movingObject.transform.RotateAround(centerPoint.position, Vector3.up, totalAngle * Mathf.Rad2Deg);

            // Число оборотов: N = ν * t + 0.5 * α * t² / (2π)
            float revolutions = frequency * currentTime + 0.5f ;

            revolutionsOutput.text = revolutions.ToString("F2");
            angularVelocityOutput.text = angularVelocity.ToString("F2");
        }
    }
}