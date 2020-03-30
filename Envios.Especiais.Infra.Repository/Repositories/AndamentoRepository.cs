using Envios.Especiais.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;
using Envios.Especiais.Domain.Entities;
using Dapper;

namespace Envios.Especiais.Infra.Repository.Repositories
{
    public class AndamentoRepository : IAndamentoRepository
    {
        public IEnumerable<Cliente> AndamentoMeridioQdtPublicacoesNaoCapturadas()
        {
            using (var con = DapperConnection.Andamento)
            {
                return con.Query<Cliente>($@"SELECT cu.IdClienteUnidade AS IdCliente, 
                                                    cu.UsuarioWebService AS Login,
	                                                c.Nome, 
	                                                count(*) QtdPublicacao  
                                               FROM Cliente c  
                                              INNER JOIN ClienteUnidade cu ON cu.IdCliente = c.IdCliente  
                                              INNER JOIN ProcessoAndamento pa ON pa.IdClienteUnidade = cu.IdClienteUnidade   
                                              WHERE c.SistemaUtilizado = 'Meridio' 
                                                AND c.IdStatusCliente in (1,2)   
                                                AND pa.EnviadoWS = 0
                                              GROUP BY cu.IdClienteUnidade, cu.UsuarioWebService, c.nome  
                                              ORDER BY c.nome").ToList();
            }
        }
    }
}
