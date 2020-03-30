using Envios.Especiais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Envios.Especiais.Domain.Interfaces.Repositories
{
    public interface ILogEnvioRepository
    {
        void InserirLogEnvio(LogEnvio logEnvio);

        List<Cliente> BuscarLogEnvio(string login, int IdProduto);
    }
}
