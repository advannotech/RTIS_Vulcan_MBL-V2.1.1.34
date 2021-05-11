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

namespace RTIS_Vulcan_MBL.Activities.J_Stock_Take
{
    [Activity(Label = "J_ScanItem")]
    public class J_ScanItem : Activity, View.IOnTouchListener
    {
        //E_Scan_Dry_Weight

        #region Variables

        TextView lblHeader1;
        TextView lblHeader2;
        EditText txtBCD;
        Button btnBack;

        private enum addState
        {
            None,
            AddLot,
            AddItem
        }

        private addState currentState = addState.None;

        #endregion

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
        Button btnWarningYes;
        Button btnWarningNo;

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

            txtBCD.Text = "";
            _player.Start();

            SetContentView(Resource.Layout.Z_YesNo);
            lblWarningMsg = FindViewById<TextView>(Resource.Id.lblErrorMsg);
            lblWarningMsg.Text = WarningText;

            btnWarningYes = FindViewById<Button>(Resource.Id.btnWarningYes);
            btnWarningYes.SetOnTouchListener(this);
            btnWarningYes.Click += BtnWarningYes_Click;

            btnWarningNo = FindViewById<Button>(Resource.Id.btnWarningNo);
            btnWarningNo.SetOnTouchListener(this);
            btnWarningNo.Click += BtnWarningNo_Click;
        }

        private void BtnWarningNo_Click(object sender, EventArgs e)
        {
            try
            {
                currentState = addState.None;
                StartActivity(typeof(J_ScanItem));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception E101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnWarningYes_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentState == addState.AddItem)
                {
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Adding item to stock take...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(addItemToST);
                    t.Start();
                }
                else if (currentState == addState.AddLot)
                {
                    ConfirmAddPGMInvestigation();
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception E101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnWarningYes_Powder_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentState == addState.AddItem)
                {
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Adding item to stock take...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(AddItemToStPowder);
                    t.Start();
                }
                else if (currentState == addState.AddLot)
                {
                    ScanPowderQtyInvestigation();
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception E101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        #region Item not on stock take

        private void addItemToST()
        {
            try
            {
                string added = Client.AddItemToStockTake(GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" +
                                                         GlobalVar.STItem + "|" + GlobalVar.STLot);
                switch (added.Split('*')[0])
                {
                    case "1":
                        RunOnUiThread(showScanTicket);
                        break;
                    case "0":
                        ErrorText = "Exception J001: " + System.Environment.NewLine + added.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    case "-1":
                        ErrorText = "Exception J002: " + System.Environment.NewLine + added.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                    "Unexpected error while retreiving data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception E101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void AddItemToStPowder()
        {
            try
            {
                string added = Client.AddItemToStockTake(GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" +
                                                         GlobalVar.STItem + "|" + GlobalVar.STLot);
                switch (added.Split('*')[0])
                {
                    case "1":
                        RunOnUiThread(showNextPowder);
                        break;
                    case "0":
                        ErrorText = "Exception J001: " + System.Environment.NewLine + added.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    case "-1":
                        ErrorText = "Exception J002: " + System.Environment.NewLine + added.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                    "Unexpected error while retreiving data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception E101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        #endregion

        #region Lot not in evolution

        public void ConfirmAddPGMInvestigation()
        {
            try
            {
                ScanEngineOff();
                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                Confirm.SetTitle("Confirm Item Investigation Information");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage("Item Code : " + GlobalVar.STItem + System.Environment.NewLine + "Lot Number : " +
                                   GlobalVar.STLot + System.Environment.NewLine + "Item Quantity : " +
                                   GlobalVar.STQty); //Solution Weight:
                Confirm.SetButton("YES", (s, ev) =>
                {
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Adding lot for investigation...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(AddlotForInvestigation);
                    t.Start();
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    try
                    {
                        if (GlobalVar.pgsBusy.IsShowing)
                        {
                            GlobalVar.pgsBusy.Dismiss();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    ScanEngineOn();
                    txtBCD.Text = string.Empty;
                    txtBCD.RequestFocus();
                });
                Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void ScanPowderQtyInvestigation()
        {
            try
            {
                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                EditText et = new EditText(this);
                et.Gravity = GravityFlags.Center;
                Confirm.SetTitle("Please Scan The Qty Barcode");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage("Please Scan The Powder QTy Barcode");
                Confirm.SetView(et);
                Confirm.SetButton("Back", (s, ev) =>
                {
                    Confirm.Dismiss();
                });
                
                et.KeyPress += (s, ev) =>
                {
                    
                    if (ev.KeyCode == Keycode.Enter && ev.Event.Action == KeyEventActions.Down)
                {
                    Confirm.Dismiss();
                    double dQty = Convert.ToDouble(et.Text.Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep)) / 10000;
                    GlobalVar.STQty = Convert.ToString(dQty);
                    ScanEngineOff();
                    AlertDialog.Builder build2 = new AlertDialog.Builder(this);
                    AlertDialog Confirm2 = build2.Create();
                    Confirm2.SetTitle("Confirm Item Investigation Information");
                    Confirm2.SetIcon(Resource.Drawable.Icon);
                    Confirm2.SetMessage("Item Code : " + GlobalVar.STItem + System.Environment.NewLine + "Lot Number : " +
                                       GlobalVar.STLot + System.Environment.NewLine + "Item Quantity : " +
                                       GlobalVar.STQty); //Solution Weight:
                    Confirm2.SetButton("YES", (s2, ev2) =>
                    {
                        GlobalVar.pgsBusy = new ProgressDialog(this);
                        GlobalVar.pgsBusy.Indeterminate = true;
                        GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                        GlobalVar.pgsBusy.SetMessage("Adding lot for investigation...");
                        GlobalVar.pgsBusy.SetCancelable(false);
                        GlobalVar.pgsBusy.Show();
                        Thread t = new Thread(AddlotForInvestigation);
                        t.Start();
                    });
                    Confirm2.SetButton2("NO", (s2, ev2) =>
                    {
                        try
                        {
                            if (GlobalVar.pgsBusy.IsShowing)
                            {
                                GlobalVar.pgsBusy.Dismiss();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        ScanEngineOn();
                        txtBCD.Text = string.Empty;
                        txtBCD.RequestFocus();
                    });
                    Confirm2.Show();
                }
                else
                {
                    ev.Handled = false;
                }
                };
                Confirm.Show();
            }
            catch (Exception ex)
            {
                
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void Et_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void AddlotForInvestigation()
        {
            try
            {
                string added = Client.AddLotForInvestigation(GlobalVar.StockTake + "|" + GlobalVar.STItem + "|" +
                                                             GlobalVar.STLot + "|" + GlobalVar.STQty + "|" +
                                                             GlobalVar.UserName);
                switch (added.Split('*')[0])
                {
                    case "1":
                        SuccessText = "Item logged for investigation.";
                        RunOnUiThread(showSuccess);
                        break;
                    case "0":
                        ErrorText = "Exception J001: " + System.Environment.NewLine + added.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    case "-1":
                        ErrorText = "Exception J002: " + System.Environment.NewLine + added.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                    "Unexpected error while retrieving data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        #endregion

        #endregion

        #region Warning Powder
        private void ShowWarning_Powder()
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

            txtBCD.Text = "";
            _player.Start();

            SetContentView(Resource.Layout.Z_YesNo);
            lblWarningMsg = FindViewById<TextView>(Resource.Id.lblErrorMsg);
            lblWarningMsg.Text = WarningText;

            btnWarningYes = FindViewById<Button>(Resource.Id.btnWarningYes);
            btnWarningYes.SetOnTouchListener(this);
            btnWarningYes.Click += BtnWarningYes_Powder_Click;

            btnWarningNo = FindViewById<Button>(Resource.Id.btnWarningNo);
            btnWarningNo.SetOnTouchListener(this);
            btnWarningNo.Click += BtnWarningNo_Click;
        }
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
            currentState = addState.None;
            StartActivity(typeof(J_ScanItem));
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
            SetContentView(Resource.Layout.E_Scan_Dry_Weight);
            layoutStichUp();
            ScanEngineOn();

            #region Wifi / Battery / Error / Version

            #region Version Control

            lblVersion = FindViewById<TextView>(Resource.Id.lblVersion);
            if (GlobalVar.UserPin == null)
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." +
                                  this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName +
                                  System.Environment.NewLine + GlobalVar.ScannerID;
            }
            else
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." +
                                  this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName +
                                  System.Environment.NewLine + GlobalVar.UserName;
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
                lblHeader1.Text = "Stock Take";
                lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                lblHeader2.Text = "Scan Item";
                txtBCD = FindViewById<EditText>(Resource.Id.txtBCD);
                txtBCD.KeyPress += TxtBCD_KeyPress;
                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;
                txtBCD.RequestFocus();
                //txtBCD.Text =
                //    "TPP-920$190520-001";
            }
            catch (Exception ex)
            {
                ErrorText = "Exception E101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void TxtBCD_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    if (txtBCD.Text.Contains("(") && txtBCD.Text.Contains(")"))
                    {
                        GlobalVar.STItem = Barcodes.GetItemCode(txtBCD.Text);
                        GlobalVar.STLot = Barcodes.GetItemLot(txtBCD.Text);
                        GlobalVar.STQty = Barcodes.GetItemQty(txtBCD.Text);
                        GlobalVar.STUnq = Barcodes.GetUniqCode(txtBCD.Text);

                        GlobalVar.pgsBusy = new ProgressDialog(this);
                        GlobalVar.pgsBusy.Indeterminate = true;
                        GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                        GlobalVar.pgsBusy.SetMessage("Retrieving Item Information...");
                        GlobalVar.pgsBusy.SetCancelable(false);
                        GlobalVar.pgsBusy.Show();

                        if (GlobalVar.STUnq.Substring(0, 1) == "P")
                        {
                            //Pallet
                            if (GlobalVar.STLot == "#PALLET#")
                            {
                                //RM Pallet
                                GlobalVar.currentSTBarcode = GlobalVar.STBarcodeType.Pallet;
                                Thread t = new Thread(getItemInfoRMPallet);
                                t.Start();
                            }
                            else
                            {
                                //FG Pallet
                                GlobalVar.currentSTBarcode = GlobalVar.STBarcodeType.Pallet;
                                Thread t = new Thread(getItemInfoPallet);
                                t.Start();
                            }
                        }
                        else
                        {
                            //Box
                            GlobalVar.currentSTBarcode = GlobalVar.STBarcodeType.RT2D;
                            Thread t = new Thread(getItemInfoRT2D);
                            t.Start();
                        }
                    }
                    else if (txtBCD.Text.Substring(0, 4) == "TRO_" && txtBCD.Text.Contains("$"))
                    {
                        //Fresh Slurry
                        GlobalVar.currentSTBarcode = GlobalVar.STBarcodeType.freshSlurry;
                        string barcode = txtBCD.Text;
                        barcode = barcode.Substring(4, barcode.Length - 4);
                        GlobalVar.STUnq = barcode.Split('$')[0];
                        GlobalVar.STItem = barcode.Split('$')[1];

                        GlobalVar.pgsBusy = new ProgressDialog(this);
                        GlobalVar.pgsBusy.Indeterminate = true;
                        GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                        GlobalVar.pgsBusy.SetMessage("Getting Fresh Slurry Information...");
                        GlobalVar.pgsBusy.SetCancelable(false);
                        GlobalVar.pgsBusy.Show();
                        Thread t = new Thread(getFreshSlurryInfo);
                        t.Start();
                    }
                    else if (txtBCD.Text.Substring(0, 4) == "TNK_" && txtBCD.Text.Contains("$"))
                    {
                        //Mixed Slurry Tank
                        GlobalVar.currentSTBarcode = GlobalVar.STBarcodeType.MixedSlurryT;
                        string barcode = txtBCD.Text;
                        barcode = barcode.Substring(4, barcode.Length - 4);
                        GlobalVar.STUnq = barcode.Split('$')[0];
                        GlobalVar.STItem = barcode.Split('$')[1];

                        GlobalVar.pgsBusy = new ProgressDialog(this);
                        GlobalVar.pgsBusy.Indeterminate = true;
                        GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                        GlobalVar.pgsBusy.SetMessage("Getting Mixed Slurry Information...");
                        GlobalVar.pgsBusy.SetCancelable(false);
                        GlobalVar.pgsBusy.Show();
                        Thread t = new Thread(getMixedSlurryTankInfo);
                        t.Start();
                    }
                    else if (txtBCD.Text.Substring(0, 5) == "MTNK_" && txtBCD.Text.Contains("$"))
                    {
                        //Mixed Slurry Mobile Tank
                        GlobalVar.currentSTBarcode = GlobalVar.STBarcodeType.MixedSlurryM;
                        string barcode = txtBCD.Text;
                        barcode = barcode.Substring(5, barcode.Length - 5);
                        GlobalVar.STUnq = barcode.Split('$')[0];
                        GlobalVar.STItem = barcode.Split('$')[1];

                        GlobalVar.pgsBusy = new ProgressDialog(this);
                        GlobalVar.pgsBusy.Indeterminate = true;
                        GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                        GlobalVar.pgsBusy.SetMessage("Getting Mixed Slurry Information...");
                        GlobalVar.pgsBusy.SetCancelable(false);
                        GlobalVar.pgsBusy.Show();
                        Thread t = new Thread(getMixedSlurryMobileTankInfo);
                        t.Start();
                    }
                    else if (txtBCD.Text.Substring(0, 5) == "BTNK_" && txtBCD.Text.Contains("$"))
                    {
                        //Mixed Slurry Buffer Tank 
                        ErrorText = "Exception E101: " + "You may not scan buffer tanks durring a stock take!";
                        RunOnUiThread(ShowError);
                    }
                    else if (txtBCD.Text.Substring(0, 5) == "CONT_" && txtBCD.Text.Contains("$"))
                    {
                        //PGM Container 
                        GlobalVar.currentSTBarcode = GlobalVar.STBarcodeType.PGM;
                        string barcode = txtBCD.Text;
                        barcode = barcode.Substring(5, barcode.Length - 5);
                        GlobalVar.STUnq = barcode.Split('$')[0];
                        GlobalVar.STItem = barcode.Split('$')[1];

                        GlobalVar.pgsBusy = new ProgressDialog(this);
                        GlobalVar.pgsBusy.Indeterminate = true;
                        GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                        GlobalVar.pgsBusy.SetMessage("Getting PGM Information...");
                        GlobalVar.pgsBusy.SetCancelable(false);
                        GlobalVar.pgsBusy.Show();
                        Thread t = new Thread(getPGMContainerInfo);
                        t.Start();
                    }
                    else if (txtBCD.Text.Contains('$'))
                    {
                        //Powder barcode
                        GlobalVar.currentSTBarcode = GlobalVar.STBarcodeType.powder;
                        string barcode = txtBCD.Text;
                        GlobalVar.STUnq = barcode;
                        GlobalVar.STItem = "TPP-" + barcode.Split('$')[0];

                        if (barcode.Split('$')[1].Contains('-'))
                        {
                            GlobalVar.STLot = barcode.Split('$')[1];
                        }
                        else
                        {
                            //string testLotP1 = barcode.Split('$')[1].Substring(0, 6);
                            //string testLotP2 = barcode.Split('$')[1].Substring(7, 3);
                            //GlobalVar.STLot = testLotP1 + "-" + testLotP2;

                            string testLotP1 = txtBCD.Text.Split('$')[1].Substring(0, 6);
                            string testLotP2 = txtBCD.Text.Split('$')[1].Substring(6);
                            GlobalVar.STLot = testLotP1 + "-" + testLotP2;
                        }

                        GlobalVar.pgsBusy = new ProgressDialog(this);
                        GlobalVar.pgsBusy.Indeterminate = true;
                        GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                        GlobalVar.pgsBusy.SetMessage("Checking Powder...");
                        GlobalVar.pgsBusy.SetCancelable(false);
                        GlobalVar.pgsBusy.Show();
                        Thread t = new Thread(getPowderOnST);
                        t.Start();
                    }
                    else
                    {
                        //Manual Scan
                        GlobalVar.currentSTBarcode = GlobalVar.STBarcodeType.manual;
                        GlobalVar.STItem = txtBCD.Text;
                        StartActivity(typeof(J_ScanEnterLot));
                        Finish();
                    }
                }
                else
                {
                    e.Handled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception E101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void getItemInfoRT2D()
        {
            try
            {
                string lotExists = Client.GetLotInEvo(GlobalVar.STItem, GlobalVar.STLot);
                switch (lotExists.Split('*')[0])
                {
                    case "1":
                        string onST = Client.GetItemOnSTockTake(
                            GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" + GlobalVar.STItem + "|" +
                            GlobalVar.STLot);
                        switch (onST.Split('*')[0])
                        {
                            case "1":
                                currentState = addState.None;
                                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                                string itemChecked = Client.CheckItemRT2D(
                                    GlobalVar.StockTake + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" +
                                    GlobalVar.STQty + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" +
                                    scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" +
                                    Convert.ToString(GlobalVar.recountST));
                                if (itemChecked != string.Empty)
                                {
                                    switch (itemChecked.Split('*')[0])
                                    {
                                        case "1":
                                            RunOnUiThread(showScanTicket);
                                            break;
                                        case "0":
                                            ErrorText = "Exception J001: " + System.Environment.NewLine +
                                                        itemChecked.Split('*')[1];
                                            RunOnUiThread(ShowError);
                                            break;
                                        case "-1":
                                            ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                        itemChecked.Split('*')[1];
                                            RunOnUiThread(ShowError);
                                            break;
                                        default:
                                            ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                        "Unexpected error while retreiving data";
                                            RunOnUiThread(ShowError);
                                            break;
                                    }
                                }
                                else
                                {
                                    ErrorText = "Exception J004: " + System.Environment.NewLine +
                                                "No data was returned from the server";
                                    RunOnUiThread(ShowError);
                                }

                                break;
                            case "0":
                                //Warning
                                currentState = addState.AddItem;
                                WarningText =
                                    "Item was not found on the stock take for this warehouse, would you like to add it to the stock take?";
                                RunOnUiThread(ShowWarning);
                                break;
                            case "-1":
                                ErrorText = "Exception J002: " + System.Environment.NewLine + onST.Split('*')[1];
                                RunOnUiThread(ShowError);
                                break;
                            default:
                                ErrorText = "Exception J003: " + System.Environment.NewLine +
                                            "Unexpected error while retreiving data";
                                RunOnUiThread(ShowError);
                                break;
                        }

                        break;
                    case "0":
                        currentState = addState.AddLot;
                        WarningText =
                            "The lot number was not found in evolution, would you like to add this item for investigation?";
                        RunOnUiThread(ShowWarning);
                        break;
                    case "-1":
                        ErrorText = "Exception J002: " + System.Environment.NewLine + lotExists.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                    "Unexpected error while retreiving data";
                        RunOnUiThread(ShowError);
                        break;
                }


                //string onST = Client.GetItemOnSTockTake(GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot);
                //switch (onST.Split('*')[0])
                //{
                //    case "1":
                //        string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                //        string itemChecked = Client.CheckItemRT2D(GlobalVar.StockTake + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" + GlobalVar.STQty + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" + Convert.ToString(GlobalVar.recountST));
                //        if (itemChecked != string.Empty)
                //        {
                //            switch (itemChecked.Split('*')[0])
                //            {
                //                case "1":
                //                    RunOnUiThread(showScanTicket);
                //                    break;
                //                case "0":
                //                    ErrorText = "Exception J001: " + System.Environment.NewLine + itemChecked.Split('*')[1];
                //                    RunOnUiThread(ShowError);
                //                    break;
                //                case "-1":
                //                    ErrorText = "Exception J002: " + System.Environment.NewLine + itemChecked.Split('*')[1];
                //                    RunOnUiThread(ShowError);
                //                    break;
                //                default:
                //                    ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                //                    RunOnUiThread(ShowError);
                //                    break;
                //            }
                //        }
                //        else
                //        {
                //            ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                //            RunOnUiThread(ShowError);
                //        }
                //        break;
                //    case "0":
                //        //Warning
                //        WarningText = "Item was not found on the stock take for this warehouse, would you like to add it to the stock take?";
                //        RunOnUiThread(ShowWarning);
                //        break;
                //    case "-1":
                //        ErrorText = "Exception J002: " + System.Environment.NewLine + onST.Split('*')[1];
                //        RunOnUiThread(ShowError);
                //        break;
                //    default:
                //        ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                //        RunOnUiThread(ShowError);
                //        break;
                //}           
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void getItemInfoPallet()
        {
            try
            {
                string lotExists = Client.GetLotInEvo(GlobalVar.STItem, GlobalVar.STLot);
                switch (lotExists.Split('*')[0])
                {
                    case "1":
                        string onST = Client.GetItemOnSTockTake(
                            GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" + GlobalVar.STItem + "|" +
                            GlobalVar.STLot);
                        switch (onST.Split('*')[0])
                        {
                            case "1":
                                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                                string itemChecked = Client.CheckItemRT2Pallet(
                                    GlobalVar.StockTake + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" +
                                    GlobalVar.STQty + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" +
                                    scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" +
                                    Convert.ToString(GlobalVar.recountST));
                                if (itemChecked != string.Empty)
                                {
                                    switch (itemChecked.Split('*')[0])
                                    {
                                        case "1":
                                            RunOnUiThread(showScanTicket);
                                            break;
                                        case "0":
                                            ErrorText = "Exception J001: " + System.Environment.NewLine +
                                                        itemChecked.Split('*')[1];
                                            RunOnUiThread(ShowError);
                                            break;
                                        case "-1":
                                            ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                        itemChecked.Split('*')[1];
                                            RunOnUiThread(ShowError);
                                            break;
                                        default:
                                            ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                        "Unexpected error while retreiving data";
                                            RunOnUiThread(ShowError);
                                            break;
                                    }
                                }
                                else
                                {
                                    ErrorText = "Exception J004: " + System.Environment.NewLine +
                                                "No data was returned from the server";
                                    RunOnUiThread(ShowError);
                                }

                                break;
                            case "0":
                                //Warning
                                WarningText =
                                    "Item was not found on the stock take for this warhouse, would you like to add it to the stock take?";
                                RunOnUiThread(ShowWarning);
                                break;
                            case "-1":
                                ErrorText = "Exception J002: " + System.Environment.NewLine + onST.Split('*')[1];
                                RunOnUiThread(ShowError);
                                break;
                            default:
                                ErrorText = "Exception J003: " + System.Environment.NewLine +
                                            "Unexpected error while retreiving data";
                                RunOnUiThread(ShowError);
                                break;
                        }

                        break;
                    case "0":
                        currentState = addState.AddLot;
                        WarningText =
                            "The lot number was not found in evolution, would you like to add this item for investigation?";
                        RunOnUiThread(ShowWarning);
                        break;
                    case "-1":
                        ErrorText = "Exception J002: " + System.Environment.NewLine + lotExists.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                    "Unexpected error while retreiving data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void getItemInfoRMPallet()
        {
            try
            {
                string lotExists = Client.GetLotInEvo(GlobalVar.STItem, GlobalVar.STLot);
                switch (lotExists.Split('*')[0])
                {
                    case "1":
                        string onST = Client.GetItemOnSTockTake(
                            GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" + GlobalVar.STItem + "|" +
                            GlobalVar.STLot);
                        switch (onST.Split('*')[0])
                        {
                            case "1":
                                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                                string itemChecked = Client.CheckItemRT2RMPallet(
                                    GlobalVar.StockTake + "|" + GlobalVar.STItem + "|" + GlobalVar.STQty + "|" +
                                    GlobalVar.STUnq + "|" +
                                    GlobalVar.STWarehouse + "|" + scannerNo + "|" +
                                    Convert.ToString(GlobalVar.singleST) + "|" +
                                    Convert.ToString(GlobalVar.recountST));
                                if (itemChecked != string.Empty)
                                {
                                    switch (itemChecked.Split('*')[0])
                                    {
                                        case "1":
                                            RunOnUiThread(showScanTicket);
                                            break;
                                        case "0":
                                            ErrorText = "Exception J001: " + System.Environment.NewLine +
                                                        itemChecked.Split('*')[1];
                                            RunOnUiThread(ShowError);
                                            break;
                                        case "-1":
                                            ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                        itemChecked.Split('*')[1];
                                            RunOnUiThread(ShowError);
                                            break;
                                        default:
                                            ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                        "Unexpected error while retreiving data";
                                            RunOnUiThread(ShowError);
                                            break;
                                    }
                                }
                                else
                                {
                                    ErrorText = "Exception J004: " + System.Environment.NewLine +
                                                "No data was returned from the server";
                                    RunOnUiThread(ShowError);
                                }

                                break;
                            case "0":
                                //Warning
                                WarningText =
                                    "Item was not found on the stock take for this warhouse, would you like to add it to the stock take?";
                                RunOnUiThread(ShowWarning);
                                break;
                            case "-1":
                                ErrorText = "Exception J002: " + System.Environment.NewLine + onST.Split('*')[1];
                                RunOnUiThread(ShowError);
                                break;
                            default:
                                ErrorText = "Exception J003: " + System.Environment.NewLine +
                                            "Unexpected error while retreiving data";
                                RunOnUiThread(ShowError);
                                break;
                        }

                        break;
                    case "0":
                        currentState = addState.AddLot;
                        WarningText =
                            "The lot number was not found in evolution, would you like to add this item for investigation?";
                        RunOnUiThread(ShowWarning);
                        break;
                    case "-1":
                        ErrorText = "Exception J002: " + System.Environment.NewLine + lotExists.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                    "Unexpected error while retreiving data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void getFreshSlurryInfo()
        {
            try
            {
                string slurryCheckLot = Client.GetFreshSlurryInfo_LotCheck(GlobalVar.STItem + "|" + GlobalVar.STUnq);
                switch (slurryCheckLot.Split('*')[0])
                {
                    case "1":
                        slurryCheckLot = slurryCheckLot.Remove(0, 2);
                        GlobalVar.STLot = slurryCheckLot.Split('|')[0];
                        GlobalVar.STQty = slurryCheckLot.Split('|')[1];
                        string lotExists = Client.GetLotInEvo(GlobalVar.STItem, GlobalVar.STLot);
                        switch (lotExists.Split('*')[0])
                        {
                            case "1":
                                string onST = Client.GetItemOnSTockTake(
                                    GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" +
                                    GlobalVar.STItem + "|" +
                                    GlobalVar.STLot);
                                switch (onST.Split('*')[0])
                                {
                                    case "1":
                                        string scannerNo =
                                            new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                                        string freshSLurryChecked = Client.GetFreshSlurryInfo_ST(
                                            GlobalVar.StockTake + "|" + GlobalVar.STItem + "|" + GlobalVar.STUnq + "|" +
                                            GlobalVar.STWarehouse +
                                            "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" +
                                            Convert.ToString(GlobalVar.recountST));
                                        if (freshSLurryChecked != string.Empty)
                                        {
                                            switch (freshSLurryChecked.Split('*')[0])
                                            {
                                                case "1":
                                                    freshSLurryChecked = freshSLurryChecked.Remove(0, 2);
                                                    GlobalVar.STLot = freshSLurryChecked.Split('|')[0];
                                                    GlobalVar.STQty = freshSLurryChecked.Split('|')[1];
                                                    RunOnUiThread(confirmSlurry);
                                                    break;
                                                case "0":
                                                    ErrorText = "Exception J001: " + System.Environment.NewLine +
                                                                freshSLurryChecked.Split('*')[1];
                                                    RunOnUiThread(ShowError);
                                                    break;
                                                case "-1":
                                                    ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                                freshSLurryChecked.Split('*')[1];
                                                    RunOnUiThread(ShowError);
                                                    break;
                                                default:
                                                    ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                                "Unexpected error while retreiving data";
                                                    RunOnUiThread(ShowError);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            ErrorText = "Exception J004: " + System.Environment.NewLine +
                                                        "No data was returned from the server";
                                            RunOnUiThread(ShowError);
                                        }

                                        break;
                                    case "0":
                                        //Warning
                                        currentState = addState.AddItem;
                                        WarningText =
                                            "Item was not found on the stock take for this warehouse, would you like to add it to the stock take?";
                                        RunOnUiThread(ShowWarning);
                                        break;
                                    case "-1":
                                        ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                    onST.Split('*')[1];
                                        RunOnUiThread(ShowError);
                                        break;
                                    default:
                                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                    "Unexpected error while retreiving data";
                                        RunOnUiThread(ShowError);
                                        break;
                                }

                                break;
                            case "0":
                                currentState = addState.AddLot;
                                WarningText =
                                    "The lot number was not found in evolution, would you like to add this item for investigation?";
                                RunOnUiThread(ShowWarning);
                                break;
                            case "-1":
                                ErrorText = "Exception J002: " + System.Environment.NewLine + lotExists.Split('*')[1];
                                RunOnUiThread(ShowError);
                                break;
                            default:
                                ErrorText = "Exception J003: " + System.Environment.NewLine +
                                            "Unexpected error while retreiving data";
                                RunOnUiThread(ShowError);
                                break;
                        }

                        break;
                    case "0":
                        ErrorText = "Exception J001: " + System.Environment.NewLine +
                                    slurryCheckLot.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    case "-1":
                        ErrorText = "Exception J002: " + System.Environment.NewLine +
                                    slurryCheckLot.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                    "Unexpected error while retreiving data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void getMixedSlurryTankInfo()
        {
            try
            {
                string slurryCheckLot =
                    Client.GetMixedSlurryTankInfo_CheckLot(GlobalVar.STItem + "|" + GlobalVar.STUnq);
                switch (slurryCheckLot.Split('*')[0])
                {
                    case "1":
                        slurryCheckLot = slurryCheckLot.Remove(0, 2);
                        GlobalVar.STLot = slurryCheckLot.Split('|')[0];
                        GlobalVar.STQty = slurryCheckLot.Split('|')[1];
                        string lotExists = Client.GetLotInEvo(GlobalVar.STItem, GlobalVar.STLot);
                        switch (lotExists.Split('*')[0])
                        {
                            case "1":
                                string onST = Client.GetItemOnSTockTake(
                                    GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" +
                                    GlobalVar.STItem + "|" +
                                    GlobalVar.STLot);
                                switch (onST.Split('*')[0])
                                {
                                    case "1":
                                        string scannerNo =
                                            new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                                        string freshSLurryChecked = Client.GetMixedSLurryTankInfo(
                                            GlobalVar.StockTake + "|" + GlobalVar.STItem + "|" + GlobalVar.STUnq + "|" +
                                            GlobalVar.STWarehouse +
                                            "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" +
                                            Convert.ToString(GlobalVar.recountST));
                                        if (freshSLurryChecked != string.Empty)
                                        {
                                            switch (freshSLurryChecked.Split('*')[0])
                                            {
                                                case "1":
                                                    freshSLurryChecked = freshSLurryChecked.Remove(0, 2);
                                                    GlobalVar.STLot = freshSLurryChecked.Split('|')[0];
                                                    GlobalVar.STQty = freshSLurryChecked.Split('|')[1];
                                                    RunOnUiThread(confirmSlurry);
                                                    break;
                                                case "0":
                                                    ErrorText = "Exception J001: " + System.Environment.NewLine +
                                                                freshSLurryChecked.Split('*')[1];
                                                    RunOnUiThread(ShowError);
                                                    break;
                                                case "-1":
                                                    ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                                freshSLurryChecked.Split('*')[1];
                                                    RunOnUiThread(ShowError);
                                                    break;
                                                default:
                                                    ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                                "Unexpected error while retreiving data";
                                                    RunOnUiThread(ShowError);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            ErrorText = "Exception J004: " + System.Environment.NewLine +
                                                        "No data was returned from the server";
                                            RunOnUiThread(ShowError);
                                        }

                                        break;
                                    case "0":
                                        //Warning
                                        currentState = addState.AddItem;
                                        WarningText =
                                            "Item was not found on the stock take for this warehouse, would you like to add it to the stock take?";
                                        RunOnUiThread(ShowWarning);
                                        break;
                                    case "-1":
                                        ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                    onST.Split('*')[1];
                                        RunOnUiThread(ShowError);
                                        break;
                                    default:
                                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                    "Unexpected error while retreiving data";
                                        RunOnUiThread(ShowError);
                                        break;
                                }

                                break;
                            case "0":
                                currentState = addState.AddLot;
                                WarningText =
                                    "The lot number was not found in evolution, would you like to add this item for investigation?";
                                RunOnUiThread(ShowWarning);
                                break;
                            case "-1":
                                ErrorText = "Exception J002: " + System.Environment.NewLine + lotExists.Split('*')[1];
                                RunOnUiThread(ShowError);
                                break;
                            default:
                                ErrorText = "Exception J003: " + System.Environment.NewLine +
                                            "Unexpected error while retreiving data";
                                RunOnUiThread(ShowError);
                                break;
                        }

                        break;
                    case "0":
                        ErrorText = "Exception J001: " + System.Environment.NewLine +
                                    slurryCheckLot.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    case "-1":
                        ErrorText = "Exception J002: " + System.Environment.NewLine +
                                    slurryCheckLot.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                    "Unexpected error while retreiving data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void getMixedSlurryMobileTankInfo()
        {
            try
            {
                string slurryCheckLot =
                    Client.GetMixedSlurryMobileTankInfo_CheckLot(GlobalVar.STItem + "|" + GlobalVar.STUnq);
                switch (slurryCheckLot.Split('*')[0])
                {
                    case "1":
                        slurryCheckLot = slurryCheckLot.Remove(0, 2);
                        GlobalVar.STLot = slurryCheckLot.Split('|')[0];
                        GlobalVar.STQty = slurryCheckLot.Split('|')[1];
                        string lotExists = Client.GetLotInEvo(GlobalVar.STItem, GlobalVar.STLot);
                        switch (lotExists.Split('*')[0])
                        {
                            case "1":
                                string onST = Client.GetItemOnSTockTake(
                                    GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" +
                                    GlobalVar.STItem + "|" +
                                    GlobalVar.STLot);
                                switch (onST.Split('*')[0])
                                {
                                    case "1":
                                        string scannerNo =
                                            new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                                        string mixedSlurryTankChecked = Client.GetMixedSlurryMobileTankInfo(
                                            GlobalVar.StockTake + "|" + GlobalVar.STItem + "|" + GlobalVar.STUnq + "|" +
                                            GlobalVar.STWarehouse +
                                            "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" +
                                            Convert.ToString(GlobalVar.recountST));
                                        if (mixedSlurryTankChecked != string.Empty)
                                        {
                                            switch (mixedSlurryTankChecked.Split('*')[0])
                                            {
                                                case "1":
                                                    mixedSlurryTankChecked = mixedSlurryTankChecked.Remove(0, 2);
                                                    GlobalVar.STLot = mixedSlurryTankChecked.Split('|')[0];
                                                    GlobalVar.STQty = mixedSlurryTankChecked.Split('|')[1];
                                                    RunOnUiThread(confirmSlurry);
                                                    break;
                                                case "0":
                                                    ErrorText = "Exception J001: " + System.Environment.NewLine +
                                                                mixedSlurryTankChecked.Split('*')[1];
                                                    RunOnUiThread(ShowError);
                                                    break;
                                                case "-1":
                                                    ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                                mixedSlurryTankChecked.Split('*')[1];
                                                    RunOnUiThread(ShowError);
                                                    break;
                                                default:
                                                    ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                                "Unexpected error while retreiving data";
                                                    RunOnUiThread(ShowError);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            ErrorText = "Exception J004: " + System.Environment.NewLine +
                                                        "No data was returned from the server";
                                            RunOnUiThread(ShowError);
                                        }

                                        break;
                                    case "0":
                                        //Warning
                                        currentState = addState.AddItem;
                                        WarningText =
                                            "Item was not found on the stock take for this warehouse, would you like to add it to the stock take?";
                                        RunOnUiThread(ShowWarning);
                                        break;
                                    case "-1":
                                        ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                    onST.Split('*')[1];
                                        RunOnUiThread(ShowError);
                                        break;
                                    default:
                                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                    "Unexpected error while retreiving data";
                                        RunOnUiThread(ShowError);
                                        break;
                                }

                                break;
                            case "0":
                                currentState = addState.AddLot;
                                WarningText =
                                    "The lot number was not found in evolution, would you like to add this item for investigation?";
                                RunOnUiThread(ShowWarning);
                                break;
                            case "-1":
                                ErrorText = "Exception J002: " + System.Environment.NewLine + lotExists.Split('*')[1];
                                RunOnUiThread(ShowError);
                                break;
                            default:
                                ErrorText = "Exception J003: " + System.Environment.NewLine +
                                            "Unexpected error while retreiving data";
                                RunOnUiThread(ShowError);
                                break;
                        }

                        break;
                    case "0":
                        ErrorText = "Exception J001: " + System.Environment.NewLine +
                                    slurryCheckLot.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    case "-1":
                        ErrorText = "Exception J002: " + System.Environment.NewLine +
                                    slurryCheckLot.Split('*')[1];
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                    "Unexpected error while retreiving data";
                        RunOnUiThread(ShowError);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void getPGMContainerInfo()
        {
            try
            {
                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                string pgmContChecked = Client.GetPGMContainerInfo(
                    GlobalVar.StockTake + "|" + GlobalVar.STItem + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse +
                    "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" +
                    Convert.ToString(GlobalVar.recountST));
                if (pgmContChecked != string.Empty)
                {
                    switch (pgmContChecked.Split('*')[0])
                    {
                        case "1":
                            pgmContChecked = pgmContChecked.Remove(0, 2);
                            GlobalVar.STLot = pgmContChecked.Split('|')[0];
                            GlobalVar.STQty = pgmContChecked.Split('|')[1];
                            string lotExists = Client.GetLotInEvo(GlobalVar.STItem, GlobalVar.STLot);
                            switch (lotExists.Split('*')[0])
                            {
                                case "1":
                                    string onST = Client.GetItemOnSTockTake(
                                        GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" + GlobalVar.STItem +
                                        "|" + GlobalVar.STLot);
                                    switch (onST.Split('*')[0])
                                    {
                                        case "1":
                                            currentState = addState.None;
                                            RunOnUiThread(confirmPGM);
                                            break;
                                        case "0":
                                            //Warning
                                            currentState = addState.AddItem;
                                            WarningText =
                                                "Item was not found on the stock take for this warehouse, would you like to add it to the stock take?";
                                            RunOnUiThread(ShowWarning);
                                            break;
                                        case "-1":
                                            ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                        onST.Split('*')[1];
                                            RunOnUiThread(ShowError);
                                            break;
                                        default:
                                            ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                        "Unexpected error while retreiving data";
                                            RunOnUiThread(ShowError);
                                            break;
                                    }

                                    break;
                                case "0":
                                    currentState = addState.AddLot;
                                    WarningText =
                                        "The lot number was not found in evolution, would you like to add this item for investigation?";
                                    RunOnUiThread(ShowWarning);
                                    break;
                                case "-1":
                                    ErrorText = "Exception J002: " + System.Environment.NewLine +
                                                lotExists.Split('*')[1];
                                    RunOnUiThread(ShowError);
                                    break;
                                default:
                                    ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                "Unexpected error while retreiving data";
                                    RunOnUiThread(ShowError);
                                    break;
                            }

                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + pgmContChecked.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + pgmContChecked.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine +
                                        "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine +
                                "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void getPowderOnST()
        {
            try
            {
                string lotExists = Client.GetLotInEvo(GlobalVar.STItem, GlobalVar.STLot);
                        switch (lotExists.Split('*')[0])
                        {
                            case "1":
                                string onST = Client.GetItemOnSTockTake(GlobalVar.StockTake + "|" + GlobalVar.STWarehouse + "|" +
                                                                        GlobalVar.STItem + "|" + GlobalVar.STLot);
                                switch (onST.Split('*')[0])
                                {
                                    case "1":
                                        RunOnUiThread(showNextPowder);
                                        break;
                                    case "0":
                                        //Warning
                                        WarningText =
                                            "Item was not found on the stock take for this warhouse, would you like to add it to the stock take?";
                                        RunOnUiThread(ShowWarning);
                                        break;
                                    case "-1":
                                        ErrorText = "Exception J002: " + System.Environment.NewLine + onST.Split('*')[1];
                                        RunOnUiThread(ShowError);
                                        break;
                                    default:
                                        ErrorText = "Exception J003: " + System.Environment.NewLine +
                                                    "Unexpected error while retreiving data";
                                        RunOnUiThread(ShowError);
                                        break;
                                }
                                break;
                            case "0":
                                currentState = addState.AddLot;
                                WarningText =
                                    "The lot number was not found in evolution, would you like to add this item for investigation?";
                                RunOnUiThread(ShowWarning_Powder);
                                break;
                            case "-1":
                                ErrorText = "Exception J002: " + System.Environment.NewLine + lotExists.Split('*')[1];
                                RunOnUiThread(ShowError);
                                break;
                            default:
                                ErrorText = "Exception J003: " + System.Environment.NewLine +
                                            "Unexpected error while retreiving data";
                                RunOnUiThread(ShowError);
                                break;
                        }
                
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void showNextPowder()
        {
            try
            {
                if (GlobalVar.pgsBusy.IsShowing)
                {
                    GlobalVar.pgsBusy.Dismiss();
                }
            }
            catch (Exception)
            {
            }

            StartActivity(typeof(J_ScanQty));
            Finish();
        }

        public void confirmSlurry()
        {
            try
            {
                ScanEngineOff();
                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                switch (GlobalVar.currentSTBarcode)
                {
                    case GlobalVar.STBarcodeType.freshSlurry:
                        Confirm.SetTitle("Confirm Trolley Information");
                        break;
                    case GlobalVar.STBarcodeType.MixedSlurryT:
                        Confirm.SetTitle("Confirm Tank Information");
                        break;
                    case GlobalVar.STBarcodeType.MixedSlurryM:
                        Confirm.SetTitle("Confirm Mobile Tank Information");
                        break;
                    default:
                        break;
                }

                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage("Item Code: " + GlobalVar.STItem + System.Environment.NewLine + "Lot Number:" +
                                   GlobalVar.STLot + System.Environment.NewLine + "Dry Weight:" + GlobalVar.STQty);
                Confirm.SetButton("YES", (s, ev) =>
                {
                    ScanEngineOn();
                    RunOnUiThread(showScanTicket);
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    try
                    {
                        if (GlobalVar.pgsBusy.IsShowing)
                        {
                            GlobalVar.pgsBusy.Dismiss();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    ScanEngineOn();
                    txtBCD.Text = string.Empty;
                    txtBCD.RequestFocus();
                });
                Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void confirmPGM()
        {
            try
            {
                ScanEngineOff();
                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                Confirm.SetTitle("Confirm PGM Container Information");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage("Item Code: " + GlobalVar.STItem + System.Environment.NewLine + "Lot Number:" +
                                   GlobalVar.STLot + System.Environment.NewLine + "Solution Weight:" + GlobalVar.STQty);
                Confirm.SetButton("YES", (s, ev) =>
                {
                    ScanEngineOn();
                    RunOnUiThread(showScanTicket);
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    try
                    {
                        if (GlobalVar.pgsBusy.IsShowing)
                        {
                            GlobalVar.pgsBusy.Dismiss();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    ScanEngineOn();
                    txtBCD.Text = string.Empty;
                    txtBCD.RequestFocus();
                });
                Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J005: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        public void showScanTicket()
        {
            try
            {
                try
                {
                    if (GlobalVar.pgsBusy.IsShowing)
                    {
                        GlobalVar.pgsBusy.Dismiss();
                    }
                }
                catch (Exception)
                {
                }

                StartActivity(typeof(J_ScanTicket));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H908: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(J_SelectStockTake));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H908: " + System.Environment.NewLine + ex.ToString();
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
            SetContentView(Resource.Layout.E_Scan_Dry_Weight);
            layoutStichUp();
            ScanEngineOn();

            #region Wifi / Battery / Error / Version

            #region Version Control

            lblVersion = FindViewById<TextView>(Resource.Id.lblVersion);
            if (GlobalVar.UserPin == null)
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." +
                                  this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;
            }
            else
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." +
                                  this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;
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
            Button thisbutton = (Button) v;
            if (thisbutton.Text == "ERROR OK" || thisbutton.Text == "YES" || thisbutton.Text == "NO")
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
            var s = (Context) this;
            Intent intent = new Intent("ACTION_BAR_SCANCFG");
            intent.PutExtra("EXTRA_SCAN_POWER", 1);
            s.SendBroadcast(intent);
        }

        private void ScanEngineOff()
        {
            var s = (Context) this;
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

            int BPercetage = (int) System.Math.Floor(level * 100D / scale);
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

                int BPercetage = (int) System.Math.Floor(level * 100D / scale);
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