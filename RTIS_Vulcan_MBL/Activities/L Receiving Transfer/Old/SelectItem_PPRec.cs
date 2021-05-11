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

namespace RTIS_Vulcan_MBL.Activities
{
    [Activity(Label = "SelectItem_PPRec")]
    public class SelectItem_PPRec : Activity, View.IOnTouchListener
    {


        TextView lblHeader;
        TextView lblHeader2;

        Button btnDone;
        Button btnBack;

        ListView DetailList;

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
            SetContentView(Resource.Layout.E_SelectItem);

            btnDone = FindViewById<Button>(Resource.Id.btnDone);
            btnDone.Click += BtnDone_Click;
            btnDone.SetOnTouchListener(this);

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBack_Click;
            btnBack.SetOnTouchListener(this);

            DetailList = FindViewById<ListView>(Resource.Id.Transferlist);
            DetailList.ItemClick += DetailList_ItemClick;

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

            SetOrderDetails();
        }

        public void SetOrderDetails()
        {
            try
            {
                GlobalVar.CurrentWIPItems = new List<GlobalVar.WIPItemDetail>();               
                string[] Data = GlobalVar.allWIPLines.Split('~');
                foreach (string item in Data)
                {
                    if (item != string.Empty)
                    {
                        GlobalVar.WIPItemDetail newItem = new GlobalVar.WIPItemDetail();
                        newItem.ItemCode = item.Split('|')[0];
                        newItem.ItemDesc = item.Split('|')[1];
                        newItem.ItemLotNum = item.Split('|')[2];
                        newItem.Qty = item.Split('|')[3];
                        newItem.theme = Resource.Drawable.blackborder;
                        GlobalVar.CurrentWIPItems.Add(newItem);
                    }
                }
                DetailList.Adapter = new GlobalVar.WIPItemDetailAdapter(this, GlobalVar.CurrentWIPItems.ToArray());

            }
            catch (Exception ex)
            {
                ErrorText = "Exception L101: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            UpdateList();
        }
        private void UpdateList()
        {
            DetailList.Adapter = new GlobalVar.WIPItemDetailAdapter(this, GlobalVar.CurrentWIPItems.ToArray());
            RunOnUiThread(() =>
            {
                ((GlobalVar.WIPItemDetailAdapter)DetailList.Adapter).NotifyDataSetChanged();
            });
        }
        private void DetailList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                GlobalVar.selectedPPWIPItem = GlobalVar.CurrentWIPItems[e.Position];
                foreach (GlobalVar.WIPItemDetail item in GlobalVar.CurrentWIPItems)
                {
                    if (item.ItemCode == GlobalVar.selectedPPWIPItem.ItemCode && item.ItemLotNum == GlobalVar.selectedPPWIPItem.ItemLotNum)
                    {
                        item.theme = Resource.Drawable.blackborderblue;
                    }
                    else
                    {
                        item.theme = Resource.Drawable.blackborder;
                    }
                }
                int visibleItem = DetailList.FirstVisiblePosition;
                int focusedItem = GlobalVar.CurrentWIPItems.IndexOf(GlobalVar.selectedPPWIPItem);
                DetailList.Adapter = new GlobalVar.WIPItemDetailAdapter(this, GlobalVar.CurrentWIPItems.ToArray());
                int xScroll = DetailList.ScrollX;
                DetailList.SetSelection(visibleItem);
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L102: " + ex.ToString();
                RunOnUiThread(ShowError);
            }           
        }
        private void BtnDone_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalVar.RecTransItemCode =  GlobalVar.selectedPPWIPItem.ItemCode;
                GlobalVar.RecTransLotNumber = GlobalVar.selectedPPWIPItem.ItemLotNum;
                GlobalVar.RecTransQty = GlobalVar.selectedPPWIPItem.Qty.Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                StartActivity(typeof(ConfirmRec_PPRec));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L103: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(SelectType_PPRec));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L104: " + ex.ToString();
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
            SetContentView(Resource.Layout.D_ScanItem);

            DetailList = FindViewById<ListView>(Resource.Id.POlist);

            btnDone = FindViewById<Button>(Resource.Id.btnDone);
            btnDone.Click += BtnDone_Click;
            btnDone.SetOnTouchListener(this);

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBack_Click;
            btnBack.SetOnTouchListener(this);

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
            DetailList.Adapter = new GlobalVar.OrderDetailAdapter(this, GlobalVar.CurrentOrderItem.ToArray());
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