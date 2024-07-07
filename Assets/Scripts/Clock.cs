using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] public float dayDur;
    [SerializeField] public float nightDur;
    [SerializeField] GameObject Day, Night;

    float currentDur;
    float lastTime;

    public static bool IsDay = true;
    public static event Action OnTimeChange;

    private void OnEnable()
    {
        Day.SetActive(true);
        OnTimeChange += ChangeTimeDuration;
    }
    private void OnDisable()
    {
        OnTimeChange -= ChangeTimeDuration;
    }
    private void Start()
    {
        lastTime = Time.time;
        
        currentDur = dayDur;
    }

    private void ChangeTimeDuration()
    {
        Day.SetActive(IsDay);
        Night.SetActive(!IsDay);

        currentDur = IsDay? dayDur : nightDur;
    }


    private void Update()
    {
        if (Time.time - lastTime >= currentDur)
        {
            lastTime = Time.time;
            IsDay = !IsDay;
            OnTimeChange?.Invoke();
        }

    }
}
