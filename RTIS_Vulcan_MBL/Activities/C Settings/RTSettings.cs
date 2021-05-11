using System;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Net.Wifi;
using Android.Preferences;
using System.Linq;
using Android.Media;

namespace RTIS_Vulcan_MBL
{
    [Activity(Label = "RTSettings")]
    public class RTSettings : Activity, View.IOnTouchListener
    {
        Button btnSave;
        EditText txtServerIP;
        EditText txtServerPort;
        EditText txtBranch;
        EditText txtScannerID;

        EditText txtPPtFS;
        EditText txtFStMS;
        EditText txtMStZect;
        EditText txtToProd;
        EditText txtZectWhse;
        EditText txtAWWhse;
        EditText txtCanWhse;

        CheckBox chkReceive;
        CheckBox chkPowderPrep;
        CheckBox chkFreshSlurry;
        CheckBox chkMixedSlurry;
        CheckBox chkToProd;
        CheckBox chkZectTrans;
        CheckBox chkAWTrans;
        CheckBox chkCanning;
        CheckBox chkDispatch;
        CheckBox chkFGReceiving;
        private CheckBox chkPalletizing;

        CheckBox chkStockTake;
        CheckBox chkCYCCount;
        CheckBox chkUT;

        RadioButton radSingle;
        RadioButton radDouble;
        RadioButton radRecount;

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

            SetContentView(Resource.Layout.C_RTSettings);

            #region Text Input            

            txtServerIP = FindViewById<EditText>(Resource.Id.txtServerIP);
            txtServerPort = FindViewById<EditText>(Resource.Id.txtServerPort);
            txtBranch = FindViewById<EditText>(Resource.Id.txtBranch);
            txtScannerID = FindViewById<EditText>(Resource.Id.txtScannerID);
            txtPPtFS = FindViewById<EditText>(Resource.Id.txtPPtFS);
            txtFStMS = FindViewById<EditText>(Resource.Id.txtFStMS);
            txtMStZect = FindViewById<EditText>(Resource.Id.txtMStZect);
            txtToProd = FindViewById<EditText>(Resource.Id.txtToProd);
            txtZectWhse = FindViewById<EditText>(Resource.Id.txtZectWhse);
            txtAWWhse = FindViewById<EditText>(Resource.Id.txtAWWhse);
            txtCanWhse = FindViewById<EditText>(Resource.Id.txtCanWhse);
            #endregion

            #region Check Boxes

            chkReceive = FindViewById<CheckBox>(Resource.Id.chkReceive);
            chkPowderPrep = FindViewById<CheckBox>(Resource.Id.chkPowderPrep);
            chkPowderPrep.CheckedChange += ChkPowderPrep_CheckedChange;
            chkFreshSlurry = FindViewById<CheckBox>(Resource.Id.chkFreshSlurry);
            chkFreshSlurry.CheckedChange += ChkFreshSlurry_CheckedChange;
            chkMixedSlurry = FindViewById<CheckBox>(Resource.Id.chkMixedSlurry);
            chkMixedSlurry.CheckedChange += ChkMixedSlurry_CheckedChange;

            chkToProd = FindViewById<CheckBox>(Resource.Id.chkToProd);
            chkToProd.CheckedChange += ChkToProd_CheckedChange;

            chkZectTrans = FindViewById<CheckBox>(Resource.Id.chkZectTrans);
            chkZectTrans.CheckedChange += ChkZectTrans_CheckedChange;
            chkAWTrans = FindViewById<CheckBox>(Resource.Id.chkAWTrans);
            chkAWTrans.CheckedChange += ChkAWTrans_CheckedChange;
            chkCanning = FindViewById<CheckBox>(Resource.Id.chkCanning);
            chkCanning.CheckedChange += ChkCanning_CheckedChange;

            chkDispatch = FindViewById<CheckBox>(Resource.Id.chkDispatch);
            chkDispatch.CheckedChange += ChkDispatch_CheckedChange; ;

            chkPalletizing = FindViewById<CheckBox>(Resource.Id.chkPalletizing);
            chkPalletizing.CheckedChange += ChkPalletizing_CheckedChange;

            chkFGReceiving = FindViewById<CheckBox>(Resource.Id.chkFGReceiving);
            chkStockTake = FindViewById<CheckBox>(Resource.Id.chkStockTake);
            chkCYCCount = FindViewById<CheckBox>(Resource.Id.chkCYCCount);
            chkUT = FindViewById<CheckBox>(Resource.Id.chkUserTrack);
            

            #endregion

            #region Buttons

            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += BtnSave_Click;
            btnSave.SetOnTouchListener(this);

            #endregion

            #region Radio Buttons
            radSingle = FindViewById<RadioButton>(Resource.Id.radSingle);
            radSingle.CheckedChange += RadSingle_CheckedChange;
            radDouble = FindViewById<RadioButton>(Resource.Id.radDouble);
            radDouble.CheckedChange += RadDouble_CheckedChange;
            radRecount = FindViewById<RadioButton>(Resource.Id.radRecount);
            radRecount.CheckedChange += RadRecount_CheckedChange;
            #endregion

            OnLoad();
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

        private void ChkPalletizing_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
           
        }

        private void RadRecount_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (radRecount.Checked == true)
                {
                    radSingle.Checked = false;
                    radDouble.Checked = false;
                }
                //else
                //{
                //    radDouble.Checked = true;
                //}
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR B114" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void RadDouble_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (radDouble.Checked == true)
                {
                    radSingle.Checked = false;
                    radRecount.Checked = false;
                }
                //else
                //{
                //    radDouble.Checked = true;
                //}
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR B114" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void RadSingle_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (radSingle.Checked == true)
                {
                    radDouble.Checked = false;
                    radRecount.Checked = false;
                }
                //else
                //{
                //    radDouble.Checked = true;
                //}
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR B114" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        private void ChkPowderPrep_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (chkPowderPrep.Checked == true)
                {
                    txtPPtFS.Enabled = true;
                }
                else
                {
                    txtPPtFS.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR C201" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }    
        private void ChkFreshSlurry_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (chkFreshSlurry.Checked == true)
                {
                    txtFStMS.Enabled = true;
                }
                else
                {
                    txtFStMS.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR C202" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void ChkMixedSlurry_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (chkMixedSlurry.Checked == true)
                {
                    txtMStZect.Enabled = true;
                }
                else
                {
                    txtMStZect.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR C203" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void ChkToProd_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (chkToProd.Checked == true)
                {
                    txtToProd.Enabled = true;
                }
                else
                {
                    txtToProd.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR C203" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void ChkZectTrans_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (chkZectTrans.Checked == true)
                {
                    txtZectWhse.Enabled = true;
                }
                else
                {
                    txtZectWhse.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR C201" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void ChkAWTrans_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (chkAWTrans.Checked == true)
                {
                    txtAWWhse.Enabled = true;
                }
                else
                {
                    txtAWWhse.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR C202" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void ChkCanning_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (chkCanning.Checked == true)
                {
                    txtCanWhse.Enabled = true;
                }
                else
                {
                    txtCanWhse.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR C202" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void ChkDispatch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ErrorText = "ERROR C202" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtServerIP.Text != "")
                {
                    bool PortInt = txtServerPort.Text.All(char.IsDigit);
                    if (PortInt == true)
                    {
                        int ServerPort = Convert.ToInt32(txtServerPort.Text);
                        if (txtScannerID.Text.Substring(0, 3) == "SCN")
                        {
                            bool IsInt = txtScannerID.Text.Substring(3, 3).All(char.IsDigit);
                            if (IsInt == true)
                            {
                                var c = (Context)this;
                                ISharedPreferences prefs = c.GetSharedPreferences("RTIS_Vulcan_MBL", FileCreationMode.WorldReadable);
                                ISharedPreferencesEditor prefEditor = prefs.Edit();

                                #region Modules

                                GlobalVar.MReceive = chkReceive.Checked;

                                GlobalVar.MPowderPrep = chkPowderPrep.Checked;
                                GlobalVar.MFreshSlurry = chkFreshSlurry.Checked;
                                GlobalVar.MMixedSlurry = chkMixedSlurry.Checked;
                                GlobalVar.MToProd = chkToProd.Checked;
                                GlobalVar.MZectTrans = chkZectTrans.Checked;
                                GlobalVar.MAWTrans = chkAWTrans.Checked;
                                GlobalVar.MCanTrans = chkCanning.Checked;
                                GlobalVar.MDispatch = chkDispatch.Checked;

                                GlobalVar.MFGReceiving = chkFGReceiving.Checked;
                                GlobalVar.MStockTake = chkStockTake.Checked;
                                GlobalVar.MCYCCount = chkCYCCount.Checked;
                                GlobalVar.MUserTracking = chkUT.Checked;

                                prefEditor.PutBoolean("MReceive", chkReceive.Checked);

                                prefEditor.PutBoolean("MPowderPrep", chkPowderPrep.Checked);
                                prefEditor.PutBoolean("MFreshSlurry", chkFreshSlurry.Checked);
                                prefEditor.PutBoolean("MMixedSlurry", chkMixedSlurry.Checked);
                                prefEditor.PutBoolean("MToProd", chkToProd.Checked);
                                prefEditor.PutBoolean("MZectTransfer", chkZectTrans.Checked);
                                prefEditor.PutBoolean("MAWTransfer", chkAWTrans.Checked);
                                prefEditor.PutBoolean("MCanTransfer", chkCanning.Checked);
                                prefEditor.PutBoolean("MDispatch", chkDispatch.Checked);

                                prefEditor.PutBoolean("MFGReceiving", chkFGReceiving.Checked);

                                prefEditor.PutBoolean("MStockTake", chkStockTake.Checked);
                                prefEditor.PutBoolean("MCYCCount", chkCYCCount.Checked);
                                prefEditor.PutBoolean("MUserTrack", chkUT.Checked);
                                prefEditor.PutBoolean("MPallet", chkPalletizing.Checked);

                                prefEditor.Apply();

                                #endregion

                                #region Server IP / Port

                                prefEditor.PutString("ServerIP", txtServerIP.Text);
                                prefEditor.PutInt("ServerPort", ServerPort);
                                prefEditor.Apply();
                                GlobalVar.ServerIP = txtServerIP.Text;
                                GlobalVar.ServerPort = ServerPort;

                                #endregion

                                #region Branch

                                prefEditor.PutString("SBranch", txtBranch.Text);
                                prefEditor.Apply();
                                GlobalVar.Branch = txtBranch.Text;

                                #endregion

                                #region Scanner ID

                                prefEditor.PutString("ScannerID", txtScannerID.Text);
                                prefEditor.Apply();
                                GlobalVar.ScannerID = txtScannerID.Text;

                                #endregion

                                #region User Tracking

                                if (GlobalVar.MUserTracking == true)
                                {
                                    StartActivity(typeof(Login));
                                }
                                else
                                {
                                    GlobalVar.LoginTime = 200;
                                    GlobalVar.UserPin = null;
                                    StartActivity(typeof(MainActivity));
                                }

                                #endregion

                                #region Warehouses
                                prefEditor.PutString("sPPtFSWhse", txtPPtFS.Text);
                                prefEditor.Apply();
                                GlobalVar.PPtFSWhse = txtPPtFS.Text;

                                prefEditor.PutString("sFStMSWhse", txtFStMS.Text);
                                prefEditor.Apply();
                                GlobalVar.FStMSWhse = txtFStMS.Text;

                                prefEditor.PutString("sMStZectWhse", txtMStZect.Text);
                                prefEditor.Apply();
                                GlobalVar.MStZectWhse = txtMStZect.Text;

                                prefEditor.PutString("sToProdWhse", txtToProd.Text);
                                prefEditor.Apply();
                                GlobalVar.ToProdWhse = txtToProd.Text;

                                prefEditor.PutString("sZectWhse", txtZectWhse.Text);
                                prefEditor.Apply();
                                GlobalVar.ZectWhse = txtZectWhse.Text;

                                prefEditor.PutString("sAWWhse", txtAWWhse.Text);
                                prefEditor.Apply();
                                GlobalVar.AWWhse = txtAWWhse.Text;

                                prefEditor.PutString("sCanWhse", txtCanWhse.Text);
                                prefEditor.Apply();
                                GlobalVar.CanWhse = txtCanWhse.Text;
                                #endregion

                                #region Stock Take
                                if (radSingle.Checked == true)
                                {
                                    GlobalVar.singleST = true;
                                    GlobalVar.doubleST = false;
                                    GlobalVar.recountST = false;
                                    prefEditor.PutBoolean("singleST", true);
                                    prefEditor.PutBoolean("doubleST", false);
                                    prefEditor.PutBoolean("recountST", false);
                                    prefEditor.Apply();
                                }
                                else if (radDouble.Checked == true)
                                {
                                    GlobalVar.singleST = false;
                                    GlobalVar.doubleST = true;
                                    GlobalVar.recountST = false;
                                    prefEditor.PutBoolean("singleST", false);
                                    prefEditor.PutBoolean("doubleST", true);
                                    prefEditor.PutBoolean("recountST", false);
                                    prefEditor.Apply();
                                }
                                else
                                {
                                    GlobalVar.singleST = false;
                                    GlobalVar.doubleST = false;
                                    GlobalVar.recountST = true;
                                    prefEditor.PutBoolean("singleST", false);
                                    prefEditor.PutBoolean("doubleST", false);
                                    prefEditor.PutBoolean("recountST", true);
                                    prefEditor.Apply();
                                }
                                #endregion

                                Finish();
                            }
                            else
                            {
                                ErrorText = "ERROR C204" + System.Environment.NewLine + "SCANNER ID INCORRECT. THE ID MUST START WITH SCN FOLLOWED BY 3 NUMERIC NUMBERS. EXAMPLE SCN001";
                                RunOnUiThread(ShowError);
                            }
                        }
                        else
                        {
                            ErrorText = "ERROR C205" + System.Environment.NewLine + "SCANNER ID INCORRECT. THE ID MUST START WITH SCN FOLLOWED BY 3 NUMERIC NUMBERS. EXAMPLE SCN001";
                            RunOnUiThread(ShowError);
                        }
                    }
                    else
                    {
                        ErrorText = "ERROR C206" + System.Environment.NewLine + "SERVER PORT INCORRECT. THE SERVER PORT MUST BE NUMERIC NUMBERS. EXAMPLE 12345";
                        RunOnUiThread(ShowError);
                    }
                }
                else
                {
                    ErrorText = "ERROR C207" + System.Environment.NewLine + "SERVER IP INCORRECT.";
                    RunOnUiThread(ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "ERROR C208" + System.Environment.NewLine + "Exception: " + ex.ToString();
                RunOnUiThread(ShowError);
            }
        }

        protected void OnLoad()
        {
            btnSave.RequestFocus();

            #region Modules

            chkReceive.Checked = GlobalVar.MReceive;
            chkPowderPrep.Checked = GlobalVar.MPowderPrep;
            chkFreshSlurry.Checked = GlobalVar.MFreshSlurry;
            chkMixedSlurry.Checked = GlobalVar.MMixedSlurry;
            chkToProd.Checked = GlobalVar.MToProd;
            chkFGReceiving.Checked = GlobalVar.MFGReceiving;
            chkStockTake.Checked = GlobalVar.MStockTake;
            chkCYCCount.Checked = GlobalVar.MCYCCount;
            chkUT.Checked = GlobalVar.MUserTracking;
            chkZectTrans.Checked = GlobalVar.MZectTrans;
            chkAWTrans.Checked = GlobalVar.MAWTrans;
            chkCanning.Checked = GlobalVar.MCanTrans;
            chkDispatch.Checked = GlobalVar.MDispatch;
            chkPalletizing.Checked = GlobalVar.MPalletizing;

            if (chkPowderPrep.Checked == true)
            {
                txtPPtFS.Enabled = true;
            }
            else
            {
                txtPPtFS.Enabled = false;
            }

            if (chkFreshSlurry.Checked == true)
            {
                txtFStMS.Enabled = true;
            }
            else
            {
                txtFStMS.Enabled = false;
            }

            if (chkMixedSlurry.Checked == true)
            {
                txtMStZect.Enabled = true;
            }
            else
            {
                txtMStZect.Enabled = false;
            }

            if (chkZectTrans.Checked == true)
            {
                txtZectWhse.Enabled = true;
            }
            else
            {
                txtZectWhse.Enabled = false;
            }

            if (chkAWTrans.Checked == true)
            {
                txtAWWhse.Enabled = true;
            }
            else
            {
                txtAWWhse.Enabled = false;
            }

            if (chkCanning.Checked == true)
            {
                txtCanWhse.Enabled = true;
            }
            else
            {
                txtCanWhse.Enabled = false;
            }
            #endregion

            #region Server IP / Port

            txtServerIP.Text = GlobalVar.ServerIP;
            txtServerPort.Text = Convert.ToString(GlobalVar.ServerPort);

            #endregion

            #region Branch

            txtBranch.Text = GlobalVar.Branch;

            #endregion
            
            #region Scanner ID

            txtScannerID.Text = GlobalVar.ScannerID;

            #endregion

            #region Warehouses
            txtPPtFS.Text = GlobalVar.PPtFSWhse;
            txtFStMS.Text = GlobalVar.FStMSWhse;
            txtMStZect.Text = GlobalVar.MStZectWhse;
            txtToProd.Text = GlobalVar.ToProdWhse;
            txtZectWhse.Text = GlobalVar.ZectWhse;
            txtAWWhse.Text = GlobalVar.AWWhse;
            txtCanWhse.Text = GlobalVar.CanWhse;
            #endregion

            #region Stock Take
            if (GlobalVar.singleST == true)
            {
                radSingle.Checked = true;
            }
            else if(GlobalVar.doubleST == true)
            {
                radDouble.Checked = true;
            }
            else
            {
                radRecount.Checked = true;
            }
            #endregion

            int position = txtServerIP.Length();
            txtServerIP.SetSelection(position);
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
            txtServerIP.Text = "";
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
            SetContentView(Resource.Layout.C_RTSettings);

            #region Text Input            

            txtServerIP = FindViewById<EditText>(Resource.Id.txtServerIP);
            txtServerPort = FindViewById<EditText>(Resource.Id.txtServerPort);
            txtBranch = FindViewById<EditText>(Resource.Id.txtBranch);
            txtScannerID = FindViewById<EditText>(Resource.Id.txtScannerID);

            #endregion

            #region Check Boxes

            chkUT = FindViewById<CheckBox>(Resource.Id.chkUserTrack);

            #endregion
            
            #region Buttons

            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += BtnSave_Click;
            btnSave.SetOnTouchListener(this);

            #endregion

            txtServerIP.RequestFocus();
            OnLoad();

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