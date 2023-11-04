## Show System Stats on SSD1306
 This app was created to show Raspberry pi system running ubuntu stats on a SSD1306 128x64 OLED
 
 Runs every 5 Seconds
 
![screen](https://github.com/timothydodd/DotNetRaspStats/assets/8201238/9faae8b0-8f06-400d-aa2d-fe281cc21713)

Fonts Used
https://www.fontspace.com/roboto-font-f13281


### Install Script
``` bash
sudo  mkdir /usr/sbin/DotNetRaspStats
sudo chmod 0755 /usr/sbin/DotNetRaspStats

sudo wget https://github.com/timothydodd/DotNetRaspStats/releases/download/v1.0.3/timothydodd.DotNetRaspStats-refs.tags.v1.0.3-linux-x64.zip &&
	unzip timothydodd.DotNetRaspStats-refs.tags.v1.0.3-linux-x64.zip &&
	sudo rm timothydodd.DotNetRaspStats-refs.tags.v1.0.3-linux-x64.zip &&
    sudo cp -r ./bin/linux-x64/dotnetraspstats.service /etc/systemd/system/ &&
	sudo  cp -r ./bin/linux-x64/. /usr/sbin/DotNetRaspStats &&
    sudo  rm -rf bin &&
	sudo  systemctl start dotnetraspstats.service
```
## ReInstall Script
``` bash

sudo systemctl stop dotnetraspstats.service &&
	rm -rf bin &&
	wget https://github.com/timothydodd/DotNetRaspStats/releases/download/v1.0.3/timothydodd.DotNetRaspStats-refs.tags.v1.0.3-linux-x64.zip &&
	unzip timothydodd.DotNetRaspStats-refs.tags.v1.0.3-linux-x64.zip &&
	rm timothydodd.DotNetRaspStats-refs.tags.v1.0.3-linux-x64.zip &&
	sudo cp -r ./bin/linux-x64/. /usr/sbin/DotNetRaspStats &&
	sudo systemctl start dotnetraspstats.service
```

### Notes
These notes will be cleaned up soon
dotnetraspstats.service
``` bash

requirements

sudo apt-get update && 
sudo apt-get install -y dotnet-sdk-7.0 &&
sudo apt-get install raspi-config &&
sudo apt-get install unzip

#optional configurations
dtoverlay=gpio-shutdown,gpio_pin=21
dtoverlay=act-led,gpio=19



dotnet publish -c Release -r linux-arm64 --self-contained=true -p:PublishSingleFile=true -p:GenerateRuntimeConfigurationFiles=true -o artifacts
#/usr/sbin/DotNetRaspStats

sudo chmod 0755 /usr/sbin/DotNetRaspStats
#copy Files to sbin
sudo cp -r ./ /usr/sbin/DotNetRaspStats

#copy .service file to system
sudo cp ./dotnetraspstats.service /etc/systemd/system/

sudo systemctl daemon-reload

sudo systemctl status dotnetraspstats.service

sudo systemctl start dotnetraspstats.service
sudo systemctl stop dotnetraspstats.service
sudo systemctl restart dotnetraspstats.service
sudo systemctl enable dotnetraspstats.service

compress
    tar -zcvf rasp-stat.tar.gz artifacts
decompress
 tar -zxvf rasp-stat.tar.gz


 scp [file_name]  remoteuser@remotehost:/remote/directory
```


