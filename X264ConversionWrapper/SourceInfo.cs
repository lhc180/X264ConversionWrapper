using System;
using System.Text.RegularExpressions;

namespace X264ConversionWrapper
{
	public class SourceInfo
	{
		public const String VIDEO_STREAM_PATTERN = @"Stream .*?: Video: (?<vcodec>.*?),.*?, (?<vwidth>\d+)x(?<vheight>\d+).*?, (?<vbitrate>.*?) kb/s,";
		public const String DURATION_PATTERN = @"Duration: (?<duration>.*?),";
		public const String AUDIO_STREAM_PATTERN = @"Stream .*?: Audio: (?<acodec>.*?), .*, (?<abitrate>.*?) kb/s";
		
		public String Filename { get; set; }
		public String VCodec
		{
			get
			{
				return m_VCodec;
			}
		}
		public String ACodec
		{
			get
			{
				return m_ACodec;
			}
		}
		public int VBitRate
		{
			get
			{
				return m_VBitRate;
			}
		}
		public int ABitRate
		{
			get
			{
				return m_ABitRate;
			}
		}
		public float Duration
		{
			get
			{
				return m_Duration;
			}
		}
		public int VWidth
		{
			get
			{
				return m_VWidth;
			}
		}
		public int VHeight
		{
			get
			{
				return m_VHeight;
			}
		}
		private String m_VCodec;
		private String m_ACodec;
		private int m_VBitRate;
		private int m_ABitRate;
		private float m_Duration;
		private int m_VWidth;
		private int m_VHeight;
		
		public SourceInfo (String filename)
		{
			Filename = filename;
		}
		
		public void parse()
		{
			String opts = "-i \"" + Filename + "\"";
			var proc = Shell.Run(FFMpegCommand.DEFAULT_COMMAND_PATH, opts).Process;
			String output = proc.StandardError.ReadToEnd();
			Match match = Regex.Match(output, VIDEO_STREAM_PATTERN);
			m_VCodec = match.Groups["vcodec"].Value;
			m_VWidth = Int32.Parse(match.Groups["vwidth"].Value);	
			m_VHeight = Int32.Parse(match.Groups["vheight"].Value);
			m_VBitRate = Int32.Parse(match.Groups["vbitrate"].Value);	

			match = Regex.Match(output, DURATION_PATTERN);

			parseDuration(match.Groups["duration"].Value);
			
			match = Regex.Match(output, AUDIO_STREAM_PATTERN);
			m_ACodec = match.Groups["acodec"].Value;
			m_ABitRate = Int32.Parse(match.Groups["abitrate"].Value);
		}
		
		private void parseDuration(String durationStr)
		{
			String [] components = durationStr.Split(new char[]{':'});
			float result = float.Parse(components[2]);
			result += float.Parse(components[1]) * 60.0f;
			result += float.Parse(components[0]) * 3600.0f;
			m_Duration = result;
		}
	}
}

