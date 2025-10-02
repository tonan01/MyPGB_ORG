using PGB.BuildingBlocks.Application.Queries;
using PGB.Todo.Application.Dtos;
using System;
using System.Collections.Generic;

namespace PGB.Todo.Application.Queries
{
    public class GetTodoItemsQuery : BaseQuery<IEnumerable<TodoItemDto>>
    {
        // UserId đã có sẵn trong BaseQuery, controller sẽ gán vào
    }
}