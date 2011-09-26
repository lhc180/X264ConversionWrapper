using System;

namespace X264ConversionWrapper
{
	public static class FFMpegOutputFormatExtensions 
	{
		public static String extSuffix(this FFMpegOutputFormats format)
		{
			switch (format)
			{
			case FFMpegOutputFormats.MP4:
				return "mp4";
			case FFMpegOutputFormats.AVI:
				return "avi";
			case FFMpegOutputFormats.MKV:
				return "mkv";
			default:
				return null;
			}
		}
	}
		
	public enum FFMpegOutputFormats
	{
		MP4,
		MKV,
		AVI
	}
}

