using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Common {

    public unsafe class FastBinaryWriter : BinaryWriter {

        byte[] m_buffer = new byte[32];

        public FastBinaryWriter( Stream input ) : base( input ) { }
        public FastBinaryWriter( Stream input, Encoding encoding ) : base( input, encoding ) { }

        public override void Write( double value ) {
            fixed ( byte* p = m_buffer ) {
                *(double*)p = value;
            }
            base.Write( m_buffer, 0, sizeof( double ) );
        }

        public override void Write( float value ) {
            fixed ( byte* p = m_buffer ) {
                *(float*)p = value;
            }
            base.Write( m_buffer, 0, sizeof( float ) );
        }

        public override void Write( int value ) {
            fixed ( byte* p = m_buffer ) {
                *(int*)p = value;
            }
            base.Write( m_buffer, 0, sizeof( int ) );
        }

        public override void Write( long value ) {
            fixed ( byte* p = m_buffer ) {
                *(long*)p = value;
            }
            base.Write( m_buffer, 0, sizeof( long ) );
        }

        public override void Write( short value ) {
            fixed ( byte* p = m_buffer ) {
                *(short*)p = value;
            }
            base.Write( m_buffer, 0, sizeof( short ) );
        }

        public override void Write( uint value ) {
            fixed ( byte* p = m_buffer ) {
                *(uint*)p = value;
            }
            base.Write( m_buffer, 0, sizeof( uint ) );
        }

        public override void Write( ulong value ) {
            fixed ( byte* p = m_buffer ) {
                *(ulong*)p = value;
            }
            base.Write( m_buffer, 0, sizeof( ulong ) );
        }

        public override void Write( ushort value ) {
            fixed ( byte* p = m_buffer ) {
                *(ushort*)p = value;
            }
            base.Write( m_buffer, 0, sizeof( ushort ) );
        }
    }

    public unsafe class FastestBinaryWriter : IDisposable {

        GCHandle m_gcHandle;
        byte[] m_buff = null;
        bool m_pinned = false;
        byte* m_current = default( byte* );
        byte* m_head = default( byte* );
        _BaseStream m_baseStream = null;

        public class _BaseStream {
            internal FastestBinaryWriter _this;
            public long Position {
                get {
                    return _this.m_current - _this.m_head;
                }
                set {
                    _this.m_current = _this.m_head + value;
                }
            }
            public long Seek( long offset, SeekOrigin opt ) {
                switch ( opt ) {
                    case SeekOrigin.Begin:
                        _this.m_current = _this.m_head + offset;
                        break;
                    case SeekOrigin.Current:
                        _this.m_current = _this.m_current + offset;
                        break;
                    case SeekOrigin.End:
                        _this.m_current = _this.m_head + _this.m_buff.Length + offset;
                        break;
                }
                return (long)_this.m_current;
            }
        }

        public FastestBinaryWriter( byte[] buff ) {
            m_buff = buff;
            if ( m_buff != null ) {
                m_gcHandle = GCHandle.Alloc( m_buff, GCHandleType.Pinned );
                m_head = (byte*)m_gcHandle.AddrOfPinnedObject();
                m_current = m_head;
                m_pinned = true;
            }
            m_baseStream = new _BaseStream() { _this = this };
        }

        public FastestBinaryWriter( byte[] buff, byte* _buff ) {
            m_buff = buff;
            m_head = _buff;
            m_current = m_head;
            m_pinned = false;
            m_baseStream = new _BaseStream() { _this = this };
        }

        public _BaseStream BaseStream {
            get {
                return m_baseStream;
            }
        }

        public byte* Current {
            get {
                return m_current;
            }
        }

        public byte* Forward( int size ) {
            var p = m_current;
            m_current += size;
            return p;
        }

        public byte[] GetBuffer() {
            return m_buff;
        }

        public int GetOffset() {
            return (int)( m_current - m_head );
        }

        public long GetOffsetL() {
            return m_current - m_head;
        }

        ~FastestBinaryWriter() {
            Dispose( false );
        }

        public void Close() {
            Dispose();
        }

        public void Dispose() {
            if ( m_buff != null ) {
                Dispose( true );
                GC.SuppressFinalize( this );
            }
        }

        private void Dispose( bool disposing ) {
            if ( m_buff == null ) {
                return;
            }
            if ( disposing ) {
                m_buff = null;
                m_baseStream._this = null;
            }
            if ( m_pinned ) {
                m_pinned = false;
                m_gcHandle.Free();
            }
        }

        public long Seek( long offset, SeekOrigin opt ) {
            switch ( opt ) {
                case SeekOrigin.Begin:
                    m_current = m_head + offset;
                    break;
                case SeekOrigin.Current:
                    m_current = m_current + offset;
                    break;
                case SeekOrigin.End:
                    m_current = m_head + m_buff.Length + offset;
                    break;
            }
            return (long)m_current;
        }

        public void Write( byte* bytes, int size ) {
            m_current += size;
        }

        public void Write( byte[] bytes ) {
            Marshal.Copy( bytes, 0, (IntPtr)m_current, bytes.Length );
            m_current += bytes.Length;
        }

        public void Write( byte[] bytes, int startIndex, int length ) {
            Marshal.Copy( bytes, startIndex, (IntPtr)m_current, length );
            m_current += length;
        }

        public void Write( bool value ) {
            Marshal.WriteByte( (IntPtr)m_current, (Byte)( value ? 1 : 0 ) );
            m_current += 1;
        }

        public void Write( char value ) {
            Marshal.WriteInt16( (IntPtr)m_current, (Int16)( value ) );
            m_current += 2;
        }

        public void Write( byte value ) {
            Marshal.WriteByte( (IntPtr)m_current, (Byte)value );
            ++m_current;
        }

        public void Write( sbyte value ) {
            Marshal.WriteByte( (IntPtr)m_current, (Byte)value );
            ++m_current;
        }

        public void Write( double value ) {
            long* p = (long*)&value;
            Marshal.WriteInt64( (IntPtr)m_current, (Int64)( *p ) );
            m_current += 8;
        }

        public void Write( float value ) {
            int* p = (int*)&value;
            Marshal.WriteInt32( (IntPtr)m_current, (Int32)( *p ) );
            m_current += 4;
        }

        public void Write( int value ) {
            Marshal.WriteInt32( (IntPtr)m_current, (Int32)value );
            m_current += 4;
        }

        public void Write( long value ) {
            Marshal.WriteInt64( (IntPtr)m_current, (Int64)value );
            m_current += 8;
        }

        public void Write( short value ) {
            Marshal.WriteInt16( (IntPtr)m_current, (Int16)value );
            m_current += 2;
        }

        public void Write( uint value ) {
            Marshal.WriteInt32( (IntPtr)m_current, (Int32)value );
            m_current += 4;
        }

        public void Write( ulong value ) {
            Marshal.WriteInt64( (IntPtr)m_current, (Int64)value );
            m_current += 8;
        }

        public void Write( ushort value ) {
            Marshal.WriteInt16( (IntPtr)m_current, (Int16)value );
            m_current += 2;
        }
    }
}
