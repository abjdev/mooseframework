# moose

![moose logo](https://avatars.githubusercontent.com/u/106945309?s=100&u=a6ab9b8ac3898884b55f5fd1770f07f5514bbe38&v=4)

## Implementation Table

| Feature | Implemented | Works On Hardware (Tested on Supermicro X9DRI-LN4F+) | Notes |
| ------- | ----------- | ---------------------------------------------------- | ----- |
| Error Throwing / Catching | 🟥 | 🟥 | 
| GC | 🟨 | ❓ | Not safe |
| Multiprocessor | 🟩 | 🟩 |
| Multithreading | 🟩 | 🟩 |
| PS2 Keyboard/Mouse(USB Compatible) | 🟩 | 🟩 |
| Intel® Gigabit Ethernet Network | 🟩 | 🟩 |
| Realtek RTL8139 | 🟩 | ❓ |
| ExFAT | 🟩 | 🟩 |
| I/O APIC | 🟩 | 🟩 |
| Local APIC | 🟩 | 🟩 |
| SATA | 🟨 | 🟥 | Cannot read >1 sector at once, Cannot detect sata controller on hardware |
| IDE | 🟩 | 🟩 |
| SMBIOS | 🟩 | 🟩 |
| ACPI | 🟩 | 🟩 |
| IPv4 | 🟩 | 🟩 |
| IPv6 | 🟥 | 🟥 |
| TCP | 🟨 | ❓ | Can't receive large package |
| UDP | 🟩 | ❓ |
| Lan | 🟩 | 🟩 |
| Wan | 🟩 | 🟩 |

| Status | Meaning |
| ------ | ------- |
| 🟩 | Yes | 
| 🟥 | No |
| 🟨 | Partially / W.I.P |
| ⬜ | Depretecated / Obsolete |
| ❓ | Unknown |