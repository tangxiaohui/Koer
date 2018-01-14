using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;

public class MD5Utils {
    private const int m_maxBufCount = 4;
    static private int m_nBufferCount = 2 * 1024;
    static private int m_curBufIndex = 0;
    static private byte[][] m_buffer = new byte[m_maxBufCount][] {
        new byte[m_nBufferCount],
        new byte[m_nBufferCount],
        new byte[m_nBufferCount],
        new byte[m_nBufferCount]
    };  // 2kb

    static public string GetMd5Hash(MD5 md5Hash, string input) {

        // Convert the input string to a byte array and compute the hash. 
        byte[] data = md5Hash.ComputeHash( Encoding.UTF8.GetBytes( input ) );
        // Create a new Stringbuilder to collect the bytes and create a string.
        StringBuilder sBuilder = new StringBuilder();
        // Loop through each byte of the hashed data  
        // and format each one as a hexadecimal string. 
        for( int i = 0; i < data.Length; i++ ) {
            sBuilder.Append( data[i].ToString( "x2" ) );
        }

        // Return the hexadecimal string. 
        return sBuilder.ToString();
    }

    public static string GetMd5HashFile(MD5 md5Hash, string file, byte[] buf = null) {
        if( string.IsNullOrEmpty( file ) || md5Hash == null ) {
            return null;
        }
        string result = "";
        try {
            if( buf == null ) {
                var curIndex = m_curBufIndex % m_maxBufCount;
                m_curBufIndex++;
                buf = m_buffer[curIndex];
            }
            using( FileStream stream = File.Open( file, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
                int index = 0;
                int length = (int)stream.Length;
                byte[] data = null;
                try {
                    while( length > index ) {
                        var count = stream.Read( buf, 0, m_nBufferCount );
                        index += count;
                        if( count == m_nBufferCount ) {
                            md5Hash.TransformBlock( buf, 0, count, buf, 0 );
                        }

                        if( index >= length || count != m_nBufferCount ) {
                            md5Hash.TransformFinalBlock( buf, 0, count );
                            data = md5Hash.Hash;
                        }
                        Common.BlockPool.MemSet( buf, 0 );
                    }
                } catch( Exception e ) {
                    Console.WriteLine( "GetMd5HashFile " + stream.Name + " : " + e );
                }
                finally {
                    stream.Close();
                    stream.Dispose();

                    // Convert the input string to a byte array and compute the hash. 
                    if( data != null ) {
                        StringBuilder sBuilder = new StringBuilder();

                        // Loop through each byte of the hashed data  
                        // and format each one as a hexadecimal string. 
                        for( int i = 0; i < data.Length; i++ ) {
                            sBuilder.Append( data[i].ToString( "x2" ) );
                        }

                        // Return the hexadecimal string. 
                        result = sBuilder.ToString();
                    }
                }
            }
        } catch( Exception e ) {
            Common.ULogFile.sharedInstance.LogError( file );
            Common.ULogFile.sharedInstance.LogError( e );
        }
        return result;
    }

    // Verify a hash against a string. 
    public static bool VerifyMd5Hash(string input, string hash, MD5 md5Hash) {
        // Hash the input. 
        string hashOfInput = GetMd5Hash( md5Hash, input );

        // Create a StringComparer an compare the hashes.
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        if( 0 == comparer.Compare( hashOfInput, hash ) ) {
            return true;
        } else {
            return false;
        }
    }

    // Verify a hash against a string. 
    public static bool VerifyMd5HashFile(string file, string hash, MD5 md5Hash) {
        // Hash the input. 
        string hashOfInput = GetMd5HashFile( md5Hash, file );

        // Create a StringComparer an compare the hashes.
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        if( 0 == comparer.Compare( hashOfInput, hash ) ) {
            return true;
        } else {
            return false;
        }
    }
}
