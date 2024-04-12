using Meadow;
using Meadow.Devices;
using Meadow.Units;
using Munit;

namespace ReleaseValidation.ProjectLab;

public class AnalogTests
{
    [Fact]
    public void PwmToAnalogTest()
    {
        var ccm = Resolver.Device as F7CoreComputeV2;
        Assert.NotNull(ccm);

        // using 10k resistor and a 0.1uF cap low-pass RC filter
        // http://sim.okawa-denshi.jp/en/PWMtool.php

        var pwm = ccm.Pins.PB8.CreatePwmPort(10_000.Hertz(), 0.25f);
        var anin = ccm.Pins.PA3.CreateAnalogInputPort();

        pwm.Start();
        var analog = anin.Read().Result.Volts;
        // measured: 0.84
        // theory: 0.825
        Resolver.Log.Debug($"25% PWM: {analog:N1} V"); // 0.84
        Assert.True(analog > 0.8, $"25% PWM expected ADC > 0.08V");
        Assert.True(analog < 0.9, $"25% PWM expected ADC < 0.09V");

        pwm.DutyCycle = 0.5f;
        analog = anin.Read().Result.Volts;
        // measured: 1.57
        // theory: 1.65
        Resolver.Log.Debug($"50% PWM: {analog:N1} V");
        Assert.True(analog > 1.5, $"50% PWM expected ADC > 1.5V");
        Assert.True(analog < 1.7, $"50% PWM expected ADC < 1.7V");

        pwm.DutyCycle = 0.75f;
        analog = anin.Read().Result.Volts;
        // measured: 2.35
        // theory: 2.475
        Resolver.Log.Debug($"75% PWM: {analog:N1} V");
        Assert.True(analog > 2.3, $"75% PWM expected ADC > 2.3V");
        Assert.True(analog < 2.5, $"75% PWM expected ADC < 2.5V");

        pwm.DutyCycle = 1.0f;
        analog = anin.Read().Result.Volts;
        // measured: 3.17
        // theory: 3.3
        Resolver.Log.Debug($"100% PWM: {analog:N1} V"); // 0.84
        Assert.True(analog > 3.1, $"100% PWM expected ADC > 3.1V");
    }
}
