using System;
using System.Collections.Generic;

namespace Envios.Especiais.Domain.Entities
{
    public class Cliente
    {
        public int IDCliente { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string UsuarioLogin { get; set; }
        public int DiasTeste { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataFimTeste { get; set; }
        public string EmailDestinatario { get; set; }
        public List<string> EmailCopiaOculta { get; set; }
        public string CorpoMensagem { get; set; }
        public string Assunto { get; set; }
        public DateTime DataPublicacao { get; set; }
        public int QtdPublicacao { get; set; }
    }
}
