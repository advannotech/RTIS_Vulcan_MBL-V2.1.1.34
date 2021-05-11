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
using static RTIS_Vulcan_MBL.GlobalVar;

namespace RTIS_Vulcan_MBL.Activities.P_Disatch
{
    [Activity(Label = "P_ScanItems")]
    public class P_ScanItems : Activity, View.IOnTouchListener
    {
        EditText txtScanItem;
        Button btnDone;
        Button btnBack;

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
            SetContentView(Resource.Layout.D_ScanItem);

            txtScanItem = FindViewById<EditText>(Resource.Id.txtItem);
            txtScanItem.KeyPress += TxtScanItem_KeyPress;
            txtScanItem.RequestFocus();

            btnDone = FindViewById<Button>(Resource.Id.btnDone);
            btnDone.Click += BtnDone_Click;
            btnDone.SetOnTouchListener(this);

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBack_Click;
            btnBack.SetOnTouchListener(this);

            DetailList = FindViewById<ListView>(Resource.Id.POlist);
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
            ScanEngineOn();

            //txtScanItem.Text = "(240)18471-N60H/2 - 9R08      (15)      (10)45827511EK          (30)00000480(90)P150319013503";
        }
        public void SetOrderDetails()
        {
            try
            {
                GlobalVar.AllSOItems = new List<SODetail>();
                string[] Data = GlobalVar._soDetails.Split('~');
                foreach (string detail in Data)
                {
                    if (detail != string.Empty)
                    {
                        GlobalVar.SODetail d = new SODetail();
                        d.ItemCode = detail.Split('|')[0];
                        d.ItemDesc = detail.Split('|')[1];
                        d.ItemLotNum = detail.Split('|')[2];
                        d.OrderQty = detail.Split('|')[3];
                        d.ProcQty = detail.Split('|')[4];
                        d.ToProcQty = detail.Split('|')[5];
                        d.lotItem = detail.Split('|')[6];

                        decimal totalOrderQty = Convert.ToDecimal(d.OrderQty) - Convert.ToDecimal(d.ProcQty);
                        d.TotalOrderQty = Convert.ToString(totalOrderQty);

                        if (Convert.ToDecimal(d.TotalOrderQty) == Convert.ToDecimal(d.ToProcQty))
                        {
                            //tempitem.complete = complete; //BitmapFactory.DecodeResource(Resources, Resource.Drawable.found);
                            d.theme = Resource.Drawable.blackbordergreen;
                        }
                        else
                        {
                            //tempitem.complete = incomplete; //BitmapFactory.DecodeResource(Resources, Resource.Drawable.notfound);
                            d.theme = Resource.Drawable.blackborder;
                        }

                        GlobalVar.AllSOItems.Add(d);
                    }
                }
                DetailList.Adapter = new GlobalVar.SODetailAdapter(this, GlobalVar.AllSOItems.ToArray());
            }
            catch (Exception ex)
            {
                ErrorText = "Exception P101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void resfreshDetails()
        {
            try
            {
                string retreived = Client.getSOLines(GlobalVar.SONumber);
                switch (retreived.Split('*')[0])
                {
                    case "1":
                        retreived = retreived.Remove(0, 2);
                        GlobalVar._soDetails = retreived;
                        SetOrderDetails();
                        break;
                    case "0":
                        ErrorText = "Exception P102: " + System.Environment.NewLine + retreived.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    case "-1":
                        ErrorText = "Exception P103: " + System.Environment.NewLine + retreived.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception P104: " + System.Environment.NewLine + "Unexpected error while retreiving PO file";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception P105: " + System.Environment.NewLine + ex.ToString();
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
            DetailList.Adapter = new GlobalVar.SODetailAdapter(this, GlobalVar.AllSOItems.ToArray());
            RunOnUiThread(() =>
            {
                ((GlobalVar.SODetailAdapter)DetailList.Adapter).NotifyDataSetChanged();
            });
        }
        private void TxtScanItem_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    if (txtScanItem.Text != "")
                    {
                        string itemCode = Barcodes.GetItemCode(txtScanItem.Text);
                        string lot = Barcodes.GetItemLot(txtScanItem.Text);
                        string qty = Barcodes.GetItemQty(txtScanItem.Text).Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                        string unq = Barcodes.GetUniqCode(txtScanItem.Text);
                        if (lot != "#NOLOT#")
                        {
                            bool itemFound = false;
                            string checkQty = string.Empty;
                            foreach (SODetail item in GlobalVar.AllSOItems)
                            {
                                if (item.ItemCode == itemCode && Convert.ToBoolean(item.lotItem))
                                {
                                    itemFound = true;
                                    checkQty = item.TotalOrderQty;
                                }
                            }

                            if (itemFound)
                            {
                                decimal _qty = Convert.ToDecimal(qty.Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep));
                                decimal _OrderQty = Convert.ToDecimal(checkQty.Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep));
                                if (_OrderQty >= _qty)
                                {
                                    string updated = Client.updateSOLineLot(GlobalVar.SONumber + "|" + itemCode + "|" + lot + "|" + qty + "|" + unq);
                                    switch (updated.Split('*')[0])
                                    {
                                        case "1":
                                            resfreshDetails();
                                            break;
                                        case "0":
                                            ErrorText = "Exception P106: " + System.Environment.NewLine + updated.Split('*')[1];
                                            RunOnUiThread(ShowError);
                                            break;
                                        case "-1":
                                            ErrorText = "Exception P107: " + System.Environment.NewLine + updated.Split('*')[1];
                                            RunOnUiThread(ShowError);
                                            break;
                                        default:
                                            ErrorText = "Exception P108: " + System.Environment.NewLine + "Unexpected error while updating sales order";
                                            RunOnUiThread(ShowError);
                                            break;
                                    }
                                }
                                else
                                {
                                    ErrorText = "THE QUANTITY SCANNED WOULD EXCEED THE QUANTITY ON THE ORDER!";
                                    RunOnUiThread(ShowError);
                                }                                
                            }
                            else
                            {
                                ErrorText = "ITEM NOT FOUND ON SO!";
                                RunOnUiThread(ShowError);
                            }
                        }
                        else
                        {
                            bool itemFound = false;
                            foreach (SODetail item in GlobalVar.AllSOItems)
                            {
                                if (item.ItemCode == itemCode && Convert.ToBoolean(item.lotItem) == false)
                                {
                                    itemFound = true;
                                }
                            }

                            if (itemFound)
                            {
                                string updated = Client.updateSOLineNoLot(GlobalVar.SONumber + "|" + itemCode + "|#NOLOT#|" + qty + "|" + unq);
                                switch (updated.Split('*')[0])
                                {
                                    case "1":
                                        resfreshDetails();
                                        break;
                                    case "0":
                                        ErrorText = "Exception P109: " + System.Environment.NewLine + updated.Split('*')[1];
                                        RunOnUiThread(ShowError);
                                        break;
                                    case "-1":
                                        ErrorText = "Exception P110: " + System.Environment.NewLine + updated.Split('*')[1];
                                        RunOnUiThread(ShowError);
                                        break;
                                    default:
                                        ErrorText = "Exception P111: " + System.Environment.NewLine + "Unexpected error while retreiving PO file";
                                        RunOnUiThread(ShowError);
                                        break;
                                }
                            }
                            else
                            {
                                ErrorText = "ITEM NOT FOUND ON SO!";
                                RunOnUiThread(ShowError);
                            }
                        }
                    }
                    else
                    {
                        ErrorText = "PLEASE SCAN AN ITEM!";
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
                ErrorText = "Exception P102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(P_ScanSO));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception P103: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(P_ScanSO));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception P104: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void DetailList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //throw new NotImplementedException();
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
            txtScanItem.Text = "";
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

            txtScanItem = FindViewById<EditText>(Resource.Id.txtItem);
            txtScanItem.KeyPress += TxtScanItem_KeyPress;
            txtScanItem.RequestFocus();

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
            DetailList.Adapter = new GlobalVar.SODetailAdapter(this, GlobalVar.AllSOItems.ToArray());
            //txtScanItem.Text = "(240)18471-N60H/2 - 9R08      (15)      (10)45827511EK          (30)00000480(90)P150319013503";
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