using UnityEngine;
using TMPro;

public class Lab7_1 : BaseLab
{
    public TMP_InputField massInput;
    public TMP_InputField frictionInput;
    public TMP_InputField angleInput;
    public TMP_InputField force0Input;
    public TMP_InputField forceRateInput;
    public TMP_Text resultText;
    public GameObject inclinedPlane;

    private float m, mu, thetaDeg, F0, A;
    private float g = 9.81f;

    private float startTime;
    private bool isMoving = false;
    private bool hasStarted = false;
    private bool hasResult = false;

    private Vector3 movementDirection;

    public override void ExecuteTask()
    {
        if (
            float.TryParse(massInput.text, out m) &&
            float.TryParse(frictionInput.text, out mu) &&
            float.TryParse(angleInput.text, out thetaDeg) &&
            float.TryParse(force0Input.text, out F0) &&
            float.TryParse(forceRateInput.text, out A)
        )
        {
            ResetSimulation();
            isMoving = false;
            hasStarted = true;
            hasResult = false;
            startTime = Time.time;
            resultText.text = "Ожидание начала движения...";

            float thetaRad = thetaDeg * Mathf.Deg2Rad;
            movementDirection = new Vector3(Mathf.Cos(thetaRad), Mathf.Sin(thetaRad), 0).normalized;

            if (inclinedPlane != null)
            {
                inclinedPlane.transform.rotation = Quaternion.Euler(0, 0, thetaDeg);
                movingObject.transform.rotation = Quaternion.Euler(0, 0, thetaDeg);
            }

            
            
            
        }
        else
        {
            resultText.text = "Некорректный ввод!";
        }
    }

    void Update()
    {
        if (!hasStarted || hasResult) return;

        float t = Time.time - startTime;
        float F = F0 + A * t;
        float thetaRad = thetaDeg * Mathf.Deg2Rad;

        float gravityComponent = m * g * Mathf.Sin(thetaRad);
        float frictionForce = mu * m * g * Mathf.Cos(thetaRad);

        if (!isMoving)
        {
            if (F + gravityComponent > frictionForce)
            {
                isMoving = true;
                string surfaceType = thetaDeg == 0 ? "горизонтали" : "наклонной";
                resultText.text = $"Объект начал движение по {surfaceType} при силе {F:F2} Н на {t:F2} с";
            }
            else if (t > 10f) 
            {
                hasResult = true;
                resultText.text = $"Объект не начал движение в течение 10 секунд.\nМакс сила: {F:F2} Н, нужно больше {frictionForce - gravityComponent:F2} Н";
            }
        }
        else
        {

            movingObject.transform.position += movementDirection * Time.deltaTime * 2f;
        }
    }
    private void ResetSimulation()
    {
        movingObject.transform.position = new Vector3(0f, 69f, 96f);
        resultText.text += "Прогноз...";
        movingObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        inclinedPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
