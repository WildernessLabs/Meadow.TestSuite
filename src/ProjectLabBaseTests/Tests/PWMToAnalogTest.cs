using Meadow;
using Meadow.Devices;
using Meadow.Units;

namespace ProjectLabBaseTests;

public class PWMToAnalogTest : TestBase
{
    public override bool Execute(IMeadowDevice device)
    {
        if (device is F7CoreComputeV2 ccm)
        {
            // using 10k resistor and a 0.1uF cap low-pass RC filter
            // http://sim.okawa-denshi.jp/en/PWMtool.php

            var pwm = ccm.Pins.PB8.CreatePwmPort(10_000.Hertz(), 0.25f);
            var anin = ccm.Pins.PA3.CreateAnalogInputPort();

            pwm.Start();
            var analog = anin.Read().Result.Volts;
            // measured: 0.84
            // theory: 0.825
            ReportInfo($"25% PWM: {analog:N1} V"); // 0.84
            if (analog < 0.8 || analog > 0.9) return false;

            pwm.DutyCycle = 0.5f;
            analog = anin.Read().Result.Volts;
            // measured: 1.57
            // theory: 1.65
            ReportInfo($"50% PWM: {analog:N1} V");
            if (analog < 1.5 || analog > 1.7) return false;

            pwm.DutyCycle = 0.75f;
            analog = anin.Read().Result.Volts;
            // measured: 2.35
            // theory: 2.475
            ReportInfo($"75% PWM: {analog:N1} V");
            if (analog < 2.3 || analog > 2.5) return false;

            pwm.DutyCycle = 1.0f;
            analog = anin.Read().Result.Volts;
            // measured: 3.17
            // theory: 3.3
            ReportInfo($"100% PWM: {analog:N1} V"); // 0.84
            if (analog < 3.1) return false;

            return true;
        }
        return false;
    }
}
