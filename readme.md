## Show System Stats on SSD1306
 This app was created to show Raspberry pi system running ubuntu stats on a SSD1306 128x64 OLED
 
 Runs every 5 Seconds
 
 ![image](https://github.com/timothydodd/DotNetRaspStats/assets/8201238/a0ff867e-8e43-46d1-b7a7-af6eed7aed21)

Fonts Used
https://www.fontspace.com/roboto-font-f13281


### Notes

dotnetraspstats.service
``` bash

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
```


