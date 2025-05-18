#!/bin/bash -e

scriptdir="$( cd "$(dirname "$0")" ; pwd -P )"

#
#   Work out the OS so that we can change actions per OS where necessary.
#
set_os_name()
{
  shopt -s nocasematch
  case "$(uname -a)" in
    *darwin*)
      OS="mac"
      ;;
    *linux*)
      OS="linux"
      ;;
    cygwin*|mingw32*|msys*|mingw*)
      OS="windows"
      ;;
    *)
      OS="unknown"
      ;;
  esac
}

#
# Check if the shell is interactive.
#
check_if_interactive() {
  if [[ $- == *i* ]]; then
    red=`tput setaf 1`
    green=`tput setaf 2`
    reset=`tput sgr0`
  fi
}

#
#   Check if the last command was successful.
#
check_command_status() {
  exit_status=$?
  if [ $exit_status -ne 0 ]; then
    printf " ${red}error${reset}\n"
    if ! $VERBOSE; then
        printf "Re-run the script with --verbose flag to see the output.\n"
    fi
    exit 1
  else
    printf " ${green}success${reset}\n"
  fi
}

#
#   Start Nginx in a docker container.
#       Set the name of the container to MQTTServer
#       Configuration, data and logs will be stored in the directories under mosquitto
#       Expose port 1883
#       Run in detached mode
#       Set the container to be removed when it is stopped
#
docker run --rm --name MQTTServer -it -p 1883:1883 -v "$PWD/mosquitto/config:/mosquitto/config" -v "$PWD/mosquitto/data:/mosquitto/data" -v "$PWD/mosquitto/log:/mosquitto/log" -d eclipse-mosquitto
check_command_status