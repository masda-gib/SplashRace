using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RombergIntegration : Integration
{
    public float Integrate(double a, double b, Function F)
    {
        const int order = 5;
        double[,] rom = new double[2, order];
    
        double h = b-a;
    
        // initialize T_{1,1} entry
        rom[0, 0] = h*(F(a)+F(b))/2;
        for (int i = 2, ipower = 1; i <= order; i++, ipower *= 2, h /= 2)
        {
            // calculate summation in recursion formula for T_{k,1}
            float sum = 0;
            for (int j = 1; j <= ipower; j++)
                sum += F(a + h * (j - 0.5f));
            
            // trapezoidal approximations
            rom[1, 0] = (rom[0, 0]+h*sum)/2;
            
            // Richardson extrapolation
            for (int k = 1, kpower = 4; k < i; k++, kpower *= 4)
                rom[1, k] = (kpower*rom[1, k-1] - rom[0, k-1])/(kpower-1);
            
            // save extrapolated values for next pass
            for (int j = 0; j < i; j++)
                rom[0, j] = rom[1, j];
        }
        return (float) rom[0, order-1];
    }
}
