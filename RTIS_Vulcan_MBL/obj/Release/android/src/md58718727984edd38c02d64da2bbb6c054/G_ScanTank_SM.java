package md58718727984edd38c02d64da2bbb6c054;


public class G_ScanTank_SM
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
		mono.android.Runtime.register ("RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry.G_ScanTank_SM, RTIS_Vulcan_MBL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", G_ScanTank_SM.class, __md_methods);
	}


	public G_ScanTank_SM () throws java.lang.Throwable
	{
		super ();
		if (getClass () == G_ScanTank_SM.class)
			mono.android.TypeManager.Activate ("RTIS_Vulcan_MBL.Activities.G_Mixed_Slurry.G_ScanTank_SM, RTIS_Vulcan_MBL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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
