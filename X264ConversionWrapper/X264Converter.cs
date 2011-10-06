using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Data.Linq;
using System.Reflection;

namespace X264ConversionWrapper
{
	public class X264ConversionRules
	{
		public const String DEFAULT_CONVERSION_RULES_FILE_NAME = "rules.xml";
		
		public class VideoRule	
		{
			public int SourceVWidth { get; set; }
			public int SourceVHeight { get; set; }
			public int SourceBitRate { get; set; }
			public float TargetSizeRatio { get; set;}
			public float VBitRate { get; set; }
			public bool Quit { get; set; }
		}
		
		public class AudioRule	
		{
			public int SourceBitRate { get; set; }
			public float ABitRate { get; set; }
		}
		
		private VideoRule[] m_VideoRules;
		private AudioRule[] m_AudioRules;
		
		public VideoRule[] VideoRules { get { return m_VideoRules; } }
		public AudioRule[] AudioRules { get { return m_AudioRules; } }
		
		public static X264ConversionRules GetRules()
		{
			Assembly asm = Assembly.GetExecutingAssembly();		
			String path = System.IO.Path.GetDirectoryName(asm.Location);
			path = System.IO.Path.Combine(path, DEFAULT_CONVERSION_RULES_FILE_NAME);
			
			XElement root = XElement.Load(path);
			
			var vrules = (from vrule in root.Elements("VRule") 
				select new VideoRule {
					SourceVWidth = Convert.ToInt32(vrule.Attribute("SourceVWidth").Value),
					SourceVHeight = Convert.ToInt32(vrule.Attribute("SourceVHeight").Value),
					SourceBitRate = Convert.ToInt32(vrule.Attribute("SourceBitRate").Value),
					TargetSizeRatio = Convert.ToSingle(vrule.Attribute("TargetSizeRatio").Value),
					VBitRate = Convert.ToSingle(vrule.Attribute("VBitRate").Value),
					Quit = Convert.ToBoolean(vrule.Attribute("Quit").Value)}).ToArray();
			
			var arules = (from arule in root.Elements("ARule")
				select new AudioRule {
					SourceBitRate = Convert.ToInt32(arule.Attribute("SourceBitRate").Value),
					ABitRate = Convert.ToSingle(arule.Attribute("ABitRate").Value)}).ToArray();

			X264ConversionRules rules = new X264ConversionRules();
			rules.m_VideoRules = vrules.ToArray();
			rules.m_AudioRules = arules.ToArray();

			return rules;
		}
	}
	
	public class X264Converter
	{
		private static readonly log4net.ILog CONVERTER_LOGGER = log4net.LogManager.GetLogger(typeof(X264Converter));
		public const int LAST_COMMAND_QUEUE_SIZE = 10;

		public String InputFilename { get; set; }
		public X264Options X264Options { get; set; }
		public AACOptions AACOptions { get; set; }
		
		private SourceInfo m_SourceInfo;
		public SourceInfo SourceInfo { get { return m_SourceInfo; } }
		
		private Queue<String> m_LastCommandOutputs;
		public Queue<String> LastCommandOutputs { get { return m_LastCommandOutputs; } }
		
		public X264Converter (String inputFilename)
		{
			InputFilename = inputFilename;
			X264Options = new X264Options();
			AACOptions = new AACOptions();
			m_SourceInfo = new SourceInfo(InputFilename);
			m_SourceInfo.parse();
			m_LastCommandOutputs = new Queue<String>(LAST_COMMAND_QUEUE_SIZE);
		}
		
		private void AdjustSizeWithOriginalRatio(X264Options options, float ratio)
		{
			options.VWidth = (int)((float)options.VWidth * ratio);
			options.VHeight = (int)((float)options.VHeight * ratio);
		}
		
		private bool GenerateOutputOptions()
		{
			
			if (SourceInfo.VCodec.StartsWith("h264")) return false;
			
			X264ConversionRules rules = X264ConversionRules.GetRules();
			
			if (CONVERTER_LOGGER.IsDebugEnabled)
			{
				CONVERTER_LOGGER.DebugFormat("{0} video rules acquired", rules.VideoRules.Length);
				CONVERTER_LOGGER.DebugFormat("{0} audio rules acquired", rules.AudioRules.Length);
			}
			
			foreach (X264ConversionRules.VideoRule vrule in rules.VideoRules)
			{
				if ((SourceInfo.VWidth >= vrule.SourceVWidth || SourceInfo.VHeight >= vrule.SourceVHeight) && 
					SourceInfo.VBitRate >= vrule.SourceBitRate)
				{
					if (vrule.Quit) return false;
					
					if (vrule.VBitRate < 10)
					{
						X264Options.BitRate = (int)(SourceInfo.VBitRate * vrule.VBitRate);
					}
					else
					{
						X264Options.BitRate = (int) vrule.VBitRate;
					}
					
					if (vrule.TargetSizeRatio != 1.0f)
					{
						X264Options.VWidth = SourceInfo.VWidth;
						X264Options.VHeight = SourceInfo.VHeight;
						AdjustSizeWithOriginalRatio(X264Options, vrule.TargetSizeRatio);
					}
					
					AACOptions.BitRate = SourceInfo.ABitRate;
					foreach (X264ConversionRules.AudioRule arule in rules.AudioRules)
					{
						if (AACOptions.BitRate >= arule.SourceBitRate)
						{
							if (arule.ABitRate < 10)
							{
								AACOptions.BitRate = (int)(SourceInfo.ABitRate * arule.ABitRate);
							}
							else
							{
								AACOptions.BitRate = (int)arule.ABitRate;
							}
							break;
						}
					}
					return true;
				}
			}
			
			return false;
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
			CleanTempFiles();
		}
		
		public void CleanTempFiles()
		{
			System.IO.File.Delete("x264_2pass.log");
			System.IO.File.Delete("x264_2pass.log.mbtree");
		}
		
		public void Convert()
		{
			if (GenerateOutputOptions())
			{
				ConvertPass1();
				ConvertPass2();
			}
			else
			{
				Console.WriteLine("No need to convert");
			}
		}
		
		public void DataReceived(Object process, DataReceivedEventArgs recvData)
		{
			if (m_LastCommandOutputs.Count == LAST_COMMAND_QUEUE_SIZE)
			{
				m_LastCommandOutputs.Dequeue();
			}
			m_LastCommandOutputs.Enqueue(recvData.Data);
		}
	}
}

