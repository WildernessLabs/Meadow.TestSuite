 AnsibleScripts   AddAnsibleScripts ● ? ⍟1  ssh tester@wl-test-server.local
 ✔  09:02:07
The authenticity of host 'wl-test-server.local (fe80::64b:a060:3465:ff7b%en0)' can't be established.
ED25519 key fingerprint is SHA256:6Xd+aj3wkkNlfJycLlDG5d6x2/j/lRjXCs9KQaF8vRQ.
This key is not known by any other names.
Are you sure you want to continue connecting (yes/no/[fingerprint])? yes
Warning: Permanently added 'wl-test-server.local' (ED25519) to the list of known hosts.
tester@wl-test-server.local's password: 

The programs included with the Debian GNU/Linux system are free software;
the exact distribution terms for each program are described in the
individual files in /usr/share/doc/*/copyright.

Debian GNU/Linux comes with ABSOLUTELY NO WARRANTY, to the extent
permitted by applicable law.

Wi-Fi is currently blocked by rfkill.
Use raspi-config to set the country before use.

tester@wl-test-server:~ $ exit
exit
Connection to wl-test-server.local closed.
 AnsibleScripts   AddAnsibleScripts ● ? ⍟1  ssh-copy-id tester@wl-test-server.local
 ✔  11.62 Dur  09:02:23
/usr/bin/ssh-copy-id: INFO: Source of key(s) to be installed: ssh-add -L
/usr/bin/ssh-copy-id: INFO: attempting to log in with the new key(s), to filter out any that are already installed
/usr/bin/ssh-copy-id: INFO: 1 key(s) remain to be installed -- if you are prompted now it is to install the new keys
tester@wl-test-server.local's password: 

Number of key(s) added:        1

Now try logging into the machine, with: "ssh 'tester@wl-test-server.local'"
and check to make sure that only the key(s) you wanted were added



ssh tester@wl-test-server.local
 ✔  09:03:18

The programs included with the Debian GNU/Linux system are free software;
the exact distribution terms for each program are described in the
individual files in /usr/share/doc/*/copyright.

Debian GNU/Linux comes with ABSOLUTELY NO WARRANTY, to the extent
permitted by applicable law.

Wi-Fi is currently blocked by rfkill.
Use raspi-config to set the country before use.




ansible-playbook main.yml

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




Modifying (add Samba and Docker) and re-running:

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