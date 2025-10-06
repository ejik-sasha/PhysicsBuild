using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class MovableObstacle
{
    public Transform transform;
    public bool isMovable = false;
    public bool isAccelerating = false;
    public float pushStrength = 200f;
    public float speedBoost = 2f;
    [HideInInspector] public Vector3 initialPosition;
    [HideInInspector] public Vector3 velocity = Vector3.zero;
}

public class Lab6_2_2 : BaseLab
{
    public TMP_InputField speedInput;
    public TMP_InputField angleInput;
    public TMP_Text resultText;
    public TMP_Text speedText;

    public Transform pointA;
    public LineRenderer trajectoryRenderer;
    public List<MovableObstacle> extendedObstacles;

    public float boundaryX = 150f;
    public float boundaryY = 70f;
    public float radius = 10f;
    public float epsilon = 20f;

    private Vector2 direction;
    private float speed;
    private bool isMoving = false;


    private void Start()
    {
        foreach (var ob in extendedObstacles)
            ob.initialPosition = ob.transform.position;
    }
    private void Update()
    {
        if (!isMoving || movingObject == null)
            return;

        Vector2 pos = new Vector2(movingObject.transform.position.x, movingObject.transform.position.y);
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

        foreach (MovableObstacle ob in extendedObstacles)
        {
            Transform obstacle = ob.transform;
            Vector2 obstaclePos = obstacle.position;
            float angleZ = obstacle.eulerAngles.z;
            Quaternion invRot = Quaternion.Inverse(Quaternion.Euler(0, 0, angleZ));
            Vector2 localPos = invRot * (pos - obstaclePos);
            Vector2 halfSize = obstacle.localScale * 0.5f;

            if (Mathf.Abs(localPos.x) <= halfSize.x + radius && Mathf.Abs(localPos.y) <= halfSize.y + radius)
            {
                Vector2 normalLocal;
                float dx = halfSize.x - Mathf.Abs(localPos.x);
                float dy = halfSize.y - Mathf.Abs(localPos.y);
                normalLocal = (dx < dy) ? new Vector2(Mathf.Sign(localPos.x), 0) : new Vector2(0, Mathf.Sign(localPos.y));
                Vector2 normalWorld = Quaternion.Euler(0, 0, angleZ) * normalLocal;

                if (ob.isAccelerating)
                {
                    speed *= ob.speedBoost;
                    speedText.text = speed.ToString("F2");
                }

                if (ob.isMovable)
                {
                    Debug.Log($"Obstacle before: {obstacle.position}");
                    Vector2 pushDir = (obstaclePos - pos).normalized; 
                    
                    obstacle.position += (Vector3)(pushDir * ob.pushStrength * Time.deltaTime);
                    Debug.Log($"Obstacle after: {obstacle.position}");
                    Debug.DrawRay(obstacle.position, normalWorld * 10f, Color.green);
                }
                if (ob.isMovable)
                {
                    Vector2 pushDir = (obstaclePos - pos).normalized; 
                    ob.velocity += (Vector3)(pushDir * ob.pushStrength * Time.deltaTime); 
                    
                    Debug.DrawRay(obstacle.position, pushDir * 10f, Color.green);
                }


                direction = Vector2.Reflect(direction, normalWorld);
                pos += normalWorld * (radius + 1f);
                break;
            }
        }

        movingObject.transform.position = new Vector3(pos.x, pos.y, movingObject.transform.position.z);

        Vector2 pointAPos = new Vector2(pointA.position.x, pointA.position.y);
        if (Vector2.Distance(pos, pointAPos) <= epsilon)
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
            resultText.text = "Прогноз:";
            speedText.text = speed.ToString("F2");

            DrawTrajectory();
        }
        else
        {
            resultText.text = "Ошибка ввода данных!";
        }
    }

    private void ResetSimulation()
    {
        movingObject.transform.position = new Vector3(-120f, -50f, 96f);
        movingObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        

        foreach (var ob in extendedObstacles)
            ob.transform.position = ob.initialPosition;
    }


    private void DrawTrajectory()
    {
        Vector2 pos = new Vector2(-120f, -50f);
        Vector2 dir = direction;
        float tempSpeed = speed;
        List<Vector3> points = new List<Vector3>();
        points.Add(new Vector3(pos.x, pos.y, movingObject.transform.position.z));

        float time = 0;
        float maxTime = 30f;
        float step = 0.1f;

        while (time < maxTime)
        {
            pos += dir * tempSpeed * step;

            if (pos.x < -boundaryX || pos.x > boundaryX)
            {
                dir.x *= -1;
                pos.x = Mathf.Clamp(pos.x, -boundaryX, boundaryX);
            }

            if (pos.y < -boundaryY || pos.y > boundaryY)
            {
                dir.y *= -1;
                pos.y = Mathf.Clamp(pos.y, -boundaryY, boundaryY);
            }

            foreach (MovableObstacle ob in extendedObstacles)
            {
                Transform obstacle = ob.transform;
                Vector2 obstaclePos = obstacle.position;
                float angleZ = obstacle.eulerAngles.z;
                Quaternion invRot = Quaternion.Inverse(Quaternion.Euler(0, 0, angleZ));
                Vector2 localPos = invRot * (pos - obstaclePos);
                Vector2 halfSize = obstacle.localScale * 0.5f;

                if (Mathf.Abs(localPos.x) <= halfSize.x + radius && Mathf.Abs(localPos.y) <= halfSize.y + radius)
                {
                    Vector2 normalLocal;
                    float dx = halfSize.x - Mathf.Abs(localPos.x);
                    float dy = halfSize.y - Mathf.Abs(localPos.y);
                    normalLocal = (dx < dy) ? new Vector2(Mathf.Sign(localPos.x), 0) : new Vector2(0, Mathf.Sign(localPos.y));
                    Vector2 normalWorld = Quaternion.Euler(0, 0, angleZ) * normalLocal;

                    if (ob.isAccelerating)
                        tempSpeed *= ob.speedBoost;

                    dir = Vector2.Reflect(dir, normalWorld);
                    pos += normalWorld * (radius + 1f);
                    break;
                }
            }

            points.Add(new Vector3(pos.x, pos.y, movingObject.transform.position.z));

            if (Vector2.Distance(pos, new Vector2(pointA.position.x, pointA.position.y)) <= epsilon)
            {
                resultText.text += "\nПопадание за " + time.ToString("F1") + " сек";
                break;
            }

            time += step;
        }

        trajectoryRenderer.positionCount = points.Count;
        trajectoryRenderer.SetPositions(points.ToArray());
    }
}
