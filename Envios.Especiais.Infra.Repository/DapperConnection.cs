using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace Envios.Especiais.Infra.Repository
{
    public class DapperConnection
    {
        public static SqlConnection ConTeste
        {
            get { return new SqlConnection(ConfigurationManager.ConnectionStrings["KurierTeste"].ConnectionString); }
        }

        public static SqlConnection Con
        {
            get { return new SqlConnection(ConfigurationManager.ConnectionStrings["Kurier"].ConnectionString); }
        }

        public static SqlConnection ConDistribuicao
        {
            get { return new SqlConnection(ConfigurationManager.ConnectionStrings["KurierDistribuicao"].ConnectionString); }
        }

        public static SqlConnection ConTribunal
        {
            get { return new SqlConnection(ConfigurationManager.ConnectionStrings["KurierTribunal"].ConnectionString); }
        }

        public static SqlConnection ConBiju
        {
            get { return new SqlConnection(ConfigurationManager.ConnectionStrings["KurierBiju"].ConnectionString); }
        }

        public static SqlConnection Andamento
        {
            get { return new SqlConnection(ConfigurationManager.ConnectionStrings["KurierAndamento"].ConnectionString); }
        }
    }
}
