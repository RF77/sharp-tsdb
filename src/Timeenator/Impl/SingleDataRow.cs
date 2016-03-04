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
using System.Diagnostics;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    [DebuggerDisplay("{Key}: {Value}")]
    public class SingleDataRow<T> : DataRow, ISingleDataRow<T>
    {
        public SingleDataRow(DateTime time, T value)
        {
            Key = time;
            Value = value;
        }

        public DateTime TimeUtc
        {
            get { return Key; }
            set { Key = value; }
        }

        public new T Value
        {
            get { return (T) base.Value; }
            set { base.Value = value; }
        }

        public object[] ToArray()
        {
            return new object[] {Key.ToFileTimeUtc(), Value};
        }
    }
}