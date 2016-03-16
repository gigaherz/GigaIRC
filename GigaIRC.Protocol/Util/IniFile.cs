using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GigaIRC.Util
{
    internal class IniFile
    {
        private readonly string _path;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public IniFile(string iniPath)
        {
            _path = iniPath;
        }

        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _path);
        }

        public void Write<T>(string section, string key, T value)
        {
            WritePrivateProfileString(section, key, value.ToString(), _path);
        }

        public string Read(string section, string key)
        {
            var ret = Read(section, key, null);
            if(ret == null)
                throw new KeyNotFoundException("The specified section or key was not found.");
            return ret;
        }

        public string Read(string section, string key, string @default)
        {
            var temp = new StringBuilder(255);
            if(GetPrivateProfileString(section, key, @default, temp, 255, _path)==0)
            {
                return @default;
            }
            return temp.ToString();
        }
    }
}