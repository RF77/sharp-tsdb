using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DbInterfaces.Interfaces;

namespace FileDb.Impl
{
    [DataContract]
    public class MeasurementMetadata : IMeasurementMetadata
    {
        /// <summary>
        /// Only for deserialization
        /// </summary>
        private MeasurementMetadata()
        {
            
        }
        public MeasurementMetadata(string name)
        {
            Name = name;
            Id = Guid.NewGuid().ToString().Replace("-","");
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public HashSet<string> Aliases { get; } = new HashSet<string>();

        [DataMember]
        public HashSet<string> Tags { get; } = new HashSet<string>();
        [DataMember]
        public List<Column> ColumnsInternal { get; } = new List<Column>();

        public IEnumerable<IColumn> Columns => ColumnsInternal;

        [DataMember]
        public string Id { get; set; }
    }
}