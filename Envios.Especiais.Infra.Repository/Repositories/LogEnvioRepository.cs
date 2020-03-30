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
    public class LogEnvioRepository : ILogEnvioRepository
    {
        public void InserirLogEnvio(LogEnvio log)
        {
            using (var con = DapperConnection.ConTeste)
            {
                string sql = @"INSERT INTO LogEnviosEspeciais
                                           ( IdProduto
                                           , Login
                                           , Email
                                           , Nome
                                           , Status
                                           , Observacao
                                           , DataFimTeste
                                           , DataHoraRegistro
                                           , IDMensagemAPI)
                                     VALUES
                                           (@IdProduto,
                                            @Login,
                                            @Email,
                                            @Nome,
                                            @Status,
                                            @Observacao,
                                            @DataFimTeste,
                                            @DataHoraRegistro,
                                            @IDMensagemAPI)
                                     SELECT CAST(SCOPE_IDENTITY() as int)";

                log.IdLogEnvio = con.Query<int>(sql, log).Single();
            }
        }

        public List<Cliente> BuscarLogEnvio(string login, int IdProduto)
        {
            using (var con = DapperConnection.ConTeste)
            {
                return con.Query<Cliente>($@"SELECT * 
                                               FROM LogEnviosEspeciais
                                              WHERE Login = '{login}'
                                                AND IdProduto = {IdProduto}
                                                AND CONVERT(DATE, DataHoraRegistro) = CONVERT(DATE, getdate())").ToList();
            }
        }
    }
}
