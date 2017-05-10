﻿using Net.Chdk.Providers.Boot;
using System.IO;

namespace Net.Chdk.Encoders.Binary
{
    public sealed class BinaryEncoder : BinaryEncoderDecoder, IBinaryEncoder
    {
        public BinaryEncoder(IBootProvider bootProvider)
            : base(bootProvider)
        {
        }

        public void Encode(Stream inStream, Stream outStream, int version)
        {
            Validate(inStream: inStream, outStream: outStream, version: version);

            if (version == 0)
            {
                inStream.CopyTo(outStream);
                return;
            }

            Encode(inStream, outStream, Offsets[version - 1]);
        }

        private void Encode(Stream decStream, Stream encStream, int[] offsets)
        {
            var decBuffer = new byte[ChunkSize];
            var encBuffer = new byte[ChunkSize];

            encStream.Write(Prefix, 0, Prefix.Length);

            int size;
            while ((size = decStream.Read(decBuffer, 0, ChunkSize)) > 0)
            {
                for (var start = 0; start < size; start += offsets.Length)
                    for (var index = 0; index < offsets.Length; index++)
                        encBuffer[start + offsets[index]] = Dance(decBuffer[start + index], start + index);
                encStream.Write(encBuffer, 0, size);
            }
        }
    }
}
