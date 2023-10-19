using Shared.Model;
using System.Net.Sockets;

namespace Shared.Utilities;

public delegate void SocketAcceptedHandler(object sender, Socket e);
public delegate Task ReceiveHandler(byte[] data, int dataSize ,object e);
public delegate void ExceptionHandler(object sender, Exception ex);
public delegate void ConnectedHandler(object sender, string error);
public delegate void DisconnectedHandler(object sender);
