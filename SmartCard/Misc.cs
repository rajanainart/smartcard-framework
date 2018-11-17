using System;
using System.Text;
using System.IO;
using System.Drawing;

namespace SmartCardManagement
{
    #region Convertor

    public enum PaddingStyle { START, END }

    public sealed class Convertor
    {
        public static string ByteArrayToString(byte[] data)
        {
            return ASCIIEncoding.ASCII.GetString(data);
        }

        public static string ByteArrayToUnicode(byte[] data)
        {
            return UnicodeEncoding.Unicode.GetString(data);
        }

        public static byte[] UnicodeToByteArray(string data, int? arrayLength, PaddingStyle? style)
        {
            if (!arrayLength.HasValue)
                return UnicodeEncoding.Unicode.GetBytes(data);
            else
            {
                if ((arrayLength.Value % 2) != 0)
                    throw new InvalidDataException("For Unicode format, the array length should be an even number");
                byte[] temp  = UnicodeEncoding.Unicode.GetBytes(data);
                int length   = 0;
                if (arrayLength.Value < temp.Length)
                    length   = temp.Length;
                else
                    length   = arrayLength.Value;
                byte[] array = new byte[length];

                if (!style.HasValue || (style.HasValue && style.Value == PaddingStyle.START))
                {
                    for (byte idx = 0; idx < length - temp.Length; idx += 2)
                        array[idx] = 32; //space
                    for (byte idx = Convert.ToByte(length - temp.Length), idx1 = 0; idx < length; idx++, idx1++)
                        array[idx] = temp[idx1];
                }
                else
                {
                    for (byte idx = 0; idx < temp.Length; idx++)
                        array[idx] = temp[idx];
                    for (byte idx = (byte)temp.Length; idx < array.Length; idx+=2)
                        array[idx] = 32;
                }
                return array;
            }
        }

        public static byte[] StringToByteArray(string data, int? arrayLength, PaddingStyle? style)
        {
            if (!arrayLength.HasValue)
                return ASCIIEncoding.ASCII.GetBytes(data);
            else
            {
                byte[] array = new byte[arrayLength.Value];
                byte[] temp  = ASCIIEncoding.ASCII.GetBytes(data);

                if (!style.HasValue || (style.HasValue && style.Value == PaddingStyle.START))
                {
                    for (byte idx = 0; idx < arrayLength.Value - temp.Length; idx++)
                        array[idx] = 32; //space
                    for (byte idx = Convert.ToByte(arrayLength.Value - temp.Length), idx1 = 0; idx < arrayLength.Value; idx++, idx1++)
                        array[idx] = temp[idx1];
                }
                else
                {
                    for (byte idx = 0; idx < temp.Length; idx++)
                        array[idx] = temp[idx];
                    for (byte idx = (byte)temp.Length; idx < array.Length; idx++)
                        array[idx] = 32;
                }
                return array;
            }
        }

        public static Image ByteArrayToImage(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            return Image.FromStream(stream);
        }

        public static byte[] ImageToByteArray(Image data, System.Drawing.Imaging.ImageFormat format, int? arrayLength)
        {
            MemoryStream stream = new MemoryStream();
            data.Save(stream, format);
            
            byte[] bytes = stream.GetBuffer();
            if (arrayLength.HasValue)
            {
                if (arrayLength.Value <= bytes.Length)
                    return bytes;
                else
                {
                    byte[] temp = new byte[arrayLength.Value];
                    for (int idx = 0; idx < bytes.Length; idx++)
                        temp[idx] = bytes[idx];
                    return temp;
                }
            }
            else
                return bytes;
        }

        public static byte[] NumberToByteArray(int number, byte? arrayLength)
        {
            byte[] array = { };
            byte length = 0;
            string hex  = string.Empty;

            if (number >= 0 && number <= Math.Pow(2, 8) - 1)
            {
                length = 1;
                hex    = string.Format("{0:X2}", number);
            }
            else if (number >= Math.Pow(2, 8) && number <= Math.Pow(2, 16) - 1)
            {
                length = 2;
                hex    = string.Format("{0:X4}", number);
            }
            else if (number >= Math.Pow(2, 16) && number <= Math.Pow(2, 24) - 1)
            {
                length = 3;
                hex    = string.Format("{0:X6}", number);
            }
            else if (number >= Math.Pow(2, 24) && number <= Math.Pow(2, 32) - 1)
            {
                length = 4;
                hex    = string.Format("{0:X8}", number);
            }

            array = new byte[length];
            for (byte idx = 0; idx < length; idx++)
                array[idx] = Convert.ToByte(hex.Substring(idx * 2, 2), 16);

            if (!arrayLength.HasValue)
                return array;
            else
            {
                byte[] temp = new byte[arrayLength.Value];
                for (byte idx = 0; idx < arrayLength.Value - array.Length; idx++)
                    temp[idx] = 0;
                for (byte idx = Convert.ToByte(arrayLength.Value - array.Length), idx1 = 0; idx < arrayLength.Value; idx++, idx1++)
                    temp[idx] = array[idx1];

                return temp;
            }
        }

        public static int ByteArrayToNumber(byte[] bytes)
        {
            return int.Parse(GetHexString(bytes).Replace(" ", ""), System.Globalization.NumberStyles.HexNumber);
        }

        public static byte[] Get255ByteArray(byte[] bytes, byte part)
        {
            byte[] temp = { };
            
            if (bytes.Length <= 255)
                return bytes;
            else
            {
                temp = new byte[255];
                for (int idx = ((part - 1) * 255), idx1 = 0; idx < (part * 255) && idx < bytes.Length; idx++, idx1++)
                    temp[idx1] = bytes[idx];
                return temp;
            }
        }

        public static string GetHexString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.AppendFormat("{0:X2} ", b);

            return builder.ToString();
        }
    }

    #endregion
}
