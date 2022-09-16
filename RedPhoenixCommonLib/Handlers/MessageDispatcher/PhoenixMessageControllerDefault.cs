using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedPhoenix.Common.Handlers.MessageDispatcher
{
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

        

        private static void SetMainMessageDataDefault(object obj)
        {
            throw new NotImplementedException();
        }

        private static void SetSubMessageDataDefault(object obj)
        {
            throw new NotImplementedException();
        }

        private static void SendMainMessageDefault(object obj)
        {
            throw new NotImplementedException();
        }

        private static void SendSubMessageDefault(object obj)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Default
        #region Create message object
        private static object CreateMainMessageDefault(object message, object typeOrException)
        {
            throw new NotImplementedException();
        }

        private static object CreateSubMessageDefault(object arg1, object arg2)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
        #endregion
    }
}
