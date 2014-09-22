DEF_TABNAME     = def
DEF_BISON_SRCS  = def.y

FAKE_ALL: all

LIBTARGET =	libcdef.a

HEADERS =	 \
			DFEF_malloc.h \
			DFEF_stringhash.h \
			DFEF_util.h \
			def.tab.h \
			lex.h \
			lex.cpph

PUBLIC_HDRS = \
			defiAlias.h \
			defiAssertion.h \
			defiBlockage.h \
			defiComponent.h \
			defiDebug.h \
			defiDefs.h \
			defiFill.h \
			defiFPC.h \
			defiGroup.h \
			defiIOTiming.h \
			defiKRDefs.h \
			defiMisc.h \
			defiNet.h \
			defiNonDefault.h \
			defiPartition.h \
			defiPath.h \
			defiPinCap.h \
			defiPinProp.h \
			defiProp.h \
			defiPropType.h \
			defiRegion.h \
			defiRowTrack.h \
			defiScanchain.h \
			defiSite.h \
			defiSlot.h \
			defiTimingDisable.h \
			defiUser.h \
			defiUtil.h \
			defiVia.h \
			defrCallBacks.h \
			defrReader.h \
			defwWriter.h \
			defwWriterCalls.h

LIBSRCS =	\
			DFEF_malloc.c \
			DFEF_stringhash.c \
			def.tab.c \
			def_keywords.c \
			defiAlias.c \
			defiAssertion.c \
			defiBlockage.c \
			defiComponent.c \
			defiDebug.c \
			defiFPC.c \
			defiFill.c \
			defiGroup.c \
			defiIOTiming.c \
			defiMisc.c \
			defiNet.c \
			defiNonDefault.c \
			defiPartition.c \
			defiPath.c \
			defiPinCap.c \
			defiPinProp.c \
			defiProp.c \
			defiPropType.c \
			defiRegion.c \
			defiRowTrack.c \
			defiScanchain.c \
			defiSite.c \
			defiSlot.c \
			defiTimingDisable.c \
			defiUtil.c \
			defiVia.c \
			defrReader.c \
			defwWriter.c \
			defwWriterCalls.c

def.tab.c : $(DEF_BISON_SRCS)
	bison -v -p$(DEF_TABNAME)yy -d $(DEF_BISON_SRCS)

include ../template.mk
