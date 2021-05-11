using System;
using System.Collections.Generic;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Net.Wifi;
using Android.Graphics;
using System.Linq;
using Android.Media;
using RTIS_Vulcan_MBL.Classes;

namespace RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry.Add_Fresh_Slurry
{
    [Activity(Label = "G_SlurryView_FS")]
    public class G_SlurryView_FS : Activity, View.IOnTouchListener
    {
        Button btnAdd;
        Button btnBack;

        TextView lblHeader;

        ListView DetailList;
        GlobalVar.OrderDetail SelectedItem;

        #region WiFi / Battery / OnError / Version

        #region Error

        bool threadrunning;
        string ErrorText;
        TextView lblErrorMsg;
        Button btnErrorOk;
        MediaPlayer _player;

        #endregion

        #region Warning
        string WarningText;
        TextView lblWarningMsg;
        ImageView imgError;
        Button btnWarningOk;
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
            SetContentView(Resource.Layout.G_ViewSlurries);

            lblHeader = FindViewById<TextView>(Resource.Id.lblHeader);

            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnAdd.Click += BtnAdd_Click;
            btnAdd.SetOnTouchListener(this);

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBack_Click;
            btnBack.SetOnTouchListener(this);

            DetailList = FindViewById<ListView>(Resource.Id.POlist);
            //DetailList.ItemClick += DetailList_ItemClick;

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

            setSlurryDetails();
            ScanEngineOn();
        }
        public void setSlurryDetails()
        {
            try
            {
                GlobalVar.CurrentSlurries = new List<GlobalVar.SlurryDetail>();
                string[] slurries = GlobalVar.MSAllSlurries.Split('~');
                foreach (string slurry in slurries)
                {
                    if (slurry != string.Empty)
                    {
                        string[] temp2 = slurry.Split('|');
                        GlobalVar.SlurryDetail tempitem = new GlobalVar.SlurryDetail();
                        tempitem.TrolleyCode = temp2[0];
                        tempitem.ItemCode = temp2[1];
                        tempitem.ItemDesc = temp2[2];
                        tempitem.ItemLotNum = temp2[3];
                        tempitem.weight = temp2[4];
                        GlobalVar.CurrentSlurries.Add(tempitem);
                    }
                }
                DetailList.Adapter = new GlobalVar.SlurryDetailAdapter(this, GlobalVar.CurrentSlurries.ToArray());
                if (GlobalVar.CurrentSlurries.Count == 0)
                {
                    lblHeader.Text = "No slurries added";
                    DetailList.SetBackgroundColor(Color.Argb(128, 128, 128, 128));
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_ScanTrolley_FS));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_ScanTank_FS));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G101: " + System.Environment.NewLine + ex.ToString();
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
            SetContentView(Resource.Layout.G_ViewSlurries);

            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnAdd.Click += BtnAdd_Click;
            btnAdd.SetOnTouchListener(this);

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBack_Click;
            btnBack.SetOnTouchListener(this);

            DetailList = FindViewById<ListView>(Resource.Id.POlist);
            //DetailList.ItemClick += DetailList_ItemClick;

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

            ScanEngineOn();
            DetailList.Adapter = new GlobalVar.SlurryDetailAdapter(this, GlobalVar.CurrentSlurries.ToArray());
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