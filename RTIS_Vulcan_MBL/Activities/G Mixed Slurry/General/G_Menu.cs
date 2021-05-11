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
using RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry.Decant;
using RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry.Solidity;
using RTIS_Vulcan_MBL.Activities.H_Transfers.Mixed_Slurry;

namespace RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry
{
    [Activity(Label = "G_Menu")]
    public class G_Menu : Activity, View.IOnTouchListener
    {
        Button btnRec;
        Button btnMix;
        Button btnEnterRem;
        Button btnEnterRec;
        Button btnAddSlurry;
        Button btnDecant;
        Button btnZonen;
        Button btnSolid;
        Button btnTransfer;
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
            SetContentView(Resource.Layout.G_Sub_Menu);
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
        public void layoutStichUp()
        {
            try
            {
                btnRec = FindViewById<Button>(Resource.Id.btnRec);
                btnRec.SetOnTouchListener(this);
                btnRec.Click += BtnRec_Click;

                btnMix = FindViewById<Button>(Resource.Id.btnMix);
                btnMix.SetOnTouchListener(this);
                btnMix.Click += BtnMix_Click;
          
                btnEnterRem = FindViewById<Button>(Resource.Id.btnEnterRem);
                btnEnterRem.SetOnTouchListener(this);
                btnEnterRem.Click += BtnEnterRem_Click;

                btnEnterRec = FindViewById<Button>(Resource.Id.btnEnterRec);
                btnEnterRec.SetOnTouchListener(this);
                btnEnterRec.Click += BtnEnterRec_Click;

                btnAddSlurry = FindViewById<Button>(Resource.Id.btnAddSlurry);
                btnAddSlurry.SetOnTouchListener(this);
                btnAddSlurry.Click += BtnAddSlurry_Click;

                btnDecant = FindViewById<Button>(Resource.Id.btnDecant);
                btnDecant.SetOnTouchListener(this);
                btnDecant.Click += BtnDecant_Click;

                btnZonen = FindViewById<Button>(Resource.Id.btnZonen);
                btnZonen.SetOnTouchListener(this);
                btnZonen.Click += BtnZonen_Click;

                btnSolid = FindViewById<Button>(Resource.Id.btnSolid);
                btnSolid.SetOnTouchListener(this);
                btnSolid.Click += BtnSolid_Click;

                btnTransfer = FindViewById<Button>(Resource.Id.btnTransfer);
                btnTransfer.SetOnTouchListener(this);
                btnTransfer.Click += BtnTransfer_Click;

                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G001: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnRec_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalVar.ProcessName = "MStZECT";
                StartActivity(typeof(SelectWhseFrom));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G002: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnMix_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_ScanTank_SM));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G003: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnEnterRem_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    StartActivity(typeof(G_ScanTank_REM));
                    Finish();
                }
                catch (Exception ex)
                {
                    ErrorText = "Exception G003: " + System.Environment.NewLine + ex.ToString();
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G004: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnEnterRec_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_ScanTank_Rec));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnAddSlurry_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_ScanTank_FS));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G006: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnDecant_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_ScanTank_Dec));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G007: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnZonen_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_ScanTank_ZAC));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G008: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnSolid_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_ScanTank_Sol));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G009: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(H_ScanTank_MS));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G010: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
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
                ErrorText = "Exception G011: " + System.Environment.NewLine + ex.ToString();
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
            SetContentView(Resource.Layout.H_Scan_Slurry_Tank);
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