using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;
using Envios.Especiais.Infra.Service.Services;
using Envios.Especiais.Domain.Interfaces.Services;
using Envios.Especiais.Infra.Service.Services.Envio;
using Envios.Especiais.Domain.Interfaces.Repositories;
using Envios.Especiais.Infra.Repository.Repositories;

namespace Envios.Especiais.Infra.DependencyResolver
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IControllerService>().To<ControllerService>();
            Bind<IDistribuicaoService>().To<EnvioDistribuicao>();

            Bind<IDistribuicaoRepository>().To<DistribuicaoRepository>();
            Bind<ILogEnvioRepository>().To<LogEnvioRepository>();

            Bind<IJuridicoService>().To<EnvioJuridico>();
            Bind<IJuridicoRepository>().To<JuridicoRepository>();

            Bind<IAndamentoRepository>().To<AndamentoRepository>();
            Bind<IAndamentoService>().To<EnvioAndamento>();            
        }
    }
}
