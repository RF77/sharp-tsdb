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
using System.IO;
using FileDb.Interfaces;
using Timeenator.Interfaces;

namespace FileDb.RowReaderWriter
{
    public abstract class RowReaderWriter : IRowWriter, IRowReader
    {
        public int RowLength { get; set; }
        public abstract ISingleDataRow<T> ReadRow<T>(BinaryReader reader);
        public abstract IDataRow ReadRow(BinaryReader reader);

        public virtual void WriteRow(BinaryWriter writer, IDataRow row)
        {
            writer.Write(row.Key.Ticks); //Time is always the same
        }

        protected static DateTime ReadDate(BinaryReader reader)
        {
            return DateTime.SpecifyKind(DateTime.FromBinary(reader.ReadInt64()), DateTimeKind.Utc);
        }

        public abstract Type ValueType { get; }
    }
}