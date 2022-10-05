using System.Text;
using UnityEngine;

namespace ET
{
    public class StringReader
    {
        private string savedString;
        private int currentPos;

        public StringReader(string text)
        {
            this.savedString = text;
            currentPos = 0;
        }

        public string Next()
        {
            if (currentPos < savedString.Length)
            {
                var nextStr= savedString.Substring(currentPos, 1);
                currentPos ++;
                return nextStr;
            }
            else
            {
                return null;
            }
        }

        public bool IsNextExist()
        {
            return currentPos < savedString.Length;
        }

        public string GetByWidth(int targetWidth)
        {
            if (!IsNextExist()) return null;
            
            // TextGenerator textGen = new TextGenerator();
            // TextGenerationSettings genSetting = new TextGenerationSettings();
            // genSetting.font= Font.CreateDynamicFontFromOSFont("微软雅黑", 1);

            float currentWidth = 0f;
            string line = "";
            while (currentWidth<targetWidth && IsNextExist())
            {
                var nextChar = Next();
                var nextWidth = IsCnChar(nextChar) ? 2 : 1;
                // var nextWidth = textGen.GetPreferredWidth(nextChar, genSetting);
                line += nextChar;
                currentWidth += nextWidth;
            }

            // builder.AppendLine(line);
            return line;//+$":{currentWidth}";
        }
        
        public static bool IsCnChar(string charstr)
        {
            var chars = charstr.ToCharArray();
            var val = chars[0];
            if (val >= 0x4e00 && val <= 0x9fbb) return true;
            else return false;

        }
        
    }
}