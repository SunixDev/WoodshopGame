using UnityEngine;
using System.Collections;

public class DadoBlock : MonoBehaviour 
{
    //public DadoBlockArea FrontFace;
    //public DadoBlockArea BackFace;
    //public LineSegment Line;
    public float NumberOfCuts;
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    private float ScalingAmountPerCut;
    private Vector3 StartingScale;
    private bool Scalable;
    private int CutsMade;
    
    void Start()
    {
        Scalable = true;
        ScalingAmountPerCut = 1.0f / NumberOfCuts;
        StartingScale = transform.localScale;
        CutsMade = 0;
    }

    void OnEnable()
    {
        Scalable = true;
        ScalingAmountPerCut = 1.0f / NumberOfCuts;
        StartingScale = transform.localScale;
    }

    public void ScaleDown()
    {
        float previousScale = transform.localScale.y;
        float resizing = StartingScale.y * ScalingAmountPerCut;
        float changeAmount = previousScale - resizing;
        transform.localScale = new Vector3(StartingScale.x, changeAmount, StartingScale.z);
        CutsMade++;
        //float newX = transform.position.x + (((previousScale - transform.localScale.x) / (NumberOfCuts * NumberOfCuts)) / NumberOfCuts);
        //Vector3 movement = new Vector3(newX, transform.position.y, transform.position.z) - transform.position;
        //Vector3 newPosition = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z) * movement;
        //transform.position += movement;

        if (transform.localScale.x <= 0.0f || CutsMade == NumberOfCuts)
        {
            Scalable = false;
        }
    }

    public bool AnyCutsLeft()
    {
        return Scalable;
    }
}
