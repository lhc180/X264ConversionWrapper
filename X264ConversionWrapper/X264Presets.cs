using System;

namespace X264ConversionWrapper
{
	public static class X264PresetsExtensions
	{
		public const String ULTRA_FAST = "ultrafast";
		public const String SUPER_FAST = "superfast";
		public const String VERY_FAST = "veryfast";
		public const String FAST = "fast";
		public const String MEDIUM = "medium";
		public const String SLOW = "slow";
		public const String SLOWER = "slower";
		public const String VERY_SLOW = "veryslow";
		public const String PLACEBO = "placebo";
		
		public static String OptionString(this X264Presets preset) {
			switch (preset) {
			case X264Presets.UltraFast:
				return ULTRA_FAST;
			case X264Presets.SuperFast:
				return SUPER_FAST;
			case X264Presets.VeryFast:
				return VERY_FAST;
			case X264Presets.Fast:
				return FAST;
			case X264Presets.Medium:
				return MEDIUM;
			case X264Presets.Slow:
				return SLOW;
			case X264Presets.Slower:
				return SLOWER;
			case X264Presets.VerySlow:
				return VERY_SLOW;
			case X264Presets.Placebo:
				return PLACEBO;
			default:
				return null;
			}
		}
	}
	
	public enum X264Presets
	{
		UltraFast,
		SuperFast,
		VeryFast,
		Faster,
		Fast,
		Medium,
		Slow,
		Slower,
		VerySlow,
		Placebo
	}
}

