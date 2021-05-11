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

namespace RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry
{
    [Activity(Label = "G_ScanTank_SM")]
    public class G_ScanTank_SM : Activity, View.IOnTouchListener
    {
        TextView lblHeader1;
        TextView lblHeader2;
        EditText txtTank;
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
            SetContentView(Resource.Layout.F_G_Scan_Slurry_Tank);
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
                lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                lblHeader1.Text = "Mixed Slurry Manufacture";

                lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                lblHeader2.Text = "Scan Slurry Tank";

                txtTank = FindViewById<EditText>(Resource.Id.txtTank);
                txtTank.KeyPress += TxtTank_KeyPress;
                //txtTank.Text = "BTNK_TANK002$TSP-669";
                //txtTank.Text = "TNK_TANK001$TSP-669";
                //txtTank.Text = "TNK_65$TSP-669";

                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }

        }
        private void TxtTank_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    if (txtTank.Text != string.Empty)
                    {
                        if ((txtTank.Text.Substring(0, 5) == "BTNK_" || txtTank.Text.Substring(0, 4) == "TNK_") && txtTank.Text.Contains("$"))
                        {
                            string barcode = txtTank.Text;
                            barcode = barcode.Split('_')[1];
                            string check = txtTank.Text.Split('_')[0];
                            GlobalVar.MSTankType = txtTank.Text.Split('_')[0];
                            GlobalVar.MSTankCode = barcode.Split('$')[0];
                            GlobalVar.MSItemCode = barcode.Split('$')[1];

                            GlobalVar.pgsBusy = new ProgressDialog(this);
                            GlobalVar.pgsBusy.Indeterminate = true;
                            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                            GlobalVar.pgsBusy.SetMessage("Checking Slurry Tank...");
                            GlobalVar.pgsBusy.SetCancelable(false);
                            GlobalVar.pgsBusy.Show();
                            Thread t = new Thread(checkTankInUse);
                            t.Start();
                        }
                        else
                        {
                            ErrorText = "Please scan a valid tank barcode!";
                            RunOnUiThread(ShowError);
                        }
                    }
                    else
                    {
                        ErrorText = "Please scan the tank barcode!";
                        RunOnUiThread(ShowError);
                    }
                }
                else
                {
                    e.Handled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void checkTankInUse()
        {
            try
            {
                try
                {
                    if (GlobalVar.pgsBusy.IsShowing == true)
                    {
                        GlobalVar.pgsBusy.Dismiss();
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                string tankInfo = Client.checkMixedSlurryTankInUse(GlobalVar.MSTankType + "|" + GlobalVar.MSTankCode + "|" + GlobalVar.MSItemCode);
                if (tankInfo != string.Empty)
                {
                    switch (tankInfo.Split('*')[0])
                    {
                        case "1":
                            tankInfo = tankInfo.Remove(0, 2);
                            GlobalVar.MSItemDesc = tankInfo;
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            switch (GlobalVar.MSTankType)
                            {
                                case "BTNK":
                                    RunOnUiThread(askToClose);
                                    break;
                                case "TNK":
                                    ErrorText = "Exception G104: " + System.Environment.NewLine + tankInfo.Split('*')[1];
                                    RunOnUiThread(ShowError);
                                    break;
                            }
                            break;
                        case "-1":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + tankInfo.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception G104: Unexpected result from server!" + System.Environment.NewLine + tankInfo;
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception G104: No data was returned from the server!";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G103: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void showNext()
        {
            try
            {
                StartActivity(typeof(G_EnterLot_SM));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G103: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void askToClose()
        {
            try
            {
                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                Confirm.SetTitle("Close last slurry");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage("To re-use this tank you will need to close off the last slurry used, would you like to close the last slurry now?");
                Confirm.SetButton("YES", (s, ev) =>
                {
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Closing slurry...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(closeLastSlurry);
                    t.Start();
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    ScanEngineOn();
                    StartActivity(typeof(G_ScanTank_SM));
                    Finish();
                });
                Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G103: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void closeLastSlurry()
        {
            try
            {
                try
                {
                    if (GlobalVar.pgsBusy.IsShowing == true)
                    {
                        GlobalVar.pgsBusy.Dismiss();
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                string slurriesClosed = Client.closeBufferSlurry(GlobalVar.MSTankType + "|" + GlobalVar.MSTankCode + "|" + GlobalVar.MSItemCode + "|" + GlobalVar.UserName);
                if (slurriesClosed != string.Empty)
                {
                    switch (slurriesClosed.Split('*')[0])
                    {
                        case "1":
                            slurriesClosed = slurriesClosed.Remove(0, 2);
                            GlobalVar.MSItemDesc = slurriesClosed;
                            RunOnUiThread(showNext);
                            break;
                        case "-1":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + slurriesClosed.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception G104: Unexpected result from server!" + System.Environment.NewLine + slurriesClosed;
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception G104: No data was returned from the server!";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G103: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_Menu));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G104: " + System.Environment.NewLine + ex.ToString();
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
            SetContentView(Resource.Layout.F_G_Scan_Slurry_Tank);
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