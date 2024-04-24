namespace pnyx.net.impl;

public enum TrimStyleEnum
{
    /// <summary> Leave strings as-is </summary>
    None,
    
    /// <summary> Removes white-space from the beginning and ending of a string </summary>
    Trim,
    
    /// <summary> Removes white-space, like Trim, and then replaces empty strings with NULL </summary>
    TrimToNull
}