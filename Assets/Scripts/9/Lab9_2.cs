using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Lab9_2 : BaseLab
{
    [Header("UI")]
    public TMP_InputField omegaInInput;    
    public TMP_InputField teethInput;      
    public TMP_InputField moduleInput;     

    public TMP_Text resultText;

    [Header("Gear Prefab")]
    public GameObject gearPrefab;

    private List<GameObject> gears = new List<GameObject>();

    public override void ExecuteTask()
    {
        foreach (var g in gears) Destroy(g);
        gears.Clear();

        if (!float.TryParse(omegaInInput.text, out float omegaIn))
        {
            resultText.text = "Ошибка ω";
            return;
        }

        string[] parts = teethInput.text.Split(',');
        List<int> Z = new List<int>();
        foreach (string p in parts)
        {
            if (int.TryParse(p.Trim(), out int val) && val > 0)
                Z.Add(val);
        }
        if (Z.Count < 2) { resultText.text = "Введите минимум 2 зубчатых колеса"; return; }

        if (!float.TryParse(moduleInput.text, out float m)) m = 1f;

        float totalU = 1f;
        float currentOmega = omegaIn;

        Vector3 pos = Vector3.zero;
        float offsetX = 0f;

        for (int i = 0; i < Z.Count; i++)
        {
            int teeth = Z[i];
            float d = teeth * m;

            GameObject g = Instantiate(gearPrefab, pos + new Vector3(offsetX-50, 60, 100), Quaternion.identity);
            var gen = g.GetComponent<GearGenerator>();
            var gear = g.GetComponent<Gear>();

            gen.teethCount = teeth;
            gen.module = m;
            gen.GenerateGear();

            bool clockwise = (i % 2 == 0) ? (omegaIn >= 0) : (omegaIn < 0);
            gear.angularSpeed = Mathf.Abs(currentOmega);
            gear.clockwise = clockwise;

            gears.Add(g);

            if (i < Z.Count - 1)
            {
                int nextTeeth = Z[i + 1];
                float dNext = nextTeeth * m;
                float u = (float)nextTeeth / teeth;
                totalU *= u;
                currentOmega = currentOmega / u;

                float dist = (d + dNext) / 2f;
                offsetX += dist;
            }
        }

        float omegaOut = omegaIn / totalU;
        float fOut = omegaOut / (2 * Mathf.PI);

        resultText.text =
            $"Кол-во колёс: {Z.Count}\n" +
            $"Общее U = {totalU:F2}\n" +
            $"ω выход = {omegaOut:F2} рад/с\n" +
            $"f выход = {fOut:F2} Гц ({fOut*60:F0} об/мин)";
    }
}
