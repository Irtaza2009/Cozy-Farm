using UnityEngine;

public class ChickBehavior : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 henhousePosition;
    private bool inHenhouse = false;

    void Start()
    {
        initialPosition = transform.position;
        henhousePosition = GameObject.Find("Henhouse").transform.position; // Ensure the henhouse object is named "Henhouse"
    }

    public void GoToHenhouse()
    {
        transform.position = henhousePosition;
       
        inHenhouse = true;
    }

    public void GoOutside()
    {

        transform.position = initialPosition;
       
        inHenhouse = false;
    }
}
