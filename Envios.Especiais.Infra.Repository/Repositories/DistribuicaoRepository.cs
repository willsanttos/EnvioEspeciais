using Envios.Especiais.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Envios.Especiais.Domain.Entities;
using Dapper;

namespace Envios.Especiais.Infra.Repository.Repositories
{
    public class DistribuicaoRepository : IDistribuicaoRepository
    {
        public IEnumerable<Cliente> ConsultarClienteFimTeste()
        {
            using (var con = DapperConnection.ConDistribuicao)
            {
                return con.Query<Cliente>($@"SELECT Nome, 
                                                    EmailCentralizador AS EmailDestinatario, 
                                                    Login,
                                                    UsuarioLogin, 
                                                    DiasTeste, 
                                                    CONVERT(DATE, DataCadastro) AS DataCadastro, 
                                                    DATEADD(DAY, DiasTeste, CONVERT(DATE, DataCadastro)) AS DataFimTeste,
                                                    'Kurier Distribuição - Seu plano de teste expira em 5 dias' AS Assunto 
                                               FROM Cliente  
                                              WHERE Situacao = 'Teste'
                                                AND DATEADD(DAY, DiasTeste, CONVERT(DATE, DataCadastro)) = DATEADD(DAY, 5, CONVERT(DATE, GETDATE())) ").ToList();
            }
        }
       
        public IEnumerable<Cliente> ConsultarClienteSituacaoParaPendente()
        {
            using (var con = DapperConnection.ConDistribuicao)
            {
                return con.Query<Cliente>($@"SELECT IDCliente,
                                                    Nome, 
                                                    EmailCentralizador AS EmailDestinatario, 
                                                    Login,
                                                    UsuarioLogin, 
                                                    DiasTeste, 
                                                    CONVERT(DATE, DataCadastro) AS DataCadastro, 
                                                    DATEADD(DAY, DiasTeste, CONVERT(DATE, DataCadastro)) AS DataFimTeste,
                                                    'Kurier Distribuição - Seu plano de teste expirou' AS Assunto 
                                               FROM Cliente  
                                              WHERE Situacao = 'Teste' 
                                                AND CONVERT(DATE, DATEADD(DAY, DiasTeste, DataCadastro), 103) <= CONVERT(DATE, GETDATE(), 103) ").ToList();
            }
        }

        public void AtualizarClienteSituacaoPendente(Cliente cliente)
        {
            using (var con = DapperConnection.ConDistribuicao)
            {                
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        con.Execute($@"UPDATE Cliente  
                                          SET Situacao = 'Pendente',
                                              UsuarioLogin = @Login,
                                              DataUltimaModificacao = {DateTime.Now}
                                        WHERE IDCliente = @IDCliente", cliente, transaction);

                        con.Execute($@"INSERT INTO ClienteHistorico  
                                       SELECT IDCliente, Login, Nome, EmailUnico, EmailCentralizador, RepetirProcessos, FormatoEnvio, MeioEnvio, EnderecoFTP_WS, Situacao,DiasTeste,DataCadastro,UsuarioLogin,DataUltimaModificacao,EnviarDataHoraAudienciaSeparado,RecuperarIniciais,TodosTribunais,
                                              ApenasComInicial,DataMinimaDistribuicao,QtdDiasUteisDistribuicao,QtdDiasUteisInicial,ReprocessarDistribuicao,EnvioMultiplo,ExcelAbaUnica,LoteEnvio1,LoteEnvio2,LoteEnvio3,LoginFaturamento 
                                         FROM Cliente WHERE IDCliente = @IDCliente", cliente, transaction);

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
                
        public void AtualizarTermoPesquisaSituacaoPendente(Cliente cliente)
        {
            using (var con = DapperConnection.ConDistribuicao)
            {                
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        con.Execute($@"UPDATE TermoPesquisa  
	                                      SET Situacao = 'Pendente',  
		                                      UsuarioLogin = @UsuarioLogin,  
		                                      DataUltimaModificacao = {DateTime.Now}  
                                        WHERE IDCliente = @IDCliente ", cliente, transaction: transaction);

                        con.Execute($@"INSERT INTO TermoPesquisaHistorico  
                                       SELECT * FROM TermoPesquisa WHERE IDCliente = @IDCliente", cliente, transaction: transaction);

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
        
        public void AtualizarTermoPesquisaTribunalSituacaoPendente(Cliente cliente)
        {
            using (var con = DapperConnection.ConDistribuicao)
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        con.Execute($@"UPDATE TermoPesquisa_Tribunal  
                                          SET Situacao = 'Pendente',  
                                              UsuarioLogin = @UsuarioLogin,  
                                              DataUltimaModificacao = {DateTime.Now}
                                        WHERE IDCliente = @IDCliente ", cliente, transaction: transaction);

                        con.Execute($@"INSERT INTO TermoPesquisa_TribunalHistorico  
                                       SELECT * FROM TermoPesquisa_Tribunal WHERE IDCliente = @IDCliente", cliente, transaction: transaction);

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

        public void AtualizarTodosTermosPesquisasClientesParaPendente()
        {
            using (var con = DapperConnection.ConDistribuicao)
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        con.Execute($@"UPDATE TermoPesquisa_Tribunal 
                                          SET Situacao = 'Pendente'  
                                        WHERE Situacao = 'Teste'  
                                          AND CONVERT(DATE, DATEADD(DAY, DiasTeste, DataCadastro), 103) < CONVERT(DATE, GETDATE(), 103) ", transaction: transaction);

                        con.Execute($@"UPDATE TermoPesquisa
                                          SET Situacao = 'Pendente'  
                                        WHERE Situacao = 'Teste'  
                                          AND CONVERT(DATE, DATEADD(DAY, DiasTeste, DataCadastro), 103) < CONVERT(DATE, GETDATE(), 103) ", transaction: transaction);

                        con.Execute($@"UPDATE Cliente
                                          SET Situacao = 'Pendente'  
                                        WHERE Situacao = 'Teste'  
                                          AND CONVERT(DATE, DATEADD(DAY, DiasTeste, DataCadastro), 103) < CONVERT(DATE, GETDATE(), 103)   ", transaction: transaction);

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
    }
}
