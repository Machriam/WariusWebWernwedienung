#!/bin/bash

# 1. Check if an argument is provided; if not, show help and exit
if [ -z "$1" ]; then
    echo "Usage: $0 [on|off]"
    echo "Example: $0 on"
    exit 1
fi

# 2. Grab the first argument and force it to lowercase (handles "ON", "On", etc.)
ACTION="${1,,}"

# 3. Validate that the action is either 'on' or 'off'
if [[ "$ACTION" == "on" || "$ACTION" == "off" ]]; then
    
    # Execute the curl command, injecting the $ACTION variable into the URL
    curl -s -X POST "http://192.168.178.36/$ACTION" \
         -H "Content-Type: application/json" \
         -d ""
    
    # Print a clean confirmation message
    echo -e "\nSuccess: Sent '$ACTION' command."
    
else
    # Show an error if they typed something like ./script.sh restart
    echo "Error: Invalid command '$1'."
    echo "Usage: $0 [on|off]"
    exit 1
fi