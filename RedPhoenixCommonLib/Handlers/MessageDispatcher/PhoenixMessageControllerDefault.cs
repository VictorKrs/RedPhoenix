using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedPhoenix.Common.Handlers.MessageDispatcher
{
    /// <summary>
    /// Not completed
    /// </summary>
    public static class PhoenixMessageControllerDefault
    {
        #region Private fields

        #endregion

        #region Properties

        #endregion

        #region Delegates

        #endregion

        #region Methods
        #region Initializaion
        public static void Init()
        {
            PhoenixMessageDispatcherBase.CreateMainMessageEvent += CreateMainMessageDefault;
            PhoenixMessageDispatcherBase.CreateSubMessageEvent += CreateSubMessageDefault;

            PhoenixMessageDispatcherBase.SetMainMessageDataEvent += SetMainMessageDataDefault;
            PhoenixMessageDispatcherBase.SetSubMessageDataEvent += SetSubMessageDataDefault;

            PhoenixMessageDispatcherBase.SendMainMessageEvent += SendMainMessageDefault;
            PhoenixMessageDispatcherBase.SendSubMessageEvent += SendSubMessageDefault;
        }
        #endregion

        #region Default
        #region Create message object
        private static object CreateMainMessageDefault(object message, object typeOrException)
        {
            throw new NotImplementedException();
        }

        private static object CreateSubMessageDefault(object message, object typeOrException)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Set message data
        private static void SetMainMessageDataDefault(object messageObj)
        {
            throw new NotImplementedException();
        }

        private static void SetSubMessageDataDefault(object messageObj)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Send message
        private static void SendMainMessageDefault(object obj)
        {
            throw new NotImplementedException();
        }

        private static void SendSubMessageDefault(object obj)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
        #endregion
    }
}
