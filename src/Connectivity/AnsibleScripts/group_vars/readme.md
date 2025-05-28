# Ansible Variables

<i>all.yml</i> contains the variable definitions that will be applied to all of the scripts. The variables are as follows:

```yaml
---
ansible_user: clusteruser
hostname: TestServer1000
nvme_duo: false
format_nvmebase: true
esp_idf_installation_directory: ~/esp
esp_idf_branch: release/v5.3
install_nvmebase: true
install_esp_idf: true
install_samba: true
install_rust: true
```

## ansible_user: clusteruser

Name of the default user for the device.  This is used as part of the Samba installation.

## hostname: TestServer1000

Host name for the Raspberry Pi.  This is used by the `ChangeHostname.yml` script.  Note that the reboot task will not reconnect to the Raspberry Pi correctly as the script will be assuming the old name for the host.

## nvme_duo: false

Set this to `true` if the scripts are bing run against against a NVMe Base Duo.  Doing this will configure the system to use both drives.

## format_nvmebase: false

This determines if the NVMe drive should be formatted (have a new file system created on the drive) as part of the installation process.

Set this to <i>true</i> for new drives (or drives where the data is to be discarded.  Set this to <i>false</i> to preserve the contents of the drive.

Setting <i>format_nvmebase</i> to <i>false</i> for an unformatted (new) drive will cause the <i>ConfigureNVMeBase.yml</i> script to fail.

## ESP-IDF Related Variables

### esp_idf_installation_directory: ~/esp

Installation location for the ESP-IDF and related tools

### esp_idf_branch: release/v5.3

ESP-IDF branch to check out.

## install_nvmebase: true

Install the NVME base?

## install_esp_idf: true

Install the ESP-IDF.

## install_samba: true

Install and configure Samba?

## install_rust: true

Install rust and cargo.
