using Envios.Especiais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Envios.Especiais.Domain.Interfaces.Repositories
{
    public interface IDistribuicaoRepository
    {
        IEnumerable<Cliente> ConsultarClienteFimTeste();
        IEnumerable<Cliente> ConsultarClienteSituacaoParaPendente();
        void AtualizarClienteSituacaoPendente(Cliente cliente);        
        void AtualizarTermoPesquisaSituacaoPendente(Cliente cliente);        
        void AtualizarTermoPesquisaTribunalSituacaoPendente(Cliente cliente);
        void AtualizarTodosTermosPesquisasClientesParaPendente();


    }
}
