using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EKGSistem
{
    class QRS
    {
        private int Sig;
        private double[] Ampl;
        private bool Detected;
        private int DetCounter;
        public QRS()
            
        {
            Ampl = new double[10];
        }
        public bool QRSCheck(double V) 
    {
  double Filter;
   bool Back;
if (Sig != 10)
{
    Ampl[Sig] = V;
    Sig = Sig + 1;
}
else
{
    Ampl[0] = Ampl[1];
    Ampl[1] = Ampl[2];
    Ampl[2] = Ampl[3];
    Ampl[3] = Ampl[4];
    Ampl[4] = Ampl[5];
    Ampl[5] = Ampl[6];
    Ampl[6] = Ampl[7];
    Ampl[7] = Ampl[8];
    Ampl[8] = Ampl[9];
    Ampl[9] = V;
}
    Filter = Ampl[0] +  Ampl[1]*4 + Ampl[2]*6 + Ampl[3]*4 + Ampl[4]
              - Ampl[5] - Ampl[6]*4 - Ampl[7]*6 - Ampl[8]*4 -Ampl[9];
    Back = false;
    if ( (Math.Abs(Filter) > 2) && (Detected == false))  {
           Detected = true;
           Back = Detected;
               if (DetCounter >= 15 ) DetCounter = 5;
    }
    DetCounter++;
    if (DetCounter >= 15)  
        if (Filter > 2)  {
            DetCounter = 0;
            Detected = false;
        }
    return Back;
}
    }
   
}
