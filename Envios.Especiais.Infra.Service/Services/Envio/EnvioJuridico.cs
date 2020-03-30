using Envios.Especiais.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using Envios.Especiais.Domain.Entities;
using Envios.Especiais.Domain.Interfaces.Repositories;
using Envios.Especiais.Domain.Enums;
using Newtonsoft.Json;

namespace Envios.Especiais.Infra.Service.Services.Envio
{
    public class EnvioJuridico : IJuridicoService
    {
        private readonly IJuridicoRepository _juridicoRepository;
        private readonly ILogEnvioRepository _logEnvioRepository;
        public EnvioJuridico(IJuridicoRepository juridicoRepository, ILogEnvioRepository logEnvioRepository)
        {
            _juridicoRepository = juridicoRepository;
            _logEnvioRepository = logEnvioRepository;
        }

        public void EnvioJuridicoSituacaoPendente(Cliente cliente)
        {
            if (!VerificarEnvio(cliente.Login, (int)ProdutoEnvio.JURIDICO))
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
                    htmlDoc.DocumentNode.SelectSingleNode("//span[@id='DataFimTeste']").InnerHtml = cliente.DataFimTeste.ToShortDateString();

                    cliente.CorpoMensagem = htmlDoc.DocumentNode.OuterHtml;

                    SendGrid obj = new SendGrid();
                    var responseApi = obj.Sender(cliente, (int)ProdutoEnvio.JURIDICO);
                    if (responseApi.Result != null)
                    {
                        ResponseSendGrid responseSendGrid = JsonConvert.DeserializeObject<ResponseSendGrid>(responseApi.Result);
                        if (responseSendGrid.Success)
                        {
                            var IdMensagem = responseSendGrid.IdMensagem;

                            log.IDMensagemAPI = IdMensagem;
                            log.Status = StatusLog.ENVIADO.ToString();
                            log.IdProduto = (int)ProdutoEnvio.JURIDICO;

                            _juridicoRepository.AtualizarClienteSituacaoPendente(cliente);

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

        public void EnvioJuridicoMeridioQdtPublicacoesNaoCapturadas()
        {
            string textoEmail = string.Empty;
            LogEnvio log = new LogEnvio
            {
                Login = "silvio.brayner.base",
                Email = "consultores.meridio@kurier.com.br;evelyn.enir@kurier.com.br;jonathan.rabelo@kurier.com.br;Silvio.araujo@kurier.com.br",
                Nome = "Kurier - Meridio",
                DataFimTeste = DateTime.Now,
                DataHoraRegistro = DateTime.Now,
            };
            try
            {
                //if (!VerificarEnvio("silvio.brayner.base", (int)ProdutoEnvio.JURIDICO))
                //{
                    Cliente cliente = new Cliente
                    {
                        EmailDestinatario = "consultores.meridio@kurier.com.br;evelyn.enir@kurier.com.br;jonathan.rabelo@kurier.com.br;Silvio.araujo@kurier.com.br",
                        Assunto = $"k-Jurídico, Quantidade de publicações NÃO capturadas no dia {DateTime.Now.ToShortDateString()}"
                    };
                    var publicacoes = _juridicoRepository.JuridicoMeridioQdtPublicacoesNaoCapturadas().ToList();

                    if (publicacoes.Count > 0)
                    {
                        foreach (var pub in publicacoes)
                        {
                            textoEmail += "<tr>";
                            textoEmail += $"<td><font size=2><b>{pub.IDCliente}</b></font></td>";
                            textoEmail += $"<td><font size=2>{pub.Login}</font></td>";
                            textoEmail += $"<td><font size=2>{pub.Nome}</font></td>";
                            textoEmail += $"<td><font size=2>{pub.DataPublicacao.ToShortDateString()}</font></td>";
                            textoEmail += $"<td><font size=2>{pub.QtdPublicacao}</font></td>";
                            textoEmail += $"</tr>";
                        }
                    }

                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

                    htmlDoc.LoadHtml(Resource.EmailJuridicoMeridioQdtPublicacoesNaoCapturadas);

                    htmlDoc.DocumentNode.SelectSingleNode("//span[@id='textoEmail']").InnerHtml = textoEmail;


                    cliente.CorpoMensagem = htmlDoc.DocumentNode.OuterHtml;

                    SendGrid obj = new SendGrid();
                    var responseApi = obj.Sender(cliente, (int)ProdutoEnvio.JURIDICO);
                    if (responseApi.Result != null)
                    {
                        ResponseSendGrid responseSendGrid = JsonConvert.DeserializeObject<ResponseSendGrid>(responseApi.Result);
                        if (responseSendGrid.Success)
                        {
                            var IdMensagem = responseSendGrid.IdMensagem;

                            log.IDMensagemAPI = IdMensagem;
                            log.Status = StatusLog.ENVIADO.ToString();
                            log.IdProduto = (int)ProdutoEnvio.JURIDICO;

                            _logEnvioRepository.InserirLogEnvio(log);

                            WriteLine($"*** ENVIO CLIENTE: {log.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                        //}
                        //else
                        //{
                        //    log.Status = StatusLog.ERRO.ToString();
                        //    log.Observacao = responseSendGrid.ErrorMsg;
                        //    _logEnvioRepository.InserirLogEnvio(log);
                        //    WriteLine($"*** CLIENTE NÃO ENVIADO: {log.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                        //}
                    }
                    else
                    {
                        log.Status = StatusLog.ERRO.ToString();
                        log.Observacao = "Erro na API";
                        _logEnvioRepository.InserirLogEnvio(log);
                        WriteLine($"*** CLIENTE NÃO ENVIADO: {log.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Status = StatusLog.ERRO.ToString();
                log.Observacao = ex.ToString();
                _logEnvioRepository.InserirLogEnvio(log);
                WriteLine($"*** CLIENTE NÃO ENVIADO: {log.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
            }

        }

        private bool VerificarEnvio(string login, int IdProduto)
        {
            bool toReturn = false;
            var cliente = _logEnvioRepository.BuscarLogEnvio(login, IdProduto).ToList();

            if (cliente.Count() > 0)
            {
                toReturn = true;
                WriteLine($"*** (NÃO ENVIADO) CLIENTE {cliente[0].Nome} JÁ ENVIADO ANTERIORMENTE, NA Data: {cliente[0].DataCadastro} ***");
            }

            return toReturn;
        }   
        
        private List<Cliente> ConsultarLogEnvio(string login, int IdProduto)
        {            
            return _logEnvioRepository.BuscarLogEnvio(login, IdProduto);            
        }     
    }
}
