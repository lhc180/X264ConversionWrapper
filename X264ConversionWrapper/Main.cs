using System;
using System.Threading;
using System.Diagnostics;

namespace X264ConversionWrapper
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			X264Options x264Options = new X264Options();
			Console.WriteLine (x264Options.GetArgumentsString());
			
			AACOptions aacOptions = new AACOptions();
			Console.WriteLine (aacOptions.GetArgumentsString());
			
			FFMpegCommand command = new FFMpegCommand(aacOptions, x264Options, true, "aced.avi", FFMpegOutputFormats.MP4);
			Console.WriteLine (command.GetCommandString());
			
			SourceInfo src = new SourceInfo("/home/grerlrr/sample.mpg");
			src.parse();
			Console.WriteLine(src.Duration);
			

			Shell.ShellProcess proc = Shell.Run("ffmpeg", "-y -i /home/grerlrr/sample.mpg -vcodec libx264 /home/grerlrr/sample.mp4", null, TestDataReceivedEventHandler);
			//Console.WriteLine(proc.Process.StandardError.ReadToEnd());
			
			Console.WriteLine("Process Started");
			Thread.Sleep(60000);
			
			/*

			while (proc.AsyncThread.ThreadState == System.Threading.ThreadState.Unstarted)
			{
				Console.WriteLine(String.Format("Waiting; Thread Status: {0}", proc.AsyncThread.ThreadState));
				Thread.Sleep(1000);
			}
			*/
		}
		
		private static void TestDataReceivedEventHandler(object process, DataReceivedEventArgs outLine)
		{
			if (!String.IsNullOrEmpty(outLine.Data))
			{
				Console.WriteLine(outLine.Data);
				Thread.Sleep(100);
			}
		}
	}
}
