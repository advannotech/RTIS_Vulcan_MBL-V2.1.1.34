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

using RTIS_Vulcan_MBL.Classes;

namespace RTIS_Vulcan_MBL.Activities
{
    [Activity(Label = "H_ScanDryWeight_PP")]
    public class H_ScanDryWeight_PP : Activity, View.IOnTouchListener
    {
        public string itemDesc = string.Empty;

        TextView lblHeader2;
        EditText txtBCD;
        Button btnBack;

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
            StartActivity(typeof(H_Scan_Prep_Sheet_PP));
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
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.H_Scan_Dry_Weight);
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
                lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                lblHeader2.Text = "Scan Dry Weight BCD";
                txtBCD = FindViewById<EditText>(Resource.Id.txtBCD);
                txtBCD.KeyPress += TxtBCD_KeyPress;
                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H501: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }

        }

        private void TxtBCD_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    ScanEngineOff();
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(getItemDescription);
                    t.Start();
                }
                else
                {
                    e.Handled = false;
                }                             
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H502: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void getItemDescription()
        {
            try
            {
                string itemDescription = Client.getItemDescription(GlobalVar.PPtFSItemCode);
                if (itemDescription != string.Empty)
                {
                    switch (itemDescription.Split('*')[0])
                    {
                        case "1":
                            itemDescription = itemDescription.Remove(0, 2);
                            itemDesc = itemDescription;
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
                            RunOnUiThread(confirm);
                            break;
                        case "0":
                            ErrorText = "Exception H503: " + System.Environment.NewLine + itemDescription.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception H504: " + System.Environment.NewLine + itemDescription.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception H505: " + System.Environment.NewLine + "Unexpected error while transferring powder!";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception H506: " + System.Environment.NewLine + "Unexpected error while transferring powder" + System.Environment.NewLine + "No data was returned from the server!";
                    RunOnUiThread(ShowError);
                }
                
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H502: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void confirm()
        {
            try
            {
                if (txtBCD.Text != string.Empty)
                {
                    double dQty = Convert.ToDouble(txtBCD.Text.Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep)) / 10000;

                    ScanEngineOff();
                    AlertDialog.Builder build = new AlertDialog.Builder(this);
                    AlertDialog Confirm = build.Create();
                    Confirm.SetTitle("Confirm Transfer Info");
                    Confirm.SetIcon(Resource.Drawable.Icon);
                    Confirm.SetMessage(itemDesc + System.Environment.NewLine + "Powder Code: " + GlobalVar.PPtFSItemCode + System.Environment.NewLine + "Lot: " + GlobalVar.PPtFSLotNum + System.Environment.NewLine + "Dry Weight: " + Convert.ToString(dQty));
                    Confirm.SetButton("YES", (s, ev) =>
                    {
                        ScanEngineOn();
                        GlobalVar.pgsBusy = new ProgressDialog(this);
                        GlobalVar.pgsBusy.Indeterminate = true;
                        GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                        GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                        GlobalVar.pgsBusy.SetCancelable(false);
                        GlobalVar.pgsBusy.Show();
                        Thread t = new Thread(transferItem);
                        t.Start();
                    });
                    Confirm.SetButton2("NO", (s, ev) =>
                    {
                        ScanEngineOn();
                        StartActivity(typeof(H_Scan_Prep_Sheet_PP));
                        Finish();
                    });
                    Confirm.Show();
                }
                else
                {
                    ErrorText = "PLEASE SCAN A VALID QTY BARCODE!";
                    RunOnUiThread(ShowError);
                }

            }
            catch (Exception ex)
            {
                ErrorText = "Exception H502: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void transferItem()
        {

            try
            {
                string sendString = GlobalVar.PPtFSItemCode + "|" + GlobalVar.PPtFSLotNum + "|" + txtBCD.Text + "|" + GlobalVar.PPtFSWhse + "|" + GlobalVar.UserName;
                string transferred = Client.tansferPowder(sendString);
                if (transferred != string.Empty)
                {
                    switch (transferred.Split('*')[0])
                    {
                        case "1":
                            SuccessText = "Powder has been transferred!";
                            RunOnUiThread(showSuccess);
                            break;
                        case "0":
                            ErrorText = "Exception H503: " + System.Environment.NewLine + transferred.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception H504: " + System.Environment.NewLine + transferred.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception H505: " + System.Environment.NewLine + "Unexpected error while transferring powder!";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception H506: " + System.Environment.NewLine + "Unexpected error while transferring powder" + System.Environment.NewLine + "No data was returned from the server!";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H507: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(H_ScanBCD_PP));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H508: " + System.Environment.NewLine + ex.ToString();
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
            SetContentView(Resource.Layout.H_Scan_Dry_Weight);
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