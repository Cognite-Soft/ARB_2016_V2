using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Cognite.Arb.Sts.Business
{
    public static class SecurityTokenServiceManager
    {
        private static readonly string _logFile;

        static SecurityTokenServiceManager()
        {
            try
            {
                _logFile = ConfigurationManager.AppSettings["logFilePath"];
                Log("Started");
            }
            catch
            {
            }
        }

        private static readonly Dictionary<string, KeyValuePair<string, string>[]> _tokens =
            new Dictionary<string, KeyValuePair<string, string>[]>();

        public static string Add(KeyValuePair<string, string>[] user)
        {
            try
            {
                Log("Add");
                lock (_tokens)
                {
                    Log("Add - locked");
                    var token = Guid.NewGuid().ToString();
                    _tokens.Add(token, user);
                    Log("Add - added " + token);
                    return token;
                }
            }
            catch (Exception ex)
            {                
                Log("Add Exception : " + ex.Message);
                throw;
            }
        }

        public static KeyValuePair<string, string>[] Get(string token)
        {
            try
            {
                Log("Get " + token);
                lock (_tokens)
                {
                    Log("Get " + token + " locked");
                    return _tokens.ContainsKey(token) ? _tokens[token] : null;
                }
            }
            catch (Exception ex)
            {
                Log("Get Exception : " + ex.Message);
                throw;
            }
            finally
            {
                Log("Get " + token + " End");                
            }
        }

        public static void Remove(string token)
        {
            try
            {
                Log("Remove " + token);
                lock (_tokens)
                {
                    Log("Remove " + token + " locked");
                    if (_tokens.ContainsKey(token))
                        _tokens.Remove(token);
                    Log("Remove " + token + " removed");
                }
                Log("Remove " + token + " end");
            }
            catch (Exception ex)
            {                
                Log("Remove Exception : " + ex.Message);
                throw;
            }
        }

        private static void Log(string message)
        {
            if (_logFile == null) return;
            File.AppendAllText(_logFile, DateTime.Now + " : " + message + Environment.NewLine);
        }
    }
}