using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using System;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Net.Wifi;

using Android.Content.PM;
using Android.Media;
using System.Collections.Generic;

namespace RTIS_Vulcan_MBL.Activities.J_Stock_Take
{
    [Activity(Label = "J_SelectStockTake")]
    public class J_SelectStockTake : Activity, View.IOnTouchListener
    {
        Button btnNext;
        Button btnBack;
        Spinner spnSTNum;
        Spinner spnWH;

        int Count = 0;

        #region WiFi / Battery / OnError / Version

        #region Error

        bool threadrunning;
        string ErrorText;
        TextView lblErrorMsg;
        Button btnErrorOk;
        MediaPlayer _player;

        #endregion

        #region Success
        string SuccessText = string.Empty;
        TextView lblSuccessMsg;
        Button btnSuccessOk;
        public void showSuccess()
        {
            try
            {
                if (GlobalVar.pgsBusy != null)
                {
                    GlobalVar.pgsBusy.Dismiss();
                }
            }
            catch (Exception)
            {
            }
            SetContentView(Resource.Layout.SuccessMsg);
            lblSuccessMsg = FindViewById<TextView>(Resource.Id.lblSuccessMsg);
            lblSuccessMsg.Text = SuccessText;
            btnSuccessOk = FindViewById<Button>(Resource.Id.btnSuccessOk);
            btnSuccessOk.Click += BtnSuccessOk_Click;
        }

        private void BtnSuccessOk_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ScanItem_RecTrans));
            Finish();
        }
        #endregion

        #region Battery

        int batterylevel = 0;
        bool batrun = false;
        bool ischarging = false;
        ImageView imgBat;

        #endregion

        #region WiFi

        int signalstrength = 0;
        bool run = false;
        ImageView imgWifi;

        #endregion

        TextView lblVersion;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.J_SelectST);
            #region Wifi / Battery / Error / Version

            #region Version Control

            lblVersion = FindViewById<TextView>(Resource.Id.lblVersion);
            if (GlobalVar.UserPin == null)
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName + System.Environment.NewLine + GlobalVar.ScannerID;
            }
            else
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName + System.Environment.NewLine + GlobalVar.UserName;
            }

            #endregion

            _player = MediaPlayer.Create(this, Resource.Drawable.beeperror);
            imgBat = FindViewById<ImageView>(Resource.Id.imgBat);
            imgBat.Click += ImgBat_Click;
            imgWifi = FindViewById<ImageView>(Resource.Id.imgWifi);
            BatteryLevel();
            WifiSignal();

            #endregion
            layoutStichUp();
            GlobalVar.pgsBusy = new ProgressDialog(this);
            GlobalVar.pgsBusy.Indeterminate = true;
            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
            GlobalVar.pgsBusy.SetMessage("Retrieving Stocktakes...");
            GlobalVar.pgsBusy.SetCancelable(false);
            GlobalVar.pgsBusy.Show();
            Thread t = new Thread(GetSpinnerDetails);
            if (threadrunning == false)
            {
                t.Start();
            }
            else
            {
                SetSpinners();
            }

            ScanEngineOff();
        }

        private void layoutStichUp()
        {
            btnNext = FindViewById<Button>(Resource.Id.btnSTNext);
            btnNext.Click += BtnNext_Click;
            btnNext.SetOnTouchListener(this);

            btnBack = FindViewById<Button>(Resource.Id.btnSTBack);
            btnBack.Click += BtnBack_Click;
            btnBack.SetOnTouchListener(this);

            spnSTNum = FindViewById<Spinner>(Resource.Id.spnStockTake);
            spnSTNum.ItemSelected += SpnSTNum_ItemSelected;

            spnWH = FindViewById<Spinner>(Resource.Id.spnWarehouse);
            spnWH.ItemSelected += SpnWH_ItemSelected;
        }
        private void GetSpinnerDetails()
        {
            try
            {
                threadrunning = true;
                string stockTakes = Client.GetSTNumbers();
                if (stockTakes != string.Empty)
                {
                    switch (stockTakes.Split('*')[0])
                    {
                        case "1":
                            stockTakes = stockTakes.Remove(0, 2);

                            #region Warehouse

                            GlobalVar.STWarehouses = new List<GlobalVar.WarehouseDetailsST>();
                            GlobalVar.WarehouseDetailsST thiswh = new GlobalVar.WarehouseDetailsST();
                            thiswh.Code = "62017";
                            thiswh.Description = "SELECT WAREHOUSE";
                            GlobalVar.STWarehouses.Add(thiswh);

                            #endregion

                            #region StockTake Numbers

                            GlobalVar.StockTakes = new List<GlobalVar.STDetails>();
                            GlobalVar.STDetails st1 = new GlobalVar.STDetails();
                            st1.Number = "SELECT ST";
                            GlobalVar.StockTakes.Add(st1);
                            string[] tempST = stockTakes.Split('~');
                            foreach (string t in tempST)
                            {
                                if (t != "")
                                {
                                    if (t.Substring(0, 3) != "CYC")
                                    {
                                        GlobalVar.STDetails thisST = new GlobalVar.STDetails();
                                        thisST.Number = t;
                                        GlobalVar.StockTakes.Add(thisST);
                                    }
                                }
                            }

                            #endregion

                            RunOnUiThread(SetSpinners);
                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + stockTakes.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + stockTakes.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
                threadrunning = false;
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void SetSpinners()
        {
            #region Warehouse Spinner

            List<string> temp = new List<string>();
            foreach (GlobalVar.WarehouseDetailsST w in GlobalVar.STWarehouses)
            {
                temp.Add(w.Description);
            }
            ArrayAdapter localadapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, temp.ToArray());
            localadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spnWH.Adapter = localadapter;
            spnWH.Enabled = false;

            #endregion

            #region StockTake Spinner

            List<string> temp2 = new List<string>();
            foreach (GlobalVar.STDetails st in GlobalVar.StockTakes)
            {
                temp2.Add(st.Number);
            }
            ArrayAdapter localadapter2 = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, temp2.ToArray());
            localadapter2.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spnSTNum.Adapter = localadapter2;

            #endregion

            if (GlobalVar.pgsBusy.IsShowing)
            {
                GlobalVar.pgsBusy.Dismiss();
            }
        }
        private void SpnSTNum_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                Spinner thisspinner = (Spinner)sender;
                GlobalVar.StockTake = GlobalVar.StockTakes[e.Position].Number;
                if (Count == 1)
                {
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Retrieving Warehouses For This Stock Take...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();

                    Thread t = new Thread(GetWHDetails);
                    if (threadrunning == false)
                    {
                        t.Start();
                    }
                }
                else
                {
                    Count = 1;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J006: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void GetWHDetails()
        {
            try
            {
                string warehouses = Client.GetWHDetails(GlobalVar.StockTake);
                if (warehouses != string.Empty)
                {
                    switch (warehouses.Split('*')[0])
                    {
                        case "1":
                            warehouses = warehouses.Remove(0, 2);
                            GlobalVar.STWarehouses = new List<GlobalVar.WarehouseDetailsST>();
                            string[] temp = warehouses.Split('~');
                            foreach (string t in temp)
                            {
                                if (t != "")
                                {
                                    string[] temp2 = t.Split('|');
                                    GlobalVar.WarehouseDetailsST thiswh = new GlobalVar.WarehouseDetailsST();
                                    thiswh.Code = temp2[0];
                                    thiswh.Description = temp2[1];
                                    GlobalVar.STWarehouses.Add(thiswh);
                                }
                            }

                            RunOnUiThread(SetWHSpinner);
                            break;
                        case "0":
                            ErrorText = "Exception J007: " + System.Environment.NewLine + warehouses.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J008: " + System.Environment.NewLine + warehouses.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J009: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J010: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J011: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void SetWHSpinner()
        {
            #region Warehouse Spinner

            List<string> temp = new List<string>();
            foreach (GlobalVar.WarehouseDetailsST w in GlobalVar.STWarehouses)
            {
                temp.Add(w.Description);
            }
            ArrayAdapter localadapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, temp.ToArray());
            localadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spnWH.Adapter = localadapter;
            spnWH.Enabled = true;

            #endregion

            if (GlobalVar.pgsBusy.IsShowing)
            {
                GlobalVar.pgsBusy.Dismiss();
            }
        }
        private void SpnWH_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner thisspinner = (Spinner)sender;
            GlobalVar.STWarehouse = GlobalVar.STWarehouses[e.Position].Code;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(MainActivity));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J012: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(J_ScanItem));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        #region WiFi / Battery / OnTouch / OnError / ScanEngine

        private void ShowError()
        {
            try
            {
                if (GlobalVar.pgsBusy != null)
                {
                    GlobalVar.pgsBusy.Dismiss();
                }
            }
            catch (Exception)
            {
            }
            _player.Start();

            SetContentView(Resource.Layout.Z_ErrorMsg);
            lblErrorMsg = FindViewById<TextView>(Resource.Id.lblErrorMsg);
            lblErrorMsg.Text = ErrorText;
            btnErrorOk = FindViewById<Button>(Resource.Id.btnErrorOk);
            btnErrorOk.SetOnTouchListener(this);
            btnErrorOk.Click += BtnErrorOk_Click;
        }

        private void BtnErrorOk_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
            Finish();
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            Button thisbutton = (Button)v;
            if (thisbutton.Text == "ERROR OK")
            {
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        thisbutton.SetBackgroundResource(Resource.Drawable.roundedbuttonpressred);
                        break;
                    case MotionEventActions.Up:
                        thisbutton.SetBackgroundResource(Resource.Drawable.roundedbuttonred);
                        break;
                }
            }
            else
            {
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        thisbutton.SetBackgroundResource(Resource.Drawable.roundedbuttonpress);
                        break;
                    case MotionEventActions.Up:
                        thisbutton.SetBackgroundResource(Resource.Drawable.roundedbutton);
                        break;
                }
            }
            return false;
        }

        #region Scan Engine Control
        private void ScanEngineOn()
        {
            var s = (Context)this;
            Intent intent = new Intent("ACTION_BAR_SCANCFG");
            intent.PutExtra("EXTRA_SCAN_POWER", 1);
            s.SendBroadcast(intent);
        }
        private void ScanEngineOff()
        {
            var s = (Context)this;
            Intent intent = new Intent("ACTION_BAR_SCANCFG");
            intent.PutExtra("EXTRA_SCAN_POWER", 0);
            s.SendBroadcast(intent);
        }
        #endregion

        #region Battery
        private void ImgBat_Click(object sender, EventArgs e)
        {
            var filter = new IntentFilter(Intent.ActionBatteryChanged);
            var battery = RegisterReceiver(null, filter);
            int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
            int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);

            int BPercetage = (int)System.Math.Floor(level * 100D / scale);
            Toast.MakeText(this, "Battery is at " + BPercetage.ToString() + "% power...", ToastLength.Long).Show();
        }
        private void BatteryLevel()
        {
            batrun = true;
            Thread t = new Thread(BatteryLevelThread);
            t.Start();
        }
        private void BatteryLevelThread()
        {
            while (batrun == true)
            {
                IntentFilter filter = new IntentFilter(Intent.ActionBatteryChanged);
                Intent battery = RegisterReceiver(null, filter);

                int chargestate = battery.GetIntExtra(BatteryManager.ExtraStatus, -1);
                switch (chargestate)
                {
                    case 2:
                        ischarging = true;
                        break;
                    case 5:
                        ischarging = false;
                        break;
                    default:
                        ischarging = false;
                        break;
                }

                int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
                int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);

                int BPercetage = (int)System.Math.Floor(level * 100D / scale);
                batterylevel = BPercetage;
                RunOnUiThread(UpdateBatteryLevel);
                Thread.Sleep(5000);
            }
        }
        private void UpdateBatteryLevel()
        {
            if (ischarging == true)
            {
                imgBat.SetImageResource(Resource.Drawable.baterychargelevel);
                ischarging = false;
            }
            else
            {
                imgBat.SetImageResource(Resource.Drawable.baterylevel);
            }
            imgBat.Visibility = ViewStates.Visible;
            imgBat.SetImageLevel((batterylevel));
        }
        #endregion

        #region WiFi

        private void WifiSignal()
        {
            run = true;
            Thread t = new Thread(WifiSignalThread);
            t.Start();
        }

        private void WifiSignalThread()
        {
            while (run == true)
            {
                WifiManager testwifi = WifiManager.FromContext(this);
                signalstrength = testwifi.ConnectionInfo.Rssi;
                RunOnUiThread(UpdateWifiSignal);
                Thread.Sleep(5000);
            }
        }

        private void UpdateWifiSignal()
        {
            if (signalstrength < 0)
            {
                imgWifi.Visibility = ViewStates.Visible;
                imgWifi.SetImageLevel((signalstrength * -1));
            }
            else
            {
                imgWifi.Visibility = ViewStates.Invisible;
            }

        }

        #endregion

        #endregion
    }
}