using TMPro;
using UnityEngine;

public class Lab10_3 : BaseLab
{
    [Header("UI Elements (TextMeshPro)")]
    public TMP_InputField fluidDensityInput;   
    public TMP_InputField volumeInput;
    public TMP_InputField speedInput;
    public TextMeshProUGUI resultText;

    [Header("Simulation Settings")]
    [Tooltip("Кривая изменения плотности объекта во времени (ρ2 в кг/м³): ось X — время, ось Y — плотность")]
    public AnimationCurve densityCurve;
    [Tooltip("Y-координата поверхности жидкости")] public float surfaceY = 1f;
    [Tooltip("Y-координата дна")] public float bottomY = 0f;

    [Header("Scene Object")]
    public Transform objectTransform;

    private float fluidDensity;    
    private float volume;          
    private float horizontalSpeed; 
    private float dragCoeff;       
    private float startTime;
    private bool running = false;
    private float verticalVelocity;
    private float area;

    public override void ExecuteTask()
    {
        if (!float.TryParse(fluidDensityInput.text, out fluidDensity) ||
            !float.TryParse(volumeInput.text, out volume) ||
            !float.TryParse(speedInput.text, out horizontalSpeed))
        {
            resultText.text = "Ошибка: проверьте введённые ρ1, V, v.";
            return;
        }
        transform.position = new Vector3(-131f, 0f, 141f);
        dragCoeff = 0.05f; 
        area = Mathf.Pow(volume, 2f / 3f);

        startTime = Time.time;
        verticalVelocity = 0f;
        running = true;
        resultText.text = "Симуляция запущена...";
    }

    void Update()
    {
        if (!running) return;

        float t = Time.time - startTime;
        float objectDensity = densityCurve.Evaluate(t); 
        float mass = objectDensity * volume;


        float buoyantForce = fluidDensity * volume * -Physics.gravity.y;  
        float weightForce  = objectDensity * volume * -Physics.gravity.y;  

        float dragForce = 0f;
        if (objectTransform.position.y <= surfaceY)
        {
            dragForce = 0.5f * dragCoeff * fluidDensity * area * verticalVelocity * Mathf.Abs(verticalVelocity);
            dragForce *= -Mathf.Sign(verticalVelocity);
        }

        float netForce = buoyantForce - weightForce + dragForce;
        float acceleration = netForce / mass;

        verticalVelocity += acceleration * Time.deltaTime;
        Vector3 pos = objectTransform.position;
        pos.y += verticalVelocity * Time.deltaTime;
        pos.x += horizontalSpeed * Time.deltaTime; 
        objectTransform.position = pos;

        if (objectTransform.position.y >= surfaceY)
        {
            objectTransform.position = new Vector3(pos.x, surfaceY, pos.z);
            if (verticalVelocity > 0) verticalVelocity = 0;
        }
        else if (objectTransform.position.y <= bottomY)
        {
            objectTransform.position = new Vector3(pos.x, bottomY, pos.z);
            if (verticalVelocity < 0) verticalVelocity = 0;
        }

        resultText.text =
            $"t={t:F2} c\n" +
            $"ρ2={objectDensity:F1} кг/м³\n" +
            $"F_A={buoyantForce:F1} Н, F_g={weightForce:F1} Н, F_d={dragForce:F1} Н\n" +
            $"a={acceleration:F2} м/с², vY={verticalVelocity:F2} м/с\n";
    }
}
