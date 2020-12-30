using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceProcess;
using Geocoding.Google;
using Geocoding;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using PosToWebPosBridge.Models;
using PosToWebPosBridge.Enumarators;
using System.Globalization;
using log4net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Runtime.Serialization.Formatters.Binary;

namespace PosToWebPosBridge
{
    class Program
    {
        static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static bool IS_TEST = false;

        public static int[] Key_Tbl = new int[6];

        public static string flName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\lostEOD_Ids.log";

        public static string SERVICENAME = "PosToWebPosBridge";
        public static string EXENAME = "PosToWebPosBridge";

        private static System.Timers.Timer tmNew;

        private static SettingsModel settings;

        #region Nested classes to support running as service

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.SERVICENAME;

            }

            protected override void OnStart(string[] args)
            {

                //try
                //{
                //    Program.Start(args);
                //}
                //catch (Exception ex)
                //{
                //    Log.ToErrorLog("at starting service :" + ex.Message + "  Stack: " + ex.StackTrace);

                //}
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        #endregion

        static void Main(string[] args)
        {
            string exName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            if (!string.IsNullOrEmpty(exName))
            {
                SERVICENAME = exName;
                EXENAME = exName;
            }


            Key_Tbl[0] = 72;
            Key_Tbl[1] = 73;
            Key_Tbl[2] = 84;
            Key_Tbl[3] = 45;
            Key_Tbl[4] = 83;
            Key_Tbl[5] = 65; //[H,I,T,-,S,A]

            tmNew = new System.Timers.Timer(Settings.TimerUpdate * 1000);
            IS_TEST = Settings.IsTest == 1;
            if (!IS_TEST)
                tmNew.Elapsed += new System.Timers.ElapsedEventHandler(PerformOperations);
            tmNew.Enabled = true;
            tmNew.Start();

            //new System.Timers.ElapsedEventHandler(timer_Elapsed);

            if (!Environment.UserInteractive)
            { // running as service
                logger.Info("Application opened as Windows Service");

                using (var service = new Service())
                    ServiceBase.Run(service);
            }
            else
            {
                //// running as console app
                //// running as console app
                logger.Info("Application opened as Console Application");
                ServiceController sc = new ServiceController(SERVICENAME);

                bool isServicePresent = false;
                try { var test = sc.Status; isServicePresent = true; }
                catch { }

                ConsoleKeyInfo key;
                if (isServicePresent)
                {

                    Console.WriteLine("Application is installed as Windows Service.");
                    Console.WriteLine(sc.Status != ServiceControllerStatus.Running
                                            ? "Service is stopped\n\n[S] to start Service " : "Service is running!\n\n[S] to stop Service ");
                    Console.WriteLine("[U] to unistall as Windows Service ");
                    Console.WriteLine("ANY other key to exit");

                    key = Console.ReadKey(true);
                    Console.Clear();
                    switch (key.KeyChar)
                    {
                        case 'S':
                        case 's':
                            if (sc.Status == ServiceControllerStatus.Running && sc.CanStop)
                            {
                                sc.Stop();
                                sc.WaitForStatus(ServiceControllerStatus.Stopped);
                                Console.WriteLine("Windows Service stopped!\n\nPress any key to exit...");
                                Console.ReadKey(true);
                            }
                            else
                                if (sc.Status == ServiceControllerStatus.Stopped || sc.Status == ServiceControllerStatus.Paused)
                            {
                                sc.Start();
                                sc.WaitForStatus(ServiceControllerStatus.Running);
                                Console.WriteLine("Windows Service is up and running!\n\nPress any key to exit...");
                                Console.ReadKey(true);
                            }


                            break;
                        case 'U':
                        case 'u':
                            try
                            {
                                if (sc.CanStop)
                                {
                                    sc.Stop();
                                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                                }
                            }
                            catch
                            { }

                            Helpers.ServiceInstaller cu = new Helpers.ServiceInstaller();
                            cu.UninstallService(SERVICENAME);
                            logger.Info("Application uninstalled as Windows Service");
                            Console.WriteLine("Application uninstalled as Windows Service succesfully!\n\nPress any key to exit...");
                            Console.ReadKey(true);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Application is not installed as Windows Service.");
                    Console.WriteLine("");
                    Console.WriteLine("[I] to install as Windows Service");
                    Console.WriteLine("[R] to run as Console Application ");

                    key = Console.ReadKey(true);
                    Console.Clear();
                    switch (key.KeyChar)
                    {
                        case 'I':
                        case 'i':
                            logger.Info("Application installing as Windows Service");
                            Helpers.ServiceInstaller ci = new Helpers.ServiceInstaller();
                            ci.InstallService(Environment.CurrentDirectory + @"\" + EXENAME + ".exe", SERVICENAME, SERVICENAME);
                            Console.WriteLine("Application installed as Windows Service succesfully and is up and runnning!\n\nPress any key to exit...");
                            Console.ReadKey(true);
                            break;
                        case 'R':
                        case 'r':
                            try
                            {
                                //StartJobTest();
                                StartJob();
                            }
                            finally
                            {
                                Stop();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void PerformOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmNew.Enabled = false;
                tmNew.Elapsed -= PerformOperations;
                if (IS_TEST)
                {

                }
                else
                {
                    //StartJobTest();
                    StartJob();
                }
            }
            catch (Exception ex)
            {
                logger.Error("PerformOperations exeption : \r\n " + ex.ToString());
                tmNew.Enabled = true;
            }
            finally
            {
                tmNew.Elapsed += new System.Timers.ElapsedEventHandler(PerformOperations);
                tmNew.Enabled = true;
            }
        }

        static void StartJobTest()
        {
            try
            {
                int idx = 0;

                for (int i = 0; i < 5000; i++)
                {
                    idx = i;
                }
            }
            catch (Exception ex)
            {
                string sMess = ex.Message;
            }
        }

        static void StartJob()
        {
            InitializeSettingsModel();

            if (settings.ExtType == 3)
            {
                ForkeyDelivery();
            }
            else
            {
                AtcomDelivery();
            }
        }

        public static void Stop()
        {
            //conn.DisconnectSocket();

            if (!Environment.UserInteractive)
                //if (Environment.CurrentDirectory.Contains("system"))
                logger.Info("Application stopped as Windows Service");
            else
                logger.Info("Application closed as Console Application");

        }

        #region General

        /// <summary>
        /// Get's parameter's from Settings.ini to a model
        /// </summary>
        private static void InitializeSettingsModel()
        {
            settings = new SettingsModel();

            settings.InsertIngr = Settings.InsertIngr;
            settings.WebApiURL = Settings.WebApiURL;
            if (!string.IsNullOrEmpty(settings.WebApiURL))
            {
                if (settings.WebApiURL[settings.WebApiURL.Length - 1] != '/')
                    settings.WebApiURL += '/';
            }
            settings.URLUserName = Settings.URLUserName;
            settings.URLPass = Helpers.GenFunctions.DecodingString(Settings.URLPass);
            settings.ShopId = Settings.ShopId;
            settings.ShopInfoId = Settings.ShopInfoId;
            settings.ShopAddress = Settings.ShopAddress;
            settings.ShopCity = Settings.ShopCity;
            settings.ShopLatitude = Settings.ShopLatitude;
            settings.ShopLongtitude = Settings.ShopLongtitude;
            settings.PosInfoId = Settings.PosInfoId;
            settings.PriceListId = Settings.PriceListId;
            settings.StaffId = Settings.StaffId;
            settings.InvoiceType = Settings.InvoiceType;
            settings.CashId = Settings.CashId;
            settings.CreditCardId = Settings.CreditCardId;
            settings.WebPosServer = Settings.WebPosServer;
            settings.WebPosDB = Settings.WebPosDB;
            settings.WebPosUser = Settings.WebPosUser;
            settings.WebPosPass = Helpers.GenFunctions.DecodingString(Settings.WebPosPass);
            settings.HitPosServer = Settings.HitPosServer;
            settings.HitPosDB = Settings.HitPosDB;
            settings.HitPosUser = Settings.HitPosUser;
            settings.HitPosPass = Helpers.GenFunctions.DecodingString(Settings.HitPosPass);
            settings.ExtType = Settings.ExtType;

            settings.SetDelayAfterMinutes = Settings.SetDelayAfterMinutes;

            //settings.SalesTypeMapped = Settings.SalesTypeMap;

            //settings.InvoiceTypesMapped = Settings.InvoiceTypesMap;

            settings.CheckCount = Settings.CheckCount;
            settings.ForkeyURL = Settings.ForkeyURL;
            if (!string.IsNullOrEmpty(settings.ForkeyURL))
            {
                if (settings.ForkeyURL[settings.ForkeyURL.Length - 1] != '/')
                    settings.ForkeyURL += '/';
            }
            settings.ForkeyUserHeader = Settings.ForkeyUserHeader;
            settings.ForkeyInvoiceType = Settings.ForkeyInvoiceType;
            settings.ForkeyReceiptType = Settings.ForkeyReceiptType;
            settings.ForkeySalesTypeId = Settings.ForkeySalesTypeId;
            settings.ForkyPosInfoCaptensOrder = Settings.ForkyPosInfoCaptensOrder;

            string jsonFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\salesType.json";

            if (File.Exists(jsonFile))
            {
                string json = File.ReadAllText(jsonFile);
                settings.SalesTypeMapped = JsonConvert.DeserializeObject<List<SalesTypeMap>>(json);
            }
            else
                settings.SalesTypeMapped = new List<SalesTypeMap>();

            jsonFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\InvoiceTypes.json";
            if (File.Exists(jsonFile))
            {
                string json = File.ReadAllText(jsonFile);
                settings.InvoiceTypesMapped = JsonConvert.DeserializeObject<List<InvoiceTypesMap>>(json);
            }
            else
                settings.InvoiceTypesMapped = new List<InvoiceTypesMap>();
        }

        /// <summary>
        /// Convert's a string to UTF8
        /// </summary>
        /// <param name="sStr"></param>
        /// <returns></returns>
        private static String ConvertToUtf8(String sStr)
        {
            byte[] bytDestination;
            string strTo = String.Empty;

            var utf8 = Encoding.GetEncoding(1253);
            byte[] utfBytes = utf8.GetBytes(sStr);
            bytDestination = Encoding.Convert(Encoding.GetEncoding(1253), Encoding.UTF8, utfBytes);
            strTo = Encoding.UTF8.GetString(bytDestination);
            return strTo;
        }

        /// <summary>
        /// Executes SQL commands
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static bool ExecSQLs(SqlConnection conn, string command)
        {
            try
            {
                SqlCommand comm = new SqlCommand(command, conn);
                comm.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("ExecSQLs [SQL : " + command + "] \r\n" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Converts Int To String
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static string NumToText(int? num)
        {
            if (num == null)
                return "NULL";
            else
                return num.ToString();
        }

        /// <summary>
        /// Converts Decimal to String
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static string NumToText(decimal? num)
        {
            if (num == null)
                return "NULL";
            else
                return num.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Converts float to String
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static string NumToText(float? num)
        {
            if (num == null)
                return "NULL";
            else
                return num.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Converts Date to string
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="nLong"></param>
        /// <returns></returns>
        private static string DateToText(DateTime? dt, int nLong = 1)
        {
            if (dt == null)
            {
                return "NULL";
            }
            else
            {
                if (nLong == 1)
                    return "'" + (dt ?? new DateTime(1900, 1, 1)).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                else
                    return "'" + (dt ?? new DateTime(1900, 1, 1)).ToString("yyyy-MM-dd") + "'";
            }
        }

        #endregion



        #region Atcom

        /// <summary>
        /// Delivery from Atcom
        /// Using HitPos database.
        /// ExtType (1,2)
        /// ExtType 1=> Order
        /// ExtType 2=> Order on kitchen
        /// </summary>
        private static void AtcomDelivery()
        {
            bool bOK = true;

            SqlConnection WebPosConn = new SqlConnection("Data Source = " + settings.WebPosServer + ";Initial Catalog=" + settings.WebPosDB +
                    ";User id=" + settings.WebPosUser + ";Password=" + settings.WebPosPass + ";MultipleActiveResultSets=true;");
            try
            {
                WebPosConn.Open();
                logger.Error("Web  Pos Connection Opened");
            }
            catch (Exception ex)
            {
                logger.Error("Web  Pos Connection " + ex.ToString());
                return;
            }
            SqlConnection HitPosConn = new SqlConnection("Data Source = " + settings.HitPosServer + ";Initial Catalog=" + settings.HitPosDB +
                    ";User id=" + settings.HitPosUser + ";Password=" + settings.HitPosPass + ";MultipleActiveResultSets=true;");
            try
            {
                HitPosConn.Open();
            }
            catch (Exception ex)
            {
                logger.Error("Hit  Pos Connection " + ex.ToString());
                return;
            }

            StringBuilder SQL = new StringBuilder();
            StringBuilder SQLHPos = new StringBuilder();
            try
            {


                /*Sales mapping declaration*/
                string SalesTypes = "DECLARE @salesType TABLE(HitSales NTEXT, WebSales BIGINT, PosInfoId BIGINT, PricelistId BIGINT) \n";
                foreach (SalesTypeMap item in settings.SalesTypeMapped)
                    SalesTypes = SalesTypes + "INSERT INTO @salesType (HitSales,WebSales,PosInfoId,PricelistId) VALUES ('" + item.HitSales + "'," + item.WebSales.ToString() + "," + item.PosInfoId.ToString() + "," + item.PricelistId.ToString() + ") \n";

                /*Invoice type declaration*/
                string InvoiceTypes = "DECLARE @InvoiceCode TABLE(hitCode VARCHAR(10), WebCode VARCHAR(20), WebType INT) \n";
                foreach (InvoiceTypesMap item in settings.InvoiceTypesMapped)
                    InvoiceTypes = InvoiceTypes + "INSERT INTO @InvoiceCode(hitCode, WebCode, WebType) VALUES ('" + item.HitCode + "', '" + item.WebCode + "'," + item.WebType.ToString() + ") \n";

                if (!CreateTables(WebPosConn))
                    throw new EvaluateException("CreateTables");

                if (!InsertAtcomIngredients(HitPosConn, WebPosConn))
                    throw new EvaluateException("InsertAtcomIngredients");

                if (!FillTables(WebPosConn, HitPosConn))
                    throw new EvaluateException("FillTables");

                if (settings.InsertIngr != 0)
                {
                    if (!InsertNewIngredients(HitPosConn, WebPosConn))
                        throw new EvaluateException("InsertNewIngredients");
                }

                List<Int64> updOrders = new List<Int64>();

                List<OrdersExternal> ExtOrders = GetOrders(WebPosConn, SalesTypes, InvoiceTypes);

                List<Orders> orders = CreateOrderModel(ExtOrders, WebPosConn, SalesTypes, out updOrders);

                //Add a - (minus) when shipping address is null or empty spaces
                orders.ForEach(f => f.ShippingAddress = (string.IsNullOrWhiteSpace(f.ShippingAddress) ? "-" : f.ShippingAddress));

                string sUpdOrds = "";
                List<string> DeleteLostOrders = new List<string>();

                string OrderJSON = JsonConvert.SerializeObject(orders);
                if (OrderJSON.Trim() != "" && OrderJSON.Trim() != "[]")
                {

                    if (PostDataWithApi(OrderJSON))
                    {
                        List<string> LostOrders = new List<string>();
                        LostOrders = WebPosConn.Query<string>("SELECT OrderNo FROM ExternalLostOrders WHERE ExtType = " + settings.ExtType.ToString()).ToList();

                        foreach (var item in LostOrders)
                        {
                            int nCnt = WebPosConn.Query<int>("SELECT ISNULL(COUNT(*),0) nCnt FROM [Order] WHERE ExtKey = " + item.ToString() + " AND ExtType = " + settings.ExtType.ToString()).FirstOrDefault();
                            if (nCnt != 0)
                                DeleteLostOrders.Add(item.ToString());
                        }
                        string DelOrders = "";
                        foreach (var item in DeleteLostOrders)
                        {
                            LostOrders.Remove(item.ToString());
                            DelOrders = DelOrders + "'" + item.ToString() + "',";
                        }
                        if (DelOrders.Length > 0)
                        {
                            DelOrders = DelOrders.Substring(0, DelOrders.Length - 1);
                            SQL.Clear();
                            SQL.Append("DELETE FROM ExternalLostOrders WHERE ExtType = " + settings.ExtType.ToString() + " AND OrderNo IN (" + DelOrders + ")");
                            if (!ExecSQLs(WebPosConn, SQL.ToString()))
                                throw new EvaluateException("DELETE FROM ExternalLostOrders");
                        }

                        foreach (var item in updOrders)
                        {
                            if (!LostOrders.Contains(item.ToString()))
                            {
                                int nCnt = WebPosConn.Query<int>("SELECT ISNULL(COUNT(*),0) nCnt FROM [Order] WHERE ExtKey = " + item.ToString() + " AND ExtType = " + settings.ExtType.ToString()).FirstOrDefault();
                                if (nCnt != 0)
                                    sUpdOrds += item.ToString() + ",";
                            }
                        }
                        if (sUpdOrds.Trim() != "")
                        {
                            if (sUpdOrds[sUpdOrds.Length - 1] == ',')
                                sUpdOrds = sUpdOrds.Substring(0, sUpdOrds.Length - 1);

                            SQL.Clear();
                            SQL.Append("UPDATE HitPosOrders SET completed = 1 WHERE orderno IN (" + sUpdOrds + ")");
                            if (!ExecSQLs(WebPosConn, SQL.ToString()))
                                throw new EvaluateException("UPDATE HitPosOrders");
                            SQL.Clear();
                            SQL.Append("UPDATE posuser.orders SET status = 0 WHERE orderno In (" + sUpdOrds + ")");
                            if (!ExecSQLs(HitPosConn, SQL.ToString()))
                                throw new EvaluateException("UPDATE posuser.orders");
                        }

                        SQL.Clear();
                        SQLHPos.Clear();
                        foreach (var item in LostOrders)
                        {
                            int nDays = WebPosConn.Query<int>("SELECT TOP 1 DATEDIFF(d, hpo.CreationDate, CAST(CONVERT(VARCHAR(10),GETDATE(),120) AS DATETIME)) nDays FROM HitPosOrders hpo WHERE hpo.orderno = " + item.ToString()).FirstOrDefault();
                            if (nDays != 0)
                            {
                                SQL.Append("UPDATE HitPosOrders SET completed = 2 WHERE orderno = " + item.ToString() + "; \n");
                                SQLHPos.Append("UPDATE posuser.orders SET [status] = 9 WHERE orderno = " + item.ToString() + " AND shop_id = '" + settings.ShopId + "'; \n");
                            }
                        }
                        if (SQL.Length > 0)
                        {
                            if (!ExecSQLs(WebPosConn, SQL.ToString()))
                                throw new EvaluateException(SQL.ToString());
                        }
                        if (SQLHPos.Length > 0)
                        {
                            if (!ExecSQLs(HitPosConn, SQLHPos.ToString()))
                                throw new EvaluateException(SQLHPos.ToString());
                        }
                    }

                }
                if (!UpdateHitPosStatuses(WebPosConn, HitPosConn))
                    throw new EvaluateException("UpdateHitPosStatuses");

                //                HitTransaction.Commit();
                //                WebTransaction.Commit();
            }
            catch (Exception ex)
            {
                logger.Error("StartJob \r\n " + ex.ToString());
                //                HitTransaction.Rollback();
                //                WebTransaction.Rollback();
            }
        }

        /// <summary>
        /// Makes a model of delvery customers to send for new or update
        /// </summary>
        /// <param name="WebPos"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        private static DeliveryCustomerModel MakeCustomerModel(SqlConnection WebPos, string CustomerId)
        {
            DeliveryCustomerModel res = new DeliveryCustomerModel();
            StringBuilder SQL = new StringBuilder();

            try
            {
                SQL.Append("SELECT ISNULL(hpc.name,'') LastName, ISNULL(hpc.fname,'') FirstName, ISNULL(hpc.afm,'') VatNo, \n"
                           + "  ISNULL(hpc.doy,'') DOY, ISNULL(hpc.orofos1,'0') [Floor], ISNULL(hpc.email,'') email,  \n"
                           + "  ISNULL(hpc.remarks,'') Comments, '" + CustomerId + "' ExtCustId, \n"
                           + "  CASE WHEN ISNULL(hpc.company_name,'') = '' THEN ISNULL(hpc.name,'')+' '+ISNULL(hpc.fname,'') ELSE hpc.company_name END BillingName,  \n"
                           + "  ISNULL(hpc.afm,'') BillingVatNo, ISNULL(hpc.doy,'') BillingDOY, \n"
                           + "  ISNULL(hpc.profession,'') BillingJob, 'False' IsDeleted, \n"
                           + "  ISNULL(hpc.tel1,'') tel1, ISNULL(hpc.tel2,'') tel2, ISNULL(hpc.fax,'') fax, ISNULL(hpc.mobile,'') mobile,  \n"
                           + "	CASE WHEN LTRIM(RTRIM(ISNULL(hpc.address1,''))) = '' THEN 'Unkown' ELSE LTRIM(RTRIM(ISNULL(hpc.address1,''))) END address1, \n"
                           + "	LTRIM(RTRIM(ISNULL(hpc.address2,''))) address2, ISNULL(hpc.orofos2,'0') orofos2, \n"
                           + "	CASE WHEN ISNULL(hpc.address_no,'') = '' THEN '-' ELSE hpc.address_no END address_no, \n"
                           + "	 ISNULL(hpc.city,'') city, LTRIM(RTRIM(ISNULL(hpc.zipcode,''))) zipcode, \n"
                           + "   \n"
                           + "	  hpc.amount, \n "
                           + "	CASE WHEN ISNULL(hpc.bl_address,'') = '' THEN 'Unkown' ELSE hpc.bl_address END bl_address,  \n"
                           + "	CASE WHEN ISNULL(hpc.bl_address_no,'') = '' THEN '-' ELSE hpc.bl_address_no END bl_address_no, \n"
                           + "	ISNULL(hpc.bl_city,'') bl_city, ISNULL(hpc.doycode,'') doycode  \n"
                           + "FROM HitPosCustomers AS hpc WHERE hpc.customerid = '" + CustomerId + "'");
                DeliveryCustomerExtendedModel cust = WebPos.Query<DeliveryCustomerExtendedModel>(SQL.ToString()).FirstOrDefault();

                if (cust != null)
                {
                    res = (DeliveryCustomerModel)cust;

                    //res.LastName = (string)row["name"];
                    //res.FirstName = (string)row["fname"];
                    //res.VatNo = (string)row["afm"];
                    //res.DOY = (string)row["doy"];
                    //res.Floor = (string)row["orofos1"];
                    //res.email = (string)row["email"];
                    //res.Comments = (string)row["remarks"];
                    //res.ExtCustId = CustomerId;
                    //res.BillingName = ((string)row["company_name"]) == "" ? ((string)row["name"] + " " + (string)row["fname"]) : (string)row["company_name"];
                    //res.BillingVatNo = (string)row["afm"];
                    //res.BillingDOY = (string)row["doy"];
                    //res.BillingJob = (string)row["profession"];
                    //res.IsDeleted = false;
                    res.Phones = new List<DeliveryCustomersPhonesModel>();
                    if (!string.IsNullOrEmpty(cust.tel1))
                    {
                        DeliveryCustomersPhonesModel tel1 = new DeliveryCustomersPhonesModel();
                        tel1.ID = 0;
                        tel1.CustomerID = 0;
                        tel1.PhoneNumber = cust.tel1;
                        tel1.IsDeleted = false;
                        tel1.IsSelected = true;
                        res.Phones.Add(tel1);
                    }
                    if (!string.IsNullOrEmpty(cust.tel2))
                    {
                        DeliveryCustomersPhonesModel tel2 = new DeliveryCustomersPhonesModel();
                        tel2.ID = 0;
                        tel2.CustomerID = 0;
                        tel2.PhoneNumber = cust.tel2;
                        tel2.IsDeleted = false;
                        tel2.IsSelected = cust.tel1 == "" ? true : false;
                        res.Phones.Add(tel2);
                    }
                    if (!string.IsNullOrEmpty(cust.mobile))
                    {
                        DeliveryCustomersPhonesModel mobile = new DeliveryCustomersPhonesModel();
                        mobile.ID = 0;
                        mobile.CustomerID = 0;
                        mobile.PhoneNumber = cust.mobile;
                        mobile.IsDeleted = false;
                        mobile.IsSelected = ((cust.tel1 == "") && (cust.tel2 == "")) ? true : false;
                        res.Phones.Add(mobile);
                    }
                    if (!string.IsNullOrEmpty(cust.fax))
                    {
                        DeliveryCustomersPhonesModel fax = new DeliveryCustomersPhonesModel();
                        fax.ID = 0;
                        fax.CustomerID = 0;
                        fax.PhoneNumber = cust.fax;
                        fax.IsDeleted = false;
                        fax.IsSelected = ((cust.tel1 == "") && (cust.tel2 == "") && (cust.mobile == "")) ? true : false;
                        res.Phones.Add(fax);
                    }

                    double Latitude = 0;
                    double Longtitude = 0;
                    string adrs = "";

                    res.ShippingAddresses = new List<DeliveryCustomersShippingAddressModel>();

                    DeliveryCustomersShippingAddressModel shipping = new DeliveryCustomersShippingAddressModel();
                    shipping.ID = 0;
                    shipping.CustomerID = 0;
                    shipping.AddressStreet = cust.address1;
                    shipping.AddressNo = cust.address_no;
                    shipping.City = cust.city;
                    shipping.Zipcode = cust.zipcode;
                    adrs += shipping.AddressStreet;
                    adrs += (shipping.AddressNo != "" ? (" " + shipping.AddressNo) : "");
                    adrs += (shipping.City != "" ? (" " + shipping.City) : "");
                    adrs += (shipping.Zipcode != "" ? (" " + shipping.Zipcode) : "");
                    //GetAddressPosition(adrs, out Latitude, out Longtitude);
                    if (Latitude == 0)
                        shipping.Latitude = settings.ShopLatitude.ToString();
                    else
                        shipping.Latitude = Latitude.ToString();
                    if (Longtitude == 0)
                        shipping.Longtitude = settings.ShopLongtitude.ToString();
                    else
                        shipping.Longtitude = Longtitude.ToString();
                    shipping.Floor = cust.Floor;
                    shipping.IsDeleted = false;
                    shipping.IsSelected = (!string.IsNullOrEmpty(cust.address1));
                    res.ShippingAddresses.Add(shipping);

                    Longtitude = 0;
                    Latitude = 0;

                    if (!string.IsNullOrEmpty(cust.address2))
                    {
                        shipping = new DeliveryCustomersShippingAddressModel();
                        shipping.ID = 0;
                        shipping.CustomerID = 0;
                        shipping.AddressStreet = cust.address2;
                        shipping.AddressNo = "-";
                        shipping.City = "";
                        shipping.Zipcode = "";
                        adrs += shipping.AddressStreet;
                        adrs += (shipping.AddressNo != "" ? (" " + shipping.AddressNo) : "");
                        adrs += (shipping.City != "" ? (" " + shipping.City) : "");
                        adrs += (shipping.Zipcode != "" ? (" " + shipping.Zipcode) : "");
                        //GetAddressPosition(adrs, out Latitude, out Longtitude);
                        if (Latitude == 0)
                            shipping.Latitude = settings.ShopLatitude.ToString();
                        else
                            shipping.Latitude = Latitude.ToString();
                        if (Longtitude == 0)
                            shipping.Longtitude = settings.ShopLongtitude.ToString();
                        else
                            shipping.Longtitude = Longtitude.ToString();
                        shipping.Floor = cust.orofos2;
                        shipping.IsDeleted = false;
                        shipping.IsSelected = (string.IsNullOrEmpty(cust.address1));
                        res.ShippingAddresses.Add(shipping);
                    }

                    res.BillingAddresses = new List<DeliveryCustomersBillingAddressModel>();

                    DeliveryCustomersBillingAddressModel billing = new DeliveryCustomersBillingAddressModel();
                    billing.ID = 0;
                    billing.CustomerID = 0;
                    billing.AddressStreet = cust.bl_address;
                    billing.AddressNo = cust.bl_address_no;
                    billing.City = cust.bl_city;
                    billing.Zipcode = "";
                    adrs += billing.AddressStreet;
                    adrs += (billing.AddressNo != "" ? (" " + billing.AddressNo) : "");
                    adrs += (billing.City != "" ? (" " + billing.City) : "");
                    adrs += (billing.Zipcode != "" ? (" " + billing.Zipcode) : "");

                    Latitude = 0;
                    Longtitude = 0;
                    //GetAddressPosition(adrs, out Latitude, out Longtitude);
                    if (Latitude == 0)
                        billing.Latitude = settings.ShopLatitude.ToString();
                    else
                        billing.Latitude = Latitude.ToString();
                    if (Longtitude == 0)
                        billing.Longtitude = settings.ShopLongtitude.ToString();
                    else
                        billing.Longtitude = Longtitude.ToString();
                    billing.Floor = cust.Floor;
                    billing.IsDeleted = false;
                    billing.IsSelected = true;
                    res.BillingAddresses.Add(billing);

                    res.Assocs = new List<DeliveryCustomersPhonesAndAddressModel>();
                }
            }
            catch (Exception ex)
            {
                logger.Error("MakeCustomerModel : \r\n" + ex.ToString());
            }
            return res;
        }

        /// <summary>
        /// Update Hit Pos With new Order status
        /// </summary>
        /// <param name="WebPos"></param>
        /// <param name="HitPos"></param>
        /// <returns></returns>
        private static bool UpdateHitPosStatuses(SqlConnection WebPos, SqlConnection HitPos)
        {
            StringBuilder SQL = new StringBuilder();
            StringBuilder SQLHp = new StringBuilder();
            try
            {
                int Status;
                String TimeChanged;
                string updIds = "";
                string ExtKey;
                SQL.Append("SELECT esm.MatchValue, o.ExtKey, os.TimeChanged, os.Id \n"
                        + "FROM [Order] o  \n"
                        + "INNER JOIN OrderStatus os ON os.OrderId = o.Id AND ISNULL(os.ExtState,0) = 0 \n"
                        + "INNER JOIN ExternalStateMatch esm ON esm.[Status] = os.[Status] AND esm.ExtType = " + settings.ExtType.ToString() + " \n"
                        + "CROSS APPLY ( \n"
                        + "     SELECT TOP 1 CAST(hpo.orderno AS VARCHAR(20)) orderno \n"
                        + "     FROM HitPosOrders hpo \n"
                        + "     WHERE hpo.orderno = CAST(o.ExtKey AS BIGINT) AND hpo.CreationDate = CAST(CONVERT(VARCHAR(10), GETDATE(), 120) AS DATETIME) \n"
                        + ")hpo \n"
                        + "WHERE o.EndOfDayId IS NULL AND o.ExtType = " + settings.ExtType.ToString() + " AND o.ExtKey IS NOT NULL \n"
                        + "ORDER BY o.Id, os.TimeChanged");
                List<OrdersOrderStatusModel> dsCheck = WebPos.Query<OrdersOrderStatusModel>(SQL.ToString()).ToList();
                SQL.Clear();
                foreach (OrdersOrderStatusModel row in dsCheck)
                {
                    Status = row.MatchValue ?? -1;
                    TimeChanged = row.TimeChanged == null ? "NULL" : (row.TimeChanged ?? new DateTime(1900, 1, 1)).ToString("yyyy-MM-dd HH:mm:ss");
                    ExtKey = string.IsNullOrEmpty(row.ExtKey) ? "not exists" : row.ExtKey;
                    if (Status > -1)
                    {
                        SQL.Append("UPDATE HitPosOrders SET \n "
                                  + "status_time2 = CASE WHEN (2 <= " + Status.ToString() + ") AND (status_time2 IS NULL) THEN '" + TimeChanged + "' ELSE status_time2 END, \n "
                                  + "status_time3 = CASE WHEN (3 <= " + Status.ToString() + ") AND (status_time3 IS NULL) THEN '" + TimeChanged + "' ELSE status_time3 END, \n"
                                  + "status_time4 = CASE WHEN (4 <= " + Status.ToString() + ") AND (status_time4 IS NULL) THEN '" + TimeChanged + "' ELSE status_time4 END, \n"
                                  + "[status] = " + Status.ToString() + " \n"
                                  + "WHERE orderno = " + ExtKey + "; \n"
                            );
                        SQLHp.Append("UPDATE PosUser.Orders SET \n "
                                  + "status_time2 = CASE WHEN (2 <= " + Status.ToString() + ") AND (status_time2 IS NULL) THEN '" + TimeChanged + "' ELSE status_time2 END, \n "
                                  + "status_time3 = CASE WHEN (3 <= " + Status.ToString() + ") AND (status_time3 IS NULL) THEN '" + TimeChanged + "' ELSE status_time3 END, \n"
                                  + "status_time4 = CASE WHEN (4 <= " + Status.ToString() + ") AND (status_time4 IS NULL) THEN '" + TimeChanged + "' ELSE status_time4 END, \n"
                                  + "[status] = " + Status.ToString() + " \n"
                                  + "WHERE orderno = " + ExtKey + " AND shop_id = '" + settings.ShopId + "'; \n"
                            );
                    };
                    if (row.Id != null)
                        updIds = updIds + row.Id.ToString() + ",";
                }

                if (SQL.Length > 0)
                {
                    if (!ExecSQLs(WebPos, SQL.ToString()))
                        return false;
                }
                if (SQLHp.Length > 0)
                {
                    if (!ExecSQLs(HitPos, SQLHp.ToString()))
                        return false;
                }
                if (updIds.Trim() != "")
                {
                    updIds = updIds.Substring(0, updIds.Length - 1);
                    SQL.Clear();
                    SQL.Append("UPDATE OrderStatus SET ExtState = 1 WHERE ID IN (" + updIds + ")");
                    if (!ExecSQLs(WebPos, SQL.ToString()))
                        return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                logger.Error("UpdateHitPosStatuses  SQL1 [" + SQL.ToString() + "]  SQL2 [" + SQLHp.ToString() + "] \r\n " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Update Or Insert new Customer to WebPos
        /// NOT IN USE ANY MORE. API CALL EXITS
        /// </summary>
        /// <param name="WebPos"></param>
        /// <param name="HitPos"></param>
        /// <param name="Code"></param>
        /// <param name="BillingId"></param>
        /// <param name="ShippingID"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        private static bool UpdateDeliveryCustomers(CCUtility WebPos, CCUtility HitPos, string Code,
            out Int64 BillingId, out Int64 ShippingID, out Int64 CustomerId)
        {
            BillingId = 0;
            ShippingID = 0;
            CustomerId = 0;
            string SQL = "";
            try
            {
                SQL = "";
                SQL = "SELECT ISNULL(company_name,ISNULL(fname,'')+' '+ISNULL(name,'')) billingName, * FROM posuser.labcustomer WHERE customerid = '" + Code + "'";
                DataSet dsCust = HitPos.FillDataSet(SQL.ToString());
                DataSet wbCustomer;
                DataSet dsWebCust = new DataSet();
                if (dsCust.Tables.Count > 0 && dsCust.Tables[0].Rows.Count > 0)
                {
                    DataRow row = dsCust.Tables[0].Rows[0];

                    SQL = "UPDATE g SET g.[Address] = " + CCUtility.ToSQL((DBNull.Value.Equals(row["address1"]) ? "" : ConvertToUtf8((String)row["address1"])), FieldTypes.Text) + ", \n"
                        + "      g.City = " + (DBNull.Value.Equals(row["city"]) ? "NULL" : CCUtility.ToSQL((String)row["city"], FieldTypes.Text)) + ", \n"
                        + "      g.PostalCode = " + (DBNull.Value.Equals(row["address_no"]) ? "NULL" : CCUtility.ToSQL((String)row["address_no"], FieldTypes.Text)) + ", \n"
                        + "      g.Telephone = " + (DBNull.Value.Equals(row["tel1"]) ? "NULL" : CCUtility.ToSQL((String)row["tel1"], FieldTypes.Text)) + " \n"
                        + "FROM Guest g \n"
                        + "INNER JOIN Delivery_Customers dc ON dc.ID = g.ProfileNo AND dc.ExtCustId = '" + Code + "'";
                    if (!WebPos.ExeSQL(SQL))
                        return false;

                    int AddressType = -1;
                    int PhoneType = -1;
                    SQL = "SELECT TOP 1 * FROM Delivery_PhoneTypes";
                    dsWebCust.Clear();
                    dsWebCust = WebPos.FillDataSet(SQL);
                    if (dsWebCust.Tables.Count > 0 && dsWebCust.Tables[0].Rows.Count > 0)
                        PhoneType = (int)dsWebCust.Tables[0].Rows[0]["ID"];

                    SQL = "SELECT * FROM Delivery_Customers WHERE ExtCustId = '" + Code + "'";
                    wbCustomer = WebPos.FillDataSet(SQL);
                    if (wbCustomer.Tables.Count < 1 || wbCustomer.Tables[0].Rows.Count < 1)
                    {

                        int CustType = -1;
                        Int64 CustId = -1;

                        SQL = "SELECT TOP 1 * FROM Delivery_CustomersTypes";
                        dsWebCust = WebPos.FillDataSet(SQL);
                        if (dsWebCust.Tables.Count > 0 && dsWebCust.Tables[0].Rows.Count > 0)
                            CustType = (int)dsWebCust.Tables[0].Rows[0]["ID"];
                        SQL = "SELECT TOP 1 * FROM Delivery_AddressTypes";
                        dsWebCust.Clear();
                        dsWebCust = WebPos.FillDataSet(SQL);
                        if (dsWebCust.Tables.Count > 0 && dsWebCust.Tables[0].Rows.Count > 0)
                            AddressType = (int)dsWebCust.Tables[0].Rows[0]["ID"];


                        SQL = "INSERT INTO Delivery_Customers (LastName,FirstName,VatNo,DOY,Floor,email,Comments,CustomerType,ExtCustId,BillingName, \n"
                            + "BillingVatNo,BillingDOY,BillingJob) VALUES ( \n"
                            + (DBNull.Value.Equals(row["fname"]) ? "NULL" : CCUtility.ToSQL((String)row["fname"], FieldTypes.Text)) + ", \n"
                            + (DBNull.Value.Equals(row["name"]) ? "NULL" : CCUtility.ToSQL((String)row["name"], FieldTypes.Text)) + ", \n"
                            + (DBNull.Value.Equals(row["afm"]) ? "NULL" : CCUtility.ToSQL((String)row["afm"], FieldTypes.Text)) + ", \n"
                            + (DBNull.Value.Equals(row["doy"]) ? "NULL" : CCUtility.ToSQL((String)row["doy"], FieldTypes.Text)) + ", \n"
                            + (DBNull.Value.Equals(row["orofos1"]) ? "NULL" : CCUtility.ToSQL((String)row["orofos1"], FieldTypes.Text)) + ", \n"
                            + (DBNull.Value.Equals(row["email"]) ? "NULL" : CCUtility.ToSQL((String)row["email"], FieldTypes.Text)) + ", \n"
                            + (DBNull.Value.Equals(row["remarks"]) ? "NULL" : CCUtility.ToSQL((String)row["remarks"], FieldTypes.Text)) + ", \n"
                            + CCUtility.ToSQL(CustType.ToString(), FieldTypes.Number) + ", \n"
                            + (DBNull.Value.Equals(row["customerid"]) ? "NULL" : CCUtility.ToSQL((String)row["customerid"], FieldTypes.Text)) + ", \n"
                            + (DBNull.Value.Equals(row["billingName"]) ? "NULL" : CCUtility.ToSQL((String)row["billingName"], FieldTypes.Text)) + ", \n"
                            + (DBNull.Value.Equals(row["afm"]) ? "NULL" : CCUtility.ToSQL((String)row["afm"], FieldTypes.Text)) + ", \n"
                            + (DBNull.Value.Equals(row["doy"]) ? "NULL" : CCUtility.ToSQL((String)row["doy"], FieldTypes.Text)) + ", \n"
                            + CCUtility.ToSQL("", FieldTypes.Text) + ") \n";
                        if (!WebPos.ExeSQL(SQL))
                            return false;
                        else
                        {
                            SQL = "SELECT MAX(ID) ID FROM Delivery_Customers WHERE ExtCustId = '" + Code + "'";
                            dsWebCust.Clear();
                            dsWebCust = WebPos.FillDataSet(SQL);
                            CustId = (DBNull.Value.Equals(dsWebCust.Tables[0].Rows[0]["ID"]) ? -1 : (Int64)dsWebCust.Tables[0].Rows[0]["ID"]);
                            CustomerId = CustId;
                        }
                        if (CustId < 1)
                            return false;
                        SQL = "SELECT COUNT(*) nCnt FROM Guest WHERE ProfileNo = " + CustId.ToString();
                        DataSet gst = WebPos.FillDataSet(SQL);

                        if (gst.Tables.Count > 0 && gst.Tables[0].Rows.Count > 0 && (int)gst.Tables[0].Rows[0]["nCnt"] == 0)
                        {
                            try
                            {

                                SQL = "INSERT INTO Guest (ProfileNo, FirstName, LastName, [Address], City, PostalCode, Telephone) VALUES ( \n"
                                    + CustId.ToString() + ", \n"
                                    + (DBNull.Value.Equals(row["fname"]) ? "NULL" : CCUtility.ToSQL((String)row["fname"], FieldTypes.Text)) + ", \n"
                                    + (DBNull.Value.Equals(row["name"]) ? "NULL" : CCUtility.ToSQL((String)row["name"], FieldTypes.Text)) + ", \n"
                                    + CCUtility.ToSQL((DBNull.Value.Equals(row["address1"]) ? "" : ConvertToUtf8((String)row["address1"])) + " " + (DBNull.Value.Equals(row["address1"]) ? "" : ConvertToUtf8((String)row["address_no"])), FieldTypes.Text) + ", \n"
                                    + (DBNull.Value.Equals(row["city"]) ? "NULL" : CCUtility.ToSQL((String)row["city"], FieldTypes.Text)) + ", \n"
                                    + (DBNull.Value.Equals(row["zipcode"]) ? "NULL" : CCUtility.ToSQL((String)row["zipcode"], FieldTypes.Text)) + ", \n"
                                    + (DBNull.Value.Equals(row["tel1"]) ? "NULL" : CCUtility.ToSQL((String)row["tel1"], FieldTypes.Text)) + "); \n";
                                if (!WebPos.ExeSQL(SQL))
                                    return false;
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.ToString());
                            }
                        }
                        string tel1 = "";
                        tel1 = ((String)row["tel1"]).Trim();
                        if (string.IsNullOrEmpty(tel1))
                            tel1 = "";
                        if (!DBNull.Value.Equals(row["tel1"]) && ((String)row["tel1"]).Trim() != "")
                        {
                            SQL = "INSERT INTO Delivery_CustomersPhones (CustomerID,PhoneNumber,PhoneType,IsSelected) VALUES ( \n"
                                + CCUtility.ToSQL(CustId.ToString(), FieldTypes.Number) + ", \n"
                                + CCUtility.ToSQL((String)row["tel1"], FieldTypes.Text) + ", \n"
                                + CCUtility.ToSQL(PhoneType.ToString(), FieldTypes.Number) + ", 'True') ";
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                        }
                        if (string.IsNullOrEmpty(tel1))
                            ((String)row["tel2"]).Trim();
                        if (string.IsNullOrEmpty(tel1))
                            tel1 = "";
                        if (!DBNull.Value.Equals(row["tel2"]) && ((String)row["tel2"]).Trim() != "")
                        {
                            SQL = "INSERT INTO Delivery_CustomersPhones (CustomerID,PhoneNumber,PhoneType,IsSelected) VALUES ( \n"
                                + CCUtility.ToSQL(CustId.ToString(), FieldTypes.Number) + ", \n"
                                + CCUtility.ToSQL((String)row["tel2"], FieldTypes.Text) + ", \n"
                                + CCUtility.ToSQL(PhoneType.ToString(), FieldTypes.Number) + ",'False') ";
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                        }
                        if (!DBNull.Value.Equals(row["fax"]) && ((String)row["fax"]).Trim() != "")
                        {
                            SQL = "INSERT INTO Delivery_CustomersPhones (CustomerID,PhoneNumber,PhoneType,IsSelected) VALUES ( \n"
                                + CCUtility.ToSQL(CustId.ToString(), FieldTypes.Number) + ", \n"
                                + CCUtility.ToSQL((String)row["fax"], FieldTypes.Text) + ", \n"
                                + CCUtility.ToSQL(PhoneType.ToString(), FieldTypes.Number) + ",'False') ";
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                        }
                        if (string.IsNullOrEmpty(tel1))
                            ((String)row["mobile"]).Trim();
                        if (string.IsNullOrEmpty(tel1))
                            tel1 = "";
                        if (!DBNull.Value.Equals(row["mobile"]) && ((String)row["mobile"]).Trim() != "")
                        {
                            SQL = "INSERT INTO Delivery_CustomersPhones (CustomerID,PhoneNumber,PhoneType,IsSelected) VALUES ( \n"
                                + CCUtility.ToSQL(CustId.ToString(), FieldTypes.Number) + ", \n"
                                + CCUtility.ToSQL((String)row["mobile"], FieldTypes.Text) + ", \n"
                                + CCUtility.ToSQL(PhoneType.ToString(), FieldTypes.Number) + ",'False') ";
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                        }

                        if (!string.IsNullOrEmpty(tel1))
                        {
                            SQL = "UPDATE Delivery_CustomersPhones SET IsSelected = 'False' WHERE CustomerID = " + CCUtility.ToSQL(CustId.ToString(), FieldTypes.Number);
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                            SQL = "UPDATE Delivery_CustomersPhones SET IsSelected = 'True' WHERE CustomerID = " + CCUtility.ToSQL(CustId.ToString(), FieldTypes.Number) + " AND \n"
                                + " PhoneNumber = '" + tel1 + "'";
                            if (!WebPos.ExeSQL(SQL))
                                return false;

                        }

                        if (!DBNull.Value.Equals(row["address1"]) && ((String)row["address1"]).Trim() != "")
                        {
                            SQL = "UPDATE Delivery_CustomersShippingAddress SET IsSelected = 'False' WHERE CustomerID = " + CCUtility.ToSQL(CustId.ToString(), FieldTypes.Number);
                            if (!WebPos.ExeSQL(SQL))
                                return false;

                            SQL = "INSERT INTO Delivery_CustomersShippingAddress (CustomerID,AddressStreet,AddressNo,City,Zipcode,Type,IsSelected) VALUES ( \n"
                                + CCUtility.ToSQL(CustId.ToString(), FieldTypes.Number) + ", \n"
                                + CCUtility.ToSQL(((String)row["address1"]).Trim(), FieldTypes.Text) + ", \n"
                                + (DBNull.Value.Equals(row["address_no"]) ? "NULL" : CCUtility.ToSQL((String)row["address_no"], FieldTypes.Text)) + ", \n"
                                + (DBNull.Value.Equals(row["city"]) ? "NULL" : CCUtility.ToSQL((String)row["city"], FieldTypes.Text)) + ", \n"
                                + (DBNull.Value.Equals(row["zipcode"]) ? "NULL" : CCUtility.ToSQL((String)row["zipcode"], FieldTypes.Text)) + ", \n"
                                + CCUtility.ToSQL(AddressType.ToString(), FieldTypes.Number) + ",'True')";
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                        }
                        if (!DBNull.Value.Equals(row["bl_address"]) && ((String)row["bl_address"]).Trim() != "")
                        {
                            SQL = "UPDATE Delivery_CustomersBillingAddress SET IsSelected = 'False' WHERE CustomerID = " + CCUtility.ToSQL(CustId.ToString(), FieldTypes.Number);
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                            SQL = "INSERT INTO Delivery_CustomersBillingAddress (CustomerID,AddressStreet,AddressNo,City,Zipcode,Type,IsSelected) VALUES ( \n"
                                + CCUtility.ToSQL(CustId.ToString(), FieldTypes.Number) + ", \n"
                                + CCUtility.ToSQL(((String)row["bl_address"]).Trim(), FieldTypes.Text) + ", \n"
                                + (DBNull.Value.Equals(row["bl_address_no"]) ? "NULL" : CCUtility.ToSQL((String)row["bl_address_no"], FieldTypes.Text)) + ", \n"
                                + (DBNull.Value.Equals(row["bl_city"]) ? "NULL" : CCUtility.ToSQL((String)row["bl_city"], FieldTypes.Text)) + ", \n"
                                + (DBNull.Value.Equals(row["zipcode"]) ? "NULL" : CCUtility.ToSQL((String)row["zipcode"], FieldTypes.Text)) + ", \n"
                                + CCUtility.ToSQL(AddressType.ToString(), FieldTypes.Number) + ",'True')";
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                        }

                        SQL = "INSERT INTO Delivery_CustomersPhonesAndAddress (CustomerID, PhoneID, AddressID,IsShipping) \n"
                            + "SELECT * \n"
                            + "FROM ( \n"
                            + "     SELECT cp.CustomerID, cp.ID PhoneId, dl.ID BillId, 0 IsShipping \n"
                            + "     FROM Delivery_CustomersPhones cp \n"
                            + "     INNER JOIN Delivery_CustomersBillingAddress dl ON dl.CustomerID = cp.CustomerID \n "
                            + "     WHERE cp.CustomerID = " + CustId.ToString() + " \n"
                            + "     UNION ALL \n"
                            + "     SELECT cp.CustomerID, cp.ID PhoneId, dl.ID ShipId, 1 IsShipping \n"
                            + "     FROM Delivery_CustomersPhones cp \n"
                            + "     INNER JOIN Delivery_CustomersShippingAddress dl ON dl.CustomerID = cp.CustomerID \n"
                            + "     WHERE cp.CustomerID = " + CustId.ToString() + " \n"
                            + ") a \n"
                            + "WHERE CAST(a.CustomerID AS VARCHAR(20)) + '-' + CAST(a.PhoneId AS VARCHAR(20)) + '-' + CAST(a.BillId AS VARCHAR(20)) + '-' +CAST(a.IsShipping AS VARCHAR(20)) NOT IN ( \n"
                            + "     SELECT CAST(CustomerID AS VARCHAR(20)) + '-' + CAST(PhoneID AS VARCHAR(20)) + '-' + CAST(AddressID AS VARCHAR(20)) + '-' + CAST(IsShipping AS VARCHAR(20)) \n"
                            + "     FROM Delivery_CustomersPhonesAndAddress \n"
                            + "     WHERE CustomerID = " + CustId.ToString() + ")";
                        if (!WebPos.ExeSQL(SQL))
                            return false;
                        SQL = "SELECT TOP 1 * FROM Delivery_CustomersShippingAddress WHERE CustomerID = " + CustId.ToString() + " ORDER BY ID DESC";
                        dsWebCust.Clear();
                        dsWebCust = WebPos.FillDataSet(SQL);
                        if (dsWebCust.Tables.Count > 0 && dsWebCust.Tables[0].Rows.Count > 0)
                            ShippingID = (Int64)dsWebCust.Tables[0].Rows[0]["ID"];
                        SQL = "SELECT TOP 1 * FROM Delivery_CustomersBillingAddress WHERE CustomerID = " + CustId.ToString() + " ORDER BY ID DESC";
                        dsWebCust.Clear();
                        dsWebCust = WebPos.FillDataSet(SQL);
                        if (dsWebCust.Tables.Count > 0 && dsWebCust.Tables[0].Rows.Count > 0)
                            BillingId = (Int64)dsWebCust.Tables[0].Rows[0]["ID"];
                        if (ShippingID <= 0 && BillingId > 0)
                            ShippingID = BillingId;
                        else
                        if (BillingId <= 0 && ShippingID > 0)
                            BillingId = ShippingID;
                    }
                    else
                    {
                        CustomerId = (Int64)wbCustomer.Tables[0].Rows[0]["ID"];

                        bool bUpdPhone = false;
                        SQL = "SELECT TOP 1 * FROM Delivery_AddressTypes";
                        dsWebCust = WebPos.FillDataSet(SQL);
                        if (dsWebCust.Tables.Count > 0 && dsWebCust.Tables[0].Rows.Count > 0)
                            AddressType = (int)dsWebCust.Tables[0].Rows[0]["ID"];

                        DataRow custRow = wbCustomer.Tables[0].Rows[0];
                        if (!DBNull.Value.Equals(row["address1"]) && ((String)row["address1"]).Trim() != "")
                        {
                            SQL = "SELECT ISNULL(COUNT(*),0) nCnt FROM Delivery_CustomersShippingAddress WHERE CustomerID = " + ((Int64)custRow["ID"]).ToString() + " \n"
                            + " AND AddressStreet = '" + ((String)row["address1"]).Trim() + "'";
                            dsWebCust.Clear();
                            dsWebCust = WebPos.FillDataSet(SQL);
                            if (dsWebCust.Tables.Count < 1 || dsWebCust.Tables[0].Rows.Count < 1 || ((int)dsWebCust.Tables[0].Rows[0]["nCnt"] < 1))
                            {
                                SQL = "UPDATE Delivery_CustomersShippingAddress SET IsSelected = 'False' WHERE CustomerID = " + CCUtility.ToSQL(((Int64)custRow["ID"]).ToString(), FieldTypes.Number);
                                if (!WebPos.ExeSQL(SQL))
                                    return false;
                                SQL = "INSERT INTO Delivery_CustomersShippingAddress (CustomerID,AddressStreet,AddressNo,City,Zipcode,Type,IsSelected) VALUES ( \n"
                                    + CCUtility.ToSQL(((Int64)custRow["ID"]).ToString(), FieldTypes.Number) + ", \n"
                                    + CCUtility.ToSQL(((String)row["address1"]).Trim(), FieldTypes.Text) + ", \n"
                                    + (DBNull.Value.Equals(row["address_no"]) ? "NULL" : CCUtility.ToSQL((String)row["address_no"], FieldTypes.Text)) + ", \n"
                                    + (DBNull.Value.Equals(row["city"]) ? "NULL" : CCUtility.ToSQL((String)row["city"], FieldTypes.Text)) + ", \n"
                                    + (DBNull.Value.Equals(row["zipcode"]) ? "NULL" : CCUtility.ToSQL((String)row["zipcode"], FieldTypes.Text)) + ", \n"
                                    + CCUtility.ToSQL(AddressType.ToString(), FieldTypes.Number) + ",'True')";
                                if (!WebPos.ExeSQL(SQL))
                                    return false;
                                bUpdPhone = true;
                            }
                            else
                            {
                                SQL = "UPDATE Delivery_CustomersShippingAddress SET IsSelected = 'False' \n"
                                    + "WHERE CustomerID = " + CCUtility.ToSQL(((Int64)custRow["ID"]).ToString(), FieldTypes.Number);
                                if (!WebPos.ExeSQL(SQL))
                                    return false;
                                SQL = "UPDATE Delivery_CustomersShippingAddress SET IsSelected = 'True' \n"
                                    + "WHERE CustomerID = " + CCUtility.ToSQL(((Int64)custRow["ID"]).ToString(), FieldTypes.Number) + " AND \n"
                                    + " AddressStreet = '" + ((String)row["address1"]).Trim() + "'";
                                if (!WebPos.ExeSQL(SQL))
                                    return false;
                            }
                        }

                        if (!DBNull.Value.Equals(row["bl_address"]) && ((String)row["bl_address"]).Trim() != "")
                        {
                            SQL = "SELECT ISNULL(COUNT(*),0) nCnt FROM Delivery_CustomersBillingAddress WHERE CustomerID = " + ((Int64)custRow["ID"]).ToString() + " \n"
                            + " AND AddressStreet = '" + ((String)row["bl_address"]).Trim() + "'";
                            dsWebCust.Clear();
                            dsWebCust = WebPos.FillDataSet(SQL);
                            if (dsWebCust.Tables.Count < 1 || dsWebCust.Tables[0].Rows.Count < 1 || ((int)dsWebCust.Tables[0].Rows[0]["nCnt"] < 1))
                            {
                                SQL = "UPDATE Delivery_CustomersBillingAddress SET IsSelected = 'False' WHERE CustomerID = " + CCUtility.ToSQL(((Int64)custRow["ID"]).ToString(), FieldTypes.Number);
                                if (!WebPos.ExeSQL(SQL))
                                    return false;
                                SQL = "INSERT INTO Delivery_CustomersBillingAddress (CustomerID,AddressStreet,AddressNo,City,Zipcode,Type,IsSelected) VALUES ( \n"
                                    + CCUtility.ToSQL(((Int64)custRow["ID"]).ToString(), FieldTypes.Number) + ", \n"
                                    + CCUtility.ToSQL(((String)row["bl_address"]).Trim(), FieldTypes.Text) + ", \n"
                                    + (DBNull.Value.Equals(row["bl_address_no"]) ? "NULL" : CCUtility.ToSQL((String)row["bl_address_no"], FieldTypes.Text)) + ", \n"
                                    + (DBNull.Value.Equals(row["bl_city"]) ? "NULL" : CCUtility.ToSQL((String)row["bl_city"], FieldTypes.Text)) + ", \n"
                                    + (DBNull.Value.Equals(row["zipcode"]) ? "NULL" : CCUtility.ToSQL((String)row["zipcode"], FieldTypes.Text)) + ", \n"
                                    + CCUtility.ToSQL(AddressType.ToString(), FieldTypes.Number) + ",'True')";
                                if (!WebPos.ExeSQL(SQL))
                                    return false;
                                bUpdPhone = true;
                            }
                            else
                            {
                                SQL = "UPDATE Delivery_CustomersBillingAddress SET IsSelected = 'False' \n"
                                    + "WHERE CustomerID = " + CCUtility.ToSQL(((Int64)custRow["ID"]).ToString(), FieldTypes.Number);
                                if (!WebPos.ExeSQL(SQL))
                                    return false;
                                SQL = "UPDATE Delivery_CustomersBillingAddress SET IsSelected = 'True' \n"
                                    + "WHERE CustomerID = " + CCUtility.ToSQL(((Int64)custRow["ID"]).ToString(), FieldTypes.Number) + " AND \n"
                                    + " AddressStreet = '" + ((String)row["bl_address"]).Trim() + "'";
                                if (!WebPos.ExeSQL(SQL))
                                    return false;
                            }
                        }

                        string tel = "";
                        tel = ((String)row["tel1"]).Trim();
                        if (string.IsNullOrEmpty(tel))
                            tel = ((String)row["tel2"]).Trim();
                        if (string.IsNullOrEmpty(tel))
                            tel = ((String)row["mobile"]).Trim();
                        if (!string.IsNullOrEmpty(tel))
                        {
                            SQL = "UPDATE Delivery_CustomersPhones SET IsSelected = 'False' WHERE CustomerID = " + ((Int64)custRow["ID"]).ToString();
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                            SQL = "SELECT ISNULL(COUNT(*),0) nCnt FROM Delivery_CustomersPhones WHERE CustomerID = " + ((Int64)custRow["ID"]).ToString() + " AND PhoneNumber = '" + tel + "'";
                            dsWebCust.Clear();
                            dsWebCust = WebPos.FillDataSet(SQL);
                            if (dsWebCust.Tables.Count < 1 || dsWebCust.Tables[0].Rows.Count < 1 || ((int)dsWebCust.Tables[0].Rows[0]["nCnt"] < 1))
                            {
                                SQL = "INSERT INTO Delivery_CustomersPhones (CustomerID,PhoneNumber,PhoneType,IsSelected) VALUES ( \n"
                                    + CCUtility.ToSQL(((Int64)custRow["ID"]).ToString(), FieldTypes.Number) + ", \n"
                                    + CCUtility.ToSQL(tel, FieldTypes.Text) + ", \n"
                                    + CCUtility.ToSQL(PhoneType.ToString(), FieldTypes.Number) + ", 'True') ";
                                bUpdPhone = true;
                            }
                            else
                                SQL = "UPDATE Delivery_CustomersPhones SET IsSelected = 'True' WHERE CustomerID = " + CCUtility.ToSQL(((Int64)custRow["ID"]).ToString(), FieldTypes.Number) + " AND \n"
                                    + " PhoneNumber = '" + tel + "'";
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                        }

                        if (bUpdPhone)
                        {
                            SQL = "INSERT INTO Delivery_CustomersPhonesAndAddress (CustomerID, PhoneID, AddressID,IsShipping) \n"
                            + "SELECT * \n"
                            + "FROM ( \n"
                            + "     SELECT cp.CustomerID, cp.ID PhoneId, dl.ID BillId, 0 IsShipping \n"
                            + "     FROM Delivery_CustomersPhones cp \n"
                            + "     INNER JOIN Delivery_CustomersBillingAddress dl ON dl.CustomerID = cp.CustomerID \n "
                            + "     WHERE cp.CustomerID = " + ((Int64)custRow["ID"]).ToString() + " \n"
                            + "     UNION ALL \n"
                            + "     SELECT cp.CustomerID, cp.ID PhoneId, dl.ID ShipId, 1 IsShipping \n"
                            + "     FROM Delivery_CustomersPhones cp \n"
                            + "     INNER JOIN Delivery_CustomersShippingAddress dl ON dl.CustomerID = cp.CustomerID \n"
                            + "     WHERE cp.CustomerID = " + ((Int64)custRow["ID"]).ToString() + " \n"
                            + ") a \n"
                            + "WHERE CAST(a.CustomerID AS VARCHAR(20)) + '-' + CAST(a.PhoneId AS VARCHAR(20)) + '-' + CAST(a.BillId AS VARCHAR(20)) + '-' +CAST(a.IsShipping AS VARCHAR(20)) NOT IN ( \n"
                            + "     SELECT CAST(CustomerID AS VARCHAR(20)) + '-' + CAST(PhoneID AS VARCHAR(20)) + '-' + CAST(AddressID AS VARCHAR(20)) + '-' + CAST(IsShipping AS VARCHAR(20)) \n"
                            + "     FROM Delivery_CustomersPhonesAndAddress \n"
                            + "     WHERE CustomerID = " + ((Int64)custRow["ID"]).ToString() + ")";
                            if (!WebPos.ExeSQL(SQL))
                                return false;
                        }

                        SQL = "SELECT TOP 1 * FROM Delivery_CustomersShippingAddress WHERE CustomerID = " + ((Int64)custRow["ID"]).ToString() + " ORDER BY ID DESC";
                        dsWebCust.Clear();
                        dsWebCust = WebPos.FillDataSet(SQL);
                        if (dsWebCust.Tables.Count > 0 && dsWebCust.Tables[0].Rows.Count > 0)
                            ShippingID = (Int64)dsWebCust.Tables[0].Rows[0]["ID"];
                        SQL = "SELECT TOP 1 * FROM Delivery_CustomersBillingAddress WHERE CustomerID = " + ((Int64)custRow["ID"]).ToString() + " ORDER BY ID DESC";
                        dsWebCust.Clear();
                        dsWebCust = WebPos.FillDataSet(SQL);
                        if (dsWebCust.Tables.Count > 0 && dsWebCust.Tables[0].Rows.Count > 0)
                            BillingId = (Int64)dsWebCust.Tables[0].Rows[0]["ID"];
                        if (ShippingID <= 0 && BillingId > 0)
                            ShippingID = BillingId;
                        else
                        if (BillingId <= 0 && ShippingID > 0)
                            BillingId = ShippingID;

                        SQL = "SELECT COUNT(*) nCnt FROM Guest WHERE ProfileNo = " + CustomerId.ToString();
                        wbCustomer = WebPos.FillDataSet(SQL);
                        if (wbCustomer.Tables.Count < 1 || wbCustomer.Tables[0].Rows.Count < 1 || ((int)wbCustomer.Tables[0].Rows[0]["nCnt"]) == 0)
                        {
                            SQL = " SELECT dc.LastName, dc.FirstName, da.AddressStreet, da.City, da.Zipcode, ISNULL(dcp.PhoneNumber,'') PhoneNumber \n"
                                 + " FROM Delivery_Customers dc \n"
                                + " OUTER APPLY( \n"
                                + "     SELECT TOP 1 ISNULL(dcba.AddressStreet,'')+' '+ISNULL(dcba.AddressNo,'') AddressStreet, dcba.City, dcba.Zipcode \n"
                                + "     FROM Delivery_CustomersBillingAddress dcba \n"
                                + "     WHERE dcba.CustomerID = dc.ID \n"
                                + " ) da \n"
                                + "OUTER APPLY( \n"
                                + "    SELECT TOP 1 dcp.PhoneNumber \n"
                                + "    FROM Delivery_CustomersPhones dcp \n"
                                + "    WHERE dcp.CustomerID = dc.ID \n"
                                + ") dcp \n"
                                + " WHERE dc.Id = " + CustomerId.ToString();
                            dsWebCust = WebPos.FillDataSet(SQL);
                            if (dsWebCust.Tables.Count > 0 && dsWebCust.Tables[0].Rows.Count > 0)
                            {
                                row = dsWebCust.Tables[0].Rows[0];
                                SQL = "INSERT INTO Guest (ProfileNo, FirstName, LastName, [Address], City, PostalCode, Telephone) VALUES ( \n"
                                    + CustomerId.ToString() + ", \n"
                                    + (DBNull.Value.Equals(row["FirstName"]) ? "NULL" : CCUtility.ToSQL((String)row["FirstName"], FieldTypes.Text)) + ", \n"
                                    + (DBNull.Value.Equals(row["LastName"]) ? "NULL" : CCUtility.ToSQL((String)row["LastName"], FieldTypes.Text)) + ", \n"
                                    + CCUtility.ToSQL((DBNull.Value.Equals(row["AddressStreet"]) ? "" : ConvertToUtf8((String)row["AddressStreet"])), FieldTypes.Text) + ", \n"
                                    + (DBNull.Value.Equals(row["City"]) ? "NULL" : CCUtility.ToSQL((String)row["City"], FieldTypes.Text)) + ", \n"
                                    + (DBNull.Value.Equals(row["Zipcode"]) ? "NULL" : CCUtility.ToSQL((String)row["Zipcode"], FieldTypes.Text)) + ", \n"
                                    + (DBNull.Value.Equals(row["PhoneNumber"]) ? "NULL" : CCUtility.ToSQL((String)row["PhoneNumber"], FieldTypes.Text)) + "); \n";
                                if (!WebPos.ExeSQL(SQL))
                                    return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error("UpdateDeliveryCustomers SQL [" + SQL + "] \r\n " + ex.ToString());
                return false;
            }

        }

        /// <summary>
        /// Insert new ingrentients from HitPos.Orders
        /// </summary>
        /// <param name="hitPos"></param>
        /// <param name="WebPos"></param>
        /// <returns></returns>
        private static bool InsertNewIngredients(SqlConnection hitPos, SqlConnection WebPos)
        {
            StringBuilder SQL = new StringBuilder();
            try
            {
                List<Ingredients> ing = new List<Ingredients>();
                //List<Ingredients> InsertIng = new List<Ingredients>();
                //List<string> WbIng = new List<string>();
                string CodeDescr = "";
                string IngrDescr = "";
                Int64 UnitId = -1;

                SQL.Clear();
                SQL.Append("SELECT TOP 1 ID FROM Units");
                UnitId = WebPos.Query<Int64>(SQL.ToString()).FirstOrDefault();

                SQL.Clear();
                SQL.Append("SELECT i.DescA [Description], i.DescB [ExtendedDescription], i.DescA [SalesDescription],  \n"
                     + "   CAST(i.codart AS VARCHAR(20)) Code \n"
                     + "FROM[posuser].[ITEMS] i \n"
                     + "WHERE i.codart BETWEEN 1000 AND 1300 AND i.DescA IS NOT NULL");

                List<HitPosIngredientsModels> dsIng = hitPos.Query<HitPosIngredientsModels>(SQL.ToString()).ToList();

                foreach (HitPosIngredientsModels row in dsIng)
                {
                    CodeDescr = ((string.IsNullOrEmpty(row.Code) ? "" : row.Code) + "-" + (string.IsNullOrEmpty(row.Description) ? "" : row.Description)).Replace("\'", "\'\'");

                    SQL.Clear();
                    SQL.Append("SELECT ISNULL(COUNT(*),0) nCnt FROM Ingredients WHERE Code = '" + CodeDescr + "'");
                    int nCnt = WebPos.Query<int>(SQL.ToString()).FirstOrDefault();

                    if (nCnt < 1)
                    {
                        Ingredients ingOb = new Ingredients();
                        ingOb.Description = row.Description ?? null;
                        ingOb.ExtendedDescription = row.ExtendedDescription ?? null;
                        ingOb.SalesDescription = row.SalesDescription ?? null;
                        ingOb.Qty = null;
                        ingOb.ItemId = null;
                        ingOb.UnitId = UnitId;
                        ingOb.Code = row.Code ?? null;
                        ingOb.IsDeleted = null;
                        ingOb.Background = null;
                        ingOb.Color = null;
                        ing.Add(ingOb);
                    }
                }

                SQL.Clear();
                foreach (Ingredients item in ing)
                {
                    if (string.IsNullOrEmpty(item.Description))
                        IngrDescr = "";
                    else
                    {
                        if (item.Description.Length > 50)
                            IngrDescr = item.Description.Substring(0, 50);
                        else
                            IngrDescr = item.Description;
                    }

                    CodeDescr = (string.IsNullOrEmpty(item.Code) ? "" : item.Code) + "-" +
                                (string.IsNullOrEmpty(item.Description) ? "" : item.Description);
                    SQL.Append("INSERT INTO Ingredients([Description],ExtendedDescription,SalesDescription,Qty,ItemId,UnitId,Code,IsDeleted,Background,Color) VALUES ( \n"
                        + (string.IsNullOrEmpty(item.Description) ? "NULL" : CCUtility.ToSQL(item.Description, FieldTypes.Text)) + ", \n"
                        + (string.IsNullOrEmpty(item.ExtendedDescription) ? "NULL" : CCUtility.ToSQL(item.ExtendedDescription, FieldTypes.Text)) + ", \n"
                        + (string.IsNullOrEmpty(item.SalesDescription) ? "NULL" : CCUtility.ToSQL(item.SalesDescription, FieldTypes.Text)) + ", \n"
                        + (item.Qty == null ? "NULL" : CCUtility.ToSQL(item.Qty.ToString(), FieldTypes.Number)) + ", \n"
                        + (item.ItemId == null ? "NULL" : CCUtility.ToSQL(item.ItemId.ToString(), FieldTypes.Number)) + ", \n"
                        + CCUtility.ToSQL(item.UnitId.ToString(), FieldTypes.Number) + ", \n"
                        + CCUtility.ToSQL(CodeDescr, FieldTypes.Text) + ", \n"
                        + (item.IsDeleted == null ? "NULL" : CCUtility.ToSQL(item.IsDeleted.ToString(), FieldTypes.Number)) + ", \n"
                        + (string.IsNullOrEmpty(item.Background) ? "NULL" : CCUtility.ToSQL(item.Background, FieldTypes.Text)) + ", \n"
                        + (string.IsNullOrEmpty(item.Color) ? "NULL" : CCUtility.ToSQL(item.Color, FieldTypes.Text)) + "); \n");
                }
                if (SQL.Length > 0)
                {
                    if (!ExecSQLs(WebPos, SQL.ToString()))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error("InsertNewIngredients SQL [" + SQL.ToString() + "] \r\n " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Insert nee Ingredients from ATCOM
        /// </summary>
        /// <param name="hitPos"></param>
        /// <param name="WebPos"></param>
        /// <returns></returns>
        private static bool InsertAtcomIngredients(SqlConnection hitPos, SqlConnection WebPos)
        {
            StringBuilder SQL = new StringBuilder();
            try
            {
                List<Ingredients> ing = new List<Ingredients>();
                string CodeDescr = "";
                string IngrDescr = "";

                Int64 UnitId = -1;

                SQL.Clear();
                SQL.Append("SELECT TOP 1 ID FROM Units");
                UnitId = WebPos.Query<Int64>(SQL.ToString()).FirstOrDefault();


                SQL.Clear();
                SQL.Append("SELECT DISTINCT CAST(o.item_code AS VARCHAR(20)) Code, o.item_descr Description \n"
                      + "FROM posuser.orders o \n"
                      + "INNER JOIN ( \n"
                      + "   SELECT o.orderno \n"
                      + "   FROM posuser.orders o \n"
                      + "   WHERE o.flag_up IS NULL AND o.shop_id = '" + settings.ShopId + "'  \n"
                      + "   GROUP BY o.orderno, o.mqty \n"
                      + "   HAVING(COUNT(o.orderno) = o.mqty) \n"
                      + ") ord ON ord.orderno = o.orderno \n"
                      + "WHERE o.item_code BETWEEN 1000 AND 1300 AND ISNULL(o.item_descr,'') <> ''");

                List<HitPosIngredientsModels> dsIng = hitPos.Query<HitPosIngredientsModels>(SQL.ToString()).ToList();
                string itemDescr;

                foreach (HitPosIngredientsModels row in dsIng)
                {
                    itemDescr = string.IsNullOrEmpty(row.Description) ? "" : row.Description;
                    itemDescr = itemDescr.Replace("'", "");
                    itemDescr = itemDescr.Replace("!", "");
                    itemDescr = itemDescr.Replace("@", "");
                    itemDescr = itemDescr.Replace("#", "");
                    itemDescr = itemDescr.Replace("$", "");
                    itemDescr = itemDescr.Replace("%", "");
                    itemDescr = itemDescr.Replace("^", "");
                    itemDescr = itemDescr.Replace("&", "");
                    itemDescr = itemDescr.Replace("*", "");
                    itemDescr = itemDescr.Replace("(", "");
                    itemDescr = itemDescr.Replace(")", "");
                    itemDescr = itemDescr.Replace("+", "");
                    itemDescr = itemDescr.Replace("=", "");
                    itemDescr = itemDescr.Replace("<", "");
                    itemDescr = itemDescr.Replace(">", "");
                    itemDescr = itemDescr.Replace("[", "");
                    itemDescr = itemDescr.Replace("]", "");
                    itemDescr = itemDescr.Replace("{", "");
                    itemDescr = itemDescr.Replace("}", "");
                    itemDescr = itemDescr.Replace(@"\", "");

                    CodeDescr = ((string.IsNullOrEmpty(row.Code) ? "" : row.Code) + "-" + itemDescr).Replace("\'", "\'\'");
                    SQL.Clear();
                    SQL.Append("SELECT ISNULL(COUNT(*),0) nCnt FROM Ingredients WHERE Code = '" + CodeDescr + "'");
                    int exists = WebPos.Query<int>(SQL.ToString()).FirstOrDefault();
                    if (exists < 1)
                    {
                        Ingredients ingOb = new Ingredients();
                        ingOb.Description = itemDescr.Replace("\'", "\'\'");
                        ingOb.ExtendedDescription = itemDescr.Replace("\'", "\'\'");
                        ingOb.SalesDescription = itemDescr.Replace("\'", "\'\'");
                        ingOb.Qty = null;
                        ingOb.ItemId = null;
                        ingOb.UnitId = UnitId;
                        ingOb.Code = (string.IsNullOrEmpty(row.Code) ? null : row.Code);
                        ingOb.IsDeleted = null;
                        ingOb.Background = null;
                        ingOb.Color = null;
                        ing.Add(ingOb);
                    }
                }
                SQL.Clear();
                foreach (Ingredients item in ing)
                {
                    CodeDescr = (string.IsNullOrEmpty(item.Code) ? "" : item.Code) + "-" +
                                (string.IsNullOrEmpty(item.Description) ? "" : item.Description);
                    if (string.IsNullOrEmpty(item.Description))
                        IngrDescr = "";
                    else
                    {
                        if (item.Description.Length > 50)
                            IngrDescr = item.Description.Substring(0, 50);
                        else
                            IngrDescr = item.Description;
                    }

                    SQL.Append("INSERT INTO Ingredients([Description],ExtendedDescription,SalesDescription,Qty,ItemId,UnitId,Code,IsDeleted,Background,Color) VALUES ( \n"
                        + (string.IsNullOrEmpty(IngrDescr) ? "NULL" : CCUtility.ToSQL(IngrDescr, FieldTypes.Text)) + ", \n"
                        + (string.IsNullOrEmpty(item.ExtendedDescription) ? "NULL" : CCUtility.ToSQL(item.ExtendedDescription, FieldTypes.Text)) + ", \n"
                        + (string.IsNullOrEmpty(item.SalesDescription) ? "NULL" : CCUtility.ToSQL(item.SalesDescription, FieldTypes.Text)) + ", \n"
                        + (item.Qty == null ? "NULL" : CCUtility.ToSQL(item.Qty.ToString(), FieldTypes.Number)) + ", \n"
                        + (item.ItemId == null ? "NULL" : CCUtility.ToSQL(item.ItemId.ToString(), FieldTypes.Number)) + ", \n"
                        + CCUtility.ToSQL(item.UnitId.ToString(), FieldTypes.Number) + ", \n"
                        + CCUtility.ToSQL(CodeDescr, FieldTypes.Text) + ", \n"
                        + (item.IsDeleted == null ? "NULL" : CCUtility.ToSQL(item.IsDeleted.ToString(), FieldTypes.Number)) + ", \n"
                        + (string.IsNullOrEmpty(item.Background) ? "NULL" : CCUtility.ToSQL(item.Background, FieldTypes.Text)) + ", \n"
                        + (string.IsNullOrEmpty(item.Color) ? "NULL" : CCUtility.ToSQL(item.Color, FieldTypes.Text)) + "); \n");
                }
                if (SQL.Length > 0)
                {
                    if (!ExecSQLs(WebPos, SQL.ToString()))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error("InsertAtcomIngredients SQL [" + SQL.ToString() + "] \r\n " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Get's Map Position for customer
        /// </summary>
        /// <param name="address"></param>
        /// <param name="Latintude"></param>
        /// <param name="Longtitude"></param>
        private static void GetAddressPosition(string address, out double Latintude, out double Longtitude)
        {
            Latintude = 0;
            Longtitude = 0;
            try
            {
                IGeocoder geocoder = new GoogleGeocoder() { };
                //Address[] addresses = geocoder.Geocode(address).ToArray();
                //address = "Ιωνίας 15 Ανθούπολη";
                //address = "dfsa gsdg sdfgsdgs";

                var addresses = geocoder.Geocode(address);
                int nCnt = 0;
                foreach (Address adrs in addresses)
                {
                    if (nCnt > 1)
                        break;
                    Latintude = adrs.Coordinates.Latitude;
                    Longtitude = adrs.Coordinates.Longitude;
                    nCnt++;
                }
                if (nCnt > 1)
                {
                    Latintude = 0;
                    Longtitude = 0;
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult != -2146233088)
                    logger.Error("GetAddressPosition \r\n " + ex.ToString());
            }
        }

        /// <summary>
        /// Get's List of payment for WepPos Receipt
        /// </summary>
        /// <param name="wbCon"></param>
        /// <param name="OrderNo"></param>
        /// <param name="GuestId"></param>
        /// <param name="Guest"></param>
        /// <returns></returns>
        private static List<PaymentsDetails> GetPayments(SqlConnection wbCon, Int64 OrderNo, long GuestId, string Guest)
        {
            //long GuestId = -1;

            //CustomerModel cm = new CustomerModel();

            //cm.Address = "";
            //cm.FirstName = "";
            //cm.LastName = "";
            //cm.ProfileNo = 0;
            //cm.City = "";
            //if (CustId > 0)
            //{
            //    string SQLC = "";
            //    try
            //    {
            //        SQLC = "SELECT ISNULL(dcba.sAddress, ISNULL(dcsa.sAddress,'')) [Address], ISNULL(dc.FirstName,'') FirstName, ISNULL(dc.LastName,'') LastName, ISNULL(dcba.City, ISNULL(dcsa.City,'')) City, gst.Id GuestId \n"
            //            + "FROM Delivery_Customers dc \n"
            //            + "OUTER APPLY( \n"
            //            + "     SELECT TOP 1 ISNULL(dcba.AddressStreet, '') + ' ' + ISNULL(dcba.AddressNo, '') sAddress, dcba.City \n"
            //            + "     FROM Delivery_CustomersBillingAddress dcba \n"
            //            + "     WHERE dcba.CustomerID = dc.ID \n"
            //            + ") dcba \n"
            //            + "OUTER APPLY( \n"
            //            + "     SELECT TOP 1 ISNULL(dcsa.AddressStreet, '') + ' ' + ISNULL(dcsa.AddressNo, '') sAddress, dcsa.City \n"
            //            + "     FROM Delivery_CustomersShippingAddress dcsa \n"
            //            + "     WHERE dcsa.CustomerID = dc.ID \n"
            //            + ") dcsa \n"
            //            + " OUTER APPLY( \n"
            //            + "       SELECT TOP 1 * \n"
            //            + "       FROM Guest g WHERE g.ProfileNo = dc.ID \n"
            //            + ") gst \n"
            //            + "WHERE dc.ID = " + CustId.ToString();
            //        DataSet dsCust = wbCon.FillDataSet(SQLC);
            //        if (dsCust.Tables.Count > 0 && dsCust.Tables[0].Rows.Count > 0)
            //        {
            //            cm.Address = (String)dsCust.Tables[0].Rows[0]["Address"];
            //            cm.FirstName = (String)dsCust.Tables[0].Rows[0]["FirstName"];
            //            cm.LastName = (String)dsCust.Tables[0].Rows[0]["LastName"];
            //            cm.City = (String)dsCust.Tables[0].Rows[0]["City"];
            //            cm.ProfileNo = int.Parse((DBNull.Value.Equals(dsCust.Tables[0].Rows[0]["GuestId"]) ? -1 : (long)(dsCust.Tables[0].Rows[0]["GuestId"])).ToString());
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        Log.ToErrorLog("GetPayments (Customer Model) " + ex.ToString() + " SQL [" + SQLC + "]");
            //    }
            //}
            string SQL = "";
            try
            {


                //SQL = "SELECT * FROM Guest WHERE ProfileNo = " + CustId.ToString();
                //DataSet dsPaym = wbCon.FillDataSet(SQL);
                //if (dsPaym.Tables.Count > 0 && dsPaym.Tables[0].Rows.Count > 0)
                //    GuestId = (long)dsPaym.Tables[0].Rows[0]["Id"];

                SQL = "SELECT NULL PreviousAmount,CAST(SUM(ROUND((CAST(hpo.amount*hpo.qty AS FLOAT)/100),2)) AS FLOAT) Amount,CAST(hpo.orderno AS VARCHAR(250)) OrderNo, \n"
                    + "     CAST(CASE WHEN CHARINDEX(';', hpo.payment) <> 0 THEN " + settings.CreditCardId.ToString() + " ELSE " + settings.CashId.ToString() + " END AS BIGINT) AccountId, \n"
                    + "     a.[Description] [Description],a.[Description] AccountDescription,a.[Type] AccountType,CAST(" + settings.PosInfoId.ToString() + " AS BIGINT) PosInfoId, \n"
                    + "     CAST(" + settings.StaffId.ToString() + " AS BIGINT) StaffId,'False' SendsTransfer," + GuestId.ToString() + "GuestId, \n"
                    + "     NULL CreditAccountId,NULL CreditCodeId,NULL CreditAccountDescription, \n"
                    + "     NULL NewCreditBalance,1 Percentage, '" + Guest + "' Guest, \n"
                    + "     CASE WHEN CHARINDEX(';', hpo.payment) <> 0 THEN SUBSTRING(hpo.payment, CHARINDEX(';', hpo.payment)+1, LEN(hpo.payment)) ELSE hpo.payment END PayDescr \n"
                    + "FROM HitPosOrders hpo \n"
                    + "CROSS APPLY( \n"
                    + "      SELECT TOP 1 * \n"
                    + "      FROM Accounts a \n"
                    + "      WHERE a.[Type] = CASE WHEN CHARINDEX(';', hpo.payment) <> 0 THEN " + settings.CreditCardId.ToString() + " ELSE " + settings.CashId.ToString() + " END \n"
                    + ") a \n"
                    + "WHERE hpo.orderno = " + OrderNo.ToString() + " \n"
                    + "GROUP BY hpo.orderno, hpo.payment, a.[Description], a.[Type] \n";
                return wbCon.Query<PaymentsDetails>(SQL).ToList();
                //DataSet dsPaym = wbCon.FillDataSet(SQL);
                //foreach (PaymentsDetails row in dsPaym)
                //{
                //    PaymentsDetails pm = new PaymentsDetails();
                //    pm.PreviousAmount = (DBNull.Value.Equals(row["PreviousAmount"]) ? null : (Double?)(row["PreviousAmount"]));
                //    pm.Amount = (DBNull.Value.Equals(row["Amount"]) ? null : (Double?)(row["Amount"]));
                //    pm.OrderNo = (DBNull.Value.Equals(row["OrderNo"]) ? null : ConvertToUtf8((String)(row["OrderNo"])));
                //    pm.AccountId = (DBNull.Value.Equals(row["AccountId"]) ? null : (Int64?)(row["AccountId"]));
                //    pm.Description = (DBNull.Value.Equals(row["Description"]) ? null : ConvertToUtf8((String)(row["Description"])));
                //    pm.AccountDescription = (DBNull.Value.Equals(row["AccountDescription"]) ? null : ConvertToUtf8((String)(row["AccountDescription"])));
                //    pm.AccountType = (DBNull.Value.Equals(row["AccountType"]) ? null : (Int16?)(row["AccountType"]));
                //    pm.PosInfoId = (DBNull.Value.Equals(row["PosInfoId"]) ? null : (Int64?)(row["PosInfoId"]));
                //    pm.StaffId = (DBNull.Value.Equals(row["StaffId"]) ? null : (Int64?)(row["StaffId"]));
                //    pm.SendsTransfer = (DBNull.Value.Equals(row["SendsTransfer"]) ? (bool?)null : Convert.ToBoolean(row["SendsTransfer"].ToString()));
                //    pm.GuestId = GuestId;
                //    //pm.CreditAccountId = (DBNull.Value.Equals(row["CreditAccountId"]) ? null : (string)(row["CreditAccountId"]));
                //    //pm.CreditCodeId = (DBNull.Value.Equals(row["CreditCodeId"]) ? null : (string)(row["CreditCodeId"]));
                //    //pm.CreditAccountDescription = (DBNull.Value.Equals(row["CreditAccountDescription"]) ? null : (string)(row["CreditAccountDescription"]));
                //    //pm.NewCreditBalance = (DBNull.Value.Equals(row["NewCreditBalance"]) ? null : (string)(row["NewCreditBalance"]));
                //    pm.Percentage = (DBNull.Value.Equals(row["Percentage"]) ? null : (int?)(row["Percentage"]));
                //    pm.Guest = Guest;
                //    res.Add(pm);
                //}

            }
            catch (Exception ex)
            {
                logger.Error("GetPayments SQL [ " + SQL + "] \r\n " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get Detail's from WebPos HitPosOrder
        /// </summary>
        /// <param name="wbCon"></param>
        /// <param name="OrderNo"></param>
        /// <param name="SalesTypeId"></param>
        /// <returns></returns>
        private static List<OrdertDetails> GetOrderDetails(SqlConnection wbCon, Int64 OrderNo, String SalesTypeId)
        {
            string SQL = "";
            try
            {
                List<OrdertDetails> res = new List<OrdertDetails>();
                SQL = SalesTypeId + " \n " + "SELECT  fin.Id,SUBSTRING(fin.[Description],1,50) [Description], \n"
                    + "       CASE WHEN fin.Discount <> 0 THEN CAST((fin.TotalBeforeDiscount)/(100/fin.Discount) AS FLOAT) ELSE 0 END Discount, fin.Guid, fin.IsExtra, fin.IsChangeItem, \n"
                    + "       CAST(fin.ItemCode AS VARCHAR(255)) ItemCode, SUBSTRING(fin.ItemDescr,1,50) ItemDescr, CAST(fin.ItemDiscount AS FLOAT) ItemDiscount, fin.ItemPrice, CAST(fin.ItemQty AS FLOAT) ItemQty, \n"
                    + "       fin.ItemRegion, fin.ItemVatRate, CAST(fin.KdsId AS BIGINT) KdsId, fin.KitchenCode, CAST(fin.KitchenId AS INT) KitchenId, \n"
                    + "       fin.PosInfoId, fin.PreparationTime, fin.PreviousStatus, CAST(fin.Price AS FLOAT) Price, \n"
                    + "       CAST(fin.PriceListDetailId AS BIGINT) PriceListDetailId, fin.PriceListId, fin.ProductCategoryId, \n"
                    + "       fin.ProductId, fin.Qty, fin.ReceiptSplitedDiscount, fin.RegionId, \n"
                    + "       fin.RegionPosition, fin.SalesTypeExtDesc, fin.SalesTypeId, fin.StaffId, \n"
                    + "       fin.TableCode, fin.TableId, \n"
                    + "       CAST(ROUND(CASE WHEN fin.Discount = 0 THEN fin.ItemGross ELSE (fin.ItemGross/(1+(fin.Discount/100))) END,2) AS FLOAT) TotalAfterDiscount, \n"
                    + "       fin.TotalBeforeDiscount,fin.VatCode, CAST(fin.VatDesc AS VARCHAR(100)) VatDesc, fin.VatId,ROW_NUMBER() OVER (PARTITION BY fin.externalOrderNo ORDER BY fin.externalOrderNo)  ItemSort, \n"
                    + "       fin.Abbreviation, fin.InvoiceType, fin.[status], fin.ItemGross,CAST(ROUND((fin.ItemGross - (fin.ItemGross/(1+(fin.ItemVatRate/100)))),2) AS FLOAT) ItemVatValue, \n"
                    + "       CAST(ROUND((fin.ItemGross/(1+(fin.ItemVatRate/100))),2) AS FLOAT) ItemNet, CAST(fin.cont_line AS BIGINT) cont_line, 0 PaidStatus \n"
                    + "FROM ( \n"
                    + "	    SELECT TOP 100000 hpo.Id, hpo.item_descr [Description], CASE WHEN ISNUMERIC(ISNULL(hpo.member,'0')) = 1 THEN CAST(ISNULL(hpo.member,'0') AS DECIMAL(19,4)) ELSE 0 END Discount, \n"
                    + "		    NEWID() [Guid],CASE WHEN hpo.cont_line = hpo.id THEN 'False' ELSE 'True' END IsExtra,'FALSE' IsChangeItem, \n"
                    + "         CASE WHEN hpo.cont_line = hpo.id THEN CAST(p.Code AS VARCHAR(255)) ELSE CAST(i.Code AS VARCHAR(255)) END ItemCode, \n"
                    + "         hpo.item_descr ItemDescr, 0 ItemDiscount,CAST(ROUND((CAST(hpo.amount AS FLOAT)/100),2) AS FLOAT) ItemPrice, \n"
                    + "         CASE WHEN hpo.item_code = 913 AND hpo.amount <> 0 AND hpo.total <> 0 THEN hpo.total/hpo.amount \n"
                    + "              WHEN ISNULL(hpo.qty,0) = 0 AND hpo.cont_line = hpo.id THEN 1 \n"
                    + "              WHEN ISNULL(hpo.qty,0) = 0 AND hpo.cont_line <> hpo.id THEN 0 \n"
                    + "              ELSE hpo.qty END ItemQty, NULL ItemRegion,CAST(v.Percentage AS FLOAT) ItemVatRate, \n"
                    + "         ISNULL(p.KdsId,pIng.KdsId) KdsId,k.Code KitchenCode,ISNULL(p.KitchenId, pIng.KitchenId) KitchenId,CAST(CASE WHEN ISNULL(st.PosInfoId,0) > 0 THEN st.PosInfoId ELSE " + settings.PosInfoId.ToString() + " END AS BIGINT) PosInfoId, \n"
                    + "         ISNULL(p.PreparationTime,pIng.PreparationTime) PreparationTime,-1 PreviousStatus,CAST(ROUND(CAST(hpo.amount AS FLOAT)/100, 2) AS DECIMAL(19,4)) Price, \n"
                    + "         pd.Id PriceListDetailId,pd.PricelistId PriceListId,ISNULL(p.ProductCategoryId,pIng.ProductCategoryId) ProductCategoryId, \n "
                    + "         CASE WHEN hpo.cont_line = hpo.id THEN p.Id ELSE i.Id END ProductId, \n"
                    + "         CAST(CASE WHEN hpo.item_code = 913 AND hpo.amount <> 0 AND hpo.total <> 0 THEN hpo.total / hpo.amount \n"
                    + "                   WHEN ISNULL(hpo.qty,0) = 0 AND hpo.cont_line = hpo.id THEN 1 \n"
                    + "                   WHEN ISNULL(hpo.qty,0) = 0 AND hpo.cont_line <> hpo.id THEN 0 \n"
                    + "                   ELSE hpo.qty END  AS FLOAT) Qty, \n"
                    + "         0 ReceiptSplitedDiscount,NULL RegionId,NULL RegionPosition,stl.[Description] SalesTypeExtDesc,st.WebSales SalesTypeId,CAST(" + settings.StaffId.ToString() + " AS BIGINT) StaffId, \n"
                    + "         NULL TableCode,NULL TableId,0 TotalAfterDiscount, \n"
                    + "         CAST(CASE WHEN hpo.item_code = 913 THEN ROUND(CAST(hpo.total AS FLOAT)/100,2) ELSE ROUND(CAST((hpo.qty*hpo.amount) AS FLOAT)/100,2) END AS FLOAT) TotalBeforeDiscount, \n"
                    + "         CAST(hpo.item_vat AS BIGINT) VatCode,v.Percentage VatDesc,v.Id VatId,hpo.orderno externalOrderNo,1 ItemSort,abr.Abbreviation,CAST(" + settings.InvoiceType.ToString() + " AS BIGINT) InvoiceType, \n"
                    + "         CASE WHEN " + settings.ExtType.ToString() + " = 1 THEN 1 ELSE 0 END [status] , \n"
                    + "         CAST(CASE WHEN hpo.item_code = 913 THEN ROUND(CAST(hpo.total AS FLOAT)/100,2) ELSE ROUND(((CAST(hpo.amount AS FLOAT)/100) * hpo.qty),2) END AS FLOAT) ItemGross,0 ItemVatValue,0 ItemNet, hpo.cont_line \n"
                    + "     FROM HitPosOrders hpo \n"
                    + "     LEFT OUTER JOIN @salesType st ON CAST(st.HitSales AS VARCHAR(8000)) = CAST(hpo.sp AS VARCHAR(8000)) \n"
                    + "     LEFT OUTER JOIN Vat v ON v.Code = hpo.item_vat \n"
                    + "     LEFT OUTER JOIN Product p ON p.ProductCategoryId NOT IN (31,60) AND \n"
                    + "         CAST(p.Code AS VARCHAR(255)) = CASE WHEN hpo.cont_line = hpo.id THEN CAST(hpo.item_code AS VARCHAR(150)) \n"
                    + "                                             ELSE 'fsdfsds234123' END \n"
                    + "     OUTER APPLY( \n"
                    + "        SELECT TOP 1 i.* \n"
                    + "        FROM Ingredients i \n"
                    + "        WHERE \n"
                    + "            CAST(i.Code AS VARCHAR(255)) = \n"
                    + "                 CASE WHEN(hpo.cont_line <> hpo.id) AND hpo.item_code NOT BETWEEN 1000 AND 1300 AND \n"
                    + "                            EXISTS(SELECT 1 \n"
                    + "                                   FROM Ingredients ig \n"
                    + "                                   WHERE ig.Code = CAST(hpo.item_code AS VARCHAR(255))) THEN CAST(hpo.item_code AS VARCHAR(255)) \n"
                    + "                      WHEN(hpo.cont_line <> hpo.id) THEN CAST(hpo.item_code AS VARCHAR(20))+'-' + hpo.item_descr \n"
                    + "                 ELSE 'fsdfsds234123' END \n"
                    + "      ) i \n"
                    + "     LEFT OUTER JOIN Product pIng ON \n"
                    + "        pIng.Code = CASE WHEN(hpo.cont_line <> hpo.id) AND EXISTS(SELECT 1 FROM Ingredients ig WHERE ig.Code = CAST(hpo.item_code AS VARCHAR(255)) ) AND \n"
                    + "                          (hpo.item_code NOT BETWEEN 1000 AND 1300) THEN CAST(hpo.item_code AS VARCHAR(255)) \n"
                    + "                    ELSE 'fsdfsds234123' END AND pIng.ProductCategoryId IN (31, 60)  \n"
                    + "     LEFT OUTER JOIN Kitchen k ON k.Id = ISNULL(p.KitchenId, pIng.KitchenId) \n"
                    + "     LEFT OUTER JOIN PricelistDetail pd ON pd.PricelistId =  CASE WHEN ISNULL(st.PricelistId,0) > 0 THEN st.PricelistId ELSE " + settings.PriceListId.ToString() + " END AND  \n"
                    + "         CAST(ISNULL(pd.ProductId,'') AS VARCHAR(100)) LIKE CASE WHEN hpo.cont_line = hpo.id THEN CAST(p.Id AS VARCHAR(100)) ELSE '%' END \n"
                    + "         AND CAST(ISNULL(pd.IngredientId,'') AS VARCHAR(100)) LIKE CASE WHEN hpo.cont_line <> hpo.id THEN CAST(i.Id AS VARCHAR(100)) ELSE '%' END \n"
                    + "     LEFT OUTER JOIN SalesType stl ON stl.Id = st.WebSales \n"
                    + "     OUTER APPLY( \n"
                    + "     	SELECT TOP 1 pid.Abbreviation \n"
                    + "     	FROM PosInfoDetail pid \n"
                    + "     	WHERE pid.PosInfoId	= CASE WHEN ISNULL(st.PosInfoId,0) > 0 THEN st.PosInfoId ELSE " + settings.PosInfoId.ToString() + " END AND pid.InvoicesTypeId = " + settings.InvoiceType.ToString() + " \n"
                    + "     ) abr \n"
                    + "     WHERE hpo.completed = 0 AND hpo.orderno = " + OrderNo.ToString() + " \n"
                    + ") fin \n"
                    //+ "WHERE fin.externalOrderNo = " + OrderNo.ToString() + " \n"
                    + " ORDER BY fin.id ";
                res = wbCon.Query<OrdertDetails>(SQL.ToString()).ToList();
                //DataSet dsOrderDet = wbCon.FillDataSet(SQL);
                //foreach (DataRow row in dsOrderDet.Tables[0].Rows)
                //{
                //    OrdertDetails ord = new OrdertDetails();
                //    ord.Description = (DBNull.Value.Equals(row["Description"]) ? null : ConvertToUtf8((String)row["Description"]));
                //    ord.Discount = (DBNull.Value.Equals(row["Discount"]) ? null : (Double?)row["Discount"]);
                //    ord.Guid = (DBNull.Value.Equals(row["Guid"]) ? null : (Guid?)row["Guid"]);
                //    ord.IsExtra = (DBNull.Value.Equals(row["IsExtra"]) ? (bool?)null : Convert.ToBoolean(row["IsExtra"].ToString()));
                //    ord.IsChangeItem = (DBNull.Value.Equals(row["IsChangeItem"]) ? (bool?)null : Convert.ToBoolean(row["IsChangeItem"].ToString()));
                //    ord.ItemRemark = null;
                //    ord.ItemCode = (DBNull.Value.Equals(row["ItemCode"]) ? null : (String)row["ItemCode"]);
                //    ord.ItemDescr = (DBNull.Value.Equals(row["ItemDescr"]) ? null : ConvertToUtf8((String)row["ItemDescr"]));
                //    ord.ItemDiscount = (DBNull.Value.Equals(row["Discount"]) ? null : (Double?)row["Discount"]);// (DBNull.Value.Equals(row["ItemDiscount"]) ? null : (Double?)row["ItemDiscount"]);
                //    ord.ItemPrice = (DBNull.Value.Equals(row["ItemPrice"]) ? null : (Double?)row["ItemPrice"]);
                //    ord.ItemQty = (DBNull.Value.Equals(row["ItemQty"]) ? null : (Double?)row["ItemQty"]);
                //    ord.ItemRegion = (DBNull.Value.Equals(row["ItemRegion"]) ? null : ConvertToUtf8((String)row["ItemRegion"]));
                //    ord.ItemVatRate = (DBNull.Value.Equals(row["ItemVatRate"]) ? null : (Double?)row["ItemVatRate"]);
                //    ord.KdsId = (DBNull.Value.Equals(row["KdsId"]) ? null : (Int64?)row["KdsId"]);
                //    ord.KitchenCode = (DBNull.Value.Equals(row["KitchenCode"]) ? null : ConvertToUtf8((String)row["KitchenCode"]));
                //    ord.KitchenId = (DBNull.Value.Equals(row["KitchenId"]) ? null : (int?)row["KitchenId"]);
                //    ord.OrderId = null;
                //    ord.PosInfoId = (DBNull.Value.Equals(row["PosInfoId"]) ? null : (Int64?)row["PosInfoId"]);
                //    ord.PreparationTime = (DBNull.Value.Equals(row["PreparationTime"]) ? null : (Int16?)Int16.Parse(((int?)row["PreparationTime"]).ToString()));
                //    ord.PreviousStatus = (DBNull.Value.Equals(row["PreviousStatus"]) ? null : (int?)row["PreviousStatus"]);
                //    ord.Price = (DBNull.Value.Equals(row["Price"]) ? null : (Double?)row["Price"]);
                //    ord.PriceListDetailId = (DBNull.Value.Equals(row["PriceListDetailId"]) ? null : (Int64?)row["PriceListDetailId"]);
                //    ord.PriceListId = (DBNull.Value.Equals(row["PriceListId"]) ? null : (Int64?)row["PriceListId"]);
                //    ord.ProductCategoryId = (DBNull.Value.Equals(row["ProductCategoryId"]) ? null : (Int64?)row["ProductCategoryId"]);
                //    ord.ProductId = (DBNull.Value.Equals(row["ProductId"]) ? null : (Int64?)row["ProductId"]);
                //    ord.Qty = (DBNull.Value.Equals(row["Qty"]) ? null : (Double?)row["Qty"]);
                //    ord.ReceiptSplitedDiscount = (DBNull.Value.Equals(row["ReceiptSplitedDiscount"]) ? null : (int?)row["ReceiptSplitedDiscount"]);
                //    ord.RegionId = (DBNull.Value.Equals(row["RegionId"]) ? null : (long?)row["RegionId"]);
                //    ord.RegionPosition = (DBNull.Value.Equals(row["RegionPosition"]) ? null : (int?)row["RegionPosition"]);
                //    ord.SalesTypeExtDesc = (DBNull.Value.Equals(row["SalesTypeExtDesc"]) ? null : ConvertToUtf8((String)row["SalesTypeExtDesc"]));
                //    ord.SalesTypeId = (DBNull.Value.Equals(row["SalesTypeId"]) ? null : (Int64?)row["SalesTypeId"]);
                //    ord.StaffId = (DBNull.Value.Equals(row["StaffId"]) ? null : (Int64?)row["StaffId"]);
                //    ord.TableCode = (DBNull.Value.Equals(row["TableCode"]) ? null : ConvertToUtf8((String)row["TableCode"]));
                //    ord.TableId = (DBNull.Value.Equals(row["TableId"]) ? null : (long?)row["TableId"]);
                //    ord.TotalAfterDiscount = (DBNull.Value.Equals(row["TotalAfterDiscount"]) ? null : (Double?)row["TotalAfterDiscount"]);
                //    ord.TotalBeforeDiscount = (DBNull.Value.Equals(row["TotalBeforeDiscount"]) ? null : (Double?)row["TotalBeforeDiscount"]);
                //    ord.VatCode = (DBNull.Value.Equals(row["VatCode"]) ? null : (Int64?)row["VatCode"]);
                //    ord.VatDesc = (DBNull.Value.Equals(row["VatDesc"]) ? null : (String)row["VatDesc"]);
                //    ord.VatId = (DBNull.Value.Equals(row["VatId"]) ? null : (Int64?)row["VatId"]);
                //    ord.OrderNo = OrderNo;
                //    ord.ItemSort = (DBNull.Value.Equals(row["ItemShort"]) ? null : (int?)Convert.ToInt16(((Int64?)row["ItemShort"]).ToString()));
                //    ord.Abbreviation = (DBNull.Value.Equals(row["Abbreviation"]) ? null : ConvertToUtf8((String)row["Abbreviation"]));
                //    ord.InvoiceType = (DBNull.Value.Equals(row["InvoiceType"]) ? null : (Int64?)row["InvoiceType"]);
                //    ord.Status = (DBNull.Value.Equals(row["status"]) ? null : (int?)row["status"]);
                //    ord.ItemGross = (DBNull.Value.Equals(row["ItemGross"]) ? null : (Double?)row["ItemGross"]);
                //    ord.ItemVatValue = (DBNull.Value.Equals(row["ItemVatValue"]) ? null : (Double?)row["ItemVatValue"]);
                //    ord.ItemNet = (DBNull.Value.Equals(row["ItemNet"]) ? null : (Double?)row["ItemNet"]);
                //    ord.PaidStatus = 0;

                //    ord.ItemId = (DBNull.Value.Equals(row["Id"]) ? null : (int?)row["Id"]);
                //    ord.cont_line = (DBNull.Value.Equals(row["cont_line"]) ? null : (long?)row["cont_line"]);


                //    res.Add(ord);
                //}
                MasterProductProperties mstPrProp = new MasterProductProperties();
                foreach (OrdertDetails item in res)
                {
                    if ((item.IsExtra ?? false) == false)
                    {
                        mstPrProp.KdsId = item.KdsId;
                        mstPrProp.KitchenCode = item.KitchenCode;
                        mstPrProp.KitchenId = item.KitchenId;
                        mstPrProp.PreparationTime = item.PreparationTime;
                        mstPrProp.PriceListDetailId = item.PriceListDetailId;
                        mstPrProp.PriceListId = item.PriceListId;
                        mstPrProp.ProductCategoryId = item.ProductCategoryId;
                        mstPrProp.Qty = item.Qty;
                        mstPrProp.ItemQty = item.ItemQty;
                        mstPrProp.ItemId = item.ItemId;
                    }
                    else
                    {
                        item.KdsId = item.KdsId == null ? mstPrProp.KdsId : item.KdsId;
                        item.KitchenCode = string.IsNullOrEmpty(item.KitchenCode) ? mstPrProp.KitchenCode : item.KitchenCode;
                        item.KitchenId = item.KitchenId == null ? mstPrProp.KitchenId : item.KitchenId;
                        item.PreparationTime = item.PreparationTime == null ? mstPrProp.PreparationTime : item.PreparationTime;
                        item.PriceListDetailId = item.PriceListDetailId == null ? mstPrProp.PriceListDetailId : item.PriceListDetailId;
                        item.PriceListId = item.PriceListId == null ? mstPrProp.PriceListId : item.PriceListId;
                        item.ProductCategoryId = item.ProductCategoryId == null ? mstPrProp.ProductCategoryId : item.ProductCategoryId;
                        item.Qty = item.Qty == null || item.Qty == 0 ? mstPrProp.Qty : item.Qty;
                        item.ItemQty = item.ItemQty == null || item.ItemQty == 0 ? mstPrProp.ItemQty : item.ItemQty;
                        item.ItemId = mstPrProp.ItemId == null || mstPrProp.ItemId == 0 ? -1 : mstPrProp.ItemId;
                    }
                }
                OrdertDetails masterProduct = new OrdertDetails();
                masterProduct = res.Find(f => f.IsExtra == false);
                if (masterProduct != null)
                {
                    foreach (OrdertDetails item in res)
                    {
                        if (item.cont_line == 0 && item.ItemId < 0)
                        {
                            item.KdsId = masterProduct.KdsId;
                            item.KitchenCode = masterProduct.KitchenCode;
                            item.KitchenId = masterProduct.KitchenId;
                            item.PreparationTime = masterProduct.PreparationTime;
                            item.PriceListDetailId = masterProduct.PriceListDetailId;
                            item.PriceListId = masterProduct.PriceListId;
                            item.ProductCategoryId = masterProduct.ProductCategoryId;
                            item.Qty = masterProduct.Qty;
                            item.ItemQty = masterProduct.ItemQty;
                            item.ItemId = masterProduct.ItemId;
                        }
                    }
                }
                //return res.OrderBy(o => o.ItemId).ThenBy(n => n.IsExtra).ToList();
                return res.OrderBy(o => o.cont_line).ThenBy(n => n.IsExtra).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("GetOrderDetails SQL [" + SQL + "] \r\n " + ex.ToString());
                return null;
            }

        }

        /// <summary>
        /// Call WebPos API to insert or update a customer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static DeliveryCustomerModel PostCustomerToApi(DeliveryCustomerModel model)
        {
            try
            {
                //jsonToSend.store = gstoreid;
                var request = (HttpWebRequest)WebRequest.Create(settings.WebApiURL + "v3/DeliveryCustomer/UpsertCustomerFromService");
                // "http://sisifos:5420/api/";

                //var json = JsonConvert.SerializeObject(jsonToSend);
                var postData = JsonConvert.SerializeObject(model);

                var data = Encoding.UTF8.GetBytes(postData);
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(settings.URLUserName + ":" + settings.URLPass));
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = data.Length;
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.Headers.Add("Accept-Language", "en");
                using (var stream = request.GetRequestStream())
                {
                    try
                    {

                        stream.Write(data, 0, data.Length);
                    }
                    catch (Exception ecx)
                    {
                        logger.Error("PostDataWithApi Execute Call \r\n " + ecx.ToString());
                    }
                }

                var response = (HttpWebResponse)request.GetResponse();



                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return JsonConvert.DeserializeObject<DeliveryCustomerModel>(responseString);
                //return (response.StatusCode.ToString().ToUpper().Trim() == "CREATED");
                //return true;// responseString.ToString();

            }
            catch (Exception ex)
            {
                logger.Error("PostCustomerToApi \r\n " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Post New Receipt to WebPos
        /// </summary>
        /// <param name="Orders"></param>
        /// <returns></returns>
        public static bool PostDataWithApi(string Orders)
        {
            try
            {
                //jsonToSend.store = gstoreid;
                var request = (HttpWebRequest)WebRequest.Create(settings.WebApiURL + "InvoiceForDisplay?storeid=" + settings.ShopInfoId);// "http://sisifos:5420/api/";
                //var json = JsonConvert.SerializeObject(jsonToSend);
                var postData = Orders;

                var data = Encoding.UTF8.GetBytes(postData);
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(settings.URLUserName + ":" + settings.URLPass));
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = data.Length;
                request.Headers.Add("Authorization", "Basic " + encoded);
                logger.Info("Post Data To API (Not forky) " + settings.WebApiURL + "InvoiceForDisplay?storeid=" + settings.ShopInfoId + "    ContentType = application/json; charset=utf-8, Authorization = Basic " + encoded + " Model [" + Orders + "]");
                using (var stream = request.GetRequestStream())
                {
                    try
                    {

                        stream.Write(data, 0, data.Length);
                    }
                    catch (Exception ecx)
                    {
                        logger.Error("PostDataWithApi Execute Call \r\n " + ecx.ToString());
                    }
                }

                var response = (HttpWebResponse)request.GetResponse();



                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return (response.StatusCode.ToString().ToUpper().Trim() == "CREATED");
                //return true;// responseString.ToString();

            }
            catch (Exception ex)
            {
                logger.Error("PostDataWithApi \r\n " + ex.ToString());
                return false;
            }


        }

        /// <summary>
        /// Create hitpos tables to webpos
        /// </summary>
        /// <param name="wbCon"></param>
        /// <returns></returns>
        private static bool CreateTables(SqlConnection wbCon)
        {
            string SQL = "";
            try
            {
                SQL = "IF NOT EXISTS (SELECT 1  \n"
                    + "               FROM sys.tables s  \n"
                    + "               INNER JOIN sys.schemas sm ON sm.name = 'dbo' \n"
                    + "               WHERE s.name = 'HitPosOrders' AND s.schema_id = sm.schema_id)  \n"
                    + "	CREATE TABLE dbo.HitPosOrders ( \n"
                    + "		[CurId] [bigint] NOT NULL IDENTITY(1,1), \n"
                    + "		[id] [int] NOT NULL, \n"
                    + "		[orderno] [int] NULL, \n"
                    + "		[pos] [int] NULL, \n"
                    + "		[shop_id] [nvarchar](50) NULL, \n"
                    + "		[item_group] [int] NULL, \n"
                    + "		[item_code] [int] NULL, \n"
                    + "		[item_descr] [nvarchar](60) NULL, \n"
                    + "		[item_subgroup] [int] NULL, \n"
                    + "		[item_vat] [int] NULL, \n"
                    + "		[cont_line] [decimal](18, 0) NULL, \n"
                    + "		[sp] [ntext] NULL, \n"
                    + "		[prep_time] [datetime] NULL, \n"
                    + "		[start_time] [datetime] NULL, \n"
                    + "		[load_time] [int] NULL, \n"
                    + "		[otd] [int] NULL, \n"
                    + "		[qty] [int] NULL, \n"
                    + "		[amount] [int] NULL, \n"
                    + "		[total] [int] NULL, \n"
                    + "		[waiter] [int] NULL, \n"
                    + "		[ttable] [int] NULL, \n"
                    + "		[listino] [int] NULL, \n"
                    + "		[receipt] [int] NULL, \n"
                    + "		[member] [nvarchar](50) NULL, \n"
                    + "		[priority] [int] NULL, \n"
                    + "		[kdws] [int] NULL, \n"
                    + "		[ready] [int] NULL, \n"
                    + "		[rqty] [int] NULL, \n"
                    + "		[nieko_flag] [int] NULL, \n"
                    + "		[status] [int] NULL, \n"
                    + "		[status_time] [datetime] NULL, \n"
                    + "		[rest_time] [int] NULL, \n"
                    + "		[room] [nvarchar](50) NULL, \n"
                    + "		[payment] [nvarchar](50) NULL, \n"
                    + "		[type] [nvarchar](50) NULL, \n"
                    + "		[comments] [nvarchar](255) NULL, \n"
                    + "		[mqty] [int] NULL, \n"
                    + "		[rec_time_start] [datetime] NULL, \n"
                    + "		[status_time2] [datetime] NULL, \n"
                    + "		[status_time3] [datetime] NULL, \n"
                    + "		[status_time4] [datetime] NULL, \n"
                    + "		[status_time5] [datetime] NULL, \n"
                    + "		[fo_day] [datetime] NULL, \n"
                    + "		[delivery_time] [datetime] NULL, \n"
                    + "		[agent] [int] NULL, \n"
                    + "		[flag_up] [int] NULL, \n"
                    + "		[sent] [int] NULL, \n"
                    + "		[correct] [int] NULL, \n"
                    + "		[completed] [int] NULL, \n"
                    + "     [CreationDate] [datetime] null, \n"
                    + "		CONSTRAINT [HitPosOrders_PK] PRIMARY KEY NONCLUSTERED  \n"
                    + "		( \n"
                    + "			[CurId] ASC \n"
                    + "		) \n"
                    + "	) ON [PRIMARY] \n"
                    + " \n"
                    + "";
                if (!ExecSQLs(wbCon, SQL))
                {
                    return false;
                }

                SQL = "IF NOT EXISTS (SELECT 1  \n"
                    + "               FROM sys.tables s  \n"
                    + "               INNER JOIN sys.schemas sm ON sm.name = 'dbo' \n"
                    + "               WHERE s.name = 'HitPosCustomers' AND s.schema_id = sm.schema_id)  \n"
                    + "	CREATE TABLE dbo.HitPosCustomers ( \n"
                    + "		[CurId] [bigint] NOT NULL IDENTITY(1,1), \n"
                    + "		[customerid] [nvarchar](15) NOT NULL, \n"
                    + "		[name] [varchar](50) NULL, \n"
                    + "		[fname] [varchar](50) NULL, \n"
                    + "		[title] [nvarchar](50) NULL, \n"
                    + "		[profession] [varchar](50) NULL, \n"
                    + "		[tel1] [nvarchar](40) NULL, \n"
                    + "		[tel2] [nvarchar](20) NULL, \n"
                    + "		[fax] [nvarchar](20) NULL, \n"
                    + "		[mobile] [nvarchar](20) NULL, \n"
                    + "		[address1] [varchar](200) NULL, \n"
                    + "		[address2] [char](50) NULL, \n"
                    + "		[address_no] [varchar](30) NULL, \n"
                    + "		[orofos1] [varchar](30) NULL, \n"
                    + "		[orofos2] [nvarchar](10) NULL, \n"
                    + "		[city] [varchar](50) NULL, \n"
                    + "		[zipcode] [char](10) NULL, \n"
                    + "		[doy] [varchar](30) NULL, \n"
                    + "		[afm] [varchar](30) NULL, \n"
                    + "		[email] [nvarchar](200) NULL, \n"
                    + "		[contact] [char](50) NULL, \n"
                    + "		[vip] [nchar](5) NULL, \n"
                    + "		[member] [nvarchar](10) NULL, \n"
                    + "		[tomeas] [nvarchar](20) NULL, \n"
                    + "		[store] [varchar](30) NULL, \n"
                    + "		[sector] [nvarchar](10) NULL, \n"
                    + "		[diet] [nvarchar](50) NULL, \n"
                    + "		[entolh] [nvarchar](50) NULL, \n"
                    + "		[farsa] [nvarchar](50) NULL, \n"
                    + "		[remarks] [char](200) NULL, \n"
                    + "		[amount] [float] NULL, \n"
                    + "		[expireddate] [datetime] NULL, \n"
                    + "		[order_comments] [nvarchar](200) NULL, \n"
                    + "		[first_order] [datetime] NULL, \n"
                    + "		[last_order] [datetime] NULL, \n"
                    + "		[no_of_orders] [int] NULL, \n"
                    + "		[tziros] [numeric](18, 0) NULL, \n"
                    + "		[bonus] [int] NULL, \n"
                    + "		[epitages] [int] NULL, \n"
                    + "		[zerobonus] [int] NULL, \n"
                    + "		[domino_false] [int] NULL, \n"
                    + "		[lates] [int] NULL, \n"
                    + "		[credit] [money] NULL, \n"
                    + "		[max_charge] [money] NULL, \n"
                    + "		[company_name] [varchar](50) NULL, \n"
                    + "		[bl_address] [varchar](200) NULL, \n"
                    + "		[bl_address_no] [varchar](30) NULL, \n"
                    + "		[bl_city] [varchar](50) NULL, \n"
                    + "		[doycode] [int] NULL, \n"
                    + "		CONSTRAINT [HitPosCustomers_PK] PRIMARY KEY NONCLUSTERED  \n"
                    + "		( \n"
                    + "			[CurId] ASC \n"
                    + "		) \n"
                    + "	) ON [PRIMARY]";
                if (!ExecSQLs(wbCon, SQL))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Create Tables : [SQL: " + SQL + "] \r\n " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Change and correct items for Order Line
        /// </summary>
        /// <param name="wbCon"></param>
        /// <param name="Id"></param>
        /// <param name="itemSubGr"></param>
        /// <param name="ItemCode"></param>
        /// <param name="codeLine"></param>
        /// <param name="kdws"></param>
        /// <returns></returns>
        private static decimal MakeOrdLine(SqlConnection wbCon, Int32? Id, ref Int32? itemSubGr,
            Int32? ItemCode, decimal? codeLine, ref Int32? kdws)
        {
            string SQL = "";
            if (ItemCode > 999 && ItemCode < 1301 && itemSubGr == null)
            {
                itemSubGr = 60;
            }
            else
            {
                SQL = "SELECT p.ProductCategoryId, p.KdsId FROM Product p WHERE p.Code = '" + ItemCode.ToString() + "'";
                WebPosProductsProductCategoriesModels ds = wbCon.Query<WebPosProductsProductCategoriesModels>(SQL).FirstOrDefault();
                if (ds != null)
                {
                    if (itemSubGr == null)
                    {
                        itemSubGr = ds.ProductCategoryId ?? (Int32?)null;
                        //DBNull.Value.Equals(ds.Tables[0].Rows[0]["ProductCategoryId"]) ? (Int32?)null : Int32.Parse(((long)ds.Tables[0].Rows[0]["ProductCategoryId"]).ToString());
                    }
                    if (kdws == null)
                    {
                        kdws = ds.KdsId ?? (Int32?)null;
                        //DBNull.Value.Equals(ds.Tables[0].Rows[0]["KdsId"]) ? (Int32?)null : Int32.Parse(((long)ds.Tables[0].Rows[0]["KdsId"]).ToString());
                    }
                }
            }
            if (itemSubGr != 31 && itemSubGr != 60)
            {
                //if (codeLine != null)
                //    return codeLine ?? 0;
                //else
                return decimal.Parse((Id ?? 0).ToString());
            }
            else
                return codeLine ?? 0;

        }

        /// <summary>
        /// Fill tables from HitPos to WebPos
        /// </summary>
        /// <param name="wbCon"></param>
        /// <param name="htCon"></param>
        /// <param name="ShopId"></param>
        /// <returns></returns>
        private static bool FillTables(SqlConnection wbCon, SqlConnection htCon)
        {
            StringBuilder SQL = new StringBuilder();
            try
            {

                List<string> lstCust = new List<string>();
                List<string> HitCust = new List<string>();
                List<LabCustomer> finCust = new List<LabCustomer>();

                SQL.Clear();

                //if (CheckCount == 1)
                //Get New Orders
                SQL.Append("SELECT CASE WHEN ISNULL(o.qty,0) = 0 THEN 0 ELSE amount END newAmount,  o.id, o.orderno, o.pos, SUBSTRING(o.shop_id,1,50) shop_id,  \n"
                        + "	o.item_group, o.item_code, SUBSTRING(o.item_descr,1,60) item_descr, o.item_subgroup, o.item_vat, o.cont_line, o.sp, o.prep_time,  \n"
                        + "	o.start_time, o.load_time, o.otd, o.qty, o.amount, o.total, o.waiter, o.ttable, o.listino, o.receipt,  \n"
                        + "	SUBSTRING(o.member,1,50) member, o.priority, o.kdws, o.ready, o.rqty, o.nieko_flag, o.[status], o.status_time, o.rest_time,  \n"
                        + "	SUBSTRING(o.room,1,50) room, SUBSTRING(o.payment,1,50) payment, SUBSTRING(o.[type],1,50) [type],SUBSTRING(o.comments,1,255) comments,  \n"
                        + "	o.mqty, o.rec_time_start, o.status_time2, o.status_time3, o.status_time4, o.status_time5, o.fo_day, o.delivery_time, o.agent, \n"
                        + "    o.flag_up, o.[sent], o.correct  \n"
                        + "FROM posuser.orders o \n"
                        + "INNER JOIN ( \n"
                        + "   SELECT ords.orderno \n"
                        + "   FROM ( \n"
                        + "       SELECT o.orderno, SUM(o.qty) qty, MAX(o.mqty) mqty, COUNT(o.orderno) cntRows \n"
                        + "       FROM posuser.orders o \n"
                        + "       WHERE o.flag_up IS NULL AND o.shop_id = '" + settings.ShopId + "'  \n"
                        + "       GROUP BY o.orderno \n"
                        + "   ) ords \n"
                        + "   WHERE ords.qty = ords.mqty OR ords.cntRows = ords.mqty \n"
                        + ") ord ON ord.orderno = o.orderno \n"
                        + "ORDER BY id");
                //else
                //    SQL.Append("SELECT CASE WHEN ISNULL(qty,0) = 0 THEN 0 ELSE amount END newAmount,  o.* \n"
                //              + "FROM posuser.orders o \n"
                //              + "WHERE o.flag_up IS NULL AND o.shop_id = '" + ShopId + "' ORDER BY id");

                List<HitPosOrderModel> dsNewOrd = htCon.Query<HitPosOrderModel>(SQL.ToString()).ToList();
                string sInsert = "";
                string sValues = "";
                string sUpdIds = "";

                //Check if customer exists


                SQL.Clear();
                SQL.Append("SELECT DISTINCT hpo.room room \n"
                     + "FROM HitPosOrders hpo \n"
                     + "WHERE hpo.completed = 0 AND ISNULL(hpo.room, '') <> '' AND hpo.room NOT IN(SELECT hpc.customerid FROM HitPosCustomers hpc)");
                lstCust = wbCon.Query<string>(SQL.ToString()).ToList();

                SQL.Clear();
                //completed
                //0 => New Order (Send It to Apy)
                //1 => Waiting   (Check For Status)
                //2 => Completed (No more check)
                Int32? itmSbGrp;
                Int32? itmkdws;

                decimal? curCodLn;
                decimal codLine; //row["cont_line"]
                decimal? lstCodeLn = null;
                Int32 prevOrderNo = -1;
                List<Int32?> excludeCodes = new List<Int32?>();
                excludeCodes.Add(31);
                excludeCodes.Add(60);

                string itemDescr;

                foreach (HitPosOrderModel row in dsNewOrd)
                {
                    itmSbGrp = row.item_subgroup;
                    itmkdws = row.kdws;

                    if ((prevOrderNo != row.orderno) || (itmSbGrp != null && !excludeCodes.Contains(itmSbGrp)))
                    {
                        prevOrderNo = row.orderno ?? 0;
                        lstCodeLn = null;
                    };

                    curCodLn = lstCodeLn ?? null;// DBNull.Value.Equals(row["cont_line"]) ? (lstCodeLn ?? null) : (decimal?)row["cont_line"];
                    codLine = MakeOrdLine(wbCon, row.id, ref itmSbGrp, row.item_code, curCodLn, ref itmkdws);
                    lstCodeLn = codLine;

                    sInsert = "INSERT INTO HitPosOrders (id,orderno,pos,shop_id,item_group,item_code,item_descr,item_subgroup,item_vat,cont_line,sp, \n"
                          + "       prep_time,start_time,load_time,otd,qty,amount,total,waiter,ttable,listino,receipt,member,priority, \n"
                          + "       kdws,ready,rqty,nieko_flag,status,status_time,rest_time,room,payment,type,comments,mqty,rec_time_start, \n"
                          + "       status_time2,status_time3,status_time4,status_time5,fo_day,delivery_time,agent,flag_up,sent,correct,completed,CreationDate) VALUES(  \n";
                    //Replace sympols with "" for item_Descr
                    itemDescr = (string.IsNullOrEmpty(row.item_descr) ? "" : row.item_descr).Replace("/", "");
                    itemDescr = itemDescr.Replace("'", "");
                    itemDescr = itemDescr.Replace("!", "");
                    itemDescr = itemDescr.Replace("@", "");
                    itemDescr = itemDescr.Replace("#", "");
                    itemDescr = itemDescr.Replace("$", "");
                    itemDescr = itemDescr.Replace("%", "");
                    itemDescr = itemDescr.Replace("^", "");
                    itemDescr = itemDescr.Replace("&", "");
                    itemDescr = itemDescr.Replace("*", "");
                    itemDescr = itemDescr.Replace("(", "");
                    itemDescr = itemDescr.Replace(")", "");
                    itemDescr = itemDescr.Replace("+", "");
                    itemDescr = itemDescr.Replace("=", "");
                    itemDescr = itemDescr.Replace("<", "");
                    itemDescr = itemDescr.Replace(">", "");
                    itemDescr = itemDescr.Replace("[", "");
                    itemDescr = itemDescr.Replace("]", "");
                    itemDescr = itemDescr.Replace("{", "");
                    itemDescr = itemDescr.Replace("}", "");
                    itemDescr = itemDescr.Replace(@"\", "");

                    sValues = (row.id.ToString() ?? "NULL") + ", \n " +
                        (NumToText(row.orderno)) + ", \n " +
                        (NumToText(row.pos)) + ", \n " +
                        ("'" + row.shop_id + "'" ?? "NULL") + ", \n " +
                        (NumToText(row.item_group)) + ", \n " +
                        (NumToText(row.item_code)) + ", \n " +
                        ("'" + itemDescr + "'") + ", \n " +
                        (NumToText(itmSbGrp)) + ", \n " +
                        (NumToText(row.item_vat)) + ", \n " +
                        (NumToText(codLine)) + ", \n " +
                        ("'" + row.sp + "'" ?? "NULL") + ", \n " +
                        (DateToText(row.prep_time)) + ", \n " +
                        (DateToText(row.start_time)) + ", \n " +
                        (NumToText(row.load_time)) + ", \n " +
                        (NumToText(row.otd)) + ", \n " +
                        (NumToText(row.qty)) + ", \n " +
                        (NumToText(row.newAmount)) + ", \n " +
                        (NumToText(row.total)) + ", \n " +
                        (NumToText(row.waiter)) + ", \n " +
                        (NumToText(row.ttable)) + ", \n " +
                        (NumToText(row.listino)) + ", \n " +
                        (NumToText(row.receipt)) + ", \n " +
                        ("'" + row.member + "'" ?? "NULL") + ", \n " +
                        (NumToText(row.priority)) + ", \n " +
                        (NumToText(row.kdws)) + ", \n " +
                        (NumToText(row.ready)) + ", \n " +
                        (NumToText(row.rqty)) + ", \n " +
                        (NumToText(row.nieko_flag)) + ", \n " +
                        (NumToText(row.status)) + ", \n " +
                        (DateToText(row.status_time)) + ", \n " +
                        (NumToText(row.rest_time)) + ", \n " +
                        ("'" + row.room + "'" ?? "NULL") + ", \n " +
                        ("'" + row.payment + "'" ?? "NULL") + ", \n " +
                        ("'" + row.type + "'" ?? "NULL") + ", \n " +
                        ("'" + row.comments + "'" ?? "NULL") + ", \n " +
                        (NumToText(row.mqty)) + ", \n " +
                        (DateToText(row.rec_time_start)) + ", \n " +
                        (DateToText(row.status_time2)) + ", \n " +
                        (DateToText(row.status_time3)) + ", \n " +
                        (DateToText(row.status_time4)) + ", \n " +
                        (DateToText(row.status_time5)) + ", \n " +
                        (DateToText(row.fo_day)) + ", \n " +
                        (DateToText(row.delivery_time)) + ", \n " +
                        (NumToText(row.agent)) + ", \n " +
                        (NumToText(row.flag_up)) + ", \n " +
                        (NumToText(row.sent)) + ", \n " +
                        (NumToText(row.correct)) + ", \n " +
                        "0,CAST(CONVERT(VARCHAR(10), GETDATE(),120) AS DATETIME)); ";
                    SQL.Append(sInsert + sValues);
                    sUpdIds = sUpdIds + row.id.ToString() + ",";
                    if (!string.IsNullOrEmpty(row.room))
                        lstCust.Add(row.room);
                }
                if (SQL.Length > 0)
                {
                    if (!ExecSQLs(wbCon, SQL.ToString()))
                        return false;
                    sUpdIds = sUpdIds.Substring(0, sUpdIds.Length - 1);
                    SQL.Clear();
                    SQL.Append("UPDATE posuser.orders SET flag_up = 1 WHERE id IN (" + sUpdIds + ");");
                    if (!ExecSQLs(htCon, SQL.ToString()))
                        return false;
                }

                SQL.Clear();
                SQL.Append("SELECT DISTINCT customerid FROM HitPosCustomers");
                HitCust = wbCon.Query<string>(SQL.ToString()).ToList();

                foreach (string item in lstCust)
                {
                    LabCustomer obj = new LabCustomer();
                    if (!HitCust.Contains(item))
                    {
                        obj.CustomerId = item;
                        obj.Insert = 1;
                    }
                    else
                    {
                        obj.CustomerId = item;
                        obj.Insert = 0;
                    }
                    var fld = finCust.Find(f => f.CustomerId == obj.CustomerId);
                    if (fld == null)
                        finCust.Add(obj);
                }


                sUpdIds = "";
                //foreach (string item in finCust)
                //{
                //    sUpdIds = sUpdIds + "'" + item + "'" + ",";
                //}
                //if (sUpdIds.Trim() != "")
                foreach (LabCustomer item in finCust)
                {
                    //sUpdIds = sUpdIds.Substring(0, sUpdIds.Length - 1);
                    SQL.Clear();
                    SQL.Append("SELECT SUBSTRING(customerid,1,15) customerid, SUBSTRING(NAME,1,50) name, SUBSTRING(fname,1,50) fname,  \n"
                               + "	SUBSTRING(title,1,50) title, SUBSTRING(profession,1,50) profession, SUBSTRING(tel1,1,40) tel1,  \n"
                               + "	SUBSTRING(tel2,1,20) tel2, SUBSTRING(fax,1,20) fax, SUBSTRING(mobile,1,20) mobile, \n"
                               + "	SUBSTRING(address1,1,200) address1, SUBSTRING(address2,1,50) address2, SUBSTRING(address_no,1,30) address_no,  \n"
                               + "	SUBSTRING(orofos1,1,30) orofos1, SUBSTRING(orofos2,1,10) orofos2, SUBSTRING(city,1,50) city,  \n"
                               + "	SUBSTRING(zipcode,1,10) zipcode, SUBSTRING(doy,1,30) doy, SUBSTRING(afm,1,30) afm,SUBSTRING(email,1,200) email,  \n"
                               + "	SUBSTRING(contact,1,50) contact, SUBSTRING(vip,1,5) vip,SUBSTRING(member,1,10) member,SUBSTRING(tomeas,1,20) tomeas,  \n"
                               + "	SUBSTRING(store,1,30) store, SUBSTRING(sector,1,10) sector,SUBSTRING(diet,1,50) diet, SUBSTRING(entolh,1,50) entolh,  \n"
                               + "	SUBSTRING(farsa,1,50) farsa,SUBSTRING(remarks,1,200) remarks, amount, expireddate,  \n"
                               + "	SUBSTRING(order_comments,1,200) order_comments, first_order, last_order, no_of_orders, tziros, bonus, epitages,  \n"
                               + "	zerobonus, domino_false, lates, credit, max_charge, SUBSTRING(company_name,1,50) company_name,  \n"
                               + "	SUBSTRING(bl_address,1,200) bl_address,SUBSTRING(bl_address_no,1,30) bl_address_no, SUBSTRING(bl_city,1,50) bl_city,doycode \n"
                               + "FROM posuser.labcustomer \n"
                               + "WHERE customerid IN ('" + item.CustomerId + "')");
                    List<HitPosCustomersModel> hitCust = htCon.Query<HitPosCustomersModel>(SQL.ToString()).ToList();
                    SQL.Clear();
                    foreach (HitPosCustomersModel row in hitCust)
                    {
                        if (item.Insert == 1)
                        {
                            sInsert = "INSERT INTO HitPosCustomers (customerid,name,fname,title,profession,tel1,tel2,fax,mobile,address1,address2,address_no,orofos1,orofos2, \n"
                                    + "city,zipcode,doy,afm,email,contact,vip,member,tomeas,store,sector,diet,entolh,farsa,remarks,amount,expireddate, \n"
                                    + "order_comments,first_order,last_order,no_of_orders,tziros,bonus,epitages,zerobonus,domino_false,lates,credit, \n"
                                    + "max_charge,company_name,bl_address,bl_address_no,bl_city,doycode) VALUES ( \n";
                            sValues = ("'" + row.customerid + "'" ?? "NULL") + ", \n " +
                                ("'" + row.name + "'" ?? "NULL") + ", \n " +
                                ("'" + row.fname + "'" ?? "NULL") + ", \n " +
                                ("'" + row.title + "'" ?? "NULL") + ", \n " +
                                ("'" + row.profession + "'" ?? "NULL") + ", \n " +
                                ("'" + row.tel1 + "'" ?? "NULL") + ", \n " +
                                ("'" + row.tel2 + "'" ?? "NULL") + ", \n " +
                                ("'" + row.fax + "'" ?? "NULL") + ", \n " +
                                ("'" + row.mobile + "'" ?? "NULL") + ", \n " +
                                ("'" + row.address1 + "'" ?? "NULL") + ", \n " +
                                ("'" + row.address2 + "'" ?? "NULL") + ", \n " +
                                ("'" + row.address_no + "'" ?? "NULL") + ", \n " +
                                ("'" + row.orofos1 + "'" ?? "NULL") + ", \n " +
                                ("'" + row.orofos2 + "'" ?? "NULL") + ", \n " +
                                ("'" + row.city + "'" ?? "NULL") + ", \n " +
                                ("'" + row.zipcode + "'" ?? "NULL") + ", \n " +
                                ("'" + row.doy + "'" ?? "NULL") + ", \n " +
                                ("'" + row.afm + "'" ?? "NULL") + ", \n " +
                                ("'" + row.email + "'" ?? "NULL") + ", \n " +
                                ("'" + row.contact + "'" ?? "NULL") + ", \n " +
                                ("'" + row.vip + "'" ?? "NULL") + ", \n " +
                                ("'" + row.member + "'" ?? "NULL") + ", \n " +
                                ("'" + row.tomeas + "'" ?? "NULL") + ", \n " +
                                ("'" + row.store + "'" ?? "NULL") + ", \n " +
                                ("'" + row.sector + "'" ?? "NULL") + ", \n " +
                                ("'" + row.diet + "'" ?? "NULL") + ", \n " +
                                ("'" + row.entolh + "'" ?? "NULL") + ", \n " +
                                ("'" + row.farsa + "'" ?? "NULL") + ", \n " +
                                ("'" + row.remarks + "'" ?? "NULL") + ", \n " +
                                (NumToText(row.amount)) + ", \n " +
                                (DateToText(row.expireddate)) + ", \n " +
                                ("'" + row.order_comments + "'" ?? "NULL") + ", \n " +
                                (DateToText(row.first_order)) + ", \n " +
                                (DateToText(row.last_order)) + ", \n " +
                                (NumToText(row.no_of_orders)) + ", \n " +
                                (NumToText(row.tziros)) + ", \n " +
                                (NumToText(row.bonus)) + ", \n " +
                                (NumToText(row.epitages)) + ", \n " +
                                (NumToText(row.zerobonus)) + ", \n " +
                                (NumToText(row.domino_false)) + ", \n " +
                                (NumToText(row.lates)) + ", \n " +
                                (NumToText(row.credit)) + ", \n " +
                                (NumToText(row.max_charge)) + ", \n " +
                                ("'" + row.company_name + "'" ?? "NULL") + ", \n " +
                                ("'" + row.bl_address + "'" ?? "NULL") + ", \n " +
                                ("'" + row.bl_address_no + "'" ?? "NULL") + ", \n " +
                                ("'" + row.bl_city + "'" ?? "NULL") + ", \n " +
                                (NumToText(row.doycode)) + " \n " +
                                 "); \n ";
                            SQL.Append(sInsert + sValues + " \n ");
                        }
                        else
                        {
                            sInsert = "UPDATE HitPosCustomers SET \n " +
                                ("name = '" + row.name + "'" ?? "NULL") + ", \n " +
                                ("fname = '" + row.fname + "'" ?? "NULL") + ", \n " +
                                ("title ='" + row.title + "'" ?? "NULL") + ", \n " +
                                ("profession = '" + row.profession + "'" ?? "NULL") + ", \n " +
                                ("tel1 = '" + row.tel1 + "'" ?? "NULL") + ", \n " +
                                ("tel2 = '" + row.tel2 + "'" ?? "NULL") + ", \n " +
                                ("fax = '" + row.fax + "'" ?? "NULL") + ", \n " +
                                ("mobile = '" + row.mobile + "'" ?? "NULL") + ", \n " +
                                ("address1 = '" + row.address1 + "'" ?? "NULL") + ", \n " +
                                ("address2 = '" + row.address2 + "'" ?? "NULL") + ", \n " +
                                ("address_no = '" + row.address_no + "'" ?? "NULL") + ", \n " +
                                ("orofos1 = '" + row.orofos1 + "'" ?? "NULL") + ", \n " +
                                ("orofos2 = '" + row.orofos2 + "'" ?? "NULL") + ", \n " +
                                ("city = '" + row.city + "'" ?? "NULL") + ", \n " +
                                ("zipcode = '" + row.zipcode + "'" ?? "NULL") + ", \n " +
                                ("doy = '" + row.doy + "'" ?? "NULL") + ", \n " +
                                ("afm = '" + row.afm + "'" ?? "NULL") + ", \n " +
                                ("email = '" + row.email + "'" ?? "NULL") + ", \n " +
                                ("contact = '" + row.contact + "'" ?? "NULL") + ", \n " +
                                ("vip = '" + row.vip + "'" ?? "NULL") + ", \n " +
                                ("member = '" + row.member + "'" ?? "NULL") + ", \n " +
                                ("tomeas = '" + row.tomeas + "'" ?? "NULL") + ", \n " +
                                ("store = '" + row.store + "'" ?? "NULL") + ", \n " +
                                ("sector = '" + row.sector + "'" ?? "NULL") + ", \n " +
                                ("diet = '" + row.diet + "'" ?? "NULL") + ", \n " +
                                ("entolh = '" + row.entolh + "'" ?? "NULL") + ", \n " +
                                ("farsa = '" + row.farsa + "'" ?? "NULL") + ", \n " +
                                ("remarks = '" + row.remarks + "'" ?? "NULL") + ", \n " +
                                ("amount = " + NumToText(row.amount)) + ", \n " +
                                ("expireddate = " + DateToText(row.expireddate)) + ", \n " +
                                ("order_comments = '" + row.order_comments + "'" ?? "NULL") + ", \n " +
                                ("first_order = " + DateToText(row.first_order)) + ", \n " +
                                ("last_order = " + DateToText(row.last_order)) + ", \n " +
                                ("no_of_orders = " + NumToText(row.no_of_orders)) + ", \n " +
                                ("tziros = " + NumToText(row.tziros)) + ", \n " +
                                ("bonus = " + NumToText(row.bonus)) + ", \n " +
                                ("epitages = " + NumToText(row.epitages)) + ", \n " +
                                ("zerobonus = " + NumToText(row.zerobonus)) + ", \n " +
                                ("domino_false = " + NumToText(row.domino_false)) + ", \n " +
                                ("lates = " + NumToText(row.lates)) + ", \n " +
                                ("credit = " + NumToText(row.credit)) + ", \n " +
                                ("max_charge = " + NumToText(row.max_charge)) + ", \n " +
                                ("company_name = '" + row.company_name + "'" ?? "NULL") + ", \n " +
                                ("bl_address = '" + row.bl_address + "'" ?? "NULL") + ", \n " +
                                ("bl_address_no = '" + row.bl_address_no + "'" ?? "NULL") + ", \n " +
                                ("bl_city = '" + row.bl_city + "'" ?? "NULL") + ", \n " +
                                ("doycode = " + NumToText(row.doycode)) + " \n " +
                             "WHERE customerid = " + CCUtility.ToSQL(item.CustomerId, FieldTypes.Text) + ";";
                            SQL.Append(sInsert);
                        }
                    }

                    if (SQL.Length > 0)
                    {
                        if (!ExecSQLs(wbCon, SQL.ToString()))
                            return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error("FillTables : SQL [" + SQL.ToString() + "] \r\n " + ex.ToString());
                return false;

            }
        }

        /// <summary>
        /// Get's List Of Orders From HitPosOrders
        /// </summary>
        /// <param name="WebPos"></param>
        /// <param name="SalesTypes"></param>
        /// <param name="InvoiceTypes"></param>
        /// <returns></returns>
        private static List<OrdersExternal> GetOrders(SqlConnection WebPos, string SalesTypes, string InvoiceTypes)
        {
            StringBuilder SQL = new StringBuilder();
            try
            {
                SQL.Clear();
                SQL.Append(SalesTypes + " \n " + InvoiceTypes + " \n" +
                 "SELECT DISTINCT NULL ClientPosId,0 Cover,MAX(hpo.start_time) [Day],d.[Description] DepartmentDescription,d.Id DepartmentId, \n"
                    + "     CAST(0 AS DECIMAL(19,4)) Discount,NULL DiscountRemark,'True' IsPrinted,CAST(0 AS BIGINT) ModifyOrderDetails,NULL PdaModuleId,CAST(CASE WHEN ISNULL(sl.PosInfoId,0) > 0 THEN sl.PosInfoId ELSE " + settings.PosInfoId.ToString() + " END AS BIGINT) PosInfoId,NULL Sender, \n"
                    + "     ISNULL(s.LastName,'')+' '+ISNULL(s.FirstName,'') Staff,s.Id StaffId,NULL TableCode,NULL TableId,NULL TableSum, \n"
                    + "     ISNULL(hpc.bl_address, '') + ' ' + ISNULL(hpc.bl_address_no, '') BillingAddress, \n"
                    + "     hpc.bl_city BillingCity,hpc.zipcode BillingZipCode,ISNULL(hpc.fname,'')+' '+ISNULL(hpc.name,'') CustomerName, \n"
                    + "     LTRIM(RTRIM(MAX(ISNULL(hpc.remarks,''))))+' '+LTRIM(RTRIM(MAX(ISNULL(hpo.comments,'')))) CustomerRemarks,hpc.orofos1 [Floor], \n"
                    + "     hpc.tel1 Phone,ISNULL(hpc.address1,'')+' '+ISNULL(hpc.address_no,'') ShippingAddress, \n"
                    + "     hpc.city ShippingCity,hpc.zipcode ShippingZipCode,'' StoreRemarks,CAST(0 AS BIGINT) [Counter], \n"
                    + "     1 ReceiptNo,CAST(" + settings.InvoiceType.ToString() + " AS BIGINT) InvoiceTypeId, \n"
                    + "     CAST(" + settings.InvoiceType.ToString() + " AS BIGINT) InvoiceIndex,pid.Id PosInfoDetailId,pid.[Description] InvoiceDescription,it.[Type] InvoiceTypeType, \n"
                    + "     pid.Abbreviation Abbreviation,'False' CreateTransactions,hpo.orderno OrderNo,hp.Total Total,hp.Total TotalBeforeDiscount, \n"
                    + "     hp.TotalVat Vat,hp.TotalNet Net,CAST(hpo.orderno AS VARCHAR(255)) ExtKey, CASE WHEN CAST(sl.HitSales AS NVARCHAR(200)) = 'TA' THEN 1 ELSE 2 END ExtType, hpo.room, \n"
                    + "     pif.[Description] PosInfoDescription, s.Code StaffCode, ISNULL(s.FirstName,'')+' '+ISNULL(s.LastName,'') StaffName, 'False' IsVoided, \n"
                    + "     0 IsPaid, 0 PaidTotal, i.WebCode, i.WebType, \n "
                    + "     CASE WHEN CHARINDEX(';',hpo.payment) > 0 THEN " + settings.CreditCardId.ToString() + " ELSE " + settings.CashId.ToString() + " END AccountType, \n"
                    + "     CASE WHEN (hpo.pos IS NULL) AND (CAST(hpo.sp AS VARCHAR(20)) = 'D') THEN " + ((int)OrderOriginEnum.Web).ToString() + " ELSE " + ((int)OrderOriginEnum.CallCenter).ToString() + " END OrderOrigin, \n"
                    + "     MAX(del.EstTakeoutDate) EstTakeoutDate, MAX(del.IsDelay) IsDelay \n"
                    + "FROM HitPosOrders hpo \n"
                    + "INNER JOIN @salesType sl ON CAST(ISNULL(sl.HitSales,'') AS VARCHAR(100)) = CAST(ISNULL(hpo.sp,'') AS VARCHAR(100)) \n"
                    + "INNER JOIN PosInfo pif ON pif.Id = CASE WHEN ISNULL(sl.PosInfoId,0) > 0 THEN sl.PosInfoId ELSE " + settings.PosInfoId.ToString() + " END \n"
                    + "INNER JOIN Department d ON d.Id = pif.DepartmentId \n"
                    + "INNER JOIN Staff s ON s.Id = sl.WebSales \n"
                    + "INNER JOIN HitPosCustomers hpc ON hpc.customerid = hpo.room \n"
                    + "INNER JOIN @InvoiceCode i ON i.hitCode = LTRIM(RTRIM((CASE WHEN CHARINDEX(';',hpo.payment) > 0 THEN SUBSTRING(hpo.payment,1,CHARINDEX(';',hpo.payment)-1) ELSE hpo.payment END))) \n"
                    + "OUTER APPLY( \n"
                    + "	SELECT TOP 1 * \n"
                    + "	FROM PosInfoDetail pid \n"
                    + "	WHERE pid.PosInfoId = CASE WHEN ISNULL(sl.PosInfoId,0) > 0 THEN sl.PosInfoId ELSE " + settings.PosInfoId.ToString() + " END AND pid.InvoicesTypeId = " + settings.InvoiceType.ToString() + " \n"
                    + ") pid \n"
                    + "LEFT OUTER JOIN InvoiceTypes it ON it.Id = " + settings.InvoiceType.ToString() + " \n"
                    + "CROSS APPLY( \n"
                    + "	SELECT CAST(SUM(finTot.Total) AS DECIMAL(19,4)) Total, CAST(SUM(finTot.TotalVat) AS DECIMAL(19,4)) TotalVat, CAST(SUM(finTot.Total - finTot.TotalVat) AS DECIMAL(19,4)) TotalNet \n"
                    + "	FROM ( \n"
                    + "		SELECT tot.Total Total, ROUND(tot.Total - (tot.Total/(1+(tot.Percentage/100))),2) TotalVat \n"
                    + "		FROM ( \n"
                    + "			SELECT CASE WHEN hps.item_code = 913 THEN ROUND(CAST(hps.total AS FLOAT)/100,2) ELSE ROUND((CAST(hps.amount * hps.qty AS FLOAT))/100,2) END Total, v.Percentage \n"
                    + "			FROM HitPosOrders hps \n"
                    + "			LEFT OUTER JOIN Vat v ON v.Code = hps.item_vat \n"
                    + "			WHERE hps.orderno = hpo.orderno \n"
                    + "		) tot \n"
                    + "	) finTot \n"
                    + ")hp \n"
                    + "CROSS APPLY ( \n"
                    + "	SELECT CASE WHEN DATEDIFF(minute, GETDATE(), fn.delivery_time) > " + settings.SetDelayAfterMinutes.ToString() + " AND " + settings.SetDelayAfterMinutes.ToString() + " > 0 AND " + settings.ExtType.ToString() + " = 1 THEN fn.delivery_time ELSE NULL END EstTakeoutDate, \n"
                    + "		CASE WHEN DATEDIFF(minute, GETDATE(), fn.delivery_time) > " + settings.SetDelayAfterMinutes.ToString() + " AND " + settings.SetDelayAfterMinutes.ToString() + " > 0 AND " + settings.ExtType.ToString() + " = 1 THEN 1 ELSE 0 END IsDelay \n"
                    + "	FROM ( \n"
                    + "		SELECT MAX(hpor.delivery_time) delivery_time \n"
                    + "		FROM HitPosOrders AS hpor \n"
                    + "		WHERE hpor.orderno = hpo.orderno AND hpor.completed = hpo.completed AND hpor.shop_id = hpo.shop_id \n"
                    + "	) fn \n"
                    + ") del \n"
                    + "WHERE hpo.completed = 0 AND hpo.shop_id = '" + settings.ShopId + "' \n"
                    + "GROUP BY hpo.orderno, pif.[Description], pid.Id, pid.[Description],it.[Type], pid.Abbreviation, \n"
                    + "     d.[Description], d.Id, s.Code, s.LastName, s.FirstName, s.Id, hpc.bl_address, hpc.bl_address_no, hpc.bl_city, hpc.zipcode, \n"
                    + "     hpc.name, hpc.fname, hpc.remarks, hpc.orofos1, hpc.tel1, hpc.address1, hpc.address_no, hpc.city, hp.Total, hp.TotalVat, hp.TotalNet, \n"
                    + "     hpo.room, i.WebCode, i.WebType, hpo.payment, hpo.pos, CAST(hpo.sp AS VARCHAR(20)), CASE WHEN ISNULL(sl.PosInfoId,0) > 0 THEN sl.PosInfoId ELSE " + settings.PosInfoId.ToString() + " END, \n"
                    + "     CASE WHEN CAST(sl.HitSales AS NVARCHAR(200)) = 'TA' THEN 1 ELSE 2 END ");

                return WebPos.Query<OrdersExternal>(SQL.ToString()).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("GetOrders [SQL : " + SQL.ToString() + "] \r\n " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// create Model of orders to send to api
        /// </summary>
        /// <param name="extOrd"></param>
        /// <param name="WebPosConn"></param>
        /// <param name="SalesTypes"></param>
        /// <returns></returns>
        private static List<Orders> CreateOrderModel(List<OrdersExternal> extOrd, SqlConnection WebPosConn,
            string SalesTypes, out List<Int64> updOrders)
        {
            List<Orders> res = new List<Orders>();
            updOrders = new List<Int64>();
            try
            {
                bool bCont = true;
                foreach (OrdersExternal row in extOrd)
                {
                    //Counter ++;
                    ExtObj exObj = new ExtObj();
                    exObj.OrderNo = row.ExtKey ?? null;
                    exObj.Status = 0; //Get Status From Table
                    exObj.InvoiceCode = row.WebCode ?? null;
                    exObj.InvoiceType = row.WebType ?? null;
                    exObj.AccountType = row.AccountType ?? null;
                    row.ExtObj = JsonConvert.SerializeObject(exObj);
                    row.ReceiptDetails = new List<OrdertDetails>();
                    row.ReceiptDetails = GetOrderDetails(WebPosConn, Int64.Parse(row.ExtKey), SalesTypes);

                    DeliveryCustomerModel CustomerModel = new DeliveryCustomerModel();

                    bCont = true;
                    if (!string.IsNullOrEmpty(row.Room))
                    {
                        CustomerModel = MakeCustomerModel(WebPosConn, row.Room);
                        CustomerModel = PostCustomerToApi(CustomerModel);
                        if (CustomerModel == null)
                            bCont = false;
                        else
                        {
                            string tmp = (CustomerModel.BillingAddresses != null ? CustomerModel.BillingAddresses.Where(w => w.IsSelected == true).FirstOrDefault().Latitude : settings.ShopLatitude.ToString()).Replace(',', '.');
                            row.Latitude = float.Parse(tmp, CultureInfo.InvariantCulture);
                            tmp = (CustomerModel.BillingAddresses != null ? CustomerModel.BillingAddresses.Where(w => w.IsSelected == true).FirstOrDefault().Longtitude : settings.ShopLatitude.ToString()).Replace(',', '.');
                            row.Longtitude = float.Parse(tmp, CultureInfo.InvariantCulture);
                            row.ShippingAddressId = ((CustomerModel.ShippingAddresses == null) || (CustomerModel.ShippingAddresses.Count < 1)) ? 0 : (CustomerModel.ShippingAddresses.Where(w => w.IsSelected == true).FirstOrDefault().ID);

                            row.BillingAddressId = (CustomerModel.BillingAddresses == null) ? 0 : CustomerModel.BillingAddresses.Where(w => w.IsSelected == true).FirstOrDefault().ID;
                            row.BillingName = CustomerModel.BillingName;
                            row.BillingVatNo = CustomerModel.BillingVatNo;
                            row.BillingDOY = CustomerModel.BillingDOY;
                            row.BillingJob = CustomerModel.BillingJob;
                            row.CustomerID = CustomerModel.ID;

                        }
                    }

                    if (bCont)
                    {
                        row.ReceiptPayments = new List<PaymentsDetails>();

                        row.ReceiptPayments = GetPayments(WebPosConn, Int64.Parse((row.ExtKey)), CustomerModel.GuestId ?? 0, CustomerModel.LastName + " " + CustomerModel.FirstName);
                        row.OrderNo = "";
                        row.ReceiptNo = 0;
                        res.Add(row);
                        updOrders.Add(long.Parse((string.IsNullOrEmpty(row.ExtKey) ? "0" : row.ExtKey)));

                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                logger.Error("Create Orde Model \r\n" + ex.ToString());
                return null;
            }

        }

        #endregion



        #region Forkey

        /// <summary>
        /// Forkey connection (ExtType = 3)
        /// </summary>
        private static void ForkeyDelivery()
        {
            string ForkeyModelStr = GetForkeyModel();
            logger.Info("Forkey JSoN : " + ForkeyModelStr);
            //string ForkeyModelStr = File.ReadAllText(@"C:\forkmodel.txt");
            if (!string.IsNullOrEmpty(ForkeyModelStr))
            {
                List<ForkeyDeliveryOrder> model = JsonConvert.DeserializeObject<List<ForkeyDeliveryOrder>>(ForkeyModelStr);

                foreach (ForkeyDeliveryOrder item in model)
                {
                    item.dependencies = new ForkeyPosDependencies();
                    item.dependencies.PosInfoId = settings.PosInfoId;
                    item.dependencies.PosInfoDetailId = settings.ForkyPosInfoCaptensOrder ?? 0;
                    item.dependencies.StaffId = settings.StaffId;
                    item.dependencies.SalesTypeId = settings.ForkeySalesTypeId ?? 0;
                    item.dependencies.PricelistId = settings.PriceListId;

                    if (item.invoice_type.ToUpper() == "INVOICE")
                    {
                        item.dependencies.InvoiceTypeId = settings.ForkeyInvoiceType ?? 0;
                    }
                    else
                        item.dependencies.InvoiceTypeId = settings.ForkeyReceiptType ?? 0;
                    if (item.payment_method.ToUpper() == "CASH")
                        item.dependencies.AccountId = settings.CashId;
                    else
                        item.dependencies.AccountId = settings.CreditCardId;
                }
                //string ts = string.Join("", model);

                if (model != null && model.Count > 0)
                {
                    logger.Info("Forkey Model To WebAPI : " + JsonConvert.SerializeObject(model));

                    //ForkeyModelStr = JsonConvert.SerializeObject(model);
                    //if (!PostForkeyDataWithApi(ForkeyModelStr))
                    if (!PostForkeyDataWithApi(model))
                        logger.Error("ForkeyDelivery : Model[" + ForkeyModelStr + "] \r\n not posted");
                }
            }
        }

        /// <summary>
        /// Create Headers for the rest client
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="authenticationType">type of authentication (Basic or OAuth2)</param>
        /// <param name="user">user and password or token for Authentication Header. Format for Basic: "Username:Password", Format for OAuth2: "Bearer  ZTdmZmY1Zjc5MTQ4NDQ5ZTEzMzIyZTOQ"</param>
        /// <param name="headers">custom headers </param>
        private static void setHeaders(HttpClient client, string authenticationType, string user, 
            Dictionary<string, string> headers, bool Lang = false)
        {
            //1. Greate Authorization header
            if (!string.IsNullOrEmpty(user))
            {
                switch (authenticationType)
                {
                    case "Basic":
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(user)));
                        if (Lang)
                            client.DefaultRequestHeaders.Add("Accept-Language", "en");
                        break;
                    case "OAuth2":
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", user);
                        break;
                }
            }

            //2. Greate custom headers
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    if (key != null && headers[key] != null)
                        client.DefaultRequestHeaders.Add(key, headers[key]);
                }
            }
        }

        /// <summary>
        /// Get model from Forkey
        /// </summary>
        /// <returns></returns>
        private static string GetForkeyModel()
        {
            string res = "";
            try
            {
                HttpRequestMessage request;
                string mediaType = "application/json";
                string authenticationType = "OAuth2";
                Dictionary<string, string> headers = null;
                int returnCode = 0;

                request = new HttpRequestMessage();
                request.RequestUri = new Uri(settings.ForkeyURL + "backend/venues/" + settings.ShopId + "/orders");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                request.Method = HttpMethod.Get;
                // HttpClient client = new HttpClient();
                logger.Info("Get Orders From Forky : " + settings.ForkeyURL + "backend/venues/" + settings.ShopId + "/orders" + "   mediaType = application/json, authenticationType = OAuth2 ");
                using (HttpClient client = new HttpClient())
                {
                    //1. Greate  headers
                    setHeaders(client, authenticationType, settings.ForkeyUserHeader, headers);

                    //3. Send Request
                    using (HttpResponseMessage response = client.SendAsync(request).Result)
                    {
                        var readAsStringAsync = response.Content.ReadAsStringAsync();
                        returnCode = response.StatusCode.GetHashCode();
                        if (returnCode == 200)
                        {
                            return readAsStringAsync.Result;
                        }
                        else
                        {
                            res = "";
                            logger.Error("GetForkeyModel Error Code " + returnCode.ToString() + " - Error Message : " + readAsStringAsync.Result);
                        }
                    }
                }
                request.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error("GetForkeyModel : \r\n " + ex.ToString());
                res = "";
            }
            return res;
        }

        /// <summary>
        /// Post model to forkey
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static bool PatchForkeyModel(ForkeyPatchOrderModel model)
        {
            bool res = true;
            try
            {
                int returnCode = 0;
                string ErrorMess = "";
                string mediaType = "application/json";
                string authenticationType = "OAuth2";
                string result = "";
                logger.Info("Patch Forky Model : " + settings.ForkeyURL + "backend/orders/2" + "    mediaType = application/json, authenticationType = OAuth2,  Model [" + JsonConvert.SerializeObject(model) + "]");
                res = CallAPI_PostMethod(model, settings.ForkeyURL + "backend/orders/2", "PATCH",
                    settings.ForkeyUserHeader, null, out returnCode, out ErrorMess, out result, mediaType, authenticationType);
            }
            catch(Exception ex)
            {
                logger.Error("PatchForkeyModel : [Model : " + model.ToString() + "] \r\n " + ex.ToString());
                res = false;
            }
            return res;
        }

        /// <summary>
        /// Post data to WebPos API
        /// </summary>
        /// <param name="ForkeyDelivery"></param>
        /// <returns></returns>
        private static bool PostForkeyDataWithApi(List<ForkeyDeliveryOrder> ForkeyDelivery)
        {
            bool res = true;
            try
            {
                int returnCode = 0;
                string ErrorMess = "";
                string mediaType = "application/json";
                string authenticationType = "Basic";
                string result = "";

                logger.Info("Post Forky  odel to API : " + settings.WebApiURL + "v3/Forkey/InsertRange" + "   mediaType = application/json, authenticationType = Basic");

                res = CallAPI_PostMethod(ForkeyDelivery, settings.WebApiURL + "v3/Forkey/InsertRange", "POST",
                    settings.URLUserName + ":" + settings.URLPass, null, out returnCode, out ErrorMess, out result,
                    mediaType, authenticationType, true);
            }
            catch (Exception ex)
            {
                logger.Error("PostDataWithApi \r\n " + ex.ToString());
                res = false;
            }
            return res;

        }

        /// <summary>
        /// Call's an API URL and return ErrorCode, ErrorMess and Result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="url"></param>
        /// <param name="Method"></param>
        /// <param name="user"></param>
        /// <param name="headers"></param>
        /// <param name="returnCode"></param>
        /// <param name="ErrorMess"></param>
        /// <param name="result"></param>
        /// <param name="mediaType"></param>
        /// <param name="authenticationType"></param>
        /// <param name="AddLanguange"></param>
        /// <returns></returns>
        private static bool CallAPI_PostMethod<T>(T model, string url, string Method, string user, 
            Dictionary<string, string> headers, out int returnCode, out string ErrorMess,out string result, 
            string mediaType = "application/json", string authenticationType = "Basic", bool AddLanguange=false)
        {
            bool res = true;
            returnCode = 0;
            ErrorMess = "";
            result = "";
            try
            {
                HttpRequestMessage request;
                request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                request.Method = new HttpMethod(Method);
                MediaTypeFormatter formatter;
                if (mediaType == "application/json")
                    formatter = new JsonMediaTypeFormatter();
                else
                    formatter = new XmlMediaTypeFormatter();

                request.Content = new ObjectContent<T>(model, formatter);

                using (HttpClient client = new HttpClient())
                {
                    //1. Greate headers
                    setHeaders(client, authenticationType, user, headers, AddLanguange);

                    //2. Send Request
                    using (HttpResponseMessage response = client.SendAsync(request).Result)
                    {
                        var readAsStringAsync = response.Content.ReadAsStringAsync();
                        returnCode = response.StatusCode.GetHashCode();
                        if (returnCode >= 200 && returnCode <= 299)
                        {
                            res = true;
                            result = readAsStringAsync.Result;
                        }
                        else
                        {
                            res = false;
                            logger.Error("CallAPI_PostMethod : \r\n " + readAsStringAsync.Result);
                        }
                    }
                }
                request.Dispose();
            }
            catch (Exception ex)
            {
                res = false;
                logger.Error("CallAPI_PostMethod : \r\n" + ex.ToString());
            }
            return res;
        }


        #endregion

    }


}
