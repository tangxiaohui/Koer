using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
#pragma warning disable 0168
namespace Common {

    public unsafe class FastBinaryReader : BinaryReader {

        byte[] m_buffer = new byte[32];

        public FastBinaryReader( Stream input ) : base( input ) { }
        public FastBinaryReader( Stream input, Encoding encoding ) : base( input, encoding ) { }

        public override double ReadDouble() {
            base.Read( m_buffer, 0, 8 );
            fixed ( byte* p = m_buffer ) {
                return *( ( (double*)p ) );
            }
        }
        public override short ReadInt16() {
            base.Read( m_buffer, 0, 2 );
            fixed ( byte* p = m_buffer ) {
                return *( ( (short*)p ) );
            }
        }
        public override int ReadInt32() {
            base.Read( m_buffer, 0, 4 );
            fixed ( byte* p = m_buffer ) {
                return *( ( (int*)p ) );
            }
        }
        public override long ReadInt64() {
            base.Read( m_buffer, 0, 8 );
            fixed ( byte* p = m_buffer ) {
                return *( ( (long*)p ) );
            }
        }
        public override float ReadSingle() {
            base.Read( m_buffer, 0, 4 );
            fixed ( byte* p = m_buffer ) {
                return *( ( (float*)p ) );
            }
        }
        public override ushort ReadUInt16() {
            base.Read( m_buffer, 0, 2 );
            fixed ( byte* p = m_buffer ) {
                return *( ( (ushort*)p ) );
            }
        }
        public override uint ReadUInt32() {
            base.Read( m_buffer, 0, 4 );
            fixed ( byte* p = m_buffer ) {
                return *( ( (uint*)p ) );
            }
        }
        public override ulong ReadUInt64() {
            base.Read( m_buffer, 0, 8 );
            fixed ( byte* p = m_buffer ) {
                return *( ( (ulong*)p ) );
            }
        }
    }

    public unsafe class FastestBinaryReader : IDisposable {

        GCHandle m_gcHandle;
        bool m_pinned = false;
        byte[] m_buff = null;
        byte* m_current = default( byte* );
        byte* m_head = default( byte* );
        _BaseStream m_baseStream = null;

        public class _BaseStream {
            internal FastestBinaryReader _this;
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

        public FastestBinaryReader( byte[] buff ) {
            m_buff = buff;
            if ( m_buff != null ) {
                m_gcHandle = GCHandle.Alloc( m_buff, GCHandleType.Pinned );
                m_head = (byte*)m_gcHandle.AddrOfPinnedObject();
                m_current = m_head;
                m_pinned = true;
            }
            m_baseStream = new _BaseStream() { _this = this };
        }

        public FastestBinaryReader( byte[] buff, byte* _buff ) {
            m_buff = buff;
            m_pinned = false;
            m_head = _buff;
            m_current = m_head;
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

        ~FastestBinaryReader() {
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
                m_gcHandle.Free();
                m_pinned = false;
            }
        }

        public byte[] ReadBytes( int length ) {
            var r = new byte[length];
            Marshal.Copy( (IntPtr)m_current, r, 0, length ); 
            m_current += length;
            return r;
        }

        public int Read( byte[] buffer, int index, int count ) {
            Marshal.Copy( (IntPtr)m_current, buffer, 0, count ); 
            m_current += count;
            return count;
        }

        public bool ReadBoolean() {
            var r = Marshal.ReadByte( (IntPtr)m_current ) != 0 ? true : false;
            ++m_current;
            return r;
        }

        public byte ReadByte() {
            var r = Marshal.ReadByte( (IntPtr)m_current );
            ++m_current;
            return r;
        }

        public sbyte ReadSByte() {
            var r = (sbyte)Marshal.ReadByte( (IntPtr)m_current );
            ++m_current;
            return r;
        }

        public char ReadChar() {
            var r = (char)Marshal.ReadInt16( (IntPtr)m_current );
            m_current += 2;
            return r;
        }

        public double ReadDouble() {
            var _r = Marshal.ReadInt64( (IntPtr)m_current );
            double* p = (double*)&_r;
            m_current += 8;
            return *p;
        }

        public short ReadInt16() {
            var r = Marshal.ReadInt16( (IntPtr)m_current );
            m_current += 2;
            return r;
        }

        public int ReadInt32() {
            var r = Marshal.ReadInt32( (IntPtr)m_current );
            m_current += 4;
            return r;
        }

        public long ReadInt64() {
            var r = Marshal.ReadInt64( (IntPtr)m_current );
            m_current += 8;
            return r;
        }

        public float ReadSingle() {
            var _r = Marshal.ReadInt32( (IntPtr)m_current );
            float* p = (float*)&_r;
            m_current += 4;
            return *p;
        }

        public ushort ReadUInt16() {
            var r = (ushort)Marshal.ReadInt16( (IntPtr)m_current );
            m_current += 2;
            return r;
        }

        public uint ReadUInt32() {
            var r = (uint)Marshal.ReadInt32( (IntPtr)m_current );
            m_current += 4;
            return r;
        }

        public ulong ReadUInt64() {
            var r = (ulong)Marshal.ReadInt64( (IntPtr)m_current );
            m_current += 8;
            return r;
        }
    }
}
