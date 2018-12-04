using System;
using System.Numerics;

namespace FFT
{
    public class FFT
    {
        /// <summary>
        /// Вычисление поворачивающего модуля e^(-i*2*PI*k/N)
        /// </summary>
        /// <param name="k"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        private static Complex W(int k, int N)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }
        /// <summary>
        /// Возвращает спектр сигнала
        /// </summary>
        /// <param name="x">Массив значений сигнала. Количество значений должно быть степенью 2</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        public static Complex[] Fft(Complex[] x)
        {
            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                Complex[] X_even = Fft(x_even);
                Complex[] X_odd = Fft(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + W(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - W(i, N) * X_odd[i];
                }
            }
            return X;
        }
        /// <summary>
        /// Центровка массива значений полученных в fft (спектральная составляющая при нулевой частоте будет в центре массива)
        /// </summary>
        /// <param name="X">Массив значений полученный в fft</param>
        /// <returns></returns>
        public static Complex[] Nfft(Complex[] X)
        {
            int N = X.Length;
            Complex[] X_n = new Complex[N];
            for (int i = 0; i < N / 2; i++)
            {
                X_n[i] = X[N / 2 + i];
                X_n[N / 2 + i] = X[i];
            }
            return X_n;
        }
    }
  //  public class smoothly_filter
  //  {
  //      float conteiner = new float();
  //
  //      public smoothly_filter()
  //      {
  //
  //      }
  //
  //  }

    public class FilterButterworth
    {
        /// <summary>
        /// rez amount, from sqrt(2) to ~ 0.1
        /// </summary>
        private  float resonance;

        private  float frequency;
        private  int sampleRate;
        private  PassType passType;

        private  float c, a1, a2, a3, b1, b2;

        /// <summary>
        /// Array of input values, latest are in front
        /// </summary>
        private float[] inputHistory = new float[2];

        /// <summary>
        /// Array of output values, latest are in front
        /// </summary>
        private float[] outputHistory = new float[3];

        public FilterButterworth(float frequency, int sampleRate, PassType passType, float resonance)
        {
            this.resonance = resonance;
            this.frequency = frequency;
            this.sampleRate = sampleRate;
            this.passType = passType;

            switch (passType)
            {
                case PassType.Lowpass:
                    c = 1.0f / (float)Math.Tan(Math.PI * frequency / sampleRate);
                    a1 = 1.0f / (1.0f + resonance * c + c * c);
                    a2 = 2f * a1;
                    a3 = a1;
                    b1 = 2.0f * (1.0f - c * c) * a1;
                    b2 = (1.0f - resonance * c + c * c) * a1;
                    break;
                case PassType.Highpass:
                    c = (float)Math.Tan(Math.PI * frequency / sampleRate);
                    a1 = 1.0f / (1.0f + resonance * c + c * c);
                    a2 = -2f * a1;
                    a3 = a1;
                    b1 = 2.0f * (c * c - 1.0f) * a1;
                    b2 = (1.0f - resonance * c + c * c) * a1;
                    break;
            }
        }

        public enum PassType
        {
            Highpass,
            Lowpass,
        }

        public void Update(float newInput)
        {
            float newOutput = a1 * newInput + a2 * this.inputHistory[0] + a3 * this.inputHistory[1] - b1 * this.outputHistory[0] - b2 * this.outputHistory[1];

            this.inputHistory[1] = this.inputHistory[0];
            this.inputHistory[0] = newInput;

            this.outputHistory[2] = this.outputHistory[1];
            this.outputHistory[1] = this.outputHistory[0];
            this.outputHistory[0] = newOutput;
        }

        public float Value
        {
            get { return this.outputHistory[0]; }
        }
        public void Reset()
        {
            this.outputHistory[2] = 0;
            this.outputHistory[1] = 0;
            this.outputHistory[0] = 0;
        }
        public void Setup(float frequency, int sampleRate, PassType passType, float resonance)
        {
            this.resonance = resonance;
            this.frequency = frequency;
            this.sampleRate = sampleRate;
            this.passType = passType;

            switch (passType)
            {
                case PassType.Lowpass:
                    c = 1.0f / (float)Math.Tan(Math.PI * frequency / sampleRate);
                    a1 = 1.0f / (1.0f + resonance * c + c * c);
                    a2 = 2f * a1;
                    a3 = a1;
                    b1 = 2.0f * (1.0f - c * c) * a1;
                    b2 = (1.0f - resonance * c + c * c) * a1;
                    break;
                case PassType.Highpass:
                    c = (float)Math.Tan(Math.PI * frequency / sampleRate);
                    a1 = 1.0f / (1.0f + resonance * c + c * c);
                    a2 = -2f * a1;
                    a3 = a1;
                    b1 = 2.0f * (c * c - 1.0f) * a1;
                    b2 = (1.0f - resonance * c + c * c) * a1;
                    break;
            }
        }
    }

    public class KalmanFilterSimple1D
    {
        public double X0 { get; private set; } // predicted state
        public double P0 { get; private set; } // predicted covariance

        public double F { get; private set; } // factor of real value to previous real value
        public double Q { get; private set; } // measurement noise
        public double H { get; private set; } // factor of measured value to real value
        public double R { get; private set; } // environment noise

        public double State { get; private set; }
        public double Covariance { get; private set; }

        public KalmanFilterSimple1D(double q, double r, double f = 1, double h = 1)
        {
            Q = q;
            R = r;
            F = f;
            H = h;
        }

        public void SetState(double state, double covariance)
        {
            State = state;
            Covariance = covariance;
        }

        public void Correct(double data)
        {
            //time update - prediction
            X0 = F * State;
            P0 = F * Covariance * F + Q;

            //measurement update - correction
            var K = H * P0 / (H * P0 * H + R);
            State = X0 + K * (data - H * X0);
            Covariance = (1 - K * H) * P0;
        }
    }
}