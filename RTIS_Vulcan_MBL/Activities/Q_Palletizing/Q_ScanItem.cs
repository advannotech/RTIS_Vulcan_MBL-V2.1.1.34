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
using RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry;

namespace RTIS_Vulcan_MBL.Activities.Q_Palletizing
{
    [Activity(Label = "Q_ScanItem")]
    public class Q_ScanItem : Activity, View.IOnTouchListener
    {
        TextView lblHeader1;
        TextView lblHeader2;
        EditText txtItem;
        Button btnBack;
        Button btnDone;

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
            StartActivity(typeof(Q_ScanItem));
            Finish();
        }
        #endregion

        TextView lblVersion;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Q_ScanItem);
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
                lblHeader1.Text = "Pallet Breakdown";

                lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                lblHeader2.Text = "Scan Item To Remove";

                txtItem = FindViewById<EditText>(Resource.Id.txtOrder);
                txtItem.KeyPress += TxtItem_KeyPress;
                //txtItem.Text =
                //    "(240)CHEM-1410                (15)      (10)BDD095              (30)00001000(90)020320072819";

                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;

                btnDone = FindViewById<Button>(Resource.Id.btnDone);
                btnDone.SetOnTouchListener(this);
                btnDone.Click += BtnDone_Click;

                txtItem.RequestFocus();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception Q201: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void TxtItem_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    if (!string.IsNullOrWhiteSpace(txtItem.Text))
                    {
                        if (GlobalVar.PalletList.Contains(txtItem.Text))
                        {
                            GlobalVar.pgsBusy = new ProgressDialog(this);
                            GlobalVar.pgsBusy.Indeterminate = true;
                            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                            GlobalVar.pgsBusy.SetMessage("Removing item from pallet...");
                            GlobalVar.pgsBusy.SetCancelable(false);
                            GlobalVar.pgsBusy.Show();
                            Thread t = new Thread(RemoveItemFromPallet);
                            t.Start();
                        }
                        else
                        {
                            ErrorText = "Item not found on pallet!";
                            RunOnUiThread(ShowError);
                        }
                    }
                    else
                    {
                        ErrorText = "Please scan a pallet barcode!";
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
                ErrorText = "Exception Q201: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void RemoveItemFromPallet()
        {
            try
            {
                string itemRemoved = Client.RemoveItemFromPallet(GlobalVar.ScannedPallet + "|" + txtItem.Text);
                if (itemRemoved != string.Empty)
                {
                    switch (itemRemoved.Split('*')[0])
                    {
                        case "1":
                            SuccessText = "Item has been removed from the pallet";
                            RunOnUiThread(showSuccess);
                            break;
                        case "0":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + itemRemoved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + itemRemoved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception G104: Unexpected result from server!" + System.Environment.NewLine + itemRemoved;
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
                ErrorText = "Exception Q201: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnDone_Click(object sender, EventArgs e)
        {
            try
            {
                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                Confirm.SetTitle("Please Confirm");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage("Would you like to print a new pallet label?");
                Confirm.SetButton("YES", (s, ev) =>
                {
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Printing Label...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(PrintPalletLabel);
                    t.Start();
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    ScanEngineOn();
                    StartActivity(typeof(Q_ScanItem));
                    Finish();
                });
                Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception Q201: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void PrintPalletLabel()
        {
            try
            {
                string labelPrinted = Client.PrintPalletLabel(GlobalVar.ScannedPallet);
                if (labelPrinted != string.Empty)
                {
                    switch (labelPrinted.Split('*')[0])
                    {
                        case "1":
                            SuccessText = "A new pallet label has been printed.";
                            RunOnUiThread(showSuccess);
                            break;
                        case "0":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + labelPrinted.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + labelPrinted.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception G104: Unexpected result from server!" + System.Environment.NewLine + labelPrinted;
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
                ErrorText = "Exception Q201: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(Q_Scan_Pallet));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception Q201: " + System.Environment.NewLine + ex.ToString();
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
            try
            {
                StartActivity(typeof(Q_ScanItem));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G104: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
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