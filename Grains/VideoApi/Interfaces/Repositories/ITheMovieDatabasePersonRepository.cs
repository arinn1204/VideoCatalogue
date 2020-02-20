﻿using Grains.VideoApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Grains.VideoApi.Interfaces
{
    internal interface ITheMovieDatabasePersonRepository
    {
        Task<PersonDetail> GetPersonDetail(int personId);
    }
}
