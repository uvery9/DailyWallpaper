﻿// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2007/11/19/calculating-crc-64-in-c-and-net

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;

namespace DailyWallpaper.HashCalc
{
    /// <summary>
    /// Implements a 64-bit CRC hash algorithm for a given polynomial.
    /// </summary>
    /// <remarks>
    /// For ISO 3309 compliant 64-bit CRC's use CRC64ISO.
    /// This is the ISO-3309 version of CRC-64 algorithm. It is not compatible with the ECMA-182 algorithm.
    /// </remarks>
    public class CRC64 : HashAlgorithm
    {
        public const UInt64 DefaultSeed = 0x0;

        readonly UInt64[] table;

        readonly UInt64 seed;
        UInt64 hash;
        static CancellationToken _token;

        public CRC64(UInt64 polynomial)
            : this(polynomial, DefaultSeed)
        {
        }
        public CRC64(UInt64 polynomial, CancellationToken token)
            : this(polynomial, DefaultSeed, token)
        {
        }

        public CRC64(UInt64 polynomial, UInt64 seed, CancellationToken token = default)
        {
            if (!BitConverter.IsLittleEndian)
                throw new PlatformNotSupportedException("Not supported on Big Endian processors");

            table = InitializeTable(polynomial);
            this.seed = hash = seed;
            _token = token;
        }

        public override void Initialize()
        {
            hash = seed;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            // can't report to progressbar here, because it will HashCore many time.
            hash = CalculateHash(hash, table, array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt64ToBigEndianBytes(hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        public override int HashSize { get { return 64; } }

        protected static UInt64 CalculateHash(UInt64 seed, UInt64[] table, IList<byte> buffer, int start, int size)
        {
            var hash = seed;
            for (var i = start; i < start + size; i++)
                unchecked
                {
                    if (_token.IsCancellationRequested)
                    {
                        _token.ThrowIfCancellationRequested();
                    }
                    hash = (hash >> 8) ^ table[(buffer[i] ^ hash) & 0xff];
                }
            return hash;
        }

        static byte[] UInt64ToBigEndianBytes(UInt64 value)
        {
            var result = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        static UInt64[] InitializeTable(UInt64 polynomial)
        {
            if (polynomial == CRC64ISO.Iso3309Polynomial && CRC64ISO.Table != null)
                return CRC64ISO.Table;

            var createTable = CreateTable(polynomial);

            if (polynomial == CRC64ISO.Iso3309Polynomial)
                CRC64ISO.Table = createTable;

            return createTable;
        }

        protected static ulong[] CreateTable(ulong polynomial)
        {
            var createTable = new UInt64[256];
            for (var i = 0; i < 256; ++i)
            {
                var entry = (UInt64)i;
                for (var j = 0; j < 8; ++j)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry >>= 1;
                createTable[i] = entry;
            }
            return createTable;
        }
    }

    public class CRC64ISO : CRC64
    {
        internal static UInt64[] Table;

        public const UInt64 Iso3309Polynomial = 0xD800000000000000;

        public CRC64ISO()
            : base(Iso3309Polynomial)
        {
        }

        public CRC64ISO(CancellationToken token)
            : base(Iso3309Polynomial, token)
        {
        }

        public CRC64ISO(UInt64 seed)
            : base(Iso3309Polynomial, seed)
        {
        }

        public static UInt64 Compute(byte[] buffer)
        {
            return Compute(DefaultSeed, buffer);
        }

        public static UInt64 Compute(UInt64 seed, byte[] buffer)
        {
            if (Table == null)
                Table = CreateTable(Iso3309Polynomial);

            return CalculateHash(seed, Table, buffer, 0, buffer.Length);
        }
    }
}