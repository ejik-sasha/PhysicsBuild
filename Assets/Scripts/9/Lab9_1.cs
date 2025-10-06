using UnityEngine;
using TMPro;

public class Lab9_1 : BaseLab
{
    [Header("UI")]
    public TMP_InputField omega1Input;
    public TMP_InputField Z1Input;
    public TMP_InputField Z2Input;
    public TMP_InputField mInput;
    public TMP_Text resultText;

    [Header("Gear Prefab")]
    public GameObject gearPrefab;

    private Gear gear1;
    private Gear gear2;
    private GearGenerator gen1;
    private GearGenerator gen2;

    public override void ExecuteTask()
    {
        if (float.TryParse(omega1Input.text, out float omega1) &&
            int.TryParse(Z1Input.text, out int Z1) &&
            int.TryParse(Z2Input.text, out int Z2) &&
            float.TryParse(mInput.text, out float m))
        {
            if (Z1 <= 0 || Z2 <= 0 || m <= 0)
            {
                resultText.text = "Ошибка: Z и m должны быть > 0!";
                return;
            }

            if (gear1 != null) Destroy(gear1.gameObject);
            if (gear2 != null) Destroy(gear2.gameObject);

            GameObject g1 = Instantiate(gearPrefab, new Vector3(-50, 60, 100), Quaternion.identity);
            gen1 = g1.GetComponent<GearGenerator>();
            gear1 = g1.GetComponent<Gear>();

            gen1.teethCount = Z1;
            gen1.module = m;
            gen1.GenerateGear();

            float d1 = gen1.PitchDiameter;
            float d2 = m * Z2;

            Vector3 pos2 = new Vector3(((d1 + d2) / 2f) - 50, 60, 100); 
            GameObject g2 = Instantiate(gearPrefab, pos2, Quaternion.identity);
            gen2 = g2.GetComponent<GearGenerator>();
            gear2 = g2.GetComponent<Gear>();

            gen2.teethCount = Z2;
            gen2.module = m;
            gen2.GenerateGear();
            gen2.transform.Rotate(0, 0, 10); 

            float u = (float)Z2 / Z1;
            float omega2 = omega1 / u;
            float f2 = omega2 / (2 * Mathf.PI);

            gear1.angularSpeed = Mathf.Abs(omega1);
            gear2.angularSpeed = Mathf.Abs(omega2);

            bool leadingClockwise = omega1 >= 0;
            gear1.clockwise = leadingClockwise;
            gear2.clockwise = !leadingClockwise;

            resultText.text =
                $"d1={d1:F2}, d2={d2:F2}\n" +
                $"u = {u:F2}\n" +
                $"ω2 = {omega2:F2} рад/с\n" +
                $"f2 = {f2:F2} Гц ({f2 * 60:F0} об/мин)";
        }
        else
        {
            resultText.text = "Ошибка ввода!";
        }
    }
}
