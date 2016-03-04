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

namespace Timeenator.Extensions
{
    public static class TypeExtensions
    {
        public static Type ToType(this string type)
        {
            switch (type)
            {
                case "float":
                    return typeof (float);
                case "double":
                    return typeof (double);
                case "bool":
                    return typeof (bool);
                case "long":
                    return typeof (long);
                case "int":
                    return typeof (int);
                case "short":
                    return typeof (short);
                case "byte":
                    return typeof (byte);
                case "decimal":
                    return typeof (decimal);
                case "DateTime":
                    return typeof (DateTime);
            }

            return Type.GetType(type);
        }

        public static string ToShortCode(this Type type)
        {
            if (type == typeof (float))
            {
                return "float";
            }
            if (type == typeof (double))
            {
                return "double";
            }
            if (type == typeof (bool))
            {
                return "bool";
            }
            if (type == typeof (long))
            {
                return "long";
            }
            if (type == typeof (int))
            {
                return "int";
            }
            if (type == typeof (short))
            {
                return "short";
            }
            if (type == typeof (byte))
            {
                return "byte";
            }
            if (type == typeof (decimal))
            {
                return "decimal";
            }
            if (type == typeof (DateTime))
            {
                return "DateTime";
            }

            return type.FullName;
        }
    }
}