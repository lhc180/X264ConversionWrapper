using System;
using System.Threading;
using System.Data.Linq;

namespace X264ConversionWrapper
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			LoggerInitializer.Initialize();
			
			try
			{
				X264Converter converter = new X264Converter("/home/grerlrr/sample.avi");
				converter.Convert();
				foreach (String output in converter.LastCommandOutputs)
				{
					Console.WriteLine(output);
				}
			}
			catch (SourceInfo.SourceInfoParsingException ex)
			{
				Console.WriteLine(ex.Message);
			}
			
		}
		
	}
}
