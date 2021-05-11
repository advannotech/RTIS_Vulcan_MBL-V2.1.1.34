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
    [Activity(Label = "FS_Enter_Solidity")]
    public class FS_Enter_Solidity : Activity, View.IOnTouchListener
    {
        TextView lblHeader1;
        TextView lblHeader2;

        EditText txtWeight1;
        EditText txtWeight2;

        Button btnBack;
        Button btnNext;

        string qty = string.Empty;

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
            SetContentView(Resource.Layout.F_G_EnterWeight);
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
            try
            {
                lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                lblHeader1.Text = "Fresh Slurry Solidity";

                lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                lblHeader2.Text = "Enter Solidity";

                txtWeight1 = FindViewById<EditText>(Resource.Id.txtWeight1);
                txtWeight1.KeyPress += TxtWeight1_KeyPress;
                txtWeight1.Text = "0";

                txtWeight2 = FindViewById<EditText>(Resource.Id.txtWeight2);
                txtWeight2.KeyPress += TxtWeight2_KeyPress;
                txtWeight2.Text = "0";

                btnNext = FindViewById<Button>(Resource.Id.btnNext);
                btnNext.SetOnTouchListener(this);
                btnNext.Click += BtnNext_Click;

                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;

                txtWeight1.SelectAll();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception FS101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }

        }

        private void TxtWeight1_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    if (txtWeight1.Text.All(char.IsDigit))
                    {
                        txtWeight2.RequestFocus();
                        txtWeight2.SelectAll();
                    }
                    else
                    {
                        ErrorText = "PLEASE ENTER A VALID QTY!";
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
                ErrorText = "Exception FS102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
            
        }

        private void TxtWeight2_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    if (txtWeight2.Text.All(char.IsDigit))
                    {
                        qty = txtWeight1.Text + GlobalVar.sep + txtWeight2.Text;
                        AlertDialog.Builder build = new AlertDialog.Builder(this);
                        AlertDialog Confirm = build.Create();
                        Confirm.SetTitle("Confirm Slurry Info");
                        Confirm.SetIcon(Resource.Drawable.Icon);
                        Confirm.SetMessage("Trolley Code: " + GlobalVar.FSSTrolleyCode + System.Environment.NewLine + "Item: " + GlobalVar.FSSItemDesc + System.Environment.NewLine + "Lot: " + GlobalVar.FSSLotNum + System.Environment.NewLine + "Solidity: " + Convert.ToString(qty));
                        Confirm.SetButton("YES", (s, ev) =>
                        {
                            GlobalVar.pgsBusy = new ProgressDialog(this);
                            GlobalVar.pgsBusy.Indeterminate = true;
                            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                            GlobalVar.pgsBusy.SetMessage("Storing Data...");
                            GlobalVar.pgsBusy.SetCancelable(false);
                            GlobalVar.pgsBusy.Show();
                            Thread t = new Thread(saveSolidity);
                            t.Start();
                        });
                        Confirm.SetButton2("NO", (s, ev) =>
                        {
                            ScanEngineOn();
                            StartActivity(typeof(FS_Scan_Trolley));
                            Finish();
                        });
                        Confirm.Show();
                    }
                    else
                    {
                        ErrorText = "PLEASE ENTER A VALID QTY!";
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
                ErrorText = "Exception FS103: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtWeight1.Text.All(char.IsDigit) && txtWeight2.Text.All(char.IsDigit))
                {
                    qty = txtWeight1.Text + GlobalVar.sep + txtWeight2.Text;
                    AlertDialog.Builder build = new AlertDialog.Builder(this);
                    AlertDialog Confirm = build.Create();
                    Confirm.SetTitle("Confirm Slurry Info");
                    Confirm.SetIcon(Resource.Drawable.Icon);
                    Confirm.SetMessage("Trolley Code: " + GlobalVar.FSSTrolleyCode + System.Environment.NewLine + "Item: " + GlobalVar.FSSItemDesc + System.Environment.NewLine + "Lot: " + GlobalVar.FSSLotNum + System.Environment.NewLine + "Solidity: " + Convert.ToString(qty));
                    Confirm.SetButton("YES", (s, ev) =>
                    {
                        ScanEngineOn();
                        GlobalVar.pgsBusy = new ProgressDialog(this);
                        GlobalVar.pgsBusy.Indeterminate = true;
                        GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                        GlobalVar.pgsBusy.SetMessage("Storing Data...");
                        GlobalVar.pgsBusy.SetCancelable(false);
                        GlobalVar.pgsBusy.Show();
                        Thread t = new Thread(saveSolidity);
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
                else
                {
                    ErrorText = "PLEASE ENTER A VALID QTY!";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception FS104: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void saveSolidity()
        {
            try
            {
                string saved = Client.setSlurrySolidity(GlobalVar.FSSTrolleyCode + "|" + GlobalVar.FSSItemCode + "|" + qty + "|" + GlobalVar.UserName);
                switch (saved.Split('*')[0])
                {
                    case "1":
                        SuccessText = "DATA CAPTURED SUCCESSFULLY";
                        RunOnUiThread(showSuccess);
                        break;
                    case "0":
                        ErrorText = "Exception FS105: " + System.Environment.NewLine + saved.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    case "-1":
                        ErrorText = "Exception FS106: " + System.Environment.NewLine + saved.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception FS107: " + System.Environment.NewLine + "Unexpected error while storing data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception FS108: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(FS_Scan_Trolley));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception FS109: " + System.Environment.NewLine + ex.ToString();
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
            StartActivity(typeof(FS_Enter_Solidity));
            Finish();
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