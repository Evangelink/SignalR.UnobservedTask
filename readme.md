# SignalR demo

Disclaimer: The reproducer is far from ideal but that's the best I could come up with so far.

1. Open `SignarlRDemo.sln`
2. Open `App.fs`
3. Put a breakpoint on line 45 `| Something -> count <- count + 1`
4. Put a breakpoint on line 55 `Debug.Fail (sprintf "Unobserved after %d\n\n%A" count args.Exception))`
5. Run the server
6. Debug the client
7. When 1st breakpoint is reached, wait for a solid minute then continue debug, in the following couple of seconds the 2nd breakpoint should be hit.

NOTE: If the 2nd breakpoint is not hit, restart the client side and repeat the process (I usually get the 2nd breakpoint hit every 2-3 runs).