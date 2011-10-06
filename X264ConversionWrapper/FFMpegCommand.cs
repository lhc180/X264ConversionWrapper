using System;
using System.Text;

namespace X264ConversionWrapper
{
	public class FFMpegCommand
	{
		public const String DEFAULT_COMMAND_PATH = "ffmpeg";
		public AACOptions AACOptions { get; set; }
		public X264Options X264Options { get; set; }
		public bool OverwriteFile { get; set; }
		public String InputFileName { get; set; }
		public FFMpegOutputFormats OutputFormat { get; set; }
		public String OutputFileName
		{
			get
			{
				if (X264Options.Pass == 1) {
					return "-f rawvideo /dev/null";	
				} else {
					int lastIndex = InputFileName.LastIndexOf(".");
					String filename = InputFileName.Substring(0, lastIndex + 1);
					return filename + OutputFormat.extSuffix();
				}
			}
		}
		public String CommandPath { get; set; }
		
		public FFMpegCommand ()
		{
			AACOptions = new AACOptions();
			X264Options = new X264Options();
			OverwriteFile = true;
			OutputFormat = FFMpegOutputFormats.MP4;
			CommandPath = DEFAULT_COMMAND_PATH;
		}
		
		public FFMpegCommand ( AACOptions aacOptions, X264Options x264Options,
			bool overwriteFile, String inputFilename, FFMpegOutputFormats outputFormat)
		{
			AACOptions = aacOptions;
			X264Options = x264Options;
			OverwriteFile = overwriteFile;
			InputFileName = inputFilename;
			OutputFormat = outputFormat;
			CommandPath = DEFAULT_COMMAND_PATH;
		}
		
		public String GetCommandString()
		{
			return CommandPath + ' ' + GetCommandArgs();
		}
		
		public String GetCommandArgs()
		{
			StringBuilder result = new StringBuilder();
			result.Append("-i ");
			result.Append(InputFileName);
			if (OverwriteFile) 
			{
				result.Append(' ');
				result.Append("-y");
			}
			if (AACOptions != null && X264Options.Pass != 1)
			{
				result.Append(' ');
				result.Append(AACOptions.GetArgumentsString());
			}
			result.Append(' ');
			result.Append(X264Options.GetArgumentsString());
			result.Append(' ');
			result.Append(OutputFileName);
			return result.ToString();
		}
		
	}
}
