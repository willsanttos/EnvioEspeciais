using Dapper;
using Envios.Especiais.Domain.Entities;
using Envios.Especiais.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Envios.Especiais.Infra.Repository.Repositories
{
    public class JuridicoRepository : IJuridicoRepository
    {

        public IEnumerable<Cliente> ConsultarClienteFimTeste()
        {
            using (var con = DapperConnection.Con)
            {
                return con.Query<Cliente>($@"SELECT Nome, 
                                                    e_mail AS EmailDestinatario, 
                                                    Login,                                                    
                                                    --DiasTeste,                                                    
                                                    CONVERT(varchar,dataInicioTeste + diasDeTeste, 103) as DataFimTeste,
                                                    'Kurier Juridíco - Seu plano de teste expirou' AS Assunto 
                                               FROM Cliente  
                                              WHERE flag = 'T'
                                                AND CONVERT(datetime,dataInicioTeste + diasDeTeste, 103) <= CONVERT(datetime, GETDATE(), 103) ").ToList();
            }
        }


        public void AtualizarClienteSituacaoPendente(Cliente cliente)
        {
            using (var con = DapperConnection.Con)
            {
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        con.Execute($@"UPDATE cliente 
                                          SET usuario = 'kurier', 
	                                          dataAlteracao = CONVERT(DATETIME, {DateTime.Now}, 103), 
		                                      flag = 'P'
                                        WHERE IDCliente = @IDCliente", cliente, transaction);

                        con.Execute($@"UPDATE Nomes_Pesquisa
                                          SET USUARIO = 'kurier',  
                                                 FLAG = 'P',  
                                              DATAALTERACAO = CONVERT(DATETIME,(CONVERT(VARCHAR, GETDATE(), 103)), 103)  
                                        WHERE IdCliente = @IDCliente  
                                          AND Nomes_Pesquisa.Flag <> 'E'", cliente, transaction);

                        con.Execute($@"UPDATE Nomes_Dv_diario   
                                          SET flag = 'P',  
                                              data_alteracao = CONVERT(datetime,(CONVERT(VARCHAR, GETDATE(), 103)), 103),  
                                              usuario = 'kurier'  
                                         FROM Cliente, Nomes_pesquisa, Nomes_Dv_diario  
                                        WHERE Cliente.IdCliente = Nomes_pesquisa.IdCliente  
                                          AND Nomes_pesquisa.IdNomePesquisa = Nomes_Dv_Diario.IdNomePesquisa  
                                          AND Nomes_Dv_Diario.Flag <> 'E'  
                                          AND Cliente.IdCliente = @IDCLIENTE", cliente, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public IEnumerable<Cliente> JuridicoMeridioQdtPublicacoesNaoCapturadas()
        {
            using (var con = DapperConnection.Con)
            {
                return con.Query<Cliente>($@"SELECT c.idCliente, c.login, c.nome, CONVERT(varchar,p.data,105) DataPublicacao, COUNT(*) QtdPublicacao  
                                               FROM cliente c, Nomes_pesquisa np, Pesquisa p 
                                              WHERE c.Idcliente = np.idcliente 
                                                AND np.idnomepesquisa = p.idnomepesquisa   
                                                AND p.data >= (GETDATE() - 5) and p.flag = 'A'
                                                AND p.enviado = 'S' and p.enviadoWebService = 'N' 
	                                            AND c.flag = 'PRD' and np.Flag <> 'E' and c.Integracao = 'meridio'  
                                              GROUP BY c.idCliente, c.login, c.nome, p.data  
                                              ORDER by c.nome, p.data").ToList();
            }
        }
    }
}
