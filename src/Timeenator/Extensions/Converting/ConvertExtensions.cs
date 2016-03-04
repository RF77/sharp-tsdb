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

namespace Timeenator.Extensions.Converting
{
    public static class ConvertExtensions
    {
        public static float ToFloat(this object val)
        {
            return Convert.ToSingle(val);
        }

        public static double ToDouble(this object val)
        {
            return Convert.ToDouble(val);
        }

        public static T ToType<T>(this object val) where T : struct
        {
            return (T) Convert.ChangeType(val, typeof (T));
        }
    }
}