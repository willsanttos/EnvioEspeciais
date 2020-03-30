using Envios.Especiais.Domain.Entities;
using Envios.Especiais.Domain.Enums;
using Envios.Especiais.Domain.Interfaces.Repositories;
using Envios.Especiais.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Envios.Especiais.Infra.Service.Services
{
    public class ControllerService : IControllerService
    {
        private readonly IDistribuicaoRepository _distribuicaoRepository;
        private readonly IDistribuicaoService _distribuicaoService;
        private readonly IJuridicoRepository _juridicoRepository;
        private readonly IJuridicoService _juridicoService;
        private readonly IAndamentoService _andamentoService;
        private readonly ILogEnvioRepository _logEnvioRepository;



        public ControllerService(IDistribuicaoRepository distribuicaoRepository, IDistribuicaoService distribuicaoService,
                                 IJuridicoService juridicoService, IJuridicoRepository juridicoRepository,
                                 IAndamentoRepository andamentoRepository, IAndamentoService andamentoService,
                                 ILogEnvioRepository logEnvioRepository)
        {
            _distribuicaoRepository = distribuicaoRepository;
            _distribuicaoService = distribuicaoService;

            _juridicoRepository = juridicoRepository;
            _juridicoService = juridicoService;

            _andamentoService = andamentoService;
            _logEnvioRepository = logEnvioRepository;
        }
        public void Executar()
        {            
            _distribuicaoService.AtualizarTodosTermosPesquisasParaPendente();

            EnviarEmailDistribuicaoPlanoAExpirar();

            EnviarEmailDistribuicaoSituacaoPendente();

            EnvioJuridicoSituacaoPendente();

            EnvioJuridicoMeridioQdtPublicacoesNaoCapturadas();

            EnvioAndamentoMeridioQdtPublicacoesNaoCapturadas();
         }


        private void EnviarEmailDistribuicaoPlanoAExpirar()
        {
            try
            {
                var scheduledTime = DateTime.Parse(ConfigurationManager.AppSettings["ScheduledTimeDistribuicao"]);
                if (DateTime.Now >= scheduledTime && DateTime.Now <= scheduledTime.AddHours(1))
                {
                    var clientes = _distribuicaoRepository.ConsultarClienteFimTeste().ToList();

                    if (clientes.Count > 0)
                    {
                        Task task = Task.Run(() =>
                            Parallel.ForEach(clientes, new ParallelOptions { MaxDegreeOfParallelism = 1 }, currentcliente =>
                            {
                                _distribuicaoService.EnviarEmailDistribuicaoPlanoAExpirar(currentcliente);
                            })
                        );

                        task.Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void EnviarEmailDistribuicaoSituacaoPendente()
        {
            try
            {
                var scheduledTime = DateTime.Parse(ConfigurationManager.AppSettings["ScheduledTimeDistribuicao"]);
                if (DateTime.Now >= scheduledTime && DateTime.Now <= scheduledTime.AddHours(1))
                {
                    var clientes = _distribuicaoRepository.ConsultarClienteSituacaoParaPendente().ToList();

                    if (clientes.Count > 0)
                    {
                        Task task = Task.Run(() =>
                            Parallel.ForEach(clientes, new ParallelOptions { MaxDegreeOfParallelism = 1 }, currentcliente =>
                            {
                                _distribuicaoService.EnvioDistribuicaoSituacaoPendente(currentcliente);
                            })
                        );

                        task.Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EnvioJuridicoSituacaoPendente()
        {
            try
            {
                var scheduledTime = DateTime.Parse(ConfigurationManager.AppSettings["ScheduledTimeJuridico"]);
                if (DateTime.Now >= scheduledTime && DateTime.Now <= scheduledTime.AddHours(1))
                {
                    var clientes = _juridicoRepository.ConsultarClienteFimTeste().ToList();

                    if (clientes.Count > 0)
                    {
                        Task task = Task.Run(() =>
                            Parallel.ForEach(clientes, new ParallelOptions { MaxDegreeOfParallelism = 1 }, currentcliente =>
                            {
                                _juridicoService.EnvioJuridicoSituacaoPendente(currentcliente);
                            })
                        );
                        task.Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EnvioJuridicoMeridioQdtPublicacoesNaoCapturadas()
        {
            string scheduledTime = ConfigurationManager.AppSettings["ScheduledTimeJuridicoMeridio"];            
            var log = _logEnvioRepository.BuscarLogEnvio("silvio.brayner.base", (int)ProdutoEnvio.JURIDICO);
            var list = scheduledTime.Split(';');

            if(AutorizarEnvioMeridio(log, list))
            {
                _juridicoService.EnvioJuridicoMeridioQdtPublicacoesNaoCapturadas();
            }
        }

        private void EnvioAndamentoMeridioQdtPublicacoesNaoCapturadas()
        {
            string scheduledTime = ConfigurationManager.AppSettings["ScheduledTimeAndamentoMeridio"];
            var log = _logEnvioRepository.BuscarLogEnvio("Andamento.Meridio", (int)ProdutoEnvio.JURIDICO);
            var list = scheduledTime.Split(';');

            if (AutorizarEnvioMeridio(log, list))
            {
                _andamentoService.EnvioAndamentoMeridioQdtPublicacoesNaoCapturadas();
            }

        }

        private bool AutorizarEnvioMeridio(List<Cliente> c, string[] listSchedule)
        {
            bool toReturn = false;
            
            for (int i = 0; i <= 3; i++)
            {
                if (c.Count <= i && DateTime.Parse(listSchedule[i]) <= DateTime.Now)
                {
                    toReturn = true;
                    break;
                }
            }
            return toReturn;
        }

    }
}
