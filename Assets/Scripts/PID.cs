using System;

public class PID
{
    private float Ts;                  // Sample period in seconds
    private float K;                   // Rollup parameter
    private float b0, b1, b2;          // Rollup parameters
    private float a0, a1, a2;          // Rollup parameters
    private float y0 = 0;              // Current output
    private float y1 = 0;              // Output one iteration old
    private float y2 = 0;              // Output two iterations old
    private float e0 = 0;              // Current error
    private float e1 = 0;              // Error one iteration old
    private float e2 = 0;              // Error two iterations old

    /// <summary>
    /// PID Constructor
    /// </summary>
    /// <param name="Kp">Proportional Gain</param>
    /// <param name="Ki">Integral Gain</param>
    /// <param name="Kd">Derivative Gain</param>
    /// <param name="N">Derivative Filter Coefficient</param>
    /// <param name="OutputUpperLimit">Controller Upper Output Limit</param>
    /// <param name="OutputLowerLimit">Controller Lower Output Limit</param>
    public PID(float Kp, float Ki, float Kd, float N, float OutputUpperLimit, float OutputLowerLimit)
    {
        this.Kp = Kp;
        this.Ki = Ki;
        this.Kd = Kd;
        this.N = N;
        this.OutputUpperLimit = OutputUpperLimit;
        this.OutputLowerLimit = OutputLowerLimit;
    }

    /// <summary>
    /// PID iterator, call this function every sample period to get the current controller output.
    /// setpoint and processValue should use the same units.
    /// </summary>
    /// <param name="setPoint">Current Desired Setpoint</param>
    /// <param name="processValue">Current Process Value</param>
    /// <param name="ts">Timespan Since Last Iteration, Use Default Sample Period for First Call</param>
    /// <returns>Current Controller Output</returns>
    public float PID_iterate(float setPoint, float processValue, float ts)
    {
        // Ensure the timespan is not too small or zero.
        Ts = (ts >= TsMin) ? ts : TsMin;

        // Calculate rollup parameters
        K = 2 / Ts;
        var k2 = K * K;
        b0 = (float)(k2 * Kp + K * Ki + Ki * N + K * Kp * N + k2 * Kd * N);
        b1 = (float)(2 * Ki * N - 2 * k2 * Kp - 2 * k2 * Kd * N);
        b2 = (float)(k2 * Kp - K * Ki + Ki * N - K * Kp * N + k2 * Kd * N);
        a0 = (float)(k2 + N * K);
        a1 = (float)(-2 * k2);
        a2 = (float)(k2 - K * N);

        // Age errors and output history
        e2 = e1;                        // Age errors one iteration
        e1 = e0;                        // Age errors one iteration
        e0 = setPoint - processValue;   // Compute new error
        y2 = y1;                        // Age outputs one iteration
        y1 = y0;                        // Age outputs one iteration
        y0 = -a1 / a0 * y1 - a2 / a0 * y2 + b0 / a0 * e0 + b1 / a0 * e1 + b2 / a0 * e2; // Calculate current output

        // Clamp output if needed
        if (y0 > OutputUpperLimit)
        {
            y0 = OutputUpperLimit;
        }
        else if (y0 < OutputLowerLimit)
        {
            y0 = OutputLowerLimit;
        }

        return y0;
    }

    /// <summary>
    /// Reset controller history effectively resetting the controller.
    /// </summary>
    public void ResetController()
    {
        e2 = 0;
        e1 = 0;
        e0 = 0;
        y2 = 0;
        y1 = 0;
        y0 = 0;
    }

    /// <summary>
    /// Proportional Gain, consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public float Kp { get; set; }

    /// <summary>
    /// Integral Gain, consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public float Ki { get; set; }

    /// <summary>
    /// Derivative Gain, consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public float Kd { get; set; }

    /// <summary>
    /// Derivative filter coefficient.
    /// A smaller N for more filtering.
    /// A larger N for less filtering.
    /// Consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public float N { get; set; }

    /// <summary>
    /// Minimum allowed sample period to avoid dividing by zero!
    /// The Ts value can be mistakenly set to too low of a value or zero on the first iteration.
    /// TsMin by default is set to 1 millisecond.
    /// </summary>
    public float TsMin { get; set; } = 0.001f;

    /// <summary>
    /// Upper output limit of the controller.
    /// This should obviously be a numerically greater value than the lower output limit.
    /// </summary>
    public float OutputUpperLimit { get; set; }

    /// <summary>
    /// Lower output limit of the controller
    /// This should obviously be a numerically lesser value than the upper output limit.
    /// </summary>
    public float OutputLowerLimit { get; set; }
}