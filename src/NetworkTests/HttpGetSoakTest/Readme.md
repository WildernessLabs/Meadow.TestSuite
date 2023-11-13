# HTTP Get Soak Test

This test performs multiple HTTP Get requests and can be used for soak testing the system.

## Usage



# Configuration

The application 

```json
{
    "NetworkHttpGetSoakTestConfiguration": {
      "TestURL": "http://192.168.1.174:8080/small.html",
      "Iterations": 1000000
    }
}
```

## Sample Output

```
Meadow StdOut: 08:31:32: Using URL: http://192.168.1.174:8080/small.html, for 1,000,000 iterations.
Meadow StdOut: 08:31:48: Request count - 1
Meadow StdOut: 08:31:48: Free memory: 702,944 bytes
Meadow StdOut: 08:31:49: Request count - 2
.
.
.
Meadow StdOut: 08:32:35: Request count - 49
Meadow StdOut: 08:32:35: Free memory: 706,976 bytes
Meadow StdOut: 08:33:24: Request count - 100
Meadow StdOut: 08:33:24: Free memory: 1,005,352 bytes
.
.
.
Meadow StdOut: 09:01:35: Free memory: 1,007,664 bytes
Meadow StdOut: 09:03:09: Request count - 2,000
Meadow StdOut: 09:03:09: Free memory: 1,033,832 bytes
Meadow StdOut: 09:04:43: Request count - 2,100
Meadow StdOut: 09:04:43: Free memory: 1,009,672 bytes
Meadow StdOut: 09:06:16: Request count - 2,200
Meadow StdOut: 09:06:16: Free memory: 999,440 bytes
```
