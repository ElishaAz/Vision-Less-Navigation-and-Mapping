using System;

namespace Algorithms
{
    public class SimplePID
    {
        private float Kp, Ki, Kd;
        private float min, max;
        private float previousError;
        private float integral;

        public SimplePID(float kp, float ki, float kd, float min, float max)
        {
            Kp = kp;
            Ki = ki;
            Kd = kd;
            this.min = min;
            this.max = max;
        }

        public float Get(float target, float current, float dt)
        {
            var error = target - current;
            var p = error;
            integral += error * dt;
            var d = (error - previousError) / dt;
            var output = Kp * p + Ki * integral + Kd * d;
            previousError = error;
            return Math.Clamp(output, min, max);
        }

        public void Reset()
        {
            integral = 0;
            previousError = 0;
        }
    }
}