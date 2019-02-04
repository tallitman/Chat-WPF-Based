namespace ILogger
{
    public class Logger                     //singletone creation for logger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Logger instance;
        private Logger() { }
        public static Logger Instance       //the instance of the logger
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger();
                }
                return instance;
            }
        }
        /// <summary>
        /// events that happen following the system process.
        /// </summary>
        /// <param name="message"></param>
        public void logInfoMessage(string message)
        {
            log.Info(message);
        }
        /// <summary>
        ///errors that appear when system is not working properly.
        /// </summary>
        /// <param name="message"></param>
        public void logErrorMessage(string message)
        {
            log.Error(message);
        }
        /// <summary>
        /// errors that crash the system
        /// </summary>
        /// <param name="message"></param>
        public void logFatalMessage(string message)
        {
            log.Fatal(message);
        }
        /// <summary>
        /// warnings that occur due to user misuse.
        /// </summary>
        /// <param name="message"></param>
        public void logWarnMessage(string message)
        {
            log.Warn(message);
        }
    }
}
