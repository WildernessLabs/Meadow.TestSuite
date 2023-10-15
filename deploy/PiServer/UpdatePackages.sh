#!/bin/bash -e

set -e
SCRIPT_DIR="$( cd "$(dirname "$0")" ; pwd -P )"

#
#   Update the packages installed on each node in the cluster.
#
# ansible-playbook 02-UpdatePackages.yml -i Node0-3.ini --extra-vars "ansible_password=$CLUSTER_PASSWORD"
ansible-playbook cluster.yml -e target=cluster -i hosts -vvv --extra-vars "ansible_user=WLTestUser ansible_password=$CLUSTER_PASSWORD"
# ansible-playbook cluster.yml --check -e target=cluster -i hosts --extra-vars "ansible_user=clusteruser ansible_password=$CLUSTER_PASSWORD"
# ansible cluster -m ping -i hosts --extra-vars "ansible_user=clusteruser ansible_password=$CLUSTER_PASSWORD"
