﻿using OSK.UML.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.UML.Ports
{
    public interface IUmlParser : IDisposable
    {
        Task<IEnumerable<UmlComponent>> ParseUmlAsync(CancellationToken cancellationToken = default);
    }
}
