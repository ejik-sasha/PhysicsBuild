using UnityEngine;
using TMPro;

public class Lab8_1_1 : BaseLab
{
    public TMP_InputField angleInput;
    public TMP_InputField thicknessInput;
    public TMP_InputField n2Input;
    public GameObject cccube;
    public GameObject fakeCube;

    public float maxDistance = 100f;

    public float thickness = 50f;
    public float n1 = 1f; 
    public float n2 = 1.5f; 
    public float a = 30f;  
    

     private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        transform.Rotate(0, -90, 0);
        
        ExecuteTask();

    }

    void Update()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        Debug.DrawRay(transform.position, -transform.up * 1000f, Color.yellow);

        RaycastHit hit;
        if ((Physics.Raycast(ray, out hit, maxDistance) && cccube.transform.localScale.y != 0 && (a != 0) && n2 > 1) || fakeCube.activeSelf)
        {
            Vector3 entryPoint = hit.point;
            Vector3 normal = hit.normal;

            float sign = Mathf.Sign(a);

            float incidentAngle = Vector3.Angle(-transform.up, normal) * Mathf.Deg2Rad;

            float refractedAngle = Mathf.Asin(n1 * Mathf.Sin(incidentAngle) / n2) * sign;

            Vector3 tangent = Vector3.Cross(normal, Vector3.forward).normalized;
            Vector3 refractedDir = Mathf.Cos(refractedAngle) * -normal + Mathf.Sin(refractedAngle) * tangent;

            Vector3 exitPoint = entryPoint + refractedDir.normalized * thickness;
            float exitAngle = (Mathf.Asin(n2 * Mathf.Sin(Mathf.Abs(refractedAngle)) / n1) * sign) + 90;
            if (a < 0)
            {
                exitAngle = (Mathf.Asin(n2 * Mathf.Sin(Mathf.Abs(refractedAngle)) / n1) * sign) - 90;
            }
            
            Vector3 exitDir = Mathf.Cos(exitAngle) * normal + Mathf.Sin(exitAngle) * tangent;

            lineRenderer.positionCount = 4;
            lineRenderer.SetPosition(0, transform.position);           
            lineRenderer.SetPosition(1, entryPoint);
            lineRenderer.SetPosition(2, exitPoint);
            lineRenderer.SetPosition(3, exitPoint + exitDir.normalized * 200f); 

            if (cccube.transform.localScale.y != 0)
                fakeCube.SetActive(false);
        }
        else
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position - transform.up * 1000f);
        }
    }

    public override void ExecuteTask()
    {
        if (
            float.TryParse(thicknessInput.text, out thickness) &&
            float.TryParse(n2Input.text, out n2) &&
            float.TryParse(angleInput.text, out a)
        )
        {

            transform.Rotate(0, 90, a);


            cccube.transform.localScale = new Vector3(
            cccube.transform.localScale.x,
            thickness,
            cccube.transform.localScale.z);


            
        }
        else
        {
            Debug.LogWarning("Некорректный ввод!");
        }
    }

    

    
    
}
