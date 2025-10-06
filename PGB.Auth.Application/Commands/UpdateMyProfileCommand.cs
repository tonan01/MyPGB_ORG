using PGB.BuildingBlocks.Application.Commands;
using System;

namespace PGB.Auth.Application.Commands
{
    public class UpdateMyProfileCommand : BaseCommand
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}