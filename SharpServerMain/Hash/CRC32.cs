using System.Security.Cryptography;
using System.Text;

namespace SharpServerMain.Hash
{


    public class CRC32 : HashAlgorithm
    {
        private const uint Polynomial = 0xEDB88320;
        private uint[] table;

        public CRC32()
        {
            table = InitializeTable(Polynomial);
            HashSizeValue = 32;
        }

        public static uint CalculateCrc32(byte[] bytes)
        {
            using (var crc32 = new CRC32())
            {
                byte[] hashBytes = crc32.ComputeHash(bytes);
                return BitConverter.ToUInt32(hashBytes, 0);
            }
        }
        public static uint[] InitializeTable(uint polynomial)
        {
            var table = new uint[256];

            for (uint i = 0; i < 256; i++)
            {
                var entry = i;
                for (var j = 0; j < 8; j++)
                {
                    entry = (entry & 1) != 0 ? (entry >> 1) ^ polynomial : entry >> 1;
                }
                table[i] = entry;
            }

            return table;
        }

        public override void Initialize()
        {
            int p = 5;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            uint crc = 0xFFFFFFFF;
            for (int i = ibStart; i < cbSize; i++)
            {
                crc = (crc >> 8) ^ table[array[i] ^ crc & 0xFF];
            }
            HashValue = BitConverter.GetBytes(~crc);
        }

        protected override byte[] HashFinal()
        {
            return HashValue;
        }
    }
}
