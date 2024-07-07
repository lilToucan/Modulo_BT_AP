using UnityEngine;

public class ClockHand : MonoBehaviour
{
    [SerializeField] Clock clock;
    float currentDur;
    float time;

    private void OnEnable()
    {
        currentDur = clock.dayDur;
        Clock.OnTimeChange += TimeChanged;
    }
    private void OnDisable()
    {
        Clock.OnTimeChange -= TimeChanged;
    }

    private void TimeChanged()
    {



        if (Clock.IsDay)
        {
            currentDur = clock.dayDur;
     
        }
        else
        {
            currentDur = clock.nightDur;

        }
    }

    private void Update()
    {
        time += Time.deltaTime / currentDur;
        var normilizedTime = time % 1f;

        transform.eulerAngles = Vector3.forward * -(normilizedTime * 360);
    }
}
