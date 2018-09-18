using System;
using System.ServiceProcess;
using System.Timers;

namespace TWIConnect.Client.Service
{
    public partial class Service : ServiceBase
    {
        private const int defaultIntervalSec = 5;
        private System.Timers.Timer _timer;
        private System.Timers.Timer Timer
        {
            get
            {
                if (this._timer == null)
                {
                    this._timer = new Timer(defaultIntervalSec * 1000);
                }
                return this._timer;
            }
        }

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                this.Timer.Elapsed += new ElapsedEventHandler(Process);
                this.Timer.Enabled = true;
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
                throw ex;
            }
        }

        protected override void OnStop()
        {
        }

        internal void Process(object source, ElapsedEventArgs e)
        {
            try
            {
                Configuration configuration = Configuration.Load();
                //TWIConnect.Client.Processor.Run(configuration);
                //this.Timer.Interval = ((configuration != null) ? configuration.ScheduledIntervalSec : defaultIntervalSec) * 1000;
            }
            catch
            {
                //No action on failure - retry later
            }
        }
    }
}
