namespace VoiceAttack.Proxy.Variables;

public class VoiceAttackVariables
{
    private readonly dynamic _proxy;

    private List<(string name, string value)> _setVariables;

    public IReadOnlyList<(string name, string value)> SetVariables => _setVariables.AsReadOnly();

    internal VoiceAttackVariables(dynamic vaProxy)
    {
        _proxy = vaProxy;
        _setVariables = new List<(string, string)>();
    }
    
    public void ClearStartingWith(string name)
    {
        _setVariables = _setVariables.Where(x => !x.name.Split(':')[1].StartsWith(name)).ToList();
    }


    /// <summary>
    /// Set a variable
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The value of the variable</param>
    public void Set<T>(string name, T value) where T : struct => Set(name, value,Type.GetTypeCode(typeof(T)));
    
    private void Set(string name, object value, TypeCode code)
    {
        
        switch (code)
        {
            case TypeCode.Boolean:
                SetBoolean(name, bool.Parse(value.ToString()));
                break;

            case TypeCode.DateTime:
                SetDate(name, DateTime.Parse(value.ToString().Trim('"')));
                break;

            case TypeCode.Single:
            case TypeCode.Decimal:
            case TypeCode.Double:
                SetDecimal(name, decimal.Parse(value.ToString()));
                break;

            case TypeCode.Char:
            case TypeCode.String:
                SetText(name, value.ToString().Trim('"'));
                break;

            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.SByte:
                SetShort(name, short.Parse(value.ToString()));
                break;

            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
                try
                {
                    SetInt(name, int.Parse(value.ToString()));
                }
                catch (OverflowException)
                {
                    SetDecimal(name, decimal.Parse(value.ToString()));
                } 
                break;
        }
    }
    
    /// <summary>
    /// Get a variable
    /// </summary>
    /// <typeparam name="T">The type of variable</typeparam>
    /// <param name="name">The name of the variable</param>
    public T? Get<T>(string name)
    {
        var code = Type.GetTypeCode(typeof(T));

        switch (code)
        {
            case TypeCode.Boolean:
                return (T) Convert.ChangeType(GetBoolean(name), typeof(T));

            case TypeCode.DateTime:
                return (T) Convert.ChangeType(GetDate(name), typeof(T));

            case TypeCode.Single:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int64:
            case TypeCode.UInt64:
                return (T) Convert.ChangeType(GetDecimal(name), typeof(T));

            case TypeCode.Char:
            case TypeCode.String:
                return (T) Convert.ChangeType(GetText(name), typeof(T));

            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.SByte:
                return (T) Convert.ChangeType(GetShort(name), typeof(T));

            case TypeCode.Int32:
            case TypeCode.UInt32:
                return (T) Convert.ChangeType(GetInt(name), typeof(T));

            default:
                return default;
        }
    }

    private short? GetShort(string name)
    {
        return _proxy.GetSmallInt(name);
    }

    private int? GetInt(string name)
    {
        return _proxy.GetInt(name);
    }

    private string GetText(string name)
    {
        return _proxy.GetText(name);
    }

    private decimal? GetDecimal(string name)
    {
        return _proxy.GetDecimal(name);
    }

    private bool? GetBoolean(string name)
    {
        return _proxy.GetBoolean(name);
    }

    private DateTime? GetDate(string name)
    {
        return _proxy.GetDate(name);
    }

    private void SetShort(string name, short? value)
    {
        var variable = $"{{SHORT:{name}}}";
        SetVariable(variable, value.ToString());

        _proxy.SetSmallInt(name, value);
    }

    private void SetInt(string name, int? value)
    {
        var variable = $"{{INT:{name}}}";
        SetVariable(variable, value.ToString());

        _proxy.SetInt(name, value);
    }

    private void SetText(string name, string value)
    {
        var variable = $"{{TXT:{name}}}";
        SetVariable(variable, value ?? "");

        _proxy.SetText(name, value);
    }

    private void SetDecimal(string name, decimal? value)
    {
        var variable = $"{{DEC:{name}}}";
        SetVariable(variable, value.ToString());

        _proxy.SetDecimal(name, value);
    }

    private void SetBoolean(string name, bool? value)
    {
        var variable = $"{{BOOL:{name}}}";
        SetVariable(variable, value.ToString());

        _proxy.SetBoolean(name, value);
    }

    private void SetDate(string name, DateTime? value)
    {
        var variable = $"{{DATE:{name}}}";
        SetVariable(variable, value.ToString());

        _proxy.SetDate(name, value);
    }

    private void SetVariable(string name, string value)
    {
        // Newest entries are at the bottom
        var index = _setVariables.FindIndex(x => x.name == name);
        if (index >= 0)
        {
            _setVariables.RemoveAt(index);
        }

        _setVariables.Insert(0, (name, value));
    }
}