	.arch	armv7-a
	.syntax unified
	.eabi_attribute 67, "2.09"	@ Tag_conformance
	.eabi_attribute 6, 10	@ Tag_CPU_arch
	.eabi_attribute 7, 65	@ Tag_CPU_arch_profile
	.eabi_attribute 8, 1	@ Tag_ARM_ISA_use
	.eabi_attribute 9, 2	@ Tag_THUMB_ISA_use
	.fpu	vfpv3-d16
	.eabi_attribute 34, 1	@ Tag_CPU_unaligned_access
	.eabi_attribute 15, 1	@ Tag_ABI_PCS_RW_data
	.eabi_attribute 16, 1	@ Tag_ABI_PCS_RO_data
	.eabi_attribute 17, 2	@ Tag_ABI_PCS_GOT_use
	.eabi_attribute 20, 2	@ Tag_ABI_FP_denormal
	.eabi_attribute 21, 0	@ Tag_ABI_FP_exceptions
	.eabi_attribute 23, 3	@ Tag_ABI_FP_number_model
	.eabi_attribute 24, 1	@ Tag_ABI_align_needed
	.eabi_attribute 25, 1	@ Tag_ABI_align_preserved
	.eabi_attribute 38, 1	@ Tag_ABI_FP_16bit_format
	.eabi_attribute 18, 4	@ Tag_ABI_PCS_wchar_t
	.eabi_attribute 26, 2	@ Tag_ABI_enum_size
	.eabi_attribute 14, 0	@ Tag_ABI_PCS_R9_use
	.file	"typemaps.armeabi-v7a.s"

/* map_module_count: START */
	.section	.rodata.map_module_count,"a",%progbits
	.type	map_module_count, %object
	.p2align	2
	.global	map_module_count
map_module_count:
	.size	map_module_count, 4
	.long	2
/* map_module_count: END */

/* java_type_count: START */
	.section	.rodata.java_type_count,"a",%progbits
	.type	java_type_count, %object
	.p2align	2
	.global	java_type_count
java_type_count:
	.size	java_type_count, 4
	.long	282
/* java_type_count: END */

	.include	"typemaps.armeabi-v7a-shared.inc"
	.include	"typemaps.armeabi-v7a-managed.inc"

/* Managed to Java map: START */
	.section	.data.rel.map_modules,"aw",%progbits
	.type	map_modules, %object
	.p2align	2
	.global	map_modules
map_modules:
	/* module_uuid: 9758981b-c8cd-483f-89a7-95c82fe7602e */
	.byte	0x1b, 0x98, 0x58, 0x97, 0xcd, 0xc8, 0x3f, 0x48, 0x89, 0xa7, 0x95, 0xc8, 0x2f, 0xe7, 0x60, 0x2e
	/* entry_count */
	.long	78
	/* duplicate_count */
	.long	0
	/* map */
	.long	module0_managed_to_java
	/* duplicate_map */
	.long	0
	/* assembly_name: RTIS_Vulcan_MBL */
	.long	.L.map_aname.0
	/* image */
	.long	0
	/* java_name_width */
	.long	0
	/* java_map */
	.long	0

	/* module_uuid: 7c793e8e-0480-4fdf-b188-92b38f62e8b3 */
	.byte	0x8e, 0x3e, 0x79, 0x7c, 0x80, 0x04, 0xdf, 0x4f, 0xb1, 0x88, 0x92, 0xb3, 0x8f, 0x62, 0xe8, 0xb3
	/* entry_count */
	.long	204
	/* duplicate_count */
	.long	35
	/* map */
	.long	module1_managed_to_java
	/* duplicate_map */
	.long	module1_managed_to_java_duplicates
	/* assembly_name: Mono.Android */
	.long	.L.map_aname.1
	/* image */
	.long	0
	/* java_name_width */
	.long	0
	/* java_map */
	.long	0

	.size	map_modules, 96
/* Managed to Java map: END */

/* Java to managed map: START */
	.section	.rodata.map_java,"a",%progbits
	.type	map_java, %object
	.p2align	2
	.global	map_java
map_java:
	/* #0 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554730
	/* java_name */
	.ascii	"android/app/Activity"
	.zero	50
	.zero	2

	/* #1 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554731
	/* java_name */
	.ascii	"android/app/AlertDialog"
	.zero	47
	.zero	2

	/* #2 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554732
	/* java_name */
	.ascii	"android/app/AlertDialog$Builder"
	.zero	39
	.zero	2

	/* #3 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554733
	/* java_name */
	.ascii	"android/app/Application"
	.zero	47
	.zero	2

	/* #4 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554734
	/* java_name */
	.ascii	"android/app/Dialog"
	.zero	52
	.zero	2

	/* #5 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554736
	/* java_name */
	.ascii	"android/app/ProgressDialog"
	.zero	44
	.zero	2

	/* #6 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554743
	/* java_name */
	.ascii	"android/content/BroadcastReceiver"
	.zero	37
	.zero	2

	/* #7 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554750
	/* java_name */
	.ascii	"android/content/ComponentCallbacks"
	.zero	36
	.zero	2

	/* #8 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554752
	/* java_name */
	.ascii	"android/content/ComponentCallbacks2"
	.zero	35
	.zero	2

	/* #9 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554745
	/* java_name */
	.ascii	"android/content/ComponentName"
	.zero	41
	.zero	2

	/* #10 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554741
	/* java_name */
	.ascii	"android/content/Context"
	.zero	47
	.zero	2

	/* #11 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554747
	/* java_name */
	.ascii	"android/content/ContextWrapper"
	.zero	40
	.zero	2

	/* #12 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554758
	/* java_name */
	.ascii	"android/content/DialogInterface"
	.zero	39
	.zero	2

	/* #13 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554754
	/* java_name */
	.ascii	"android/content/DialogInterface$OnClickListener"
	.zero	23
	.zero	2

	/* #14 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554742
	/* java_name */
	.ascii	"android/content/Intent"
	.zero	48
	.zero	2

	/* #15 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554759
	/* java_name */
	.ascii	"android/content/IntentFilter"
	.zero	42
	.zero	2

	/* #16 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554765
	/* java_name */
	.ascii	"android/content/SharedPreferences"
	.zero	37
	.zero	2

	/* #17 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554761
	/* java_name */
	.ascii	"android/content/SharedPreferences$Editor"
	.zero	30
	.zero	2

	/* #18 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554763
	/* java_name */
	.ascii	"android/content/SharedPreferences$OnSharedPreferenceChangeListener"
	.zero	4
	.zero	2

	/* #19 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554767
	/* java_name */
	.ascii	"android/content/pm/PackageInfo"
	.zero	40
	.zero	2

	/* #20 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554769
	/* java_name */
	.ascii	"android/content/pm/PackageManager"
	.zero	37
	.zero	2

	/* #21 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554771
	/* java_name */
	.ascii	"android/content/res/Configuration"
	.zero	37
	.zero	2

	/* #22 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554772
	/* java_name */
	.ascii	"android/content/res/Resources"
	.zero	41
	.zero	2

	/* #23 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554533
	/* java_name */
	.ascii	"android/database/DataSetObserver"
	.zero	38
	.zero	2

	/* #24 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554718
	/* java_name */
	.ascii	"android/graphics/Bitmap"
	.zero	47
	.zero	2

	/* #25 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554721
	/* java_name */
	.ascii	"android/graphics/BitmapFactory"
	.zero	40
	.zero	2

	/* #26 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554722
	/* java_name */
	.ascii	"android/graphics/Point"
	.zero	48
	.zero	2

	/* #27 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554723
	/* java_name */
	.ascii	"android/graphics/Rect"
	.zero	49
	.zero	2

	/* #28 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554724
	/* java_name */
	.ascii	"android/graphics/drawable/Drawable"
	.zero	36
	.zero	2

	/* #29 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554726
	/* java_name */
	.ascii	"android/graphics/drawable/Drawable$Callback"
	.zero	27
	.zero	2

	/* #30 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554716
	/* java_name */
	.ascii	"android/media/MediaPlayer"
	.zero	45
	.zero	2

	/* #31 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554715
	/* java_name */
	.ascii	"android/net/wifi/WifiInfo"
	.zero	45
	.zero	2

	/* #32 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554714
	/* java_name */
	.ascii	"android/net/wifi/WifiManager"
	.zero	42
	.zero	2

	/* #33 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554706
	/* java_name */
	.ascii	"android/os/BaseBundle"
	.zero	49
	.zero	2

	/* #34 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554707
	/* java_name */
	.ascii	"android/os/Bundle"
	.zero	53
	.zero	2

	/* #35 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554708
	/* java_name */
	.ascii	"android/os/Environment"
	.zero	48
	.zero	2

	/* #36 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554705
	/* java_name */
	.ascii	"android/os/Handler"
	.zero	52
	.zero	2

	/* #37 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554711
	/* java_name */
	.ascii	"android/os/Looper"
	.zero	53
	.zero	2

	/* #38 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554712
	/* java_name */
	.ascii	"android/os/Parcel"
	.zero	53
	.zero	2

	/* #39 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554710
	/* java_name */
	.ascii	"android/os/Parcelable"
	.zero	49
	.zero	2

	/* #40 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554818
	/* java_name */
	.ascii	"android/runtime/JavaProxyThrowable"
	.zero	36
	.zero	2

	/* #41 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554687
	/* java_name */
	.ascii	"android/text/Editable"
	.zero	49
	.zero	2

	/* #42 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554690
	/* java_name */
	.ascii	"android/text/GetChars"
	.zero	49
	.zero	2

	/* #43 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554693
	/* java_name */
	.ascii	"android/text/InputFilter"
	.zero	46
	.zero	2

	/* #44 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554695
	/* java_name */
	.ascii	"android/text/NoCopySpan"
	.zero	47
	.zero	2

	/* #45 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554697
	/* java_name */
	.ascii	"android/text/Spannable"
	.zero	48
	.zero	2

	/* #46 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554700
	/* java_name */
	.ascii	"android/text/Spanned"
	.zero	50
	.zero	2

	/* #47 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554703
	/* java_name */
	.ascii	"android/text/TextWatcher"
	.zero	46
	.zero	2

	/* #48 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554685
	/* java_name */
	.ascii	"android/util/AttributeSet"
	.zero	45
	.zero	2

	/* #49 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554683
	/* java_name */
	.ascii	"android/util/DisplayMetrics"
	.zero	43
	.zero	2

	/* #50 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554627
	/* java_name */
	.ascii	"android/view/ActionMode"
	.zero	47
	.zero	2

	/* #51 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554629
	/* java_name */
	.ascii	"android/view/ActionMode$Callback"
	.zero	38
	.zero	2

	/* #52 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554631
	/* java_name */
	.ascii	"android/view/ActionProvider"
	.zero	43
	.zero	2

	/* #53 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554640
	/* java_name */
	.ascii	"android/view/ContextMenu"
	.zero	46
	.zero	2

	/* #54 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554638
	/* java_name */
	.ascii	"android/view/ContextMenu$ContextMenuInfo"
	.zero	30
	.zero	2

	/* #55 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554633
	/* java_name */
	.ascii	"android/view/ContextThemeWrapper"
	.zero	38
	.zero	2

	/* #56 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554634
	/* java_name */
	.ascii	"android/view/Display"
	.zero	50
	.zero	2

	/* #57 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554649
	/* java_name */
	.ascii	"android/view/InputEvent"
	.zero	47
	.zero	2

	/* #58 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554608
	/* java_name */
	.ascii	"android/view/KeyEvent"
	.zero	49
	.zero	2

	/* #59 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554610
	/* java_name */
	.ascii	"android/view/KeyEvent$Callback"
	.zero	40
	.zero	2

	/* #60 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554611
	/* java_name */
	.ascii	"android/view/LayoutInflater"
	.zero	43
	.zero	2

	/* #61 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554613
	/* java_name */
	.ascii	"android/view/LayoutInflater$Factory"
	.zero	35
	.zero	2

	/* #62 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554615
	/* java_name */
	.ascii	"android/view/LayoutInflater$Factory2"
	.zero	34
	.zero	2

	/* #63 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554642
	/* java_name */
	.ascii	"android/view/Menu"
	.zero	53
	.zero	2

	/* #64 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554648
	/* java_name */
	.ascii	"android/view/MenuItem"
	.zero	49
	.zero	2

	/* #65 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554644
	/* java_name */
	.ascii	"android/view/MenuItem$OnActionExpandListener"
	.zero	26
	.zero	2

	/* #66 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554646
	/* java_name */
	.ascii	"android/view/MenuItem$OnMenuItemClickListener"
	.zero	25
	.zero	2

	/* #67 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554616
	/* java_name */
	.ascii	"android/view/MotionEvent"
	.zero	46
	.zero	2

	/* #68 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554652
	/* java_name */
	.ascii	"android/view/SubMenu"
	.zero	50
	.zero	2

	/* #69 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554592
	/* java_name */
	.ascii	"android/view/View"
	.zero	53
	.zero	2

	/* #70 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554594
	/* java_name */
	.ascii	"android/view/View$OnClickListener"
	.zero	37
	.zero	2

	/* #71 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554597
	/* java_name */
	.ascii	"android/view/View$OnCreateContextMenuListener"
	.zero	25
	.zero	2

	/* #72 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554599
	/* java_name */
	.ascii	"android/view/View$OnKeyListener"
	.zero	39
	.zero	2

	/* #73 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554603
	/* java_name */
	.ascii	"android/view/View$OnTouchListener"
	.zero	37
	.zero	2

	/* #74 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554671
	/* java_name */
	.ascii	"android/view/ViewGroup"
	.zero	48
	.zero	2

	/* #75 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554672
	/* java_name */
	.ascii	"android/view/ViewGroup$LayoutParams"
	.zero	35
	.zero	2

	/* #76 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554654
	/* java_name */
	.ascii	"android/view/ViewManager"
	.zero	46
	.zero	2

	/* #77 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554656
	/* java_name */
	.ascii	"android/view/ViewParent"
	.zero	47
	.zero	2

	/* #78 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554617
	/* java_name */
	.ascii	"android/view/ViewTreeObserver"
	.zero	41
	.zero	2

	/* #79 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554619
	/* java_name */
	.ascii	"android/view/ViewTreeObserver$OnGlobalLayoutListener"
	.zero	18
	.zero	2

	/* #80 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554621
	/* java_name */
	.ascii	"android/view/ViewTreeObserver$OnPreDrawListener"
	.zero	23
	.zero	2

	/* #81 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554623
	/* java_name */
	.ascii	"android/view/ViewTreeObserver$OnTouchModeChangeListener"
	.zero	15
	.zero	2

	/* #82 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554624
	/* java_name */
	.ascii	"android/view/Window"
	.zero	51
	.zero	2

	/* #83 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554626
	/* java_name */
	.ascii	"android/view/Window$Callback"
	.zero	42
	.zero	2

	/* #84 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554659
	/* java_name */
	.ascii	"android/view/WindowManager"
	.zero	44
	.zero	2

	/* #85 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554657
	/* java_name */
	.ascii	"android/view/WindowManager$LayoutParams"
	.zero	31
	.zero	2

	/* #86 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554676
	/* java_name */
	.ascii	"android/view/accessibility/AccessibilityEvent"
	.zero	25
	.zero	2

	/* #87 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554682
	/* java_name */
	.ascii	"android/view/accessibility/AccessibilityEventSource"
	.zero	19
	.zero	2

	/* #88 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554677
	/* java_name */
	.ascii	"android/view/accessibility/AccessibilityRecord"
	.zero	24
	.zero	2

	/* #89 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554535
	/* java_name */
	.ascii	"android/widget/AbsListView"
	.zero	44
	.zero	2

	/* #90 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554554
	/* java_name */
	.ascii	"android/widget/AbsSpinner"
	.zero	45
	.zero	2

	/* #91 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554576
	/* java_name */
	.ascii	"android/widget/Adapter"
	.zero	48
	.zero	2

	/* #92 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554537
	/* java_name */
	.ascii	"android/widget/AdapterView"
	.zero	44
	.zero	2

	/* #93 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554539
	/* java_name */
	.ascii	"android/widget/AdapterView$OnItemClickListener"
	.zero	24
	.zero	2

	/* #94 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554543
	/* java_name */
	.ascii	"android/widget/AdapterView$OnItemSelectedListener"
	.zero	21
	.zero	2

	/* #95 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554557
	/* java_name */
	.ascii	"android/widget/ArrayAdapter"
	.zero	43
	.zero	2

	/* #96 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	0
	/* java_name */
	.ascii	"android/widget/BaseAdapter"
	.zero	44
	.zero	2

	/* #97 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554560
	/* java_name */
	.ascii	"android/widget/Button"
	.zero	49
	.zero	2

	/* #98 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554561
	/* java_name */
	.ascii	"android/widget/CheckBox"
	.zero	47
	.zero	2

	/* #99 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554578
	/* java_name */
	.ascii	"android/widget/Checkable"
	.zero	46
	.zero	2

	/* #100 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554562
	/* java_name */
	.ascii	"android/widget/CompoundButton"
	.zero	41
	.zero	2

	/* #101 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554564
	/* java_name */
	.ascii	"android/widget/CompoundButton$OnCheckedChangeListener"
	.zero	17
	.zero	2

	/* #102 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554570
	/* java_name */
	.ascii	"android/widget/EditText"
	.zero	47
	.zero	2

	/* #103 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554571
	/* java_name */
	.ascii	"android/widget/Filter"
	.zero	49
	.zero	2

	/* #104 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554573
	/* java_name */
	.ascii	"android/widget/Filter$FilterListener"
	.zero	34
	.zero	2

	/* #105 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554580
	/* java_name */
	.ascii	"android/widget/Filterable"
	.zero	45
	.zero	2

	/* #106 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554583
	/* java_name */
	.ascii	"android/widget/ImageView"
	.zero	46
	.zero	2

	/* #107 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554586
	/* java_name */
	.ascii	"android/widget/LinearLayout"
	.zero	43
	.zero	2

	/* #108 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554582
	/* java_name */
	.ascii	"android/widget/ListAdapter"
	.zero	44
	.zero	2

	/* #109 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554587
	/* java_name */
	.ascii	"android/widget/ListView"
	.zero	47
	.zero	2

	/* #110 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554588
	/* java_name */
	.ascii	"android/widget/RadioButton"
	.zero	44
	.zero	2

	/* #111 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554589
	/* java_name */
	.ascii	"android/widget/Spinner"
	.zero	48
	.zero	2

	/* #112 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554585
	/* java_name */
	.ascii	"android/widget/SpinnerAdapter"
	.zero	41
	.zero	2

	/* #113 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554553
	/* java_name */
	.ascii	"android/widget/TextView"
	.zero	47
	.zero	2

	/* #114 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554590
	/* java_name */
	.ascii	"android/widget/Toast"
	.zero	50
	.zero	2

	/* #115 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554495
	/* java_name */
	.ascii	"crc6421ec20e922cf268e/G_EnterQty_ZAC"
	.zero	34
	.zero	2

	/* #116 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554496
	/* java_name */
	.ascii	"crc6421ec20e922cf268e/G_SelectChem_ZAC"
	.zero	32
	.zero	2

	/* #117 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554497
	/* java_name */
	.ascii	"crc643aef748381d50d63/G_EnterSolidity_Sol"
	.zero	29
	.zero	2

	/* #118 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554498
	/* java_name */
	.ascii	"crc643aef748381d50d63/G_EnterWetWeight_Sol"
	.zero	28
	.zero	2

	/* #119 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554499
	/* java_name */
	.ascii	"crc643aef748381d50d63/G_ScanTank_Sol"
	.zero	34
	.zero	2

	/* #120 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554507
	/* java_name */
	.ascii	"crc644044a8ee476610fd/G_ScanTrolley_FS"
	.zero	32
	.zero	2

	/* #121 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554508
	/* java_name */
	.ascii	"crc644044a8ee476610fd/G_SlurryView_FS"
	.zero	33
	.zero	2

	/* #122 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554482
	/* java_name */
	.ascii	"crc6460757586539d716f/H_Scan_RT2D_Canning"
	.zero	29
	.zero	2

	/* #123 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554483
	/* java_name */
	.ascii	"crc6460757586539d716f/H_SelectCanningWhse"
	.zero	29
	.zero	2

	/* #124 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554484
	/* java_name */
	.ascii	"crc64656cdd845491c3b1/H_ScanTank_MS"
	.zero	35
	.zero	2

	/* #125 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554522
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/GlobalVar_ChemiclDetailAdapter"
	.zero	18
	.zero	2

	/* #126 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554514
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/GlobalVar_OrderDetailAdapter"
	.zero	20
	.zero	2

	/* #127 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554516
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/GlobalVar_PalletItemAdapter"
	.zero	21
	.zero	2

	/* #128 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554528
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/GlobalVar_SODetailAdapter"
	.zero	23
	.zero	2

	/* #129 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554520
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/GlobalVar_SlurryDetailAdapter"
	.zero	19
	.zero	2

	/* #130 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554518
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/GlobalVar_WIPItemDetailAdapter"
	.zero	18
	.zero	2

	/* #131 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554434
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/Login"
	.zero	43
	.zero	2

	/* #132 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554435
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/MainActivity"
	.zero	36
	.zero	2

	/* #133 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554442
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/RTSettings"
	.zero	38
	.zero	2

	/* #134 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554437
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/ScanDocument"
	.zero	36
	.zero	2

	/* #135 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554438
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/ScanItems"
	.zero	39
	.zero	2

	/* #136 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554436
	/* java_name */
	.ascii	"crc6481dcc08d204e0cea/SettingsPassword"
	.zero	32
	.zero	2

	/* #137 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554502
	/* java_name */
	.ascii	"crc6485b61643270f881a/G_EnterLot_Rec"
	.zero	34
	.zero	2

	/* #138 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554503
	/* java_name */
	.ascii	"crc6485b61643270f881a/G_EnterSol_Rec"
	.zero	34
	.zero	2

	/* #139 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554472
	/* java_name */
	.ascii	"crc6490cfb10e33bfca7e/P_ScanItems"
	.zero	37
	.zero	2

	/* #140 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554473
	/* java_name */
	.ascii	"crc6490cfb10e33bfca7e/P_ScanSO"
	.zero	40
	.zero	2

	/* #141 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554509
	/* java_name */
	.ascii	"crc6497136a7be26e327a/F_ScanPowder"
	.zero	36
	.zero	2

	/* #142 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554510
	/* java_name */
	.ascii	"crc6497136a7be26e327a/F_ScanPowderQty"
	.zero	33
	.zero	2

	/* #143 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554474
	/* java_name */
	.ascii	"crc64aa3342e590966aba/ScanZectItem"
	.zero	36
	.zero	2

	/* #144 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554475
	/* java_name */
	.ascii	"crc64aa3342e590966aba/ScanZectJob"
	.zero	37
	.zero	2

	/* #145 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554445
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/B_Sub_Menu"
	.zero	38
	.zero	2

	/* #146 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554449
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/E_ScanBCD"
	.zero	39
	.zero	2

	/* #147 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554450
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/E_ScanWeight"
	.zero	36
	.zero	2

	/* #148 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554455
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/FS_Enter_Solidity"
	.zero	31
	.zero	2

	/* #149 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554454
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/FS_Scan_Trolley"
	.zero	33
	.zero	2

	/* #150 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554452
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/F_Enter_Lot_Number"
	.zero	30
	.zero	2

	/* #151 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554453
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/F_Enter_Wet_Weight"
	.zero	30
	.zero	2

	/* #152 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554451
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/F_Scan_Trolley"
	.zero	34
	.zero	2

	/* #153 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554462
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_ScanBCD_PP"
	.zero	36
	.zero	2

	/* #154 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554463
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_ScanDryWeight_PP"
	.zero	30
	.zero	2

	/* #155 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554458
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_Scan_Item_ToProd"
	.zero	30
	.zero	2

	/* #156 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554464
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_Scan_Prep_Sheet_PP"
	.zero	28
	.zero	2

	/* #157 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554456
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_Scan_RT2D_AW"
	.zero	34
	.zero	2

	/* #158 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554461
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_Scan_RT2D_Barcode"
	.zero	29
	.zero	2

	/* #159 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554465
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_Scan_Trolley"
	.zero	34
	.zero	2

	/* #160 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554460
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_SelectZectWhse"
	.zero	32
	.zero	2

	/* #161 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554457
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_Select_AWWhse"
	.zero	33
	.zero	2

	/* #162 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554466
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_Select_MS_Whse"
	.zero	32
	.zero	2

	/* #163 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554459
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/H_Select_Whse_ToProd"
	.zero	28
	.zero	2

	/* #164 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554468
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/N_ScanJobTag"
	.zero	36
	.zero	2

	/* #165 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554467
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/N_ScanRawMaterial"
	.zero	31
	.zero	2

	/* #166 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554446
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/ScanItem_RecTrans"
	.zero	31
	.zero	2

	/* #167 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554447
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/ScanQty_FSRec"
	.zero	35
	.zero	2

	/* #168 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554448
	/* java_name */
	.ascii	"crc64ac4129b53d1d6418/SelectWhseFrom"
	.zero	34
	.zero	2

	/* #169 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554476
	/* java_name */
	.ascii	"crc64bc006d815ceadf74/J_EnterQty"
	.zero	38
	.zero	2

	/* #170 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554477
	/* java_name */
	.ascii	"crc64bc006d815ceadf74/J_ScanEnterLot"
	.zero	34
	.zero	2

	/* #171 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554478
	/* java_name */
	.ascii	"crc64bc006d815ceadf74/J_ScanItem"
	.zero	38
	.zero	2

	/* #172 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554479
	/* java_name */
	.ascii	"crc64bc006d815ceadf74/J_ScanQty"
	.zero	39
	.zero	2

	/* #173 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554480
	/* java_name */
	.ascii	"crc64bc006d815ceadf74/J_ScanTicket"
	.zero	36
	.zero	2

	/* #174 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554481
	/* java_name */
	.ascii	"crc64bc006d815ceadf74/J_SelectStockTake"
	.zero	31
	.zero	2

	/* #175 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554500
	/* java_name */
	.ascii	"crc64cb86a298923b04bb/G_EnterLot_REM"
	.zero	34
	.zero	2

	/* #176 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554501
	/* java_name */
	.ascii	"crc64cb86a298923b04bb/G_EnterSol_REM"
	.zero	34
	.zero	2

	/* #177 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554469
	/* java_name */
	.ascii	"crc64ce794921513a9d4e/Q_ScanItem"
	.zero	38
	.zero	2

	/* #178 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554470
	/* java_name */
	.ascii	"crc64ce794921513a9d4e/Q_ScanItems"
	.zero	37
	.zero	2

	/* #179 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554471
	/* java_name */
	.ascii	"crc64ce794921513a9d4e/Q_Scan_Pallet"
	.zero	35
	.zero	2

	/* #180 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554493
	/* java_name */
	.ascii	"crc64cfa3c6614224726a/G_EnterLot_SM"
	.zero	35
	.zero	2

	/* #181 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554487
	/* java_name */
	.ascii	"crc64cfa3c6614224726a/G_EnterQty_REM"
	.zero	34
	.zero	2

	/* #182 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554489
	/* java_name */
	.ascii	"crc64cfa3c6614224726a/G_EnterQty_Rec"
	.zero	34
	.zero	2

	/* #183 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554491
	/* java_name */
	.ascii	"crc64cfa3c6614224726a/G_EnterSlurryWeight_FS"
	.zero	26
	.zero	2

	/* #184 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554485
	/* java_name */
	.ascii	"crc64cfa3c6614224726a/G_Menu"
	.zero	42
	.zero	2

	/* #185 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554490
	/* java_name */
	.ascii	"crc64cfa3c6614224726a/G_ScanTank_FS"
	.zero	35
	.zero	2

	/* #186 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554486
	/* java_name */
	.ascii	"crc64cfa3c6614224726a/G_ScanTank_REM"
	.zero	34
	.zero	2

	/* #187 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554488
	/* java_name */
	.ascii	"crc64cfa3c6614224726a/G_ScanTank_Rec"
	.zero	34
	.zero	2

	/* #188 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554494
	/* java_name */
	.ascii	"crc64cfa3c6614224726a/G_ScanTank_SM"
	.zero	35
	.zero	2

	/* #189 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554492
	/* java_name */
	.ascii	"crc64cfa3c6614224726a/G_ScanTank_ZAC"
	.zero	34
	.zero	2

	/* #190 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554504
	/* java_name */
	.ascii	"crc64fce1ebf6832fdc10/G_EnterWeight_Dec"
	.zero	31
	.zero	2

	/* #191 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554505
	/* java_name */
	.ascii	"crc64fce1ebf6832fdc10/G_ScanMobTank_Dec"
	.zero	31
	.zero	2

	/* #192 */
	/* module_index */
	.long	0
	/* type_token_id */
	.long	33554506
	/* java_name */
	.ascii	"crc64fce1ebf6832fdc10/G_ScanTank_Dec"
	.zero	34
	.zero	2

	/* #193 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554935
	/* java_name */
	.ascii	"java/io/Closeable"
	.zero	53
	.zero	2

	/* #194 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554932
	/* java_name */
	.ascii	"java/io/File"
	.zero	58
	.zero	2

	/* #195 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554933
	/* java_name */
	.ascii	"java/io/FileInputStream"
	.zero	47
	.zero	2

	/* #196 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554937
	/* java_name */
	.ascii	"java/io/Flushable"
	.zero	53
	.zero	2

	/* #197 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554940
	/* java_name */
	.ascii	"java/io/IOException"
	.zero	51
	.zero	2

	/* #198 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554938
	/* java_name */
	.ascii	"java/io/InputStream"
	.zero	51
	.zero	2

	/* #199 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554943
	/* java_name */
	.ascii	"java/io/OutputStream"
	.zero	50
	.zero	2

	/* #200 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554945
	/* java_name */
	.ascii	"java/io/PrintWriter"
	.zero	51
	.zero	2

	/* #201 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554942
	/* java_name */
	.ascii	"java/io/Serializable"
	.zero	50
	.zero	2

	/* #202 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554946
	/* java_name */
	.ascii	"java/io/StringWriter"
	.zero	50
	.zero	2

	/* #203 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554947
	/* java_name */
	.ascii	"java/io/Writer"
	.zero	56
	.zero	2

	/* #204 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554907
	/* java_name */
	.ascii	"java/lang/Appendable"
	.zero	50
	.zero	2

	/* #205 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554885
	/* java_name */
	.ascii	"java/lang/Boolean"
	.zero	53
	.zero	2

	/* #206 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554886
	/* java_name */
	.ascii	"java/lang/Byte"
	.zero	56
	.zero	2

	/* #207 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554908
	/* java_name */
	.ascii	"java/lang/CharSequence"
	.zero	48
	.zero	2

	/* #208 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554887
	/* java_name */
	.ascii	"java/lang/Character"
	.zero	51
	.zero	2

	/* #209 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554888
	/* java_name */
	.ascii	"java/lang/Class"
	.zero	55
	.zero	2

	/* #210 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554904
	/* java_name */
	.ascii	"java/lang/ClassCastException"
	.zero	42
	.zero	2

	/* #211 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554889
	/* java_name */
	.ascii	"java/lang/ClassNotFoundException"
	.zero	38
	.zero	2

	/* #212 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554911
	/* java_name */
	.ascii	"java/lang/Cloneable"
	.zero	51
	.zero	2

	/* #213 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554913
	/* java_name */
	.ascii	"java/lang/Comparable"
	.zero	50
	.zero	2

	/* #214 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554890
	/* java_name */
	.ascii	"java/lang/Double"
	.zero	54
	.zero	2

	/* #215 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554905
	/* java_name */
	.ascii	"java/lang/Error"
	.zero	55
	.zero	2

	/* #216 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554891
	/* java_name */
	.ascii	"java/lang/Exception"
	.zero	51
	.zero	2

	/* #217 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554892
	/* java_name */
	.ascii	"java/lang/Float"
	.zero	55
	.zero	2

	/* #218 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554914
	/* java_name */
	.ascii	"java/lang/IllegalArgumentException"
	.zero	36
	.zero	2

	/* #219 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554915
	/* java_name */
	.ascii	"java/lang/IllegalStateException"
	.zero	39
	.zero	2

	/* #220 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554916
	/* java_name */
	.ascii	"java/lang/IndexOutOfBoundsException"
	.zero	35
	.zero	2

	/* #221 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554894
	/* java_name */
	.ascii	"java/lang/Integer"
	.zero	53
	.zero	2

	/* #222 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554919
	/* java_name */
	.ascii	"java/lang/LinkageError"
	.zero	48
	.zero	2

	/* #223 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554895
	/* java_name */
	.ascii	"java/lang/Long"
	.zero	56
	.zero	2

	/* #224 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554920
	/* java_name */
	.ascii	"java/lang/NoClassDefFoundError"
	.zero	40
	.zero	2

	/* #225 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554921
	/* java_name */
	.ascii	"java/lang/NullPointerException"
	.zero	40
	.zero	2

	/* #226 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554922
	/* java_name */
	.ascii	"java/lang/Number"
	.zero	54
	.zero	2

	/* #227 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554896
	/* java_name */
	.ascii	"java/lang/Object"
	.zero	54
	.zero	2

	/* #228 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554924
	/* java_name */
	.ascii	"java/lang/ReflectiveOperationException"
	.zero	32
	.zero	2

	/* #229 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554918
	/* java_name */
	.ascii	"java/lang/Runnable"
	.zero	52
	.zero	2

	/* #230 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554897
	/* java_name */
	.ascii	"java/lang/RuntimeException"
	.zero	44
	.zero	2

	/* #231 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554898
	/* java_name */
	.ascii	"java/lang/Short"
	.zero	55
	.zero	2

	/* #232 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554899
	/* java_name */
	.ascii	"java/lang/String"
	.zero	54
	.zero	2

	/* #233 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554901
	/* java_name */
	.ascii	"java/lang/Thread"
	.zero	54
	.zero	2

	/* #234 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554903
	/* java_name */
	.ascii	"java/lang/Throwable"
	.zero	51
	.zero	2

	/* #235 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554925
	/* java_name */
	.ascii	"java/lang/UnsupportedOperationException"
	.zero	31
	.zero	2

	/* #236 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554927
	/* java_name */
	.ascii	"java/lang/reflect/GenericDeclaration"
	.zero	34
	.zero	2

	/* #237 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554929
	/* java_name */
	.ascii	"java/lang/reflect/Type"
	.zero	48
	.zero	2

	/* #238 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554931
	/* java_name */
	.ascii	"java/lang/reflect/TypeVariable"
	.zero	40
	.zero	2

	/* #239 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554842
	/* java_name */
	.ascii	"java/net/InetSocketAddress"
	.zero	44
	.zero	2

	/* #240 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554843
	/* java_name */
	.ascii	"java/net/Proxy"
	.zero	56
	.zero	2

	/* #241 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554844
	/* java_name */
	.ascii	"java/net/ProxySelector"
	.zero	48
	.zero	2

	/* #242 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554846
	/* java_name */
	.ascii	"java/net/SocketAddress"
	.zero	48
	.zero	2

	/* #243 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554848
	/* java_name */
	.ascii	"java/net/URI"
	.zero	58
	.zero	2

	/* #244 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554863
	/* java_name */
	.ascii	"java/nio/Buffer"
	.zero	55
	.zero	2

	/* #245 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554865
	/* java_name */
	.ascii	"java/nio/ByteBuffer"
	.zero	51
	.zero	2

	/* #246 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554870
	/* java_name */
	.ascii	"java/nio/channels/ByteChannel"
	.zero	41
	.zero	2

	/* #247 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554872
	/* java_name */
	.ascii	"java/nio/channels/Channel"
	.zero	45
	.zero	2

	/* #248 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554867
	/* java_name */
	.ascii	"java/nio/channels/FileChannel"
	.zero	41
	.zero	2

	/* #249 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554874
	/* java_name */
	.ascii	"java/nio/channels/GatheringByteChannel"
	.zero	32
	.zero	2

	/* #250 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554876
	/* java_name */
	.ascii	"java/nio/channels/InterruptibleChannel"
	.zero	32
	.zero	2

	/* #251 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554878
	/* java_name */
	.ascii	"java/nio/channels/ReadableByteChannel"
	.zero	33
	.zero	2

	/* #252 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554880
	/* java_name */
	.ascii	"java/nio/channels/ScatteringByteChannel"
	.zero	31
	.zero	2

	/* #253 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554882
	/* java_name */
	.ascii	"java/nio/channels/WritableByteChannel"
	.zero	33
	.zero	2

	/* #254 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554883
	/* java_name */
	.ascii	"java/nio/channels/spi/AbstractInterruptibleChannel"
	.zero	20
	.zero	2

	/* #255 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554851
	/* java_name */
	.ascii	"java/security/KeyStore"
	.zero	48
	.zero	2

	/* #256 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554853
	/* java_name */
	.ascii	"java/security/KeyStore$LoadStoreParameter"
	.zero	29
	.zero	2

	/* #257 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554855
	/* java_name */
	.ascii	"java/security/KeyStore$ProtectionParameter"
	.zero	28
	.zero	2

	/* #258 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554856
	/* java_name */
	.ascii	"java/security/cert/Certificate"
	.zero	40
	.zero	2

	/* #259 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554858
	/* java_name */
	.ascii	"java/security/cert/CertificateFactory"
	.zero	33
	.zero	2

	/* #260 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554861
	/* java_name */
	.ascii	"java/security/cert/X509Certificate"
	.zero	36
	.zero	2

	/* #261 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554860
	/* java_name */
	.ascii	"java/security/cert/X509Extension"
	.zero	38
	.zero	2

	/* #262 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554810
	/* java_name */
	.ascii	"java/util/ArrayList"
	.zero	51
	.zero	2

	/* #263 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554799
	/* java_name */
	.ascii	"java/util/Collection"
	.zero	50
	.zero	2

	/* #264 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554801
	/* java_name */
	.ascii	"java/util/HashMap"
	.zero	53
	.zero	2

	/* #265 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554819
	/* java_name */
	.ascii	"java/util/HashSet"
	.zero	53
	.zero	2

	/* #266 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554850
	/* java_name */
	.ascii	"java/util/Iterator"
	.zero	52
	.zero	2

	/* #267 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554523
	/* java_name */
	.ascii	"javax/net/ssl/TrustManager"
	.zero	44
	.zero	2

	/* #268 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554526
	/* java_name */
	.ascii	"javax/net/ssl/TrustManagerFactory"
	.zero	37
	.zero	2

	/* #269 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554525
	/* java_name */
	.ascii	"javax/net/ssl/X509TrustManager"
	.zero	40
	.zero	2

	/* #270 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554970
	/* java_name */
	.ascii	"mono/android/TypeManager"
	.zero	46
	.zero	2

	/* #271 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554756
	/* java_name */
	.ascii	"mono/android/content/DialogInterface_OnClickListenerImplementor"
	.zero	7
	.zero	2

	/* #272 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554795
	/* java_name */
	.ascii	"mono/android/runtime/InputStreamAdapter"
	.zero	31
	.zero	2

	/* #273 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	0
	/* java_name */
	.ascii	"mono/android/runtime/JavaArray"
	.zero	40
	.zero	2

	/* #274 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554816
	/* java_name */
	.ascii	"mono/android/runtime/JavaObject"
	.zero	39
	.zero	2

	/* #275 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554834
	/* java_name */
	.ascii	"mono/android/runtime/OutputStreamAdapter"
	.zero	30
	.zero	2

	/* #276 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554595
	/* java_name */
	.ascii	"mono/android/view/View_OnClickListenerImplementor"
	.zero	21
	.zero	2

	/* #277 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554601
	/* java_name */
	.ascii	"mono/android/view/View_OnKeyListenerImplementor"
	.zero	23
	.zero	2

	/* #278 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554541
	/* java_name */
	.ascii	"mono/android/widget/AdapterView_OnItemClickListenerImplementor"
	.zero	8
	.zero	2

	/* #279 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554546
	/* java_name */
	.ascii	"mono/android/widget/AdapterView_OnItemSelectedListenerImplementor"
	.zero	5
	.zero	2

	/* #280 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554566
	/* java_name */
	.ascii	"mono/android/widget/CompoundButton_OnCheckedChangeListenerImplementor"
	.zero	1
	.zero	2

	/* #281 */
	/* module_index */
	.long	1
	/* type_token_id */
	.long	33554902
	/* java_name */
	.ascii	"mono/java/lang/RunnableImplementor"
	.zero	36
	.zero	2

	.size	map_java, 22560
/* Java to managed map: END */


/* java_name_width: START */
	.section	.rodata.java_name_width,"a",%progbits
	.type	java_name_width, %object
	.p2align	2
	.global	java_name_width
java_name_width:
	.size	java_name_width, 4
	.long	72
/* java_name_width: END */
