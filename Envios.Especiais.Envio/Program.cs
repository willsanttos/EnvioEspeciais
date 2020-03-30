using Envios.Especiais.Domain.Interfaces.Services;
using Envios.Especiais.Infra.DependencyResolver;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Envios.Especiais.Envio
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine($"******************************** INICIO DO PROCESSO - {DateTime.Now.ToLongTimeString()} ****************************************");
            DateTime localDateInicio = DateTime.Now;
            if(localDateInicio.DayOfWeek != DayOfWeek.Saturday && localDateInicio.DayOfWeek != DayOfWeek.Sunday)
                RunApplication(localDateInicio);
        }


        private static void RunApplication(DateTime data)
        {
            try
            {
                var kernel = new StandardKernel(new ServiceModule());
                var controller = kernel.Get<IControllerService>();
                controller.Executar();
                WriteLine($"******************************** FINAL DO PROCESSO : Hora Inicio: {data} Hora Fim: {DateTime.Now.ToLongTimeString()} ****************************************");
                ReadKey();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
    }
}
