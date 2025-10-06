using UnityEngine;
using TMPro;

public class Lab6_1_1 : BaseLab
{
    public TMP_InputField mass1Input;
    public TMP_InputField mass2Input;
    public TMP_InputField velocity1Input;

    public TMP_Text timeOutput;
    public TMP_Text velocity1AfterOutput;
    public TMP_Text velocity2AfterOutput;


    public GameObject object2;

    private float m1, m2, v1;
    private float v1After, v2After;

    private Vector3 object1StartPos;
    private Vector3 object2StartPos;

    private float startTime;
    private float collisionTime;
    private bool hasCollided = false;
    private bool isRunning = false;

    private const float objectSize = 20f;

    public override void ExecuteTask()
    {
        if (float.TryParse(mass1Input.text, out m1) &&
            float.TryParse(mass2Input.text, out m2) &&
            float.TryParse(velocity1Input.text, out v1))
        {
            ResetSimulation();

            object1StartPos = movingObject.transform.position;
            object2StartPos = object2.transform.position;

            float distance = object2StartPos.x - object1StartPos.x - objectSize;
            collisionTime = distance / v1;

            v1After = ((m1 - m2) * v1) / (m1 + m2);
            v2After = (2 * m1 * v1) / (m1 + m2);

            velocity1AfterOutput.text = v1After.ToString("F2") + " м/с";
            velocity2AfterOutput.text = v2After.ToString("F2") + " м/с";

            startTime = Time.time;
            isRunning = true;
            hasCollided = false;
        }
        else
        {
            Debug.LogError("Неверные входные данные.");
        }
    }

    private void ResetSimulation()
    {
        isRunning = false;
        hasCollided = false;

        movingObject.transform.position = new Vector3(-60f, 0f, 96f);
        object2.transform.position = new Vector3(60f, 0f, 96f);

        timeOutput.text = "0.00 с";
        velocity1AfterOutput.text = "---";
        velocity2AfterOutput.text = "---";
    }

    void FixedUpdate()
    {
        if (!isRunning) return;

        float currentTime = Time.time - startTime;
        timeOutput.text = currentTime.ToString("F2") + " с";

        if (!hasCollided && currentTime >= collisionTime)
        {
            object1StartPos = movingObject.transform.position;
            object2StartPos = object2.transform.position;

            startTime = Time.time;
            hasCollided = true;
        }

        float t = Time.time - startTime;

        if (!hasCollided)
        {
            movingObject.transform.position = object1StartPos + Vector3.right * (v1 * t);
            object2.transform.position = object2StartPos;
        }
        else
        {
            movingObject.transform.position = object1StartPos + Vector3.right * (v1After * t);
            object2.transform.position = object2StartPos + Vector3.right * (v2After * t);
        }
    }
    
    public void SetEqualMasses()
    {
        mass1Input.text = "10";
        mass2Input.text = "10";
        velocity1Input.text = "50";
    }

    public void SetM1MuchLess()
    {
        mass1Input.text = "1";
        mass2Input.text = "100";
        velocity1Input.text = "50";
    }

    public void SetM1MuchGreater()
    {
        mass1Input.text = "100";
        mass2Input.text = "1";
        velocity1Input.text = "50";
    }

}
