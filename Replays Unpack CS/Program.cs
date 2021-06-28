﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BlowFishCS;
using System.IO.Compression;

namespace Replays_Unpack_CS
{
    class NetPacket
    {
        public uint size;
        public string type;
        public float time;
        public MemoryStream rawData;

        public NetPacket(MemoryStream stream)
        {
            var payloadSize = new byte[4];
            var payloadType = new byte[4];
            var payloadTime = new byte[4];

            stream.Read(payloadSize);
            stream.Read(payloadType);
            stream.Read(payloadTime);

            size = BitConverter.ToUInt32(payloadSize);
            type = BitConverter.ToUInt32(payloadType).ToString("X2");
            time = BitConverter.ToSingle(payloadTime);

            var data = new byte[size];
            stream.Read(data);
            rawData = new MemoryStream(data);
        }
    }

    class BinaryStream
    {
        private uint length;
        public MemoryStream value;

        public BinaryStream(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var bLen = new byte[4];
            stream.Read(bLen);
            length = BitConverter.ToUInt32(bLen);
            var bValue = new byte[length];
            stream.Read(bValue);
            value = new MemoryStream(bValue);
        }
    }

    class EntityMethod
    {
        public uint entityId;
        public uint messageId;
        public BinaryStream data;

        public EntityMethod(MemoryStream stream)
        {
            var bEntityId = new byte[4];
            var bMessageId = new byte[4];

            stream.Read(bEntityId);
            stream.Read(bMessageId);

            entityId = BitConverter.ToUInt32(bEntityId);
            messageId = BitConverter.ToUInt32(bMessageId);

            var payload = new MemoryStream();
            stream.CopyTo(payload);
            data = new BinaryStream(payload);
        }
    }


    class Program
    {
        static IEnumerable<(int, byte[])> ChunkData(byte[] data, int len = 8)
        {
            int idx = 0;
            for (var s = 0; s <= data.Length; s += len)
            {
                byte[] g;
                try
                {
                    g = data[s..(s + len)];
                }
                catch (ArgumentOutOfRangeException)
                {
                    g = data[s..];
                }

                idx += 1;
                yield return (idx, g);
            }
        }

        static void Main(string[] args)
        {
            using (FileStream fs = File.OpenRead(@"C:\Games\World_of_Warships\replays\20210627_151609_PBSD598-Black-Cossack_53_Shoreside.wowsreplay"))
            {
                byte[] bReplaySignature = new byte[4];
                byte[] bReplayBlockCount = new byte[4];
                byte[] bReplayBlockSize = new byte[4];
                byte[] bReplayJSONData;

                fs.Read(bReplaySignature, 0, 4);
                fs.Read(bReplayBlockCount, 0, 4);
                fs.Read(bReplayBlockSize, 0, 4);

                int jsonDataSize = BitConverter.ToInt32(bReplayBlockSize, 0);

                bReplayJSONData = new byte[jsonDataSize];
                fs.Read(bReplayJSONData, 0, jsonDataSize);

                string sReplayJSONData = Encoding.UTF8.GetString(bReplayJSONData);
                Console.WriteLine(sReplayJSONData);

                using (var memStream = new MemoryStream())
                {
                    fs.CopyTo(memStream);
                    var sBfishKey = "\x29\xB7\xC9\x09\x38\x3F\x84\x88\xFA\x98\xEC\x4E\x13\x19\x79\xFB";
                    var bBfishKey = sBfishKey.Select(x => Convert.ToByte(x)).ToArray();
                    var bfish = new BlowFish(bBfishKey);
                    long prev = 0;
                    using var compressedData = new MemoryStream();
                    foreach (var chunk in ChunkData(memStream.ToArray()[8..]))
                    {
                        try
                        {
                            var decrypted_block = BitConverter.ToInt64(bfish.Decrypt_ECB(chunk.Item2));
                            if (prev != 0)
                            {
                                decrypted_block ^= prev;
                            }
                            prev = decrypted_block;
                            compressedData.Write(BitConverter.GetBytes(decrypted_block));
                        }
                        catch (ArgumentOutOfRangeException)
                        {

                        }
                    }
                    compressedData.Seek(2, SeekOrigin.Begin); //DeflateStream doesn't strip the header so we strip it manually.
                    var decompressedData = new MemoryStream(); 
                    using (DeflateStream df = new(compressedData, CompressionMode.Decompress))
                    {
                        df.CopyTo(decompressedData);
                    }
                    Console.WriteLine(decompressedData.Length);
                    decompressedData.Seek(0, SeekOrigin.Begin);
                    while (decompressedData.Position != decompressedData.Length)
                    {
                        var np = new NetPacket(decompressedData);
                        Console.WriteLine("{0}: {1}", np.time, np.type);
                        if (np.type == "08")
                        {
                            var em = new EntityMethod(np.rawData);
                            //Console.WriteLine(Encoding.UTF8.GetString(em.data.value.ToArray()));
                            //Console.WriteLine("{0}: {1}", em.entityId, em.messageId);
                        }
                    }
                }
                Console.ReadLine();
            }
        }
    }
}
