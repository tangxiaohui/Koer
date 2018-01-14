using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{

using BinaryReader = Common.FastestBinaryReader;
using BinaryWriter = Common.FastestBinaryWriter;

public partial class BinarySerializer
{
    static byte[] ___string_temp_buffer = new byte[512];
    static byte[] ___binary_temp_buffer = new byte[128];
    public static byte[] binBuff {
        get {
            return ___binary_temp_buffer;
        }
    }
    static public Boolean Read_Boolean( BinaryReader reader ) {
        return reader.ReadBoolean();
    }
    static public SByte Read_SByte( BinaryReader reader ) {
        return reader.ReadSByte();
    }
    static public Byte Read_Byte( BinaryReader reader ) {
        return reader.ReadByte();
    }
    static public Byte[] Read_Bytes( BinaryReader reader ) {
        Int16 len = reader.ReadInt16();
        return reader.ReadBytes( (int)len );
    }
    static public Byte[] Read_BytesFixSize( BinaryReader reader, UInt16 length ) {
        if ( length > 0 ) {
            return reader.ReadBytes( (int)length );
        } else {
            return null;
        }
    }
    static public Double Read_Double( BinaryReader reader ) {
        return reader.ReadDouble();
    }
    static public Single Read_Single( BinaryReader reader ) {
        return reader.ReadSingle();
    }
    static public Int16 Read_Int16( BinaryReader reader ) {
        return reader.ReadInt16();
    }
    static public Int32 Read_Int32( BinaryReader reader ) {
        return reader.ReadInt32();
    }
    static public Int64 Read_Int64( BinaryReader reader ) {
        return reader.ReadInt64();
    }
    static public UInt16 Read_UInt16( BinaryReader reader ) {
        return reader.ReadUInt16();
    }
    static public UInt32 Read_UInt32( BinaryReader reader ) {
        return reader.ReadUInt32();
    }
    static public UInt64 Read_UInt64( BinaryReader reader ) {
        return reader.ReadUInt64();
    }
    static public String Read_String( BinaryReader reader ) {
        Int16 len = reader.ReadInt16();
        if ( len > 0 ) {
            Byte[] s = null;
            if ( len > ___string_temp_buffer.Length ) {
                s = new Byte[len];
            } else {
                s = ___string_temp_buffer;
            }
            reader.Read( s, 0, len );
            return System.Text.Encoding.UTF8.GetString( s, 0, len );
        } else {
            return String.Empty;
        }
    }
    static public Byte[] Read_RawString( BinaryReader reader, bool cstyle = false ) {
        Int16 len = reader.ReadInt16();
        Byte[] ret = null;
        if ( cstyle ) {
            ret = new Byte[len + 1];
            ret[len] = 0;
        } else {
            if ( len > 0 ) {
                ret = new Byte[len];
            } else {
                return null;
            }
        }
        if ( len > 0 ) {
            reader.Read( ret, 0, (int)len );
        }
        return ret;
    }
    static public String Read_StringFixSize( BinaryReader reader, UInt16 length ) {
        if ( length > 0 ) {
            Byte[] s = null;
            if ( length <= ___string_temp_buffer.Length ) {
                s = ___string_temp_buffer;
                reader.Read( s, 0, length );
            } else {
                s = reader.ReadBytes( (int)length );
            }
            return System.Text.Encoding.UTF8.GetString( s, 0, length );
        } else {
            return String.Empty;
        }
    }
    static public List<Boolean> ReadList_Boolean( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<Boolean>( count );
        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadBoolean() );
        }
        return ret;
    }
    static public List<SByte> ReadList_SByte( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<SByte>( count );
        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadSByte() );
        }
        return ret;
    }
    static public List<Byte> ReadList_Byte( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<Byte>( count );
        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadByte() );
        }
        return ret;
    }
    static public List<Byte[]> ReadList_Bytes( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<Byte[]>( count );
        for ( int i = 0; i < count; ++i ) {
            Int16 len = reader.ReadInt16();
            ret.Add( reader.ReadBytes( len ) );
        }
        return ret;
    }
    static public List<Double> ReadList_Double( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<Double>();
        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadDouble() );
        }
        return ret;
    }
    static public List<Single> ReadList_Single( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<Single>( count );
        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadSingle() );
        }
        return ret;
    }
    static public List<Int16> ReadList_Int16( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<Int16>();
        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadInt16() );
        }
        return ret;
    }
    static public List<Int32> ReadList_Int32( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<Int32>( count );
        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadInt32() );
        }
        return ret;
    }
    static public List<Int64> ReadList_Int64( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<Int64>( count );
        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadInt64() );
        }
        return ret;
    }
    static public List<UInt16> ReadList_UInt16( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<UInt16>( count );
        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadUInt16() );
        }
        return ret;
    }
    static public List<UInt32> ReadList_UInt32( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<UInt32>( count );

        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadUInt32() );
        }
        return ret;
    }
    static public List<UInt64> ReadList_UInt64( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<UInt64>( count );
        for ( int i = 0; i < count; ++i ) {
            ret.Add( reader.ReadUInt64() );
        }
        return ret;
    }
    static public List<String> ReadList_String( BinaryReader reader ) {
        Int16 count = reader.ReadInt16();
        if ( count <= 0 ) {
            return null;
        }
        var ret = new List<String>( count );
        for ( int i = 0; i < count; ++i ) {
            ret.Add( Read_String( reader ) );
        }
        return ret;
    }
    static public void Write_Boolean( BinaryWriter writer, Boolean value ) {
        writer.Write( value );
    }
    static public void Write_SByte( BinaryWriter writer, SByte value ) {
        writer.Write( value );
    }
    static public void Write_Byte( BinaryWriter writer, Byte value ) {
        writer.Write( value );
    }
    static public void Write_Bytes( BinaryWriter writer, Byte[] value ) {
        writer.Write( (Int16)value.Length );
        writer.Write( value );
    }
    static public void Write_BytesFixSize( BinaryWriter writer, Byte[] value, UInt16 length = UInt16.MaxValue ) {
        if ( length > 0 ) {
            if ( length == UInt16.MaxValue ) {
                length = (UInt16)value.Length;
            }
            writer.Write( value, 0, Math.Min( length, value.Length ) );
            for ( int i = value.Length; i < length; ++i ) {
                writer.Write( (Byte)0 );
            }
        }
    }
    static public void Write_Double( BinaryWriter writer, Double value ) {
        writer.Write( value );
    }
    static public void Write_Single( BinaryWriter writer, Single value ) {
        writer.Write( value );
    }
    static public void Write_Int16( BinaryWriter writer, Int16 value ) {
        writer.Write( value );
    }
    static public void Write_Int32( BinaryWriter writer, Int32 value ) {
        writer.Write( value );
    }
    static public void Write_Int64( BinaryWriter writer, Int64 value ) {
        writer.Write( value );
    }
    static public void Write_UInt16( BinaryWriter writer, UInt16 value ) {
        writer.Write( value );
    }
    static public void Write_UInt32( BinaryWriter writer, UInt32 value ) {
        writer.Write( value );
    }
    static public void Write_UInt64( BinaryWriter writer, UInt64 value ) {
        writer.Write( value );
    }
    static public void Write_String( BinaryWriter writer, String value ) {
        if ( value.Length > 0 ) {
            Byte[] bytes = null;
            int length = 0;
            if ( value.Length * 6 <= ___string_temp_buffer.Length ) {
                length = System.Text.Encoding.UTF8.GetBytes( value, 0, value.Length, ___string_temp_buffer, 0 );
                bytes = ___string_temp_buffer;
            } else {
                bytes = System.Text.Encoding.UTF8.GetBytes( value );
                length = bytes.Length;
            }
            writer.Write( (UInt16)length );
            writer.Write( bytes, 0, length );
        } else {
            writer.Write( (UInt16)0 );
        }
    }
    static public void Write_StringFixSize( BinaryWriter writer, String value, UInt16 length = UInt16.MaxValue ) {
        if ( length > 0 ) {
            Byte[] bytes = null;
            int _length = 0;
            if ( value.Length * 6 <= ___string_temp_buffer.Length ) {
                _length = System.Text.Encoding.UTF8.GetBytes( value, 0, value.Length, ___string_temp_buffer, 0 );
                bytes = ___string_temp_buffer;
            } else {
                bytes = System.Text.Encoding.UTF8.GetBytes( value );
                _length = bytes.Length;
            }
            if ( length == UInt16.MaxValue ) {
                length = (UInt16)bytes.Length;
            }
            var _realLength = Math.Min( length, _length );
            writer.Write( bytes, 0, _realLength );
            for ( int i = _realLength; i < length; ++i ) {
                writer.Write( (Byte)0 );
            }
        }
    }
    static public void WriteList_Boolean( BinaryWriter writer, List<Boolean> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_Boolean( writer, value[i] );
        }
    }
    static public void WriteList_SByte( BinaryWriter writer, List<SByte> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_SByte( writer, value[i] );
        }
    }
    static public void WriteList_Byte( BinaryWriter writer, List<Byte> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_Byte( writer, value[i] );
        }
    }
    static public void WriteList_Bytes( BinaryWriter writer, List<Byte[]> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_Bytes( writer, value[i] );
        }
    }
    static public void WriteList_Double( BinaryWriter writer, List<Double> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_Double( writer, value[i] );
        }
    }
    static public void WriteList_Single( BinaryWriter writer, List<Single> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_Single( writer, value[i] );
        }
    }
    static public void WriteList_Int16( BinaryWriter writer, List<Int16> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_Int16( writer, value[i] );
        }
    }
    static public void WriteList_Int32( BinaryWriter writer, List<Int32> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_Int32( writer, value[i] );
        }
    }
    static public void WriteList_Int64( BinaryWriter writer, List<Int64> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_Int64( writer, value[i] );
        }
    }
    static public void WriteList_UInt16( BinaryWriter writer, List<UInt16> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_UInt16( writer, value[i] );
        }
    }
    static public void WriteList_UInt32( BinaryWriter writer, List<UInt32> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_UInt32( writer, value[i] );
        }
    }
    static public void WriteList_UInt64( BinaryWriter writer, List<UInt64> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_UInt64( writer, value[i] );
        }
    }
    static public void WriteList_String( BinaryWriter writer, List<String> value ) {
        if ( value == null || value.Count == 0 ) {
            Write_Int16( writer, 0 );
            return;
        }
        Write_Int16( writer, (short)value.Count );
        for ( int i = 0; i < value.Count; ++i ) {
            Write_String( writer, value[i] );
        }
    }
    static unsafe public Vector4 Read_Vector4( BinaryReader reader ) {
        reader.Read( ___binary_temp_buffer, 0, 16 );
        Vector4 ret;
        fixed ( byte* p = ___binary_temp_buffer ) {
            var _p = (Vector4*)p;
            ret = *_p;
        }
        return ret;
    }
    static unsafe public Vector3 Read_Vector3( BinaryReader reader ) {
        reader.Read( ___binary_temp_buffer, 0, 12 );
        Vector3 ret;
        fixed ( byte* p = ___binary_temp_buffer ) {
            var _p = (Vector3*)p;
            ret = *_p;
        }
        return ret;
    }
    static unsafe public Quaternion Read_Quaternion( BinaryReader reader ) {
        reader.Read( ___binary_temp_buffer, 0, 16 );
        Quaternion ret;
        fixed ( byte* p = ___binary_temp_buffer ) {
            var _p = (Quaternion*)p;
            ret = *_p;
        }
        return ret;
    }
    static unsafe public Vector2 Read_Vector2( BinaryReader reader ) {
        reader.Read( ___binary_temp_buffer, 0, 8 );
        Vector2 ret;
        fixed ( byte* p = ___binary_temp_buffer ) {
            var _p = (Vector2*)p;
            ret = *_p;
        }
        return ret;
    }
    static unsafe public void Write_Quaternion( BinaryWriter writer, Quaternion value ) {
        fixed ( byte* p = ___binary_temp_buffer ) {
            var _p = (Quaternion*)p;
            *_p = value;
        }
        writer.Write( ___binary_temp_buffer, 0, 16 );
    }
    static unsafe public void Write_Vector4( BinaryWriter writer, Vector4 value ) {
        fixed ( byte* p = ___binary_temp_buffer ) {
            var _p = (Vector4*)p;
            *_p = value;
        }
        writer.Write( ___binary_temp_buffer, 0, 16 );
    }
    static unsafe public void Write_Vector3( BinaryWriter writer, Vector3 value ) {
        fixed ( byte* p = ___binary_temp_buffer ) {
            var _p = (Vector3*)p;
            *_p = value;
        }
        writer.Write( ___binary_temp_buffer, 0, 12 );
    }
    static unsafe public void Write_Vector2( BinaryWriter writer, Vector2 value ) {
        fixed ( byte* p = ___binary_temp_buffer ) {
            var _p = (Vector2*)p;
            *_p = value;
        }
        writer.Write( ___binary_temp_buffer, 0, 8 );
    }
}

}
