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
using Timeenator.Extensions.Converting;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace SharpTsdbTypes.Communication
{
    public static class ObjectArrayExtensions
    {
        public static ISingleDataRow<T> ToSingleDataRow<T>(this object[] objects) where T : struct
        {
            return new SingleDataRow<T>(DateTime.FromFileTimeUtc((long) objects[0]), objects[1].ToType<T>());
        }
    }
}