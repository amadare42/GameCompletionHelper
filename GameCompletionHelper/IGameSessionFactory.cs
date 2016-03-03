﻿using GameCompletionHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCompletionHelper
{
    public interface IGameSessionFactory
    {
        GameSession CreateGameSession(DateTime startTime);
    }
}