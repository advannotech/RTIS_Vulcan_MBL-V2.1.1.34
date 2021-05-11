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

namespace RTIS_Vulcan_MBL
{
    [Activity(Label = "ScanItems")]
    public class ScanItems : Activity, View.IOnTouchListener
    {
        EditText txtScanItem;
        Button btnDone;
        Button btnBack;

        ListView DetailList;
        GlobalVar.OrderDetail SelectedItem;

        public string failedLines = string.Empty;
        public string failedLabels = string.Empty;

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
            
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(ScanDocument));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
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

                        string barcodeFound = SQLite.checkUnqBarcode(txtScanItem.Text);
                        switch (barcodeFound.Split('*')[0])
                        {
                            case "1":
                                barcodeFound = barcodeFound.Remove(0, 2);
                                string recPO = barcodeFound.Split('|')[0];
                                if (recPO == string.Empty)
                                {
                                    List<GlobalVar.OrderDetail> itemlist = GlobalVar.CurrentOrderItem;
                                    if (lot != "#NOLOT#")
                                    {
                                        #region Lot Item
                                        int index = 0;
                                        int itemIndex = 0;
                                        bool itemFound = false;
                                        decimal orderQty = 0;
                                        decimal currentRecQty = 0;
                                        decimal newRecQty = 0;
                                        foreach (GlobalVar.OrderDetail t in itemlist)
                                        {
                                            if (t.ItemCode == itemCode && t.ItemLotNum == lot)
                                            {
                                                decimal recQty = Convert.ToDecimal(t.ToProcQty);
                                                orderQty = Convert.ToDecimal(t.OrderQty.Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep));
                                                currentRecQty = recQty;
                                                itemFound = true;
                                                itemIndex = index;
                                            }
                                            index++;
                                        }

                                        if (itemFound == true)
                                        {
                                            decimal scanQty = Convert.ToDecimal(qty);
                                            newRecQty = currentRecQty + scanQty;
                                            if (newRecQty <= orderQty)
                                            {
                                                string poUpdated = SQLite.updatePOItemLot(itemCode, lot, Convert.ToString(newRecQty).Replace(",", "."));
                                                switch (poUpdated.Split('*')[0])
                                                {
                                                    case "1":
                                                        string unqUpdated = SQLite.updateUnqScanned(txtScanItem.Text, GlobalVar.OrderNum);
                                                        switch (unqUpdated.Split('*')[0])
                                                        {
                                                            case "1":
                                                                GlobalVar.CurrentOrderItem[itemIndex].ToProcQty = Convert.ToString(newRecQty);
                                                                if (GlobalVar.CurrentOrderItem[itemIndex].ToProcQty == GlobalVar.CurrentOrderItem[itemIndex].OrderQty)
                                                                {
                                                                    //GlobalVar.CurrentOrderItem[itemIndex].complete = BitmapFactory.DecodeResource(Resources, Resource.Drawable.found);
                                                                    GlobalVar.CurrentOrderItem[itemIndex].theme = Resource.Drawable.blackbordergreen;
                                                                }
                                                                else
                                                                {
                                                                    GlobalVar.CurrentOrderItem[itemIndex].theme = Resource.Drawable.blackborderblue;
                                                                }
                                                                RunOnUiThread(refreshList);
                                                                break;
                                                            case "-1":
                                                                ErrorText = poUpdated.Split('*')[1].ToUpper();
                                                                RunOnUiThread(ShowError);
                                                                break;
                                                        }
                                                        break;
                                                    case "-1":
                                                        ErrorText = poUpdated.Split('*')[1].ToUpper();
                                                        RunOnUiThread(ShowError);
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                ErrorText = "ORDER QTY WOULD BE EXCEEDED!";
                                                RunOnUiThread(ShowError);
                                            }
                                        }
                                        else
                                        {
                                            ErrorText = "ITEM NOT FOUND ON PO!";
                                            RunOnUiThread(ShowError);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region No Lot
                                        int index = 0;
                                        int itemIndex = 0;
                                        bool itemFound = false;
                                        decimal orderQty = 0;
                                        decimal currentRecQty = 0;
                                        decimal newRecQty = 0;
                                        foreach (GlobalVar.OrderDetail t in itemlist)
                                        {
                                            if (t.ItemCode == itemCode && Convert.ToBoolean(t.lotItem.Replace("1", "true").Replace("0", "false")) == false)
                                            {
                                                decimal recQty = Convert.ToDecimal(t.ToProcQty);
                                                orderQty = Convert.ToDecimal(t.OrderQty.Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep));
                                                currentRecQty = recQty;
                                                itemFound = true;
                                                itemIndex = index;
                                            }
                                            index++;
                                        }

                                        if (itemFound == true)
                                        {
                                            decimal scanQty = Convert.ToDecimal(qty);
                                            newRecQty = currentRecQty + scanQty;
                                            if (newRecQty <= orderQty)
                                            {
                                                string poUpdated = SQLite.updatePOItemNoLot(itemCode, Convert.ToString(newRecQty).Replace(",", "."));
                                                switch (poUpdated.Split('*')[0])
                                                {
                                                    case "1":
                                                        string unqUpdated = SQLite.updateUnqScanned(txtScanItem.Text, GlobalVar.OrderNum);
                                                        switch (unqUpdated.Split('*')[0])
                                                        {
                                                            case "1":
                                                                GlobalVar.CurrentOrderItem[itemIndex].ToProcQty = Convert.ToString(newRecQty);
                                                                if (GlobalVar.CurrentOrderItem[itemIndex].ToProcQty == GlobalVar.CurrentOrderItem[itemIndex].OrderQty)
                                                                {
                                                                    GlobalVar.CurrentOrderItem[itemIndex].theme = Resource.Drawable.blackbordergreen;
                                                                    //GlobalVar.CurrentOrderItem[itemIndex].complete = BitmapFactory.DecodeResource(Resources, Resource.Drawable.found);
                                                                }
                                                                else
                                                                {
                                                                    GlobalVar.CurrentOrderItem[itemIndex].theme = Resource.Drawable.blackborderblue;
                                                                }
                                                                RunOnUiThread(refreshList);
                                                                break;
                                                            case "-1":
                                                                ErrorText = poUpdated.Split('*')[1].ToUpper();
                                                                RunOnUiThread(ShowError);
                                                                break;
                                                        }
                                                        break;
                                                    case "-1":
                                                        ErrorText = poUpdated.Split('*')[1].ToUpper();
                                                        RunOnUiThread(ShowError);
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                ErrorText = "ITEM NOT FOUND ON PO!";
                                                RunOnUiThread(ShowError);
                                            }
                                        }
                                        else
                                        {
                                            ErrorText = "ORDER QTY WOULD BE EXCEEDED!";
                                            RunOnUiThread(ShowError);
                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    ErrorText = "ITEM ALREADY SCANNED!";
                                    RunOnUiThread(ShowError);
                                }                               
                                break;
                            case "-1":
                                ErrorText = barcodeFound.Split('*')[1].ToUpper();
                                RunOnUiThread(ShowError);
                                break;
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
                ErrorText = "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void refreshList()
        {
            DetailList.Adapter = new GlobalVar.OrderDetailAdapter(this, GlobalVar.CurrentOrderItem.ToArray());
            txtScanItem.Text = string.Empty;
        }
        private void BtnDone_Click(object sender, EventArgs e)
        {
            GlobalVar.pgsBusy = new ProgressDialog(this);
            GlobalVar.pgsBusy.Indeterminate = true;
            GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
            GlobalVar.pgsBusy.SetMessage("Updating Purchase Order Details, Depending on the size of the purchase order this may take a few minutes...");
            GlobalVar.pgsBusy.SetCancelable(false);
            GlobalVar.pgsBusy.Show();
            Thread t = new Thread(updatePOLines);
            t.Start();
        }
        public void updatePOLines()
        {
            try
            {
                bool failedUpdateds = false;
                List<GlobalVar.OrderDetail> itemlist = GlobalVar.CurrentOrderItem;
                foreach (GlobalVar.OrderDetail t in itemlist)
                {
                    if (Convert.ToDouble(t.ToProcQty) !=0)
                    {
                        string code = t.ItemCode;
                        string lot = t.ItemLotNum;
                        string qty = t.ToProcQty;

                        string updated = Client.postPOLineSingle(GlobalVar.OrderNum + "|" + code + "|" + lot + "|" + qty);
                        switch (updated.Split('*')[0])
                        {
                            case "1":
                                string allUnqs = SQLite.getUpdatedUnqs_ItemLot(code, lot);
                                switch (allUnqs.Split('*')[0])
                                {
                                    case "1":
                                        allUnqs = allUnqs.Remove(0, 2);
                                        string unqsUpdated = Client.postPOLineUnq(GlobalVar.OrderNum + "*" + allUnqs);
                                        switch (unqsUpdated.Split('*')[0])
                                        {
                                            case "1":

                                                break;
                                            case "-1":
                                                failedUpdateds = true;
                                                unqsUpdated = unqsUpdated.Remove(0, 3);
                                                failedLabels += (System.Environment.NewLine + System.Environment.NewLine + "Labels for item : " + code +  " with lot : " + lot + " could not be set to received " + System.Environment.NewLine + "Reason: " + System.Environment.NewLine + unqsUpdated);
                                                break;
                                            default:
                                                failedUpdateds = true;
                                                if (unqsUpdated == string.Empty)
                                                {
                                                    unqsUpdated = "no data eturned from server";
                                                }
                                                failedLabels += (System.Environment.NewLine + System.Environment.NewLine + "Labels for item: " + code + " with lot : " + lot + " could not be set to received " + System.Environment.NewLine + "Reason: " + System.Environment.NewLine + unqsUpdated);
                                                break;
                                        }
                                        break;
                                    case "-1":
                                        failedUpdateds = true;
                                        allUnqs = allUnqs.Remove(0, 3);
                                        failedLabels += (System.Environment.NewLine + System.Environment.NewLine + "Labels for item: " + code + "with lot: " + lot + "could not be set to received " + System.Environment.NewLine + "Reason: " + System.Environment.NewLine + allUnqs);
                                        break;
                                }
                                break;
                            case "-1":
                                failedUpdateds = true;
                                updated = updated.Remove(0, 3);
                                failedLines += (System.Environment.NewLine + System.Environment.NewLine + "Item:" + code + System.Environment.NewLine + "Lot: " + lot + System.Environment.NewLine + "Qty: " + qty + System.Environment.NewLine + "Reason: " + updated);
                                break;
                            default:
                                failedUpdateds = true;
                                if (updated == string.Empty)
                                {
                                    updated = "no data eturned from server";
                                }
                                failedLines += (System.Environment.NewLine + System.Environment.NewLine + "Item:" + code + System.Environment.NewLine + "Lot: " + lot + System.Environment.NewLine + "Qty: " + qty + System.Environment.NewLine + "Reason: " + updated);
                                break;
                        }                        
                    }
                }

                if (failedUpdateds == false)
                {
                    RunOnUiThread(showNext);
                }
                else
                {
                    if (failedLines != string.Empty)
                    {
                        failedLines = "The following items failed to update" + failedLines;
                    }

                    if (failedLabels != string.Empty)
                    {
                        failedLabels = "The following labels failed to update" + failedLabels;
                    }

                    WarningText = failedLines + System.Environment.NewLine + System.Environment.NewLine + failedLabels;
                    RunOnUiThread(ShowWarning);
                }

            }
            catch (Exception ex)
            {
                ErrorText = "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        //public void updatePOLines()
        //{
        //    try
        //    {
        //        string poLines = string.Empty;
        //        List<GlobalVar.OrderDetail> itemlist = GlobalVar.CurrentOrderItem;
        //        foreach (GlobalVar.OrderDetail t in itemlist)
        //        {
        //            if (Convert.ToDouble(t.ToProcQty) != 0)
        //            {
        //                poLines += t.ItemCode + "|" + t.ItemLotNum + "|" + t.ToProcQty + "~";
        //            }
        //        }

        //        string unqLines = SQLite.getUpdatedUnqs();
        //        switch (unqLines.Split('*')[0])
        //        {
        //            case "1":
        //                unqLines = unqLines.Remove(0, 2);
        //                string sendString = GlobalVar.OrderNum + "*" + poLines + "*" + unqLines;
        //                string linesPosted = Client.postPOLines(sendString);
        //                switch (linesPosted.Split('*')[0])
        //                {
        //                    case "1":
        //                        RunOnUiThread(showNext);
        //                        break;
        //                    case "-1":
        //                        ErrorText = linesPosted.Split('*')[1].ToUpper();
        //                        RunOnUiThread(ShowError);
        //                        break;
        //                    case "-2":
        //                        WarningText = linesPosted.Split('*')[1].ToUpper();
        //                        RunOnUiThread(ShowWarning);
        //                        break;
        //                }
        //                break;
        //            case "-1":
        //                ErrorText = unqLines.Split('*')[1].ToUpper();
        //                RunOnUiThread(ShowError);
        //                break;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorText = "Exception: " + ex.ToString();
        //        RunOnUiThread(ShowError);
        //    }
        //}

        public void showNext()
        {
            StartActivity(typeof(MainActivity));
            Finish();
        }
        private void SetOrderDetails()
        {
            int linCount = 0;
            try
            {
                GlobalVar.CurrentOrderItem = new List<GlobalVar.OrderDetail>();
                string[] Data = GlobalVar._orderdetails.Split('*');
                string Code = Data[0];
                string Details = Data[1];
                GlobalVar.CurrentOrderItem.Clear();
                if (Code != "")
                {
                    Bitmap complete = BitmapFactory.DecodeResource(Resources, Resource.Drawable.found);
                    Bitmap incomplete = BitmapFactory.DecodeResource(Resources, Resource.Drawable.notfound);
                    string[] temp = Details.Split('~');
                    foreach (string t in temp)
                    {
                        if (t != "")
                        {
                            linCount++;
                            string[] temp2 = t.Split('|');
                            GlobalVar.OrderDetail tempitem = new GlobalVar.OrderDetail();
                            tempitem.ItemCode = temp2[0];
                            tempitem.ItemDesc = temp2[1];
                            tempitem.ItemLotNum = temp2[2];
                            tempitem.OrderQty = temp2[3];
                            //tempitem.ProcQty = "0"; //temp2[4];
                            tempitem.ProcQty = temp2[4];
                            tempitem.ToProcQty = "0"; //temp2[4];
                            tempitem.lotItem = temp2[5];
                            decimal RecQty = Convert.ToDecimal(tempitem.ProcQty) + Convert.ToDecimal(tempitem.ToProcQty);
                            tempitem.TotalRecQty = Convert.ToString(RecQty);
                            if (Convert.ToDecimal(tempitem.OrderQty) == Convert.ToDecimal(tempitem.ToProcQty))
                            {
                                //tempitem.complete = complete; //BitmapFactory.DecodeResource(Resources, Resource.Drawable.found);
                                tempitem.theme = Resource.Drawable.blackbordergreen;
                            }
                            else
                            {
                                //tempitem.complete = incomplete; //BitmapFactory.DecodeResource(Resources, Resource.Drawable.notfound);
                                tempitem.theme = Resource.Drawable.blackborder;
                            }

                            if (tempitem.OrderQty != "0")
                            {
                                bool testLot = Convert.ToBoolean(tempitem.lotItem.Replace("1", "true").Replace("0", "false"));
                                if (Convert.ToBoolean(testLot) == true && tempitem.ItemLotNum != string.Empty)
                                {
                                    GlobalVar.CurrentOrderItem.Add(tempitem);
                                }
                                else if (Convert.ToBoolean(testLot) == false)
                                {
                                    GlobalVar.CurrentOrderItem.Add(tempitem);
                                }                                
                            }
                            
                        }
                    }
                    DetailList.Adapter = new GlobalVar.OrderDetailAdapter(this, GlobalVar.CurrentOrderItem.ToArray());
                }
                else
                {
                    GlobalVar.pgsBusy.Dismiss();
                    Finish();
                }

                GlobalVar.pgsBusy.Dismiss();
            }
            catch (Exception ex)
            {
                GlobalVar.pgsBusy.Dismiss();
                ErrorText = "FAILED TO LOAD DETAIL TO LIST ADAPTER: " + ex.ToString();
                RunOnUiThread(ShowError);
                
            }
        }
        private void DetailList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

        }
        protected override void OnResume()
        {
            base.OnResume();
            UpdateList();
        }
        private void UpdateList()
        {
            DetailList.Adapter = new GlobalVar.OrderDetailAdapter(this, GlobalVar.CurrentOrderItem.ToArray());
            RunOnUiThread(() =>
            {
                ((GlobalVar.OrderDetailAdapter)DetailList.Adapter).NotifyDataSetChanged();
            });
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
            DetailList.Adapter = new GlobalVar.OrderDetailAdapter(this, GlobalVar.CurrentOrderItem.ToArray());
        }

        private void ShowWarning()
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

            SetContentView(Resource.Layout.Z_Warning);
            lblWarningMsg = FindViewById<TextView>(Resource.Id.lblErrorMsg);
            lblWarningMsg.Text = WarningText;
            
            btnWarningOk = FindViewById<Button>(Resource.Id.btnErrorOk);
            btnWarningOk.SetOnTouchListener(this);
            btnWarningOk.Click += BtnWarningOk_Click;
        }

        private void BtnWarningOk_Click(object sender, EventArgs e)
        {
            RunOnUiThread(showNext);
            //SetContentView(Resource.Layout.D_ScanItem);

            //txtScanItem = FindViewById<EditText>(Resource.Id.txtItem);
            //txtScanItem.KeyPress += TxtScanItem_KeyPress;
            //txtScanItem.RequestFocus();

            //DetailList = FindViewById<ListView>(Resource.Id.POlist);

            //btnDone = FindViewById<Button>(Resource.Id.btnDone);
            //btnDone.Click += BtnDone_Click;
            //btnDone.SetOnTouchListener(this);

            //btnBack = FindViewById<Button>(Resource.Id.btnBack);
            //btnBack.Click += BtnBack_Click;
            //btnBack.SetOnTouchListener(this);

            //ScanEngineOn();
            //#region Wifi / Battery / Error / Version

            //#region Version Control

            //lblVersion = FindViewById<TextView>(Resource.Id.lblVersion);
            //if (GlobalVar.UserPin == null)
            //{
            //    lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName + System.Environment.NewLine + GlobalVar.ScannerID;
            //}
            //else
            //{
            //    lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName + System.Environment.NewLine + GlobalVar.UserName;
            //}

            //#endregion

            //_player = MediaPlayer.Create(this, Resource.Drawable.beeperror);
            //imgBat = FindViewById<ImageView>(Resource.Id.imgBat);
            //imgBat.Click += ImgBat_Click;
            //imgWifi = FindViewById<ImageView>(Resource.Id.imgWifi);
            //BatteryLevel();
            //WifiSignal();

            //#endregion
            //DetailList.Adapter = new GlobalVar.OrderDetailAdapter(this, GlobalVar.CurrentOrderItem.ToArray());
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