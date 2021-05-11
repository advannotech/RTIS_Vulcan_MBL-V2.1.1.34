package md53bb0abbe82940618994b59763f8291b6;


public class Q_Options
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("RTIS_Vulcan_MBL.Activities.Q_Palletizing.Q_Options, RTIS_Vulcan_MBL", Q_Options.class, __md_methods);
	}


	public Q_Options ()
	{
		super ();
		if (getClass () == Q_Options.class)
			mono.android.TypeManager.Activate ("RTIS_Vulcan_MBL.Activities.Q_Palletizing.Q_Options, RTIS_Vulcan_MBL", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
