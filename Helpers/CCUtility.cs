using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge
{
    public enum FieldTypes { Text, Number, Date, Memo }
    class CCUtility
    {
        public static string ToSQL(string Param, FieldTypes Type)
        {
            if (Param == null || Param.Length == 0)
            {
                if (Type == FieldTypes.Number)
                    return "0";
                if (Type == FieldTypes.Text)
                    return "''";
                return "''";
            }
            else
            {
                string str = Quote(Param);
                if (Type == FieldTypes.Number)
                {
                    return str.Replace(',', '.');
                }
                else
                {
                    return "\'" + str + "\'";
                }
            }
        }

        public static string ToSQLDecimal(Decimal? Param)
        {
            if (Param.HasValue)
            {
                return Param.Value.ToString(new CultureInfo("en-US"));
            }
            else
            {
                return "0";
            }
        }

        public static string ToSQLDecimal(string Param)
        {
            decimal dParam;

            if (decimal.TryParse(Param.Replace(',', '.'), NumberStyles.Number, new CultureInfo("en-US"), out dParam))
            {
                return Math.Round(dParam, 0, System.MidpointRounding.AwayFromZero).ToString(new CultureInfo("en-US"));
            }
            else
            {
                return "0";
            }
        }

        public static string ToSQLDecimal(string Param, int nDecs)
        {
            decimal dParam;

            if (decimal.TryParse(Param.Replace(',', '.'), NumberStyles.Number, new CultureInfo("en-US"), out dParam))
            {
                return Math.Round(dParam, nDecs, System.MidpointRounding.AwayFromZero).ToString(new CultureInfo("en-US"));
            }
            else
            {
                return "0";
            }
        }

        public static string NullToSQL(string Param, FieldTypes Type)
        {
            if (Param == null || Param.Length == 0)
            {
                if (Type == FieldTypes.Number)
                    return "0";
                if (Type == FieldTypes.Text)
                    return "";
                return "";
            }
            else
            {
                string str = Quote(Param);
                if (Type == FieldTypes.Number)
                {
                    return str.Replace(',', '.');
                }
                else
                {
                    return str;
                }
            }
        }
        

        public CCUtility(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }


        public static void work() { }



        public static String GetValFromLOV(String val, String[] arr)
        {
            String ret = "";
            if (arr.Length % 2 == 0)
            {
                int temp = Array.IndexOf(arr, val);
                ret = temp == -1 ? "" : arr[temp + 1];
            }
            return ret;
        }



        public bool IsNumeric(object source, string value)
        {
            try
            {
                Decimal temp = Convert.ToDecimal(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string Quote(string Param)
        {
            if (Param == null || Param.Length == 0)
            {
                return "";
            }
            else
            {
                return Param.Replace("'", "''");
            }
        }

        public static string GetValue(DataRow row, string field)
        {
            if (row[field].ToString() == null)
                return "";
            else
                return row[field].ToString();
        }

        public SqlConnection Connection;

        public DataSet FillDataSet(string sSQL)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter command = new SqlDataAdapter(sSQL, Connection);
            command.SelectCommand.CommandTimeout = 60;
            command.Fill(ds);
            return ds;
        }

        public int FillDataSet(string sSQL, ref DataSet ds)
        {
            SqlDataAdapter command = new SqlDataAdapter(sSQL, Connection);
            return command.Fill(ds, "Table");
        }

        public int FillDataSet(string sSQL, ref DataSet ds, int start, int count)
        {
            SqlDataAdapter command = new SqlDataAdapter(sSQL, Connection);
            return command.Fill(ds, start, count, "Table");
        }


        private string ConnectionString;
        public void DBOpen()
        {
            // open DB Connection via Sql
            Connection = new SqlConnection(ConnectionString);
            Connection.Open();
        }


        public void DBClose()
        {
            Connection.Close();
        }



        public void DBDispose()
        {
            Connection.Dispose();
        }



        public string Dlookup(string table, string field, string sWhere)
        {
            string sSQL = "SELECT " + field + " FROM " + table + " WHERE " + sWhere;

            SqlCommand command = new SqlCommand(sSQL, Connection);
            SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
            string sReturn;

            if (reader.Read())
            {
                sReturn = reader[0].ToString();
                if (sReturn == null)
                    sReturn = "";
            }
            else
            {
                sReturn = "";
            }

            reader.Close();
            return sReturn;
        }



        public string Dlookup(string table, string field)
        {
            string sSQL = "SELECT " + field + " FROM " + table;

            SqlCommand command = new SqlCommand(sSQL, Connection);
            SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
            string sReturn;

            if (reader.Read())
            {
                sReturn = reader[0].ToString();
                if (sReturn == null)
                    sReturn = "";
            }
            else
            {
                sReturn = "";
            }

            reader.Close();
            return sReturn;
        }

        public string Dlookup(string table, string field, SqlConnection Connection)
        {
            string sSQL = "SELECT " + field + " FROM " + table;

            SqlCommand command = new SqlCommand(sSQL, Connection);
            SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
            string sReturn;

            if (reader.Read())
            {
                sReturn = reader[0].ToString();
                if (sReturn == null)
                    sReturn = "";
            }
            else
            {
                sReturn = "";
            }

            reader.Close();
            return sReturn;
        }

        public int DlookupInt(string table, string field, string sWhere)
        {
            string sSQL = "SELECT " + field + " FROM " + table + " WHERE " + sWhere;

            SqlCommand command = new SqlCommand(sSQL, Connection);
            SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
            int iReturn = -1;

            if (reader.Read())
            {
                iReturn = reader.GetInt32(0);
            }

            reader.Close();
            return iReturn;
        }
        public int DlookupInt(string table, string field)
        {
            string sSQL = "SELECT " + field + " FROM " + table;
            SqlCommand command = new SqlCommand(sSQL, Connection);
            SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
            int iReturn = -1;

            if (reader.Read())
            {
                iReturn = reader.GetInt32(0);
            }
            reader.Close();
            return iReturn;
        }

        public void Execute(string sSQL)
        {
            SqlCommand cmd = new SqlCommand(sSQL, Connection);
            cmd.ExecuteNonQuery();
        }

        public bool ExeSQL(string sSQL)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(sSQL, Connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Log.ToErrorLog(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return false;
            }
        }

        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            int ch;
            for (int i = 0; i < size; i++)
            {
                //	ch = Convert.ToChar(Convert.ToInt32(24 * random.NextDouble() + 65)) ;
                ch = Convert.ToChar(random.Next(9));
                //Convert.ToChar(Convert.ToInt32(24 * random.NextDouble() + 65)) ;
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public double DateDiff(string howtocompare, System.DateTime
            startDate, System.DateTime endDate)
        {
            double diff = 0;
            try
            {
                System.TimeSpan TS = new
                    System.TimeSpan(startDate.Ticks - endDate.Ticks);
                #region converstion options
                switch (howtocompare.ToLower())
                {
                    case "m":
                        diff = Convert.ToDouble(TS.TotalMinutes);
                        break;
                    case "s":
                        diff = Convert.ToDouble(TS.TotalSeconds);
                        break;
                    case "t":
                        diff = Convert.ToDouble(TS.Ticks);
                        break;
                    case "mm":
                        diff = Convert.ToDouble(TS.TotalMilliseconds);
                        break;
                    case "yyyy":
                        diff = Convert.ToDouble(TS.TotalDays / 365);
                        break;
                    case "q":
                        diff = Convert.ToDouble((TS.TotalDays / 365) / 4);
                        break;
                    default:
                        //d
                        diff = Convert.ToDouble(TS.TotalDays);
                        break;
                }
                #endregion
                int diffint = Convert.ToInt32(diff);
                if (diffint > diff)
                    diffint = diffint - 1;
                diff = diffint;

            }
            catch (Exception)
            {
                diff = -1;
            }
            return diff;
        }

        public DataSet ConvertToDataset(string File, string TableName, string delimiter)
        {
            //The DataSet to Return
            DataSet result = new DataSet();

            //Open the file in a stream reader.
            StreamReader s = new StreamReader(File);

            //Split the first line into the columns       
            string[] columns = { "MEMBER", "FNAME", "LNAME", "ARRIVAL", "DEPARTURE", "RESDATE", "ROOMNUMBER", "ROOMTYPE", "LOGIS", "FNB", "TYPE", "ISCREW", "ISGROUP", "ISTRAVEL", "EMPTY", "sMEMBER", "sFNAME", "sLNAME", "sARRIVAL", "sDEPARTURE", "sRESDATE", "sROOMNUMBER", "sROOMTYPE", "sLOGIS", "sFNB", "sTYPE", "sISCREW", "sISGROUP", "sISTRAVEL", "sEMPTY" };//.ReadLine().Split(delimiter.ToCharArray());

            //Add the new DataTable to the RecordSet
            result.Tables.Add(TableName);

            //Cycle the colums, adding those that don't exist yet 
            //and sequencing the one that do.
            foreach (string col in columns)
            {
                bool added = false;
                string next = "";
                int i = 0;
                while (!added)
                {
                    //Build the column name and remove any unwanted characters.
                    string columnname = col + next;
                    columnname = columnname.Replace("#", "");
                    columnname = columnname.Replace("'", "");
                    columnname = columnname.Replace("&", "");

                    //See if the column already exists
                    if (!result.Tables[TableName].Columns.Contains(columnname))
                    {
                        //if it doesn't then we add it here and mark it as added
                        result.Tables[TableName].Columns.Add(columnname);
                        added = true;
                    }
                    else
                    {
                        //if it did exist then we increment the sequencer and try again.
                        i++;
                        next = "_" + i.ToString();
                    }
                }
            }

            //Read the rest of the data in the file.        
            string AllData = s.ReadToEnd();

            //Split off each row at the Carriage Return/Line Feed
            //Default line ending in most windows exports.  
            //You may have to edit this to match your particular file.
            //This will work for Excel, Access, etc. default exports.
            string[] rows = AllData.Split("\r\n".ToCharArray());

            //Now add each row to the DataSet        
            foreach (string r in rows)
            {
                //Split the row at the delimiter.
                if (!(r.Equals("")))
                {
                    string[] items = r.Split(delimiter.ToCharArray());

                    //Add the item
                    result.Tables[TableName].Rows.Add(items);
                }
            }

            //Return the imported data.        
            return result;
        }

        public static int GetNumberofDays(string dteDateToBePassed)
        {
            DateTime tmpDateTest = System.Convert.ToDateTime(dteDateToBePassed, new CultureInfo("el-GR"));

            //Gets the First Month of the Year and Add One Month to get number of days in Feb
            DateTime tmpDate = (System.Convert.ToDateTime("31/1/" + tmpDateTest.Year.ToString(), new CultureInfo("el-GR"))).AddMonths(1);

            // G E T S N U M B E R O F D A Y S I N F E B
            bool isLeapYear = ((tmpDate.Day == 29) ? true : false);
            // A R R A Y F O R M O N T H D A Y S
            string[] getDays = { "", "31", (isLeapYear == true ? "29" : "28"), "31", "30", "31", "30", "31", "31", "30", "31", "30", "31" };

            int numberOfDays = System.Convert.ToInt32(getDays[tmpDateTest.Month]);
            return numberOfDays;
        }
    }
}
