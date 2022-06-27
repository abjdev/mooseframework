﻿//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Collections.Generic;

namespace moose.Audio
{
    public static unsafe class Audio
    {
        public const int SampleRate = 48000;

        public static bool HasAudioDevice;

        public static Queue<byte[]> Queue;

        public static void Initialize()
        {
            Queue = new();
            HasAudioDevice = false;
        }

        /// <summary>
        /// Play a 48khz dual channel PCM
        /// To convert an "mp3" file to a "pcm" file with ffmpeg, use this command in your terminal
        /// ffmpeg -i input.mp3 -ar 48000 -ac 2 -f s16le output.pcm
        /// </summary>
        /// <param name="pcm">PCM File ( in byte[] )</param>
        public static void Play(byte[] pcm)
        {
            fixed (byte* ptr_pcm = pcm)
            {
                for (int i = pcm.Length; i >= 0; i -= SampleRate * 2)
                {
                    byte[] pack = new byte[SampleRate * 2];
                    fixed (byte* ptr = pack)
                    {
                        Native.Movsb(ptr, ptr_pcm + i, (ulong)pack.Length);
                    }
                    Queue.Enqueue(pack);
                }
            }
        }
    }
}