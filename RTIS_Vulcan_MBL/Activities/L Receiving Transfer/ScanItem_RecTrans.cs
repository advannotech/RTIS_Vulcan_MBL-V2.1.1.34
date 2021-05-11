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
using RTIS_Vulcan_MBL.Activities.L_Receiving_Transfer;

namespace RTIS_Vulcan_MBL.Activities
{
    [Activity(Label = "ScanItem_PPRec")]
    public class ScanItem_RecTrans : Activity, View.IOnTouchListener
    {

        string itemCode = string.Empty;
        string lotNum = string.Empty;
        string qty = string.Empty;
        string unq = string.Empty;

        TextView lblHeader1;
        TextView lblHeader2;
        EditText txtItem;
        Button btnBack;

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
            StartActivity(typeof(ScanItem_RecTrans));
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
            SetContentView(Resource.Layout.H_Scan_Slurry_Tank);
            layoutStichUp();
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
        }
        public void layoutStichUp()
        {
            try
            {
                switch (GlobalVar.ProcessName)
                {
                    case "PPtFS":
                        lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                        lblHeader1.Text = "From Transit to Powder Prep";

                        lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                        lblHeader2.Text = "Scan CATScan Barcode";
                        break;
                    case "FStMS":
                        lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                        lblHeader1.Text = "From Transit to Fresh Slurry";

                        lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                        lblHeader2.Text = "Scan CATScan Or Powder Barcode";
                        break;
                    case "MStZECT":
                        lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                        lblHeader1.Text = "From Transit to Mixed Slurry";

                        lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                        lblHeader2.Text = "Scan CATScan Or Trolley Barcode";
                        break;
                    case "ZECT1":
                        lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                        lblHeader1.Text = "From Transit to Zect 1";

                        lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                        lblHeader2.Text = "Scan CATScan Or Tank Barcode";
                        //StartActivity(typeof(ScanZectJob));
                        //Finish();
                        break;
                    case "ZECT2":
                        lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                        lblHeader1.Text = "From Transit to Zect 2";

                        lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                        lblHeader2.Text = "Scan CATScan Or Tank Barcode";
                        //StartActivity(typeof(ScanZectJob));
                        //Finish();
                        break;
                    case "AW":
                        lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                        lblHeader1.Text = "From Transit to Zect 2";

                        lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                        lblHeader2.Text = "Scan ZECT Pallet Tag";
                        //StartActivity(typeof(ScanZectJob));
                        //Finish();
                        break;
                    case "Canning":
                        lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                        lblHeader1.Text = "From Transit to Canning";

                        lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                        lblHeader2.Text = "Scan CATscan Barcode";
                        //StartActivity(typeof(ScanZectJob));
                        //Finish();
                        break;
                    default:
                        break;
                }

                txtItem = FindViewById<EditText>(Resource.Id.txtTank);
                txtItem.KeyPress += TxtItem_KeyPress;
                //txtItem.Text = "CONT_001$SOL-10H VW";

                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L101: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void TxtItem_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {               
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    switch (GlobalVar.ProcessName)
                    {
                        case "PPtFS":
                            #region Powder Prep
                            GlobalVar.RecTransWarehouseTo = GlobalVar.PPtFSWhse;
                            if (txtItem.Text.Contains("(") && txtItem.Text.Contains(")"))
                            {
                                GlobalVar.RecTransItemCode = Barcodes.GetItemCode(txtItem.Text);
                                GlobalVar.RecTransLotNumber = Barcodes.GetItemLot(txtItem.Text);
                                GlobalVar.RecTransQty = Barcodes.GetItemQty(txtItem.Text).Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                                GlobalVar.RecTransUnq = Barcodes.GetUniqCode(txtItem.Text);
                                ScanEngineOff();

                                GlobalVar.pgsBusy = new ProgressDialog(this);
                                GlobalVar.pgsBusy.Indeterminate = true;
                                GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                GlobalVar.pgsBusy.SetMessage("Getting Item Information...");
                                GlobalVar.pgsBusy.SetCancelable(false);
                                GlobalVar.pgsBusy.Show();
                                Thread t = new Thread(getItemDesc);
                                t.Start();
                            }
                            else if (txtItem.Text.Substring(0, 5) == "CONT_" && txtItem.Text.Contains("$"))
                            {
                                txtItem.Text = txtItem.Text.Substring(5, txtItem.Text.Length - 5);
                               
                                GlobalVar.pgsBusy = new ProgressDialog(this);
                                GlobalVar.pgsBusy.Indeterminate = true;
                                GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                GlobalVar.pgsBusy.SetMessage("Getting Item Information...");
                                GlobalVar.pgsBusy.SetCancelable(false);
                                GlobalVar.pgsBusy.Show();
                                Thread t = new Thread(grtPGMInfo);
                                t.Start();
                            }
                            else
                            {
                                ErrorText = "Please scan a valid CATScan Barcode!";
                                RunOnUiThread(ShowError);
                            }
                            #endregion
                            break;
                        case "FStMS":
                            #region Fresh Slurry
                            GlobalVar.RecTransWarehouseTo = GlobalVar.FStMSWhse;
                            if (txtItem.Text.Contains("(") && txtItem.Text.Contains(")"))
                            {
                                GlobalVar.RecTransItemCode = Barcodes.GetItemCode(txtItem.Text);
                                GlobalVar.RecTransLotNumber = Barcodes.GetItemLot(txtItem.Text);
                                GlobalVar.RecTransQty = Barcodes.GetItemQty(txtItem.Text).Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                                GlobalVar.RecTransUnq = Barcodes.GetUniqCode(txtItem.Text);
                                ScanEngineOff();

                                GlobalVar.pgsBusy = new ProgressDialog(this);
                                GlobalVar.pgsBusy.Indeterminate = true;
                                GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                GlobalVar.pgsBusy.SetMessage("Getting Item Information...");
                                GlobalVar.pgsBusy.SetCancelable(false);
                                GlobalVar.pgsBusy.Show();
                                Thread t = new Thread(getItemDesc);
                                t.Start();

                            }
                            else if (txtItem.Text.Substring(0, 5) == "CONT_" && txtItem.Text.Contains("$"))
                            {
                                txtItem.Text = txtItem.Text.Substring(5, txtItem.Text.Length - 5);
                               
                                GlobalVar.pgsBusy = new ProgressDialog(this);
                                GlobalVar.pgsBusy.Indeterminate = true;
                                GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                GlobalVar.pgsBusy.SetMessage("Getting Item Information...");
                                GlobalVar.pgsBusy.SetCancelable(false);
                                GlobalVar.pgsBusy.Show();
                                Thread t = new Thread(grtPGMInfo);
                                t.Start();
                            }
                            else if (txtItem.Text.Contains('$') == true && txtItem.Text.Contains("TRO_") == false && txtItem.Text.Contains("TNK_") == false && txtItem.Text.Contains("CONT_") == false)
                            {
                                if (GlobalVar.RecTransScannedFS.Contains(txtItem.Text) == false)
                                {
                                    GlobalVar.RccTransFSBarcode = txtItem.Text;
                                    GlobalVar.RecTransItemCode = "TPP-" + txtItem.Text.Split('$')[0];
                                    GlobalVar.RecTransLotNumber = txtItem.Text.Split('$')[1];

                                    GlobalVar.pgsBusy = new ProgressDialog(this);
                                    GlobalVar.pgsBusy.Indeterminate = true;
                                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                    GlobalVar.pgsBusy.SetMessage("Getting Item Information...");
                                    GlobalVar.pgsBusy.SetCancelable(false);
                                    GlobalVar.pgsBusy.Show();
                                    Thread t = new Thread(getItemDescFreshSlurry);
                                    t.Start();
                                }
                                else
                                {
                                    ErrorText = "Barcode already scanned!";
                                    RunOnUiThread(ShowError);
                                }                             
                            }
                            else
                            {
                                ErrorText = "Please scan a valid CATScan Barcode!";
                                RunOnUiThread(ShowError);
                            }
                            #endregion
                            break;
                        case "MStZECT":
                            #region Mixed Slurry
                            GlobalVar.RecTransWarehouseTo = GlobalVar.MStZectWhse;
                            if (txtItem.Text.Contains("(") && txtItem.Text.Contains(")"))
                            {
                                GlobalVar.RecTransItemCode = Barcodes.GetItemCode(txtItem.Text);
                                GlobalVar.RecTransLotNumber = Barcodes.GetItemLot(txtItem.Text);
                                GlobalVar.RecTransQty = Barcodes.GetItemQty(txtItem.Text).Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                                GlobalVar.RecTransUnq = Barcodes.GetUniqCode(txtItem.Text);
                                ScanEngineOff();

                                GlobalVar.pgsBusy = new ProgressDialog(this);
                                GlobalVar.pgsBusy.Indeterminate = true;
                                GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                GlobalVar.pgsBusy.SetMessage("Getting Item Information...");
                                GlobalVar.pgsBusy.SetCancelable(false);
                                GlobalVar.pgsBusy.Show();
                                Thread t = new Thread(getItemDesc);
                                t.Start();
                            }
                            else if (txtItem.Text.Contains('$') == true && txtItem.Text.Contains("TRO_") == true)
                            {
                                string trolley = txtItem.Text.Split('$')[0];
                                GlobalVar.RecTransTrolleyCode = trolley.Substring(4, trolley.Length - 4);
                                GlobalVar.RecTransItemCode = txtItem.Text.Split('$')[1];

                                GlobalVar.pgsBusy = new ProgressDialog(this);
                                GlobalVar.pgsBusy.Indeterminate = true;
                                GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                GlobalVar.pgsBusy.SetMessage("Getting fresh slurry info...");
                                GlobalVar.pgsBusy.SetCancelable(false);
                                GlobalVar.pgsBusy.Show();
                                Thread t = new Thread(getFreshSlurryInfo);
                                t.Start();
                            }
                            else
                            {
                                ErrorText = "Please scan a valid CATScan Barcode!";
                                RunOnUiThread(ShowError);
                            }
                            #endregion
                            break;
                        case "ZECT1":
                            #region Zect 1
                            GlobalVar.RecTransWarehouseTo = GlobalVar.ZectWhse;
                            if (txtItem.Text.Contains("(") && txtItem.Text.Contains(")"))
                            {
                                GlobalVar.RecTransItemCode = Barcodes.GetItemCode(txtItem.Text);
                                GlobalVar.RecTransLotNumber = Barcodes.GetItemLot(txtItem.Text);
                                GlobalVar.RecTransQty = Barcodes.GetItemQty(txtItem.Text).Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                                GlobalVar.RecTransUnq = Barcodes.GetUniqCode(txtItem.Text);
                                ScanEngineOff();

                                GlobalVar.pgsBusy = new ProgressDialog(this);
                                GlobalVar.pgsBusy.Indeterminate = true;
                                GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                GlobalVar.pgsBusy.SetMessage("Getting Item Information...");
                                GlobalVar.pgsBusy.SetCancelable(false);
                                GlobalVar.pgsBusy.Show();
                                Thread t = new Thread(getItemDesc);
                                t.Start();
                            }
                            else
                            {
                                ErrorText = "Please scan a valid CATScan Barcode!";
                                RunOnUiThread(ShowError);
                            }
                            #endregion
                            break;
                        case "ZECT2":
                            #region Zect 2
                            GlobalVar.RecTransWarehouseTo = GlobalVar.ZectWhse;
                            if (txtItem.Text.Contains("(") && txtItem.Text.Contains(")"))
                            {
                                GlobalVar.RecTransItemCode = Barcodes.GetItemCode(txtItem.Text);
                                GlobalVar.RecTransLotNumber = Barcodes.GetItemLot(txtItem.Text);
                                GlobalVar.RecTransQty = Barcodes.GetItemQty(txtItem.Text).Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                                GlobalVar.RecTransUnq = Barcodes.GetUniqCode(txtItem.Text);
                                ScanEngineOff();
                                AlertDialog.Builder build = new AlertDialog.Builder(this);
                                AlertDialog Confirm = build.Create();
                                Confirm.SetTitle("Confirm Receiving Transfer Info");
                                Confirm.SetIcon(Resource.Drawable.Icon);
                                Confirm.SetMessage("Item Code: " + GlobalVar.RecTransItemCode + System.Environment.NewLine + "Lot: " + GlobalVar.RecTransLotNumber + System.Environment.NewLine + "Qty Weight: " + GlobalVar.RecTransQty + System.Environment.NewLine + "Warehouse From : " + GlobalVar.RecTransWarehouseFrom + System.Environment.NewLine + "Warehouse To : " + GlobalVar.ZectWhse);
                                Confirm.SetButton("YES", (s, ev) =>
                                {
                                    ScanEngineOn();
                                    GlobalVar.pgsBusy = new ProgressDialog(this);
                                    GlobalVar.pgsBusy.Indeterminate = true;
                                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                    GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                                    GlobalVar.pgsBusy.SetCancelable(false);
                                    GlobalVar.pgsBusy.Show();
                                    Thread t = new Thread(transferItemRT2D);
                                    t.Start();
                                });
                                Confirm.SetButton2("NO", (s, ev) =>
                                {
                                    ScanEngineOn();
                                    StartActivity(typeof(ScanItem_RecTrans));
                                    Finish();
                                });
                                Confirm.Show();
                            }
                            else
                            {
                                ErrorText = "Please scan a valid CATScan Barcode!";
                                RunOnUiThread(ShowError);
                            }
                            #endregion
                            break;
                        case "AW":
                            #region A&W
                            string test = GlobalVar.AWWhse;
                            GlobalVar.RecTransWarehouseTo = GlobalVar.AWWhse;
                            if (txtItem.Text.Contains("(") && txtItem.Text.Contains(")"))
                            {
                                GlobalVar.RecTransItemCode = Barcodes.GetItemCode(txtItem.Text);
                                GlobalVar.RecTransLotNumber = Barcodes.GetItemLot(txtItem.Text);
                                GlobalVar.RecTransQty = Barcodes.GetItemQty(txtItem.Text).Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                                GlobalVar.RecTransUnq = Barcodes.GetUniqCode(txtItem.Text);
                                ScanEngineOff();
                                AlertDialog.Builder build = new AlertDialog.Builder(this);
                                AlertDialog Confirm = build.Create();
                                Confirm.SetTitle("Confirm Receiving Transfer Info");
                                Confirm.SetIcon(Resource.Drawable.Icon);
                                Confirm.SetMessage("Item Code: " + GlobalVar.RecTransItemCode + System.Environment.NewLine + "Lot: " + GlobalVar.RecTransLotNumber + System.Environment.NewLine + "Qty Weight: " + GlobalVar.RecTransQty + System.Environment.NewLine + "Warehouse From : " + GlobalVar.RecTransWarehouseFrom + System.Environment.NewLine + "Warehouse To : " + GlobalVar.AWWhse);
                                Confirm.SetButton("YES", (s, ev) =>
                                {
                                    ScanEngineOn();
                                    GlobalVar.pgsBusy = new ProgressDialog(this);
                                    GlobalVar.pgsBusy.Indeterminate = true;
                                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                    GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                                    GlobalVar.pgsBusy.SetCancelable(false);
                                    GlobalVar.pgsBusy.Show();
                                    Thread t = new Thread(transferItemRT2D);
                                    t.Start();
                                });
                                Confirm.SetButton2("NO", (s, ev) =>
                                {
                                    ScanEngineOn();
                                    StartActivity(typeof(ScanItem_RecTrans));
                                    Finish();
                                });
                                Confirm.Show();
                            }
                            else
                            {
                                ErrorText = "Please scan a valid CATScan Barcode!";
                                RunOnUiThread(ShowError);
                            }
                            #endregion
                            break;
                        case "Canning":
                            #region Canning
                            GlobalVar.RecTransWarehouseTo = GlobalVar.CanWhse;
                            if (txtItem.Text.Contains("(") && txtItem.Text.Contains(")"))
                            {
                                GlobalVar.RecTransItemCode = Barcodes.GetItemCode(txtItem.Text);
                                GlobalVar.RecTransLotNumber = Barcodes.GetItemLot(txtItem.Text);
                                GlobalVar.RecTransQty = Barcodes.GetItemQty(txtItem.Text).Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                                GlobalVar.RecTransUnq = Barcodes.GetUniqCode(txtItem.Text);
                                ScanEngineOff();
                                AlertDialog.Builder build = new AlertDialog.Builder(this);
                                AlertDialog Confirm = build.Create();
                                Confirm.SetTitle("Confirm Receiving Transfer Info");
                                Confirm.SetIcon(Resource.Drawable.Icon);
                                Confirm.SetMessage("Item Code: " + GlobalVar.RecTransItemCode + System.Environment.NewLine + "Lot: " + GlobalVar.RecTransLotNumber + System.Environment.NewLine + "Qty Weight: " + GlobalVar.RecTransQty + System.Environment.NewLine + "Warehouse From : " + GlobalVar.RecTransWarehouseFrom + System.Environment.NewLine + "Warehouse To : " + GlobalVar.CanWhse);
                                Confirm.SetButton("YES", (s, ev) =>
                                {
                                    ScanEngineOn();
                                    GlobalVar.pgsBusy = new ProgressDialog(this);
                                    GlobalVar.pgsBusy.Indeterminate = true;
                                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                                    GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                                    GlobalVar.pgsBusy.SetCancelable(false);
                                    GlobalVar.pgsBusy.Show();
                                    Thread t = new Thread(transferItemRT2D);
                                    t.Start();
                                });
                                Confirm.SetButton2("NO", (s, ev) =>
                                {
                                    ScanEngineOn();
                                    StartActivity(typeof(ScanItem_RecTrans));
                                    Finish();
                                });
                                Confirm.Show();
                            }
                            else
                            {
                                ErrorText = "Please scan a valid CATScan Barcode!";
                                RunOnUiThread(ShowError);
                            }
                            #endregion
                            break;
                        default:
                            break;
                    }                  
                }
                else
                {
                    e.Handled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void getItemDesc()
        {
            try
            {
                string itemDesc = Client.getItemDescription(GlobalVar.RecTransItemCode);
                if (itemDesc != string.Empty)
                {
                    switch (itemDesc.Split('*')[0])
                    {
                        case "1":
                            try
                            {
                                if (GlobalVar.pgsBusy.IsShowing == true)
                                {
                                    GlobalVar.pgsBusy.Dismiss();
                                }
                            }
                            catch (Exception)
                            {

                            }
                            itemDesc = itemDesc.Remove(0, 2);
                            GlobalVar.RecTransItemDesc = itemDesc;
                            RunOnUiThread(confirmRT2DTransfers);
                            break;
                        case "0":
                            ErrorText = "Exception H710: " + System.Environment.NewLine + itemDesc.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception H711: " + System.Environment.NewLine + itemDesc.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception H712: " + System.Environment.NewLine + "Unexpected error while storing data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception H712: " + System.Environment.NewLine + "Unexpected error while storing data";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H713: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void confirmRT2DTransfers()
        {
            try
            {
                string whseTo = string.Empty;
                switch (GlobalVar.ProcessName)
                {
                    case "PPtFS":
                        whseTo = GlobalVar.PPtFSWhse;
                        break;
                    case "FStMS":
                        whseTo = GlobalVar.FStMSWhse;
                        break;
                    case "MStZECT":
                        whseTo = GlobalVar.MStZectWhse;
                        break;
                    case "ZECT1":
                        whseTo = GlobalVar.ZectWhse;
                        break;
                    case "ZECT2":
                        whseTo = GlobalVar.ZectWhse;
                        break;
                    case "AW":
                        whseTo = GlobalVar.AWWhse;
                        break;
                    default:
                        break;
                }

                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                Confirm.SetTitle("Confirm Receiving Transfer Info");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage(GlobalVar.RecTransItemDesc + System.Environment.NewLine + "Lot: " + GlobalVar.RecTransLotNumber + System.Environment.NewLine + "Qty Weight: " + GlobalVar.RecTransQty + System.Environment.NewLine + "Warehouse From : " + GlobalVar.RecTransWarehouseFrom + System.Environment.NewLine + "Warehouse To : " + whseTo);
                Confirm.SetButton("YES", (s, ev) =>
                {
                    ScanEngineOn();
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(transferItemRT2D);
                    t.Start();
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    ScanEngineOn();
                    StartActivity(typeof(ScanItem_RecTrans));
                    Finish();
                });
                Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void grtPGMInfo()
        {
            try
            {
                string pgmInfo = Client.getPGMItemInfo(txtItem.Text.Split('$')[0] + "|" + txtItem.Text.Split('$')[1]);
                switch (pgmInfo.Split('*')[0])
                {
                    case "1":
                        pgmInfo = pgmInfo.Remove(0, 2);
                        GlobalVar.RecTransItemCode = pgmInfo.Split('|')[0];
                        GlobalVar.RecTransLotNumber = pgmInfo.Split('|')[1];
                        GlobalVar.RecTransQty = pgmInfo.Split('|')[2];
                        GlobalVar.RecTransItemDesc = pgmInfo.Split('|')[3];
                        RunOnUiThread(showPGMdetails);
                        break;
                    case "-1":
                        ErrorText = "Exception L113: " + pgmInfo.Split('*')[1].ToUpper(); 
                        RunOnUiThread(ShowError);
                        break;
                    default:
                        ErrorText = "Exception L114: " + "An unexpected error occured while retrieving the PGM item information";
                        RunOnUiThread(ShowError);
                        break;
                }
                
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void showPGMdetails()
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
                string whseTo = string.Empty;
                switch (GlobalVar.ProcessName)
                {
                    case "PPtFS":
                        whseTo = GlobalVar.PPtFSWhse;
                        break;
                    case "FStMS":
                        whseTo = GlobalVar.FStMSWhse;
                        break;
                    case "MStZECT":
                        whseTo = GlobalVar.MStZectWhse;
                        break;
                    case "ZECT1":
                        whseTo = GlobalVar.ZectWhse;
                        break;
                    case "ZECT2":
                        whseTo = GlobalVar.ZectWhse;
                        break;
                    default:
                        break;
                }

                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                Confirm.SetTitle("Confirm Receiving Transfer Info");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage(GlobalVar.RecTransItemDesc + System.Environment.NewLine + "Lot: " + GlobalVar.RecTransLotNumber + System.Environment.NewLine + "Qty: " + GlobalVar.RecTransQty + System.Environment.NewLine + "Warehouse From : " + GlobalVar.RecTransWarehouseFrom + System.Environment.NewLine + "Warehouse To : " + whseTo);
                Confirm.SetButton("YES", (s, ev) =>
                {
                    ScanEngineOn();
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(transferItemPGM);
                    t.Start();
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    ScanEngineOn();
                    StartActivity(typeof(ScanItem_RecTrans));
                    Finish();
                });
                Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void transferItemPGM()
        {
            try
            {
                string transfered = Client.transferFromITPGM(txtItem.Text.Split('$')[0] + "|" + GlobalVar.RecTransItemCode + "|" + GlobalVar.RecTransLotNumber + "|" + GlobalVar.RecTransWarehouseFrom + "|" + GlobalVar.RecTransWarehouseTo + "|" + GlobalVar.RecTransQty + "|" + GlobalVar.UserName + "|" + GlobalVar.ProcessName);
                if (transfered != string.Empty)
                {
                    switch (transfered.Split('*')[0])
                    {
                        case "1":
                            SuccessText = "Information Captured Successfully!";
                            RunOnUiThread(showSuccess);
                            break;
                        case "-1":
                            ErrorText = "Exception L113: " + transfered.Split('*')[1].ToUpper();
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception L114: " + "An unexpected error occured while transferring the PGM item";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception L115: " + "Unexpected error getting mixed slurry info";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        #region Fresh Slurry
        public void getItemDescFreshSlurry()
        {
            try
            {
                string itemDesc = Client.getItemDescription(GlobalVar.RecTransItemCode);
                if (itemDesc != string.Empty)
                {
                    switch (itemDesc.Split('*')[0])
                    {
                        case "1":
                            try
                            {
                                if (GlobalVar.pgsBusy.IsShowing == true)
                                {
                                    GlobalVar.pgsBusy.Dismiss();
                                }
                            }
                            catch (Exception)
                            {

                            }
                            itemDesc = itemDesc.Remove(0, 2);
                            GlobalVar.RecTransItemDesc = itemDesc;
                            RunOnUiThread(showNextPPBCD);
                            break;
                        case "0":
                            ErrorText = "Exception H710: " + System.Environment.NewLine + itemDesc.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception H711: " + System.Environment.NewLine + itemDesc.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception H712: " + System.Environment.NewLine + "Unexpected error while storing data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception H712: " + System.Environment.NewLine + "Unexpected error while storing data";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception H713: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void showNextPPBCD()
        {
            try
            {
                StartActivity(typeof(ScanQty_FSRec));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L102: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        #endregion

        #region Mixed Slurry
        public void getFreshSlurryInfo()
        {
            try
            {
                string slurryInfo = Client.GetFreshSlurryInfoRec(GlobalVar.RecTransTrolleyCode + "|" + GlobalVar.RecTransItemCode);
                if (slurryInfo != string.Empty)
                {
                    switch (slurryInfo.Split('*')[0])
                    {
                        case "1":
                            slurryInfo = slurryInfo.Remove(0, 2);
                            GlobalVar.RecTransLotNumber = slurryInfo.Split('|')[0];
                            GlobalVar.RecTransQty = slurryInfo.Split('|')[1].Replace(",", GlobalVar.sep).Replace(".", GlobalVar.sep);
                            GlobalVar.RecTransItemDesc = slurryInfo.Split('|')[3];
                            string slurryRec = slurryInfo.Split('|')[2];
                            if (Convert.ToBoolean(slurryRec) == false)
                            {
                                RunOnUiThread(showNextFreshSlurry);
                            }
                            else
                            {
                                ErrorText = "Slurry already received!";
                                RunOnUiThread(ShowError);
                            }
                            break;
                        case "-1":
                            ErrorText = "Exception L103: " + slurryInfo.Split('*')[1].ToUpper(); ;
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception L104: " + "An unexpected error occured while transferring the item";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception L105: " + "Unexpected error getting mixed slurry info";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L106: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void showNextFreshSlurry()
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
                ScanEngineOff();
                AlertDialog.Builder build = new AlertDialog.Builder(this);
                AlertDialog Confirm = build.Create();
                Confirm.SetTitle("Confirm Receiving Transfer Info");
                Confirm.SetIcon(Resource.Drawable.Icon);
                Confirm.SetMessage(GlobalVar.RecTransItemDesc + System.Environment.NewLine + "Lot: " + GlobalVar.RecTransLotNumber + System.Environment.NewLine + "Qty Weight: " + GlobalVar.RecTransQty + System.Environment.NewLine + "Warehouse From : " + GlobalVar.RecTransWarehouseFrom + System.Environment.NewLine + "Warehouse To : " + GlobalVar.MStZectWhse);
                Confirm.SetButton("YES", (s, ev) =>
                {
                    ScanEngineOn();
                    GlobalVar.pgsBusy = new ProgressDialog(this);
                    GlobalVar.pgsBusy.Indeterminate = true;
                    GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                    GlobalVar.pgsBusy.SetMessage("Transferring Item...");
                    GlobalVar.pgsBusy.SetCancelable(false);
                    GlobalVar.pgsBusy.Show();
                    Thread t = new Thread(transferItemFSBCD);
                    t.Start();
                });
                Confirm.SetButton2("NO", (s, ev) =>
                {
                    ScanEngineOn();
                    StartActivity(typeof(ScanItem_RecTrans));
                    Finish();
                });
                Confirm.Show();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L107: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void transferItemFSBCD()
        {
            try
            {
                string transferred = Client.tansferFromITMSBCD(GlobalVar.RecTransTrolleyCode + "|" + GlobalVar.RecTransItemCode + "|" + GlobalVar.RecTransLotNumber + "|" + GlobalVar.RecTransWarehouseFrom + "|" + GlobalVar.RecTransWarehouseTo + "|" + GlobalVar.RecTransQty + "|" + GlobalVar.UserName + "|" + GlobalVar.ProcessName);
                if (transferred != string.Empty)
                {
                    switch (transferred.Split('*')[0])
                    {
                        case "1":
                            SuccessText = "Infromation Captured Successfully!";
                            RunOnUiThread(showSuccess);
                            break;
                        case "0":
                            ErrorText = "Exception L108: " + System.Environment.NewLine + transferred.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception L109: " + System.Environment.NewLine + transferred.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception L110: " + System.Environment.NewLine + "Unexpected error while transferring powder!";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception L111: " + "Unexpected error while saving transfer";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L112: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        #endregion

        #region RT2D
        public void transferItemRT2D()
        {
            try
            {
                string transferred = Client.tansferFromITRT2D(GlobalVar.RecTransItemCode + "|" + GlobalVar.RecTransLotNumber + "|" + GlobalVar.RecTransWarehouseFrom + "|" + GlobalVar.RecTransWarehouseTo + "|" + GlobalVar.RecTransQty + "|" + GlobalVar.UserName + "|" + GlobalVar.ProcessName + "|" + GlobalVar.RecTransUnq);
                if (transferred != string.Empty)
                {
                    switch (transferred.Split('*')[0])
                    {
                        case "1":
                            SuccessText = "Infromation Captured Successfully!";
                            RunOnUiThread(showSuccess);
                            break;
                        case "0":
                            ErrorText = "Exception L123: " + System.Environment.NewLine + transferred.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception L124: " + System.Environment.NewLine + transferred.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception L125: " + System.Environment.NewLine + "Unexpected error while transferring powder!";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception L126: " + "Unexpected error while saving transfer";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L127: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        #endregion
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(MainActivity));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception L303: " + ex.ToString();
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
            SetContentView(Resource.Layout.H_Scan_Slurry_Tank);
            layoutStichUp();
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
            txtItem.RequestFocus();
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