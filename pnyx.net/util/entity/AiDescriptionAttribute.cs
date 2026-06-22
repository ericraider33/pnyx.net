using System;

namespace pnyx.net.util.entity;

public class AiDescriptionAttribute : Attribute
{
    public string description { get; }

    public AiDescriptionAttribute(string description)
    {
        this.description = description;
    }
}