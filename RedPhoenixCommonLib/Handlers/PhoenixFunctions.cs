using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RedPhoenix.Common.Handlers
{
    /// <summary>
    /// Some common functions
    /// </summary>
    public static class PhoenixFunctions
    {
        #region Properties
        public static string ProgramLocation { get; } = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        #endregion

        #region Functions

        #endregion
    }
}
