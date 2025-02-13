using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private bool _isRunning; // Флаг состояния таймера.

    private float _timeRemaining; // Время, оставшееся на таймере.

    /// <summary>
    ///     Обновляет таймер в каждом кадре.
    /// </summary>
    private void Update()
    {
        if (_isRunning && _timeRemaining > 0)
        {
            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                _isRunning = false;
                OnTimerEnd?.Invoke(); // Вызываем событие окончания таймера.
            }
        }
    }

    public event Action OnTimerEnd; // Событие, которое вызывается при завершении таймера.

    /// <summary>
    ///     Устанавливает время таймера.
    /// </summary>
    /// <param name="time">Время в секундах.</param>
    public void SetTime(float time)
    {
        _timeRemaining = Mathf.Max(0, time);
    }

    /// <summary>
    ///     Запускает таймер.
    /// </summary>
    public void StartTimer()
    {
        if (_timeRemaining > 0) _isRunning = true;
    }

    /// <summary>
    ///     Останавливает таймер.
    /// </summary>
    public void StopTimer()
    {
        _isRunning = false;
    }

    /// <summary>
    ///     Возвращает оставшееся время.
    /// </summary>
    /// <returns>Оставшееся время в секундах.</returns>
    public float GetTimeRemaining()
    {
        return _timeRemaining;
    }

    /// <summary>
    ///     Проверяет, запущен ли таймер.
    /// </summary>
    /// <returns>True, если таймер запущен; иначе false.</returns>
    public bool IsRunning()
    {
        return _isRunning;
    }
}