using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Envios.Especiais.Service
{
    public static class Library
    {
        public static void WriteErroLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"LogFile.txt", true);
                sw.WriteLine($"{DateTime.Now.ToString()}: {ex.Source.ToString().Trim()}; {ex.Message.ToString().Trim()}");
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void WriteErroLog(string message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"LogFile.txt", true);
                sw.WriteLine($"{DateTime.Now.ToString()}: {message}");
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
