public delegate float Function(double t);

public interface Integration
{
    float Integrate(double tmin, double tmax, Function function);
}

