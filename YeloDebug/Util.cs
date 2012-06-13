using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace YeloDebug
{
	/// <summary>
	/// Miscellaneous helper functions.
	/// </summary>
	public static class Util
	{
        /// <summary>
        /// Converts a timestamp to its local DateTime equivalent.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime TimeStampToLocalDateTime(uint timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0) + new TimeSpan(0, 0, (int)timestamp);
        }

        /// <summary>
        /// Converts a timestamp to its universal DateTime equivalent.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime TimeStampToUniversalDateTime(uint timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, (int)timestamp);
        }


        public static void DataToFile(ref byte[] data, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write)) fs.Write(data, 0, data.Length);
        }

		public static unsafe uint FloatToUInt32(float value)	{ return *(uint*)&value; }

		public static string ReadCString(BinaryReader b)
		{
			byte btchar = 0;
			System.Text.StringBuilder cstring = new System.Text.StringBuilder();

			do
			{
				btchar = b.ReadByte();
				if (btchar != 0) cstring.Append((char)btchar);
			} while (btchar != 0);

			if (cstring.Length != 0)
				return cstring.ToString();
			else
				return string.Empty;
		}

		public static void WriteCString(BinaryWriter b, string str)
		{
			b.Write(str.ToCharArray());
			b.Write(byte.MinValue);
		}

		public static byte[] StringToASCIIArray(string str)		{ return ASCIIEncoding.ASCII.GetBytes(str); }

		public static byte[] StringToUnicodeArray(string str)	{ return UnicodeEncoding.Unicode.GetBytes(str); }

		public static byte[] StringToHexBytes(string str)
		{
			byte[] Hex = new byte[str.Length / 2];

			for (int i = 0; i < str.Length / 2; i++)
				Hex[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);

			return Hex;
		}

        public static string HexBytesToString(byte[] hexBytes)
        {
            string hexStr = string.Empty;
            for (int i = 0; i < hexBytes.Length; i++)
                hexStr += Convert.ToString(hexBytes[i], 16).PadLeft(2, '0').ToUpper();
            return hexStr;
        }

        public static byte[] ExtractResource(string Path, string Name)
		{
			Stream ResourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("YeloDebug." + Path + Name);
			int Size = (int)ResourceStream.Length;
			byte[] ResourceData = new byte[Size];
			ResourceStream.Read(ResourceData, 0, Size);
			return ResourceData;
		}

		public static void SaveResourceToFile(string ResourcePath, string ResourceName, string ExtractionPath)
		{
			FileStream fs = new FileStream(ExtractionPath + ResourceName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
			byte[] ResourceBuffer = ExtractResource(ResourcePath, ResourceName);
			fs.Write(ResourceBuffer, 0, ResourceBuffer.Length);
			fs.Close();
		}

        /// <summary>
        /// Provides an easy way to modify and inject custom assembly scripts.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static byte[] BuildScript(string name, params object[] args)
        {
            byte[] script = ExtractResource("Resource.", name);

            // initialize our streams
            MemoryStream ms = new MemoryStream(script);
            BinaryReader br = new BinaryReader(ms);
            BinaryWriter bw = new BinaryWriter(ms);

            ms.Position = 0;
            int specialIndex = 0;
            for (int i = 0; i < script.Length - 4; i++)
            {
                if (br.ReadUInt32() == 0xEFBEADDE)  // DEADBEEF
                {
                    // replace with special
                    ms.Position -= 4;
                    bw.Write((uint)args[specialIndex]);
                    specialIndex++;
                }
                ms.Position -= 3;
            }
            ms.Close();

            return script;
        }



        // methods to parse responses

        public static object GetResponseInfo(string response, int index)
        {
            char[] delimiters = { ' ', '\r' };
            string value = (response.Substring(response.IndexOf('=') + 1).Split('='))[index];
            string val = value.Remove(value.IndexOfAny(delimiters));

            if (val[0] == '\"') return (string)val.Substring(1, val.Length - 2);
            else if (val.Length > 2 && val.Remove(2) == "0x") return (uint)Convert.ToUInt32(val.Substring(2), 16);
            else return (uint)Convert.ToUInt32(val);
        }

        public static List<object> ExtractResponseInformation(string responseLine)
        {
            List<object> responses = new List<object>();
            char[] delimiters = {' ', '\r'};

            string[] values = responseLine.Substring(responseLine.IndexOf('=') + 1).Split('=');
            foreach (string value in values)
            {
                string val = value.Remove(value.IndexOfAny(delimiters));

                if (val[0] == '\"') responses.Add((string)val.Substring(1, val.Length - 2));
                else if (val.Length > 2 && val.Remove(2) == "0x") responses.Add((uint)Convert.ToUInt32(val.Substring(2), 16));
                else responses.Add((uint)Convert.ToUInt32(val));
            }
            return responses;
        }


        public static uint CelsiusToFahrenheit(uint degrees)
        {
            return (uint)(1.8f * degrees + 32);
        }

        public static string Reverse(string str)
        {
            char[] ca = str.ToCharArray();
            Array.Reverse(ca);
            return (new string(ca));
        }

        public static string ToHexString(long value)
        {
            string str = string.Empty;
            while (value > 0)
            {
                byte val = (byte)(value & 0x0f);
                str = (char)(val > 0x09 ? ((val & 0x0f) + 65 - 0x09) : (val & 0x0f) + 48) + str;
                value >>= 8;
                val = (byte)(value & 0x0f);
                str = (char)(val > 0x09 ? ((val & 0x0f) + 65 - 0x09) : (val & 0x0f) + 48) + str;
                value >>= 8;
            }
            return str;
        }
	};
}