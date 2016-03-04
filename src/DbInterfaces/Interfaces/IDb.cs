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
using Timeenator.Interfaces;

namespace DbInterfaces.Interfaces
{
    public interface IDb
    {
        IDbMetadata Metadata { get; }
        string Name { get; }
        string MeasurementDirectory { get; }
        void SaveMetadata();
        void CreateMeasurement(IMeasurementMetadata metadata);
        IMeasurement CreateMeasurement(string name, Type valueType);
        IMeasurement GetMeasurement(string name);
        IQuerySerie<T> GetSerie<T>(string measurementName, string timeExpression) where T : struct;
        IQuerySerie<T> GetSerie<T>(string measurementName) where T : struct;
        IQueryTable<T> GetTable<T>(string measurementRegex, string timeExpression) where T : struct;
        IObjectQueryTable GetTable(string measurementRegex, string timeExpression);
        IReadOnlyList<string> GetMeasurementNames();
        void DeleteMeasurement(string name);
        void DeleteAllMeasurements();
        IMeasurement GetOrCreateMeasurement(string name, string type = "float");
    }
}