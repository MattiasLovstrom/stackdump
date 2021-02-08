# stackdump
=============
Stackdump delivers functionality to troubleshoot .Net processes running .Net core. It includes:
Stackdump.exe – tool to dump all .Net running stacks to the console
Stackdump is designed to be run in a production environment. Stackdump will attach itself as a debugger to the process and temporary stop it; dump all running stacks to the console and start to process one again (this normally takes 5 – 10 seconds, and the application will be stopped during this time but it will continue running after the stacks are listed).      
  
Installation
------------
Uncompress the zip file or copy it from the another location (no installation is needed and all programs are standalone that can be copied separately to the machine you need to troubleshoot on)

Usage
------
Open a command shell as an administrator (or the same user that runs the process you want to investigate)
Open task manager to get the process identity (PID). If this column doesn’t exist select it in menu “View->Select columns…” first.
Execute StackDump with the PID of the running process you want to examine.

Example of dumping all stacks from the running site with process id 6928
stackdump.exe 6928

Syntax
------
StackDump   [/i:<regular expresion to include threads with>] [/e:<regular expresion to exclude threads with>] <process id>

Example of dumping all stacks from the same process and excluding EPiServers workflow and scheduler instance. 
stackdump.exe /e:WorkflowFoundation /e:WaitForConnect /e:BeginReadMessage 6928

Analyzing the result
--------------------
StackDump will list all running threads on the same machine; native threads will be empty but listed. The first line in a managed thread will indicate what this thread is hanging on the time when the stacks were taken.  The last line will indicate what method starting the execution of this stack.

Tips and trix
-------------
Stackdump only take a snapshot what the process is doing, taking more dumps after each other will gave a result similar to a profiler; so it is possible to see what the process is doing over time,

To troubleshooting a high CPU hang – take tree to five stack dumps when the process is loaded over 80% CPU; usually the same problematic stack appears several times on different threads on different time.       

To troubleshooting a low CPU hang – take tree to five stack dumps when the process is going slow and the CPU is not very loaded; the hanging threads will show up with the same “OS thread Id”. 



