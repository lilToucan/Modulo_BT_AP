using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] public float dayDur;
    [SerializeField] public float nightDur;

    float currentDur;
    float lastTime;

    public static bool IsDay = true;
    public static event Action OnTimeChange;

    private void OnEnable()
    {
        OnTimeChange += ChangeTimeDuration;
    }
    private void OnDisable()
    {
        OnTimeChange -= ChangeTimeDuration;
    }

    private void ChangeTimeDuration()
    {
        if (IsDay)
            currentDur = dayDur;
        else
            currentDur = nightDur;
    }

    private void Start()
    {
        lastTime = Time.time;
        
        currentDur = dayDur;
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
