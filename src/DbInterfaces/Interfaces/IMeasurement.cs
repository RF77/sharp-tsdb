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
    public interface IMeasurement
    {
        DateTime? FirstValueTimeUtc { get; }
        long Size { get; }
        string Name { get; }
        long NumberOfItems { get; }
        IMeasurementMetadata Metadata { get; }
        Type ValueType { get; }

        string BinaryFilePath { get; }
        void AppendDataPoints(IEnumerable<IDataRow> row);
        IQuerySerie<T> GetDataPoints<T>(DateTime? from = null, DateTime? to = null) where T : struct;
        IQuerySerie<T> GetDataPoints<T>(string timeExpression) where T : struct;
        ISingleDataRow<T> CurrentValue<T>() where T : struct;
        void ClearDataPoints(DateTime? after = null);
        IEnumerable<string> NameAndAliases { get; }
        void MergeDataPoints(IEnumerable<IDataRow> rows, Func<IEnumerable<IDataRow>, IEnumerable<IDataRow>> mergeFunc);
    }
}