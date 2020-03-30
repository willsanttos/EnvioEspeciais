using Envios.Especiais.Domain.Interfaces.Repositories;
using Envios.Especiais.Domain.Interfaces.Services;
using System;
using static System.Console;
using System.Linq;
using Envios.Especiais.Domain.Entities;
using Envios.Especiais.Domain.Enums;
using Newtonsoft.Json;

namespace Envios.Especiais.Infra.Service.Services.Envio
{
    public class EnvioAndamento : IAndamentoService
    {
        private readonly IAndamentoRepository _andamentoRepository;
        private readonly ILogEnvioRepository _logEnvioRepository;
        public EnvioAndamento(IAndamentoRepository andamentoRepository, ILogEnvioRepository logEnvioRepository)
        {
            _andamentoRepository = andamentoRepository;
            _logEnvioRepository = logEnvioRepository;
        }
        public void EnvioAndamentoMeridioQdtPublicacoesNaoCapturadas()
        {
            string textoEmail = string.Empty;
            LogEnvio log = new LogEnvio
            {
                Login = "Andamento.Meridio",
                Email = "consultores.meridio@kurier.com.br;evelyn.enir@kurier.com.br;Silvio.araujo@kurier.com.br",
                Nome = "Kurier - Andamento",
                DataFimTeste = DateTime.Now,
                DataHoraRegistro = DateTime.Now,
            };
            try
            {
                if (!VerificarEnvio("Andamento.Meridio", (int)ProdutoEnvio.ANDAMENTO))
                {
                    Cliente cliente = new Cliente
                    {
                        EmailDestinatario = "consultores.meridio@kurier.com.br;evelyn.enir@kurier.com.br;jonathan.rabelo@kurier.com.br;Silvio.araujo@kurier.com.br",
                        Assunto = $"k-Andamento, Quantidade de andamentos NÃO capturados no dia {DateTime.Now.ToShortDateString()}"
                    };
                    var publicacoes = _andamentoRepository.AndamentoMeridioQdtPublicacoesNaoCapturadas().ToList();

                    if (publicacoes.Count > 0)
                    {
                        foreach (var pub in publicacoes)
                        {
                            textoEmail += "<tr>";
                            textoEmail += $"<td><font size=2><b>{pub.IDCliente}</b></font></td>";
                            textoEmail += $"<td><font size=2>{pub.Login}</font></td>";
                            textoEmail += $"<td><font size=2>{pub.Nome}</font></td>";                            
                            textoEmail += $"<td><font size=2>{pub.QtdPublicacao}</font></td>";
                            textoEmail += $"</tr>";
                        }
                    }

                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

                    htmlDoc.LoadHtml(Resource.EmailAndamentoMeridioQdtPublicacoesNaoCapturadas);

                    htmlDoc.DocumentNode.SelectSingleNode("//span[@id='textoEmail']").InnerHtml = textoEmail;


                    cliente.CorpoMensagem = htmlDoc.DocumentNode.OuterHtml;

                    SendGrid obj = new SendGrid();
                    var responseApi = obj.Sender(cliente, (int)ProdutoEnvio.ANDAMENTO);
                    if (responseApi.Result != null)
                    {
                        ResponseSendGrid responseSendGrid = JsonConvert.DeserializeObject<ResponseSendGrid>(responseApi.Result);
                        if (responseSendGrid.Success)
                        {
                            var IdMensagem = responseSendGrid.IdMensagem;

                            log.IDMensagemAPI = IdMensagem;
                            log.Status = StatusLog.ENVIADO.ToString();
                            log.IdProduto = (int)ProdutoEnvio.ANDAMENTO;

                            _logEnvioRepository.InserirLogEnvio(log);

                            WriteLine($"*** ENVIO CLIENTE: {log.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                        }
                        else
                        {
                            log.Status = StatusLog.ERRO.ToString();
                            log.Observacao = responseSendGrid.ErrorMsg;
                            _logEnvioRepository.InserirLogEnvio(log);
                            WriteLine($"*** CLIENTE NÃO ENVIADO: {log.Nome} Data: {DateTime.Now.ToLongTimeString()} ***");
                        }
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

            if (cliente.Count > 0)
            {
                toReturn = true;
                WriteLine($"*** (NÃO ENVIADO) CLIENTE {cliente[0].Nome} JÁ ENVIADO ANTERIORMENTE, NA Data: {cliente[0].DataCadastro} ***");
            }

            return toReturn;
        }
    }
}
