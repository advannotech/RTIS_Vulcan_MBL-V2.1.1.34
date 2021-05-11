using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Security;
using RTIS_Vulcan_MBL.Classes;

namespace RTIS_Vulcan_MBL.Activities.Q_Palletizing
{
    [Activity(Label = "Q_ScanItems")]
    public class Q_ScanItems : Activity, View.IOnTouchListener
    {
        EditText txtScanItem;
        Button btnDone;
        Button btnBack;

        ListView DetailList;
        GlobalVar.PalletItem SelectedPalletItem;

        private string Itemcode;
        private string ItemDesc;
        private string Lot;
        private string Qty;
        private string Unq;

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
            StartActivity(typeof(Q_ScanItems));
            Finish();
        }
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

            //txtScanItem.Text =
            //    "(240)CHEM-1180                (15)      (10)LOT 15082018        (30)00000250(90)150818125433";
            GlobalVar.AllPalletItems = new List<GlobalVar.PalletItem>();
            refreshList();
            //SetOrderDetails();
            ScanEngineOn();
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
                        if (string.IsNullOrEmpty(Itemcode))
                        {
                            Itemcode = itemCode;
                        }

                        if (itemCode == Itemcode)
                        {
                            Lot = Barcodes.GetItemLot(txtScanItem.Text);
                            Qty = Barcodes.GetItemQty(txtScanItem.Text).Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                            Unq = Barcodes.GetUniqCode(txtScanItem.Text);
                            GlobalVar.pgsBusy = new ProgressDialog(this);
                            GlobalVar.pgsBusy.Indeterminate = true;
                            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                            GlobalVar.pgsBusy.SetMessage("Getting item information...");
                            GlobalVar.pgsBusy.SetCancelable(false);
                            GlobalVar.pgsBusy.Show();
                            Thread t = new Thread(GetItemDescrition);
                            t.Start();
                        }
                        else
                        {
                            ErrorText = "Only one type of item can be packed on a pallet!";
                            RunOnUiThread(ShowError);
                        }
                    }
                }
                else
                {
                    e.Handled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void GetItemDescrition()
        {
            try
            {
                string itemDesc = Client.GetPalletItemDesc(txtScanItem.Text);
                if (itemDesc != string.Empty)
                {
                    switch (itemDesc.Split('*')[0])
                    {
                        case "1":
                            ItemDesc = itemDesc.Remove(0, 2);
                            RunOnUiThread(AddItemToList);
                            break;
                        case "0":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + itemDesc.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + itemDesc.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception G104: Unexpected result from server!" + System.Environment.NewLine + itemDesc;
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
                ErrorText = "Exception G104: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void AddItemToList()
        {
            try
            {
                List<string> itemInfo = new List<string>();
                itemInfo.Add(Itemcode);
                itemInfo.Add(ItemDesc);
                itemInfo.Add(Lot);
                itemInfo.Add(Qty);
                itemInfo.Add(txtScanItem.Text);

                GlobalVar.PalletItem testItem = GlobalVar.AllPalletItems.Find(c => c.Unq == Unq);
                if (testItem == null)
                {
                    GlobalVar.AllPalletItems.Add(new GlobalVar.PalletItem(itemInfo.ToArray()));
                    txtScanItem.Text = string.Empty;
                    if (GlobalVar.pgsBusy.IsShowing)
                    {
                        GlobalVar.pgsBusy.Dismiss();
                    }
                    refreshList();
                }
                else
                {
                    ErrorText = "This item has already been scanned onto the pallet.";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void DetailList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ErrorText = ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVar.AllPalletItems.Count > 0)
                {
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Saving pallet information...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(SavePalletBarcodes);
                    t.Start();
                }
                else
                {
                    ErrorText = "Please scan at least one item onto the pallet";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void SavePalletBarcodes()
        {
            try
            {
                List<string> allUnqs = new List<string>();
                foreach (GlobalVar.PalletItem item in GlobalVar.AllPalletItems)
                {
                    if (item != null)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Unq))
                        {
                            allUnqs.Add(item.Unq);
                        }
                    }
                }

                //allUnqs.Add(GlobalVar.AllPalletItems.Find(c => c.Unq != string.Empty).Unq);
                string unqs = string.Join("~", allUnqs);
                string itemDesc = Client.SavePalletInfo(Itemcode + "|" + unqs);
                if (itemDesc != string.Empty)
                {
                    switch (itemDesc.Split('*')[0])
                    {
                        case "1":
                            SuccessText = "Items have been successfully palletized";
                            RunOnUiThread(showSuccess);
                            break;
                        case "0":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + itemDesc.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + itemDesc.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception G104: Unexpected result from server!" + System.Environment.NewLine + itemDesc;
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
                ErrorText = ex.ToString();
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
                ErrorText = ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void refreshList()
        {
            DetailList.Adapter = new GlobalVar.PalletItemAdapter(this, GlobalVar.AllPalletItems.ToArray());
        }

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

            refreshList();
            //SetOrderDetails();
            ScanEngineOn();
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
    }
}