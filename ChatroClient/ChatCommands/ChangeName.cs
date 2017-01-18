using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ChatroClient.ChatCommands
{
    [CommandArgument(Name = "Name")]
    internal class ChangeName : IChatCommand
    {
        public uint ArgumentCount { get; set; } = 1;
        public bool ServerInvoke { get; set; } = true;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class CommandArgumentAttribute : Attribute
    {
        public string Name { get; set; }
    }


    public static class AttributeHelper
    {
        public static TValue GetPropertyAttributeValue<T, TOut, TAttribute, TValue>(
            Expression<Func<T, TOut>> propertyExpression,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            MemberExpression expression = (MemberExpression) propertyExpression.Body;
            PropertyInfo propertyInfo = (PropertyInfo) expression.Member;
            TAttribute att = propertyInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            return att != null ? valueSelector(att) : default(TValue);
        }
    }
}
