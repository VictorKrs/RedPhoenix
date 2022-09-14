using RedPhoenix.Common.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RedPhoenix.Common.Handlers.Logger
{
    /// <summary>
    /// Write and read logs from LogDir.
    /// Logs file name format: yyyy-MM-dd.txt, i.e. PhoenixLogger sort logs by date
    /// </summary>
    public static class PhoenixLogger
    {
        #region Constants
        private const int NULL_STORAGE_TIME = -1;
        private const string DATE_FORMAT = "yyyy-MM-dd";
        private const string GET_FILES_PATTERN = "20??-??-??.txt";
        private const string FILENAME_FORMAT = "{0}.txt"; // 0 - date
        #endregion

        #region Private fields
        private static DateTime? _lastCheckDate = null;     // last removal date; Removal occurs once a day

        // Thread lockers by filename. In case several threads try to write data in the same file
        private static Dictionary<string, object> _fileLockers = new Dictionary<string, object>();
        private static object _deleteLocker = new object();
        #endregion

        #region Properties
        /// <summary>
        /// Number of days of storage of logs. If StorageTime == -1 (default), then the logs aren't deleted
        /// </summary>
        public static int StorageTime { get; set; }
        public static Encoding LogEncoding { get; set; }
        /// <summary>
        /// Path to the log storage folder. Default="./"
        /// </summary>
        public static string LogDir { get; set; }
        #endregion  

        #region Constructor
        static PhoenixLogger()
        {
            LogDir = PhoenixFunctions.ProgramLocation;
            StorageTime = NULL_STORAGE_TIME;
            LogEncoding = PhoenixDefaultValues.ENCODING;
        }
        #endregion

        #region Methods
        #region WriteLog
        public static void WriteLog(string message)
        {
            try
            {
                CheckDateAndDeleteOld();

                WriteInLogFile(message);
            }
            catch (Exception) { throw; }
        }

        private static void WriteInLogFile(string message)
        {
            if (!Directory.Exists(LogDir))
                Directory.CreateDirectory(LogDir);

            var fname = string.Format(FILENAME_FORMAT, DateTime.Now.ToString(DATE_FORMAT));
            var fileName = LogDir + '\\' + fname;

            lock (GetLockObj(fname))
                using (var sw = new StreamWriter(fileName, true, LogEncoding))
                    sw.WriteLine(message);
        }
        #endregion

        #region GetMessages (read logs)
        public static List<string> GetMessages(DateTime? from, DateTime? to)
        {
            List<string> files = GetFiles(from, to);

            if (files == null || files.Count == 0)
                return null;

            return GetMessagesFromFiles(files);
        }

        private static List<string> GetMessagesFromFiles(List<string> files)
        {
            List<string> res = new List<string>();

            foreach (string fileName in files)
                res.AddRange(GetMessagesFromFile(fileName));

            return res;
        }

        private static List<string> GetMessagesFromFile(string fileName)
        {
            List<string> res = new List<string>();

            try
            {
                var fName = Path.GetFileName(fileName);

                lock (GetLockObj(fName))
                    res.AddRange(File.ReadAllLines(fileName, LogEncoding));
            }
            catch (Exception exception)
            {
                throw new Exception("Error read logs", exception);
            }

            return res;
        }
        #endregion

        #region Check date and delete old logs
        private static void CheckDateAndDeleteOld()
        {
            if (StorageTime == NULL_STORAGE_TIME)
                return;

            if (CheckDate())
                lock (_deleteLocker)
                {
                    _lastCheckDate = DateTime.Now;

                    DeleteOldLogs();
                }
        }

        /// <summary>
        /// Return true if the difference in days from the moment of the last check is more then one
        /// </summary>
        private static bool CheckDate()
        {
            if (_lastCheckDate == null) return true;

            var lastDate = _lastCheckDate.Value;
            var dateNow = DateTime.Now;
            var dateDiff = (dateNow.Year - lastDate.Year) + (dateNow.Month - lastDate.Month) + (dateNow.Day - lastDate.Day);

            return dateDiff != 0;
        }
        #region Delete logs
        private static void DeleteOldLogs()
        {
            if (StorageTime == NULL_STORAGE_TIME)
                return;

            var files = GetFiles(null, DateTime.Now.AddDays(-StorageTime));

            DeleteFiles(files);
        }

        static void DeleteFiles(List<string> files)
        {
            if (files == null || files.Count == 0)
                return;

            foreach (string file in files)
                try { File.Delete(file); }
                catch (Exception exception) { throw new Exception("Error cleaning outdated log files; File: " + file, exception); }
        }
        #endregion
        #endregion

        #region GetFiles
        private static List<string> GetFiles(DateTime? from, DateTime? to)
        {
            if (!Directory.Exists(LogDir)) return null;

            string[] files = Directory.GetFiles(LogDir, GET_FILES_PATTERN);

            if (files == null || files.Length == 0) return null;
            Array.Sort(files, StringComparer.InvariantCulture);

            var start = GetStartIndex(ref files, from);
            var end = GetEndIndex(ref files, to);

            return GetFiles(files, start, end);
        }

        private static List<string> GetFiles(string[] files, int start, int end)
        {
            if (start == -1 || end == -1)
                return null;

            List<string> res = new List<string>();

            for (int i = start; i <= end && i < files.Length; i++)
                if (GetDateFromFileName(files[i]) != null)
                    res.Add(files[i]);

            return res;
        }

        private static int GetStartIndex(ref string[] files, DateTime? from) =>
            GetFileIndex(files, from, 0, files.Length, 1);

        private static int GetEndIndex(ref string[] files, DateTime? to) =>
            GetFileIndex(files, to, files.Length - 1, -1, -1);

        /// <summary>
        /// Get start or end index by date from files.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="date">Date FROM or TO</param>
        /// <param name="index">Start index in files array: 0 or files.length-1</param>
        /// <param name="endIndex">Stop index (search startIndex: files.Length; search endIndex: -1)</param>
        /// <param name="step">Search startIndex => increment (1); Search endIndex => decrement (-1)</param>
        /// <returns></returns>
        private static int GetFileIndex(string[] files, DateTime? date, int index, int endIndex, int step)
        {
            if (date == null) return index;

            // if step == 1 => search startIndex (from 0 to files.Length - 1)
            //      countinueFlag == true while fileDate < date => 
            //      ((fileDate.Value - date.Value).Days < 0) == true == (step == 1)
            // if step == -1 => search endIndex (from files.Length -1 to 0)
            //      countinueFlag == true while fileDate > date => 
            //      ((fileDate.Value - date.Value).Days < 0) == false == (step == 1)
            var countinueFlag = true;

            while (countinueFlag && index != endIndex)
            {
                var fileDate = GetDateFromFileName(files[index]);

                countinueFlag = fileDate == null || fileDate.Value != date.Value && ((step == 1) == (fileDate.Value - date.Value).Days < 0);

                if (countinueFlag) index = index + step;
            }

            if (index == endIndex) return -1;

            return index;
        }

        private static DateTime? GetDateFromFileName(string fname)
        {
            var date = Path.GetFileNameWithoutExtension(fname);

            return GetDate(date, DATE_FORMAT);
        }
        #endregion

        #region Additional methods
        private static object GetLockObj(string fName)
        {
            if (!_fileLockers.ContainsKey(fName)) _fileLockers.Add(fName, new object());

            return _fileLockers[fName];
        }

        private static DateTime? GetDate(string date, string format)
        {
            try
            {
                return DateTime.ParseExact(date, format, CultureInfo.CurrentCulture);
            }
            catch (Exception) { return null; }
        }
        #endregion
        #endregion
    }
}
