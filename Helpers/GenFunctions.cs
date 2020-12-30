using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge.Helpers
{
    class GenFunctions
    {
        
        public static string CodingString(string stStr)
        {
            string res = "";
            int iCur = 0;
            for (int i = 0; i < stStr.Length; i++)
            {
                //System.Text.Encoding.UTF8.GetBytes((stStr[iCur]).ToCharArray());
                if (iCur <= 5)
                {
                    res = res + (char)((int)stStr[i] + Program.Key_Tbl[iCur]);
                    iCur++;
                }
                if (iCur == 5)
                    iCur = 0;
            }
            Random rd = new Random();
            for (int i = 0; i < 25; i++)
            {
                res = res + (char)(rd.Next(33, 126));
            }
            for (int i = 0; i < 25; i++)
            {
                res = (char)(rd.Next(33, 126)) + res;
            }

            return res;
        }

        public static string DecodingString(string stStr)
        {
            if (string.IsNullOrEmpty(stStr) || stStr.Length < 50)
            {
                return stStr;
            }
            string sSt = stStr.Remove(0,25);
            string res = "";

            int iCur = 0;
            for (int i = 24; i >= 0; i--)
            {
                sSt = sSt.Remove(sSt.Length - 1);
            }
            for (int i = 0; i < sSt.Length; i++)
            {
                if (iCur <= 5)
                {
                    res = res + (char)((int)sSt[i] - Program.Key_Tbl[iCur]);
                    iCur++;
                }
                if (iCur == 5)
                    iCur = 0;
            }

            return res;
        }
    }
}
