// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using DbInterfaces.Interfaces;
using Newtonsoft.Json;

namespace FileDb.Impl
{
    [DataContract]
    public class DbMetadata : IDbMetadata
    {
        public DbMetadata()
        {
            Measurements = new Dictionary<string, Measurement>();
        }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string DbPath { get; set; }

        public string DbMetadataPath => GetMetadataPath(DbPath);

        Dictionary<string, IMeasurement> IDbMetadata.Measurements
        {
            get { return Measurements.ToDictionary(i => i.Key, i => (IMeasurement)i.Value); }
        }
            
        public IMeasurement GetMeasurement(string name)
        {
            return MeasurementsWithAliases[name];
        }

        public void SetMeasurement(string name, IMeasurement measurement)
        {
            Measurements[name] = (Measurement) measurement;
            foreach (var alias in measurement.NameAndAliases)
            {
                MeasurementsWithAliases[alias] = measurement;
            }
        }

        [DataMember]
        public Dictionary<string, Measurement> Measurements { get; private set; }

        [JsonIgnore]
        public Dictionary<string, IMeasurement> MeasurementsWithAliases { get; private set; } = new Dictionary<string, IMeasurement>();

        public static string GetMetadataPath(string dbPath)
        {
            return Path.Combine(dbPath, "Metadata.json");
        }

        [OnDeserialized]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            MeasurementsWithAliases =new Dictionary<string, IMeasurement>();
            foreach (var meas in Measurements.Values)
            {
                MeasurementsWithAliases[meas.Metadata.Name] = meas;
                foreach (var alias in meas.Metadata.Aliases)
                {
                    MeasurementsWithAliases[alias] = meas;
                }
            }
        }
    }
}