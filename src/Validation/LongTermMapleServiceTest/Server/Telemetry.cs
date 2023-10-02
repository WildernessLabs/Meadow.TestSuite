using System;

namespace LongTermMapleServiceTest;

public class Telemetry
{
    private static int _count = 0;
    private static float _temp = 72f;
    private static float _pressure = 1013.25f;
    private static int _start = Environment.TickCount;
    private static Random _random = new Random();

    public Telemetry()
    {
        _count++;
        _temp += ((float)_random.NextDouble() * 2f) - 1f;
        _pressure += ((float)_random.NextDouble() * 30f) - 15f;
    }

    public int Count => _count;
    public float Temperature => _temp;
    public float Pressure => _pressure;
    public int RunTime => Environment.TickCount - _start;
}