#!/bin/bash

set -e
scriptdir="$( cd "$(dirname "$0")" ; pwd -P )"

#
#   Work out the OS so that we can change actions per OS where necessary.
#
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

#
#   Validate command line.
#
input="$1"
ip_regex='^([0-9]{1,3}\.){3}[0-9]{1,3}$'
if [[ -z "$input" ]]; then
    echo "Error: IP address expected"
    echo "Usage: $0 <IPv4 address>"
    exit 1
fi
if [[ $input =~ $ip_regex ]]; then
    echo "Valid address: $input"
else
    echo "Error: '$input' is not a valid IPv4 address."
    exit 1
fi

#
#   Make 10,000 HTTP requests to the /Update endpoint.
#
for i in $(seq 1 10000); do
    echo "Request $i of 10,000"
    curl -s "http://$input/Update" -o /dev/null -m 30
    exit_status=$?
    if [ $exit_status -ne 0 ]; then
      echo "Error executing curl command. Exit status: $exit_status"
      exit 1
    fi
    sleep 10
done

echo "Done"