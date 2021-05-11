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
using RTIS_Vulcan_MBL.Activities.F_Fresh_Slurry.Manufacture;

namespace RTIS_Vulcan_MBL.Activities
{
    [Activity(Label = "F_Scan_Trolley")]
    public class F_Scan_Trolley : Activity, View.IOnTouchListener
    {
        public string tankCode = string.Empty;
        public string currentItem = string.Empty;
        public string currentLot = string.Empty;
        public string currentWetWeight = string.Empty;

        TextView lblHeader1;
        TextView lblHeader2;
        EditText txtTrolley;
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
            lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
            lblHeader1.Text = "Fresh Slurry Manufacture";

            lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
            lblHeader2.Text = "Scan Trolley";

            txtTrolley = FindViewById<EditText>(Resource.Id.txtTank);
            txtTrolley.KeyPress += TxtTrolley_KeyPress;
            //txtTrolley.Text = "TRO_106$VSP-746";

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.SetOnTouchListener(this);
            btnBack.Click += BtnBack_Click;
        }

        private void TxtTrolley_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    if (txtTrolley.Text != string.Empty)
                    {
                        if (txtTrolley.Text.Substring(0, 4) == "TRO_" && txtTrolley.Text.Contains("$"))
                        {
                            string barcode = txtTrolley.Text;
                            barcode = barcode.Substring(4, barcode.Length - 4);
                            string trolCode = barcode.Split('$')[0];
                            GlobalVar.FSTrolleyCode = barcode.Split('$')[0];
                            GlobalVar.FSItemCode = barcode.Split('$')[1];

                            GlobalVar.pgsBusy = new ProgressDialog(this);
                            GlobalVar.pgsBusy.Indeterminate = true;
                            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                            GlobalVar.pgsBusy.SetMessage("Getting Item Information...");
                            GlobalVar.pgsBusy.SetCancelable(false);
                            GlobalVar.pgsBusy.Show();
                            Thread t = new Thread(getItemDesc);
                            t.Start();
                        }
                        else
                        {
                            ErrorText = "Please scan a valid trolley barcode!";
                            RunOnUiThread(ShowError);
                        }
                    }
                    else
                    {
                        ErrorText = "Please scan the trolley barcode!";
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
                ErrorText = "Exception F001: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void getItemDesc()
        {
            try
            {
                string itemInfo = Client.CheckFreshSLurryInUse(GlobalVar.FSTrolleyCode + "|" + GlobalVar.FSItemCode);
                if (itemInfo != string.Empty)
                {
                    switch (itemInfo.Split('*')[0])
                    {
                        case "1":
                            try
                            {
                                if (GlobalVar.pgsBusy.IsShowing == true)
                                {
                                    GlobalVar.pgsBusy.Dismiss();
                                }
                            }
                            catch (Exception)
                            {

                            }
                            itemInfo = itemInfo.Remove(0, 2);
                            GlobalVar.FSItemDesc = itemInfo.Split("|")[0];
                            GlobalVar.FSReqRaw = Convert.ToBoolean(itemInfo.Split("|")[1]);
                            RunOnUiThread(showNextFreshSlurry);
                            break;
                        case "0":
                            itemInfo = itemInfo.Remove(0, 2);
                            tankCode = itemInfo.Split('|')[0];
                            currentItem = itemInfo.Split('|')[1];
                            currentLot = itemInfo.Split('|')[2];
                            currentWetWeight = itemInfo.Split('|')[3];
                            RunOnUiThread(askToInvalidate);
                            break;
                        case "-1":
                            ErrorText = "Exception H711: " + System.Environment.NewLine + itemInfo.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception H712: " + System.Environment.NewLine + "Unexpected error while storing data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception H712: " + System.Environment.NewLine + "Unexpected error while storing data";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H713: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void askToInvalidate()
        {
            try
            {
                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                Confirm.SetTitle("The following incomplete slurry was found for this trolley");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage("Trolley Code: " + tankCode + System.Environment.NewLine + "Item: " + currentItem + System.Environment.NewLine + "Lot: " + currentLot + System.Environment.NewLine + "Wet Weight: " + currentWetWeight + System.Environment.NewLine + "Would you like to close this slurry?");
                Confirm.SetButton("YES", (s, ev) =>
                {
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Closing slurry...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(inValidateSlurry);
                    t.Start();
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    ScanEngineOn();
                    StartActivity(typeof(F_Scan_Trolley));
                    Finish();
                });
                Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception F001: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void showNextFreshSlurry()
        {
            try
            {
                if (GlobalVar.FSReqRaw)
                {
                    StartActivity(typeof(F_ScanPowder));
                    Finish();
                }
                else
                {
                    StartActivity(typeof(F_Enter_Lot_Number));
                    Finish();
                }                
            }
            catch (Exception ex)
            {
                ErrorText = "Exception F001: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void inValidateSlurry()
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

                }
                string slurryClosed = Client.InvalidateSlurry(tankCode + "|" + currentItem + "|" + currentLot + "|" + GlobalVar.UserName);
                if (slurryClosed != string.Empty)
                {
                    switch (slurryClosed.Split('*')[0])
                    {
                        case "1":
                            try
                            {
                                if (GlobalVar.pgsBusy.IsShowing == true)
                                {
                                    GlobalVar.pgsBusy.Dismiss();
                                }
                            }
                            catch (Exception)
                            {

                            }
                            RunOnUiThread(restartActivity);
                            break;
                        case "0":
                            ErrorText = "Exception H711: " + System.Environment.NewLine + slurryClosed.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception H711: " + System.Environment.NewLine + slurryClosed.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception H712: " + System.Environment.NewLine + "Unexpected error while storing data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception H712: " + System.Environment.NewLine + "Unexpected error while storing data";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception F001: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void restartActivity()
        {
            try
            {
                StartActivity(typeof(F_Scan_Trolley));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception F001: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(B_Sub_Menu));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception F001: " + System.Environment.NewLine + ex.ToString();
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