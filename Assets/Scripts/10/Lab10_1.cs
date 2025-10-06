using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Lab10_1 : BaseLab
{
    [Header("UI Elements")]
    public TMP_InputField S1Input;
    public TMP_InputField S2Input;
    public TMP_InputField massInput;
    public TMP_InputField forceInput;
    public TMP_InputField liftHeightInput;
    public TMP_InputField liftSpeedInput;

    public TextMeshProUGUI resultText;

    [Header("Pistons & Load")]
    public Transform smallPiston;
    public Transform bigPiston;
    public Transform loadObject;

    private float S1, S2, mass, inputForce, liftHeight, liftSpeed;
    private const float g = 9.81f;

    void Start()
    {
        
    }

    public override void ExecuteTask()
    {
        if (!float.TryParse(S1Input.text, out S1) ||
            !float.TryParse(S2Input.text, out S2) ||
            !float.TryParse(massInput.text, out mass) ||
            !float.TryParse(forceInput.text, out inputForce) ||
            !float.TryParse(liftHeightInput.text, out liftHeight) ||
            !float.TryParse(liftSpeedInput.text, out liftSpeed))
        {
            resultText.text = "Ошибка: некорректные входные данные.";
            return;
        }

        if (CanLift())
        {
            resultText.text = "Сила достаточна. Запуск подъёма...";
            StartCoroutine(LiftRoutine());
        }
        else
        {
            resultText.text = "Недостаточно силы для подъёма груза.";
        }
    }

    bool CanLift()
    {
        float outputForce = inputForce * (S2 / S1);
        float weight = mass * g;
        return outputForce >= weight;
    }

    IEnumerator LiftRoutine()
    {
        float moved = 0f;
        while (moved < liftHeight)
        {
            float step = liftSpeed * Time.deltaTime;
            smallPiston.Translate(Vector3.down * step, Space.World);
            bigPiston.Translate(Vector3.up * step, Space.World);
            loadObject.Translate(Vector3.up * step, Space.World);
            moved += step;
            yield return null;
        }
        resultText.text = "Подъём завершён успешно.";
    }
}
