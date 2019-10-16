using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Text;

namespace awaken
{
    public class ConfigModel
    {
        private static string GetValue(string key)
        {
            string str = ConfigurationManager.AppSettings[key];
            return str;
        }

        private static void SetValue(string key, string value)
        {
            //Configuration对象
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("appSettings");
        }


        public static KeyValueConfigurationCollection GetConfigSettings(string filePath)
        {

            //string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            Configuration config = ConfigurationManager.OpenExeConfiguration(@filePath);
            if (config.HasFile)
            {
                AppSettingsSection appSection = (AppSettingsSection)config.GetSection("appSettings");
                return appSection.Settings;
            }
            return config.AppSettings.Settings;
        }


        private static void SetValue(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }
        }

        public static Font getFont()
        {
            String size_ = GetValue("font");
            if (String.IsNullOrEmpty(size_))
            {
                return new Font("微软雅黑",24);
            }
            String[] array= size_.Split('/');
            return new Font(array[0], float.Parse(array[1]));
        }

        public static void setFont(String font) {
            SetValue("font", font);
        }

        public static int[] getColorRgb() {
            String value_ = getColorStr();
            int[] color = new int[3];
            String[] coll= value_.Split('/');
            color[0] = int.Parse(coll[0]);
            color[1] = int.Parse(coll[1]);
            color[2] = int.Parse(coll[2]);
            return color;
        }

        public static String getColorStr() {
            String value_ = GetValue("fontColor");
            if (String.IsNullOrEmpty(value_))
            {
               return Color.Green.R+"/"+ Color.Green.G+"/"+ Color.Green.B;
            }
            return value_;
        }

        public static Color getColor() {
            int[] color = getColorRgb();
            return Color.FromArgb(color[0], color[1], color[2]);
        }

        public static void setColor(String color) {
            SetValue("fontColor", color);
        }

        public static int getTimer() {
            String value_ = GetValue("timer");
            int result = 1200;
            if (String.IsNullOrEmpty(value_))
            {
                return result;
            }
            int.TryParse(value_, out result);
            return result;
        }

        public static void setTimer(int timer) {
            SetValue("timer", timer.ToString());
        }

        public static String getFile() {
            String s= GetValue("file");
            if (string.IsNullOrEmpty(s)) {
                return "default.txt";
            }
            return s;
        }
        public static void setFile(String file) {
            SetValue("file",file);
        }

        public static string getDataPath() {
            return System.AppDomain.CurrentDomain.BaseDirectory + "data\\";
        }

        public static string getFullFilePath() {
            return getDataPath() + getFile();
        }

        public static int getStyle() {
            string style= GetValue("formStyle");
            if (string.IsNullOrEmpty(style)) {
                return 0;
            }
            return int.Parse(style);
        }

        public static void setStyle(int style) {
            SetValue("formStyle",style+"");
        }

        public static Font getFontMini()
        {
            String size_ = GetValue("fontMini");
            if (String.IsNullOrEmpty(size_))
            {
                return new Font("微软雅黑", 12);
            }
            String[] array = size_.Split('/');
            return new Font(array[0], float.Parse(array[1]));
        }

        public static void setFontMini(string font) {
            SetValue("fontMini",font);
        }

        public static int getRowNum() {
            string s= GetValue("rowNum");
            if (string.IsNullOrEmpty(s)) {
                return 16;
            }
            return int.Parse(s);
        }

        public static void setRowNum(int rowNum) {
            SetValue("rowNum", rowNum+"");
        }

        public static int getMiniTimer()
        {
            String value_ = GetValue("miniTimer");
            int result = 60;
            if (String.IsNullOrEmpty(value_))
            {
                return result;
            }
            int.TryParse(value_, out result);
            return result;
        }

        public static void setMiniTimer(int timer) {
            SetValue("miniTimer",timer+"");
        }
        public static String getMsgHost() {
            return GetValue("msgHost");
        }
        public static int getSleep() {
            return Convert.ToInt32(GetValue("sleep"));
        }

        public static int getWarnTime() {
            return Convert.ToInt32(GetValue("warnTime"));
        }
    }
}
