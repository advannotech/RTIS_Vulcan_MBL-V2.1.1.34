package md56a734b77218182430f36a7eb2fd2a02a;


public class G_EnterLot_REM
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer,
		android.view.View.OnTouchListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onTouch:(Landroid/view/View;Landroid/view/MotionEvent;)Z:GetOnTouch_Landroid_view_View_Landroid_view_MotionEvent_Handler:Android.Views.View/IOnTouchListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry.Enter_Remaining.G_EnterLot_REM, RTIS_Vulcan_MBL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", G_EnterLot_REM.class, __md_methods);
	}


	public G_EnterLot_REM () throws java.lang.Throwable
	{
		super ();
		if (getClass () == G_EnterLot_REM.class)
			mono.android.TypeManager.Activate ("RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry.Enter_Remaining.G_EnterLot_REM, RTIS_Vulcan_MBL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public boolean onTouch (android.view.View p0, android.view.MotionEvent p1)
	{
		return n_onTouch (p0, p1);
	}

	private native boolean n_onTouch (android.view.View p0, android.view.MotionEvent p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
