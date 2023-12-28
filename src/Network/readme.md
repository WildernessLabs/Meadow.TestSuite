# Network Tests

This folder contains the scripts to run a simple web server using a nginx docker container.  These script will require docker to be installed on the machine being used as a test host.

## Start and Stop the Web Server

Two scripts have been provided to start and stop the web server:

* StartWebServer.sh
* StopWebServer.sh

## Web Content

The web server serves static web pages and these can be found in the www directory.  Additional content should be placed in this directory.

## Test Runner

The test runner is made up of two parts:

* The application running the tests and giving feedback
* A number of registered tests.

### Test Runner Application

This application talks to the hardware and uses the application configuration file (`app.config.yaml` to determine which test should be executed.  The configuration file also contains and parameters for the various tests.

Feedback on test progress is available through a number of methods:

* SSD1306 OLED display connected via I2C.
* 8 LEDs
* Serial port
* Console output

The LEDs and OLED display allow a long running test to be executed without and attached console for feedback.

Note that the runner application assumes that automatic network startup is configured and the `wifi.config.yaml` file contains valid access point credentials.

### Soak Tests (`ISoakTest`)

The `ISoakTest` interface defines the methods that should be implemented in order to setup a test and execute one iteration of the test.  The actual test to be executed is defined in the application configuration, the test object is acquired using reflection.

## Python Web Server

The _PythonWebServer_ directory contains a small Python script to run as a test web server.  The script presents the following endpoints:

* post.html - Accept a message using POST
* small.html - Small web page using GET
* very_small.html - Smaller message using GET
* large.html - Large web page using GET
