using PGB.BuildingBlocks.Domain.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.Exceptions
{
    public class BusinessRuleValidationException : Exception
    {
        #region Properties
        public IBusinessRule BrokenRule { get; }
        #endregion

        #region Constructors
        public BusinessRuleValidationException(IBusinessRule brokenRule)
            : base(brokenRule.Message)
        {
            BrokenRule = brokenRule;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"{BrokenRule.GetType().Name}: {BrokenRule.Message}";
        } 
        #endregion
    }
}
