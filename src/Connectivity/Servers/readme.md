# Servers

This directory contains a number of scripts and associated data to run servers that can be used for testing network connectivity.  Docker containers have been used where possible to allow portability and consistency across multiple operating environments.

## Web Server

Two web servers are available:

* Python script
* Nginx

### Python Web Server

The Python web server exposes the following resources:

* /post.html
* /small.html
* /very_small.html
* /large.html

### Nginx Web Server

This server uses a Docker container to start Nginx and exposes the resources in the `www` directory.

Two scripts have been provided to start and stop the web server:

* StartWebServer.sh
* StopWebServer.sh

## MQTT Server

The Eclipse Mosquitto docker image is used to provide a MQTT server for local testing.  The configuration of the server is held in the `mosquitto` directory.  Note that this server is insecure as anonymous access is used by default.  This server should not be exposed to the Internet.

Two scripts are provided to start and stop the MQTT server:

* StartMQTTServer.sh
* StopMQTTServer.sh

## Email Server

The Mailhog email container provides a simple SMTP server and a Web Mail interface.  The image exposes the following ports:

* 1025 SMTP server
* 8025 Web interface

Two scripts are provided to start and stop the service:

* StartEmailServer.sh
* StopEmailServer.sh
