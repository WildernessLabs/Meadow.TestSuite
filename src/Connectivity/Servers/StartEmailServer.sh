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
#       Set the name of the container to MailhogEmailServer
#       Expose ports 1025 (inbound email) and 8025 (web mail)
#       Run in detached mode
#       Set the container to be removed when it is stopped
#
docker run --platform linux/amd64 -d -it -p 1025:1025 -p 8025:8025 --rm --name MailhogEmailServer mailhog/mailhog
check_command_status