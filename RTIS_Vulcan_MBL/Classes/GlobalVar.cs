using System.Collections.Generic;
using System.Threading;

using Android;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace RTIS_Vulcan_MBL
{
    class GlobalVar
    {
        public static ProgressDialog pgsBusy;
        public static string sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        #region User Information

        public static string UserPin { get; set; }
        public static string UserName { get; set; }
        public static int LoginTime { get; set; }

        #endregion

        #region Administration

        #region Settings Admin
        public static string SettingsPassword { get; set; }

        #endregion

        #region Settings Application

        public static string ServerIP { get; set; }
        public static int ServerPort { get; set; }
        public static string Branch { get; set; }
        public static bool Scanning { get; set; }
        public static string ScannerID { get; set; }

        #endregion

        #region Module Settings

        public static bool MReceive { get; set; } //PO Receiving

        public static bool MPowderPrep { get; set; } //Powder Prep Manufacture
        public static bool MFreshSlurry { get; set; } //Fresh Slurry Manufacture
        public static bool MMixedSlurry { get; set; } //Mixed Slurry Manufacture
        public static bool MToProd { get; set; } //Mixed Slurry Manufacture

        public static bool MZectTrans { get; set; } //Zect Transfer
        public static bool MAWTrans { get; set; } //A&W Transfer
        public static bool MCanTrans { get; set; } //Canning Transfer
        public static bool MDispatch { get; set; } //Dispatch
        public static bool MFGReceiving { get; set; } //FG Receiving From Production

        public static bool MStockTake { get; set; } //Stock Takes
        public static bool MCYCCount { get; set; } //Cycle Counts
        public static bool MUserTracking { get; set; } //UserTracking

        public static bool MPalletizing { get; set; } //UserTracking

        #endregion

        #endregion

        #region Warehouses
        public static string PPtFSWhse { get; set; }
        public static string FStMSWhse { get; set; }
        public static string ToProdWhse { get; set; }
        public static string MStZectWhse { get; set; }
        public static string ZectWhse { get; set; }
        public static string AWWhse { get; set; }
        public static string CanWhse { get; set; }
        #endregion

        #region Item Barcode Data

        public static string BarcodeData { get; set; }
        public static string ItemCode { get; set; }
        public static string ItemDesc { get; set; }
        public static string ItemLot { get; set; }
        public static string ItemQty { get; set; }

        #endregion

        #region Receiving

        public static string OrderNum { get; set; }

        public static string _orderdetails;
        public static List<OrderDetail> CurrentOrderItem { get; set; }
        public class OrderDetail
        {
            public string ItemCode { get; set; }
            public string ItemDesc { get; set; }
            public string ItemLotNum { get; set; }
            public string OrderQty { get; set; }
            public string ProcQty { get; set; }
            public string ToProcQty { get; set; }
            public string TotalRecQty { get; set; }
            public string lotItem { get; set; }

            public int theme { get; set; }
            //public Bitmap complete { get; set; }
        }
        public class OrderDetailAdapter : BaseAdapter<OrderDetail>
        {
            OrderDetail[] items;
            Activity context;
            public OrderDetailAdapter(Activity context, OrderDetail[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override OrderDetail this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Length; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView; // re-use an existing view, if one is available
                if (view == null) // otherwise create a new one
                    view = context.LayoutInflater.Inflate(Resource.Layout.D_ListViewItem, null);
                view.FindViewById<TextView>(Resource.Id.txtItemNo).Text = items[position].ItemCode;
                view.FindViewById<TextView>(Resource.Id.txtDesc).Text = items[position].ItemDesc;
                view.FindViewById<TextView>(Resource.Id.txtItemLot).Text = items[position].ItemLotNum;
                //view.FindViewById<TextView>(Resource.Id.txtQty).Text = items[position].TotalRecQty + "/" + items[position].OrderQty;
                view.FindViewById<TextView>(Resource.Id.txtQty).Text = items[position].ToProcQty + "/" + items[position].OrderQty;
                //view.FindViewById<ImageView>(Resource.Id.imgFound).SetImageBitmap(items[position].complete);
                view.FindViewById<LinearLayout>(Resource.Id.Text).SetBackgroundResource(items[position].theme);
                return view;
            }
        }

        #endregion

        #region Palletizing
        public class PalletItem
        {
            public PalletItem(string[] args)
            {
                ItemCode = args[0];
                ItemDesc = args[1];
                ItemLotNum = args[2];
                Qty = args[3];
                Unq = args[4];
            }
            public string ItemCode { get; set; }
            public string ItemDesc { get; set; }
            public string ItemLotNum { get; set; }
            public string Qty { get; set; }
            public string Unq { get; set; }
            
        }
        public static List<PalletItem> AllPalletItems { get; set; }
        public class PalletItemAdapter : BaseAdapter<PalletItem>
        {
            PalletItem[] items;
            Activity context;
            public PalletItemAdapter(Activity context, PalletItem[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override PalletItem this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Length; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView; // re-use an existing view, if one is available
                if (view == null) // otherwise create a new one
                    view = context.LayoutInflater.Inflate(Resource.Layout.D_ListViewItem, null);
                view.FindViewById<TextView>(Resource.Id.txtItemNo).Text = items[position].ItemCode;
                view.FindViewById<TextView>(Resource.Id.txtDesc).Text = items[position].ItemDesc;
                view.FindViewById<TextView>(Resource.Id.txtItemLot).Text = items[position].ItemLotNum;
                view.FindViewById<TextView>(Resource.Id.txtQty).Text = items[position].Qty;
                return view;
            }
        }
        public static List<string> PalletList { get; set; }
        public static string ScannedPallet { get; set; }
        #endregion

        #region Rec Transfer

        public static string RecTransType { get; set; }
        public static string RecTransProc { get; set; }

        #region General
        public static string RecTransWarehouseFrom { get; set; }
        public static string RecTransWarehouseTo { get; set; }
        public static string allWIPLines { get; set; }
        public class WIPItemDetail
        {
            public string ItemCode { get; set; }
            public string ItemDesc { get; set; }
            public string ItemLotNum { get; set; }
            public string Qty { get; set; }

            public int theme { get; set; }
        }
        public static List<WIPItemDetail> CurrentWIPItems { get; set; }
        public static WIPItemDetail selectedPPWIPItem { get; set; }
        public class WIPItemDetailAdapter : BaseAdapter<WIPItemDetail>
        {
            WIPItemDetail[] items;
            Activity context;
            public WIPItemDetailAdapter(Activity context, WIPItemDetail[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override WIPItemDetail this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Length; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView; // re-use an existing view, if one is available
                if (view == null) // otherwise create a new one
                    view = context.LayoutInflater.Inflate(Resource.Layout.D_ListViewItem, null);
                view.FindViewById<TextView>(Resource.Id.txtItemNo).Text = items[position].ItemCode;
                view.FindViewById<TextView>(Resource.Id.txtDesc).Text = items[position].ItemDesc;
                view.FindViewById<TextView>(Resource.Id.txtItemLot).Text = items[position].ItemLotNum;
                view.FindViewById<TextView>(Resource.Id.txtQty).Text = items[position].Qty;
                view.FindViewById<LinearLayout>(Resource.Id.Text).SetBackgroundResource(items[position].theme);
                return view;
            }
        }
        public static string RecTransItemCode { get; set; }
        public static string RecTransItemDesc { get; set; }
        public static string RecTransLotNumber { get; set; }
        public static string RecTransQty { get; set; }
        #endregion

        #region RT2D
        public static string RecTransUnq { get; set; }
        #endregion

        #region Fresh Slurry
        public static List<string> RecTransScannedFS { get; set; }
        public static string RccTransFSBarcode { get; set; }
        #endregion

        #region Mixed Slurry
        public static string RecTransTrolleyCode { get; set; }
        #endregion

        #region Zect
        public static string RecZectJob { get; set; }
        public static string RecTransTankCode { get; set; }
        #endregion

        #endregion

        #region Powder Prep
        public static string PPItemCode { get; set; }
        public static string PPItemDesc { get; set; }
        public static string PPLotNum { get; set; }

        #region PPtFD Transfer Out
        public static string PPtFSSlurryCode { get; set; }
        public static string PPtFSItemCode { get; set; }
        public static string PPtFSLotNum { get; set; }
        public static string PPtFSQty { get; set; }
        #endregion

        #endregion

        #region Fresh Slurry

        #region Manufacture
        public static string FSTrolleyCode { get; set; }
        public static string FSItemCode { get; set; }
        public static string FSItemDesc { get; set; }
        public static string FSLotNum { get; set; }

        public static bool FSReqRaw { get; set; }
        public static string FSRCode { get; set; }
        public static string FSRDesc { get; set; }
        public static string FSRLot { get; set; }
        public static string FSRQty { get; set; }
        #endregion

        #region Solidity
        public static string FSSTrolleyCode { get; set; }
        public static string FSSItemCode { get; set; }
        public static string FSSItemDesc { get; set; }
        public static string FSSLotNum { get; set; }
        #endregion

        #endregion

        #region Mixed Slurry

        #region General
        public static string MSTankType { get; set; }
        public static string MSTankCode { get; set; }
        public static string MSItemCode { get; set; }
        public static string MSItemDesc { get; set; }
        public static string MSLotNum { get; set; }
        #endregion

        #region Enter Remainder
        public static string MSRemWeight { get; set; }
        public static string MSRemLot { get; set; }

        public static string MSRemSol { get; set; }
        #endregion

        #region Enter Remainder
        public static string MSRecWeight { get; set; }
        public static string MSRecLot { get; set; }

        public static string MSRecSol { get; set; }
        #endregion

        #region Add Fresh Slurries
        public static string MSAllSlurries { get; set; }

        #region List 
        public class SlurryDetail
        {
            public string TrolleyCode { get; set; }
            public string ItemCode { get; set; }
            public string ItemDesc { get; set; }
            public string ItemLotNum { get; set; }
            public string weight { get; set; }
        }
        public static List<SlurryDetail> CurrentSlurries { get; set; }
        public class SlurryDetailAdapter : BaseAdapter<SlurryDetail>
        {
            SlurryDetail[] items;
            Activity context;
            public SlurryDetailAdapter(Activity context, SlurryDetail[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override SlurryDetail this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Length; }
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView; // re-use an existing view, if one is available
                if (view == null) // otherwise create a new one
                    view = context.LayoutInflater.Inflate(Resource.Layout.D_ListViewItem, null);
                view.FindViewById<TextView>(Resource.Id.txtItemNo).Text = items[position].ItemCode;
                view.FindViewById<TextView>(Resource.Id.txtDesc).Text = items[position].ItemDesc;
                view.FindViewById<TextView>(Resource.Id.txtItemLot).Text = items[position].ItemLotNum;
                view.FindViewById<TextView>(Resource.Id.txtQty).Text = items[position].weight;
                return view;
            }
        }
        #endregion

        public static string MSTrolleyCode { get; set; }
        public static string MSSlurryCode { get; set; }
        public static string MSSlurryDesc { get; set; }
        public static string MSSlurryLot { get; set; }
        public static string MSSlurryWeight { get; set; }
        #endregion

        #region Decant
        public static string MSDecantDesc { get; set; }
        public static string MSDecantLot { get; set; }

        public static string MSDecantTankType { get; set; }
        public static string MSDecantTankCode { get; set; }
        public static string MSDecantItemCode { get; set; }

        public static string MSDecantWeight { get; set; }
        #endregion

        #region ZAC
        public static string MSZACDesc { get; set; }
        public static string MSZACLot { get; set; }

        #region List View     
        public class ChemiclDetail
        {
            public string chemicalName { get; set; }
            public int theme { get; set; }
        }
        public static string selectedChemName { get; set; }
        public static ChemiclDetail selectedChem { get; set; }
        public static List<ChemiclDetail> AllChemicals { get; set; }
        public class ChemiclDetailAdapter : BaseAdapter<ChemiclDetail>
        {
            ChemiclDetail[] items;
            Activity context;
            public ChemiclDetailAdapter(Activity context, ChemiclDetail[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override ChemiclDetail this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Length; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView; // re-use an existing view, if one is available
                if (view == null) // otherwise create a new one
                    view = context.LayoutInflater.Inflate(Resource.Layout.G_ChemicalItem, null);
                view.FindViewById<TextView>(Resource.Id.txtChemical).Text = items[position].chemicalName;
                view.FindViewById<LinearLayout>(Resource.Id.Text).SetBackgroundResource(items[position].theme);
                return view;
            }
        }
        #endregion
        #endregion

        #region Solidity
        public static string MSDescSol { get; set; }
        public static string MSLotSol { get; set; }
        public static string MSWeightSol { get; set; }
        public static string MSSolidity { get; set; }
        #endregion
        #endregion

        #region Zect Transfers
        public static string ZTItemCode { get; set; }
        public static string ZTDescription { get; set; }
        public static string ZTLotNum { get; set; }
        public static string ZTQty { get; set; }
        public static string ZTUnq { get; set; }
        public static string ZTLineID { get; set; }
        public static string ZTWhseTo { get; set; }

        public class ZectWarehouseDetails
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }
        public static List<ZectWarehouseDetails> ZectTransferWarehouses { get; set; }
        #endregion

        #region A&W Transfers
        public static string AWJobNo { get; set; }
        public static string AWRMCode { get; set; }
        public static string AWRMLot { get; set; }
        public static string AWItemCode { get; set; }
        public static string AWLotNum { get; set; }
        public static string AWQty { get; set; }
        public static string AWUnq { get; set; }
        public static string AWLineID { get; set; }
        public static string AWWhseTo { get; set; }

        public class AWWarehouseDetails
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }
        public static List<AWWarehouseDetails> AWTransferWarehouses { get; set; }
        #endregion

        #region Canning Transfers
        public static string CanningItemCode { get; set; }
        public static string CanningLotNum { get; set; }
        public static string CanningQty { get; set; }
        public static string CanningUnq { get; set; }
        public static string CanningWhseTo { get; set; }
        public class CanningWarehouseDetails
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }
        public static List<CanningWarehouseDetails> CanningTransferWarehouses { get; set; }
        #endregion

        #region Transfers
        public static string ProcessName { get; set; }

        

        #region Mixed Slurry
        public class WarehouseDetails
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }
        public static List<WarehouseDetails> TransferWarehouses { get; set; }
        public static string MStZectTankCode { get; set; }
        public static string MStZectItemCode { get; set; }
        public static string MStZectItemDesc { get; set; }
        public static string MStZectLotNum { get; set; }
        public static string MStZectSQty { get; set; }
        public static string MStZectWarehouseTo { get; set; }
        #endregion

        #region To Prod
        public static string ToProdWarehouseTo { get; set; }
        public static string ToProdItemCode { get; set; }
        public static string ToProdLotNum { get; set; }
        public static string ToProdQty { get; set; }
        public static string ToProdUnq { get; set; }
        #endregion

        #endregion

        #region Dispatch

        public static string SONumber { get; set; }

        public static string _soDetails;
        public static List<SODetail> AllSOItems { get; set; }
        public class SODetail
        {
            public string ItemCode { get; set; }
            public string ItemDesc { get; set; }
            public string ItemLotNum { get; set; }
            public string OrderQty { get; set; }
            public string ProcQty { get; set; }
            public string ToProcQty { get; set; }
            public string TotalOrderQty { get; set; }
            public string lotItem { get; set; }
            public int theme { get; set; }
        }
        public class SODetailAdapter : BaseAdapter<SODetail>
        {
            SODetail[] items;
            Activity context;
            public SODetailAdapter(Activity context, SODetail[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override SODetail this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Length; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView; // re-use an existing view, if one is available
                if (view == null) // otherwise create a new one
                    view = context.LayoutInflater.Inflate(Resource.Layout.D_ListViewItem, null);
                view.FindViewById<TextView>(Resource.Id.txtItemNo).Text = items[position].ItemCode;
                view.FindViewById<TextView>(Resource.Id.txtDesc).Text = items[position].ItemDesc;
                view.FindViewById<TextView>(Resource.Id.txtItemLot).Text = items[position].ItemLotNum;
                //view.FindViewById<TextView>(Resource.Id.txtQty).Text = items[position].TotalRecQty + "/" + items[position].OrderQty;
                view.FindViewById<TextView>(Resource.Id.txtQty).Text = items[position].ToProcQty + "/" + items[position].TotalOrderQty;
                //view.FindViewById<ImageView>(Resource.Id.imgFound).SetImageBitmap(items[position].complete);
                view.FindViewById<LinearLayout>(Resource.Id.Text).SetBackgroundResource(items[position].theme);
                return view;
            }      
        }
        #endregion

        #region Stock Takes
        public static List<WarehouseDetailsST> STWarehouses { get; set; }
        public class WarehouseDetailsST
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }
        public static string STWarehouse { get; set; }
        public static List<STDetails> StockTakes { get; set; }
        public class STDetails
        {
            public string Number { get; set; }
        }
        public static string StockTake { get; set; }

        public static bool singleST { get; set; }
        public static bool doubleST { get; set; }
        public static bool recountST { get; set; }

        public enum STBarcodeType { powder, freshSlurry, MixedSlurryT, MixedSlurryM, MixedSlurryB, PGM, RT2D, Pallet, manual }
        public static STBarcodeType currentSTBarcode;

        public static string STItem { get; set; }
        public static string STLot { get; set; }
        public static string STQty { get; set; }
        public static string STUnq { get; set; }
        #endregion
    }
}