using System.Security.Claims;

namespace PostsModule.Domain.Auth;

public class ClaimValueHolder
{
    public readonly string Key;
    public readonly string Value;
    public readonly string ValueType;

    private ClaimValueHolder(string keyName, string value, string valueType)
    {
        Key = keyName;
        Value = value;
        ValueType = valueType;
    }
    public static ClaimValueHolder? Create<T>(string keyName, T value) 
    {
        var valueString = value.ToString();
        if (string.IsNullOrEmpty(valueString))
            return null;

        switch (typeof(T))
        {
            case Type type when type == typeof(string):
                return new ClaimValueHolder(keyName, valueString, ClaimValueTypes.String);
            case Type type when type == typeof(int):
                return new ClaimValueHolder(keyName, valueString, ClaimValueTypes.Integer);
            default:
                throw new Exception("Type not supported");
        }
    }

}
