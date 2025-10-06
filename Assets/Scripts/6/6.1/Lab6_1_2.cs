using UnityEngine;
using TMPro;

public class Lab6_1_2 : BaseLab
{
    public TMP_InputField massInput;
    public TMP_InputField velocityInput;
    public TMP_InputField angleInput;

    public TMP_Text timeOutput;
    public TMP_Text velocity1Output;
    public TMP_Text velocity2Output;

    public GameObject object2;
    public float size = 20f; 

    private float m, v, angleDeg;
    private float startTime;
    private bool hasCollided = false;
    private bool isRunning = false;

    private Vector3 obj1StartPos;
    private Vector3 obj2StartPos;

    private Vector2 velocityVec1, velocityVec2;

    private float m2 = 10f;

    public override void ExecuteTask()
    {
        if (float.TryParse(massInput.text, out m) &&
            float.TryParse(velocityInput.text, out v) &&
            float.TryParse(angleInput.text, out angleDeg))
        {
            m2 = m;
            ResetSimulation();

            float angleRad = (angleDeg * Mathf.Deg2Rad);
            velocityVec1 = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * v;
            velocityVec2 = Vector2.zero;

            obj1StartPos = movingObject.transform.position;
            obj2StartPos = object2.transform.position;

            hasCollided = false;
            isRunning = true;
            startTime = Time.time;
            
            UpdateVelocityDisplay();
        }
        else
        {
            Debug.LogError("Некорректный ввод.");
        }
    }

    private void ResetSimulation()
    {
        isRunning = false;
        hasCollided = false;

        movingObject.transform.position = new Vector3(-60f, 0f, 96f);
        object2.transform.position = new Vector3(60f, 0f, 96f);

        movingObject.transform.rotation = Quaternion.identity;
        object2.transform.rotation = Quaternion.identity;

        timeOutput.text = "0.00 с";
        velocity1Output.text = "---";
        velocity2Output.text = "---";
    }

    void FixedUpdate()
    {
        if (!isRunning) return;

        float t = Time.time - startTime;
        timeOutput.text = t.ToString("F2") + " с";

        Vector2 pos1 = new Vector2(movingObject.transform.position.x, movingObject.transform.position.y);
        Vector2 pos2 = new Vector2(object2.transform.position.x, object2.transform.position.y);

        if (!hasCollided)
        {
            pos1 = new Vector2(obj1StartPos.x, obj1StartPos.y) + velocityVec1 * t;
            movingObject.transform.position = new Vector3(pos1.x, pos1.y, 96f);

            float distance = Vector2.Distance(pos1, pos2);
            float minDist = size;

            if (distance <= minDist)
            {
                hasCollided = true;

                Vector2 normal = (pos2 - pos1).normalized;
                Vector2 tangent = new Vector2(-normal.y, normal.x);

                float v1n = Vector2.Dot(velocityVec1, normal);
                float v1t = Vector2.Dot(velocityVec1, tangent);
                float v2n = Vector2.Dot(velocityVec2, normal);
                float v2t = Vector2.Dot(velocityVec2, tangent);

                float v1nAfter = (v1n * (m - m2) + 2 * m2 * v2n) / (m + m2);
                float v2nAfter = (v2n * (m2 - m) + 2 * m * v1n) / (m + m2);

                velocityVec1 = v1nAfter * normal + v1t * tangent;
                velocityVec2 = v2nAfter * normal + v2t * tangent;

                obj1StartPos = movingObject.transform.position;
                obj2StartPos = object2.transform.position;
                startTime = Time.time;
                

                UpdateVelocityDisplay();
            }
        }
        else
        {
            float dt = Time.time - startTime;

            Vector2 newPos1 = new Vector2(obj1StartPos.x, obj1StartPos.y) + velocityVec1 * dt;
            Vector2 newPos2 = new Vector2(obj2StartPos.x, obj2StartPos.y) + velocityVec2 * dt;

            movingObject.transform.position = new Vector3(newPos1.x, newPos1.y, 96f);
            object2.transform.position = new Vector3(newPos2.x, newPos2.y, 96f);
        }
    }
    
    private void UpdateVelocityDisplay()
    {
        float speed1 = velocityVec1.magnitude;
        float speed2 = velocityVec2.magnitude;
        
        velocity1Output.text = speed1.ToString("F2") + " м/с";
        velocity2Output.text = speed2.ToString("F2") + " м/с";
    }
}