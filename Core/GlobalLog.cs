using System;


namespace Core
{
    public enum LogChannel
    {
        GLOBAL_ERR,
        GLOBAL_MSG,
        NOTIFY_MSG,
        CRITICAL_ERR,
        CLR_ERR,
    }

    public static class GlobalLog
    {
        public delegate void MessageDel(string message, LogChannel channel);
        public delegate void SpecifyMessageDel(string message, string channel);
        public delegate void ExceptionDel(Exception e);
        public static event MessageDel OnErr;
        public static event MessageDel OnMessage;
        public static event SpecifyMessageDel OnChannelMessage;
        public static event ExceptionDel OnException;

        public static void Exc(Exception e)
        {
            if (OnException!=null)
            {
                OnException(e);
            }
        }

        /// <summary> Write error messageto current output  </summary>
        /// <param name="description">Can be null or empty</param>
        public static void Err(Exception e, string description = null)
        {
            if (string.IsNullOrEmpty(description))
                description = "No descrpt";

            Err(string.Format("{0}; e.Msg: {1}, e.Target: {2}.", description, e.Message, e.TargetSite));
        }

        public static void Err(string message, params object[] args)
        {
            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                Err(string.Format(message, args));
            }
            catch (Exception)
            {
                Err(string.Format("(bad format){0}", message));
            }
        }

        public static void Err(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (OnErr!=null)
            {
                OnErr(message, LogChannel.GLOBAL_ERR);
            }
        }

        public static void Write(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (OnMessage != null)
            {
                OnMessage(message, LogChannel.GLOBAL_MSG);
            }
        }

        public static void Write(string message, LogChannel channel = LogChannel.NOTIFY_MSG)
        {
            Write(message, channel.ToString());
        }

        public static void Write(string message, string channel)
        {
            if (OnChannelMessage != null)
            {
                OnChannelMessage(message, channel);
            }
        }

        public static void Write(string formattedMsg, params object[] args)
        {
            if (string.IsNullOrEmpty(formattedMsg))
                return;

            try
            {
                Write(string.Format(formattedMsg, args));
            }
            catch (Exception)
            {
                Write(string.Format("(bad format){0}", formattedMsg));
            }
        }
    }
}
