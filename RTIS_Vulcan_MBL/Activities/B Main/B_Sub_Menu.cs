using System;
using System.Threading;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Net.Wifi;
using Android.Preferences;
using Android.Media;
using RTIS_Vulcan_MBL.Activities.L_Receiving_Transfer;
using RTIS_Vulcan_MBL.Activities.H_Transfers.Mixed_Slurry;
using RTIS_Vulcan_MBL.Activities.H_Transfers;
using RTIS_Vulcan_MBL.Activities.Q_Palletizing;

namespace RTIS_Vulcan_MBL.Activities
{
    [Activity(Label = "B_Sub_Menu")]
    public class B_Sub_Menu : Activity, View.IOnTouchListener
    {
        Button btnRec;
        Button btnManufacture;
        Button btnAdditions;
        Button btnSolidity;
        Button btnTransfer;
        private Button btnCreate;
        private Button btnBreakdown;
        Button btnBack;

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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.B_Sub_Menu);
            layoutStichUp();
            ScanEngineOff();
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
        }

        public void layoutStichUp()
        {
            try
            {
                btnRec = FindViewById<Button>(Resource.Id.btnRec);
                btnRec.SetOnTouchListener(this);
                btnRec.Click += BtnRec_Click;

                btnManufacture = FindViewById<Button>(Resource.Id.btnManufacture);
                btnManufacture.SetOnTouchListener(this);
                btnManufacture.Click += BtnManufacture_Click;

                btnAdditions = FindViewById<Button>(Resource.Id.btnAdditions);
                btnAdditions.SetOnTouchListener(this);
                btnAdditions.Click += BtnAdditions_Click;

                btnSolidity = FindViewById<Button>(Resource.Id.btnSolidity);
                btnSolidity.SetOnTouchListener(this);
                btnSolidity.Click += BtnSolidity_Click;

                btnTransfer = FindViewById<Button>(Resource.Id.btnTransfer);
                btnTransfer.SetOnTouchListener(this);
                btnTransfer.Click += BtnTransfer_Click;

                btnCreate = FindViewById<Button>(Resource.Id.btnCreate);
                btnCreate.SetOnTouchListener(this);
                btnCreate.Click += BtnCreate_Click;

                btnBreakdown = FindViewById<Button>(Resource.Id.btnBreakdown);
                btnBreakdown.SetOnTouchListener(this);
                btnBreakdown.Click += BtnBreakdown_Click;

                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;

                switch(GlobalVar.ProcessName)
                {
                    case "PPtFS":
                        btnSolidity.Visibility = ViewStates.Gone;
                        btnAdditions.Visibility = ViewStates.Gone;
                        btnCreate.Visibility = ViewStates.Gone;
                        btnBreakdown.Visibility = ViewStates.Gone;
                        break;
                    case "FStMS":
                        btnAdditions.Visibility = ViewStates.Gone;
                        btnCreate.Visibility = ViewStates.Gone;
                        btnBreakdown.Visibility = ViewStates.Gone;
                        break;
                    case "MStZECT":
                        btnCreate.Visibility = ViewStates.Gone;
                        btnBreakdown.Visibility = ViewStates.Gone;
                        break;
                    case "ZECT1":
                        btnSolidity.Visibility = ViewStates.Gone;    
                        btnAdditions.Visibility = ViewStates.Gone;
                        btnCreate.Visibility = ViewStates.Gone;
                        btnBreakdown.Visibility = ViewStates.Gone;
                        btnManufacture.Text = "ZECT Input";
                        break;
                    case "ZECT2":
                        btnSolidity.Visibility = ViewStates.Gone;
                        btnAdditions.Visibility = ViewStates.Gone;
                        btnCreate.Visibility = ViewStates.Gone;
                        btnBreakdown.Visibility = ViewStates.Gone;
                        btnManufacture.Text = "ZECT Input";
                        break;
                    case "AW":
                        btnSolidity.Visibility = ViewStates.Gone;
                        btnAdditions.Visibility = ViewStates.Gone;
                        btnCreate.Visibility = ViewStates.Gone;
                        btnBreakdown.Visibility = ViewStates.Gone;
                        btnManufacture.Text = "A&W Input";
                        break;
                    case "Canning":
                        btnSolidity.Visibility = ViewStates.Gone;
                        btnAdditions.Visibility = ViewStates.Gone;
                        btnCreate.Visibility = ViewStates.Gone;
                        btnBreakdown.Visibility = ViewStates.Gone;
                        btnManufacture.Visibility = ViewStates.Gone;
                        break;
                    case "Palletizing":
                        btnRec.Visibility = ViewStates.Gone;
                        btnManufacture.Visibility = ViewStates.Gone;
                        btnAdditions.Visibility = ViewStates.Gone;
                        btnSolidity.Visibility = ViewStates.Gone;
                        btnTransfer.Visibility = ViewStates.Gone;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception B101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnRec_Click(object sender, EventArgs e)
        {
            try
            {
                switch (GlobalVar.ProcessName)
                {
                    case "PPtFS":
                        StartActivity(typeof(SelectWhseFrom));
                        Finish();
                        break;
                    case "FStMS":
                        GlobalVar.RecTransScannedFS = new System.Collections.Generic.List<string>();
                        StartActivity(typeof(SelectWhseFrom));
                        Finish();
                        break;
                    case "MStZECT":
                        StartActivity(typeof(SelectWhseFrom));
                        Finish();
                        break;
                    case "ZECT1":
                        StartActivity(typeof(SelectWhseFrom));
                        Finish();
                        break;
                    case "ZECT2":
                        StartActivity(typeof(SelectWhseFrom));
                        Finish();
                        break;
                    case "AW":
                        StartActivity(typeof(SelectWhseFrom));
                        Finish();
                        break;
                    case "Canning":
                        StartActivity(typeof(SelectWhseFrom));
                        Finish();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception B102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnManufacture_Click(object sender, EventArgs e)
        {
            try
            {
                switch (GlobalVar.ProcessName)
                {
                    case "PPtFS":
                        StartActivity(typeof(E_ScanBCD));
                        Finish();
                        break;
                    case "FStMS":
                        StartActivity(typeof(F_Scan_Trolley));
                        Finish();
                        break;
                    case "MStZECT":
                        //StartActivity(typeof(G_Scan_Slurry_Tank));
                        //Finish();
                        break;
                    case "ZECT1":
                        StartActivity(typeof(ScanZectJob));
                        Finish();
                        break;
                    case "ZECT2":
                        StartActivity(typeof(ScanZectJob));
                        Finish();
                        break;
                    case "AW":
                        StartActivity(typeof(N_ScanJobTag));
                        Finish();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception B102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnAdditions_Click(object sender, EventArgs e)
        {
            try
            {
                //StartActivity(typeof(GA_Scan_Slurry_Tank));
                //Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception B103: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnSolidity_Click(object sender, EventArgs e)
        {
            try
            {
                switch (GlobalVar.ProcessName)
                {
                    case "FStMS":
                        StartActivity(typeof(FS_Scan_Trolley));
                        Finish();
                        break;
                    case "MStZECT":
                        //StartActivity(typeof(GS_Scan_Slurry_Tank));
                        //Finish();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception B104: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                switch (GlobalVar.ProcessName)
                {
                    case "PPtFS":
                        StartActivity(typeof(H_Scan_Prep_Sheet_PP));
                        Finish();
                        break;
                    case "FStMS":
                        StartActivity(typeof(H_Scan_Trolley));
                        Finish();
                        break;
                    case "MStZECT":
                        StartActivity(typeof(H_ScanTank_MS));
                        Finish();
                        break;
                    case "ZECT1":
                        StartActivity(typeof(H_Scan_RT2D_Barcode));
                        Finish();
                        break;
                    case "ZECT2":
                        StartActivity(typeof(H_Scan_RT2D_Barcode));
                        Finish();
                        break;
                    case "AW":
                        StartActivity(typeof(H_Scan_RT2D_AW));
                        Finish();
                        break;
                    case "Canning":
                        StartActivity(typeof(H_Scan_RT2D_Canning));
                        Finish();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception B105: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(Q_ScanItems));
            Finish();
        }
        private void BtnBreakdown_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(Q_Scan_Pallet));
            Finish();
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
                ErrorText = "Exception B106: " + System.Environment.NewLine + ex.ToString();
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
            SetContentView(Resource.Layout.B_Sub_Menu);
            layoutStichUp();
            ScanEngineOn();
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