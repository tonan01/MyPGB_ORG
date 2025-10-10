using PGB.BuildingBlocks.Application.Commands;
using System;

namespace PGB.Auth.Application.Commands
{
    public class UpdateMyProfileCommand : BaseCommand
    {
        #region Properties
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty; 
        #endregion
    }
}