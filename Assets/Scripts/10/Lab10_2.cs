using UnityEngine;
using TMPro;

public class Lab10_2 : BaseLab
{
    [Header("UI Elements")]
    public TMP_InputField rho1Input;  
    public TMP_InputField rho2Input;  
    public TMP_InputField VInput;    
    public TMP_Text resultText;

    [Header("Scene Objects")]
    public Transform body;       
    public Transform liquidLevel; 
    public Transform bottomLevel; 

    private float rho1, rho2, V, Cd;
    private const float g = 9.81f;
    private float velocity = 0f;
    private bool simulate = false;

    private float m; 
    private float A; 

    public override void ExecuteTask()
    {
        if (!float.TryParse(rho1Input.text, out rho1) ||
            !float.TryParse(rho2Input.text, out rho2) ||
            !float.TryParse(VInput.text, out V))
        {
            resultText.text = "Ошибка: некорректные входные данные.";
            return;
        }
        Cd = 1.05f;
        transform.position = new Vector3(0f, -30f, 141f);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        m = rho2 * V;

        A = Mathf.Pow(V, 2f / 3f);

        velocity = 0f;
        simulate = true;

        float staticNet = (rho1 - rho2) * V * g;
        if (Mathf.Approximately(staticNet, 0f))
        {
            resultText.text = "Тело будет находиться в равновесии (ρ1 ≈ ρ2).";
            simulate = false;
        }
        else if (staticNet > 0)
        {
            resultText.text = "Тело всплывает...";
        }
        else
        {
            resultText.text = "Тело тонет...";
        }
    }

    void Update()
{
    if (!simulate || body == null) return;

    float depth = liquidLevel.position.y - body.position.y;
    float Vsub = Mathf.Clamp(depth, 0f, V); 

    float F_A = rho1 * Vsub * g;     
    float F_g = rho2 * V * g;        
    float F_d = 0f;                  
    if (body.position.y < liquidLevel.position.y)
    {
        F_d = 0.5f * Cd * rho1 * A * velocity * Mathf.Abs(velocity) * -Mathf.Sign(velocity);
    }

    float F_net = F_A - F_g + F_d;
    float a = F_net / m;

    velocity += a * Time.deltaTime;
    Vector3 pos = body.position;
    pos.y += velocity * Time.deltaTime;
    body.position = pos;

    resultText.text = $"F_A={F_A:F1} Н, F_g={F_g:F1} Н, F_d={F_d:F1} Н\n" +
                      $"a={a:F2} м/с², v={velocity:F2} м/с";

    if (rho2 < rho1 && body.position.y + 1 >= liquidLevel.position.y)
    {
        resultText.text += "\nТело всплыло на поверхность.";
    }
    if (body.position.y >= liquidLevel.position.y)
    {
        simulate = false;
    }

        if (rho2 > rho1 && body.position.y <= bottomLevel.position.y)
        {
            body.position = new Vector3(body.position.x, bottomLevel.position.y, body.position.z);
            simulate = false;
            resultText.text += "\nТело утонуло до дна.";
        }
}

}
