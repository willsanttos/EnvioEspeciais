using Envios.Especiais.Domain.Interfaces.Services;
using Envios.Especiais.Infra.DependencyResolver;
using Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Envios.Especiais.Service
{
    partial class Service : ServiceBase
    {
        private Timer timer1 = null;
        private bool seviceStart = false;
        private StandardKernel kernel;
        private IControllerService controller;
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            kernel = new StandardKernel(new ServiceModule());
            controller = kernel.Get<IControllerService>();
            timer1 = new Timer();
            this.timer1.Interval = 1800000; //30 Minutos
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.Executar);
            timer1.Enabled = true;
            Library.WriteErroLog("Serviço iniciado");
        }

        private void Executar(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!seviceStart)
                {
                    seviceStart = true;
                    Library.WriteErroLog($"Iniciando Envio");
                    controller.Executar();
                    Library.WriteErroLog($"Envio Finalizado");
                    seviceStart = false;
                }
            }
            catch (Exception ex)
            {
                seviceStart = false;
                Library.WriteErroLog(ex);
            }
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.WriteErroLog("Serviço finalizado");
        }
    }
}
