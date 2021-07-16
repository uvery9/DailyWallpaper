Download from
https://docs.microsoft.com/en-us/sysinternals/downloads/handle
# Handle v4.22

- 09/07/2020

**By Mark Russinovich**

Published: June 14, 2019

[![Download](https://docs.microsoft.com/en-us/sysinternals/downloads/media/shared/download_sm.png)](https://download.sysinternals.com/files/Handle.zip) [**Download Handle**](https://download.sysinternals.com/files/Handle.zip) **(887 KB)**

## Introduction

Ever wondered which program has a particular file or directory open? Now you can find out. *Handle* is a utility that displays information about open handles for any process in the system. You can use it to see the programs that have a file open, or to see the object types and names of all the handles of a program.

You can also get a GUI-based version of this program, [*Process Explorer*](https://docs.microsoft.com/en-us/sysinternals/downloads/process-explorer), here at Sysinternals.



## Installation

You run *Handle* by typing "handle". You must have administrative privilege to run *Handle*.



## Usage

*Handle* is targeted at searching for open file references, so if you do not specify any command-line parameters it will list the values of all the handles in the system that refer to open files and the names of the files. It also takes several parameters that modify this behavior.

**usage: handle [[-a] [-u] | [-c <handle> [-l] [-y]] | [-s]] [-p <processname>|<pid>> [name]**

| Parameter | Description                                                  |
| :-------- | :----------------------------------------------------------- |
| **-a**    | Dump information about all types of handles, not just those that refer to files. Other types include ports, Registry keys, synchronization primitives, threads, and processes. |
| **-c**    | Closes the specified handle (interpreted as a hexadecimal number). You must specify the process by its PID. **WARNING:** Closing handles can cause application or system instability. |
| **-l**    | Dump the sizes of pagefile-backed sections.                  |
| **-y**    | Don't prompt for close handle confirmation.                  |
| **-s**    | Print count of each type of handle open.                     |
| **-u**    | Show the owning user name when searching for handles.        |
| **-p**    | Instead of examining all the handles in the system, this parameter narrows Handle's scan to those processes that begin with the name process. Thus: **handle -p exp** would dump the open files for all processes that start with "exp", which would include Explorer. |
| **name**  | This parameter is present so that you can direct Handle to search for references to an object with a particular name. For example, if you wanted to know which process (if any) has "c:\windows\system32" open you could type: **handle windows\system** The name match is case-insensitive and the fragment specified can be anywhere in the paths you are interested in. |

## Handle Output

When not in search mode (enabled by specifying a name fragment as a parameter), Handle divides its output into sections for each process it is printing handle information for. Dashed lines are used as a separator, immediately below which you will see the process name and its process id (PID). Beneath the process name are listed handle values (in hexadecimal), the type of object the handle is associated with, and the name of the object if it has one.

When in search mode, *Handle* prints the process names and id's are listed on the left side and the names of the objects that had a match are on the right.