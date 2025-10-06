using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class Lab6_2_1 : BaseLab
{
    public TMP_InputField speedInput;
    public TMP_InputField angleInput;
    public TMP_Text resultText;

    public Transform pointA;
    public List<Transform> obstacles;

    public float boundaryX = 150f;
    public float boundaryY = 100f;
    public float radius = 10f;
    public float epsilon = 30f;
    public float simulationDuration = 30f;
    public float simulationStep = 0.05f;

    private Vector2 direction;
    private float speed;
    private bool isMoving = false;

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.widthMultiplier = 2f;
    }

    private void Update()
    {
        if (!isMoving || movingObject == null)
            return;

        Vector2 pos = movingObject.transform.position;
        pos += direction * speed * Time.deltaTime;


        if (pos.x < -boundaryX || pos.x > boundaryX)
        {
            direction.x *= -1;
            pos.x = Mathf.Clamp(pos.x, -boundaryX, boundaryX);
        }
        if (pos.y < -boundaryY || pos.y > boundaryY)
        {
            direction.y *= -1;
            pos.y = Mathf.Clamp(pos.y, -boundaryY, boundaryY);
        }


        foreach (Transform obstacle in obstacles)
        {
            Vector2 obstaclePos = obstacle.position;
            float angleZ = obstacle.eulerAngles.z;
            Quaternion invRot = Quaternion.Inverse(Quaternion.Euler(0, 0, angleZ));
            Vector2 localPos = invRot * (pos - obstaclePos);
            Vector2 halfSize = obstacle.localScale * 0.5f;

            if (Mathf.Abs(localPos.x) <= halfSize.x + radius && Mathf.Abs(localPos.y) <= halfSize.y + radius)
            {
                Vector2 normalLocal = Vector2.zero;
                float dx = halfSize.x - Mathf.Abs(localPos.x);
                float dy = halfSize.y - Mathf.Abs(localPos.y);
                normalLocal = (dx < dy) ? new Vector2(Mathf.Sign(localPos.x), 0) : new Vector2(0, Mathf.Sign(localPos.y));
                Vector2 normalWorld = Quaternion.Euler(0, 0, angleZ) * normalLocal;
                direction = Vector2.Reflect(direction, normalWorld.normalized);
                pos += normalWorld.normalized * (radius + 1f);
                break;
            }
        }

        movingObject.transform.position = new Vector3(pos.x, pos.y, movingObject.transform.position.z);


        if (Vector2.Distance(pos, pointA.position) <= epsilon)
        {
            isMoving = false;
            resultText.text = "Попал в точку A!";
        }
    }

    public override void ExecuteTask()
    {
        if (float.TryParse(speedInput.text, out speed) &&
            float.TryParse(angleInput.text, out float angleDeg))
        {
            ResetSimulation();
            float angleRad = angleDeg * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

            isMoving = true;
            resultText.text = "Прогноз...";

            SimulateTrajectory();
        }
        else
        {
            resultText.text = "Ошибка ввода данных!";
        }
    }

    private void ResetSimulation()
    {
        movingObject.transform.position = new Vector3(-120f, -50f, 96f);
        movingObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        lineRenderer.positionCount = 0;
    }

    private void SimulateTrajectory()
    {
        List<Vector3> pathPoints = new List<Vector3>();
        Vector2 simPos = new Vector2(-120f, -50f); 
        Vector2 simDir = direction;
        float t = 0f;
        bool hit = false;

        while (t < simulationDuration)
        {
            pathPoints.Add(new Vector3(simPos.x, simPos.y, movingObject.transform.position.z));
            simPos += simDir * speed * simulationStep;


            if (simPos.x < -boundaryX || simPos.x > boundaryX)
            {
                simDir.x *= -1;
                simPos.x = Mathf.Clamp(simPos.x, -boundaryX, boundaryX);
            }
            if (simPos.y < -boundaryY || simPos.y > boundaryY)
            {
                simDir.y *= -1;
                simPos.y = Mathf.Clamp(simPos.y, -boundaryY, boundaryY);
            }


            foreach (Transform obstacle in obstacles)
            {
                Vector2 obstaclePos = obstacle.position;
                float angleZ = obstacle.eulerAngles.z;
                Quaternion invRot = Quaternion.Inverse(Quaternion.Euler(0, 0, angleZ));
                Vector2 localPos = invRot * (simPos - obstaclePos);
                Vector2 halfSize = obstacle.localScale * 0.5f;

                if (Mathf.Abs(localPos.x) <= halfSize.x + radius && Mathf.Abs(localPos.y) <= halfSize.y + radius)
                {
                    Vector2 normalLocal = Vector2.zero;
                    float dx = halfSize.x - Mathf.Abs(localPos.x);
                    float dy = halfSize.y - Mathf.Abs(localPos.y);
                    normalLocal = (dx < dy) ? new Vector2(Mathf.Sign(localPos.x), 0) : new Vector2(0, Mathf.Sign(localPos.y));
                    Vector2 normalWorld = Quaternion.Euler(0, 0, angleZ) * normalLocal;
                    simDir = Vector2.Reflect(simDir, normalWorld.normalized);
                    simPos += normalWorld.normalized * (radius + 1f);
                    break;
                }
            }


            if (Vector2.Distance(simPos, pointA.position) <= epsilon)
            {
                pathPoints.Add(new Vector3(simPos.x, simPos.y, movingObject.transform.position.z));
                resultText.text = "Попадёт в точку A!";
                hit = true;
                break;
            }

            t += simulationStep;
        }

        if (!hit)
        {
            resultText.text = "Не попадёт в точку A за 30 секунд.";
        }


        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());
    }
}
