	.arch	armv8-a
	.file	"compressed_assemblies.arm64-v8a.arm64-v8a.s"
	.include	"compressed_assemblies.arm64-v8a-data.inc"

	.section	.data.compressed_assembly_descriptors,"aw",@progbits
	.type	.L.compressed_assembly_descriptors, @object
	.p2align	3
.L.compressed_assembly_descriptors:
	/* 0: Java.Interop.dll */
	/* uncompressed_file_size */
	.word	162816
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_0

	/* 1: Microsoft.CSharp.dll */
	/* uncompressed_file_size */
	.word	300032
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_1

	/* 2: Mono.Android.dll */
	/* uncompressed_file_size */
	.word	899584
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_2

	/* 3: Mono.Data.Sqlite.dll */
	/* uncompressed_file_size */
	.word	136704
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_3

	/* 4: Mono.Security.dll */
	/* uncompressed_file_size */
	.word	121344
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_4

	/* 5: RTIS_Vulcan_MBL.dll */
	/* uncompressed_file_size */
	.word	446464
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_5

	/* 6: System.Core.dll */
	/* uncompressed_file_size */
	.word	1073664
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_6

	/* 7: System.Data.dll */
	/* uncompressed_file_size */
	.word	770560
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_7

	/* 8: System.Numerics.dll */
	/* uncompressed_file_size */
	.word	35840
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_8

	/* 9: System.Runtime.Serialization.dll */
	/* uncompressed_file_size */
	.word	5632
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_9

	/* 10: System.Transactions.dll */
	/* uncompressed_file_size */
	.word	7168
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_10

	/* 11: System.Xml.dll */
	/* uncompressed_file_size */
	.word	1047552
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_11

	/* 12: System.dll */
	/* uncompressed_file_size */
	.word	786944
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_12

	/* 13: mscorlib.dll */
	/* uncompressed_file_size */
	.word	2091008
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.xword	compressed_assembly_data_13

	.size	.L.compressed_assembly_descriptors, 224
	.section	.data.compressed_assemblies,"aw",@progbits
	.type	compressed_assemblies, @object
	.p2align	3
	.global	compressed_assemblies
compressed_assemblies:
	/* count */
	.word	14
	/* descriptors */
	.zero	4
	.xword	.L.compressed_assembly_descriptors
	.size	compressed_assemblies, 16
