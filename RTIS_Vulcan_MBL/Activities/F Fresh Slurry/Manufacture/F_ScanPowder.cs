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

namespace RTIS_Vulcan_MBL.Activities.F_Fresh_Slurry.Manufacture
{
    [Activity(Label = "F_ScanPowder")]
    public class F_ScanPowder : Activity, View.IOnTouchListener
    {
        TextView lblHeader1;
        TextView lblHeader2;
        EditText txtBCD;
        Button btnBack;

        public string itemCode = string.Empty;
        public string lotNumber = string.Empty;

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
            SetContentView(Resource.Layout.E_Scan_Slurry_BCD);
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
                lblHeader1.Text = "FRESH SLURRY MANUFACTURE";
                lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                lblHeader2.Text = "SCAN POWDER B.C.D. FOR CAT SCAN";
                txtBCD = FindViewById<EditText>(Resource.Id.txtBCD);
                txtBCD.KeyPress += TxtBCD_KeyPress;
                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;
                //txtBCD.Text = "103$1115282-284";
            }
            catch (Exception ex)
            {
                ErrorText = "Exception E001: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void TxtBCD_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    if (txtBCD.Text != string.Empty)
                    {
                        if (txtBCD.Text.Contains('$'))
                        {
                            itemCode = "TPP-" + txtBCD.Text.Split('$')[0];
                            lotNumber = txtBCD.Text.Split('$')[1];
                            if (txtBCD.Text.Split('$')[1].Contains('-'))
                            {
                                lotNumber = txtBCD.Text.Split('$')[1];
                            }
                            else
                            {
                                string testLotP1 = txtBCD.Text.Split('$')[1].Substring(0, 6);
                                string testLotP2 = txtBCD.Text.Split('$')[1].Substring(7);
                                lotNumber = testLotP1 + "-" + testLotP2;
                            }
                            GlobalVar.pgsBusy = new ProgressDialog(this);
                            GlobalVar.pgsBusy.Indeterminate = true;
                            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                            GlobalVar.pgsBusy.SetMessage("Validating Item...");
                            GlobalVar.pgsBusy.SetCancelable(false);
                            GlobalVar.pgsBusy.Show();
                            Thread t = new Thread(validateItem);
                            t.Start();
                        }
                        else
                        {
                            ErrorText = "PLEASE SCAN A VALID POWDER BARCODE!";
                            RunOnUiThread(ShowError);
                        }
                    }
                    else
                    {
                        ErrorText = "PLEASE SCAN A BARCODE!";
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
                ErrorText = "Exception E002: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void validateItem()
        {
            try
            {
                string validated = Client.ValidateFSRaw(GlobalVar.FSItemCode + "|" + itemCode);
                if (validated != string.Empty)
                {
                    switch (validated.Split('*')[0])
                    {
                        case "1":
                            validated = validated.Remove(0, 2);
                            GlobalVar.FSRDesc = validated;
                            GlobalVar.FSRCode = itemCode;
                            GlobalVar.FSRLot = lotNumber;
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            ErrorText = "Exception E003: " + System.Environment.NewLine + validated.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception E004: " + System.Environment.NewLine + validated.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception E005: " + System.Environment.NewLine + "Unexpected error while validating item";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception E006: " + System.Environment.NewLine + "Unexpected error while validating item";
                    RunOnUiThread(ShowError);
                }

            }
            catch (Exception ex)
            {
                ErrorText = "Exception E007: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void showNext()
        {
            StartActivity(typeof(F_ScanPowderQty));
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
                ErrorText = "Exception E008: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        #region WiFi / OnTouch / OnError / ScanEngine

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
            SetContentView(Resource.Layout.E_Scan_Slurry_BCD);
            layoutStichUp();
            ScanEngineOn();
            #region Wifi / Battery / Error / Version

            #region Version Control

            lblVersion = FindViewById<TextView>(Resource.Id.lblVersion);
            if (GlobalVar.UserPin == null)
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;
            }
            else
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;
            }

            #endregion

            _player = MediaPlayer.Create(this, Resource.Drawable.beeperror);
            imgBat = FindViewById<ImageView>(Resource.Id.imgBat);
            imgBat.Click += ImgBat_Click;
            imgWifi = FindViewById<ImageView>(Resource.Id.imgWifi);
            BatteryLevel();
            WifiSignal();

            #endregion
            txtBCD.RequestFocus();
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