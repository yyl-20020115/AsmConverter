﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsmConverter
{
    public static class Instructions
    {
		public static readonly HashSet<string> NasmRegisters = new ()
		{
			"al",
			"ah",
			"ax",
			"eax",
			"rax",
			"bl",
			"bh",
			"bx",
			"ebx",
			"rbx",
			"cl",
			"ch",
			"cx",
			"ecx",
			"rcx",
			"dl",
			"dh",
			"dx",
			"edx",
			"rdx",
			"sp",
			"esp",
			"rsp",
			"bp",
			"ebp",
			"rbp",
			"si",
			"esi",
			"rsi",
			"di",
			"edi",
			"rdi",
			"r8",
			"r9",
			"r10",
			"r11",
			"r12",
			"r13",
			"r14",
			"r15",
			"cs",
			"ds",
			"es",
			"fs",
			"gs",
			"sreg6",
			"sreg7",
			"cr0",
			"cr1",
			"cr2",
			"cr3",
			"cr4",
			"cr5",
			"cr6",
			"cr7",
			"cr8",
			"cr9",
			"cr10",
			"cr11",
			"cr12",
			"cr13",
			"cr14",
			"cr15",
			"dr0",
			"dr1",
			"dr2",
			"dr3",
			"dr4",
			"dr5",
			"dr6",
			"dr7",
			"dr8",
			"dr9",
			"dr10",
			"dr11",
			"dr12",
			"dr13",
			"dr14",
			"dr15",
			"tr0",
			"tr1",
			"tr2",
			"tr3",
			"tr4",
			"tr5",
			"tr6",
			"tr7",
			"st0",
			"st1",
			"st2",
			"st3",
			"st4",
			"st5",
			"st6",
			"st7",

			"mm0",
			"mm1",
			"mm2",
			"mm3",
			"mm4",
			"mm5",
			"mm6",
			"mm7",

			"xmm0",
			"xmm1",
			"xmm2",
			"xmm3",
			"xmm4",
			"xmm5",
			"xmm6",
			"xmm7",
			"xmm8",
			"xmm9",
			"xmm10",
			"xmm11",
			"xmm12",
			"xmm13",
			"xmm14",
			"xmm15",
			"xmm16",
			"xmm17",
			"xmm18",
			"xmm19",
			"xmm20",
			"xmm21",
			"xmm22",
			"xmm23",
			"xmm24",
			"xmm25",
			"xmm26",
			"xmm27",
			"xmm28",
			"xmm29",
			"xmm30",
			"xmm31",
			"ymm0",
			"ymm1",
			"ymm2",
			"ymm3",
			"ymm4",
			"ymm5",
			"ymm6",
			"ymm7",
			"ymm8",
			"ymm9",
			"ymm10",
			"ymm11",
			"ymm12",
			"ymm13",
			"ymm14",
			"ymm15",
			"ymm16",
			"ymm17",
			"ymm18",
			"ymm19",
			"ymm20",
			"ymm21",
			"ymm22",
			"ymm23",
			"ymm24",
			"ymm25",
			"ymm26",
			"ymm27",
			"ymm28",
			"ymm29",
			"ymm30",
			"ymm31",
			"zmm0",
			"zmm1",
			"zmm2",
			"zmm3",
			"zmm4",
			"zmm5",
			"zmm6",
			"zmm7",
			"zmm8",
			"zmm9",
			"zmm10",
			"zmm11",
			"zmm12",
			"zmm13",
			"zmm14",
			"zmm15",
			"zmm16",
			"zmm17",
			"zmm18",
			"zmm19",
			"zmm20",
			"zmm21",
			"zmm22",
			"zmm23",
			"zmm24",
			"zmm25",
			"zmm26",
			"zmm27",
			"zmm28",
			"zmm29",
			"zmm30",
			"zmm31",
			"tmm0",
			"tmm1",
			"tmm2",
			"tmm3",
			"tmm4",
			"tmm5",
			"tmm6",
			"tmm7",
			"k0",
			"k1",
			"k2",
			"k3",
			"k4",
			"k5",
			"k6",
			"k7",
			"bnd0",
			"bnd1",
			"bnd2",
			"bnd3",
		};
        public static readonly HashSet<string> NasmInstructionNames = new()
        {
			//"db",
			//"dw",
			//"dd",
			//"dq",
			//"dt",
			//"do",
			//"dy",
			//"dz",
			"resb",
			"resw",
			"resd",
			"resq",
			"rest",
			"reso",
			"resy",
			"resz",
			//"incbin",
			"repne",
			"rep",
			"wait",
			"lock",

			"aaa",
			"aad",
			"aam",
			"aas",
			"adc",
			"add",
			"and",
			"arpl",
			"bb0_reset",
			"bb1_reset",
			"bound",
			"bsf",
			"bsr",
			"bswap",
			"bt",
			"btc",
			"btr",
			"bts",
			"call",
			"cbw",
			"cdq",
			"cdqe",
			"clc",
			"cld",
			"cli",
			"clts",
			"cmc",
			"cmp",
			"cmpsb",
			"cmpsd",
			"cmpsq",
			"cmpsw",
			"cmpxchg",
			"cmpxchg486",
			"cmpxchg8b",
			"cmpxchg16b",
			"cpuid",
			"cpu_read",
			"cpu_write",
			"cqo",
			"cwd",
			"cwde",
			"daa",
			"das",
			"dec",
			"div",
			"dmint",
			"emms",
			"enter",
			"equ",
			"f2xm1",
			"fabs",
			"fadd",
			"faddp",
			"fbld",
			"fbstp",
			"fchs",
			"fclex",
			"fcmovb",
			"fcmovbe",
			"fcmove",
			"fcmovnb",
			"fcmovnbe",
			"fcmovne",
			"fcmovnu",
			"fcmovu",
			"fcom",
			"fcomi",
			"fcomip",
			"fcomp",
			"fcompp",
			"fcos",
			"fdecstp",
			"fdisi",
			"fdiv",
			"fdivp",
			"fdivr",
			"fdivrp",
			"femms",
			"feni",
			"ffree",
			"ffreep",
			"fiadd",
			"ficom",
			"ficomp",
			"fidiv",
			"fidivr",
			"fild",
			"fimul",
			"fincstp",
			"finit",
			"fist",
			"fistp",
			"fisttp",
			"fisub",
			"fisubr",
			"fld",
			"fld1",
			"fldcw",
			"fldenv",
			"fldl2e",
			"fldl2t",
			"fldlg2",
			"fldln2",
			"fldpi",
			"fldz",
			"fmul",
			"fmulp",
			"fnclex",
			"fndisi",
			"fneni",
			"fninit",
			"fnop",
			"fnsave",
			"fnstcw",
			"fnstenv",
			"fnstsw",
			"fpatan",
			"fprem",
			"fprem1",
			"fptan",
			"frndint",
			"frstor",
			"fsave",
			"fscale",
			"fsetpm",
			"fsin",
			"fsincos",
			"fsqrt",
			"fst",
			"fstcw",
			"fstenv",
			"fstp",
			"fstsw",
			"fsub",
			"fsubp",
			"fsubr",
			"fsubrp",
			"ftst",
			"fucom",
			"fucomi",
			"fucomip",
			"fucomp",
			"fucompp",
			"fxam",
			"fxch",
			"fxtract",
			"fyl2x",
			"fyl2xp1",
			"hlt",
			"ibts",
			"icebp",
			"idiv",
			"imul",
			"in",
			"inc",
			"insb",
			"insd",
			"insw",
			"int",
			"int01",
			"int1",
			"int03",
			"int3",
			"into",
			"invd",
			"invpcid",
			"invlpg",
			"invlpga",
			"iret",
			"iretd",
			"iretq",
			"iretw",
			"jcxz",
			"jecxz",
			"jrcxz",
			"jmp",
			"jmpe",
			"lahf",
			"lar",
			"lds",
			"lea",
			"leave",
			"les",
			"lfence",
			"lfs",
			"lgdt",
			"lgs",
			"lidt",
			"lldt",
			"lmsw",
			"loadall",
			"loadall286",
			"lodsb",
			"lodsd",
			"lodsq",
			"lodsw",
			"loop",
			"loope",
			"loopne",
			"loopnz",
			"loopz",
			"lsl",
			"lss",
			"ltr",
			"mfence",
			"monitor",
			"monitorx",
			"mov",
			"movd",
			"movq",
			"movsb",
			"movsd",
			"movsq",
			"movsw",
			"movsx",
			"movsxd",
			"movzx",
			"mul",
			"mwait",
			"mwaitx",
			"neg",
			"nop",
			"not",
			"or",
			"out",
			"outsb",
			"outsd",
			"outsw",
			"packssdw",
			"packsswb",
			"packuswb",
			"paddb",
			"paddd",
			"paddsb",
			"paddsiw",
			"paddsw",
			"paddusb",
			"paddusw",
			"paddw",
			"pand",
			"pandn",
			"pause",
			"paveb",
			"pavgusb",
			"pcmpeqb",
			"pcmpeqd",
			"pcmpeqw",
			"pcmpgtb",
			"pcmpgtd",
			"pcmpgtw",
			"pdistib",
			"pf2id",
			"pfacc",
			"pfadd",
			"pfcmpeq",
			"pfcmpge",
			"pfcmpgt",
			"pfmax",
			"pfmin",
			"pfmul",
			"pfrcp",
			"pfrcpit1",
			"pfrcpit2",
			"pfrsqit1",
			"pfrsqrt",
			"pfsub",
			"pfsubr",
			"pi2fd",
			"pmachriw",
			"pmaddwd",
			"pmagw",
			"pmulhriw",
			"pmulhrwa",
			"pmulhrwc",
			"pmulhw",
			"pmullw",
			"pmvgezb",
			"pmvlzb",
			"pmvnzb",
			"pmvzb",
			"pop",
			"popa",
			"popad",
			"popaw",
			"popf",
			"popfd",
			"popfq",
			"popfw",
			"por",
			"prefetch",
			"prefetchw",
			"pslld",
			"psllq",
			"psllw",
			"psrad",
			"psraw",
			"psrld",
			"psrlq",
			"psrlw",
			"psubb",
			"psubd",
			"psubsb",
			"psubsiw",
			"psubsw",
			"psubusb",
			"psubusw",
			"psubw",
			"punpckhbw",
			"punpckhdq",
			"punpckhwd",
			"punpcklbw",
			"punpckldq",
			"punpcklwd",
			"push",
			"pusha",
			"pushad",
			"pushaw",
			"pushf",
			"pushfd",
			"pushfq",
			"pushfw",
			"pxor",
			"rcl",
			"rcr",
			"rdshr",
			"rdmsr",
			"rdpmc",
			"rdtsc",
			"rdtscp",
			"ret",
			"retf",
			"retn",
			"retw",
			"retfw",
			"retnw",
			"retd",
			"retfd",
			"retnd",
			"retq",
			"retfq",
			"retnq",
			"rol",
			"ror",
			"rdm",
			"rsdc",
			"rsldt",
			"rsm",
			"rsts",
			"sahf",
			"sal",
			"salc",
			"sar",
			"sbb",
			"scasb",
			"scasd",
			"scasq",
			"scasw",
			"sfence",
			"sgdt",
			"shl",
			"shld",
			"shr",
			"shrd",
			"sidt",
			"sldt",
			"skinit",
			"smi",
			"smint",
			"smintold",
			"smsw",
			"stc",
			"std",
			"sti",
			"stosb",
			"stosd",
			"stosq",
			"stosw",
			"str",
			"sub",
			"svdc",
			"svldt",
			"svts",
			"swapgs",
			"syscall",
			"sysenter",
			"sysexit",
			"sysret",
			"test",
			"ud0",
			"ud1",
			"ud2b",
			"ud2",
			"ud2a",
			"umov",
			"verr",
			"verw",
			"fwait",
			"wbinvd",
			"wrshr",
			"wrmsr",
			"xadd",
			"xbts",
			"xchg",
			"xlatb",
			"xlat",
			"xor",
			"addps",
			"addss",
			"andnps",
			"andps",
			"cmpeqps",
			"cmpeqss",
			"cmpleps",
			"cmpless",
			"cmpltps",
			"cmpltss",
			"cmpneqps",
			"cmpneqss",
			"cmpnleps",
			"cmpnless",
			"cmpnltps",
			"cmpnltss",
			"cmpordps",
			"cmpordss",
			"cmpunordps",
			"cmpunordss",
			"cmpps",
			"cmpss",
			"comiss",
			"cvtpi2ps",
			"cvtps2pi",
			"cvtsi2ss",
			"cvtss2si",
			"cvttps2pi",
			"cvttss2si",
			"divps",
			"divss",
			"ldmxcsr",
			"maxps",
			"maxss",
			"minps",
			"minss",
			"movaps",
			"movhps",
			"movlhps",
			"movlps",
			"movhlps",
			"movmskps",
			"movntps",
			"movss",
			"movups",
			"mulps",
			"mulss",
			"orps",
			"rcpps",
			"rcpss",
			"rsqrtps",
			"rsqrtss",
			"shufps",
			"sqrtps",
			"sqrtss",
			"stmxcsr",
			"subps",
			"subss",
			"ucomiss",
			"unpckhps",
			"unpcklps",
			"xorps",
			"fxrstor",
			"fxrstor64",
			"fxsave",
			"fxsave64",
			"xgetbv",
			"xsetbv",
			"xsave",
			"xsave64",
			"xsavec",
			"xsavec64",
			"xsaveopt",
			"xsaveopt64",
			"xsaves",
			"xsaves64",
			"xrstor",
			"xrstor64",
			"xrstors",
			"xrstors64",
			"prefetchnta",
			"prefetcht0",
			"prefetcht1",
			"prefetcht2",
			"maskmovq",
			"movntq",
			"pavgb",
			"pavgw",
			"pextrw",
			"pinsrw",
			"pmaxsw",
			"pmaxub",
			"pminsw",
			"pminub",
			"pmovmskb",
			"pmulhuw",
			"psadbw",
			"pshufw",
			"pf2iw",
			"pfnacc",
			"pfpnacc",
			"pi2fw",
			"pswapd",
			"maskmovdqu",
			"clflush",
			"movntdq",
			"movnti",
			"movntpd",
			"movdqa",
			"movdqu",
			"movdq2q",
			"movq2dq",
			"paddq",
			"pmuludq",
			"pshufd",
			"pshufhw",
			"pshuflw",
			"pslldq",
			"psrldq",
			"psubq",
			"punpckhqdq",
			"punpcklqdq",
			"addpd",
			"addsd",
			"andnpd",
			"andpd",
			"cmpeqpd",
			"cmpeqsd",
			"cmplepd",
			"cmplesd",
			"cmpltpd",
			"cmpltsd",
			"cmpneqpd",
			"cmpneqsd",
			"cmpnlepd",
			"cmpnlesd",
			"cmpnltpd",
			"cmpnltsd",
			"cmpordpd",
			"cmpordsd",
			"cmpunordpd",
			"cmpunordsd",
			"cmppd",
			"comisd",
			"cvtdq2pd",
			"cvtdq2ps",
			"cvtpd2dq",
			"cvtpd2pi",
			"cvtpd2ps",
			"cvtpi2pd",
			"cvtps2dq",
			"cvtps2pd",
			"cvtsd2si",
			"cvtsd2ss",
			"cvtsi2sd",
			"cvtss2sd",
			"cvttpd2pi",
			"cvttpd2dq",
			"cvttps2dq",
			"cvttsd2si",
			"divpd",
			"divsd",
			"maxpd",
			"maxsd",
			"minpd",
			"minsd",
			"movapd",
			"movhpd",
			"movlpd",
			"movmskpd",
			"movupd",
			"mulpd",
			"mulsd",
			"orpd",
			"shufpd",
			"sqrtpd",
			"sqrtsd",
			"subpd",
			"subsd",
			"ucomisd",
			"unpckhpd",
			"unpcklpd",
			"xorpd",
			"addsubpd",
			"addsubps",
			"haddpd",
			"haddps",
			"hsubpd",
			"hsubps",
			"lddqu",
			"movddup",
			"movshdup",
			"movsldup",
			"clgi",
			"stgi",
			"vmcall",
			"vmclear",
			"vmfunc",
			"vmlaunch",
			"vmload",
			"vmmcall",
			"vmptrld",
			"vmptrst",
			"vmread",
			"vmresume",
			"vmrun",
			"vmsave",
			"vmwrite",
			"vmxoff",
			"vmxon",
			"invept",
			"invvpid",
			"pvalidate",
			"rmpadjust",
			"vmgexit",
			"pabsb",
			"pabsw",
			"pabsd",
			"palignr",
			"phaddw",
			"phaddd",
			"phaddsw",
			"phsubw",
			"phsubd",
			"phsubsw",
			"pmaddubsw",
			"pmulhrsw",
			"pshufb",
			"psignb",
			"psignw",
			"psignd",
			"extrq",
			"insertq",
			"movntsd",
			"movntss",
			"lzcnt",
			"blendpd",
			"blendps",
			"blendvpd",
			"blendvps",
			"dppd",
			"dpps",
			"extractps",
			"insertps",
			"movntdqa",
			"mpsadbw",
			"packusdw",
			"pblendvb",
			"pblendw",
			"pcmpeqq",
			"pextrb",
			"pextrd",
			"pextrq",
			"phminposuw",
			"pinsrb",
			"pinsrd",
			"pinsrq",
			"pmaxsb",
			"pmaxsd",
			"pmaxud",
			"pmaxuw",
			"pminsb",
			"pminsd",
			"pminud",
			"pminuw",
			"pmovsxbw",
			"pmovsxbd",
			"pmovsxbq",
			"pmovsxwd",
			"pmovsxwq",
			"pmovsxdq",
			"pmovzxbw",
			"pmovzxbd",
			"pmovzxbq",
			"pmovzxwd",
			"pmovzxwq",
			"pmovzxdq",
			"pmuldq",
			"pmulld",
			"ptest",
			"roundpd",
			"roundps",
			"roundsd",
			"roundss",
			"crc32",
			"pcmpestri",
			"pcmpestrm",
			"pcmpistri",
			"pcmpistrm",
			"pcmpgtq",
			"popcnt",
			"getsec",
			"pfrcpv",
			"pfrsqrtv",
			"movbe",
			"aesenc",
			"aesenclast",
			"aesdec",
			"aesdeclast",
			"aesimc",
			"aeskeygenassist",
			"vaesenc",
			"vaesenclast",
			"vaesdec",
			"vaesdeclast",
			"vaesimc",
			"vaeskeygenassist",
			"vaddpd",
			"vaddps",
			"vaddsd",
			"vaddss",
			"vaddsubpd",
			"vaddsubps",
			"vandpd",
			"vandps",
			"vandnpd",
			"vandnps",
			"vblendpd",
			"vblendps",
			"vblendvpd",
			"vblendvps",
			"vbroadcastss",
			"vbroadcastsd",
			"vbroadcastf128",
			"vcmpeq_ospd",
			"vcmpeqpd",
			"vcmplt_ospd",
			"vcmpltpd",
			"vcmple_ospd",
			"vcmplepd",
			"vcmpunord_qpd",
			"vcmpunordpd",
			"vcmpneq_uqpd",
			"vcmpneqpd",
			"vcmpnlt_uspd",
			"vcmpnltpd",
			"vcmpnle_uspd",
			"vcmpnlepd",
			"vcmpord_qpd",
			"vcmpordpd",
			"vcmpeq_uqpd",
			"vcmpnge_uspd",
			"vcmpngepd",
			"vcmpngt_uspd",
			"vcmpngtpd",
			"vcmpfalse_oqpd",
			"vcmpfalsepd",
			"vcmpneq_oqpd",
			"vcmpge_ospd",
			"vcmpgepd",
			"vcmpgt_ospd",
			"vcmpgtpd",
			"vcmptrue_uqpd",
			"vcmptruepd",
			"vcmplt_oqpd",
			"vcmple_oqpd",
			"vcmpunord_spd",
			"vcmpneq_uspd",
			"vcmpnlt_uqpd",
			"vcmpnle_uqpd",
			"vcmpord_spd",
			"vcmpeq_uspd",
			"vcmpnge_uqpd",
			"vcmpngt_uqpd",
			"vcmpfalse_ospd",
			"vcmpneq_ospd",
			"vcmpge_oqpd",
			"vcmpgt_oqpd",
			"vcmptrue_uspd",
			"vcmppd",
			"vcmpeq_osps",
			"vcmpeqps",
			"vcmplt_osps",
			"vcmpltps",
			"vcmple_osps",
			"vcmpleps",
			"vcmpunord_qps",
			"vcmpunordps",
			"vcmpneq_uqps",
			"vcmpneqps",
			"vcmpnlt_usps",
			"vcmpnltps",
			"vcmpnle_usps",
			"vcmpnleps",
			"vcmpord_qps",
			"vcmpordps",
			"vcmpeq_uqps",
			"vcmpnge_usps",
			"vcmpngeps",
			"vcmpngt_usps",
			"vcmpngtps",
			"vcmpfalse_oqps",
			"vcmpfalseps",
			"vcmpneq_oqps",
			"vcmpge_osps",
			"vcmpgeps",
			"vcmpgt_osps",
			"vcmpgtps",
			"vcmptrue_uqps",
			"vcmptrueps",
			"vcmplt_oqps",
			"vcmple_oqps",
			"vcmpunord_sps",
			"vcmpneq_usps",
			"vcmpnlt_uqps",
			"vcmpnle_uqps",
			"vcmpord_sps",
			"vcmpeq_usps",
			"vcmpnge_uqps",
			"vcmpngt_uqps",
			"vcmpfalse_osps",
			"vcmpneq_osps",
			"vcmpge_oqps",
			"vcmpgt_oqps",
			"vcmptrue_usps",
			"vcmpps",
			"vcmpeq_ossd",
			"vcmpeqsd",
			"vcmplt_ossd",
			"vcmpltsd",
			"vcmple_ossd",
			"vcmplesd",
			"vcmpunord_qsd",
			"vcmpunordsd",
			"vcmpneq_uqsd",
			"vcmpneqsd",
			"vcmpnlt_ussd",
			"vcmpnltsd",
			"vcmpnle_ussd",
			"vcmpnlesd",
			"vcmpord_qsd",
			"vcmpordsd",
			"vcmpeq_uqsd",
			"vcmpnge_ussd",
			"vcmpngesd",
			"vcmpngt_ussd",
			"vcmpngtsd",
			"vcmpfalse_oqsd",
			"vcmpfalsesd",
			"vcmpneq_oqsd",
			"vcmpge_ossd",
			"vcmpgesd",
			"vcmpgt_ossd",
			"vcmpgtsd",
			"vcmptrue_uqsd",
			"vcmptruesd",
			"vcmplt_oqsd",
			"vcmple_oqsd",
			"vcmpunord_ssd",
			"vcmpneq_ussd",
			"vcmpnlt_uqsd",
			"vcmpnle_uqsd",
			"vcmpord_ssd",
			"vcmpeq_ussd",
			"vcmpnge_uqsd",
			"vcmpngt_uqsd",
			"vcmpfalse_ossd",
			"vcmpneq_ossd",
			"vcmpge_oqsd",
			"vcmpgt_oqsd",
			"vcmptrue_ussd",
			"vcmpsd",
			"vcmpeq_osss",
			"vcmpeqss",
			"vcmplt_osss",
			"vcmpltss",
			"vcmple_osss",
			"vcmpless",
			"vcmpunord_qss",
			"vcmpunordss",
			"vcmpneq_uqss",
			"vcmpneqss",
			"vcmpnlt_usss",
			"vcmpnltss",
			"vcmpnle_usss",
			"vcmpnless",
			"vcmpord_qss",
			"vcmpordss",
			"vcmpeq_uqss",
			"vcmpnge_usss",
			"vcmpngess",
			"vcmpngt_usss",
			"vcmpngtss",
			"vcmpfalse_oqss",
			"vcmpfalsess",
			"vcmpneq_oqss",
			"vcmpge_osss",
			"vcmpgess",
			"vcmpgt_osss",
			"vcmpgtss",
			"vcmptrue_uqss",
			"vcmptruess",
			"vcmplt_oqss",
			"vcmple_oqss",
			"vcmpunord_sss",
			"vcmpneq_usss",
			"vcmpnlt_uqss",
			"vcmpnle_uqss",
			"vcmpord_sss",
			"vcmpeq_usss",
			"vcmpnge_uqss",
			"vcmpngt_uqss",
			"vcmpfalse_osss",
			"vcmpneq_osss",
			"vcmpge_oqss",
			"vcmpgt_oqss",
			"vcmptrue_usss",
			"vcmpss",
			"vcomisd",
			"vcomiss",
			"vcvtdq2pd",
			"vcvtdq2ps",
			"vcvtpd2dq",
			"vcvtpd2ps",
			"vcvtps2dq",
			"vcvtps2pd",
			"vcvtsd2si",
			"vcvtsd2ss",
			"vcvtsi2sd",
			"vcvtsi2ss",
			"vcvtss2sd",
			"vcvtss2si",
			"vcvttpd2dq",
			"vcvttps2dq",
			"vcvttsd2si",
			"vcvttss2si",
			"vdivpd",
			"vdivps",
			"vdivsd",
			"vdivss",
			"vdppd",
			"vdpps",
			"vextractf128",
			"vextractps",
			"vhaddpd",
			"vhaddps",
			"vhsubpd",
			"vhsubps",
			"vinsertf128",
			"vinsertps",
			"vlddqu",
			"vldqqu",
			"vldmxcsr",
			"vmaskmovdqu",
			"vmaskmovps",
			"vmaskmovpd",
			"vmaxpd",
			"vmaxps",
			"vmaxsd",
			"vmaxss",
			"vminpd",
			"vminps",
			"vminsd",
			"vminss",
			"vmovapd",
			"vmovaps",
			"vmovd",
			"vmovq",
			"vmovddup",
			"vmovdqa",
			"vmovqqa",
			"vmovdqu",
			"vmovqqu",
			"vmovhlps",
			"vmovhpd",
			"vmovhps",
			"vmovlhps",
			"vmovlpd",
			"vmovlps",
			"vmovmskpd",
			"vmovmskps",
			"vmovntdq",
			"vmovntqq",
			"vmovntdqa",
			"vmovntpd",
			"vmovntps",
			"vmovsd",
			"vmovshdup",
			"vmovsldup",
			"vmovss",
			"vmovupd",
			"vmovups",
			"vmpsadbw",
			"vmulpd",
			"vmulps",
			"vmulsd",
			"vmulss",
			"vorpd",
			"vorps",
			"vpabsb",
			"vpabsw",
			"vpabsd",
			"vpacksswb",
			"vpackssdw",
			"vpackuswb",
			"vpackusdw",
			"vpaddb",
			"vpaddw",
			"vpaddd",
			"vpaddq",
			"vpaddsb",
			"vpaddsw",
			"vpaddusb",
			"vpaddusw",
			"vpalignr",
			"vpand",
			"vpandn",
			"vpavgb",
			"vpavgw",
			"vpblendvb",
			"vpblendw",
			"vpcmpestri",
			"vpcmpestrm",
			"vpcmpistri",
			"vpcmpistrm",
			"vpcmpeqb",
			"vpcmpeqw",
			"vpcmpeqd",
			"vpcmpeqq",
			"vpcmpgtb",
			"vpcmpgtw",
			"vpcmpgtd",
			"vpcmpgtq",
			"vpermilpd",
			"vpermilps",
			"vperm2f128",
			"vpextrb",
			"vpextrw",
			"vpextrd",
			"vpextrq",
			"vphaddw",
			"vphaddd",
			"vphaddsw",
			"vphminposuw",
			"vphsubw",
			"vphsubd",
			"vphsubsw",
			"vpinsrb",
			"vpinsrw",
			"vpinsrd",
			"vpinsrq",
			"vpmaddwd",
			"vpmaddubsw",
			"vpmaxsb",
			"vpmaxsw",
			"vpmaxsd",
			"vpmaxub",
			"vpmaxuw",
			"vpmaxud",
			"vpminsb",
			"vpminsw",
			"vpminsd",
			"vpminub",
			"vpminuw",
			"vpminud",
			"vpmovmskb",
			"vpmovsxbw",
			"vpmovsxbd",
			"vpmovsxbq",
			"vpmovsxwd",
			"vpmovsxwq",
			"vpmovsxdq",
			"vpmovzxbw",
			"vpmovzxbd",
			"vpmovzxbq",
			"vpmovzxwd",
			"vpmovzxwq",
			"vpmovzxdq",
			"vpmulhuw",
			"vpmulhrsw",
			"vpmulhw",
			"vpmullw",
			"vpmulld",
			"vpmuludq",
			"vpmuldq",
			"vpor",
			"vpsadbw",
			"vpshufb",
			"vpshufd",
			"vpshufhw",
			"vpshuflw",
			"vpsignb",
			"vpsignw",
			"vpsignd",
			"vpslldq",
			"vpsrldq",
			"vpsllw",
			"vpslld",
			"vpsllq",
			"vpsraw",
			"vpsrad",
			"vpsrlw",
			"vpsrld",
			"vpsrlq",
			"vptest",
			"vpsubb",
			"vpsubw",
			"vpsubd",
			"vpsubq",
			"vpsubsb",
			"vpsubsw",
			"vpsubusb",
			"vpsubusw",
			"vpunpckhbw",
			"vpunpckhwd",
			"vpunpckhdq",
			"vpunpckhqdq",
			"vpunpcklbw",
			"vpunpcklwd",
			"vpunpckldq",
			"vpunpcklqdq",
			"vpxor",
			"vrcpps",
			"vrcpss",
			"vrsqrtps",
			"vrsqrtss",
			"vroundpd",
			"vroundps",
			"vroundsd",
			"vroundss",
			"vshufpd",
			"vshufps",
			"vsqrtpd",
			"vsqrtps",
			"vsqrtsd",
			"vsqrtss",
			"vstmxcsr",
			"vsubpd",
			"vsubps",
			"vsubsd",
			"vsubss",
			"vtestps",
			"vtestpd",
			"vucomisd",
			"vucomiss",
			"vunpckhpd",
			"vunpckhps",
			"vunpcklpd",
			"vunpcklps",
			"vxorpd",
			"vxorps",
			"vzeroall",
			"vzeroupper",
			"pclmullqlqdq",
			"pclmulhqlqdq",
			"pclmullqhqdq",
			"pclmulhqhqdq",
			"pclmulqdq",
			"vpclmullqlqdq",
			"vpclmulhqlqdq",
			"vpclmullqhqdq",
			"vpclmulhqhqdq",
			"vpclmulqdq",
			"vfmadd132ps",
			"vfmadd132pd",
			"vfmadd312ps",
			"vfmadd312pd",
			"vfmadd213ps",
			"vfmadd213pd",
			"vfmadd123ps",
			"vfmadd123pd",
			"vfmadd231ps",
			"vfmadd231pd",
			"vfmadd321ps",
			"vfmadd321pd",
			"vfmaddsub132ps",
			"vfmaddsub132pd",
			"vfmaddsub312ps",
			"vfmaddsub312pd",
			"vfmaddsub213ps",
			"vfmaddsub213pd",
			"vfmaddsub123ps",
			"vfmaddsub123pd",
			"vfmaddsub231ps",
			"vfmaddsub231pd",
			"vfmaddsub321ps",
			"vfmaddsub321pd",
			"vfmsub132ps",
			"vfmsub132pd",
			"vfmsub312ps",
			"vfmsub312pd",
			"vfmsub213ps",
			"vfmsub213pd",
			"vfmsub123ps",
			"vfmsub123pd",
			"vfmsub231ps",
			"vfmsub231pd",
			"vfmsub321ps",
			"vfmsub321pd",
			"vfmsubadd132ps",
			"vfmsubadd132pd",
			"vfmsubadd312ps",
			"vfmsubadd312pd",
			"vfmsubadd213ps",
			"vfmsubadd213pd",
			"vfmsubadd123ps",
			"vfmsubadd123pd",
			"vfmsubadd231ps",
			"vfmsubadd231pd",
			"vfmsubadd321ps",
			"vfmsubadd321pd",
			"vfnmadd132ps",
			"vfnmadd132pd",
			"vfnmadd312ps",
			"vfnmadd312pd",
			"vfnmadd213ps",
			"vfnmadd213pd",
			"vfnmadd123ps",
			"vfnmadd123pd",
			"vfnmadd231ps",
			"vfnmadd231pd",
			"vfnmadd321ps",
			"vfnmadd321pd",
			"vfnmsub132ps",
			"vfnmsub132pd",
			"vfnmsub312ps",
			"vfnmsub312pd",
			"vfnmsub213ps",
			"vfnmsub213pd",
			"vfnmsub123ps",
			"vfnmsub123pd",
			"vfnmsub231ps",
			"vfnmsub231pd",
			"vfnmsub321ps",
			"vfnmsub321pd",
			"vfmadd132ss",
			"vfmadd132sd",
			"vfmadd312ss",
			"vfmadd312sd",
			"vfmadd213ss",
			"vfmadd213sd",
			"vfmadd123ss",
			"vfmadd123sd",
			"vfmadd231ss",
			"vfmadd231sd",
			"vfmadd321ss",
			"vfmadd321sd",
			"vfmsub132ss",
			"vfmsub132sd",
			"vfmsub312ss",
			"vfmsub312sd",
			"vfmsub213ss",
			"vfmsub213sd",
			"vfmsub123ss",
			"vfmsub123sd",
			"vfmsub231ss",
			"vfmsub231sd",
			"vfmsub321ss",
			"vfmsub321sd",
			"vfnmadd132ss",
			"vfnmadd132sd",
			"vfnmadd312ss",
			"vfnmadd312sd",
			"vfnmadd213ss",
			"vfnmadd213sd",
			"vfnmadd123ss",
			"vfnmadd123sd",
			"vfnmadd231ss",
			"vfnmadd231sd",
			"vfnmadd321ss",
			"vfnmadd321sd",
			"vfnmsub132ss",
			"vfnmsub132sd",
			"vfnmsub312ss",
			"vfnmsub312sd",
			"vfnmsub213ss",
			"vfnmsub213sd",
			"vfnmsub123ss",
			"vfnmsub123sd",
			"vfnmsub231ss",
			"vfnmsub231sd",
			"vfnmsub321ss",
			"vfnmsub321sd",
			"rdfsbase",
			"rdgsbase",
			"rdrand",
			"wrfsbase",
			"wrgsbase",
			"vcvtph2ps",
			"vcvtps2ph",
			"adcx",
			"adox",
			"rdseed",
			"clac",
			"stac",
			"xstore",
			"xcryptecb",
			"xcryptcbc",
			"xcryptctr",
			"xcryptcfb",
			"xcryptofb",
			"montmul",
			"xsha1",
			"xsha256",
			"llwpcb",
			"slwpcb",
			"lwpval",
			"lwpins",
			"vfmaddpd",
			"vfmaddps",
			"vfmaddsd",
			"vfmaddss",
			"vfmaddsubpd",
			"vfmaddsubps",
			"vfmsubaddpd",
			"vfmsubaddps",
			"vfmsubpd",
			"vfmsubps",
			"vfmsubsd",
			"vfmsubss",
			"vfnmaddpd",
			"vfnmaddps",
			"vfnmaddsd",
			"vfnmaddss",
			"vfnmsubpd",
			"vfnmsubps",
			"vfnmsubsd",
			"vfnmsubss",
			"vfrczpd",
			"vfrczps",
			"vfrczsd",
			"vfrczss",
			"vpcmov",
			"vpcomb",
			"vpcomd",
			"vpcomq",
			"vpcomub",
			"vpcomud",
			"vpcomuq",
			"vpcomuw",
			"vpcomw",
			"vphaddbd",
			"vphaddbq",
			"vphaddbw",
			"vphadddq",
			"vphaddubd",
			"vphaddubq",
			"vphaddubw",
			"vphaddudq",
			"vphadduwd",
			"vphadduwq",
			"vphaddwd",
			"vphaddwq",
			"vphsubbw",
			"vphsubdq",
			"vphsubwd",
			"vpmacsdd",
			"vpmacsdqh",
			"vpmacsdql",
			"vpmacssdd",
			"vpmacssdqh",
			"vpmacssdql",
			"vpmacsswd",
			"vpmacssww",
			"vpmacswd",
			"vpmacsww",
			"vpmadcsswd",
			"vpmadcswd",
			"vpperm",
			"vprotb",
			"vprotd",
			"vprotq",
			"vprotw",
			"vpshab",
			"vpshad",
			"vpshaq",
			"vpshaw",
			"vpshlb",
			"vpshld",
			"vpshlq",
			"vpshlw",
			"vbroadcasti128",
			"vpblendd",
			"vpbroadcastb",
			"vpbroadcastw",
			"vpbroadcastd",
			"vpbroadcastq",
			"vpermd",
			"vpermpd",
			"vpermps",
			"vpermq",
			"vperm2i128",
			"vextracti128",
			"vinserti128",
			"vpmaskmovd",
			"vpmaskmovq",
			"vpsllvd",
			"vpsllvq",
			"vpsravd",
			"vpsrlvd",
			"vpsrlvq",
			"vgatherdpd",
			"vgatherqpd",
			"vgatherdps",
			"vgatherqps",
			"vpgatherdd",
			"vpgatherqd",
			"vpgatherdq",
			"vpgatherqq",
			"xabort",
			"xbegin",
			"xend",
			"xtest",
			"andn",
			"bextr",
			"blci",
			"blcic",
			"blsi",
			"blsic",
			"blcfill",
			"blsfill",
			"blcmsk",
			"blsmsk",
			"blsr",
			"blcs",
			"bzhi",
			"mulx",
			"pdep",
			"pext",
			"rorx",
			"sarx",
			"shlx",
			"shrx",
			"tzcnt",
			"tzmsk",
			"t1mskc",
			"prefetchwt1",
			"bndmk",
			"bndcl",
			"bndcu",
			"bndcn",
			"bndmov",
			"bndldx",
			"bndstx",
			"sha1msg1",
			"sha1msg2",
			"sha1nexte",
			"sha1rnds4",
			"sha256msg1",
			"sha256msg2",
			"sha256rnds2",
			"kaddb",
			"kaddd",
			"kaddq",
			"kaddw",
			"kandb",
			"kandd",
			"kandnb",
			"kandnd",
			"kandnq",
			"kandnw",
			"kandq",
			"kandw",
			"kmovb",
			"kmovd",
			"kmovq",
			"kmovw",
			"knotb",
			"knotd",
			"knotq",
			"knotw",
			"korb",
			"kord",
			"korq",
			"korw",
			"kortestb",
			"kortestd",
			"kortestq",
			"kortestw",
			"kshiftlb",
			"kshiftld",
			"kshiftlq",
			"kshiftlw",
			"kshiftrb",
			"kshiftrd",
			"kshiftrq",
			"kshiftrw",
			"ktestb",
			"ktestd",
			"ktestq",
			"ktestw",
			"kunpckbw",
			"kunpckdq",
			"kunpckwd",
			"kxnorb",
			"kxnord",
			"kxnorq",
			"kxnorw",
			"kxorb",
			"kxord",
			"kxorq",
			"kxorw",
			"kadd",
			"kand",
			"kandn",
			"kmov",
			"knot",
			"kor",
			"kortest",
			"kshiftl",
			"kshiftr",
			"ktest",
			"kunpck",
			"kxnor",
			"kxor",
			"valignd",
			"valignq",
			"vblendmpd",
			"vblendmps",
			"vbroadcastf32x2",
			"vbroadcastf32x4",
			"vbroadcastf32x8",
			"vbroadcastf64x2",
			"vbroadcastf64x4",
			"vbroadcasti32x2",
			"vbroadcasti32x4",
			"vbroadcasti32x8",
			"vbroadcasti64x2",
			"vbroadcasti64x4",
			"vcmpeq_oqpd",
			"vcmpeq_oqps",
			"vcmpeq_oqsd",
			"vcmpeq_oqss",
			"vcompresspd",
			"vcompressps",
			"vcvtpd2qq",
			"vcvtpd2udq",
			"vcvtpd2uqq",
			"vcvtps2qq",
			"vcvtps2udq",
			"vcvtps2uqq",
			"vcvtqq2pd",
			"vcvtqq2ps",
			"vcvtsd2usi",
			"vcvtss2usi",
			"vcvttpd2qq",
			"vcvttpd2udq",
			"vcvttpd2uqq",
			"vcvttps2qq",
			"vcvttps2udq",
			"vcvttps2uqq",
			"vcvttsd2usi",
			"vcvttss2usi",
			"vcvtudq2pd",
			"vcvtudq2ps",
			"vcvtuqq2pd",
			"vcvtuqq2ps",
			"vcvtusi2sd",
			"vcvtusi2ss",
			"vdbpsadbw",
			"vexp2pd",
			"vexp2ps",
			"vexpandpd",
			"vexpandps",
			"vextractf32x4",
			"vextractf32x8",
			"vextractf64x2",
			"vextractf64x4",
			"vextracti32x4",
			"vextracti32x8",
			"vextracti64x2",
			"vextracti64x4",
			"vfixupimmpd",
			"vfixupimmps",
			"vfixupimmsd",
			"vfixupimmss",
			"vfpclasspd",
			"vfpclassps",
			"vfpclasssd",
			"vfpclassss",
			"vgatherpf0dpd",
			"vgatherpf0dps",
			"vgatherpf0qpd",
			"vgatherpf0qps",
			"vgatherpf1dpd",
			"vgatherpf1dps",
			"vgatherpf1qpd",
			"vgatherpf1qps",
			"vgetexppd",
			"vgetexpps",
			"vgetexpsd",
			"vgetexpss",
			"vgetmantpd",
			"vgetmantps",
			"vgetmantsd",
			"vgetmantss",
			"vinsertf32x4",
			"vinsertf32x8",
			"vinsertf64x2",
			"vinsertf64x4",
			"vinserti32x4",
			"vinserti32x8",
			"vinserti64x2",
			"vinserti64x4",
			"vmovdqa32",
			"vmovdqa64",
			"vmovdqu16",
			"vmovdqu32",
			"vmovdqu64",
			"vmovdqu8",
			"vpabsq",
			"vpandd",
			"vpandnd",
			"vpandnq",
			"vpandq",
			"vpblendmb",
			"vpblendmd",
			"vpblendmq",
			"vpblendmw",
			"vpbroadcastmb2q",
			"vpbroadcastmw2d",
			"vpcmpequb",
			"vpcmpequd",
			"vpcmpequq",
			"vpcmpequw",
			"vpcmpgeb",
			"vpcmpged",
			"vpcmpgeq",
			"vpcmpgeub",
			"vpcmpgeud",
			"vpcmpgeuq",
			"vpcmpgeuw",
			"vpcmpgew",
			"vpcmpgtub",
			"vpcmpgtud",
			"vpcmpgtuq",
			"vpcmpgtuw",
			"vpcmpleb",
			"vpcmpled",
			"vpcmpleq",
			"vpcmpleub",
			"vpcmpleud",
			"vpcmpleuq",
			"vpcmpleuw",
			"vpcmplew",
			"vpcmpltb",
			"vpcmpltd",
			"vpcmpltq",
			"vpcmpltub",
			"vpcmpltud",
			"vpcmpltuq",
			"vpcmpltuw",
			"vpcmpltw",
			"vpcmpneqb",
			"vpcmpneqd",
			"vpcmpneqq",
			"vpcmpnequb",
			"vpcmpnequd",
			"vpcmpnequq",
			"vpcmpnequw",
			"vpcmpneqw",
			"vpcmpngtb",
			"vpcmpngtd",
			"vpcmpngtq",
			"vpcmpngtub",
			"vpcmpngtud",
			"vpcmpngtuq",
			"vpcmpngtuw",
			"vpcmpngtw",
			"vpcmpnleb",
			"vpcmpnled",
			"vpcmpnleq",
			"vpcmpnleub",
			"vpcmpnleud",
			"vpcmpnleuq",
			"vpcmpnleuw",
			"vpcmpnlew",
			"vpcmpnltb",
			"vpcmpnltd",
			"vpcmpnltq",
			"vpcmpnltub",
			"vpcmpnltud",
			"vpcmpnltuq",
			"vpcmpnltuw",
			"vpcmpnltw",
			"vpcmpb",
			"vpcmpd",
			"vpcmpq",
			"vpcmpub",
			"vpcmpud",
			"vpcmpuq",
			"vpcmpuw",
			"vpcmpw",
			"vpcompressd",
			"vpcompressq",
			"vpconflictd",
			"vpconflictq",
			"vpermb",
			"vpermi2b",
			"vpermi2d",
			"vpermi2pd",
			"vpermi2ps",
			"vpermi2q",
			"vpermi2w",
			"vpermt2b",
			"vpermt2d",
			"vpermt2pd",
			"vpermt2ps",
			"vpermt2q",
			"vpermt2w",
			"vpermw",
			"vpexpandd",
			"vpexpandq",
			"vplzcntd",
			"vplzcntq",
			"vpmadd52huq",
			"vpmadd52luq",
			"vpmaxsq",
			"vpmaxuq",
			"vpminsq",
			"vpminuq",
			"vpmovb2m",
			"vpmovd2m",
			"vpmovdb",
			"vpmovdw",
			"vpmovm2b",
			"vpmovm2d",
			"vpmovm2q",
			"vpmovm2w",
			"vpmovq2m",
			"vpmovqb",
			"vpmovqd",
			"vpmovqw",
			"vpmovsdb",
			"vpmovsdw",
			"vpmovsqb",
			"vpmovsqd",
			"vpmovsqw",
			"vpmovswb",
			"vpmovusdb",
			"vpmovusdw",
			"vpmovusqb",
			"vpmovusqd",
			"vpmovusqw",
			"vpmovuswb",
			"vpmovw2m",
			"vpmovwb",
			"vpmullq",
			"vpmultishiftqb",
			"vpord",
			"vporq",
			"vprold",
			"vprolq",
			"vprolvd",
			"vprolvq",
			"vprord",
			"vprorq",
			"vprorvd",
			"vprorvq",
			"vpscatterdd",
			"vpscatterdq",
			"vpscatterqd",
			"vpscatterqq",
			"vpsllvw",
			"vpsraq",
			"vpsravq",
			"vpsravw",
			"vpsrlvw",
			"vpternlogd",
			"vpternlogq",
			"vptestmb",
			"vptestmd",
			"vptestmq",
			"vptestmw",
			"vptestnmb",
			"vptestnmd",
			"vptestnmq",
			"vptestnmw",
			"vpxord",
			"vpxorq",
			"vrangepd",
			"vrangeps",
			"vrangesd",
			"vrangess",
			"vrcp14pd",
			"vrcp14ps",
			"vrcp14sd",
			"vrcp14ss",
			"vrcp28pd",
			"vrcp28ps",
			"vrcp28sd",
			"vrcp28ss",
			"vreducepd",
			"vreduceps",
			"vreducesd",
			"vreducess",
			"vrndscalepd",
			"vrndscaleps",
			"vrndscalesd",
			"vrndscaless",
			"vrsqrt14pd",
			"vrsqrt14ps",
			"vrsqrt14sd",
			"vrsqrt14ss",
			"vrsqrt28pd",
			"vrsqrt28ps",
			"vrsqrt28sd",
			"vrsqrt28ss",
			"vscalefpd",
			"vscalefps",
			"vscalefsd",
			"vscalefss",
			"vscatterdpd",
			"vscatterdps",
			"vscatterpf0dpd",
			"vscatterpf0dps",
			"vscatterpf0qpd",
			"vscatterpf0qps",
			"vscatterpf1dpd",
			"vscatterpf1dps",
			"vscatterpf1qpd",
			"vscatterpf1qps",
			"vscatterqpd",
			"vscatterqps",
			"vshuff32x4",
			"vshuff64x2",
			"vshufi32x4",
			"vshufi64x2",
			"rdpkru",
			"wrpkru",
			"rdpid",
			"clflushopt",
			"clwb",
			"pcommit",
			"clzero",
			"ptwrite",
			"cldemote",
			"movdiri",
			"movdir64b",
			"pconfig",
			"tpause",
			"umonitor",
			"umwait",
			"wbnoinvd",
			"gf2p8affineinvqb",
			"vgf2p8affineinvqb",
			"gf2p8affineqb",
			"vgf2p8affineqb",
			"gf2p8mulb",
			"vgf2p8mulb",
			"vpcompressb",
			"vpcompressw",
			"vpexpandb",
			"vpexpandw",
			"vpshldw",
			"vpshldd",
			"vpshldq",
			"vpshldvw",
			"vpshldvd",
			"vpshldvq",
			"vpshrdw",
			"vpshrdd",
			"vpshrdq",
			"vpshrdvw",
			"vpshrdvd",
			"vpshrdvq",
			"vpdpbusd",
			"vpdpbusds",
			"vpdpwssd",
			"vpdpwssds",
			"vpopcntb",
			"vpopcntw",
			"vpopcntd",
			"vpopcntq",
			"vpshufbitqmb",
			"v4fmaddps",
			"v4fnmaddps",
			"v4fmaddss",
			"v4fnmaddss",
			"v4dpwssds",
			"v4dpwssd",
			"encls",
			"enclu",
			"enclv",
			"clrssbsy",
			"endbr32",
			"endbr64",
			"incsspd",
			"incsspq",
			"rdsspd",
			"rdsspq",
			"rstorssp",
			"saveprevssp",
			"setssbsy",
			"wrussd",
			"wrussq",
			"wrssd",
			"wrssq",
			"enqcmd",
			"enqcmds",
			"serialize",
			"xresldtrk",
			"xsusldtrk",
			"vcvtne2ps2bf16",
			"vdpbf16ps",
			"vp2intersectd",
			"ldtilecfg",
			"sttilecfg",
			"tdpbf16ps",
			"tdpbssd",
			"tdpbsud",
			"tdpbusd",
			"tdpbuud",
			"tileloadd",
			"tileloaddt1",
			"tilerelease",
			"tilestored",
			"tilezero",
			"hint_nop0",
			"hint_nop1",
			"hint_nop2",
			"hint_nop3",
			"hint_nop4",
			"hint_nop5",
			"hint_nop6",
			"hint_nop7",
			"hint_nop8",
			"hint_nop9",
			"hint_nop10",
			"hint_nop11",
			"hint_nop12",
			"hint_nop13",
			"hint_nop14",
			"hint_nop15",
			"hint_nop16",
			"hint_nop17",
			"hint_nop18",
			"hint_nop19",
			"hint_nop20",
			"hint_nop21",
			"hint_nop22",
			"hint_nop23",
			"hint_nop24",
			"hint_nop25",
			"hint_nop26",
			"hint_nop27",
			"hint_nop28",
			"hint_nop29",
			"hint_nop30",
			"hint_nop31",
			"hint_nop32",
			"hint_nop33",
			"hint_nop34",
			"hint_nop35",
			"hint_nop36",
			"hint_nop37",
			"hint_nop38",
			"hint_nop39",
			"hint_nop40",
			"hint_nop41",
			"hint_nop42",
			"hint_nop43",
			"hint_nop44",
			"hint_nop45",
			"hint_nop46",
			"hint_nop47",
			"hint_nop48",
			"hint_nop49",
			"hint_nop50",
			"hint_nop51",
			"hint_nop52",
			"hint_nop53",
			"hint_nop54",
			"hint_nop55",
			"hint_nop56",
			"hint_nop57",
			"hint_nop58",
			"hint_nop59",
			"hint_nop60",
			"hint_nop61",
			"hint_nop62",
			"hint_nop63",
			"cmov",
			"jmp",
			"jo",
			"jno",
			"jc",
			"jnc",
			"je",
			"jne",
			"jz",
			"jnz",
			"ja",
			"jna",
			"js",
			"jns",
			"jp",
			"jnp",
			
			"jnbe",
			"jb",
			"jnae",
			"jae",
			"jnb",
			"jbe",
			"jna",
			"jg",
			"jnle",
			"jl",
			"jnge",
			"jge",
			"jnl",
			"set"
		};
	}

}
