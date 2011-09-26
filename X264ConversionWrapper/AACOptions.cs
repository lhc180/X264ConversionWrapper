using System;

namespace X264ConversionWrapper
{
	public class AACOptions : IFFMpegOptions
	{
		public const int DEFAULT_BIT_RATE = 128;
		public AACOptions ()
		{
			BitRate = DEFAULT_BIT_RATE;
		}
		
		public int BitRate { get; set; } // Unit k
		public String GetArgumentsString() 
		{
			return String.Format("-strict experimental -acodec aac -ab {0}k", BitRate);
		}
	}
}

