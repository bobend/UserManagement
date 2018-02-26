﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface IUserLookupResult<S>
    {
        TResult Accept<TResult>(IUserLookupResultVisitor<S, TResult> visitor);
    }
}
