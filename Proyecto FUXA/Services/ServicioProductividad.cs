namespace Proyecto_FUXA.Services;

public class ServicioProductividad
{
    public double CalculateHourlyProductivity(int realCycles, int targetCycles)
    {
        if (targetCycles == 0)
            return 0;

        return Math.Round((double)realCycles / targetCycles * 100, 2);
    }

    public double Calculate(int realCycles, int targetCycles) =>
        CalculateHourlyProductivity(realCycles, targetCycles);
}
