using System;

namespace X264ConversionWrapper
{
	public class X264Converter
	{
		public String InputFilename { get; set; }
		
		public X264Converter (String inputFilename)
		{
			InputFilename = inputFilename;
		}
		
		public void Convert()
		{
			SourceInfo src = new SourceInfo(InputFilename);
			
			if (!src.VCodec.StartsWith("h264"))	return;
			
			// 1280x720 at 1500 kb/s
			if (src.VWidth >= 1280 || src.VHeight >= 720) {
				if (src.VBitRate > 1500) {
					
				} else {
					if (src.VBitRate > 1000) {
						// Reduce Resolusion and Set BitRate to 1000
					} else {
						// Reduce Reolusion Even more and Set BitRate to half	
					}
				}
			}
		}
	}
}

