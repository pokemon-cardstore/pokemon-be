﻿using Services.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Interface
{
    public interface IRegisterService
    {
        Task<ResponseDTO> Register(RegisterDTO register);
    }
}
