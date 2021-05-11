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

namespace RTIS_Vulcan_MBL.Activities.J_Stock_Take
{
    [Activity(Label = "J_ScanTicket")]
    public class J_ScanTicket : Activity, View.IOnTouchListener
    {
        TextView lblHeader1;
        TextView lblHeader2;
        EditText txtBCD;
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
            SetContentView(Resource.Layout.E_Scan_Dry_Weight);
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
                lblHeader1 = FindViewById<TextView>(Resource.Id.lblHeader1);
                lblHeader1.Text = "Stock Take";
                lblHeader2 = FindViewById<TextView>(Resource.Id.lblHeader2);
                lblHeader2.Text = "Scan / Enter Ticket";
                txtBCD = FindViewById<EditText>(Resource.Id.txtBCD);
                txtBCD.KeyPress += TxtBCD_KeyPress;
                btnBack = FindViewById<Button>(Resource.Id.btnBack);
                btnBack.SetOnTouchListener(this);
                btnBack.Click += BtnBack_Click;
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
                    if (txtBCD.Text != string.Empty)
                    {
                        Thread t;
                        GlobalVar.pgsBusy = new ProgressDialog(this);
                        GlobalVar.pgsBusy.Indeterminate = true;
                        GlobalVar.pgsBusy.SetProgressStyle(ProgressDialogStyle.Spinner);
                        GlobalVar.pgsBusy.SetMessage("Updating stock take...");
                        GlobalVar.pgsBusy.SetCancelable(false);
                        GlobalVar.pgsBusy.Show();
                        switch (GlobalVar.currentSTBarcode)
                        {
                            case GlobalVar.STBarcodeType.powder:
                                t = new Thread(updateStockTake_PP);
                                t.Start();
                                break;
                            case GlobalVar.STBarcodeType.freshSlurry:                              
                                t = new Thread(updateStockTake_FS);
                                t.Start();
                                break;
                            case GlobalVar.STBarcodeType.MixedSlurryT:
                                t = new Thread(updateStockTake_MSTank);
                                t.Start();
                                break;
                            case GlobalVar.STBarcodeType.MixedSlurryM:
                                t = new Thread(updateStockTake_MSMTank);
                                t.Start();
                                break;
                            case GlobalVar.STBarcodeType.PGM:
                                t = new Thread(updateStockTake_PGM);
                                t.Start();
                                break;
                            case GlobalVar.STBarcodeType.RT2D:
                                t = new Thread(updateStockTake);
                                t.Start();
                                break;
                            case GlobalVar.STBarcodeType.Pallet:
                                if (GlobalVar.STLot == "#PALLET#")
                                {
                                    t = new Thread(updateStockTake_Pallet_RM);
                                    t.Start();
                                }
                                else
                                {
                                    t = new Thread(updateStockTake_Pallet);
                                    t.Start();
                                }
                                break;
                            case GlobalVar.STBarcodeType.manual:
                                t = new Thread(updateStockTake_manual);
                                t.Start();
                                break;
                            default:
                                ErrorText = "Exception J013: " + System.Environment.NewLine + "Invalid item type scanned!";
                                RunOnUiThread(ShowError);
                                break;
                        }                      
                    }
                    else
                    {
                        ErrorText = "Exception J013: " + System.Environment.NewLine + "Please scan a valid ticket number";
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
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void updateStockTake()
        {
            try
            {
                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                string saved = Client.SaveStockTakeItemRT2D(GlobalVar.StockTake + "|" + txtBCD.Text + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" + GlobalVar.STQty + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" + Convert.ToString(GlobalVar.recountST) + "|" + GlobalVar.UserName);
                if (saved != string.Empty)
                {
                    switch (saved.Split('*')[0])
                    {
                        case "1":
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void updateStockTake_Pallet()
        {
            try
            {
                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                string saved = Client.SaveStockTakeItemPallet(GlobalVar.StockTake + "|" + txtBCD.Text + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" + GlobalVar.STQty + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" + Convert.ToString(GlobalVar.recountST) + "|" + GlobalVar.UserName);
                if (saved != string.Empty)
                {
                    switch (saved.Split('*')[0])
                    {
                        case "1":
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void updateStockTake_Pallet_RM()
        {
            try
            {
                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                string saved = Client.SaveStockTakeItemPallet_RM(GlobalVar.StockTake + "|" + txtBCD.Text + "|" + GlobalVar.STItem + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" + Convert.ToString(GlobalVar.recountST) + "|" + GlobalVar.UserName);
                if (saved != string.Empty)
                {
                    switch (saved.Split('*')[0])
                    {
                        case "1":
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void updateStockTake_FS()
        {
            try
            {
                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                string saved = Client.SaveStockTakeItemFreshSlurry(GlobalVar.StockTake + "|" + txtBCD.Text + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" + GlobalVar.STQty + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" + Convert.ToString(GlobalVar.recountST) + "|" + GlobalVar.UserName);
                if (saved != string.Empty)
                {
                    switch (saved.Split('*')[0])
                    {
                        case "1":
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void updateStockTake_MSTank()
        {
            try
            {
                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                string saved = Client.SaveStockTakeItemMixedSlurryTank(GlobalVar.StockTake + "|" + txtBCD.Text + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" + GlobalVar.STQty + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" + Convert.ToString(GlobalVar.recountST) + "|" + GlobalVar.UserName);
                if (saved != string.Empty)
                {
                    switch (saved.Split('*')[0])
                    {
                        case "1":
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void updateStockTake_MSMTank()
        {
            try
            {
                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                string saved = Client.SaveStockTakeItemMixedSlurryMTank(GlobalVar.StockTake + "|" + txtBCD.Text + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" + GlobalVar.STQty + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" + Convert.ToString(GlobalVar.recountST) + "|" + GlobalVar.UserName);
                if (saved != string.Empty)
                {
                    switch (saved.Split('*')[0])
                    {
                        case "1":
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void updateStockTake_PGM()
        {
            try
            {
                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                string saved = Client.SaveStockTakePGM(GlobalVar.StockTake + "|" + txtBCD.Text + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" + GlobalVar.STQty + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" + Convert.ToString(GlobalVar.recountST) + "|" + GlobalVar.UserName);
                if (saved != string.Empty)
                {
                    switch (saved.Split('*')[0])
                    {
                        case "1":
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void updateStockTake_PP()
        {
            try
            {
                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                string saved = Client.SaveStockTakeItemPowderPrep(GlobalVar.StockTake + "|" + txtBCD.Text + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" + GlobalVar.STQty + "|" + GlobalVar.STUnq + "|" + GlobalVar.STWarehouse + "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" + Convert.ToString(GlobalVar.recountST) + "|" + GlobalVar.UserName);
                if (saved != string.Empty)
                {
                    switch (saved.Split('*')[0])
                    {
                        case "1":
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void updateStockTake_manual()
        {
            try
            {
                string scannerNo = new String(GlobalVar.ScannerID.Where(char.IsDigit).ToArray());
                string saved = Client.SaveStockTakeItemManual(GlobalVar.StockTake + "|" + txtBCD.Text + "|" + GlobalVar.STItem + "|" + GlobalVar.STLot + "|" + GlobalVar.STQty  + "|" + GlobalVar.STWarehouse + "|" + scannerNo + "|" + Convert.ToString(GlobalVar.singleST) + "|" + Convert.ToString(GlobalVar.recountST) + "|" + GlobalVar.UserName);
                if (saved != string.Empty)
                {
                    switch (saved.Split('*')[0])
                    {
                        case "1":
                            RunOnUiThread(showNext);
                            break;
                        case "0":
                            ErrorText = "Exception J001: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        case "-1":
                            ErrorText = "Exception J002: " + System.Environment.NewLine + saved.Split('*')[1];
                            RunOnUiThread(ShowError);
                            break;
                        default:
                            ErrorText = "Exception J003: " + System.Environment.NewLine + "Unexpected error while retreiving data";
                            RunOnUiThread(ShowError);
                            break;
                    }
                }
                else
                {
                    ErrorText = "Exception J004: " + System.Environment.NewLine + "No data was returned from the server";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        public void showNext()
        {
            try
            {
                StartActivity(typeof(J_ScanItem));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(J_ScanItem));
                Finish();
            }
            catch (Exception ex)
            {
                ErrorText = "Exception J013: " + System.Environment.NewLine + ex.ToString();
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
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;
            }
            else
            {
                lblVersion.Text = "V: " + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionCode + "." + this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;
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