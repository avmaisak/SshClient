using System;

namespace SshClient.Host.Infrastructure
{
	public class SshClientException : Exception
	{
		public SshClientException(string message) : base(message) { }
	}
}
