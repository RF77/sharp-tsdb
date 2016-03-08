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

namespace DbInterfaces.Interfaces
{
    public interface IDbMetadata
    {
        string Name { get; set; }
        Guid Id { get; set; }
        string DbPath { get; set; }
        string DbMetadataPath { get; }
        Dictionary<string, IMeasurement> MeasurementsWithAliases { get; }
        Dictionary<string, IMeasurement> Measurements { get; }
        IMeasurement GetMeasurement(string name);
        void SetMeasurement(string name, IMeasurement measurement);
    }
}