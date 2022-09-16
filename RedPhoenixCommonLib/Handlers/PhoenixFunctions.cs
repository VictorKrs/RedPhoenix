using System.IO;

namespace RedPhoenix.Common.Handlers
{
    /// <summary>
    /// Some common useful functions
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
