using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace BNetServer;

public static class BnetServerCertificate
{
	private const string BNET_SERVER_CERT_RESOURCE = "HermesProxy.BNetServer.pfx";

	public static X509Certificate2 Certificate { get; }

	static BnetServerCertificate()
	{
		Assembly currentAsm = Assembly.GetExecutingAssembly();
		using Stream stream = currentAsm.GetManifestResourceStream("HermesProxy.BNetServer.pfx");
		if (stream == null)
		{
			throw new Exception("Resource not found: 'HermesProxy.BNetServer.pfx'");
		}
		MemoryStream ms = new MemoryStream();
		stream.CopyTo(ms);
		byte[] bytes = ms.ToArray();
		Certificate = new X509Certificate2(bytes);
	}
}
