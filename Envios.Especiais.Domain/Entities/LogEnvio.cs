using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Envios.Especiais.Domain.Entities
{
    public class LogEnvio
    {
        public int IdLogEnvio { get; set; }
        public int IdProduto { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Status { get; set; }
        public string Observacao { get; set; }
        public DateTime DataFimTeste { get; set; }
        public DateTime DataHoraRegistro { get; set; }
        public int? IDMensagemAPI { get; set; }
    }
}
