using AutoMapper;
using PGB.Todo.Application.Dtos;
using PGB.Todo.Domain.Entities;

namespace PGB.Todo.Application.Mappings
{
    public class TodoMappingProfile : Profile
    {
        public TodoMappingProfile()
        {
            CreateMap<TodoItem, TodoItemDto>();
        }
    }
}