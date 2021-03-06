﻿using SchedulePath.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulePath.Services
{
    public interface ILinkService
    {
        LinkWithActivity GetLink();
        void AddLink(Link request);
        void UpdateLink(Link request);
        void DeleteLink(int id);
    }
}
