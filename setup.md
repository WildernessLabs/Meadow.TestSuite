# Wilderness Labs Meadow.TestSuite Setup

## Serial

Currently TestSuite only supports a serial transport between the test PC and the Meadow.  On the Meadow UART4 is used, which is on pins `D00` and `D01` and is a TTL UART.  

This also requires signal conditioning for a typical PC.  A UBS Serial to TTL Converter is typically required.  Only the TX and RX pins fromt he converter need to be connected.

| TTL Converter | Meadow |
| --- | --- |
| TXD | D00 |
| RXD | D01 |


```
                                              -- TX ---> Meadow D00
                                            /
PC USB <--> USB to Serial TTL Converter <--|
                                            \
                                              -- RX ---> Meadow D01
                                        
```