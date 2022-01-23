using System;

namespace HRMS.Utilities.General;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class DescriptionAttribute : Attribute
{
    private readonly string developer;
    private readonly string description;

    public DescriptionAttribute(string developer, string description)
    {
        this.developer = developer;
        this.description = description;
    }

    public virtual string Developer { get { return developer; } }
    public virtual string Description { get { return description; } }
}
