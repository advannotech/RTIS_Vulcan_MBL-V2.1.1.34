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

namespace RTIS_Vulcan_MBL
{
    [Activity(Label = "Login", MainLauncher = true)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { Intent.CategoryHome, Intent.CategoryDefault })]

    public class Login : Activity, View.IOnTouchListener
    {
        TextView txtLogin;

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
            SetContentView(Resource.Layout.A_Login);

            txtLogin = FindViewById<TextView>(Resource.Id.txtLogin);
            txtLogin.KeyPress += TxtLogin_KeyPress;

            RetrieveSettings();
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

        private void TxtLogin_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    if (txtLogin.Text != "")
                    {
                        ScanEngineOff();
                        if (txtLogin.Text == "62017")
                        {
                            GlobalVar.UserPin = txtLogin.Text;
                            GlobalVar.UserName = "RT ADMIN";
                            txtLogin.Text = "";
                            StartActivity(typeof(MainActivity));
                            Finish();
                        }
                        else
                        {
                            GlobalVar.UserPin = txtLogin.Text;
                            GlobalVar.pgsBusy = new ProgressDialog(this);
                            GlobalVar.pgsBusy.Indeterminate = true;
                            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                            GlobalVar.pgsBusy.SetMessage("Checking User Credentials...");
                            GlobalVar.pgsBusy.SetCancelable(false);
                            GlobalVar.pgsBusy.Show();
                            threadrunning = true;
                            Thread t = new Thread(GetDetails);
                            t.Start();
                        }
                    }
                    else
                    {
                        ErrorText = "PLEASE SCAN A VALID USER CARD.";
                        RunOnUiThread(ShowError);
                    }
                }
                else
                {
                    e.Handled = false;
                }
            }
            catch (Exception)
            {
                txtLogin.Text = "";
                ErrorText = "UNABLE TO LOG USER IN. PLEASE CHECK SERVER CONNECTION.";
                RunOnUiThread(ShowError);
            }
        }

        private void GetDetails()
        {
            GlobalVar.UserName = Client.GetUserName(GlobalVar.UserPin);
            //GlobalVar.UserName = "1*TestUser Offline";

            string[] Data = GlobalVar.UserName.Split('*');
            string Code = Data[0];
            string Details = Data[1];
            if (Code != "-1")
            {
                if (Code != "0")
                {
                    GlobalVar.UserName = Details;
                    RunOnUiThread(ShowNext);
                }
                else
                {
                    ErrorText = "USER DOES NOT EXIST";
                    RunOnUiThread(ShowError);
                }
            }
            else
            {
                ErrorText = "CANNOT CONNECT TO SERVER";
                RunOnUiThread(ShowError);
            }
        }

        private void ShowNext()
        {
            if (GlobalVar.pgsBusy != null)
            {
                GlobalVar.pgsBusy.Dismiss();
            }
            txtLogin.Text = "";
            StartActivity(typeof(MainActivity));
            batrun = false;
            Finish();
        }

        protected void RetrieveSettings()
        {
            var c = (Context)this;
            ISharedPreferences prefs = c.GetSharedPreferences("RTIS_Vulcan_MBL", FileCreationMode.Private);
            var IP = prefs.GetString("ServerIP", null);
            if (IP != null)
            {
                GlobalVar.ServerIP = IP;
            }
            else
            {
                GlobalVar.ServerIP = "192.168.0.1";
            }

            var Port = prefs.GetInt("ServerPort", 62017);
            if (Port != 62017)
            {
                GlobalVar.ServerPort = Port;
            }
            else
            {
                GlobalVar.ServerPort = 62017;
            }


            GlobalVar.PPtFSWhse = prefs.GetString("sPPtFSWhse", "PPtFS");
            GlobalVar.FStMSWhse = prefs.GetString("sFStMSWhse", "FStMS");
            GlobalVar.MStZectWhse = prefs.GetString("sMStZectWhse", "MStZect");
            GlobalVar.ToProdWhse = prefs.GetString("sToProdWhse", "Mstr");
            GlobalVar.ZectWhse = prefs.GetString("sZectWhse", "Zect");
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
            txtLogin.Text = "";
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
            SetContentView(Resource.Layout.A_Login);

            txtLogin = FindViewById<TextView>(Resource.Id.txtLogin);
            txtLogin.KeyPress += TxtLogin_KeyPress;

            RetrieveSettings();
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

                throw;
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