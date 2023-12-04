# Network Tests

This folder contains the scripts to run a simple web server using a nginx docker container.  These script will require docker to be installed on the machine being used as a test host.

## Start and Stop the Web Server

Two scripts have been provided to start and stop the web server:

* StartWebServer.sh
* StopWebServer.sh

## Web Content

The web server serves static web pages and these can be found in the www directory.  Additional content should be placed in this directory.

## HttpGetSoakTest Application

This application can be used to perform a number of HttpGet requests.  The URI, number of requests and pause between requests can all be configured through the `app.config.yaml` file.

Note that this application assumes that automatic network startup is configured and the `wifi.config.yaml` file contains valid access point credentials.
