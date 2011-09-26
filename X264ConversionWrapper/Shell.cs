using System;
using System.Diagnostics;

namespace X264ConversionWrapper
{
	public static class Shell
	{
		public static Process run(String cmd, String args) 
		{
			ProcessStartInfo procStartInfo = new ProcessStartInfo(cmd, args);
			procStartInfo.RedirectStandardError = true;
			procStartInfo.RedirectStandardOutput = true;
			procStartInfo.UseShellExecute = false;
			procStartInfo.CreateNoWindow = true;
			
			Process proc  = new Process();
			proc.StartInfo = procStartInfo;
			proc.Start();
			
			return proc;
		}
	}
}

