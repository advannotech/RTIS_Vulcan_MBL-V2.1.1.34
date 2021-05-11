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
using System.Collections.Generic;
using RTIS_Vulcan_MBL.Activities.H_Transfers.Mixed_Slurry;

namespace RTIS_Vulcan_MBL.Activities
{
    [Activity(Label = "H_Select_MS_Whse")]
    public class H_Select_MS_Whse : Activity, View.IOnTouchListener
    {
        TextView lblHeader1;
        TextView lblHeader2;

        Spinner spnWhses;

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
            StartActivity(typeof(B_Sub_Menu));
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
            SetContentView(Resource.Layout.H_SelectWhse);
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

            GlobalVar.pgsBusy = new ProgressDialog(this);
            GlobalVar.pgsBusy.Indeterminate = true;
            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
            GlobalVar.pgsBusy.SetMessage("Retrieving Warehouses...");
            GlobalVar.pgsBusy.SetCancelable(false);
            GlobalVar.pgsBusy.Show();
            Thread t = new Thread(GetWHTMSDetails);
            t.Start();
        }

        public void layoutStichUp()
        {
            try
            {
                lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                lblHeader1.Text = "Mixed Slurry Transfer";

                lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                lblHeader2.Text = "Select Warehouse To";

                spnWhses = FindViewById<Spinner>(Resource.Id.spnWhses);
                spnWhses.ItemSelected += SpnWhses_ItemSelected;

                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;

                btnNext = FindViewById<Button>(Resource.Id.btnNext);
                btnNext.SetOnTouchListener(this);
                btnNext.Click += BtnNext_Click;
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H201: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }

        }
             
        public void GetWHTMSDetails()
        {
            try
            {
                string Whses = Client.getMixedSlurryWarehouses();
                switch (Whses.Split('*')[0])
                {
                    case "1":
                        Whses = Whses.Remove(0, 2);
                        GlobalVar.TransferWarehouses = new System.Collections.Generic.List<GlobalVar.WarehouseDetails>();
                        string[] allWhses = Whses.Split('~');
                        foreach (string whse in allWhses)
                        {
                            if (whse != string.Empty)
                            {
                                GlobalVar.WarehouseDetails whDetail = new GlobalVar.WarehouseDetails();
                                whDetail.Code = whse.Split('|')[0];
                                whDetail.Description = whse.Split('|')[1];
                                GlobalVar.TransferWarehouses.Add(whDetail);
                            }
                        }
                        RunOnUiThread(SetWHSpinner);
                        break;
                    case "0":
                        ErrorText = "Exception H203: " + System.Environment.NewLine + Whses.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    case "-1":
                        ErrorText = "Exception H204: " + System.Environment.NewLine + Whses.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception H205: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H206: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void SetWHSpinner()
        {
            try
            {
                List<string> whses = new List<string>();
                foreach (GlobalVar.WarehouseDetails w in GlobalVar.TransferWarehouses)
                {
                    whses.Add(w.Description);
                }

                ArrayAdapter localadapterTo = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, whses.ToArray());
                localadapterTo.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnWhses.Adapter = localadapterTo;
                spnWhses.Enabled = true;

                try
                {
                    if (GlobalVar.pgsBusy.IsShowing)
                    {
                        GlobalVar.pgsBusy.Dismiss();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H207: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void SpnWhses_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                Spinner thisspinner = (Spinner)sender;
                GlobalVar.MStZectWarehouseTo = GlobalVar.TransferWarehouses[e.Position].Code;
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H208: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(H_ScanTank_MS));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H209: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            try
            {
                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                Confirm.SetTitle("Confirm Slurry Info");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage("Tank Code: " + GlobalVar.MStZectTankCode + System.Environment.NewLine + "Slurry : " + GlobalVar.MStZectItemDesc + System.Environment.NewLine + "Lot: " + GlobalVar.MStZectLotNum + System.Environment.NewLine + "Dry Weigth: " + GlobalVar.MStZectSQty + System.Environment.NewLine + "Warehouse To: " + GlobalVar.MStZectWarehouseTo);
                Confirm.SetButton("YES", (s, ev) =>
                {
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(transferMixedSlurry);
                    t.Start();
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    ScanEngineOn();
                    StartActivity(typeof(H_ScanTank_MS));
                    Finish();
                });
                Confirm.Show();

                
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H210: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void transferMixedSlurry()
        {
            try
            {
                string transferred = Client.transferMixedSlurry(GlobalVar.MSTankType + "|" + GlobalVar.MStZectTankCode + "|" + GlobalVar.MStZectItemCode + "|" + GlobalVar.MStZectLotNum + "|" + GlobalVar.MStZectSQty + "|" + GlobalVar.MStZectWhse + "|" + GlobalVar.MStZectWarehouseTo + "|" + GlobalVar.UserName );
                switch (transferred.Split('*')[0])
                {
                    case "1":
                        SuccessText = "DATA CAPTURED SUCCESSFULLY";
                        RunOnUiThread(showSuccess);
                        break;
                    case "0":
                        ErrorText = "Exception H211: " + System.Environment.NewLine + transferred.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    case "-1":
                        ErrorText = "Exception H212: " + System.Environment.NewLine + transferred.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception H213: " + System.Environment.NewLine + "Unexpected error while storing data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H214: " + System.Environment.NewLine + ex.ToString();
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
            SetContentView(Resource.Layout.H_SelectWhse);
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