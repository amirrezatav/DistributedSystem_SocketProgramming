using Shared.Model;
namespace Shared.Utilities;

public delegate void SocketAcceptedHandler(object sender, AcceptedSocket e);
public delegate void ConnectedHandler(object sender, string error);
public delegate void DisconnectedHandler(object sender);
