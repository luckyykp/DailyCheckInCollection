
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace LenovoSign
{
    /// <summary>  
    /// 类名：RSAFromPkcs8  
    /// 功能：RSA加密、解密、签名、验签  
    /// 详细：该类对Java生成的密钥进行解密和签名以及验签专用类，不需要修改  
    /// 版本：3.0  
    /// 日期：2013-07-08  
    /// 说明：  
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。  
    /// </summary>  
    public sealed class RSAHelper
    {
        /// <summary>
        /// 取得私钥和公钥 XML 格式,返回数组第一个是私钥,第二个是公钥.
        /// </summary>
        /// <param name="size">密钥长度,默认1024,可以为2048</param>
        /// <returns></returns>
        public static string[] CreateXmlKey(int size = 1024)
        {
            //密钥格式要生成pkcs#1格式的  而不是pkcs#8格式的
            RSACryptoServiceProvider sp = new RSACryptoServiceProvider(size);
            string privateKey = sp.ToXmlString(true);//private key
            string publicKey = sp.ToXmlString(false);//public  key
            return new string[] { privateKey, publicKey };
        }

        /// <summary>
        /// 取得私钥和公钥 CspBlob 格式,返回数组第一个是私钥,第二个是公钥.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string[] CreateCspBlobKey(int size = 1024)
        {
            //密钥格式要生成pkcs#1格式的  而不是pkcs#8格式的
            RSACryptoServiceProvider sp = new RSACryptoServiceProvider(size);
            string privateKey = System.Convert.ToBase64String(sp.ExportCspBlob(true));//private key
            string publicKey = System.Convert.ToBase64String(sp.ExportCspBlob(false));//public  key 

            return new string[] { privateKey, publicKey };
        }
        /// <summary>
        /// 导出PEM PKCS#1格式密钥对，返回数组第一个是私钥,第二个是公钥.
        /// </summary>
        public static string[] CreateKey_PEM_PKCS1(int size = 1024)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(size);
            string privateKey = RSA_PEM.ToPEM(rsa, false, false);
            string publicKey = RSA_PEM.ToPEM(rsa, true, false);
            return new string[] { privateKey, publicKey };
        }

        /// <summary>
        /// 导出PEM PKCS#8格式密钥对，返回数组第一个是私钥,第二个是公钥.
        /// </summary>
        public static string[] CreateKey_PEM_PKCS8(int size = 1024, bool convertToPublic = false)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(size);
            string privateKey = RSA_PEM.ToPEM(rsa, false, true);
            string publicKey = RSA_PEM.ToPEM(rsa, true, true);
            return new string[] { privateKey, publicKey };

        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="Data">原文</param>
        /// <param name="PublicKeyString">公钥</param>
        /// <param name="KeyType">密钥类型XML/PEM</param>
        /// <returns></returns>
        public static string RSAEncrypt(string Data, string PublicKeyString, string KeyType="PEM")
        {
            byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(Data);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            switch (KeyType)
            {
                case "XML":
                    rsa.FromXmlString(PublicKeyString);
                    break;
                case "PEM":
                    rsa = RSA_PEM.FromPEM(PublicKeyString);
                    break;
                default:
                    throw new Exception("不支持的密钥类型");
            }
            //加密块最大长度限制，如果加密数据的长度超过 秘钥长度/8-11，会引发长度不正确的异常，所以进行数据的分块加密
            int MaxBlockSize = rsa.KeySize / 8 - 11;
            //正常长度
            if (data.Length <= MaxBlockSize)
            {
                byte[] hashvalueEcy = rsa.Encrypt(data, false); //加密
                return System.Convert.ToBase64String(hashvalueEcy);
            }
            //长度超过正常值
            else
            {
                using (MemoryStream PlaiStream = new MemoryStream(data))
                using (MemoryStream CrypStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                    while (BlockSize > 0)
                    {
                        Byte[] ToEncrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                        Byte[] Cryptograph = rsa.Encrypt(ToEncrypt, false);
                        CrypStream.Write(Cryptograph, 0, Cryptograph.Length);
                        BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                    }
                    return System.Convert.ToBase64String(CrypStream.ToArray(), Base64FormattingOptions.None);
                }
            }
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="Data">密文</param>
        /// <param name="PrivateKeyString">私钥</param>
        /// <param name="KeyType">密钥类型XML/PEM</param>
        /// <returns></returns>
        public static string RSADecrypt(string Data, string PrivateKeyString, string KeyType="PEM")
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            switch (KeyType)
            {
                case "XML":
                    rsa.FromXmlString(PrivateKeyString);
                    break;
                case "PEM":
                    rsa = RSA_PEM.FromPEM(PrivateKeyString);
                    break;
                default:
                    throw new Exception("不支持的密钥类型");
            }
            int MaxBlockSize = rsa.KeySize / 8;    //解密块最大长度限制
            //正常解密
            if (Data.Length <= MaxBlockSize)
            {
                byte[] hashvalueDcy = rsa.Decrypt(System.Convert.FromBase64String(Data), false);//解密
                return Encoding.GetEncoding("UTF-8").GetString(hashvalueDcy);
            }
            //分段解密
            else
            {
                using (MemoryStream CrypStream = new MemoryStream(System.Convert.FromBase64String(Data)))
                using (MemoryStream PlaiStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);

                    while (BlockSize > 0)
                    {
                        Byte[] ToDecrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToDecrypt, 0, BlockSize);

                        Byte[] Plaintext = rsa.Decrypt(ToDecrypt, false);
                        PlaiStream.Write(Plaintext, 0, Plaintext.Length);
                        BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                    }
                    string output = Encoding.GetEncoding("UTF-8").GetString(PlaiStream.ToArray());
                    return output;
                }
            }
        }
    }
    public class RSA_Unit
    {
        static public string Base64EncodeBytes(byte[] byts)
        {
            return System.Convert.ToBase64String(byts);
        }
        static public byte[] Base64DecodeBytes(string str)
        {
            try
            {
                return System.Convert.FromBase64String(str);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 把字符串按每行多少个字断行
        /// </summary>
        static public string TextBreak(string text, int line)
        {
            var idx = 0;
            var len = text.Length;
            var str = new StringBuilder();
            while (idx < len)
            {
                if (idx > 0)
                {
                    str.Append('\n');
                }
                if (idx + line >= len)
                {
                    str.Append(text.Substring(idx));
                }
                else
                {
                    str.Append(text.Substring(idx, line));
                }
                idx += line;
            }
            return str.ToString();
        }

    }
    static public class Extensions
    {
        /// <summary>
        /// 从数组start开始到指定长度复制一份
        /// </summary>
        static public T[] sub<T>(this T[] arr, int start, int count)
        {
            T[] val = new T[count];
            for (var i = 0; i < count; i++)
            {
                val[i] = arr[start + i];
            }
            return val;
        }
        static public void writeAll(this Stream stream, byte[] byts)
        {
            stream.Write(byts, 0, byts.Length);
        }
    }

     public class RSA_PEM
    {
        public static RSACryptoServiceProvider FromPEM(string pem)
        {
            var rsaParams = new CspParameters();
            rsaParams.Flags = CspProviderFlags.UseMachineKeyStore;
            var rsa = new RSACryptoServiceProvider(rsaParams);

            var param = new RSAParameters();

            var base64 = _PEMCode.Replace(pem, "");
            var data = RSA_Unit.Base64DecodeBytes(base64);
            if (data == null)
            {
                throw new Exception("PEM内容无效");
            }
            var idx = 0;

            //读取长度
            Func<byte, int> readLen = (first) =>
            {
                if (data[idx] == first)
                {
                    idx++;
                    if (data[idx] == 0x81)
                    {
                        idx++;
                        return data[idx++];
                    }
                    else if (data[idx] == 0x82)
                    {
                        idx++;
                        return (((int)data[idx++]) << 8) + data[idx++];
                    }
                    else if (data[idx] < 0x80)
                    {
                        return data[idx++];
                    }
                }
                throw new Exception("PEM未能提取到数据");
            };
            //读取块数据
            Func<byte[]> readBlock = () =>
            {
                var len = readLen(0x02);
                if (data[idx] == 0x00)
                {
                    idx++;
                    len--;
                }
                var val = data.sub(idx, len);
                idx += len;
                return val;
            };
            //比较data从idx位置开始是否是byts内容
            Func<byte[], bool> eq = (byts) =>
            {
                for (var i = 0; i < byts.Length; i++, idx++)
                {
                    if (idx >= data.Length)
                    {
                        return false;
                    }
                    if (byts[i] != data[idx])
                    {
                        return false;
                    }
                }
                return true;
            };




            if (pem.Contains("PUBLIC KEY"))
            {
                /****使用公钥****/
                //读取数据总长度
                readLen(0x30);
                if (!eq(_SeqOID))
                {
                    throw new Exception("PEM未知格式");
                }
                //读取1长度
                readLen(0x03);
                idx++;//跳过0x00
                      //读取2长度
                readLen(0x30);

                //Modulus
                param.Modulus = readBlock();

                //Exponent
                param.Exponent = readBlock();
            }
            else if (pem.Contains("PRIVATE KEY"))
            {
                /****使用私钥****/
                //读取数据总长度
                readLen(0x30);

                //读取版本号
                if (!eq(_Ver))
                {
                    throw new Exception("PEM未知版本");
                }

                //检测PKCS8
                var idx2 = idx;
                if (eq(_SeqOID))
                {
                    //读取1长度
                    readLen(0x04);
                    //读取2长度
                    readLen(0x30);

                    //读取版本号
                    if (!eq(_Ver))
                    {
                        throw new Exception("PEM版本无效");
                    }
                }
                else
                {
                    idx = idx2;
                }

                //读取数据
                param.Modulus = readBlock();
                param.Exponent = readBlock();
                param.D = readBlock();
                param.P = readBlock();
                param.Q = readBlock();
                param.DP = readBlock();
                param.DQ = readBlock();
                param.InverseQ = readBlock();
            }
            else
            {
                throw new Exception("pem需要BEGIN END标头");
            }

            rsa.ImportParameters(param);
            return rsa;
        }
        static private Regex _PEMCode = new Regex(@"--+.+?--+|\s+");
        static private byte[] _SeqOID = new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        static private byte[] _Ver = new byte[] { 0x02, 0x01, 0x00 };


        /// <summary>
        /// 将RSA中的密钥对转换成PEM格式，usePKCS8=false时返回PKCS#1格式，否则返回PKCS#8格式，如果convertToPublic含私钥的RSA将只返回公钥，仅含公钥的RSA不受影响
        /// </summary>
        public static string ToPEM(RSACryptoServiceProvider rsa, bool convertToPublic, bool usePKCS8)
        {
            //https://www.jianshu.com/p/25803dd9527d
            //https://www.cnblogs.com/ylz8401/p/8443819.html
            //https://blog.csdn.net/jiayanhui2877/article/details/47187077
            //https://blog.csdn.net/xuanshao_/article/details/51679824
            //https://blog.csdn.net/xuanshao_/article/details/51672547

            var ms = new MemoryStream();
            //写入一个长度字节码
            Action<int> writeLenByte = (len) =>
            {
                if (len < 0x80)
                {
                    ms.WriteByte((byte)len);
                }
                else if (len <= 0xff)
                {
                    ms.WriteByte(0x81);
                    ms.WriteByte((byte)len);
                }
                else
                {
                    ms.WriteByte(0x82);
                    ms.WriteByte((byte)(len >> 8 & 0xff));
                    ms.WriteByte((byte)(len & 0xff));
                }
            };
            //写入一块数据
            Action<byte[]> writeBlock = (byts) =>
            {
                var addZero = (byts[0] >> 4) >= 0x8;
                ms.WriteByte(0x02);
                var len = byts.Length + (addZero ? 1 : 0);
                writeLenByte(len);

                if (addZero)
                {
                    ms.WriteByte(0x00);
                }
                ms.Write(byts, 0, byts.Length);
            };
            //根据后续内容长度写入长度数据
            Func<int, byte[], byte[]> writeLen = (index, byts) =>
            {
                var len = byts.Length - index;

                ms.SetLength(0);
                ms.Write(byts, 0, index);
                writeLenByte(len);
                ms.Write(byts, index, len);

                return ms.ToArray();
            };


            if (rsa.PublicOnly || convertToPublic)
            {
                /****生成公钥****/
                var param = rsa.ExportParameters(false);


                //写入总字节数，不含本段长度，额外需要24字节的头，后续计算好填入
                ms.WriteByte(0x30);
                var index1 = (int)ms.Length;

                //固定内容
                // encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
                ms.writeAll(_SeqOID);

                //从0x00开始的后续长度
                ms.WriteByte(0x03);
                var index2 = (int)ms.Length;
                ms.WriteByte(0x00);

                //后续内容长度
                ms.WriteByte(0x30);
                var index3 = (int)ms.Length;

                //写入Modulus
                writeBlock(param.Modulus);

                //写入Exponent
                writeBlock(param.Exponent);


                //计算空缺的长度
                var byts = ms.ToArray();

                byts = writeLen(index3, byts);
                byts = writeLen(index2, byts);
                byts = writeLen(index1, byts);


                return "-----BEGIN PUBLIC KEY-----\n" + RSA_Unit.TextBreak(RSA_Unit.Base64EncodeBytes(byts), 64) + "\n-----END PUBLIC KEY-----";
            }
            else
            {
                /****生成私钥****/
                var param = rsa.ExportParameters(true);

                //写入总字节数，后续写入
                ms.WriteByte(0x30);
                int index1 = (int)ms.Length;

                //写入版本号
                ms.writeAll(_Ver);

                //PKCS8 多一段数据
                int index2 = -1, index3 = -1;
                if (usePKCS8)
                {
                    //固定内容
                    ms.writeAll(_SeqOID);

                    //后续内容长度
                    ms.WriteByte(0x04);
                    index2 = (int)ms.Length;

                    //后续内容长度
                    ms.WriteByte(0x30);
                    index3 = (int)ms.Length;

                    //写入版本号
                    ms.writeAll(_Ver);
                }

                //写入数据
                writeBlock(param.Modulus);
                writeBlock(param.Exponent);
                writeBlock(param.D);
                writeBlock(param.P);
                writeBlock(param.Q);
                writeBlock(param.DP);
                writeBlock(param.DQ);
                writeBlock(param.InverseQ);


                //计算空缺的长度
                var byts = ms.ToArray();

                if (index2 != -1)
                {
                    byts = writeLen(index3, byts);
                    byts = writeLen(index2, byts);
                }
                byts = writeLen(index1, byts);


                var flag = " PRIVATE KEY";
                if (!usePKCS8)
                {
                    flag = " RSA" + flag;
                }
                return "-----BEGIN" + flag + "-----\n" + RSA_Unit.TextBreak(RSA_Unit.Base64EncodeBytes(byts), 64) + "\n-----END" + flag + "-----";
            }
        }
    }
}