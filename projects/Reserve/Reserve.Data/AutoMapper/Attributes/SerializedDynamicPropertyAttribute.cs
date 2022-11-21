using System;
using AutoMapper;
using AutoMapper.Configuration;
using Reserve.Data.AutoMapper.Converters;

namespace Reserve.Data.AutoMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SerializedObjectConverterAttribute : Attribute, IMemberConfigurationProvider
    {
        public void ApplyConfiguration(IMemberConfigurationExpression expression)
        {
            expression.ConvertUsing(new SerializedObjectConverter());
        }
    }
}