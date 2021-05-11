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

namespace RTIS_Vulcan_MBL.Activities.L_Receiving_Transfer
{
    [Activity(Label = "ScanZectItem")]
    public class ScanZectItem : Activity, View.IOnTouchListener
    {
        TextView lblHeader1;
        TextView lblHeader2;
        EditText txtItem;
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
            StartActivity(typeof(ScanZectItem));
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

        public void layoutStichUp()
        {
            try
            {
                switch (GlobalVar.ProcessName)
                {
                    case "ZECT1":
                        lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                        lblHeader1.Text = "From Transit to Zect 1";

                        lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                        lblHeader2.Text = "Scan CATScan Barcode";
                        break;
                    case "ZECT2":
                        lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                        lblHeader1.Text = "From Transit to Zect 2";

                        lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                        lblHeader2.Text = "Scan CATScan Barcode";
                        break;
                    default:
                        break;
                }

                txtItem = FindViewById<EditText>(Resource.Id.txtTank);
                txtItem.KeyPress += TxtItem_KeyPress;

                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L401: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void TxtItem_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    GlobalVar.RecTransWarehouseTo = GlobalVar.ZectWhse;
                    if (txtItem.Text.Contains("(") && txtItem.Text.Contains(")"))
                    {
                        GlobalVar.RecTransItemCode = Barcodes.GetItemCode(txtItem.Text);
                        GlobalVar.RecTransLotNumber = Barcodes.GetItemLot(txtItem.Text);
                        GlobalVar.RecTransQty = Barcodes.GetItemQty(txtItem.Text).Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                        GlobalVar.RecTransUnq = Barcodes.GetUniqCode(txtItem.Text);
                        ScanEngineOff();

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
                        ErrorText = "Please scan a valid CATScan Barcode!";
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
                ErrorText = "Exception L409: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void getItemDesc()
        {
            try
            {
                string itemDesc = Client.getItemDescription(GlobalVar.RecTransItemCode);
                if (itemDesc != string.Empty)
                {
                    switch (itemDesc.Split('*')[0])
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
                            itemDesc = itemDesc.Remove(0, 2);
                            GlobalVar.RecTransItemDesc = itemDesc;
                            RunOnUiThread(confirmRT2DTransfers);
                            break;
                        case "0":
                            ErrorText = "Exception H710: " + System.Environment.NewLine + itemDesc.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception H711: " + System.Environment.NewLine + itemDesc.Split('*')[1];
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
        public void confirmRT2DTransfers()
        {
            try
            {
                string whseTo = string.Empty;
                switch (GlobalVar.ProcessName)
                {
                    case "PPtFS":
                        whseTo = GlobalVar.PPtFSWhse;
                        break;
                    case "FStMS":
                        whseTo = GlobalVar.FStMSWhse;
                        break;
                    case "MStZECT":
                        whseTo = GlobalVar.MStZectWhse;
                        break;
                    case "ZECT1":
                        whseTo = GlobalVar.ZectWhse;
                        break;
                    case "ZECT2":
                        whseTo = GlobalVar.ZectWhse;
                        break;
                    default:
                        break;
                }

                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                Confirm.SetTitle("Confirm ZECT Input");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage(GlobalVar.RecTransItemDesc + System.Environment.NewLine + "Lot: " + GlobalVar.RecTransLotNumber + System.Environment.NewLine + "Qty: " + GlobalVar.RecTransQty + System.Environment.NewLine);
                Confirm.SetButton("YES", (s, ev) =>
                {
                    ScanEngineOn();
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(transferItemZect);
                    t.Start();
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    ScanEngineOn();
                    StartActivity(typeof(ScanZectItem));
                    Finish();
                });
                Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void transferItemZect()
        {
            try
            {
                string transferred = Client.issueToZectJob(GlobalVar.RecZectJob + "|" +  GlobalVar.RecTransItemCode + "|" + GlobalVar.RecTransLotNumber + "|" + GlobalVar.RecTransQty + "|" + GlobalVar.UserName + "|" + GlobalVar.RecTransUnq);
                if (transferred != string.Empty)
                {
                    switch (transferred.Split('*')[0])
                    {
                        case "1":
                            SuccessText = "Infromation Captured Successfully!";
                            RunOnUiThread(showSuccess);
                            break;
                        case "0":
                            ErrorText = "Exception L123: " + System.Environment.NewLine + transferred.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception L124: " + System.Environment.NewLine + transferred.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception L125: " + System.Environment.NewLine + "Unexpected error while transferring powder!";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception L126: " + "Unexpected error while saving transfer";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L127: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(ScanZectJob));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L409: " + ex.ToString();
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
            txtItem.RequestFocus();
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