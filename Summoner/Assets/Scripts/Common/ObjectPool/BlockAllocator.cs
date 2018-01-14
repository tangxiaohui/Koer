using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class BlockAllocator {

        const int DefaultMaxBlockNum = 100;

        public delegate void SweepAction();
        public SweepAction sweeper {
            get;
            set;
        }
        public bool extendAble {
            get {
                return extendAble;
            }
            set {
                extendAble = value;
            }
        }

        public void TrimExcess()
        {
            if ( m_freeBlocks.Count > m_initMaxBlockNum ) {
                m_freeBlocks.RemoveRange( m_initMaxBlockNum, m_freeBlocks.Count - m_initMaxBlockNum );
                m_freeBlocks.TrimExcess();
            }
        }

        public void Sweep() {
            if ( sweeper == null ) {
                TrimExcess();
            } else {
                sweeper();
            }
        }

        public BlockAllocator( int blockSize, int maxBlock, bool extendAble ) {
            m_blockSize = blockSize;
            if ( maxBlock <= 0 ) {
                maxBlock = DefaultMaxBlockNum;
            }
            m_freeBlocks = new List<byte[]>( maxBlock );
            m_maxBlockNum = m_freeBlocks.Capacity;
            m_initMaxBlockNum = m_maxBlockNum;
            m_extendAble = extendAble;
            for ( int i = 0; i < m_maxBlockNum; ++i ) {
                m_freeBlocks.Add( new byte[blockSize] );
            }
        }

        public void Clear() {
            m_freeBlocks.Clear();
        }

        public void DeepClear() {
            m_freeBlocks.Clear();
            m_freeBlocks.TrimExcess();
        }

        public int allocatedCount {
            get {
                return m_maxBlockNum - m_freeBlocks.Count;
            }
        }

        public int freeCount {
            get {
                return m_freeBlocks.Count;
            }
        }

        public Byte[] Allocate() {
            if ( m_freeBlocks.Count == 0 ) {
                if ( m_extendAble ) {
                    m_freeBlocks.Add( new byte[m_blockSize] );
                    m_maxBlockNum = m_freeBlocks.Capacity;
                } else {
                    return null;
                }
            }
            var last = m_freeBlocks.Count - 1;
            var ret = m_freeBlocks[last];
            m_freeBlocks.RemoveAt( last );
            return ret;
        }

        public bool Free( byte[] p ) {
            if ( p.Length == m_blockSize ) {
                m_freeBlocks.Add( p );
                return true;
            }
            return false;
        }
        
        public int GetTotalSize() {
            return m_freeBlocks.Capacity * m_blockSize;
        }

        int m_blockSize = 0;
        int m_maxBlockNum = 0;
        bool m_extendAble = false;
        int m_initMaxBlockNum = 0;
        List<byte[]> m_freeBlocks = null;
    }

    public interface IObjectPool {
        void Sweep();
    }

    public class BlockPool : IObjectPool {

        public class Settings {
            public int blockSize;
            public int maxBlockNum;
            public bool extendAble;
            public Settings( int s, int n, bool e ) {
                blockSize = s;
                maxBlockNum = n;
                extendAble = e;
            }
        }
        static BlockPool m_sharedInstance = null;
        static Dictionary<int, Settings> DefaultSettings = new Dictionary<int, Settings>();
        static BlockPool() {
            DefaultSettings.Add( 1 << 2, new Settings( 1 << 2, 16, true ) ); // 4
            DefaultSettings.Add( 1 << 3, new Settings( 1 << 3, 64, true ) ); // 8
            DefaultSettings.Add( 1 << 4, new Settings( 1 << 4, 64, true ) ); // 16
            DefaultSettings.Add( 1 << 5, new Settings( 1 << 5, 64, true ) ); // 32
            DefaultSettings.Add( 1 << 6, new Settings( 1 << 6, 64, true ) ); // 64
            DefaultSettings.Add( 1 << 7, new Settings( 1 << 7, 32, true ) ); // 128
            DefaultSettings.Add( 1 << 8, new Settings( 1 << 8, 16, true ) ); // 256

            m_sharedInstance = new BlockPool();
        }

        public static BlockPool sharedInstance {
            get {
                return m_sharedInstance;
            }
        }

        BlockAllocator CreateAllocator( int size ) {
            size = AlignToBlock( size );
            Settings s = null;
            if ( DefaultSettings.TryGetValue( size, out s ) ) {
                return new BlockAllocator( s.blockSize, s.maxBlockNum, s.extendAble ); 
            } else {
                return new BlockAllocator( size, 8, true ); 
            }
        }

        public static int AlignToBlock( int size ) {
            size = ( ( size + 3 ) & ~3 ); // 4 bytes aligned
            if ( ( size & ( size - 1 ) ) != 0 ) {
                size = NextPowerOf2( size );
            }
            return size;
        }

        public static int NextPowerOf2( int value ) {
            int rval = 1;
            while ( rval < value ) {
                rval <<= 1;
            }
            return rval;
	    }

        public static void MemSet( byte[] array, byte value ) {
            if ( array == null ) {
                throw new ArgumentNullException( "array" );
            }

            int block = 32, index = 0;
            int length = Math.Min( block, array.Length );

            //Fill the initial array
            while ( index < length ) {
                array[index++] = value;
            }

            length = array.Length;
            while ( index < length ) {
                Buffer.BlockCopy( array, 0, array, index, Math.Min( block, length - index ) );
                index += block;
                block *= 2;
            }
        }

        Dictionary<int, BlockAllocator> m_bPool = new Dictionary<int, BlockAllocator>();

        public byte[] Allocate( int size ) {
            size = AlignToBlock( size );
            BlockAllocator allocator = null;
            if ( !m_bPool.TryGetValue( size, out allocator ) ) {
                allocator = CreateAllocator( size );
                m_bPool.Add( size, allocator );
            }
            return allocator.Allocate();
        }

        public bool Free( ref byte[] p ) {
            if ( p != null ) {
                BlockAllocator allocator = null;
                try {
                    if ( m_bPool.TryGetValue( p.Length, out allocator ) ) {
                        return allocator.Free( p );
                    }
                } finally {
                    p = null;
                }
            }
            return false;
        }

        public BlockAllocator TryGet( int size ) {
            size = AlignToBlock( size );
            BlockAllocator ba = null;
            m_bPool.TryGetValue( size, out ba );
            return ba;
        }

        public BlockAllocator AddPool( int size, int maxnum, bool extendAble = true ) {
            size = AlignToBlock( size );
            BlockAllocator ba = null;
            if ( !m_bPool.TryGetValue( size, out ba ) ) {
                ba = new BlockAllocator( size, maxnum, extendAble );
                m_bPool.Add( size, ba );
            }
            return ba;
        }

        public int GetTotalSize() {
            int size = 0;
            foreach ( var t in m_bPool ) {
                size += t.Value.GetTotalSize();
            }
            return size;
        }

        public BlockPool() {
        }

        public void Sweep() {
            foreach ( var t in m_bPool ) {
                t.Value.Sweep();
            }
        }
    }

    public static class SimplePoolSweeper {
        static List<Action> sweepActionList = new List<Action>();
        public static void Sweep() {
            for ( int i = 0; i < sweepActionList.Count; ++i ) {
                sweepActionList[i]();
            }
        }
        public static void Register( Action sweepAction ) {
            sweepActionList.Add( sweepAction );
        }
    }

    public class SimplePool<T> where T : class {

        public static Func<T> CreateInstance = null;
        public static Action<T> CtorFunc = null;
        public static Action<T> DtorFunc = null;
        public static Action<T> ReleaseFunc = null;
        public static Func<T, int> ElementSizeFunc = null;
        static SimplePool<T> _sharedInstance = null;

        public static SimplePool<T> sharedInstance {
            get {
                return _sharedInstance;
            }
        }

        static SimplePool() {
            _sharedInstance = new SimplePool<T>();
            SimplePoolSweeper.Register( () => _sharedInstance.Sweep() );
        }

        public void Sweep() {
            try {
                if ( ReleaseFunc != null ) {
                    for ( int i = 0; i < m_stack.Count; ++i ) {
                        ReleaseFunc( m_stack[i] );
                    }
                }
            } finally {
                m_stack.Clear();
                m_stack.TrimExcess();
            }
        }

        public void TrimExcess() {
            m_stack.TrimExcess();
        }

        public T Allocate() {
            if ( m_stack.Count == 0 ) {
                T ret = null;
                if ( CreateInstance == null ) {
                    ret = Activator.CreateInstance( typeof( T ) ) as T;
                    Common.UDebug.LogWarningEx( "SimplePool: there is no public parameterless ctor. you should specify a ctor for this pool." );
                } else {
                    ret = CreateInstance();
                }
                m_stack.Add( ret );
            }
            var last = m_stack.Count - 1;
            var v = m_stack[last];
            m_stack.RemoveAt( last );
            if ( CtorFunc != null ) {
                CtorFunc( v );
            }
            return v;
        }

        public void Free( ref T o ) {
            if ( o != null ) {
                if ( DtorFunc != null ) {
                    DtorFunc( o );
                }
                m_stack.Add( o );
                o = null;
            }
        }

        public int GetTotalSize() {
            int size = 0;
            if ( ElementSizeFunc != null ) {
                for ( int i = 0; i < m_stack.Count; ++i ) {
                    size += ElementSizeFunc( m_stack[i] );
                }
            }
            return size;
        }

        public void Preload( int count ) {
            var c = m_stack.Count + count;
            if ( c > m_stack.Capacity ) {
                m_stack.Capacity = c;
            }
            if ( CreateInstance != null ) {
                for ( int i = 0; i < count; ++i ) {
                    var ret = CreateInstance();
                    if ( ret != null ) {
                        m_stack.Add( ret );
                    }
                }
            } else {
                UDebug.LogWarningEx( "SimplePool: there is no public parameterless ctor. you should specify a ctor for this pool." );
                for ( int i = 0; i < count; ++i ) {
                    var ret = Activator.CreateInstance( typeof( T ) ) as T;
                    if ( ret != null ) {
                        m_stack.Add( ret );
                    }
                }
            }
        }

        List<T> m_stack = new List<T>();
    }
}
