# Test Suite Worker Configuration

The Test Suite Worker can be configured by modifying and deploying a file named `config.json` to the root (application) folder.

## Example Config File

```
{
  "Display": "ST7789",
  "Network": {
    "SSID": "YOUR_WIFI_SSID",
    "Pass": "YOUR_PASSCODE"
  },
  "Serial": {
    "Enabled": false
  }
}
```