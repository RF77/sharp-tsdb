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

namespace Timeenator.Impl.Grouping.Configurators
{
    public class GroupSelector<T> : IGroupSelector<T>, ITimeRangeSelector<T> where T : struct
    {
        private readonly IQuerySerie<T> _serie;

        public GroupSelector(IQuerySerie<T> serie)
        {
            _serie = serie;
        }

        public IGroupByTriggerConfigurator<T> ByTrigger => new GroupByTriggerConfigurator<T>(_serie);
        public IGroupByStartEndTimesConfigurator<T> ByTimeRanges => new GroupByStartEndTimesConfigurator<T>(_serie);
        public IGroupByTimeConfigurator<T> ByTime => new GroupByTimeConfigurator<T>(_serie);

        IGroupByStartEndTimesConfiguratorOptional<T> IGroupSelector<T>.ByTrigger(Func<ISingleDataRow<T>, bool> predicate)
        {
            return ByTrigger.TriggerWhen(predicate);
        }

        IGroupByStartEndTimesConfiguratorOptional<T> IGroupSelector<T>.ByTimeRanges(
            IReadOnlyList<StartEndTime> groupTimes)
        {
            return ByTimeRanges.ByTimeRanges(groupTimes);
        }

        IGroupByStartEndTimesConfiguratorOptional<T> ITimeRangeSelector<T>.ByTrigger(
            Func<ISingleDataRow<T>, bool> predicate)
        {
            return ByTrigger.TriggerWhen(predicate);
        }
    }
}