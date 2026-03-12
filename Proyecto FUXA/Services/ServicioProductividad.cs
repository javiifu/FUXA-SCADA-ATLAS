using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Services
{

    //Para calcular la productividad, tomaremos el numero de cilocs reales registrdos y se dividirá entre el objtivo de estos ciclos por hora. se multiplica x100 para botener el porcentaje
    public class ServicioProductividad
    {
        public decimal CalculateHourlyProductivity(Machine machine, int realCycles)
        {
            if (machine.CiclosHoraObjetivo <= 0) return 0;
            return Math.Round((decimal)realCycles / machine.CiclosHoraObjetivo * 100, 2); //Aquí redondearemos en dos decimales, para ser más claros con el resultado. 
        }
    }
}
