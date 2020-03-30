using Envios.Especiais.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Envios.Especiais.Domain.Entities;
using Envios.Especiais.Domain.Interfaces.Repositories;
using Envios.Especiais.Domain.Enums;
using Newtonsoft.Json;
using static System.Console;

namespace Envios.Especiais.Infra.Service.Services.Envio
{
    public class EnvioDistribuicao : IDistribuicaoService
    {
        private readonly IDistribuicaoRepository _distribuicaoRepository;
        private readonly ILogEnvioRepository _logEnvioRepository;

        public EnvioDistribuicao(IDistribuicaoRepository distribuicaoRepository, ILogEnvioRepository iLogEnvioRepository)
        {
            _distribuicaoRepository = distribuicaoRepository;
            _logEnvioRepository = iLogEnvioRepository;
        }

        public void EnviarEmailDistribuicaoPlanoAExpirar(Cliente cliente)
        {
            if (!VerificarEnvio(cliente.Login, (int)ProdutoEnvio.DISTRIBUICAO))
            {
                LogEnvio log = new LogEnvio
                {
                    Login = cliente.Login,
                    Email = cliente.EmailDestinatario,
                    Nome = cliente.Nome,
                    DataFimTeste = cliente.DataFimTeste,
                    DataHoraRegistro = DateTime.Now,
                };

                try
                {
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

                    htmlDoc.LoadHtml(Resource.EmailDistribuicao);

                    htmlDoc.DocumentNode.SelectSingleNode("//span[@id='Nome']").InnerHtml = cliente.Nome;
                    htmlDoc.DocumentNode.SelectSingleNode("//span[@id='DataFimTeste']").InnerHtml = cliente.DataFimTeste.ToShortDateString();

                    cliente.CorpoMensagem = htmlDoc.DocumentNode.OuterHtml;

                    SendGrid obj = new SendGrid();
                    var responseApi = obj.Sender(cliente, (int)ProdutoEnvio.DISTRIBUICAO);
                    if (responseApi.Result != null)
                    {
                        ResponseSendGrid responseSendGrid = JsonConvert.DeserializeObject<ResponseSendGrid>(responseApi.Result);
                        if (responseSendGrid.Success)
                        {
                            var IdMensagem = responseSendGrid.IdMensagem;

                            log.IDMensagemAPI = IdMensagem;
                            log.Status = StatusLog.ENVIADO.ToString();

                            _logEnvioRepository.InserirLogEnvio(log);
                            WriteLine($"*** ENVIO CLIENTE: {cliente.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                        }
                        else
                        {
                            log.Status = StatusLog.ERRO.ToString();
                            log.Observacao = responseSendGrid.ErrorMsg;
                            _logEnvioRepository.InserirLogEnvio(log);
                            WriteLine($"*** CLIENTE NÃO ENVIADO: {cliente.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                        }
                    }
                    else
                    {
                        log.Status = StatusLog.ERRO.ToString();
                        log.Observacao = "Erro na API";
                        _logEnvioRepository.InserirLogEnvio(log);
                        WriteLine($"*** CLIENTE NÃO ENVIADO: {cliente.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                    }
                }
                catch (Exception ex)
                {
                    log.Status = StatusLog.ERRO.ToString();
                    log.Observacao = ex.ToString();
                    _logEnvioRepository.InserirLogEnvio(log);
                    WriteLine($"*** CLIENTE NÃO ENVIADO: {cliente.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                }
            }           
        }


        public void EnvioDistribuicaoSituacaoPendente(Cliente cliente)
        {
            if (!VerificarEnvio(cliente.Login, (int)ProdutoEnvio.DISTRIBUICAO))
            {
                LogEnvio log = new LogEnvio
                {
                    Login = cliente.Login,
                    Email = cliente.EmailDestinatario,
                    Nome = cliente.Nome,
                    DataFimTeste = cliente.DataFimTeste,
                    DataHoraRegistro = DateTime.Now,
                };

                try
                {
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

                    htmlDoc.LoadHtml(Resource.EmailDistribuicaoSituacaoPendente);

                    htmlDoc.DocumentNode.SelectSingleNode("//span[@id='Nome']").InnerHtml = cliente.Nome;
                    htmlDoc.DocumentNode.SelectSingleNode("//span[@id='Data']").InnerHtml = DateTime.Now.ToShortDateString();

                    cliente.CorpoMensagem = htmlDoc.DocumentNode.OuterHtml;

                    SendGrid obj = new SendGrid();
                    var responseApi = obj.Sender(cliente, (int)ProdutoEnvio.DISTRIBUICAO);
                    if (responseApi.Result != null)
                    {
                        ResponseSendGrid responseSendGrid = JsonConvert.DeserializeObject<ResponseSendGrid>(responseApi.Result);
                        if (responseSendGrid.Success)
                        {
                            var IdMensagem = responseSendGrid.IdMensagem;

                            log.IDMensagemAPI = IdMensagem;
                            log.Status = StatusLog.ENVIADO.ToString();
                            log.IdProduto = (int)ProdutoEnvio.DISTRIBUICAO;
                            _logEnvioRepository.InserirLogEnvio(log);                            

                            // Atualizar Cliente para Pendente
                            _distribuicaoRepository.AtualizarClienteSituacaoPendente(cliente);

                            // Atualizar todos os Termos de Pesquisa para Pendente
                            _distribuicaoRepository.AtualizarTermoPesquisaSituacaoPendente(cliente);

                            // Atualizar Edita todos os Tribunais para Pendente
                            _distribuicaoRepository.AtualizarTermoPesquisaTribunalSituacaoPendente(cliente);

                            WriteLine($"*** ENVIO CLIENTE: {cliente.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                        }
                        else
                        {
                            log.Status = StatusLog.ERRO.ToString();
                            log.Observacao = responseSendGrid.ErrorMsg;
                            _logEnvioRepository.InserirLogEnvio(log);
                            WriteLine($"*** CLIENTE NÃO ENVIADO: {cliente.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                        }
                    }
                    else
                    {
                        log.Status = StatusLog.ERRO.ToString();
                        log.Observacao = "Erro na API";
                        _logEnvioRepository.InserirLogEnvio(log);
                        WriteLine($"*** CLIENTE NÃO ENVIADO: {cliente.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                    }
                }
                catch (Exception ex)
                {
                    log.Status = StatusLog.ERRO.ToString();
                    log.Observacao = ex.ToString();
                    _logEnvioRepository.InserirLogEnvio(log);
                    WriteLine($"*** CLIENTE NÃO ENVIADO: {cliente.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                }
            }
        }

        // --ATUALIZANDO OS TERMOS DE TESTE PARA PENDENTE QUANDO OS DIAS DE TESTES EXPIRAREM 
        public void AtualizarTodosTermosPesquisasParaPendente()
        {
            try
            {
                _distribuicaoRepository.AtualizarTodosTermosPesquisasClientesParaPendente();
            }
            catch (Exception)
            {
                // Salvar erro num log
                //throw;
            }    
            
        }
        private bool VerificarEnvio(string login, int IdProduto)
        {
            bool toReturn = false;
            var cliente = _logEnvioRepository.BuscarLogEnvio(login, IdProduto);

            if(cliente.Count > 0)
            {
                toReturn = true;
                WriteLine($"*** (NÃO ENVIADO) CLIENTE {cliente[0].Nome} JÁ ENVIADO ANTERIORMENTE, NA Data: {cliente[0].DataCadastro} ***");
            }

            return toReturn;
        }
    }
}
