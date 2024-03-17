using CommandLine;
using CompactFolder.Cli.Operations.Models;
using System;
using System.Linq;
using System.Reflection;

namespace CompactFolder.Cli.Extensions
{
    public static class OptionExtensions
    {
        public static OptionAttribute GetAttributes(this Options option, string propertyName)
        {
            //Try to find in interfaces first
            var propertyInfo = typeof(Options).GetInterfaces()
                .SelectMany(interfaceType => interfaceType.GetProperties())
                .FirstOrDefault(prop => prop.Name == propertyName);

            //if not found, try in the basic OptionsType
            if (propertyInfo == null)
                propertyInfo = typeof(Options).GetProperty(propertyName);

            if (propertyInfo == null)
                throw new ArgumentException($"Property {propertyName} not found on type {typeof(Options).Name}");

            return propertyInfo.GetCustomAttribute<OptionAttribute>();
        }
    }
}
