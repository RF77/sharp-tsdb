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
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace FileDb.RowReaderWriter
{
    public class LongRowReaderWriter : RowReaderWriter
    {
        public LongRowReaderWriter()
        {
            RowLength = 16;
        }

        public override void WriteRow(BinaryWriter writer, IDataRow row)
        {
            base.WriteRow(writer, row);
            writer.Write(Convert.ToInt64(row.Value));
        }

        public override Type ValueType => typeof(long);

        public override ISingleDataRow<T> ReadRow<T>(BinaryReader reader)
        {
            var row = new SingleDataRow<T>(ReadDate(reader), (T) Convert.ChangeType(reader.ReadInt64(), typeof (T)));
            return row;
        }

        public override IObjectSingleDataRow ReadRow(BinaryReader reader)
        {
            return new DataRow {Key = ReadDate(reader), Value = reader.ReadInt64()};
        }
    }
}