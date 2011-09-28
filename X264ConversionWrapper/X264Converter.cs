using System;
using System.Diagnostics;

namespace X264ConversionWrapper
{
	public class X264Converter
	{
		public String InputFilename { get; set; }
		public X264Options X264Options { get; set; }
		public AACOptions AACOptions { get; set; }
		
		private SourceInfo m_SourceInfo;
		public SourceInfo SourceInfo { get { return m_SourceInfo; } }
		
		public X264Converter (String inputFilename)
		{
			InputFilename = inputFilename;
			X264Options = new X264Options();
			AACOptions = new AACOptions();
			m_SourceInfo = new SourceInfo(InputFilename);
			m_SourceInfo.parse();
		}
		
		private bool GenerateOutputOptions()
		{
			
			if (SourceInfo.VCodec.StartsWith("h264"))	return false;
			
			// 1280x720 at 1500 kb/s
			if (SourceInfo.VWidth >= 1280 || SourceInfo.VHeight >= 720) {
				if (SourceInfo.VBitRate > 1500)
				{
					return false;		
				}
				else
				{
					if (SourceInfo.VBitRate > 1000)
					{
						// TODO Reduce Resolusion to near 1280 x 720 
						// Set BitRate to 1000
						X264Options.BitRate = 1000;
					} 
					else
					{
						// Reduce Reolusion Even more and Set BitRate to half
						// TODO
						return false;
					}
				}
			}
			else if (SourceInfo.VWidth >= 800 || SourceInfo.VHeight >= 600)
			{
				X264Options.BitRate = 800;
			}
			else
			{
				int bitRate = SourceInfo.VBitRate / 2;
				if (bitRate < 512)
				{
					return false;
				}
				X264Options.BitRate = bitRate;
			}
			
			if (SourceInfo.ABitRate >= 192 )
				AACOptions.BitRate = 128;
			else if (SourceInfo.ABitRate >= 128)
				AACOptions.BitRate = 64;
			else
				AACOptions.BitRate = SourceInfo.ABitRate * 2 / 3;
			return true;
		}
		
		public void ConvertPass1()
		{
			X264Options.Pass = 1;
			FFMpegCommand command = new FFMpegCommand(AACOptions, X264Options, true, InputFilename, FFMpegOutputFormats.MP4);
			Shell.ShellProcess shProc = Shell.Run(command.CommandPath, command.GetCommandArgs(), null, DataReceived);
			shProc.Process.WaitForExit();
		}
		
		public void ConvertPass2()
		{
			X264Options.Pass = 2;
			FFMpegCommand command = new FFMpegCommand(AACOptions, X264Options, true, InputFilename, FFMpegOutputFormats.MP4);
			Shell.ShellProcess shProc = Shell.Run(command.CommandPath, command.GetCommandArgs(), null, DataReceived);
			shProc.Process.WaitForExit();
		}
		
		public void Convert()
		{
			if (GenerateOutputOptions())
			{
				ConvertPass1();
				ConvertPass2();
			}
		}
		
		public static void DataReceived(Object process, DataReceivedEventArgs recvData)
		{
			Console.WriteLine(recvData.Data);
		}
	}
}

