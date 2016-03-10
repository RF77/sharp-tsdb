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
    public class DataRow : IObjectSingleDataRow, IEquatable<DataRow>
    {
        public DateTime Key { get; set; }
        public object Value { get; set; }

        public bool Equals(DataRow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key.Equals(other.Key) && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DataRow) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Key.GetHashCode()*397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DataRow left, DataRow right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataRow left, DataRow right)
        {
            return !Equals(left, right);
        }

        public DateTime TimeUtc
        {
            get { return Key; }
            set { Key = value; }
        }
        public object[] ToArray()
        {
            return new [] { Key.ToFileTimeUtc(), Value };
        }
    }
}