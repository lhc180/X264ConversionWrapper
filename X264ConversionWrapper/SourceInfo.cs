using System;
using System.Text.RegularExpressions;

namespace X264ConversionWrapper
{
	public class SourceInfo
	{
		[Serializable()]
		public class SourceInfoParsingException : Exception
		{
			public SourceInfoParsingException() {}
			public SourceInfoParsingException(String message) : base (message) {}
			public SourceInfoParsingException(String message, Exception inner) : base (message, inner) {}
			public SourceInfoParsingException(System.Runtime.Serialization.SerializationInfo info,
				System.Runtime.Serialization.StreamingContext context) : base (info, context) { }
		}
		
		public const String VIDEO_STREAM_PATTERN = @"Stream .*?: Video: (?<vcodec>.*?),.*?, (?<vwidth>\d+)x(?<vheight>\d+).*?, (?<vbitrate>.*?) kb/s,";
		public const String VIDEO_STREAM_PATTERN_FALLBACK = @"Stream .*?: Video: (?<vcodec>.*?),.*?, (?<vwidth>\d+)x(?<vheight>\d+) ";
		public const String DURATION_PATTERN = @"Duration: (?<duration>.*?), .*?, bitrate: (?<bitrate>\d+) kb/s";
		public const String AUDIO_STREAM_PATTERN = @"Stream .*?: Audio: (?<acodec>.*?), .*, (?<abitrate>.*?) kb/s";
		
		private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof (SourceInfo));
		
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
			proc.WaitForExit();
			String output = proc.StandardError.ReadToEnd();
			
			/*
			if (LOGGER.IsDebugEnabled)
			{
				LOGGER.Debug("Command Output: \n" + output);
			}
			*/
			Match match = Regex.Match(output, DURATION_PATTERN);
			
			if (match.Success)
			{
				parseDuration(match.Groups["duration"].Value);
				int duraionBitrate = Int32.Parse(match.Groups["bitrate"].Value);
				
				match = Regex.Match(output, VIDEO_STREAM_PATTERN);
				if (match.Success)
				{
					m_VCodec = match.Groups["vcodec"].Value;
					m_VWidth = Int32.Parse(match.Groups["vwidth"].Value);	
					m_VHeight = Int32.Parse(match.Groups["vheight"].Value);
					m_VBitRate = Int32.Parse(match.Groups["vbitrate"].Value);						
				}
				else
				{
					match = Regex.Match(output, VIDEO_STREAM_PATTERN_FALLBACK);
					if (match.Success)
					{
						m_VCodec = match.Groups["vcodec"].Value;
						m_VWidth = Int32.Parse(match.Groups["vwidth"].Value);	
						m_VHeight = Int32.Parse(match.Groups["vheight"].Value);
						m_VBitRate = duraionBitrate;
					}
					else
					{
						String [] lines = output.Split('\n');
						if (lines.Length > 1 && lines[lines.Length -2].StartsWith(Filename + ":"))
						{
							throw new SourceInfoParsingException(lines[lines.Length - 2]);
						}
						else
						{
							throw new SourceInfoParsingException("Invalid Video Stream String");
						}
					}
				}
			} 
			else
			{
				throw new SourceInfoParsingException("Invalid Duration String");
			}
			
			match = Regex.Match(output, AUDIO_STREAM_PATTERN);
			if (match.Success)
			{
				m_ACodec = match.Groups["acodec"].Value;
				m_ABitRate = Int32.Parse(match.Groups["abitrate"].Value);
			}
			else
			{
				throw new SourceInfoParsingException("Invalid Audio Stream String");
			}
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

