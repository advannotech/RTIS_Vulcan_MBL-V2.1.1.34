using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Mono.Data.Sqlite;
using System.IO;

namespace RTIS_Vulcan_MBL.Classes
{
    class SQLite
    {
        static string dbPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim.ToString()).ToString(), "Database.db3");
        string ReturnData = string.Empty;
        public static string getPOLines()
        {
            try
            {
                string ReturnData = string.Empty;
                var conn = new SqliteConnection("Data Source=" + dbPath);
                conn.Open();
                using (var contents = conn.CreateCommand())
                {
                    contents.CommandText = "SELECT [ItemCode], [Description], [LotNumber], [ViewableQty], [RecQty], [bLotItem] FROM [PO]";
                    var r = contents.ExecuteReader();
                    while (r.Read())
                    {
                        ReturnData +=  r["ItemCode"].ToString() + "|" + r["Description"].ToString() +"|" +  r["LotNumber"].ToString() +"|" +  r["ViewableQty"].ToString() +"|" + r["RecQty"].ToString() +"|" +  r["bLotItem"].ToString() + "~";
                    }
                    r.Close();
                }
                conn.Close();
                if (ReturnData != string.Empty)
                {
                    return "1*" + ReturnData;
                }
                else
                {
                    return "-1*No lines were found for this PO";
                }
                
            }
            catch (Exception ex)
            {
                return "-1*" + ex.Message;
            }
        }
        public static string checkUnqBarcode(string unq)
        {
            try
            {
                string ReturnData = string.Empty;
                var conn = new SqliteConnection("Data Source=" + dbPath);
                conn.Open();
                using (var contents = conn.CreateCommand())
                {
                    contents.CommandText = "SELECT [Receive], [bValidated] FROM [UNQ] WHERE [Barcode] = @1";
                    contents.Parameters.Add(new SqliteParameter("@1", unq));
                    var r = contents.ExecuteReader();
                    while (r.Read())
                    {
                        ReturnData += string.Format("{0}", r["Receive"].ToString()) + "|" + r["bValidated"].ToString();
                    }
                    r.Close();
                }
                conn.Close();
                if (ReturnData != string.Empty)
                {
                    return "1*" + ReturnData;
                }
                else
                {
                    return "-1*Barcode not found!";
                }

            }
            catch (Exception ex)
            {
                return "-1*" + ex.Message;
            }
        }
        public static string getUpdatedUnqs()
        {
            try
            {
                string ReturnData = string.Empty;
                var conn = new SqliteConnection("Data Source=" + dbPath);
                conn.Open();
                using (var contents = conn.CreateCommand())
                {
                    contents.CommandText = "SELECT [Barcode] FROM [UNQ] WHERE [Receive] <> ''";
                    var r = contents.ExecuteReader();
                    while (r.Read())
                    {
                        ReturnData += string.Format("{0}", r["Barcode"].ToString()) + "~";
                    }
                    r.Close();
                }
                conn.Close();
                if (ReturnData != string.Empty)
                {
                    return "1*" + ReturnData;
                }
                else
                {
                    return "-1*Barcodes were found!";
                }

            }
            catch (Exception ex)
            {
                return "-1*" + ex.Message;
            }
        }

        public static string getUpdatedUnqs_ItemLot(string code, string lot)
        {
            try
            {
                string ReturnData = string.Empty;
                var conn = new SqliteConnection("Data Source=" + dbPath);
                conn.Open();
                using (var contents = conn.CreateCommand())
                {
                    contents.CommandText = "SELECT [Barcode] FROM [UNQ] WHERE [Receive] <> '' AND  [Barcode] LIKE '%" + code+ "%' AND [Barcode] LIKE '%" + lot + "%'";
                    var r = contents.ExecuteReader();
                    while (r.Read())
                    {
                        ReturnData += string.Format("{0}", r["Barcode"].ToString()) + "~";
                    }
                    r.Close();
                }
                conn.Close();
                if (ReturnData != string.Empty)
                {
                    return "1*" + ReturnData;
                }
                else
                {
                    return "-1*Barcodes were found!";
                }

            }
            catch (Exception ex)
            {
                return "-1*" + ex.Message;
            }
        }
        public static string updatePOItemLot(string itemCode, string lot, string qty)
        {
            try
            {
                var conn = new SqliteConnection("Data Source=" + dbPath);
                conn.Open();
                using (var contents = conn.CreateCommand())
                {
                    contents.CommandText = "UPDATE [PO] SET [RecQty] = @3 WHERE [ItemCode] = @1 AND [LotNumber] = @2";
                    contents.Parameters.Add(new SqliteParameter("@1", itemCode));
                    contents.Parameters.Add(new SqliteParameter("@2", lot));
                    contents.Parameters.Add(new SqliteParameter("@3", qty));
                    var r = contents.ExecuteNonQuery();
                }
                conn.Close();
                return "1*Success";
            }
            catch (Exception ex)
            {
                return "-1*" + ex.Message;
            }
        }
        public static string updatePOItemNoLot(string itemCode, string qty)
        {
            try
            {
                var conn = new SqliteConnection("Data Source=" + dbPath);
                conn.Open();
                using (var contents = conn.CreateCommand())
                {
                    contents.CommandText = "UPDATE [PO] SET [RecQty] = @2 WHERE [ItemCode] = @1 AND [bLotItem] = 'False'";
                    contents.Parameters.Add(new SqliteParameter("@1", itemCode));
                    contents.Parameters.Add(new SqliteParameter("@2", qty));
                    var r = contents.ExecuteNonQuery();
                }
                conn.Close();
                return "1*Success";
            }
            catch (Exception ex)
            {
                return "-1*" + ex.Message;
            }
        }
        public static string updateUnqScanned(string unq, string poNum)
        {
            try
            {
                var conn = new SqliteConnection("Data Source=" + dbPath);
                conn.Open();
                using (var contents = conn.CreateCommand())
                {
                    contents.CommandText = "UPDATE [UNQ] SET [Receive] = @2 WHERE [Barcode] = @1";
                    contents.Parameters.Add(new SqliteParameter("@1", unq));
                    contents.Parameters.Add(new SqliteParameter("@2", poNum));
                    var r = contents.ExecuteNonQuery();
                }
                conn.Close();
                return "1*Success";
            }
            catch (Exception ex)
            {
                return "-1*" + ex.Message;
            }
        }
    }
}