namespace DLS.Description
{
	public enum ChipType
	{
		Custom,

		// ---- Basic Chips ----
		Nand,
		TriStateBuffer,
		Clock,
		Pulse,
		Detector,

		// ---- Memory ----
		dev_Ram_8Bit,
		CustomRAM,
		Rom_256x16,
		BankedRom,
		EEPROM_256x16,
		CustomRom,
		CustomDisplayRGB,

		// ---- Displays ----
		SevenSegmentDisplay,
		DisplayRGB,
		DisplayRGB8x8,
		DisplayDot,
		DisplayLED,
		DisplayRGBTouch,
		DisplayRGB64x64_XY,
		DisplayRGB256x256_XY,
		DisplayRGB64x64_XY_8bit,
		DisplayRGB256x256_XY_8bit,

		// ---- Merge / Split ----
		Merge_Pin,
		Split_Pin,

		// ---- In / Out Pins ----
		In_Pin,
		Out_Pin,

        Key,
        ASCII,

        Button,		Toggle,

		Constant_8Bit,

        // ---- Buses ----
        Bus,
		BusTerminus,
		
		// ---- Audio ----
		Buzzer,

		// ---- Time ----
		RTC,

		// ---- Clock ----
		SPS,
	}
}