using System;
using System.IO;

namespace GameEngine
{
    //Ben Mullenger
    enum LogMessageType
    {
        Update = 0,
        Error = 1,
        FatalError = 2
    }
    /// <summary>
    /// Class for writting simple log messages
    /// </summary>
    static class Logger
    {
        private static string Time
        {
            get
            {
                return "Time: " + DateTime.Now.TimeOfDay.ToString();
            }
        }
        private static string Day
        {
            get
            {
                return "Day: " + DateTime.Now.Date.ToString();
            }
        }
        private static StreamWriter m_writer;
        private static string m_logName;

        private static string[] m_messageTypes = new string[] { " **ERROR** ", " >>Update<< ", "!!!FATAL_ERROR!!!" }; //used to write special message descriptions

        private static readonly string LOGGER_LOCATION = Directory.GetCurrentDirectory() + "/Logs/";

        /// <summary>
        /// Makes sure that the file exists and sets up the StreamWriter
        /// </summary>
        public static void Initialise()
        {
            DateTime now = DateTime.Now;
            m_logName = now.Day.ToString() + "_" + now.Month.ToString() + "_" + now.Year.ToString() + "__" + now.Hour + "_" + now.Minute + "_" + now.Second;

            if (!File.Exists(LOGGER_LOCATION + m_logName))
            {
                Directory.CreateDirectory(LOGGER_LOCATION);
                m_writer = new StreamWriter(LOGGER_LOCATION + m_logName, true);
                m_writer.WriteLine("Day: " + DateTime.Now.Date.ToString() + " :__:  " + Time + " -- Began Program");
            }
        }
        /// <summary>
        /// Write simple messgae to the log file
        /// </summary>
        /// <param name="p_type">Describes the description that will be placed before your message</param>
        /// <param name="p_message"></param>
        public static void Write(LogMessageType p_type, string p_message)
        {
            m_writer.WriteLine(Time + m_messageTypes[(int)p_type] + p_message);

            if (p_type == LogMessageType.FatalError)
            {
                Close();
                throw new Exception(p_message);
            }
        }
        /// <summary>
        /// Closes the StreamWriter
        /// </summary>
        public static void Close()
        {
            m_writer.WriteLine(Time + " -- Closed Program");
            m_writer.Close();
        }
    }
}
