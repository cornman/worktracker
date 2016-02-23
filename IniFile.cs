using System;
using System.Text;
using System.Runtime.InteropServices;

namespace worktracker
{
    class IniFile
    {
        private string m_path;

        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString
        (
            string section,
            string key,
            string value,
            string path
        );

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString
        (
            string section,
            string key,
            string defaultvalue,
            StringBuilder returnValue,
            int size,
            string filepath
        );

        public IniFile(string _path)
        {
            m_path = _path;
        }

        public void Flush()
        {
            WriteValue(null, null, null);
        }

        public void WriteValue(string _section, string _key, string _value)
        {
            WritePrivateProfileString(_section, _key, _value, m_path);
        }

        public string ReadValue(string _section, string _key, string _defaultValue)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(_section, _key, _defaultValue, temp, 255, m_path);
            return temp.ToString();
        }
    }
}
