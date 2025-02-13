using GameMode;
using TMPro;
using UnityEngine;

public class TimerObjective : Objective
{
    [SerializeField] private float timeLimit;
    [SerializeField] private TextMeshProUGUI timerUI;
    private float _elapsedTime;
    private bool _isStopped;

    private Timer _timer;

    public void Awake()
    {
        _timer = gameObject.AddComponent<Timer>();
        _timer.SetTime(timeLimit);
        _timer.StartTimer();
    }

    public void FixedUpdate()
    {
        timerUI.text = _timer.GetTimeRemaining().ToString("F");
    }

    private void OnEnable()
    {
        _timer.OnTimerEnd += CompleteObjective;
    }

    private void OnDisable()
    {
        _timer.OnTimerEnd -= CompleteObjective;
    }
}