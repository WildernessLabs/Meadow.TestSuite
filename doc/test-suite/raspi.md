# Setting up Test Director on a Raspberry Pi

Setup:
- - Raspberry Pi 4B 4GB
- 32GB Flash SD Card
- Ubuntu 21.04 Server (aarch64)    https://ubuntu.com/download/raspberry-pi
- Balena Etcher (to write the OS to the SD card)
- HDMI Monitor, micro HDMI to HDMI cable
- USB Keyboard
- USB 3 power cable
- Ethernet cable (WiFi is also an option, but I leave that as an exercise for the reader) 

## Writing the OS

1. Use Balena Etcher to write the OS to the SD card
2. Insert the SD card into your Pi and power it up
3. Once the OS boots, login with `ubuntu` as both the username and password.  A password change will be forced at this point.
4. If you want to use SSH from this point on (this tutoruial assumes you will), get the Pi's IP Address
```
$ ip address
```
5. SSH into the Pi
```
> ssh ubuntu@[your_pi_address]
```

## Installing .NET 5.0 SDK

Here we opt to install the full SDK.  Only the runtime is required, so if you want a smaller install, you can find online documentation for that.

```
$ wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
$ sudo dpkg -i packages-microsoft-prod.deb
$ rm packages-microsoft-prod.deb
$ wget https://dot.net/v1/dotnet-install.sh
$ chmod +x dotnet-install.sh
$ ./dotnet-install.sh -c 5.0
$ export DOTNET_ROOT=~/.dotnet
$ export PATH=$PATH:$DOTNET_ROOT
$ nano ~/.bashrc
```
Add the following lines to the bottom of the file
```
$ export DOTNET_ROOT=~/.dotnet
$ export PATH=$PATH:$DOTNET_ROOT
```

## Publish Director application

``` 
> [Test Suite Path]\Meadow.TestSuite.Director> dotnet publish
> [Test Suite Path]\Meadow.TestSuite.Director>scp -r ./bin/debug/net5.0/publish/ ubuntu@192.168.0.2:/home/ubuntu/testsuite2
```

