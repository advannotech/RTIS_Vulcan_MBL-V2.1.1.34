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

namespace RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry.ZAC
{
    [Activity(Label = "G_SelectChem_ZAC")]
    public class G_SelectChem_ZAC : Activity, View.IOnTouchListener
    {
        TextView lblHeader;

        Button btnAdd;
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
            StartActivity(typeof(G_Menu));
            Finish();
        }
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
            lblHeader.Text = "Select a Chemical";

            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnAdd.Click += BtnAdd_Click;
            btnAdd.SetOnTouchListener(this);

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

            GlobalVar.pgsBusy = new ProgressDialog(this);
            GlobalVar.pgsBusy.Indeterminate = true;
            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
            GlobalVar.pgsBusy.SetMessage("Checking Slurry Trolley...");
            GlobalVar.pgsBusy.SetCancelable(false);
            GlobalVar.pgsBusy.Show();
            Thread t = new Thread(GetChemicals);
            t.Start();

            //setChemicals();
            ScanEngineOn();
        }

        public void GetChemicals()
        {
            try
            {
                string allChemicals = Client.GetZACChemicals();
                if (!string.IsNullOrWhiteSpace(allChemicals))
                {
                    string returnCode = allChemicals.Split('*')[0];
                    string returnData = allChemicals.Split('*')[1];
                    switch (returnCode)
                    {
                        case "1":
                            GlobalVar.AllChemicals = new List<GlobalVar.ChemiclDetail>();
                            string[] returnLines = returnData.Split('~');
                            foreach (string returnLine in returnLines)
                            {
                                if (!string.IsNullOrWhiteSpace(returnLine))
                                {
                                    GlobalVar.ChemiclDetail chem = new GlobalVar.ChemiclDetail();
                                    chem.chemicalName = returnLine;
                                    chem.theme = Resource.Drawable.blackborder;
                                    GlobalVar.AllChemicals.Add(chem);
                                }
                            }
                            RunOnUiThread(setChemicals);
                            break;
                        case "0":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + returnData;
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + returnData;
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception G104: Unexpected result from server!" + System.Environment.NewLine + allChemicals;
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
                ErrorText = "Exception G101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void setChemicals()
        {
            try
            {
                //GlobalVar.AllChemicals = new List<GlobalVar.ChemiclDetail>();

                //GlobalVar.ChemiclDetail chem1 = new GlobalVar.ChemiclDetail();
                //chem1.chemicalName = "Duasyn Acid Blue";
                //chem1.theme = Resource.Drawable.blackborder;
                //GlobalVar.AllChemicals.Add(chem1);

                //GlobalVar.ChemiclDetail chem2 = new GlobalVar.ChemiclDetail();
                //chem2.chemicalName = "Citric Acid";
                //chem2.theme = Resource.Drawable.blackborder;
                //GlobalVar.AllChemicals.Add(chem2);

                //GlobalVar.ChemiclDetail chem3 = new GlobalVar.ChemiclDetail();
                //chem3.chemicalName = "HEC";
                //chem3.theme = Resource.Drawable.blackborder;
                //GlobalVar.AllChemicals.Add(chem3);

                //GlobalVar.ChemiclDetail chem4 = new GlobalVar.ChemiclDetail();
                //chem4.chemicalName = "NOPTECHS";
                //chem4.theme = Resource.Drawable.blackborder;
                //GlobalVar.AllChemicals.Add(chem4);

                //GlobalVar.ChemiclDetail chem5 = new GlobalVar.ChemiclDetail();
                //chem5.chemicalName = "GW3394";
                //chem5.theme = Resource.Drawable.blackborder;
                //GlobalVar.AllChemicals.Add(chem5);

                if (GlobalVar.pgsBusy.IsShowing)
                {
                    GlobalVar.pgsBusy.Dismiss();
                }

                DetailList.Adapter = new GlobalVar.ChemiclDetailAdapter(this, GlobalVar.AllChemicals.ToArray());
                
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G104: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void DetailList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                GlobalVar.selectedChem = GlobalVar.AllChemicals[e.Position];
                GlobalVar.selectedChemName = GlobalVar.AllChemicals[e.Position].chemicalName;
                foreach (GlobalVar.ChemiclDetail item in GlobalVar.AllChemicals)
                {
                    if (item.chemicalName == GlobalVar.selectedChemName)
                    {
                        item.theme = Resource.Drawable.blackborderblue;
                    }
                    else
                    {
                        item.theme = Resource.Drawable.blackborder;
                    }
                }
                int visibleItem = DetailList.FirstVisiblePosition;
                int focusedItem = GlobalVar.AllChemicals.IndexOf(GlobalVar.selectedChem);
                DetailList.Adapter = new GlobalVar.ChemiclDetailAdapter(this, GlobalVar.AllChemicals.ToArray());
                int xScroll = DetailList.ScrollX;
                DetailList.SetSelection(visibleItem);
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G104: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_EnterQty_ZAC));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G104: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void showNext()
        {
            try
            {
                //AlertDialog.Builder build = new AlertDialog.Builder(this);
                //AlertDialog Confirm = build.Create();
                //Confirm.SetTitle("Zonen and Charging");
                //Confirm.SetIcon(Resource.Drawable.Icon);
                //Confirm.SetMessage("Tank: " + GlobalVar.MSTankCode + System.Environment.NewLine + "Item: " + GlobalVar.MSZACDesc + System.Environment.NewLine + "Lot: " + GlobalVar.MSZACLot + System.Environment.NewLine + "Chemical: " + GlobalVar.selectedChemName);
                //Confirm.SetButton("YES", (s, ev) =>
                //{
                //    GlobalVar.pgsBusy = new ProgressDialog(this);
                //    GlobalVar.pgsBusy.Indeterminate = true;
                //    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                //    GlobalVar.pgsBusy.SetMessage("Adding Chemical...");
                //    GlobalVar.pgsBusy.SetCancelable(false);
                //    GlobalVar.pgsBusy.Show();
                //    Thread t = new Thread(setChemical);
                //    t.Start();
                //});
                //Confirm.SetButton2("NO", (s, ev) =>
                //{
                //    ScanEngineOn();
                //    StartActivity(typeof(G_SelectChem_ZAC));
                //    Finish();
                //});
                //Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception F202: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void setChemical()
        {
            try
            {
                string inserted = Client.updateZACChem(GlobalVar.MSTankType + "|" + GlobalVar.MSTankCode + "|" + GlobalVar.MSItemCode + "|" + GlobalVar.MSZACLot + "|" + GlobalVar.selectedChemName + "|" + GlobalVar.UserName);
                try
                {
                    if (GlobalVar.pgsBusy.IsShowing == true)
                    {
                        GlobalVar.pgsBusy.Dismiss();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                if (inserted != string.Empty)
                {
                    switch (inserted.Split('*')[0])
                    {
                        case "1":
                            SuccessText = "Data captured successfully";
                            RunOnUiThread(showSuccess);
                            break;
                        case "0":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + inserted.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception G104: " + System.Environment.NewLine + inserted.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception G104: Unexpected result from server!" + System.Environment.NewLine + inserted;
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
                ErrorText = "Exception F202: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(G_ScanTank_ZAC));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception G104: " + System.Environment.NewLine + ex.ToString();
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