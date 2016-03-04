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

namespace FileDb.Impl
{
    internal static class ValidTypes
    {
        public static Dictionary<Type, byte> Sizes = new Dictionary<Type, byte>();

        static ValidTypes()
        {
            Sizes[typeof (DateTime)] = 8;
            Sizes[typeof (byte)] = 1;
            Sizes[typeof (bool)] = 1;
            Sizes[typeof (short)] = 2;
            Sizes[typeof (int)] = 4;
            Sizes[typeof (long)] = 8;
            Sizes[typeof (float)] = 4;
            Sizes[typeof (double)] = 8;
            Sizes[typeof (decimal)] = 16;
        }

        public static bool IsValidType(Type type)
        {
            return Sizes.ContainsKey(type);
        }

        public static byte SizeForType(Type type)
        {
            try
            {
                return Sizes[type];
            }
            catch
            {
                throw new ArgumentException("Invalid column type!");
            }
        }
    }
}