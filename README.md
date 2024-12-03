# SharpRedirect

SharpRedirect is a simple .NET Framework-based redirector from a specified local port to a destination host and port.

# Features

* Traffic Redirection: Redirects incoming TCP connections from a local port to a specified destination host and port.

* Bidirectional Communication: Supports full duplex communication between the client and the destination.

* Ease of Use: Simple command-line interface for quick setup.

* Lightweight and Efficient: Designed with asynchronous operations for optimal performance.

# Requirements
.NET Framework: Version 4.5 or later.

# Installation
1. Clone the repository or download the source code:

```
git clone https://github.com/your-repo/SharpRedirect.git
```

2. Open the project in Visual Studio.
3. Build the solution to generate the executable. (You can also used the precompiled ones inside Releases folder)

# Usage

Run the executable from the command line with the following syntax:
```
SharpRedirect.exe <localPort> <destinationHost> <destinationPort>
```

Example:
To redirect traffic from `localhost:8080` to `example.com:80`:

```
SharpRedirect.exe 8080 example.com 80
```

# Contribution
Contributions are welcome! If you find a bug or have an idea for improvement, feel free to open an issue or submit a pull request.

# Contact
For any questions or support, please contact:

[The Red Teaming Army](https://discord.com/invite/dWCe5ZMvtQ)

# Thank You

Special thanks to my Patreons for all of their support!

If you appreciate my work you can also support me here [@Lsecqt](https://www.patreon.com/Lsecqt)

Happy redirecting! 🎉