#!/bin/bash -ex

set -e
SCRIPT_DIR="$( cd "$(dirname "$0")" ; pwd -P )"

wget -O - https://raw.githubusercontent.com/pjgpetecodes/dotnet8pi/master/install.sh | sudo bash

cat << \EOF >> ~/.bash_profile
# Add .NET Core SDK tools
export PATH="$PATH:/home/wltestuser/.dotnet/tools"
EOF

sudo raspi-config nonint do_leds 0

sudo raspi-config nonint do_spi 0

sudo raspi-config nonint do_i2c 0

sudo raspi-config nonint do_serial_hw 0

sudo raspi-config nonint do_serial_cons 0

sudo docker run -it -d -p 8080:80 --restart always--name web nginx
