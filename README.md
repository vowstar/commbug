# 串口调试助手

## 详细介绍:[Commbug](http://code.google.com/p/commbug/wiki/Commbug)

License: [GNU General Public License v3](http://www.gnu.org/licenses/gpl.html )

公告:本项目已变更版本控制系统.原版本控制系统是Mercurial,新的版本控制系统是Git.
<s>公告:本项目已变更版本控制系统.原版本控制系统是Subversion,新的版本控制系统是Mercurial.</s>

![commbug](http://i659.photobucket.com/albums/uu316/vowstar/screenshot_500.png)

[先前版本截图](http://i659.photobucket.com/albums/uu316/vowstar/screenshot7.png)

-

 * 自版本1.0.5以后使用缓冲区机制，解决了先前数据量大后卡与慢的问题

 * 支持PC所能支持的所有波特率

 * 支持数据位,停止位,奇偶校验的设定

 * 支持文本,16进制,10进制的相互转换

 * 自动列出计算机上的串口设备表

 * 支持自动发送模式,可以调整发送时间

 * 自动检测并且选定新串口设备(如USB转串口线的插入)

---

# 安装方法

## 详细介绍:[HowToInstall](http://code.google.com/p/commbug/wiki/HowToInstall)

### Linux下

#### Ubuntu下

##### Ubuntu 10.04，10.10，13.04, 13.10, 14.04, 14.10 下

打开终端,运行

```bash
    sudo add-apt-repository ppa:huangr08/ppa
    sudo apt-get update
    sudo apt-get install commbug
```

##### Ubuntu 8.04 下

打开终端,运行
    sudo apt-key adv –keyserver keyserver.ubuntu.com –recv-keys D4A1DA23
在/etc/apt/sources.list中添加:

```bash
    deb http://ppa.launchpad.net/huangr08/ppa/ubuntu hardy main
    deb-src http://ppa.launchpad.net/huangr08/ppa/ubuntu hardy main
```

然后

```bash
    sudo apt-get update
    sudo apt-get install commbug
```

##### 其他发行版下

1.使用git获取最新源码

```bash
git clone https://github.com/vowstar/commbug.git commbug
```

如果您不熟悉git，可以[手工下载最新的源码包](https://github.com/vowstar/commbug/archive/master.zip)，然后解压缩。

2.安装编译所依赖的库和软件包

```bash
    autotools-dev,
    mono-devel,  
    libglade2.0-cil-dev,
    libmono-addins-cil-dev (>= 0.3.1),
    libmono-addins-gui-cil-dev (>= 0.3.1),
    libglib2.0-cil-dev,
    libgtk2.0-cil-dev (>= 2.12),
    gconf2,
    libglib2.0-dev,
    libgtk2.0-dev (>= 2.8)
```

3.编译安装

```bash
    ./configure
    make
    sudo make install
```

4.卸载编译所依赖的库和软件包,安装运行所依赖的库和软件包,以便您获得最干净的系统.

```bash
    mono
    libmono-i18n2.0-cil
    libgdiplus
```

5.终端运行

```bash
    commbug
```

### Windows下

##### Windows 2000,XP

- 下载并且安装[.NET Framework 2.0](http://msdn.microsoft.com/en-us/netframework/aa731542.aspx)或者[.NET Framework 3.5](http://www.microsoft.com/downloads/details.aspx?displaylang=en&FamilyID=c17ba869-9671-4330-a63e-1fd44e0e2505)(当然3.5更好).
- 到 http://ftp.novell.com/pub/mono/gtk-sharp/ 下载最新的gtk-sharp并且安装.[下载gtk-sharp-2.12.9-2.win32.msi](http://ftp.novell.com/pub/mono/gtk-sharp/gtk-sharp-2.12.9-2.win32.msi)
- 下载[commbug.exe](http://code.google.com/p/commbug/downloads/list)并且运行.

##### Windows Vista

- 先尝试Windows7的安装方法.
- 若上一步失败,尝试XP下的安装方法.

##### Windows 7

- 到 http://ftp.novell.com/pub/mono/gtk-sharp/ 下载最新的gtk-sharp并且安装.[下载gtk-sharp-2.12.9-2.win32.msi](http://ftp.novell.com/pub/mono/gtk-sharp/gtk-sharp-2.12.9-2.win32.msi)
- 下载[commbug.exe](http://code.google.com/p/commbug/downloads/list)并且运行.

### MAC OS下

git clone源码后，安装monodevelop，编译运行即可。

如有Bug请在[Issues](https://github.com/vowstar/commbug/issues)中反馈。

[欢迎反馈bug给作者](https://github.com/vowstar/commbug/issues)
