using System.Net.Sockets;
using Framework;
using Framework.Logging;
using Framework.Networking;

namespace HermesProxy.World.Server;

public class WorldSocketManager : SocketManager<WorldSocket>
{
	private AsyncAcceptor _instanceAcceptor;

	private int _socketSendBufferSize;

	private bool _tcpNoDelay;

	public override bool StartNetwork(string bindIp, int realmPort, int threadCount = 1)
	{
		this._tcpNoDelay = true;
		this._socketSendBufferSize = -1;
		if (!base.StartNetwork(bindIp, realmPort, threadCount))
		{
			return false;
		}
		this._instanceAcceptor = new AsyncAcceptor();
		if (!this._instanceAcceptor.Start(bindIp, Settings.InstancePort))
		{
			Log.Print(LogType.Error, "StartNetwork failed to start instance AsyncAcceptor", "StartNetwork", "WorldSocketManager.cs");
			return false;
		}
		this._instanceAcceptor.AsyncAcceptSocket(OnSocketOpen);
		return true;
	}

	public override void StopNetwork()
	{
		this._instanceAcceptor.Close();
		base.StopNetwork();
		this._instanceAcceptor = null;
	}

	public override void OnSocketOpen(Socket sock)
	{
		Log.Print(LogType.Network, "Instance socket open.", "OnSocketOpen", "WorldSocketManager.cs");
		try
		{
			if (this._socketSendBufferSize >= 0)
			{
				sock.SendBufferSize = this._socketSendBufferSize;
			}
			sock.NoDelay = this._tcpNoDelay;
		}
		catch (SocketException ex)
		{
			Log.Print(LogType.Error, ((object)ex).ToString(), "OnSocketOpen", "WorldSocketManager.cs");
			return;
		}
		base.OnSocketOpen(sock);
	}
}
