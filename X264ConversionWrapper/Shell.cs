using System;
using System.Diagnostics;
using System.Threading;

namespace X264ConversionWrapper
{
	public class Shell
	{
		public static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(Shell));

		public class ShellProcess
		{
			public ProcessStartInfo ProcStartInfo { get; set; }
			
			private Process m_Process;
			public Process Process { get { return m_Process; } }
			
			private DataReceivedEventHandler m_OutputDataReceivedEventHandler;
			public DataReceivedEventHandler OutputDataReceivedEventHandler { get { return m_OutputDataReceivedEventHandler; } }
			
			private DataReceivedEventHandler m_ErrorDataReceivedEventHandler;
			public DataReceivedEventHandler ErrorDataReceivedEventHandler { get { return m_ErrorDataReceivedEventHandler; } }
			
			
			public ShellProcess(String cmd, String args)
			{
				ProcStartInfo = new ProcessStartInfo(cmd, args);
				ProcStartInfo.RedirectStandardError = true;
				ProcStartInfo.RedirectStandardOutput = true;
				ProcStartInfo.UseShellExecute = false;
				ProcStartInfo.CreateNoWindow = true;
				m_OutputDataReceivedEventHandler = null;
				m_ErrorDataReceivedEventHandler = null;
			}
			
			public ShellProcess(String cmd, String args, DataReceivedEventHandler outputDataReceivedEventHandler)
			{
				ProcStartInfo = new ProcessStartInfo(cmd, args);
				ProcStartInfo.RedirectStandardError = true;
				ProcStartInfo.RedirectStandardOutput = true;
				ProcStartInfo.UseShellExecute = false;
				ProcStartInfo.CreateNoWindow = true;
				m_OutputDataReceivedEventHandler = outputDataReceivedEventHandler;
				m_ErrorDataReceivedEventHandler = null;
			}
			

			public ShellProcess(String cmd, String args, DataReceivedEventHandler outputDataReceivedEventHandler,
				DataReceivedEventHandler errorDataReceivedEventHandler)
			{
				ProcStartInfo = new ProcessStartInfo(cmd, args);
				ProcStartInfo.RedirectStandardError = true;
				ProcStartInfo.RedirectStandardOutput = true;
				ProcStartInfo.UseShellExecute = false;
				ProcStartInfo.CreateNoWindow = true;
				m_OutputDataReceivedEventHandler = outputDataReceivedEventHandler;
				m_ErrorDataReceivedEventHandler = errorDataReceivedEventHandler;
			}
			
			public void Run()
			{
				m_Process = new Process();
				if (OutputDataReceivedEventHandler != null)
				{
					m_Process.OutputDataReceived += OutputDataReceivedEventHandler;
				}
				
				if (ErrorDataReceivedEventHandler != null)
				{
					m_Process.ErrorDataReceived += ErrorDataReceivedEventHandler;
				}
				
				m_Process.StartInfo = ProcStartInfo;
				m_Process.Start();
				
				if (OutputDataReceivedEventHandler != null)
				{
					m_Process.BeginOutputReadLine();
				}
				
				if (ErrorDataReceivedEventHandler != null)
				{
					m_Process.BeginErrorReadLine();
				}
			}
		}
		
		public static ShellProcess Run(String cmd, String args) 
		{
			return RunShellProcess(new ShellProcess(cmd, args));
		}
		
		public static ShellProcess Run(String cmd, String args, 
			DataReceivedEventHandler outputDataReceivedEventHandler)
		{
			return RunShellProcess(new ShellProcess(
				cmd, args, outputDataReceivedEventHandler));
		}
		
		public static ShellProcess Run(String cmd, String args,
			DataReceivedEventHandler outputDataReceivedEventHandler, 
			DataReceivedEventHandler errorDataReceivedEventHandler)
		{
			return RunShellProcess(new ShellProcess(
				cmd, args, outputDataReceivedEventHandler, errorDataReceivedEventHandler));

		}
		
		public static ShellProcess RunShellProcess(ShellProcess shProcess)
		{

			if (LOGGER.IsDebugEnabled)
			{
				LOGGER.DebugFormat("Running Command: {0} {1}",
					shProcess.ProcStartInfo.FileName,
					shProcess.ProcStartInfo.Arguments);
			}

			shProcess.Run();
			return shProcess;
		}
	}
}

