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

docker stop NginxWebServer
check_command_status