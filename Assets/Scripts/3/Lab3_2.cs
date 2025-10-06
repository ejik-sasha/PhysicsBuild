using UnityEngine;
using TMPro;

public class Lab3_2 : BaseLab
{
    public TMP_InputField AInput; 
    public TMP_InputField BInput; 
    public TMP_InputField CInput; 
    public TMP_InputField DInput; 
    public TMP_InputField t1Input; 
    public TMP_InputField t3Input;     
    public TMP_InputField v0xInput;
    public TMP_InputField v0yInput;
    public TMP_Text timeOutput;
    public TMP_Text distanceOutput;
    public TMP_Text speedOutput;
    public TMP_Text accelerationOutput;

    private float A, B, C, D, t1, t3, v0x, v0y;
    private float startTime;
    private bool isMoving = false;
    private float totalDistance = 0f;
    private Vector2 lastPosition = Vector2.zero;

    public override void ExecuteTask()
    {
        if (float.TryParse(AInput.text, out A) &&
            float.TryParse(BInput.text, out B) &&
            float.TryParse(CInput.text, out C) &&
            float.TryParse(DInput.text, out D) &&
            float.TryParse(t1Input.text, out t1) &&
            float.TryParse(t3Input.text, out t3) &&
            float.TryParse(v0xInput.text, out v0x) &&
            float.TryParse(v0yInput.text, out v0y))
        {
            startTime = Time.time;
            totalDistance = 0f;
            lastPosition = Vector2.zero;

            float ax_t3 = A + B * t3;
            float ay_t3 = C + D * t3;
            accelerationOutput.text = $"({ax_t3:F2}, {ay_t3:F2})";

            float vx_t3 = v0x + A * (t3 - t1) + 0.5f * B * (Mathf.Pow(t3, 2) - Mathf.Pow(t1, 2));
            float vy_t3 = v0y + C * (t3 - t1) + 0.5f * D * (Mathf.Pow(t3, 2) - Mathf.Pow(t1, 2));
            float v_t3 = Mathf.Sqrt(vx_t3 * vx_t3 + vy_t3 * vy_t3);
            speedOutput.text = v_t3.ToString("F2");

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
        float x, y;

        if (currentTime < t1)
        {
            x = v0x * currentTime;
            y = v0y * currentTime;
        }
        else
        {
            float relativeTime = currentTime - t1;

            x = v0x * t1 + v0x * relativeTime + 0.5f * A * Mathf.Pow(relativeTime, 2) + (1f / 6f) * B * Mathf.Pow(relativeTime, 3);
            y = v0y * t1 + v0y * relativeTime + 0.5f * C * Mathf.Pow(relativeTime, 2) + (1f / 6f) * D * Mathf.Pow(relativeTime, 3);
        }

        x = WrapPosition(x);
        y = WrapPosition(y);

        Vector2 newPosition = new Vector2(x, y);

        float deltaDistance = Vector2.Distance(newPosition, lastPosition);
        if (deltaDistance > 60)
        {
            deltaDistance = 0;
        }

        totalDistance += deltaDistance;
        lastPosition = newPosition;

        timeOutput.text = currentTime.ToString("F2");
        distanceOutput.text = totalDistance.ToString("F2");

        movingObject.transform.position = new Vector3(x, y, 0);
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