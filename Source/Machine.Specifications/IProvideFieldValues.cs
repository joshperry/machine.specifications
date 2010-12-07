using System;

namespace Machine.Specifications
{
    public interface IProvideFieldValues
    {
        object GetValue(System.Reflection.FieldInfo field);
    }
}
