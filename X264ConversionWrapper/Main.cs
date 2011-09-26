using System;

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
		}
	}
}
