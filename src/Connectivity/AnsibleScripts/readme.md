# Ansible Scripts

[Ansible](https://docs.ansible.com/) provides a mechanism to run common tasks such as software installation and configuration across multiple machines.  This ensures a consistent configuration across a number of machines or the ability to create a new machine with a known good configuration.

The scripts included in this repository provide a way of generating a consistent installation of a number of packages on a Raspberry Pi.  This allows the creations / regeneration of a consistent test environment for use with a number of the test suite applications.

## Security

It should be stressed that little has been done about securing the systems on the Raspberry Pi.  The software is not intended to be used to provide services on the Internet, these services are fo local testing only.

## Installation

In order to use Ansible a Raspberry Pi should have a default operating system installed and have had at least one ssh session opened.  Full instructions are provided on the [Raspberry Pi OS](RaspberryPiOS.md) page.

Next it is necessary to install the Ansible tools on a local machine (not the Raspberry Pi) by following the [Ansible installation guide](https://docs.ansible.com/ansible/latest/installation_guide/index.html).


## Ansible Configuration

Two configuration files may need to be changed depending upon the desired outcome:

* _hosts.ini_
* _group_vars/all.yml_

### hosts.ini

This file lists the host(s) (in our case the Raspberry Pi) details, server name, user name and password that will be the target for Ansible.

### group_vars/all.yml

This file controls which of the various install tasks will be executed on the device.  The default configuration file is:

```yaml
---
ansible_user: tester
hostname: wl-test-server
install_nvmebase: false
nvme_duo: false
format_nvmebase: false
install_samba: false
install_rpi_connect: false
install_esp_idf: false
esp_idf_installation_directory: ~/esp
esp_idf_branch: release/v5.4
install_rust: false
install_docker: false
install_bluetooth: false
```

Edit the configuration file changing the settings to deploy the required configuration to the Raspberry Pi.

#### install_nvmebase, nvme_duo and format_nvmebase

These three items install and configure the Pimironi NVME Base or NVME Base Duo.

### install_samba

Should Samba be installed and configured.

#### install_rpi_connect

Install [Raspberry Pi Connect](https://www.raspberrypi.com/software/connect/), this allows the Raspberry Pi to be accessed over the Internet.

#### install_esp_idf, esp_idf_installation_directory and esp_idf_branch

#### install_rust

Install Rust on the Raspberry Pi.

#### install_docker

Install [docker](https://www.docker.com/) on the Raspberry Pi.  This will also copy a number of files, scripts and supporting files, for the servers that can be run using docker on the Raspberry Pi.

#### install_bluetooth

Install the `bleak` Python library and any dependencies into a Python virtual environment.  Add the virtual environment start up code to the `.bashrc` file.

Finally, copy the Bluetooth test files into the `tester` home directory.

## Running the Ansible Playbook

The Ansible playbook is run using the command `ansible-playbook main.yml`.  By default this will always execute a number of standard tasks followed by the optionally configured tasks.  The standard tasks are:

* Update the OS
* Install any supporting software
* Reboot the Raspberry Pi

Running the command `ansible-playbook main.yml` with the default configuration should result in something like this:

```
PLAY [Configure Raspberry Pi] ************************************************************************************************************************

TASK [Gathering Facts] *******************************************************************************************************************************
ok: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
included: /Users/markstevens/GitHub/WildernessLabs/Meadow.TestSuite/src/Connectivity/AnsibleScripts/tasks/Update.yml for wl-test-server.local

TASK [Update apt caches and the distribution] ********************************************************************************************************
changed: [wl-test-server.local]

TASK [Update the EEPROM] *****************************************************************************************************************************
changed: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
included: /Users/markstevens/GitHub/WildernessLabs/Meadow.TestSuite/src/Connectivity/AnsibleScripts/tasks/InstallSupportingSoftware.yml for wl-test-server.local

TASK [Install supporting software.] ******************************************************************************************************************
changed: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
included: /Users/markstevens/GitHub/WildernessLabs/Meadow.TestSuite/src/Connectivity/AnsibleScripts/tasks/Reboot.yml for wl-test-server.local

TASK [Reboot the Raspberry Pi] ***********************************************************************************************************************
changed: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

PLAY RECAP *******************************************************************************************************************************************
wl-test-server.local       : ok=8    changed=4    unreachable=0    failed=0    skipped=7    rescued=0    ignored=0
```

Modifying the _group_vars/all.yml_ file and setting `install_samba: true` and `install_docker: true` will result in something like this:

```
PLAY [Configure Raspberry Pi] ************************************************************************************************************************

TASK [Gathering Facts] *******************************************************************************************************************************
ok: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
included: /Users/markstevens/GitHub/WildernessLabs/Meadow.TestSuite/src/Connectivity/AnsibleScripts/tasks/Update.yml for wl-test-server.local

TASK [Update apt caches and the distribution] ********************************************************************************************************
ok: [wl-test-server.local]

TASK [Update the EEPROM] *****************************************************************************************************************************
changed: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
included: /Users/markstevens/GitHub/WildernessLabs/Meadow.TestSuite/src/Connectivity/AnsibleScripts/tasks/InstallSupportingSoftware.yml for wl-test-server.local

TASK [Install supporting software.] ******************************************************************************************************************
ok: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
included: /Users/markstevens/GitHub/WildernessLabs/Meadow.TestSuite/src/Connectivity/AnsibleScripts/tasks/Reboot.yml for wl-test-server.local

TASK [Reboot the Raspberry Pi] ***********************************************************************************************************************
changed: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
included: /Users/markstevens/GitHub/WildernessLabs/Meadow.TestSuite/src/Connectivity/AnsibleScripts/tasks/InstallSamba.yml for wl-test-server.local

TASK [Install Samba] *********************************************************************************************************************************
changed: [wl-test-server.local]

TASK [Configure the share for the first / only NVMe drive] *******************************************************************************************
changed: [wl-test-server.local]

TASK [Configure the chare for the second drive on the NVMeBase Duo] **********************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
skipping: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
included: /Users/markstevens/GitHub/WildernessLabs/Meadow.TestSuite/src/Connectivity/AnsibleScripts/tasks/InstallDocker.yml for wl-test-server.local

TASK [Install a list of packages] ********************************************************************************************************************
changed: [wl-test-server.local]

TASK [Remove python-configparser package] ************************************************************************************************************
ok: [wl-test-server.local]

TASK [Get docker convenience script] *****************************************************************************************************************
changed: [wl-test-server.local]

TASK [Install docker] ********************************************************************************************************************************
changed: [wl-test-server.local]

TASK [Remove docker convenience script] **************************************************************************************************************
changed: [wl-test-server.local]

TASK [Make tester user execute docker commands] ******************************************************************************************************
changed: [wl-test-server.local]

TASK [include_tasks] *********************************************************************************************************************************
included: /Users/markstevens/GitHub/WildernessLabs/Meadow.TestSuite/src/Connectivity/AnsibleScripts/tasks/CopyFiles.yml for wl-test-server.local

TASK [Copy server support files.] ********************************************************************************************************************
changed: [wl-test-server.local]

PLAY RECAP *******************************************************************************************************************************************
wl-test-server.local       : ok=20   changed=10   unreachable=0    failed=0    skipped=5    rescued=0    ignored=0
```

Note that the first `ansible-playbook main.yml` command did not install docker or Samba but the second invocation after the the file edits did indeed install both docker and Sambe.
