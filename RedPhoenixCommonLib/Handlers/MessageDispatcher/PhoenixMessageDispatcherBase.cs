using RedPhoenix.Common.Handlers.MessageDispatcher.Enums;
using System;

namespace RedPhoenix.Common.Handlers.MessageDispatcher
{
    /// <summary>
    /// Static class that organizes logs formation system.
    /// </summary>
    public static class PhoenixMessageDispatcherBase 
    {
        #region Events
        //Create message object
        /// <summary>
        /// Parameters: message (string or some object), message type or exception; Result: message object
        /// </summary>
        public static event Func<object, object, object> CreateMainMessageEvent;
        /// <summary>
        /// Parameters: message (string or some object), message type or exception; Result: message object
        /// </summary>
        public static event Func<object, object, object> CreateSubMessageEvent;

        // Set data to message object
        /// <summary>
        /// Parameters: message object
        /// </summary>
        public static event Action<object> SetMainMessageDataEvent;
        /// <summary>
        /// Parameters: message object
        /// </summary>
        public static event Action<object> SetSubMessageDataEvent;

        // Send completed message
        /// <summary>
        /// Parameters: message object
        /// </summary>
        public static event Action<object> SendMainMessageEvent;
        /// <summary>
        /// Parameters: message object
        /// </summary>
        public static event Action<object> SendSubMessageEvent;

        // Add message info - 
        //  need for saving message info (filename, string number, some field name etc.) in static container class
        /// <summary>
        /// Parameters: message object
        /// </summary>
        public static event Action<object> AddMessageInfoEvent;
        public static event Action<string, object> AddMessageInfoByNameEvent;
        /// <summary>
        /// Parameters: message object
        /// </summary>
        public static event Action<object> AddSubMessageInfoEvent;
        public static event Action<string, object> AddSubMessageInfoByNameEvent;
        #endregion

        #region Methods
        #region Create and send message
        /// <summary>Main method for forming a message.
        /// Consistently calls events for getting message object (CreateMainMessageEvent/CreateSubMessageEvent),
        ///     setting message data (SetMessageDataEvent/SetSubMessageDataEvent) and 
        ///     sending message (SendMainMessageEvent/SendSubMessageEvent)
        /// </summary>
        /// <param name="message">string or object</param>
        /// <param name="mesObjType">Main or submessage</param>
        /// <param name="typeOrException">Message type (error, warning, good) or exception (type is error)</param>
        public static void CreateAndSendMessage(object message, MessageObjectType mesObjType, object typeOrException)
        {
            try
            {
                object messObj = GetMessObject(message, mesObjType, typeOrException);

                SetMessageData(messObj, mesObjType);

                SendMessage(messObj, mesObjType);
            }
            catch (Exception) { throw; }
        }

        private static object GetMessObject(object message, MessageObjectType messObjType, object typeOrException)
        {
            object result = null;

            switch (messObjType)
            {
                case MessageObjectType.Main:
                    result = CreateMainMessageEvent?.Invoke(message, typeOrException);
                    break;
                case MessageObjectType.SubMessage:
                    result = CreateSubMessageEvent?.Invoke(message, typeOrException);
                    break;
            }

            if (result == null) throw new Exception("Error create message object; Message object type: " + messObjType.ToString());

            return result;
        }

        private static void SetMessageData(object messObj, MessageObjectType direct)
        {
            if (direct == MessageObjectType.Main)
                SetMainMessageDataEvent?.Invoke(messObj);
            else
                SetSubMessageDataEvent?.Invoke(messObj);
        }

        private static void SendMessage(object messObj, MessageObjectType direct)
        {
            switch (direct)
            {
                case MessageObjectType.Main:
                    SendMainMessageEvent?.Invoke(messObj);
                    break;
                case MessageObjectType.SubMessage:
                    SendSubMessageEvent?.Invoke(messObj);
                    break;
            }
        }

        #region Default implementations
        public static void CreateAndSendMainMessage(object message, object typeOrException) =>
            CreateAndSendMessage(message, MessageObjectType.Main, typeOrException);

        public static void CreateAndSendSubMessage(object message, object typeOrException) =>
            CreateAndSendMessage(message, MessageObjectType.SubMessage, typeOrException);

        #region Main
        #region Notification
        public static void CrAndS_E_M_N(object message) =>
            CreateAndSendMainMessage(message, DefaultMessageTypes.ErrorNotif);
        public static void CrAndS_W_M_N(object message) =>
            CreateAndSendMainMessage(message, DefaultMessageTypes.WarningNotif);
        public static void CrAndS_G_M_N(object message) =>
            CreateAndSendMainMessage(message, DefaultMessageTypes.GoodNotif);
        #endregion

        #region Log
        public static void CrAndS_E_M_L(object message) =>
            CreateAndSendMainMessage(message, DefaultMessageTypes.ErrorLog);
        public static void CrAndS_W_M_L(object message) =>
            CreateAndSendMainMessage(message, DefaultMessageTypes.WarningLog);
        public static void CrAndS_G_M_L(object message) =>
            CreateAndSendMainMessage(message, DefaultMessageTypes.GoodLog);
        #endregion
        #endregion

        #region Sub
        #region Notification
        public static void CrAndS_E_S_N(object message) =>
            CreateAndSendSubMessage(message, DefaultMessageTypes.ErrorNotif);
        public static void CrAndS_W_S_N(object message) =>
            CreateAndSendSubMessage(message, DefaultMessageTypes.WarningNotif);
        public static void CrAndS_G_S_N(object message) =>
            CreateAndSendSubMessage(message, DefaultMessageTypes.GoodNotif);
        #endregion

        #region Log
        public static void CrAndS_E_S_L(object message) =>
            CreateAndSendSubMessage(message, DefaultMessageTypes.ErrorLog);
        public static void CrAndS_W_S_L(object message, string threadId = null) =>
            CreateAndSendSubMessage(message, DefaultMessageTypes.WarningLog);
        public static void CrAndS_G_S_L(object message, string threadId = null) =>
            CreateAndSendSubMessage(message, DefaultMessageTypes.GoodLog);
        #endregion
        #endregion
        #endregion
        #endregion

        #region AddMessageInfo
        public static void AddMessageInfo(object messInfo) => AddMessageInfoEvent?.Invoke(messInfo);
        public static void AddMessageInfo(string name, object messInfo) => AddMessageInfoByNameEvent?.Invoke(name, messInfo);

        public static void AddSubMessageInfo(object messInfo) => AddSubMessageInfoEvent?.Invoke(messInfo);
        public static void AddSubMessageInfo(string name, object messInfo) => AddSubMessageInfoByNameEvent?.Invoke(name, messInfo);
        #endregion
        #endregion
    }
}
