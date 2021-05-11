using System;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Net.Wifi;
using Android.Preferences;
using Android.Media;
using RTIS_Vulcan_MBL.Activities;
using RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry;
using RTIS_Vulcan_MBL.Activities.P_Disatch;
using RTIS_Vulcan_MBL.Activities.J_Stock_Take;
using RTIS_Vulcan_MBL.Activities.Q_Palletizing;

namespace RTIS_Vulcan_MBL
{
    [Activity(Label = "RTIS_Vulcan_MBL", Icon = "@drawable/icon")]
    //Intent to allow you to make the application the Home Screen on the device
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { Intent.CategoryHome, Intent.CategoryDefault })]
    //Intent for installation with Evolution Module
    //[IntentFilter(new[] { Intent.ActionRun }, Categories = new string[] { Intent.ActionRun, Intent.CategoryDefault })]
    public class MainActivity : Activity, View.IOnTouchListener
    {

        Button btnReceive;

        Button btnPowderPrep;
        Button btnFreshSlurry;
        Button btnMixedSlurry;
        Button btnToProd;
        Button btnZectTransfer;
        Button btnAWTransfer;
        Button btnCanning;
        Button btnDispatch;
        Button btnFGReceiving;
        private Button btnPalletizing;

        Button btnStockTake;
        Button btnCYCCount;

        Button btnLogout;
        Button btnSettings;

        #region WiFi / Battery / OnError / Version

        #region Error

        bool threadrunning;
        string ErrorText;
        TextView lblErrorMsg;
        Button btnErrorOk;
        MediaPlayer _player;

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

        public System.Timers.Timer Logintimer = new System.Timers.Timer();

        protected override void OnCreate(Bundle bundle)
        {            
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.B_Main);

            #region Buttons Declaration

            btnReceive = FindViewById<Button>(Resource.Id.btnReceive);
            btnReceive.SetOnTouchListener(this);
            btnReceive.Click += BtnReceive_Click;

            btnPowderPrep = FindViewById<Button>(Resource.Id.btnPowderPrep);
            btnPowderPrep.SetOnTouchListener(this);
            btnPowderPrep.Click += BtnPowderPrep_Click;

            btnFreshSlurry = FindViewById<Button>(Resource.Id.btnFreshSlurry);
            btnFreshSlurry.SetOnTouchListener(this);
            btnFreshSlurry.Click += BtnFreshSlurry_Click;

            btnMixedSlurry = FindViewById<Button>(Resource.Id.btnMixedSlurry);
            btnMixedSlurry.SetOnTouchListener(this);
            btnMixedSlurry.Click += BtnMixedSlurry_Click;

            btnToProd = FindViewById<Button>(Resource.Id.btnToProd);
            btnToProd.SetOnTouchListener(this);
            btnToProd.Click += BtnToProd_Click;

            btnZectTransfer = FindViewById<Button>(Resource.Id.btnZectTransfer);
            btnZectTransfer.SetOnTouchListener(this);
            btnZectTransfer.Click += BtnZectTransfer_Click;

            btnAWTransfer = FindViewById<Button>(Resource.Id.btnAWTransfer);
            btnAWTransfer.SetOnTouchListener(this);
            btnAWTransfer.Click += BtnAWTransfer_Click;

            btnCanning = FindViewById<Button>(Resource.Id.btnCanning);
            btnCanning.SetOnTouchListener(this);
            btnCanning.Click += BtnCanning_Click;

            btnDispatch = FindViewById<Button>(Resource.Id.btnDispatch);
            btnDispatch.SetOnTouchListener(this);
            btnDispatch.Click += BtnDispatch_Click;

            btnFGReceiving = FindViewById<Button>(Resource.Id.btnFGReceiving);
            btnFGReceiving.SetOnTouchListener(this);
            btnFGReceiving.Click += BtnFGReceiving_Click;

            //btnPalletizing
            btnPalletizing = FindViewById<Button>(Resource.Id.btnPalletizing);
            btnPalletizing.SetOnTouchListener(this);
            btnPalletizing.Click += BtnPalletizing_Click;

            btnStockTake = FindViewById<Button>(Resource.Id.btnStockTake);
            btnStockTake.SetOnTouchListener(this);
            btnStockTake.Click += BtnStockTake_Click;
            btnCYCCount = FindViewById<Button>(Resource.Id.btnCYCCount);
            btnCYCCount.SetOnTouchListener(this);
            btnCYCCount.Click += BtnCYCCount_Click;         

            btnSettings = FindViewById<Button>(Resource.Id.btnSettings);
            btnSettings.SetOnTouchListener(this);
            btnSettings.Click += BtnSettings_Click;

            btnLogout = FindViewById<Button>(Resource.Id.btnLogout);
            btnLogout.Click += BtnLogout_Click; ;
            btnLogout.SetOnTouchListener(this);
            btnLogout.Enabled = false;
            btnLogout.SetBackgroundResource(Resource.Drawable.buttonDisabled);

            #endregion           

            #region Reltech User

            if (GlobalVar.UserPin == "62017")
            {
                //btnDispatch.Enabled = false;
                //btnDispatch.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                //btnReceive.Enabled = false;
                //btnReceive.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                //btnStockTake.Enabled = false;
                //btnStockTake.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                //btnCycleCount.Enabled = false;
                //btnCycleCount.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                //btnWarehouse.Enabled = false;
                //btnWarehouse.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                //btnXperdyte.Enabled = false;
                //btnXperdyte.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnLogout.Enabled = true;
                btnLogout.SetBackgroundResource(Resource.Drawable.roundedbutton);
            }

            #endregion            
            
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

            GlobalVar.Scanning = false;
            RetrieveSettings();

            #region User Tracking

            if (GlobalVar.MUserTracking == true)
            {
                GlobalVar.LoginTime = 0;
                btnLogout.Enabled = true;
                btnLogout.SetBackgroundResource(Resource.Drawable.roundedbutton);
                Logout();
            }

            #endregion

            ScanEngineOff();
        }

        #region Buttons
        private void BtnReceive_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            batrun = false;
            GlobalVar.LoginTime = 200;
            StartActivity(typeof(ScanDocument));
            Finish();
        }
        private void BtnPowderPrep_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            GlobalVar.ProcessName = "PPtFS";
            StartActivity(typeof(B_Sub_Menu));
            Finish();
        }
        private void BtnFreshSlurry_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            GlobalVar.ProcessName = "FStMS";
            StartActivity(typeof(B_Sub_Menu));
            Finish();
        }
        private void BtnMixedSlurry_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            GlobalVar.ProcessName = "MStZECT";
            StartActivity(typeof(G_Menu));
            Finish();
        }
        private void BtnToProd_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            //GlobalVar.ProcessName = "MStZECT";
            StartActivity(typeof(H_Select_Whse_ToProd));
            Finish();
        }
        private void BtnZectTransfer_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;

            string zectWhse = GlobalVar.ZectWhse;
            if (GlobalVar.ZectWhse.Contains("1"))
            {
                GlobalVar.ProcessName = "ZECT1";
            }
            else
            {
                GlobalVar.ProcessName = "ZECT2";
            }
           
            StartActivity(typeof(B_Sub_Menu));
            Finish();
        }
        private void BtnAWTransfer_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            GlobalVar.ProcessName = "AW";
            StartActivity(typeof(B_Sub_Menu));
            Finish();
        }
        private void BtnCanning_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            GlobalVar.ProcessName = "Canning";
            StartActivity(typeof(B_Sub_Menu));
            Finish();
        }
        private void BtnDispatch_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            StartActivity(typeof(P_ScanSO));
            Finish();
        }
        private void BtnFGReceiving_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            //GlobalVar.ProcessName = "MStZECT";
            //StartActivity(typeof(H_Scan_RT2D_Barcode));
            //Finish();
        }
        private void BtnStockTake_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            StartActivity(typeof(J_SelectStockTake));
            Finish();
        }
        private void BtnCYCCount_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            //StartActivity(typeof(SettingsPassword));
            //Finish();
        }      

        private void BtnPalletizing_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            GlobalVar.ProcessName = "Palletizing";
            StartActivity(typeof(B_Sub_Menu));
            Finish();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            Logintimer.Stop();
            Logintimer.Enabled = false;
            GlobalVar.LoginTime = 200;
            StartActivity(typeof(SettingsPassword));
            Finish();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder build = new AlertDialog.Builder(this);
            AlertDialog Confirm = build.Create();
            Confirm.SetTitle("RTIS Herma");
            Confirm.SetIcon(Resource.Drawable.RTLogoTrans);
            Confirm.SetMessage("Are you sure you want to logout?");
            Confirm.SetButton("YES", (s, ev) =>
            {
                GlobalVar.UserPin = "";
                GlobalVar.UserName = "";
                GlobalVar.LoginTime = 300;
                StartActivity(typeof(Login));
                Finish();
            });
            Confirm.SetButton2("NO", (s, ev) =>
            {
                //Do Nothing
            });
            Confirm.Show();
        }
        #endregion
        private void Logout()
        {
            Logintimer.Interval = 1000;
            Logintimer.Elapsed += OnTimedEvent;
            Logintimer.Enabled = true;
        }
        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            GlobalVar.LoginTime = GlobalVar.LoginTime + 1;
            if (GlobalVar.LoginTime == 120)
            {
                GlobalVar.LoginTime = 0;
                Logintimer.Enabled = false;
                GlobalVar.UserName = "";
                GlobalVar.UserPin = "";
                StartActivity(typeof(Login));
                Finish();
            }
        }
        protected void RetrieveSettings()
        {
            GlobalVar.SettingsPassword = "1234";
            GlobalVar.ItemCode = "";

            var c = (Context)this;
            ISharedPreferences prefs = c.GetSharedPreferences("RTIS_Vulcan_MBL", FileCreationMode.Private);

            #region Modules

            GlobalVar.MReceive = prefs.GetBoolean("MReceive", true);

            GlobalVar.MPowderPrep = prefs.GetBoolean("MPowderPrep", true);
            GlobalVar.MFreshSlurry = prefs.GetBoolean("MFreshSlurry", true);
            GlobalVar.MMixedSlurry = prefs.GetBoolean("MMixedSlurry", true);
            GlobalVar.MToProd = prefs.GetBoolean("MToProd", true);

            GlobalVar.MZectTrans = prefs.GetBoolean("MZectTransfer", true);
            GlobalVar.MAWTrans = prefs.GetBoolean("MAWTransfer", true);
            GlobalVar.MCanTrans = prefs.GetBoolean("MCanTransfer", true);
            GlobalVar.MDispatch = prefs.GetBoolean("MDispatch", true);
            GlobalVar.MFGReceiving = prefs.GetBoolean("MFGReceiving", true);

            GlobalVar.MStockTake = prefs.GetBoolean("MStockTake", true);
            GlobalVar.MCYCCount = prefs.GetBoolean("MCYCCount", true);

            GlobalVar.MUserTracking = prefs.GetBoolean("MUserTrack", true);
            GlobalVar.MPalletizing = prefs.GetBoolean("MPallet", true);

            if (GlobalVar.MReceive == false)
            {
                btnReceive.Enabled = false;
                btnReceive.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnReceive.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }

            if (GlobalVar.MPowderPrep == false)
            {
                btnPowderPrep.Enabled = false;
                btnPowderPrep.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnPowderPrep.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }
            if (GlobalVar.MFreshSlurry == false)
            {
                btnFreshSlurry.Enabled = false;
                btnFreshSlurry.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnFreshSlurry.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }
            if (GlobalVar.MMixedSlurry == false)
            {
                btnMixedSlurry.Enabled = false;
                btnMixedSlurry.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnMixedSlurry.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }
            if (GlobalVar.MToProd == false)
            {
                btnToProd.Enabled = false;
                btnToProd.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnToProd.Visibility = ViewStates.Gone;
            }
            if (GlobalVar.MZectTrans == false)
            {
                btnZectTransfer.Enabled = false;
                btnZectTransfer.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnZectTransfer.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }

            if (GlobalVar.MAWTrans == false)
            {
                btnAWTransfer.Enabled = false;
                btnAWTransfer.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnAWTransfer.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }
            if (GlobalVar.MCanTrans == false)
            {
                btnCanning.Enabled = false;
                btnCanning.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnCanning.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }
            if (GlobalVar.MDispatch == false)
            {
                btnDispatch.Enabled = false;
                btnDispatch.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnDispatch.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }
            if (GlobalVar.MFGReceiving == false)
            {
                btnFGReceiving.Enabled = false;
                btnFGReceiving.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnFGReceiving.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }

            if (GlobalVar.MStockTake == false)
            {
                btnStockTake.Enabled = false;
                btnStockTake.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnStockTake.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }
            if (GlobalVar.MCYCCount == false)
            {
                btnCYCCount.Enabled = false;
                btnCYCCount.SetBackgroundResource(Resource.Drawable.buttonDisabled);
                btnCYCCount.Visibility = ViewStates.Gone;//ViewStates.Invisible;
            }
            if (GlobalVar.MUserTracking == false)
            {
                GlobalVar.LoginTime = 200;
                btnLogout.Enabled = false;
                btnLogout.SetBackgroundResource(Resource.Drawable.buttonDisabled);
            }
            if (GlobalVar.MPalletizing == false)
            {
                GlobalVar.LoginTime = 200;
                btnPalletizing.Enabled = false;
                btnPalletizing.SetBackgroundResource(Resource.Drawable.buttonDisabled);
            }

            #endregion

            #region Server IP / Port

            var IP = prefs.GetString("ServerIP", null);
            if (IP != null)
            {
                GlobalVar.ServerIP = IP;
            }
            else
            {
                GlobalVar.ServerIP = "192.168.1.13";
            }

            var Port = prefs.GetInt("ServerPort", 32017);
            if (Port != 32017)
            {
                GlobalVar.ServerPort = Port;
            }
            else
            {
                GlobalVar.ServerPort = 32017;
            }

            #endregion

            #region Branch

            var Branch = prefs.GetString("SBranch", null);
            if (Branch != null)
            {
                GlobalVar.Branch = Branch;
            }
            else
            {
                GlobalVar.Branch = "N/A";
            }

            #endregion

            #region Scanning

            bool Scanning = prefs.GetBoolean("ScanQty", true);
            if (Scanning == false)
            {
                GlobalVar.Scanning = false;
            }
            else
            {
                GlobalVar.Scanning = true;
            }

            #endregion
            
            #region Scanner ID

            var ScannerID = prefs.GetString("ScannerID", null);
            if (ScannerID != null)
            {
                GlobalVar.ScannerID = ScannerID;
            }
            else
            {
                GlobalVar.ScannerID = "SCN001";
            }

            if (GlobalVar.UserPin == null)
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName + System.Environment.NewLine + GlobalVar.ScannerID;
            }
            else
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName + System.Environment.NewLine + GlobalVar.UserName;
            }

            #endregion

            #region User Tracking

            if (GlobalVar.MUserTracking == true && GlobalVar.UserPin == null)
            {
                GlobalVar.LoginTime = 0;
                Logintimer.Enabled = false;
                GlobalVar.UserName = "";
                GlobalVar.UserPin = "";
                StartActivity(typeof(Login));
                Finish();
            }

            #endregion

            #region Warehouses
            GlobalVar.PPtFSWhse = prefs.GetString("sPPtFSWhse", "WIP-PP");
            GlobalVar.FStMSWhse = prefs.GetString("sFStMSWhse", "WIP-FS");
            GlobalVar.MStZectWhse = prefs.GetString("sMStZectWhse", "WIP-MS");
            GlobalVar.ToProdWhse = prefs.GetString("sToProdWhse", "Mstr");
            GlobalVar.ZectWhse = prefs.GetString("sZectWhse", "WIP-Z2");
            GlobalVar.AWWhse = prefs.GetString("sAWWhse", "WIP-AW");
            GlobalVar.CanWhse = prefs.GetString("sCanWhse", "WIP-CAN");
            #endregion

            #region Stock Take
            GlobalVar.singleST = prefs.GetBoolean("singleST", true);
            GlobalVar.doubleST = prefs.GetBoolean("doubleST", false);
            GlobalVar.recountST = prefs.GetBoolean("recountST", false);
            #endregion

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
            try
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
            catch (Exception)
            {

            }

        }
        private void UpdateBatteryLevel()
        {
            try
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
            catch (Exception)
            {

            }
            
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

