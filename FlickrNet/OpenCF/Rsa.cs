//==========================================================================================
//
//		OpenNETCF.Windows.Forms.Rsa
//		Copyright (c) 2003, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//		!!! A HUGE thank-you goes out to Casey Chesnut for supplying this class library !!!
//      !!! You can contact Casey at http://www.brains-n-brawn.com                      !!!
//
//==========================================================================================
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace FlickrNet.OpenCF
{
    internal class Rsa
    {
        public Rsa()
        { }

        /// <summary>
        /// rips apart rawKey into public byte [] for RsaParameters class
        /// </summary>
        public Rsa(byte[] rawKey)
        {
            this.rawKey = rawKey; //596

            pks = new PUBLICKEYSTRUC();
            pks.bType = rawKey[0]; //7
            pks.bVersion = rawKey[1]; //2
            pks.reserved = BitConverter.ToUInt16(rawKey, 2); //0
            pks.aiKeyAlg = BitConverter.ToUInt32(rawKey, 4); //41984

            kb = (KeyBlob)pks.bType; //PRIVATE
            c = (Calg)pks.aiKeyAlg; //RSA_KEYX

            if (kb != KeyBlob.PUBLICKEYBLOB && kb != KeyBlob.PRIVATEKEYBLOB)
            {
                throw new Exception("unsupported blob type");
            }

            rpk = new RSAPUBKEY();
            rpk.magic = BitConverter.ToUInt32(rawKey, 8); //843141970
            rpk.bitlen = BitConverter.ToUInt32(rawKey, 12); //1024
            rpk.pubexp = BitConverter.ToUInt32(rawKey, 16); //65537
            uint byteLen = rpk.bitlen / 8; //128

            SetSizeAndPosition(rpk.bitlen);

            //public
            Modulus = Format.GetBytes(this.rawKey, modulusPos, modulusLen, true);
            Exponent = Format.GetBytes(this.rawKey, exponentPos, exponentLen, true);
            //private
            if (kb == KeyBlob.PRIVATEKEYBLOB)
            {
                P = Format.GetBytes(this.rawKey, prime1Pos, prime1Len, true);
                Q = Format.GetBytes(this.rawKey, prime2Pos, prime2Len, true);
                DP = Format.GetBytes(this.rawKey, exponent1Pos, exponent1Len, true);
                DQ = Format.GetBytes(this.rawKey, exponent2Pos, exponent2Len, true);
                InverseQ = Format.GetBytes(this.rawKey, coefficientPos, coefficientLen, true);
                D = Format.GetBytes(this.rawKey, privateExponentPos, privateExponentLen, true);
            }
            else
            {
                P = null;
                Q = null;
                DP = null;
                DQ = null;
                InverseQ = null;
                D = null;
            }
        }

        /// <summary>
        /// returns public byte arrays in xml format
        /// </summary>
        public string ToXmlString(bool privateKey)
        {
            MemoryStream ms = new();
            XmlTextWriter xtw = new(ms, null);
            xtw.WriteStartElement("RSAKeyValue");
            xtw.WriteElementString("Modulus", Format.GetB64(Modulus));
            xtw.WriteElementString("Exponent", Format.GetB64(Exponent));
            if (privateKey == true)
            {
                xtw.WriteElementString("P", Format.GetB64(P));
                xtw.WriteElementString("Q", Format.GetB64(Q));
                xtw.WriteElementString("DP", Format.GetB64(DP));
                xtw.WriteElementString("DQ", Format.GetB64(DQ));
                xtw.WriteElementString("InverseQ", Format.GetB64(InverseQ));
                xtw.WriteElementString("D", Format.GetB64(D));
            }
            xtw.WriteEndElement();
            xtw.Flush();
            return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// builds up public byte arrays, and rawKey from xml
        /// </summary>
        public void FromXmlString(string rsaKeyValue)
        {
            bool privateKey = false;
            StringReader sr = new(rsaKeyValue);
            XmlTextReader xtr = new(sr);
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            while (xtr.Read())
            {
                if (xtr.NodeType == XmlNodeType.Element)
                {
                    switch (xtr.LocalName)
                    {
                        case "Modulus":
                            Modulus = Format.GetB64(xtr.ReadString());
                            break;

                        case "Exponent":
                            Exponent = Format.GetB64(xtr.ReadString());
                            break;

                        case "P":
                            P = Format.GetB64(xtr.ReadString());
                            break;

                        case "Q":
                            Q = Format.GetB64(xtr.ReadString());
                            break;

                        case "DP":
                            DP = Format.GetB64(xtr.ReadString());
                            break;

                        case "DQ":
                            DQ = Format.GetB64(xtr.ReadString());
                            break;

                        case "InverseQ":
                            InverseQ = Format.GetB64(xtr.ReadString());
                            break;

                        case "D":
                            privateKey = true;
                            D = Format.GetB64(xtr.ReadString());
                            break;

                        default:
                            break;
                    }
                }
            }
            BuildRawKey(privateKey);
        }

        public void BuildRawKey(bool privateKey)
        {
            //build up rawKey byte[]
            uint rsaMagic = 0;
            int caSize = 0;
            uint bitLen = (uint)Modulus.Length * 8;

            if (privateKey == true)
            {
                kb = KeyBlob.PRIVATEKEYBLOB;
                caSize = 20 + 9 * ((int)bitLen / 16);
                rsaMagic = 0x32415352; //ASCII encoding of "RSA2"
            }
            else //public
            {
                kb = KeyBlob.PUBLICKEYBLOB;
                caSize = 20 + (int)bitLen / 8;
                rsaMagic = 0x31415352; //ASCII encoding of "RSA1"
            }

            rawKey = new byte[caSize];

            //PUBLICKEYSTRUC
            rawKey[0] = (byte)kb; //bType
            rawKey[1] = 2; //bVersion
                           //reserved 2,3
            c = Calg.RSA_KEYX;
            byte[] baKeyAlg = BitConverter.GetBytes((uint)c);//aiKeyAlg
            Buffer.BlockCopy(baKeyAlg, 0, rawKey, 4, 4);

            pks = new PUBLICKEYSTRUC();
            pks.bType = rawKey[0];
            pks.bVersion = rawKey[1];
            pks.reserved = BitConverter.ToUInt16(rawKey, 2);
            pks.aiKeyAlg = BitConverter.ToUInt32(rawKey, 4);

            //RSAPUBKEY
            byte[] baMagic = BitConverter.GetBytes(rsaMagic);//magic
            Buffer.BlockCopy(baMagic, 0, rawKey, 8, 4);
            byte[] baBitlen = BitConverter.GetBytes(bitLen);//bitlen
            Buffer.BlockCopy(baBitlen, 0, rawKey, 12, 4);

            SetSizeAndPosition(bitLen);
            Format.SetBytes(rawKey, exponentPos, exponentLen, Exponent, true); //pubexp

            rpk = new RSAPUBKEY();
            rpk.magic = BitConverter.ToUInt32(rawKey, 8);
            rpk.bitlen = BitConverter.ToUInt32(rawKey, 12);
            rpk.pubexp = BitConverter.ToUInt32(rawKey, 16);
            uint byteLen = rpk.bitlen / 8;

            //public
            Format.SetBytes(rawKey, modulusPos, modulusLen, Modulus, true);
            Format.SetBytes(rawKey, exponentPos, exponentLen, Exponent, true);
            //private
            if (privateKey == true)
            {
                Format.SetBytes(rawKey, prime1Pos, prime1Len, P, true);
                Format.SetBytes(rawKey, prime2Pos, prime2Len, Q, true);
                Format.SetBytes(rawKey, exponent1Pos, exponent1Len, DP, true);
                Format.SetBytes(rawKey, exponent2Pos, exponent2Len, DQ, true);
                Format.SetBytes(rawKey, coefficientPos, coefficientLen, InverseQ, true);
                Format.SetBytes(rawKey, privateExponentPos, privateExponentLen, D, true);
            }
            else
            {
                P = null;
                Q = null;
                DP = null;
                DQ = null;
                InverseQ = null;
                D = null;
            }
        }

        /// <summary>
        /// used to extract session keys in the clear
        /// </summary>
        //http://support.microsoft.com/default.aspx?scid=http://support.microsoft.com:80/support/kb/articles/Q228/7/86.ASP&NoWebContent=1
        public void ExponentOfOne()
        {
            Exponent = new byte[exponentLen];
            Exponent[0] = 1;
            Format.SetBytes(rawKey, exponentPos, exponentLen, Exponent, false);
            DP = new byte[exponent1Len];
            DP[0] = 1;
            Format.SetBytes(rawKey, exponent1Pos, exponent1Len, DP, false);
            DQ = new byte[exponent2Len];
            DQ[0] = 1;
            Format.SetBytes(rawKey, exponent2Pos, exponent2Len, DQ, false);
            D = new byte[privateExponentLen];
            D[0] = 1;
            Format.SetBytes(rawKey, privateExponentPos, privateExponentLen, D, false);
        }

        public byte[] rawKey;
        public PUBLICKEYSTRUC pks;
        public RSAPUBKEY rpk;
        public KeyBlob kb;
        public Calg c;

        //public
        public byte[] Modulus; //n

        public byte[] Exponent; //e

        //private
        public byte[] P;

        public byte[] Q;
        public byte[] DP;
        public byte[] DQ;
        public byte[] InverseQ;
        public byte[] D;

        private uint pksLen;
        private uint rpkLen;
        private uint exponentLen;
        private uint modulusLen;
        private uint prime1Len;
        private uint prime2Len;
        private uint exponent1Len;
        private uint exponent2Len;
        private uint coefficientLen;
        private uint privateExponentLen;

        private uint rpkPos;
        private uint exponentPos;
        private uint modulusPos;
        private uint prime1Pos;
        private uint prime2Pos;
        private uint exponent1Pos;
        private uint exponent2Pos;
        private uint coefficientPos;
        private uint privateExponentPos;

        private uint privByteLen;
        private uint pubByteLen;

        private void SetSizeAndPosition(uint bitLen)
        {
            //size =  8 12 128  64  64  64  64  64 128 = 596!
            pksLen = 8;
            rpkLen = 12;
            exponentLen = 4;
            modulusLen = bitLen / 8; //128
            prime1Len = bitLen / 16; //64
            prime2Len = bitLen / 16; //64
            exponent1Len = bitLen / 16; //64
            exponent2Len = bitLen / 16; //64
            coefficientLen = bitLen / 16; //64
            privateExponentLen = bitLen / 8; //128

            privByteLen = pksLen + rpkLen + modulusLen + prime1Len + prime2Len + exponent1Len + exponent2Len + coefficientLen + privateExponentLen; //596
            pubByteLen = pksLen + rpkLen + modulusLen; //148

            //1024 =  16  20 148 212 276 340 404 468
            //uint pksPos = 0;
            rpkPos = pksLen; //8
            exponentPos = rpkPos + 8; //16
            modulusPos = rpkPos + rpkLen; //20
            prime1Pos = modulusPos + modulusLen; //148
            prime2Pos = prime1Pos + prime1Len; //212
            exponent1Pos = prime2Pos + prime2Len; //276
            exponent2Pos = exponent1Pos + exponent1Len; //340
            coefficientPos = exponent2Pos + exponent2Len; //404
            privateExponentPos = coefficientPos + coefficientLen; //468
        }
    }
}