using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client.SedRegistration
{
    /// <summary>
    /// Перечисления операций с документом
    /// </summary>
    public enum SedOperationEnum { Register = 1, Delete = 7, Refuse = 15 };
    /// <summary>
    /// Перечисления иконок в комментариях СЕДа
    /// </summary>
    public enum SedCommentIcon {NoIcon = -1, WarningIcon = 3, WhatIcon = 1, OkIcon = 2, GoldStarIcon = 0, StopIcon = 4 }
}
