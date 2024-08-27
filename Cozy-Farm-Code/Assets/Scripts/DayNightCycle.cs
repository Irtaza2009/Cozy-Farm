using UnityEngine;
using System.Collections.Generic;

public class DayNightCycle : MonoBehaviour
{

    public float dayDuration = 60f; // Duration of a day in seconds
    private float time;

    private bool isDay = true;
    private List<GameObject> chicks;
    public Canvas nightOverlayCanvas;

    void Start()
    {
        chicks = new List<GameObject>(GameObject.FindGameObjectsWithTag("Chick"));
        nightOverlayCanvas.enabled = false; // Ensure the overlay is disabled at the start
    }

    void Update()
    {
        time += Time.deltaTime;
        float normalizedTime = time / dayDuration;
        float angle = Mathf.Lerp(0, 360, normalizedTime);



        // Check for day to night transition
        if (isDay && normalizedTime >= 0.5f)
        {
            isDay = false;
            TransitionToNight();
        }
        // Check for night to day transition
        else if (!isDay && normalizedTime < 0.5f)
        {
            isDay = true;
            TransitionToDay();
        }

        // Reset the time after one day
        if (time >= dayDuration)
        {
            time = 0;
        }
        Debug.Log("Time: " + time);
        Debug.Log("isDay: " + isDay);
    }

    void TransitionToNight()
    {
        nightOverlayCanvas.enabled = true;
        Debug.Log("Transition to night");
        foreach (GameObject chick in chicks)
        {
            ChickBehavior chickBehavior = chick.GetComponent<ChickBehavior>();
            if (chickBehavior != null)
            {
                chickBehavior.GoToHenhouse();
                chick.SetActive(false);
            }
        }
    }

    void TransitionToDay()
    {
        nightOverlayCanvas.enabled = false; // Disable the night overlay
        foreach (GameObject chick in chicks)
        {
            chick.SetActive(true);
            ChickBehavior chickBehavior = chick.GetComponent<ChickBehavior>();
            if (chickBehavior != null)
            {
                chickBehavior.GoOutside();
            }
        }
    }
}
