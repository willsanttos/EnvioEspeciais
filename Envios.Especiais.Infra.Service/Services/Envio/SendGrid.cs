using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Envios.Especiais.Domain.Entities;

namespace Envios.Especiais.Infra.Service.Services.Envio
{
    public class SendGrid
    {
        public readonly static string urlApiSenderGrid = "http://smtpsendgrid.kurierservicos.com.br:8084/postreceiverapi/api/new/message";
        
        public async Task<string> Sender(Cliente cliente, int idProduto)
        {

            List<ObjetoArquivoSendGrid> ObjetosArquivoSendGrid = new List<ObjetoArquivoSendGrid>();
            var responseJson = "";

            var corpoRequisicao = JsonConvert.SerializeObject(new
            {
                idProduto = idProduto,
                destinatarioMensagem = "william.santos@kurier.com.br;Silvio.araujo@kurier.com.br",//string.Join(";", cliente.EmailDestinatario),
                copiaMensagem = "",
                copiaOcultaDestinatario = "", 
                assuntoMensagem = cliente.Assunto,
                corpoMensagem = cliente.CorpoMensagem,
                anexos = ObjetosArquivoSendGrid
            });

            var conteudo = new StringContent(
                corpoRequisicao,
                Encoding.UTF8,
                "application/json");

            var teste = JsonConvert.SerializeObject(conteudo);

            var client = new HttpClient();
            var response = client.PostAsync(urlApiSenderGrid, conteudo).Result;

            if (response.IsSuccessStatusCode)
            {
                Stream receiveStream = await response.Content.ReadAsStreamAsync();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                responseJson = readStream.ReadToEnd();
            }
            return responseJson;
        }
    }

    public class ObjetoArquivoSendGrid
    {
        public string NomeCompletoArquivo { get; set; }
        public string ConteudoArquivoBase64 { get; set; }
    }
}
