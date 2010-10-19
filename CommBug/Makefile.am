
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG_X86
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/CommBug.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = winexe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

COMMBUG_EXE_MDB_SOURCE=bin/Debug/CommBug.exe.mdb
COMMBUG_EXE_MDB=$(BUILD_DIR)/CommBug.exe.mdb

endif

if ENABLE_RELEASE_X86
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/CommBug.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

COMMBUG_EXE_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(COMMBUG_EXE_MDB)  

BINARIES = \
	$(COMMBUG)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES =  \
	AboutWindow.cs \
	AssemblyInfo.cs \
	gtk-gui/CommBug.AboutWindow.cs \
	gtk-gui/generated.cs \
	gtk-gui/MainWindow.cs \
	Main.cs \
	MainWindow.cs \
	SerialPortEX.cs \
	StringConverts.cs 

DATA_FILES = 

RESOURCES =  \
	gtk-gui/gui.stetic \
	icons/baudrate/baudrate_24x24.png,CommBug.icons.baudrate.baudrate_24x24.png \
	icons/clear_receive/clear_receive_24x24.png,CommBug.icons.clear_receive.clear_receive_24x24.png \
	icons/clear_send/clear_send_24x24.png,CommBug.icons.clear_send.clear_send_24x24.png \
	icons/commbug/commbug_24x24.png,CommBug.icons.commbug.commbug_24x24.png \
	icons/port/port_24x24.png,CommBug.icons.port.port_24x24.png \
	icons/send/send_24x24.png,CommBug.icons.send.send_24x24.png 

EXTRAS = \
	app.desktop \
	commbug.in 

REFERENCES =  \
	Mono.Posix \
	-pkg:glib-sharp-2.0 \
	-pkg:gtk-sharp-2.0 \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

COMMBUG = $(BUILD_DIR)/commbug

$(eval $(call emit-deploy-wrapper,COMMBUG,commbug,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
