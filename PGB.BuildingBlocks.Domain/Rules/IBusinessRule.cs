using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.Rules
{
    public interface IBusinessRule
    {
        #region Properties
        string Message { get; }
        bool IsBroken(); 
        #endregion
    }
}
