using System;

namespace X264ConversionWrapper
{
	public class X264Options : IFFMpegOptions
	{
		public const int DEFAULT_PASS = 1;
		public const int DEFAULT_BIT_RATE = 1000;
		public const X264Presets DEFAULT_PRESET = X264Presets.Slow;
		
		public X264Options ()
		{
			Pass = DEFAULT_PASS;
			BitRate = DEFAULT_BIT_RATE;
			Preset = DEFAULT_PRESET;
		}
		
		public int Pass { get; set; }
		public int BitRate { get; set; }
		public X264Presets Preset { get; set; }
		public String GetArgumentsString ()
		{
			return String.Format("-vcodec libx264 -preset {0} -x264opts pass={1}:bitrate={2}",
				Preset.OptionString(),
				Pass,
				BitRate);
		}
	}
}

