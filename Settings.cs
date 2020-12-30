using PosToWebPosBridge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge
{
    class Settings
    {
        //private static IniParser ini = new IniParser("settings.ini");

        private static int strToIntDef(string val)
        {
            int tmp;
            if (int.TryParse(val, out tmp))
                return int.Parse(val);
            else
                return 0;
        }

        private static IniParser ini = new IniParser(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\settings.ini");

        public static long TimerUpdate { get { return strToIntDef(ini.GetSetting("General Settings", "TimerUpdate")); } }
        public static long IsTest { get { return strToIntDef(ini.GetSetting("General Settings", "IsTest")); } }
        public static long InsertIngr { get { return strToIntDef(ini.GetSetting("General Settings", "InsertIngr")); } }
        public static string WebApiURL { get { return ini.GetSetting("General Settings", "WebApyURL"); } }
        public static string URLUserName { get { return ini.GetSetting("General Settings", "URLUserName"); } }
        public static string URLPass { get { return ini.GetSetting("General Settings", "URLPass"); } }
        public static int ExtType { get { return strToIntDef(ini.GetSetting("General Settings", "ExtType")); } }
        public static string ForkeyURL { get { return ini.GetSetting("General Settings", "ForkeyURL"); } }
        public static string ForkeyUserHeader { get { return ini.GetSetting("General Settings", "ForkeyUserHeader"); } }
        public static int SetDelayAfterMinutes { get { return strToIntDef(ini.GetSetting("General Settings", "SetDelayAfterMinutes")); } }
        public static int CheckCount { get { return strToIntDef(ini.GetSetting("General Settings", "CheckCount")); } }
        public static string WebPosServer { get { return ini.GetSetting("WebPos Settings", "WebPosServer"); } }
        public static string WebPosUser { get { return ini.GetSetting("WebPos Settings", "WebPosUser"); } }
        public static string WebPosPass { get { return ini.GetSetting("WebPos Settings", "WebPosPass"); } }
        public static string WebPosDB { get { return ini.GetSetting("WebPos Settings", "WebPosDB"); } }

        public static string HitPosServer { get { return ini.GetSetting("HitPos Settings", "HitPosServer"); } }
        public static string HitPosUser { get { return ini.GetSetting("HitPos Settings", "HitPosUser"); } }
        public static string HitPosPass { get { return ini.GetSetting("HitPos Settings", "HitPosPass"); } }
        public static string HitPosDB { get { return ini.GetSetting("HitPos Settings", "HitPosDB"); } }

        public static string ShopId { get { return ini.GetSetting("Program Settings", "ShopId"); } }
        public static string ShopInfoId { get { return ini.GetSetting("Program Settings", "ShopInfoId"); } }
        public static string ShopAddress { get { return ini.GetSetting("Program Settings", "ShopAddress"); } }
        public static string ShopCity { get { return ini.GetSetting("Program Settings", "ShopCity"); } }

        public static double ShopLongtitude{ get { return strToIntDef(ini.GetSetting("Program Settings", "ShopLongtitude")); } }
        public static double ShopLatitude { get { return strToIntDef(ini.GetSetting("Program Settings", "ShopLatitude")); } }

        public static Int64 PosInfoId { get { return strToIntDef(ini.GetSetting("Program Settings", "PosInfoId")); } }
        public static Int64 PriceListId { get { return strToIntDef(ini.GetSetting("Program Settings", "PriceListId")); } }
        public static Int64 StaffId { get { return strToIntDef(ini.GetSetting("Program Settings", "StaffId")); } }
        public static Int64 InvoiceType { get { return strToIntDef(ini.GetSetting("Program Settings", "InvoiceType")); } }
        public static Int64 CashId { get { return strToIntDef(ini.GetSetting("Program Settings", "CashId")); } }
        public static Int64 CreditCardId { get { return strToIntDef(ini.GetSetting("Program Settings", "CreditCardId")); } }
        //public static Int64 AccountId { get { return strToIntDef(ini.GetSetting("Program Settings", "AccountId")); } }

        public static Int64 ForkeyInvoiceType { get { return strToIntDef(ini.GetSetting("ForkeyParams", "IvoiceType")); } }
        public static Int64 ForkeyReceiptType { get { return strToIntDef(ini.GetSetting("ForkeyParams", "ReceiptType")); } }
        public static Int64 ForkeySalesTypeId { get { return strToIntDef(ini.GetSetting("ForkeyParams", "ForkeySalesTypeId")); } }
        public static Int64 ForkyPosInfoCaptensOrder { get { return strToIntDef(ini.GetSetting("ForkeyParams", "ForkyPosInfoCaptensOrder")); } }


        //public static List<SalesTypeMap> SalesTypeMap
        //{
        //    get
        //    {
        //        List<SalesTypeMap> res = new List<SalesTypeMap>();
        //        int nCnt = strToIntDef(ini.GetSetting("Sales Type Mapping", "nCnt"));
        //        for (int i = 1; i <= nCnt; i++)
        //        {
        //            var it = new SalesTypeMap()
        //            {
        //                HitSales = ini.GetSetting("Sales Type Mapping", "HitSales_" + i.ToString()),
        //                WebSales = strToIntDef(ini.GetSetting("Sales Type Mapping", "WebSales_" + i.ToString()))
        //            };
        //            res.Add(it);
        //        }
        //        return res;
        //    }
        //}

        //public static List<InvoiceTypesMap> InvoiceTypesMap
        //{
        //    get
        //    {
        //        List<InvoiceTypesMap> res = new List<InvoiceTypesMap>();
        //        int nCnt = strToIntDef(ini.GetSetting("InvoiceTypes Mapping", "nCnt"));
        //        for (int i = 1; i <= nCnt; i++)
        //        {
        //            var it = new InvoiceTypesMap()
        //            {
                        
        //                HitCode = ini.GetSetting("InvoiceTypes Mapping", "HitCode_" + i.ToString()),
        //                WebCode = ini.GetSetting("InvoiceTypes Mapping", "WebCode_" + i.ToString()),
        //                WebType = (Int16)strToIntDef(ini.GetSetting("InvoiceTypes Mapping", "WebType_" + i.ToString()))
        //            };
        //            res.Add(it);
        //        }
        //        return res;
        //    }
        //}

    }
}
