using System;
using Timeenator.Impl;
using Timeenator.Impl.Converting;
using Timeenator.Interfaces;

namespace SharpTsdbTypes.Communication
{
    public static class ObjectArrayExtensions
    {
        public static ISingleDataRow<T> ToSingleDataRow<T>(this object[] objects) where T:struct 
        {
            return new SingleDataRow<T>(DateTime.FromFileTimeUtc((long)objects[0]), objects[1].ToType<T>());
        } 
    }
}