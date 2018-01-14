using System.Security;

public static class SecurityElementEx {

    public static void SetAttribute(this SecurityElement se, string name, string value) {
        if( se.Attributes == null ) {
            se.AddAttribute( name, value );
        } else {
            var hashtable = se.Attributes;
            hashtable[name] = value;
            se.Attributes = hashtable;
        }
    }
}
