using UnityEngine;
using TMPro;

public class Lab7_2 : BaseLab
{
    [Header("Scene References")]
    public GameObject object1;
    public GameObject object2;
    public GameObject inclinedPlane;
    public Transform pulleyPoint;
    public LineRenderer ropeLine;

    [Header("UI Inputs")]
    public TMP_InputField mass1Input;
    public TMP_InputField mass2Input;
    public TMP_InputField addedMassInput;
    public TMP_InputField frictionInput;
    public TMP_InputField angleInput;
    public TMP_InputField momentT1Input;
    public TMP_Text resultText;
    public TMP_Text speedText;
    public TMP_Text timeText;

    private float m1, m2, mx, mu, thetaDeg, t1, startTime;
    private float g = 9.81f;

    private float velocity = 0f;
    private float elapsedTime = 0f;
    private bool massAdded = false;
    private bool isMoving = false;
    private bool moveRight = true;

    private Vector3 initialObject1Position = new Vector3(5f, 55f, 96f);
    private Vector3 initialObject2Position = new Vector3(-50f, 15f, 96f);
    private Vector3 planeSize = new Vector3(107f, 2f, 50f);
    private Vector3 planeCenter = new Vector3(18f, 43f, 96f);

    public override void ExecuteTask()
    {
        if (
            float.TryParse(mass1Input.text, out m1) &&
            float.TryParse(mass2Input.text, out m2) &&
            float.TryParse(addedMassInput.text, out mx) &&
            float.TryParse(frictionInput.text, out mu) &&
            float.TryParse(angleInput.text, out thetaDeg) &&
            float.TryParse(momentT1Input.text, out t1)
        )
        {
            ResetSimulation();


            elapsedTime = 0f;
            velocity = 0f;
            massAdded = false;
            isMoving = false;

            inclinedPlane.transform.rotation = Quaternion.identity;
            inclinedPlane.transform.RotateAround(pulleyPoint.position, Vector3.forward, thetaDeg);

            PositionObjectOnPlane();

            startTime = Time.time;

            float thetaRad = thetaDeg * Mathf.Deg2Rad;
            float F_friction = mu * m1 * g * Mathf.Cos(thetaRad);
            float F_m1 = m1 * g * Mathf.Sin(thetaRad);
            float minM2 = (F_m1 + F_friction) / g;
            resultText.text = $"Минимальная масса m2 для движения: {minM2:F2} кг";
        }
        else
        {
            resultText.text = "Ошибка ввода!";
        }
    }

    private void ResetSimulation()
    {
        object1.transform.position = initialObject1Position;
        object2.transform.position = initialObject2Position;
        inclinedPlane.transform.position = planeCenter;
        inclinedPlane.transform.rotation = Quaternion.identity;
        object1.GetComponent<Rigidbody>().useGravity = false;
        object1.GetComponent<Rigidbody>().isKinematic = true;
        object2.GetComponent<Rigidbody>().useGravity = false;
        object2.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void PositionObjectOnPlane()
    {
        Vector3 up = inclinedPlane.transform.up;
        Vector3 right = inclinedPlane.transform.right;
        Vector3 surfaceOffset = right * 50f;
        Vector3 verticalOffset = up * 12f;

        object1.transform.position = pulleyPoint.position + surfaceOffset + verticalOffset;
        object1.transform.rotation = inclinedPlane.transform.rotation;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Time.time - startTime;

        if (!massAdded && elapsedTime >= t1)
        {
            m2 += mx;
            massAdded = true;
            isMoving = CheckShouldMove();
        }

        timeText.text = t.ToString("F2");

        if (isMoving)
        {
            float thetaRad = thetaDeg * Mathf.Deg2Rad;
            float F_g1_parallel = m1 * g * Mathf.Sin(thetaRad);
            float F_friction = mu * m1 * g * Mathf.Cos(thetaRad);
            float F_g2 = m2 * g;

            float netForce = F_g1_parallel - F_g2;
            if (netForce > 0)
                netForce -= F_friction;
            else if (netForce < 0)
                netForce += F_friction;

            float a = netForce / (m1 + m2);
            velocity += a * Time.deltaTime;

            bool specialCase = (thetaDeg < 0) && (m1 > m2);

            Vector3 slideDir = inclinedPlane.transform.right;

            if (specialCase)
            {
                object1.transform.position -= slideDir * velocity * Time.deltaTime;
                if (object2.transform.position.y - 10 < pulleyPoint.position.y)
                {
                    object2.transform.position -= Vector3.up * velocity * Time.deltaTime;
                }
                else
                {
                    object2.transform.rotation = inclinedPlane.transform.rotation;
                    object2.transform.position -= slideDir * velocity * Time.deltaTime;
                }


                speedText.text = $" {velocity:F2} м/с";
            }
            else if ((thetaDeg > 0) && (m1 > m2))
            {
                object1.transform.position -= slideDir * velocity * Time.deltaTime;
                object2.transform.position += Vector3.down * velocity * Time.deltaTime;

                speedText.text = $" {velocity:F2} м/с";

            }
            else
            {

                object1.transform.position += slideDir * velocity * Time.deltaTime;
                object2.transform.position -= Vector3.down * velocity * Time.deltaTime;

                speedText.text = $" {velocity:F2} м/с";
            }


            float halfWidth = planeSize.x / 2f;
            float minX = planeCenter.x - halfWidth - 2;
            float maxX = planeCenter.x + halfWidth + 2;

            if (object1.transform.position.x + 7 < minX || object1.transform.position.x - 7 > maxX)
            {
                resultText.text += "\nОбъект 1 упал с плоскости!";

                //bject1.GetComponent<Rigidbody>().useGravity = true;
                //object1.GetComponent<Rigidbody>().isKinematic = false;
                //object2.GetComponent<Rigidbody>().useGravity = true;
                //object2.GetComponent<Rigidbody>().isKinematic = false;
                //isMoving = false;
                object1.transform.position -= Vector3.down * velocity * Time.deltaTime * 3 / 2;

            }
        }

        UpdateRope();
    }

    private bool CheckShouldMove()
{
    float thetaRad = thetaDeg * Mathf.Deg2Rad;
    float F_g1_parallel = m1 * g * Mathf.Sin(thetaRad);
    float F_friction = mu * m1 * g * Mathf.Cos(thetaRad);
    float F_g2 = m2 * g;


    return Mathf.Abs(F_g1_parallel - F_g2) > F_friction;
}
    private void UpdateRope()
    {
        if (object1.transform.position.y  > pulleyPoint.position.y &&
        object2.transform.position.y < pulleyPoint.position.y &&
        object1.transform.position.x  > pulleyPoint.position.x &&
        object2.transform.position.y < pulleyPoint.position.y)
        {
            ropeLine.positionCount = 3;
            ropeLine.SetPosition(0, object1.transform.position);
            ropeLine.SetPosition(1, pulleyPoint.position);
            ropeLine.SetPosition(2, object2.transform.position);

        }
        else if((thetaDeg < 0) &&
        object2.transform.position.x < pulleyPoint.position.x &&
        object1.transform.position.x  > pulleyPoint.position.x &&
        object2.transform.position.y < pulleyPoint.position.y)
        {
            ropeLine.positionCount = 3;
            ropeLine.SetPosition(0, object1.transform.position);
            ropeLine.SetPosition(1, pulleyPoint.position);
            ropeLine.SetPosition(2, object2.transform.position);

        }
        else
        {
            ropeLine.positionCount = 2;
            ropeLine.SetPosition(0, object1.transform.position);
            ropeLine.SetPosition(1, object2.transform.position);
            Debug.Log(pulleyPoint.position.y);
            Debug.Log(object1.transform.position.y + 10);
        }
    }

}
