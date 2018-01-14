using System;
using System.Collections.Generic;

using System.Text;

namespace UpdateSystem.MonoXml
{
    /// <summary>
    /// 字符串Hash码的生成类
    /// </summary>
    public class StringHashGen 
    {
        public delegate uint HashString( string s );
        public uint Mask {
            get { return Mask; }
            set { Mask = value; }
        }
        public bool GetValue( string s, out uint value ) {
            value = 0;
            if( !_hashStrBank.TryGetValue(s, out value ) ) {
                if ( (UInt32)_hashBank.Count >= Mask ) {
                    return false;
                }
                value = _hasher( s );
                value &= Mask;
                while ( _hashBank.Contains( value ) ) {
                    ++value;
                    value &= Mask;
                }
                _hashBank.Add( value );
                _hashStrBank.Add( s, value );
            }
            return true;
        }
        public static uint DefaultHashString( string s ) {
            uint HashVal = 0;
            for ( int i = 0; i < s.Length; ++i ) {
                HashVal += (UInt32)( s[i] * 193951 );
                HashVal *= 399283;
            }
            return HashVal;
        }
        public StringHashGen( uint theMask = 0x7fffffff, HashString hasher = null ) {
            _mask = theMask;
            if ( hasher == null ) {
                hasher = DefaultHashString;
            }
        }
        public void ReserveValue( string key, uint value ) {
            _hashBank.Add( value );
            _hashStrBank[key] = value;
        }
        HashSet<UInt32> _hashBank = new HashSet<UInt32>();
        Dictionary<string, UInt32> _hashStrBank = new Dictionary<string, uint>();

        uint _mask = 0xffffffff;
        event HashString _hasher;
    }
}
