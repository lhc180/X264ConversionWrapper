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
			
			VWidth = null;
			VHeight = null;
		}
		
		public int Pass { get; set; }
		public int BitRate { get; set; }
		public X264Presets Preset { get; set; }
		public int? VWidth { get; set; }
		public int? VHeight { get; set; }
		
		public String GetArgumentsString ()
		{
			if (VWidth == null || VHeight == null)
			{
				return String.Format("-vcodec libx264 -preset {0} -x264opts pass={1}:bitrate={2}",
					Preset.OptionString(),
					Pass,
					BitRate);
			}
			else
			{
				return String.Format("-vcodec libx264 -preset {0} -x264opts pass={1}:bitrate={2} -s {3}x{4}",
					Preset.OptionString(),
					Pass,
					BitRate,
					VWidth,
					VHeight);
			}
		}
	}
}

