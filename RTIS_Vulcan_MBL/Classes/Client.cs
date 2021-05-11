using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RTIS_Vulcan_MBL
{
    class Client
    {

        #region User Login

        public static string GetUserName(string UserPin)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETUSERNAME*@" + UserPin);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception)
            {
                DataClient.Close();
                return "-1*";
            }
        }

        #endregion

        #region PO Receiving
        public static string CreatePODB(string orderNum)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                //DataClient.SendTimeout = 30000;
                //DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CREATEPODB*@" + orderNum);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string getOfflinePODB(string orderNum)
        {
            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                //DataClient.SendTimeout = 30000;
                //DataClient.ReceiveTimeout = 30000;

                DataClient.Connect(ServerEP);
                sendbytes = ascenc.GetBytes("*GETOFFLINEPODATABASE*@" + orderNum);
                DataClient.Send(sendbytes);

                using (var output = File.Create(Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim.ToString()).ToString(), "Database.db3")))
                {
                    var buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = DataClient.Receive(buffer)) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                }

                DataClient.Close();
                return "1*Success";
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string postPOLines(string info)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                //DataClient.SendTimeout = 30000;
                //DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*MBLPOSTPOLINES*@" + info);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();                
                if (ex.Message.ToUpper() == "CONNECTION RESET BY PEER")
                {
                    return "-2*Connection error" + System.Environment.NewLine + ex.Message;
                }
                else
                {
                    return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
                }

            }
        }

        public static string postPOLineSingle(string info)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                //DataClient.SendTimeout = 30000;
                //DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*MBLPOSTPOLINESINGLE*@" + info);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;

            }
        }

        public static string postPOLineUnq(string info)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                //DataClient.SendTimeout = 30000;
                //DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*MBLPOSTPOLINESUNQ*@" + info);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;

            }
        }
        #endregion

        #region Powder Prep
        public static string ValidatePPItem(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*VALIDATEPPITEM*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string PrepPowder(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*PREPPOWDER*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string ValidatePPItemWht(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*VALIDATEPPITEMWHT*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string tansferPowder(string powderInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERFROMPOWDERPREP*@" + powderInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Fresh Slurry
        public static string CheckFSLotExists(string lotNumber)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKSLURYYLOTEXISTS*@" + lotNumber);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string CheckFreshSLurryInUse(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKSLURRYINUSEANDRMREQ*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string ValidateFSRaw(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*VALIDATEFSRM*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string InvalidateSlurry(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CLOSESLURRY*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string AddNewFreshSlurry(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*ADDNEWFRESHSLURRYWRAW*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string AddNewFreshSlurryNoRaws(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*ADDNEWFRESHSLURRY*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string checkFreshSlurryManufactured(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETSLURRYNONMANUFACTURED*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string setSlurrySolidity(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*SETSLURRYSOLIDITY*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string getFreshSlurryInfo(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GEFSTRANSFERINFO*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string tansferFreshSlurry(string powderInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERFRESHSLURRY*@" + powderInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Mixed Slurry
        public static string GetFreshSlurryInfoRec(string info)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETFRESHSLURRYINFO*@" + info);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }

        #region Start Mix
        public static string checkMixedSlurryTankInUse(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKMIXEDSLURRYTANKINUSE*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string insertMixedSlurryRecord(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*STARTMIXEDSLURRY*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string closeBufferSlurry(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CLOSEBUFFERSLURRY*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Enter Remaining
        public static string checkMizedSlurryRem(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECkMIXEDSLURRYREM*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string setMixedSlurryRemaining(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*SETMIXEDSLURRYREMAINING*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Enter Recovered
        public static string checkMizedSlurryRec(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECkMIXEDSLURRYREC*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string setMixedSlurryRecovered(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*SETMIXEDSLURRYRECOVERED*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Add Fresh Slurry
        public static string checkSlurryTankValid(string slurryInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKTANKAVAILABLE*@" + slurryInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string checkSlurryTrolley(string slurryInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETFSTROLLEYINFOMS*@" + slurryInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string saveFreshSlurryAddition(string slurryInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*ADDFRESHSLURRYTOMIX*@" + slurryInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Decant
        public static string checkBufferTankValid(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKBUFFERTANKFORDECANT*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string checkMobileTankValid(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKDECANTTANK*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string insertDecantLine(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*ADDDECANTSLURRYLINE*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region ZAC

        public static string GetZACChemicals()
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETALLZACCHEMS*@");
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string checkTankValidZAC(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKSLURRYTANKZANDC*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string updateZACChem(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATEZACCHEM*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Solidity
        public static string getTankInfoSolidity(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETTANKINFOSOL*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string setMixedSlurrySolidity(string tankInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*SETMIXEDSLURRYSOLIDITY*@" + tankInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        public static string checkMixedSlurryForTrsnsfer(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETMIXEDSLURRYTRANSFERINFO*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string getMixedSlurryWarehouses()
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETMIXEDSLURRYWAREHOUSES*@");
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string transferMixedSlurry(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERMIXEDSLURRYNEW*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Receiving Transfers
        public static string GetRecWhseFrom(string process)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETVALIDRECWAREHOUSES*@" + process);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string tansferFromITRT2D(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERFROMITRT2D*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string getPGMItemInfo(string info)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETPGMRECTRANSINFO*@" + info);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string transferFromITPGM(string info)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERFROMITPGM*@" + info);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string tansferFromITFSBCD(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERFROMITFSBCD*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string tansferFromITMSBCD(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERFROMITMSBCD*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string CheckZectJobRunning(string info)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKZECTJOBRUNNING*@" + info);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Outgoing Tranfers
        public static string getItemDescription(string itemCode)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETOUTTRANSDESC*@" + itemCode);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Transfer to Production
        public static string getToProdWarehouses()
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETTOPRODWAREHOUSES*@");
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string transferItemToProduction(string toProdInfp)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERITEMTOPROD*@" + toProdInfp);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Zect
        public static string issueToZectJob(string issueInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CONSUMERMRT2DZECT*@" + issueInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string getZectWarehouses(string zectWhse)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GEtZECTTRANSFERWAREHOUSES*@" + zectWhse);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string transferZectItem(string zectWhse)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERZECTITEM*@" + zectWhse);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region A&W
        public static string getAWWarehouses()
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETAWTRANSFERWAREHOUSES*@");
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string transferAWItem(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERAWITEM*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string getAWJobInfo(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*MBLGETAWJOBINFO*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string issueAwRM(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*ISSUEAWJOBRM*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Canning
        public static string getCanningWarehouses()
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETCANNINGTRANSFERWAREHOUSES*@");
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string transferCaningItem(string itemInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*TRANSFERCANNINGITEM*@" + itemInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Dispatch
        public static string getSOLines(string orderNum)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETSOLINES*@" + orderNum);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string updateSOLineLot(string orderInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATESOLINELOT*@" + orderInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        public static string updateSOLineNoLot(string orderInfo)
        {
            string ServerDetails = "";

            IPAddress ServerIPAddress = null;
            ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
            IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, GlobalVar.ServerPort);
            Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] sendbytes = new byte[21];
                byte[] receivebytes = new byte[3];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;

                //Send start request
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATESOLINENOLOT*@" + orderInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }

                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                DataClient.Close();
                return "-1*Cannot connect to server:" + System.Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Stock Take
        public static string GetSTNumbers()
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();

                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*EVOGETSTNUMBERS*@");
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetWHDetails(string STNum)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*EVOGETWHDETAILS*@" + STNum);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetLotInEvo(string itemCode, string lotNumber)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETLOTEXISTSINEVO*@" + itemCode + "|" + lotNumber);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string AddLotForInvestigation(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*ADDLOTFORINVESTIGATION*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetItemOnSTockTake(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETITEMONSTOCKTAKE*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string AddItemToStockTake(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*ADDITEMTOSTOCKTAKE*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string AddItemToStockTakeFreshSlurry(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*ADDITEMTOSTOCKTAKEFRESHSLURRY*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string CheckItemRT2D(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKRT2DITEMST*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string SaveStockTakeItemRT2D(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATESTOCKTAKERT2D*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string CheckItemRT2Pallet(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKPALLETST*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string CheckItemRT2RMPallet(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKRMPALLETST*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string SaveStockTakeItemPallet(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATESTOCKTAKEPALLET*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string SaveStockTakeItemPallet_RM(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATESTOCKTAKERMPALLET*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetFreshSlurryInfo_LotCheck(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETFSLOTSTOCKTAKE*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetFreshSlurryInfo_ST(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKFRESHSLURRYTROLLEY*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetMixedSlurryTankInfo_CheckLot(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETMSLOTSTOCKTAKE*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetMixedSLurryTankInfo(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKMIXEDSLURRYTANKST*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string SaveStockTakeItemFreshSlurry(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATESTOCKTAKEFRESHSLURRY*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string SaveStockTakeItemMixedSlurryTank(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATESTOCKMIXEDSLURRYTANK*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetMixedSlurryMobileTankInfo_CheckLot(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETMBLMSLOTSTOCKTAKE*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetMixedSlurryMobileTankInfo(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKMIXEDSLURRYMOBILETANKST*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string SaveStockTakeItemMixedSlurryMTank(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATESTOCKMIXEDSLURRYMOBILETANK*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetPGMContainerInfo(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*CHECKPGMSTOCKTAKE*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string SaveStockTakePGM(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATESTOCKTAKEPGM*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string SaveStockTakeItemPowderPrep(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*UPDATEPOWDERPREPSTOCKTAKE*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string SaveStockTakeItemManual(string STInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*SAVESTOCKTAKEMANUAL*@" + STInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        #endregion

        #region Palletizing
        public static string GetPalletItemDesc(string itemCode)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETPALLETITMDESC*@" + itemCode);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string SavePalletInfo(string palletInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*SAVEPALLETITEMS*@" + palletInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string GetPalletInfo(string palletNumber)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*GETPALLETINFO*@" + palletNumber);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string RemoveItemFromPallet(string palletInfo)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*REMOVEITEMFROMPALLET*@" + palletInfo);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        public static string PrintPalletLabel(string palletCode)
        {
            try
            {
                string ServerDetails = "";

                IPAddress ServerIPAddress = null;
                ServerIPAddress = IPAddress.Parse(GlobalVar.ServerIP);
                IPEndPoint ServerEP = new IPEndPoint(ServerIPAddress, Convert.ToInt32(GlobalVar.ServerPort));
                Socket DataClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                byte[] sendbytes = new byte[20];
                byte[] receivebytes = new byte[2];
                ASCIIEncoding ascenc = new ASCIIEncoding();


                DataClient.SendTimeout = 30000;
                DataClient.ReceiveTimeout = 30000;
                DataClient.Connect(ServerEP);

                sendbytes = ascenc.GetBytes("*PRINTNEWPALLETLABEL*@" + palletCode);
                DataClient.Send(sendbytes);

                receivebytes = new byte[131073];
                //                receivebytes = new byte[147483591];
                int length = DataClient.Receive(receivebytes);
                int count = length;
                while (length != 0)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        ServerDetails += Convert.ToChar(receivebytes[i]);
                        count = count - 1;
                    }
                    count = DataClient.Receive(receivebytes);
                    length = count;
                }


                DataClient.Close();
                return ServerDetails;
            }
            catch (Exception ex)
            {
                return "-1*Cannot connect to server - " + ex.Message;
            }
        }
        #endregion
    }
}