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

namespace RTIS_Vulcan_MBL.Activities
{
    [Activity(Label = "ConfirmRec_PPRec")]
    public class ConfirmRec_PPRec : Activity, View.IOnTouchListener
    {
        TextView lblHeader1;
        TextView lblHeader2;
        TextView lblCode;
        TextView lblHeader3;
        TextView lblLotNumber;
        TextView lblHeader4;

        EditText txtQty1;
        EditText txtQty2;

        Button btnBack;
        Button btnNext;

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
            StartActivity(typeof(SelectType_PPRec));
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
            SetContentView(Resource.Layout.E_Confirm_Rec_Trans);
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
            lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
            lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
            lblCode = FindViewById<TextView>(Resource.Id.lblCode);
            lblCode.Text = GlobalVar.RecTransItemCode;
            lblHeader3 = FindViewById<TextView>(Resource.Id.lblHeader3);
            lblLotNumber = FindViewById<TextView>(Resource.Id.lblLotNumber);
            lblLotNumber.Text = GlobalVar.RecTransLotNumber;
            lblHeader4 = FindViewById<TextView>(Resource.Id.lblHeader4);

            if (GlobalVar.RecTransQty.Contains(GlobalVar.sep))
            {
                txtQty1 = FindViewById<EditText>(Resource.Id.txtQty1);
                txtQty1.Text = GlobalVar.RecTransQty.Split(Convert.ToChar(GlobalVar.sep))[0];

                txtQty2 = FindViewById<EditText>(Resource.Id.txtQty2);
                txtQty2.Text = GlobalVar.RecTransQty.Split(Convert.ToChar(GlobalVar.sep))[1];
            }
            else
            {
                txtQty1 = FindViewById<EditText>(Resource.Id.txtQty1);
                txtQty1.Text = GlobalVar.RecTransQty;

                txtQty2 = FindViewById<EditText>(Resource.Id.txtQty2);
                txtQty2.Text = "0";
            }

            btnNext = FindViewById<Button>(Resource.Id.btnNext);
            btnNext.SetOnTouchListener(this);
            btnNext.Click += BtnNext_Click;

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.SetOnTouchListener(this);
            btnBack.Click += BtnBack_Click;
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalVar.pgsBusy = new ProgressDialog(this);
                GlobalVar.pgsBusy.Indeterminate = true;
                GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                GlobalVar.pgsBusy.SetCancelable(false);
                GlobalVar.pgsBusy.Show();
                Thread t = new Thread(saveTransfer);
                t.Start();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L201: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void saveTransfer()
        {
            try
            {
                string qty = txtQty1.Text + GlobalVar.sep + txtQty2.Text;
                string transferred = string.Empty;
                switch (GlobalVar.ProcessName)
                {
                    case "PPtFS":
                        transferred = Client.PPTransferFromWIP(GlobalVar.RecTransItemCode + "|" + GlobalVar.RecTransLotNumber + "|" + GlobalVar.RecTransWarehouseFrom + "|" + GlobalVar.PPtFSWhse + "|" + qty + "|" + GlobalVar.UserName);
                        break;
                    case "FStMS":
                        transferred = Client.tansferFromTransitFS(GlobalVar.RecTransItemCode + "|" + GlobalVar.RecTransLotNumber + "|" + GlobalVar.RecTransWarehouseFrom + "|" + GlobalVar.PPtFSWhse + "|" + qty + "|" + GlobalVar.UserName);
                        break;
                    case "MStZECT":
                        transferred = Client.tansferFromTransitMS(GlobalVar.RecTransItemCode + "|" + GlobalVar.RecTransLotNumber + "|" + GlobalVar.RecTransWarehouseFrom + "|" + GlobalVar.PPtFSWhse + "|" + qty + "|" + GlobalVar.UserName);
                        break;
                }
                          
                if (transferred != string.Empty)
                {
                    switch (transferred.Split('*')[0])
                    {
                        case "1":
                            SuccessText = "Transfer Captured Successfully!";
                            RunOnUiThread(showSuccess);
                            break;
                        case "-1":
                            ErrorText = "Exception L202: " + transferred.Split('*')[1].ToUpper();
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception L203: " + "An unexpected error occured while transferring the item";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception L204: " + "An unexpected error occured while transferring the item";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L205: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void showNext()
        {
            try
            {
                StartActivity(typeof(SelectType_PPRec));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L206: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                switch (GlobalVar.RecTransType)
                {
                    case "RT2D":
                        //StartActivity(typeof());
                        //Finish();
                        break;
                    case "Select":
                        StartActivity(typeof(SelectItem_PPRec));
                        Finish();
                        break;
                    default:
                        StartActivity(typeof(SelectType_PPRec));
                        Finish();
                        break;
                }                
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L207: " + System.Environment.NewLine + ex.ToString();
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
            SetContentView(Resource.Layout.B_Sub_Menu_2);
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