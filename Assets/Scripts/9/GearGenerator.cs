using UnityEngine;

[ExecuteAlways]
public class GearGenerator : MonoBehaviour
{
    [Header("Параметры колеса")]
    public int teethCount = 12;   
    public float module = 1f;   
    public float thickness = 0.2f;  

    [Header("Параметры зуба")]
    [Range(0.2f, 1f)] public float toothWidthFactor = 0.8f;  
    public float toothHeight = 0.3f;

    GameObject baseCircle;
    GameObject teethParent;

    public float PitchDiameter => module * Mathf.Max(1, teethCount);
    public float Radius => PitchDiameter * 0.5f;

    public void GenerateGear()
    {
        if (Application.isPlaying)
        {
            if (teethParent != null) Destroy(teethParent);
            if (baseCircle != null) Destroy(baseCircle);
        }
        else
        {
            if (teethParent != null) DestroyImmediate(teethParent);
            if (baseCircle != null) DestroyImmediate(baseCircle);
        }

        float radius = Radius;
        if (teethCount < 3) teethCount = 3;
        if (module <= 0.0001f) module = 0.001f;

        baseCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseCircle.name = "Base";
        baseCircle.transform.SetParent(transform, false);

        baseCircle.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        baseCircle.transform.localScale = new Vector3(radius * 2f, thickness * 0.5f, radius * 2f);

        teethParent = new GameObject("Teeth");
        teethParent.transform.SetParent(transform, false);

        float stepAngleDeg = 360f / teethCount;
        float stepAngleRad = stepAngleDeg * Mathf.Deg2Rad;

        float pitch = 2f * Mathf.PI * radius / teethCount; 
        float minW = pitch * 0.3f;
        float maxW = pitch * 0.95f;
        float targetW = pitch * Mathf.Clamp01(toothWidthFactor);
        float toothWidth = Mathf.Clamp(targetW, minW, maxW);

        for (int i = 0; i < teethCount; i++)
        {
            float ang = i * stepAngleRad;
            Vector3 radialDir = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0f); 
            Vector3 tangentDir = new Vector3(-Mathf.Sin(ang), Mathf.Cos(ang), 0f); 

            GameObject tooth = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tooth.name = $"Tooth_{i}";
            tooth.transform.SetParent(teethParent.transform, false);

            tooth.transform.localScale = new Vector3(toothWidth / 4, toothHeight * 10 , thickness);

            float centerRadius = radius + toothHeight * 0.5f;
            tooth.transform.localPosition = radialDir * centerRadius;

           
            
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, radialDir);
            tooth.transform.localRotation = rot;
  
        }
    }

    
}
