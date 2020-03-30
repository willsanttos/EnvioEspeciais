using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Envios.Especiais.Domain.Entities
{
    public class ResponseSendGrid
    {
        public bool Success { get; set; }
        public int? IdMensagem { get; set; }
        public string ErrorMsg { get; set; }
    }
}
