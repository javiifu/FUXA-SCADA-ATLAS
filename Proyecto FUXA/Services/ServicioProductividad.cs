namespace Proyecto_FUXA.Services;

public class ServicioProductividad
{
    public double CalculateHourlyProductivity(int realCycles, int targetCycles)
    {
        public decimal CalculateHourlyProductivity(Maquina machine, int realCycles)
        {
            if (machine.CiclosObjetivo <= 0) return 0;
            return Math.Round((decimal)realCycles / machine.CiclosObjetivo * 100, 2); //Aquí redondearemos en dos decimales, para ser más claros con el resultado. 
        }
    }

    public double Calculate(int realCycles, int targetCycles) =>
        CalculateHourlyProductivity(realCycles, targetCycles);
}
